import React, {
  useState,
  useEffect,
  useRef,
  useCallback,
  useMemo,
} from 'react';
import * as XLSX from 'xlsx'; // Import thư viện Excel
import './ProductionPlan.css';
import CreateProductionOrder from './CreateProductionOrder';
import { getManufacturingOrders } from '../../api/WorkOrdersPlan';
import ProductionControl from './ProductionControl/ProductionControl';
import { 
  RefreshCw, Filter, Eye, ArrowLeft, FileText, 
  Calendar, Settings, Users, Package, Download // Import thêm icon Download
} from 'lucide-react';
import{ useAuthPermissions } from '../../api/UseAuthPermissions';


const PAGE_SIZE = 31;
const TABLE_COLUMN_COUNT = 12;

const ProductionPlan = () => {
  // === STATE ===
  const [allOrders, setAllOrders] = useState([]);
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(true);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [totalItems, setTotalItems] = useState(0);
  const [isFullyLoaded, setIsFullyLoaded] = useState(false);

  // Export State
  const [isExporting, setIsExporting] = useState(false); // State loading cho nút Export

  // UI States
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [sortConfig, setSortConfig] = useState({ key: 'id', direction: 'asc' });
  const [selectedOrder, setSelectedOrder] = useState(null);
  const [showFilter, setShowFilter] = useState(false);
  const [reloadKey, setReloadKey] = useState(0);

  // Navigation States
  const [isCreatingOrder, setIsCreatingOrder] = useState(false);
  const [isProductionControl, setIsProductionControl] = useState(false);

  const { permissions, loading: permLoading, error: permError } = useAuthPermissions(3);
  const canView = permissions.some((p) => p.canView);
  const canEdit = permissions.some((p) => p.canEdit);

  const observer = useRef();

  // === DEBOUNCE SEARCH ===
  const debouncedSearchTerm = useMemo(
    () => {
      let timeout;
      return (value) => {
        clearTimeout(timeout);
        timeout = setTimeout(() => setSearchTerm(value), 400);
      };
    },
    []
  );

  // === HELPER: FORMAT DATA ===
  // Tách logic format date ra để dùng chung cho cả Table và Export
  const formatDateString = (dateString) => {
    if (!dateString || dateString.includes('0001-01-01')) return '--';
    const d = new Date(dateString);
    if (isNaN(d.getTime())) return '--';
    return `${d.getDate().toString().padStart(2, '0')}/${(d.getMonth() + 1)
      .toString()
      .padStart(2, '0')}/${d.getFullYear()}`;
  };

  const statusMap = {
    PendingScheduling: 'Đang chờ điều độ',
    Pending: 'Chờ xử lý',
    InProduction: 'Đang thực thi',
    Completed: 'Đã hoàn thành',
    Cancelled: 'Đã hủy',
    ReadyForProduction: 'Chờ sản xuất',
  };

  // === NORMALIZE DATA FOR TABLE ===
  const normalizeOrder = (item, index, currentPage) => {
    const stt = (currentPage - 1) * PAGE_SIZE + index + 1;

    return {
      id: stt,
      manufacturingOrderId: item.manufacturingOrderId,
      lsx: item.workOrderId || '--',
      ycsx: item.manufacturingOrderCode || '--',
      congDoan: item.segmentId || '--',
      sanPham: item.materialDefinitionId || '--',
      trongLuong: item.productionTarget && item.unit
        ? `${item.productionTarget} ${item.unit}`
        : '--',
      ngayBatDau: formatDateString(item.start),
      ngayKetThuc: formatDateString(item.end),
      may: Array.isArray(item.equipments) && item.equipments.length > 0
        ? item.equipments.map(eq => eq.equipmentId).join(', ')
        : '--',
      nhanSu: Array.isArray(item.persons) && item.persons.length > 0
        ? item.persons.map(p => 
            p.shift ? `${p.personId} (${p.shift})` : p.personId
          ).join(', ')
        : '--',
      thongSoSanXuat: item.productionParamId || '--',
      thongSoChatLuong: item.qualityParamId || '--',
      status: statusMap[item.status] || item.status || 'Chờ xử lý',
      rawData: item,
    };
  };

  // === FETCH DATA ===
  const fetchOrders = useCallback(async (pageNum, isAppend = false) => {
    if (loading || (isFullyLoaded && pageNum > 1)) return;

    const isInitial = pageNum === 1 && !isAppend;
    if (isInitial) setInitialLoading(true);
    else setLoading(true);

    try {
      const data = await getManufacturingOrders(pageNum, PAGE_SIZE);
      if (!Array.isArray(data)) throw new Error('Dữ liệu không hợp lệ');

      const newItems = data;
      const isLastPage = newItems.length < PAGE_SIZE;
      const mapped = newItems.map((item, idx) => normalizeOrder(item, idx, pageNum));

      setAllOrders(prev => isAppend ? [...prev, ...mapped] : mapped);
      setTotalItems(prev => isAppend ? prev + newItems.length : newItems.length);
      setIsFullyLoaded(isLastPage);
      setHasMore(!isLastPage);

    } catch (err) {
      setError('Không thể tải danh sách lệnh sản xuất.');
      console.error(err);
    } finally {
      setLoading(false);
      if (isInitial) setInitialLoading(false);
    }
  }, [loading, isFullyLoaded]);

  // === EXPORT EXCEL FUNCTION ===
  const handleExportExcel = async () => {
    try {
      setIsExporting(true);
      
      // 1. Gọi API lấy 10,000 dòng dữ liệu
      const rawData = await getManufacturingOrders(1, 10000);
      
      if (!rawData || !Array.isArray(rawData) || rawData.length === 0) {
        alert("Không có dữ liệu để xuất hoặc lỗi kết nối!");
        setIsExporting(false);
        return;
      }

      // 2. Chuẩn hóa dữ liệu cho Excel (Giống logic hiển thị bảng)
      const dataToExport = rawData.map((item, index) => {
        const mayMoc = Array.isArray(item.equipments) 
            ? item.equipments.map(eq => eq.equipmentId).join(', ') 
            : '';
            
        const nhanSu = Array.isArray(item.persons) 
            ? item.persons.map(p => p.shift ? `${p.personId} (${p.shift})` : p.personId).join(', ') 
            : '';

        const NVL = Array.isArray(item.materials)
            ? item.materials.map(m => m.materialDefinitionId ? `${m.materialDefinitionId}: ${m.quantity?.valueString || ''}(${m.valueUnitOfMeasure || ''})` : '').join('; ') 
            : '';

        return {
          "STT": index + 1,
          "Lệnh sản xuất": item.workOrderId || '--',
          "Yêu cầu SX": item.manufacturingOrderCode || '--',
          "Công đoạn": item.segmentId || '--',
          "Sản phẩm": item.materialDefinitionId || '--',
          "Trọng lượng": item.productionTarget ? `${item.productionTarget} ${item.unit || ''}` : '--',
          "Ngày bắt đầu": formatDateString(item.start),
          "Ngày kết thúc": formatDateString(item.end),
          "Thông số SX": item.productionParamId || '--',
          "Thông số CL": item.qualityParamId || '--',
          "Máy móc/Thiết bị": mayMoc,
          "Nhân sự": nhanSu,
          "Nguyên vật liệu": NVL,
        };
      });

      // 3. Tạo Workbook và Worksheet
      const worksheet = XLSX.utils.json_to_sheet(dataToExport);

      // Cấu hình độ rộng cột (Optional)
      const wscols = [
          {wch: 6},  // STT
          {wch: 15}, // LSX
          {wch: 15}, // YCSX
          {wch: 15}, // Cong doan
          {wch: 20}, // San pham
          {wch: 15}, // Trong luong
          {wch: 15}, // Ngay BD
          {wch: 15}, // Ngay KT
          {wch: 25}, // May
          {wch: 25}, // Nhan su
          {wch: 15}, // TS SX
          {wch: 15}, // TS CL
          {wch: 15}  // Trang thai
      ];
      worksheet['!cols'] = wscols;

      const workbook = XLSX.utils.book_new();
      XLSX.utils.book_append_sheet(workbook, worksheet, "DanhSachLenhSanXuat");

      // 4. Xuất file
      const fileName = `Kế hoạch sản xuất ${new Date().toISOString().slice(0,10)}.xlsx`;
      XLSX.writeFile(workbook, fileName);

    } catch (err) {
      console.error("Export Error:", err);
      alert("Có lỗi xảy ra khi xuất file Excel. Vui lòng thử lại sau.");
    } finally {
      setIsExporting(false);
    }
  };

  useEffect(() => {
    if (!isCreatingOrder && !isProductionControl) {
      setPage(1);
      setAllOrders([]);
      setHasMore(true);
      setIsFullyLoaded(false);
      fetchOrders(1, false);
    }
  }, [reloadKey, isCreatingOrder, isProductionControl]);

  useEffect(() => {
    if (page > 1 && !isFullyLoaded && !isCreatingOrder && !isProductionControl) {
      fetchOrders(page, true);
    }
  }, [page]);

  const lastOrderRef = useCallback((node) => {
    if (loading || initialLoading || isFullyLoaded || isCreatingOrder || isProductionControl) return;
    if (observer.current) observer.current.disconnect();

    observer.current = new IntersectionObserver(entries => {
      if (entries[0].isIntersecting && hasMore) {
        setPage(prev => prev + 1);
      }
    });

    if (node) observer.current.observe(node);
  }, [loading, initialLoading, hasMore, isFullyLoaded, isCreatingOrder, isProductionControl]);

  const filteredAndSortedOrders = useMemo(() => {
    let result = [...allOrders];

    if (searchTerm) {
      const lower = searchTerm.toLowerCase().trim();
      result = result.filter(o =>
        o.sanPham.toLowerCase().includes(lower) ||
        o.ycsx.toLowerCase().includes(lower) ||
        o.lsx.toLowerCase().includes(lower) ||
        o.congDoan.toLowerCase().includes(lower)
      );
    }

    if (statusFilter) {
      result = result.filter(o => o.status === statusFilter);
    }

    result.sort((a, b) => {
      let aVal = a[sortConfig.key];
      let bVal = b[sortConfig.key];

      if (sortConfig.key === 'ngayBatDau' || sortConfig.key === 'ngayKetThuc') {
        const parse = (d) => d === '--' ? new Date(0) : new Date(d.split('/').reverse().join('-'));
        aVal = parse(aVal);
        bVal = parse(bVal);
      }

      if (aVal < bVal) return sortConfig.direction === 'asc' ? -1 : 1;
      if (aVal > bVal) return sortConfig.direction === 'asc' ? 1 : -1;
      return 0;
    });

    return result;
  }, [allOrders, searchTerm, statusFilter, sortConfig]);

  useEffect(() => {
    setOrders(filteredAndSortedOrders);
  }, [filteredAndSortedOrders]);

  const handleSort = (key) => {
    setSortConfig(prev => ({
      key,
      direction: prev.key === key && prev.direction === 'asc' ? 'desc' : 'asc',
    }));
  };

  const handleReload = () => setReloadKey(prev => prev + 1);
  const toggleFilter = () => setShowFilter(prev => !prev);
  const handleResetFilter = () => {
    setSearchTerm('');
    setStatusFilter('');
  };

  const statuses = useMemo(() => [...new Set(allOrders.map(o => o.status))].sort(), [allOrders]);

  const statusColors = {
    'Chờ xử lý': '#94a3b8',
    'Đang chờ điều độ': '#a855f7',
    'Đang thực thi': '#3b82f6',
    'Đã hoàn thành': '#22c55e',
    'Đã hủy': '#ef4444',
    'Chờ sản xuất': '#eab308',
  };

  const getStatusBadge = (status) => {
     return (
        <span className="status-badge" style={{ 
            backgroundColor: `${statusColors[status]}20` || '#f1f5f9',
            color: statusColors[status] || '#64748b'
        }}>
            {status}
        </span>
     )
  };

  const handleViewDetail = (order) => setSelectedOrder(order);
  const handleBack = () => setSelectedOrder(null);

  const SkeletonRow = React.memo(() => (
  <tr className="skeleton-row">
    {[...Array(TABLE_COLUMN_COUNT)].map((_, i) => (
      <td key={i}><div className="skeleton-line"></div></td>
    ))}
  </tr>
));


  // === RENDER DETAIL VIEW ===
  const renderDetailView = () => {
    if (!selectedOrder) return null;
    const { rawData } = selectedOrder;
    const { persons, materials, equipments } = rawData;

    return (
      <div className="detail-view">
        <div className="detail-top-bar">
          <button onClick={handleBack} className="back-btn">
            <ArrowLeft size={18} /> Quay lại danh sách
          </button>
          <div className="detail-main-status">
            <span className="detail-code">{selectedOrder.lsx}</span>
            {getStatusBadge(selectedOrder.status)}
          </div>
        </div>

        <div className="detail-grid-layout">
          {/* Cột trái: Thông tin chung */}
          <div className="detail-left-col">
            <div className="detail-card">
              <div className="card-header"><FileText size={18}/> Thông tin sản xuất</div>
              <div className="info-row">
                <label>Yêu cầu SX:</label> <span>{selectedOrder.ycsx}</span>
              </div>
              <div className="info-row">
                <label>Công đoạn:</label> <span>{selectedOrder.congDoan}</span>
              </div>
              <div className="info-row">
                <label>Sản phẩm:</label> <span>{selectedOrder.sanPham}</span>
              </div>
              <div className="info-row">
                <label>Mục tiêu:</label> <span>{selectedOrder.trongLuong}</span>
              </div>
            </div>

            <div className="detail-card">
               <div className="card-header"><Calendar size={18}/> Thời gian & Thông số</div>
               <div className="info-row">
                <label>Bắt đầu:</label> <span>{selectedOrder.ngayBatDau}</span>
              </div>
              <div className="info-row">
                <label>Kết thúc:</label> <span>{selectedOrder.ngayKetThuc}</span>
              </div>
              <div className="info-row">
                <label>TS Sản xuất:</label> <span>{selectedOrder.thongSoSanXuat}</span>
              </div>
               <div className="info-row">
                <label>TS Chất lượng:</label> <span>{selectedOrder.thongSoChatLuong}</span>
              </div>
            </div>
          </div>

          {/* Cột phải: Tài nguyên chi tiết */}
          <div className="detail-right-col">
            
            {/* 1. Thiết bị */}
            <div className="detail-section">
              <h4 className="section-title"><Settings size={16}/> Thiết bị (Equipments)</h4>
              {equipments && equipments.length > 0 ? (
                <div className="resource-grid">
                  {equipments.map((eq, idx) => (
                    <div key={idx} className="resource-item equipment">
                      <div className="res-icon">M</div>
                      <div className="res-info">
                        <div className="res-id">{eq.equipmentId}</div>
                        <div className="res-desc">{eq.className || 'Machine'}</div>
                      </div>
                    </div>
                  ))}
                </div>
              ) : <div className="empty-data">Chưa gán thiết bị</div>}
            </div>

            {/* 2. Nhân sự */}
            <div className="detail-section">
              <h4 className="section-title"><Users size={16}/> Nhân sự (Persons)</h4>
              {persons && persons.length > 0 ? (
                <div className="resource-grid">
                  {persons.map((p, idx) => (
                    <div key={idx} className="resource-item person">
                      <div className="res-icon">P</div>
                      <div className="res-info">
                        <div className="res-id">{p.personId}</div>
                        <div className="res-desc">{p.shift ? `Ca: ${p.shift}` : 'Chưa phân ca'}</div>
                      </div>
                    </div>
                  ))}
                </div>
              ) : <div className="empty-data">Chưa phân công nhân sự</div>}
            </div>

             {/* 3. Vật tư */}
             <div className="detail-section">
              <h4 className="section-title"><Package size={16}/> Nguyên vật liệu (Materials)</h4>
              {materials && materials.length > 0 ? (
                <div className="material-table-wrapper">
                  <table className="detail-mini-table">
                    <thead>
                      <tr>
                        <th>Mã NVL</th>
                        <th>Loại</th>
                        <th className="text-right">Số lượng</th>
                      </tr>
                    </thead>
                    <tbody>
                      {materials.map((m, idx) => (
                        <tr key={idx}>
                          <td className="font-medium">{m.materialDefinitionId}</td>
                          <td>{m.className}</td>
                          <td className="text-right">
                            {m.quantity?.valueString} <span className="unit">{m.valueUnitOfMeasure}</span>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              ) : <div className="empty-data">Không có thông tin NVL</div>}
            </div>
          </div>
        </div>
      </div>
    );
  };

  if (isCreatingOrder) {
    return (
      <div className="production-plan-container">
        <h2 className="title">Kế hoạch sản xuất | Tạo lệnh sản xuất</h2>
        <CreateProductionOrder onBack={() => setIsCreatingOrder(false)} />
      </div>
    );
  }

  if (isProductionControl) {
    return (
      <div className="production-plan-container">
        <ProductionControl onBack={() => setIsProductionControl(false)} />
      </div>
    );
  }

  if (selectedOrder) {
    return (
      <div className="production-plan-container">
        {renderDetailView()}
      </div>
    );
  }

  return (
    <div className="production-plan-container">
      <h2 className="title">Kế hoạch sản xuất | Các lệnh sản xuất</h2>
      <div className="header">
        <div className="actions">
          {/* Nút Xuất Excel */}
          <button 
            className="action-btn export-btn" 
            onClick={handleExportExcel}
            disabled={isExporting} // Disable khi đang tải
            style={{ minWidth: '110px' }} // Đảm bảo độ rộng không bị giật khi text thay đổi
          >
             {isExporting ? (
                 <>
                   <RefreshCw size={14} className="spinning" /> Đang xuất...
                 </>
             ) : (
                 <>
                   <Download size={14} /> Xuất Excel
                 </>
             )}
          </button>

          <button className="action-btn filter-btn" onClick={toggleFilter}>
            <Filter size={14} /> Tìm kiếm
          </button>
          <button
            className="action-btn reload-btn"
            onClick={handleReload}
            disabled={loading || initialLoading}
          >
            <RefreshCw size={14} className={loading || initialLoading ? 'spinning' : ''} /> Tải lại
          </button>
          {showFilter && (
            <div className="filter-panel active">
              <div>
                <label>Tìm lệnh sản xuất / Yêu cầu sản xuất / Sản phẩm</label>
                <input
                  type="text"
                  placeholder="Nhập từ khóa..."
                  defaultValue={searchTerm}
                  onChange={(e) => debouncedSearchTerm(e.target.value)}
                />
              </div>
              <div>
                <label>Trạng thái</label>
                <select value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)}>
                  <option value="">Tất cả trạng thái</option>
                  {statuses.map(s => <option key={s} value={s}>{s}</option>)}
                </select>
              </div>
              <div className="filter-actions">
                <button className="reset-btn" onClick={handleResetFilter}>Đặt lại</button>
                <button className="close-btn" onClick={toggleFilter}>Đóng</button>
              </div>
            </div>
          )}
        </div>
      </div>
      <div className="table-container">
        <table role="grid">
          <thead>
            <tr>
              <th onClick={() => handleSort('id')}>STT {sortConfig.key === 'id' && (sortConfig.direction === 'asc' ? '↑' : '↓')}</th>
              <th>Lệnh sản xuất</th>
              <th>Yêu cầu sản xuất</th>
              <th>Công đoạn</th>
              <th>Sản phẩm</th>
              <th>Trọng lượng</th>
              <th onClick={() => handleSort('ngayBatDau')}>Ngày bắt đầu {sortConfig.key === 'ngayBatDau' && (sortConfig.direction === 'asc' ? '↑' : '↓')}</th>
              <th onClick={() => handleSort('ngayKetThuc')}>Ngày kết thúc {sortConfig.key === 'ngayKetThuc' && (sortConfig.direction === 'asc' ? '↑' : '↓')}</th>
              <th>Thông số SX</th>
              <th>Tiêu chuẩn CL</th>
              <th>Trạng thái</th>
              <th>Xem</th>
            </tr>
          </thead>
          <tbody>
            {initialLoading && [...Array(10)].map((_, i) => <SkeletonRow key={i} />)}
            {orders.map((order, idx) => (
              <tr
                key={order.id}
                ref={idx === orders.length - 1 && hasMore ? lastOrderRef : null}
              >
                <td>{order.id}</td>
                <td><strong>{order.lsx}</strong></td>
                <td>{order.ycsx}</td>
                <td>{order.congDoan}</td>
                <td>{order.sanPham}</td>
                <td>{order.trongLuong}</td>
                <td>{order.ngayBatDau}</td>
                <td>{order.ngayKetThuc}</td>
                <td>{order.thongSoSanXuat}</td>
                <td>{order.thongSoChatLuong}</td>
                <td>
                  {getStatusBadge(order.status)}
                </td>
                <td>
                  <button className="view-btn" onClick={() => handleViewDetail(order)}>
                    <Eye size={18} />
                  </button>
                </td>
              </tr>
            ))}
            {loading && !initialLoading && (
              <tr><td colSpan={TABLE_COLUMN_COUNT} className="loading-cell">Đang tải thêm...</td></tr>
            )}
            {isFullyLoaded && orders.length > 0 && (
              <tr>
                <td colSpan={TABLE_COLUMN_COUNT} className="end-message">
                  Đã hiển thị {orders.length.toLocaleString()} / {totalItems.toLocaleString()} lệnh
                </td>
              </tr>
            )}
            {error && (
              <tr>
                <td colSpan={TABLE_COLUMN_COUNT} className="error-message">
                  {error}
                  <button onClick={handleReload} className="retry-btn">Thử lại</button>
                </td>
              </tr>
            )}
            {!initialLoading && !loading && orders.length === 0 && !error && (
              <tr><td colSpan={TABLE_COLUMN_COUNT} className="no-data-message">Không có dữ liệu</td></tr>
            )}
          </tbody>
        </table>
      </div>
      <div className="footer-actions">
        <button
          className={`footer-btn create-btn ${!canEdit ? 'disabled-button' : ''}`}
          onClick={() => setIsCreatingOrder(true)}
          disabled={!canEdit}
          title={!canEdit ? "Bạn không có quyền tạo lệnh sản xuất" : "Tạo lệnh sản xuất mới"}
        >
          Tạo lệnh sản xuất
        </button>
        <button
          className={`footer-btn schedule-btn ${!canEdit ? 'disabled-button' : ''}`}
          onClick={() => setIsProductionControl(true)}
          disabled={!canEdit}
          title={!canEdit ? "Bạn không có quyền điều độ sản xuất" : "Điều độ sản xuất"}
        >
          Điều độ sản xuất
        </button>
      </div>
    </div>
  );
};

export default ProductionPlan;