import React, { useState, useEffect, useRef, useCallback, useMemo } from 'react';
import EquipmentDetail from '../Detail/EquipmentDetail';
import { CreateNewEquipments } from '../Create';
import { getEquipments,  updateEquipmentStatus} from '../../../../api/Equipments';
import { FaSyncAlt } from 'react-icons/fa';
import './Equipments.css';
import { useAuthPermissions } from '../../../../api/UseAuthPermissions';
const PAGE_SIZE = 31;

const Equipments = () => {
  // === STATE ===
  const [equipments, setEquipments] = useState([]); // Dữ liệu gốc
  const [loading, setLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(true);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [totalItems, setTotalItems] = useState(0);
  const [isFullyLoaded, setIsFullyLoaded] = useState(false);
  const [reloadKey, setReloadKey] = useState(0);

  // Xem chi tiết
  const [selectedEquipment, setSelectedEquipment] = useState(null);

  // Filter Client-side
  const [showFilter, setShowFilter] = useState(false);
  const [filterValues, setFilterValues] = useState({ code: '', type: 'Tất cả' });
  // Không cần activeFilters nữa

  // Tạo mới
  const [showCreateNew, setShowCreateNew] = useState(false);

  const observer = useRef();
  const scrollContainerRef = useRef(null);

  const { permissions, loading: permLoading, error: permError } = useAuthPermissions(7);
  const canView = permissions.some((p) => p.canView);
  const canEdit = permissions.some((p) => p.canEdit);

  // === INFINITE SCROLL CALLBACK ===
  const lastEquipmentRef = useCallback(
    (node) => {
      if (loading || initialLoading) return;
      if (observer.current) observer.current.disconnect();
      observer.current = new IntersectionObserver(
        (entries) => {
          if (entries[0].isIntersecting && hasMore && !loading) {
            setPage((prev) => prev + 1);
          }
        },
        {
          root: scrollContainerRef.current,
          rootMargin: '300px',
          threshold: 0.1
        }
      );
      if (node) observer.current.observe(node);
    },
    [loading, initialLoading, hasMore]
  );

// === HELPER: MÀU SẮC TRẠNG THÁI (Đã tối ưu độ tương phản) ===
const getStatusStyle = (statusName) => {
  switch (statusName) {
    case 'Đang chạy': // 🟢 Xanh lá (Tươi hơn)
      return {
        backgroundColor: '#DCFCE7',
        color: '#15803D',
        border: '1px solid #22C55E'
      };

    case 'Khả dụng': // 🔵 Xanh dương (Sâu hơn)
      return {
        backgroundColor: '#DBEAFE',
        color: '#1E40AF',
        border: '1px solid #3B82F6'
      };

    case 'Đã được phân công': // 🟠 Cam (Tách biệt hẳn với Vàng)
      return {
        backgroundColor: '#FFEDD5',
        color: '#9A3412',
        border: '1px solid #F97316'
      };

    case 'Bị sự cố': // 🔴 Đỏ (Gắt để cảnh báo)
      return {
        backgroundColor: '#FEE2E2',
        color: '#991B1B',
        border: '2px solid #EF4444' // Viền dày hơn để gây chú ý
      };

    case 'Đang bảo trì': // 🟡 Vàng (Sáng rõ)
      return {
        backgroundColor: '#FEF9C3',
        color: '#854D0E',
        border: '1px solid #EAB308'
      };

    default: // ⚪ Xám (Trung tính)
      return {
        backgroundColor: '#F3F4F6',
        color: '#4B5563',
        border: '1px solid #9CA3AF'
      };
  }
};



  // === FETCH DATA ===
  const fetchEquipments = useCallback(
    async (pageNum, isAppend = false) => {
      setError(null);
      const isInitial = pageNum === 1 && !isAppend;
      if (isInitial) setInitialLoading(true);
      else setLoading(true);

      // Filter Client-side nên không truyền params filter lên API
      const apiFilters = {
        code: null,
        type: null
      };

      try {
        // Cập nhật trạng thái thiết bị trước khi lấy dữ liệu
        await updateEquipmentStatus();
        const data = await getEquipments(pageNum, PAGE_SIZE, apiFilters);
        const newItems = data.items || [];
        const mapped = newItems.map((item, index) => {
          // Tính STT dựa trên trang hiện tại
          const stt = (pageNum - 1) * PAGE_SIZE + index + 1;
          const modelProp = item.properties?.find(p => p.name?.toLowerCase() === 'model');
          
          let eqStatus = '';
          switch (item.equipmentStatus) {
            case 8 : eqStatus = 'Khả dụng'; break;
            case 9: eqStatus = 'Đang chạy'; break;
            case 10: eqStatus = 'Bị sự cố'; break;
            case 11: eqStatus = 'Đang bảo trì'; break;
            case 3: eqStatus = 'Đã được phân công'; break;
            default: eqStatus = 'Không xác định'; break;
          }

          return {
            id: item.equipmentId || `temp-${stt}`,
            stt,
            type: item.equipmentClass?.name || 'Không xác định',
            code: item.equipmentId || '--',
            name: item.name || '--',
            model: modelProp?.valueString || '--',
            manufacturer: '--', // API chưa có thông tin này?
            status: eqStatus,
            location: '--',
            workCenter: item.absolutePath || '--',
            attributes: item.properties?.length || 0,
            capacities: item.equipmentCapabilities?.length || 0,
            rawData: item,
          };
        });

        const isTrulyFinished = newItems.length === 0 || newItems.length < PAGE_SIZE;
        setHasMore(!isTrulyFinished);
        setIsFullyLoaded(isTrulyFinished);

        setEquipments(prev => (isAppend ? [...prev, ...mapped] : mapped));

        if (isInitial) {
          setTotalItems(data.totalItems || 0);
        }
      } catch (err) {
        setError('Không thể tải dữ liệu thiết bị. Vui lòng thử lại.');
        console.error('Fetch error:', err);
      } finally {
        setLoading(false);
        setInitialLoading(false);
      }
    },
    []
  );

  // === EFFECT: TẢI THÊM ===
  useEffect(() => {
    if (page > 1 && !isFullyLoaded) {
      fetchEquipments(page, true);
    }
  }, [page, isFullyLoaded, fetchEquipments]);

  // === EFFECT: RELOAD ===
  useEffect(() => {
    setPage(1);
    setEquipments([]);
    setIsFullyLoaded(false);
    setHasMore(true);
    if (scrollContainerRef.current) {
      scrollContainerRef.current.scrollTo({ top: 0, behavior: 'auto' });
    }
    fetchEquipments(1, false);
  }, [reloadKey, fetchEquipments]);

  // === LOGIC FILTER CLIENT-SIDE ===
  const filteredEquipments = useMemo(() => {
    return equipments.filter(item => {
      // 1. Filter Code hoặc Name
      const searchText = filterValues.code.toLowerCase();
      const matchCode = searchText === '' || 
                        item.code.toLowerCase().includes(searchText) || 
                        item.name.toLowerCase().includes(searchText);

      // 2. Filter Type
      const matchType = filterValues.type === 'Tất cả' || item.type === filterValues.type;

      return matchCode && matchType;
    });
  }, [equipments, filterValues]);

  // === LẤY DANH SÁCH LOẠI THIẾT BỊ TỰ ĐỘNG ===
  const uniqueTypes = useMemo(() => {
      return ['Tất cả', ...new Set(equipments.map(e => e.type))];
  }, [equipments]);

  // === HANDLERS ===
  const handleReload = () => {
    setReloadKey(prev => prev + 1);
  };

  const clearFilter = () => {
      setFilterValues({ code: '', type: 'Tất cả' });
  };

  const applyFilter = () => {
    setShowFilter(false);
  };

  const handleViewDetail = (equipment) => {
    setSelectedEquipment(equipment);
  };

  const handleBackToList = () => {
    setSelectedEquipment(null);
  };

  const handleCreateNew = () => setShowCreateNew(true);
  const handleBack = () => {
    setShowCreateNew(false);
    handleReload();
  };

  // === SKELETON ROW ===
  const SkeletonRow = () => (
    <tr className="skeleton-row">
      {[...Array(11)].map((_, i) => (
        <td key={i}><div className="skeleton-line"></div></td>
      ))}
    </tr>
  );

  // === RENDER CHI TIẾT ===
  if (selectedEquipment) {
    return (
      <div>
        <div className="detail-nav">
          <button onClick={handleBackToList} className="back-btn">Quay lại danh sách</button>
        </div>
        <EquipmentDetail equipment={selectedEquipment} />
      </div>
    );
  }

  // === RENDER TẠO MỚI ===
  if (showCreateNew) {
    return (
      <div>
        <div className="detail-nav">
          <button onClick={handleBack} className="back-btn">Quay lại</button>
        </div>
        <CreateNewEquipments />
      </div>
    );
  }

  // === RENDER DANH SÁCH ===
  return (
    <div className="equipments-page">
      {/* HEADER */}
      <div className="equipments-header">
        <div className="actions">
          <button className={`action-btn filter-btn ${showFilter ? 'active' : ''}`} onClick={() => setShowFilter(!showFilter)}>
            🔍 Tìm kiếm
          </button>
          <button className={`action-btn new-btn ${!canEdit ? 'disabled-button' : ''}`}
          disabled={!canEdit}
          title={!canEdit ? "Bạn không có quyền tạo mới thiết bị" : "Tạo mới thiết bị"}
          onClick={handleCreateNew}>
            ➕ Tạo mới
          </button>
          <button
             className="action-btn reload-btn"
             onClick={handleReload}
             disabled={loading || initialLoading}
          >
             <FaSyncAlt className={loading || initialLoading ? 'spinning' : ''} />
             Tải lại
          </button>

          {/* FILTER PANEL */}
          {showFilter && (
            <div className="filter-panel active">
              <div>
                <label>Mã / Tên thiết bị</label>
                <input
                  type="text"
                  placeholder="Nhập mã hoặc tên..."
                  value={filterValues.code}
                  onChange={(e) => setFilterValues({ ...filterValues, code: e.target.value })}
                />
              </div>
              <div>
                <label>Loại thiết bị</label>
                <select
                  value={filterValues.type}
                  onChange={(e) => setFilterValues({ ...filterValues, type: e.target.value })}
                >
                    {/* Render options động dựa trên dữ liệu */}
                    {uniqueTypes.map(t => (
                        <option key={t} value={t}>{t}</option>
                    ))}
                </select>
              </div>
              <div className="filter-actions">
                <button onClick={clearFilter} className="clear-btn">Xóa lọc</button>
                <button onClick={applyFilter} className="search-btn">Đóng</button>
              </div>
            </div>
          )}
        </div>
      </div>

      {/* SUMMARY */}
      <div className="summary">
        Đang hiển thị {filteredEquipments.length.toLocaleString()} kết quả (trong tổng {equipments.length.toLocaleString()} đã tải).
      </div>

      {/* TABLE */}
      <div className="equipments-table">
        <div className="table-container" ref={scrollContainerRef}>
          <table role="grid">
            <thead>
              <tr>
                <th>STT</th>
                <th>Loại thiết bị</th>
                <th>Mã thiết bị</th>
                <th>Tên thiết bị</th>
                <th>Model</th>
                <th>NSX</th>
                <th style={{textAlign: 'center'}}>Trạng thái</th>
                <th>Vị trí</th>
                <th style={{textAlign: 'center'}}>Thuộc tính</th>
                <th style={{textAlign: 'center'}}>Năng lực</th>
                <th style={{textAlign: 'center'}}>Chi tiết</th>
              </tr>
            </thead>
            <tbody>
              {/* SKELETON */}
              {initialLoading && equipments.length === 0 && [...Array(PAGE_SIZE)].map((_, i) => <SkeletonRow key={i} />)}

              {/* DATA (Render filteredEquipments) */}
              {filteredEquipments.map((eq, index) => (
                <tr
                  key={eq.id}
                  ref={index === filteredEquipments.length - 1 ? lastEquipmentRef : null}
                  className="equipment-row"
                >
                  <td>{eq.stt}</td>
                  <td>{eq.type}</td>
                  <td>{eq.code}</td>
                  <td><b>{eq.name}</b></td>
                  <td>{eq.model}</td>
                  <td>{eq.manufacturer}</td>
                  <td style={{textAlign: 'center'}}>
                    <span 
                        className="status-badge-styled"
                        style={{
                            ...getStatusStyle(eq.status),
                            padding: '4px 10px',
                            borderRadius: '12px',
                            fontSize: '0.85rem',
                            fontWeight: '600'
                        }}
                    >
                        {eq.status}
                    </span>
                  </td>
                  <td>{eq.workCenter}</td>
                  <td style={{textAlign: 'center'}}>{eq.attributes}</td>
                  <td style={{textAlign: 'center'}}>{eq.capacities}</td>
                  <td style={{textAlign: 'center'}}>
                    <button
                      className="view-btn"
                      onClick={() => handleViewDetail(eq)}
                      title="Xem chi tiết"
                    >
                      <i className="fas fa-eye"></i>
                    </button>
                  </td>
                </tr>
              ))}

              {/* LOADING MORE */}
              {loading && !initialLoading && (
                <tr>
                  <td colSpan="11" className="loading-cell">
                    <div className="loading-spinner">
                      <div className="spinner"></div>
                      <span>Đang tải thêm...</span>
                    </div>
                  </td>
                </tr>
              )}

              {/* ERROR */}
              {error && !initialLoading && (
                <tr>
                  <td colSpan="11" className="error-message">
                    {error} <button onClick={handleReload}>Thử lại</button>
                  </td>
                </tr>
              )}

              {/* NO DATA */}
              {!initialLoading && filteredEquipments.length === 0 && !error && (
                <tr>
                  <td colSpan="11" className="no-data-message">
                    {equipments.length === 0 
                        ? "Chưa có dữ liệu thiết bị." 
                        : "Không tìm thấy thiết bị nào phù hợp với bộ lọc."}
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};

export default Equipments;