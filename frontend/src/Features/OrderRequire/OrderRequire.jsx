import React, { useState, useEffect, useRef, useCallback, useMemo } from 'react';
import OrderRequireDetail from './OrderRequireDetail';
import CreateOrderRequire from './CreateOrderRequire';
import { getManufacturingOrders } from '../../api/ManufacturingOrders';
import './OrderRequire.css';
import { FaEye, FaSyncAlt, FaPlus, FaFilter } from 'react-icons/fa';
import {useAuthPermissions} from '../../api/UseAuthPermissions';
const PAGE_SIZE = 30;

// === 1. DANH SÁCH TRẠNG THÁI (TIẾNG VIỆT) ===
const STATUS_OPTIONS = [
  'Chờ xử lý',
  'Đang chờ điều độ',
  'Đang thực thi',
  'Chờ sản xuất',
  'Đã hoàn thành',
  'Đã hủy',
];

// === DEBOUNCE HOOK ===
function useDebounce(value, delay) {
  const [debouncedValue, setDebouncedValue] = useState(value);
  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);
    return () => {
      clearTimeout(handler);
    };
  }, [value, delay]);
  return debouncedValue;
}

// === SKELETON COMPONENT ===
const SkeletonRow = React.memo(() => (
  <tr className="skeleton-row">
    {[...Array(12)].map((_, i) => (
      <td key={i}>
        <div className="skeleton-line"></div>
      </td>
    ))}
  </tr>
));

// === MAIN COMPONENT ===
const OrderRequire = () => {
  // === STATE ===
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(true);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [totalItems, setTotalItems] = useState(0);

  // UI States
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState(''); // State lưu trạng thái lọc Client
  const [sortConfig, setSortConfig] = useState({ key: 'id', direction: 'asc' });
  const [selectedOrder, setSelectedOrder] = useState(null);
  const [isCreating, setIsCreating] = useState(false);
  const [showFilter, setShowFilter] = useState(false);
  
  const [reloadKey, setReloadKey] = useState(0);

  // Chỉ debounce search term (vì search gọi API), status filter xử lý tức thì ở Client
  const debouncedSearchTerm = useDebounce(searchTerm, 400);

  // Lấy Permissions
  const { permissions, loading: permLoading, error: permError } = useAuthPermissions(2);
  const canView = permissions.some((p) => p.canView);
  const canEdit = permissions.some((p) => p.canEdit);

  const observer = useRef();

  // === HELPER ===
  const formatDate = (dateString) => {
    if (!dateString) return 'Chưa xác định';
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN');
  };

  const mapStatus = (status) => {
    const map = {
      PendingScheduling: 'Đang chờ điều độ',
      Scheduled: 'Đang điều độ',
      InProduction: 'Đang thực thi',
      ReadyForProduction: 'Chờ sản xuất',
      Completed: 'Đã hoàn thành',
      Cancelled: 'Đã hủy',
      Pending: 'Chờ xử lý'
    };
    return map[status] || status;
  };
  
  const statusColors = {
    'Chờ xử lý': '#95a5a6',
    'Đang chờ điều độ': '#C11DEB',
    'Đang thực thi': '#3498db',
    'Chờ sản xuất': '#f39c12',
    'Đã hoàn thành': '#2ecc71',
    'Đã hủy': '#e74c3c',
  };

  // === FETCH DATA (SERVER-SIDE) ===
  const fetchOrders = useCallback(async (pageNum, isAppend = false) => {
    if (loading && pageNum > 1) return;
    if (!hasMore && isAppend) return;

    setError(null);
    const isInitial = pageNum === 1 && !isAppend;
    if (isInitial) setInitialLoading(true);
    else setLoading(true);

    try {
      // 1. CHỈNH SỬA: KHÔNG GỬI PARAM STATUS LÊN SERVER NỮA
      const params = {
        page: pageNum,
        pageSize: PAGE_SIZE,
        searchTerm: debouncedSearchTerm,
        // status: ... (Đã xóa dòng này để không lọc ở server)
        sortKey: sortConfig.key,
        sortDirection: sortConfig.direction,
      };
      
      const response = await getManufacturingOrders(params); 
      const newItems = response.items || [];

      const mapped = newItems.map((item, index) => {
        const stt = (pageNum - 1) * PAGE_SIZE + index + 1;
        return {
          id: stt,
          uniqueId: item.productionRequestId || item.manufacturingOrderId || `temp-${stt}`, 
          code: item.productionRequestId || item.manufacturingOrderId || 'YCSX-TEMP',
          productCode: item.materialDefinitionId || 'N/A',
          productName: item.materialDefinitionName || 'Không tên',
          quantity: item.quantity ? `${item.quantity} ${item.unit || 'Kg'}` : 'N/A',
          deliveryDate: formatDate(item.dueDate),
          company: item.customerId || 'Không xác định',
          department: item.operationsDefinitionId || 'Chưa có',
          unit: item.qualityStandardId || 'N/A',
          status: mapStatus(item.status), // Data đã được map sang Tiếng Việt ở đây
          createdDate: formatDate(item.createDate),
          createdBy: item.createdBy || 'Hệ thống',
          rawData: item,
        };
      });

      setOrders((prev) => (isAppend ? [...prev, ...mapped] : mapped));
      setTotalItems(response.totalItems || 0);
      
      const loadedCount = (pageNum - 1) * PAGE_SIZE + newItems.length;
      const newHasMore = loadedCount < (response.totalItems || 0);
      setHasMore(newHasMore);

    } catch (err) {
      setError('Không thể tải yêu cầu sản xuất. Vui lòng thử lại.');
      console.error(err);
    } finally {
      setLoading(false);
      setInitialLoading(false);
    }
  }, [loading, hasMore, debouncedSearchTerm, sortConfig]); // Xóa statusFilter khỏi dependencies

  // === EFFECTS ===
  useEffect(() => {
    setPage(1);
    setHasMore(true);
    setOrders([]);
    fetchOrders(1, false);
  }, [debouncedSearchTerm, sortConfig, reloadKey]); // Xóa statusFilter khỏi dependencies

  useEffect(() => {
    if (page > 1) {
      fetchOrders(page, true);
    }
  }, [page]);

  // === 2. CLIENT-SIDE FILTERING LOGIC (LỌC TRÊN DỮ LIỆU ĐÃ TẢI) ===
  const filteredOrders = useMemo(() => {
    if (!statusFilter) return orders; // Nếu không chọn gì, trả về tất cả
    return orders.filter(order => order.status === statusFilter); // So sánh chuỗi Tiếng Việt
  }, [orders, statusFilter]);

  // === INFINITE SCROLL ===
  const lastOrderRef = useCallback(
    (node) => {
      if (loading || initialLoading) return;
      if (observer.current) observer.current.disconnect();

      observer.current = new IntersectionObserver((entries) => {
        if (entries[0].isIntersecting && hasMore && !loading) {
          setPage((prev) => prev + 1);
        }
      });

      if (node) observer.current.observe(node);
    },
    [loading, initialLoading, hasMore]
  );

  // === RELOAD ===
  const handleReload = () => {
    setReloadKey((prev) => prev + 1);
  };
  
  // === HANDLERS ===
  const toggleFilter = () => {
    setShowFilter(prev => !prev);
  };

  const handleResetFilter = () => {
    setSearchTerm('');
    setStatusFilter('');
  };

  const handleSort = (key) => {
    setSortConfig((prev) => ({
      key,
      direction: prev.key === key && prev.direction === 'asc' ? 'desc' : 'asc',
    }));
  };
  
  const handleViewDetail = (order) => {
    setSelectedOrder(order);
    setIsCreating(false);
  };
  const handleBack = () => {
    setSelectedOrder(null);
    setIsCreating(false);
  };
  const handleCreateNew = () => {
    setIsCreating(true);
    setSelectedOrder(null);
  };
  const handleCancelCreate = () => {
    setIsCreating(false);
    handleReload();
  };
  const handleSubmitCreate = () => {
    setIsCreating(false);
  };

  // === RENDER ===
  if (isCreating) {
    return <CreateOrderRequire onCancel={handleCancelCreate} onSubmit={handleSubmitCreate} />;
  }

  if (selectedOrder) {
    return (
      <div>
        <div className="detail-nav">
          <button onClick={handleBack} className="back-btn">← Quay lại</button>
        </div>
        <OrderRequireDetail order={selectedOrder} />
      </div>
    );
  }

  return (
    <div className="order-require-container">
      <h2 className="title">Yêu cầu sản xuất</h2>

      <div className="header">
        <div className="actions">
          <button 
            className="action-btn filter-btn" 
            onClick={toggleFilter}
            aria-label="Tìm kiếm yêu cầu sản xuất"
          >
            <FaFilter /> Tìm kiếm
          </button>

          <button className={`action-btn new-btn ${!canEdit ? 'disabled-button' : ''}`}
          onClick={handleCreateNew}
          disabled={!canEdit}
          title={!canEdit ? "Bạn không có quyền tạo mới" : "Tạo mới yêu cầu sản xuất"}
          >
            <FaPlus /> Tạo mới
          </button>

          <button className="action-btn reload-btn" onClick={handleReload} disabled={loading || initialLoading}>
            <FaSyncAlt className={loading || initialLoading ? 'spinning' : ''} /> Tải lại
          </button>

          {/* FILTER PANEL */}
          {showFilter && (
            <div className="filter-panel active">
              <div>
                <label>Tìm YCSX / Tên SP</label>
                <input
                  type="text"
                  placeholder="Tìm YCSX, tên SP..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                />
              </div>
              <div>
                <label>Trạng thái</label>
                <select value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)}>
                  <option value="">Tất cả trạng thái</option>
                  {STATUS_OPTIONS.map((s) => (
                    <option key={s} value={s}>{s}</option>
                  ))}
                </select>
              </div>
              <div className="filter-actions">
                <button className="reset-btn" onClick={handleResetFilter}>
                  Đặt lại
                </button>
                <button className="close-btn" onClick={toggleFilter}>
                  Đóng
                </button>
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
              <th onClick={() => handleSort('code')}>Mã YCSX {sortConfig.key === 'code' && (sortConfig.direction === 'asc' ? '↑' : '↓')}</th>
              <th>Mã SP</th>
              <th>Tên sản phẩm</th>
              <th onClick={() => handleSort('quantity')}>Số lượng {sortConfig.key === 'quantity' && (sortConfig.direction === 'asc' ? '↑' : '↓')}</th>
              <th onClick={() => handleSort('createdDate')}>Ngày tạo {sortConfig.key === 'createdDate' && (sortConfig.direction === 'asc' ? '↑' : '↓')}</th>
              <th onClick={() => handleSort('deliveryDate')}>Hạn giao {sortConfig.key === 'deliveryDate' && (sortConfig.direction === 'asc' ? '↑' : '↓')}</th>
              <th>Khách hàng</th>
              <th>Công thức</th>
              <th>Tiêu chuẩn</th>
              <th>Trạng thái</th>
              <th>Chi tiết</th>
            </tr>
          </thead>
          <tbody>
            {initialLoading && [...Array(10)].map((_, i) => <SkeletonRow key={i} />)}

            {/* SỬ DỤNG filteredOrders THAY VÌ orders ĐỂ HIỂN THỊ */}
            {!initialLoading && filteredOrders.map((order, index) => (
              <tr
                key={order.uniqueId} 
                // Gắn ref vào phần tử cuối cùng của danh sách ĐÃ LỌC
                // Điều này giúp Infinite Scroll vẫn hoạt động (nếu lọc xong mà danh sách ngắn quá, nó có thể tự load thêm trang sau)
                ref={index === filteredOrders.length - 1 ? lastOrderRef : null}
              >
                <td>{order.id}</td>
                <td><strong>{order.code}</strong></td>
                <td>{order.productCode}</td>
                <td>{order.productName}</td>
                <td>{order.quantity}</td>
                <td>{order.createdDate}</td>
                <td>{order.deliveryDate}</td>
                <td>{order.company}</td>
                <td>{order.department}</td>
                <td>{order.unit}</td>
                <td>
                  <span className="status" style={{ backgroundColor: statusColors[order.status] || '#95a5a6' }}>
                    {order.status}
                  </span>
                </td>
                
                <td>
                  <button 
                    className="view-btn" 
                    onClick={() => handleViewDetail(order)}
                    aria-label={`Xem chi tiết ${order.code}`}
                  >
                    <FaEye />
                  </button>
                </td>
              </tr>
            ))}

            {loading && !initialLoading && (
              <tr><td colSpan="12" className="loading-cell">Đang tải thêm...</td></tr>
            )}

            {!hasMore && filteredOrders.length > 0 && (
              <tr>
                <td colSpan="12" className="end-message">
                  {/* Cập nhật thông báo đếm số lượng */}
                  Hiển thị {filteredOrders.length} kết quả (Đã tải {orders.length} / {totalItems} từ hệ thống)
                </td>
              </tr>
            )}

            {error && (
              <tr>
                <td colSpan="12" className="error-message">
                  {error}
                  <button onClick={handleReload} className="retry-btn">Thử lại</button>
                </td>
              </tr>
            )}

            {!initialLoading && !loading && filteredOrders.length === 0 && !error && (
              <tr>
                <td colSpan="12" className="no-data-message">
                  Không tìm thấy yêu cầu sản xuất nào.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default OrderRequire;