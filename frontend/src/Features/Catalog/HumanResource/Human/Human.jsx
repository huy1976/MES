import React, { useState, useEffect, useRef, useCallback, useMemo } from 'react';
import HumanDetail from '../HumanDetail/HumanDetail';
import { CreateNewHuman } from '../Create';
import { getPerson } from '../../../../api/Person';
import { FaSyncAlt } from 'react-icons/fa';
import './Human.css';
import { useAuthPermissions } from '../../../../api/UseAuthPermissions';

const PAGE_SIZE = 31;

const Human = () => {
  // === STATE ===
  const [humans, setHumans] = useState([]); // Dữ liệu gốc từ API
  const [loading, setLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(true);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [totalItems, setTotalItems] = useState(0);
  const [isFullyLoaded, setIsFullyLoaded] = useState(false);
  const [reloadKey, setReloadKey] = useState(0);

  // Xem chi tiết
  const [selectedHuman, setSelectedHuman] = useState(null);

  // Filter (Client-side)
  const [showFilter, setShowFilter] = useState(false);
  const [filterValues, setFilterValues] = useState({
    code: '',
    type: 'Tất cả',
    status: 'Tất cả',
  });
  // Không cần activeFilters nữa vì ta lọc trực tiếp trên client

  // Tạo mới
  const [showCreateNew, setShowCreateNew] = useState(false);

  const observer = useRef();
  const scrollContainerRef = useRef(null);

  const { permissions, loading: permLoading, error: permError } = useAuthPermissions(7);
  const canView = permissions.some((p) => p.canView);
  const canEdit = permissions.some((p) => p.canEdit);

  // === INFINITE SCROLL CALLBACK ===
  const lastHumanRef = useCallback(
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
          rootMargin: '300px', // Load trước khi scroll tới đáy 300px
          threshold: 0.1
        }
      );
      if (node) observer.current.observe(node);
    },
    [loading, initialLoading, hasMore]
  );

  // === HELPER: MÀU SẮC TRẠNG THÁI ===
  const getStatusStyle = (statusName) => {
    switch (statusName) {
      case 'Đang thực hiện nhiệm vụ': // Xanh dương (Blue)
        return { 
          backgroundColor: '#e0f2fe', // Xanh dương rất nhạt
          color: '#0369a1',           // Xanh dương đậm
          border: '1px solid #0369a1' 
        };
        
      case 'Đã được phân công': // Màu cam (Orange)
        return { 
          backgroundColor: '#ffedd5', // Cam nhạt
          color: '#c2410c',           // Cam đậm
          border: '1px solid #c2410c' 
        };

      case 'Khả dụng': // Xanh lá (Green)
        return { 
          backgroundColor: '#dcfce7', // Xanh lá nhạt
          color: '#15803d',           // Xanh lá đậm
          border: '1px solid #15803d' 
        };

      case 'Tạm nghỉ': // Đỏ (Red)
        return { 
          backgroundColor: '#fee2e2', // Đỏ nhạt
          color: '#b91c1c',           // Đỏ đậm
          border: '1px solid #b91c1c' 
        };

      default: // Mặc định xám
        return { 
          backgroundColor: '#f3f4f6', 
          color: '#374151',
          border: '1px solid #9ca3af'
        };
    }
  };

  // === CHUẨN HÓA DỮ LIỆU ===
  // === CHUẨN HÓA DỮ LIỆU ===
  const normalizeHuman = (item, index) => {
    // Lưu ý: STT này tính dựa trên dữ liệu tải về, không bị đổi khi filter
    const globalIndex = (page - 1) * PAGE_SIZE + index + 1;

    const phoneProp = item.properties?.find(p =>
      p.name === 'phone' ||
      (p.description && p.description.toLowerCase().includes('dien thoai'))
    );
    const phone = phoneProp?.valueString?.trim() || '--';

    // --- BẮT ĐẦU ĐOẠN SỬA ---
    // 1. Lấy mảng năng lực
    const capList = item.personnelCapabilities || [];
    // 2. Lấy properties từ phần tử đầu tiên của mảng (nếu có)
    const capProps = capList[0]?.properties || [];
    // --- KẾT THÚC ĐOẠN SỬA ---

    // Logic đếm kỹ năng (giữ nguyên logic của bạn: true và không phải Ca)
    const skills = capProps.filter(
      (p) => p.valueString === 'true' && !p.description?.toLowerCase().includes('ca')
    );
    const capacities = skills.length; // Số lượng kỹ năng

    // Logic tính Ca làm việc (giữ nguyên logic, giờ đã chạy đúng nhờ capProps có dữ liệu)
    const shifts = [];
    capProps.forEach((p) => {
      const desc = p.description?.toLowerCase() || '';
      if (desc.includes('ca 1') && p.valueString === 'true') shifts.push('Ca 1');
      if (desc.includes('ca 2') && p.valueString === 'true') shifts.push('Ca 2');
      if (desc.includes('ca 3') && p.valueString === 'true') shifts.push('Ca 3');
    });
    const shift = shifts.length > 0 ? shifts.join(', ') : '--';

    const statusMap = {
      3: 'Đã được phân công',
      9: 'Đang thực hiện nhiệm vụ',
      8: 'Khả dụng',
      12: 'Tạm nghỉ',
    };
    const status = statusMap[item.personStatus] || '--';

    const personnelClass = item.personnelClass?.name || '--';

    return {
      id: item.personId || `H${globalIndex.toString().padStart(3, '0')}`,
      stt: globalIndex,
      name: item.name || '--',
      code: item.personId || '--',
      type: personnelClass,
      capacities, // Giờ sẽ hiển thị đúng số lượng (ví dụ: 1, 2...)
      shift,      // Giờ sẽ hiển thị đúng ca (ví dụ: Ca 1, Ca 2)
      status,
      phone,
      raw: item,
    };
  };

  // === FETCH DATA (Luôn lấy tất cả, không truyền filter lên server) ===
  const fetchHumans = useCallback(
    async (pageNum, isAppend = false) => {
      setError(null);
      const isInitial = pageNum === 1 && !isAppend;
      if (isInitial) setInitialLoading(true);
      else setLoading(true);

      // KHÔNG truyền filter params lên server nữa để đảm bảo Client-side filter hoạt động trên dữ liệu đã tải
      const apiFilters = {
        code: null,
        type: null,
        status: null,
      };

      try {
        const data = await getPerson(pageNum, PAGE_SIZE, apiFilters);
        const newItems = data.items || [];

        const mapped = newItems.map((item, index) => normalizeHuman(item, index));

        const isTrulyFinished = newItems.length === 0 || newItems.length < PAGE_SIZE;
        setHasMore(!isTrulyFinished);
        setIsFullyLoaded(isTrulyFinished);

        setHumans(prev => (isAppend ? [...prev, ...mapped] : mapped));

        if (isInitial) {
          setTotalItems(data.totalItems || 0);
        }
      } catch (err) {
        setError('Không thể tải dữ liệu nhân sự. Vui lòng thử lại.');
        console.error('Fetch error:', err);
      } finally {
        setLoading(false);
        setInitialLoading(false);
      }
    },
    []
  );

  // === EFFECT: TẢI THÊM KHI SCROLL ===
  useEffect(() => {
    // Chỉ tải thêm khi scroll xuống, không phụ thuộc vào filterValues
    if (page > 1 && !isFullyLoaded) {
      fetchHumans(page, true);
    }
  }, [page, isFullyLoaded, fetchHumans]);

  // === EFFECT: RELOAD ===
  useEffect(() => {
    setPage(1);
    setHumans([]);
    setIsFullyLoaded(false);
    setHasMore(true);
    if (scrollContainerRef.current) {
      scrollContainerRef.current.scrollTo({ top: 0, behavior: 'auto' });
    }
    fetchHumans(1, false);
  }, [reloadKey, fetchHumans]);

  // === LOGIC FILTER CLIENT-SIDE ===
  // Lọc dữ liệu `humans` hiện có dựa trên `filterValues`
  const filteredHumans = useMemo(() => {
    return humans.filter(human => {
      // 1. Filter Code hoặc Name
      const searchText = filterValues.code.toLowerCase();
      const matchCode = searchText === '' || 
                        human.code.toLowerCase().includes(searchText) || 
                        human.name.toLowerCase().includes(searchText);

      // 2. Filter Type
      const matchType = filterValues.type === 'Tất cả' || human.type === filterValues.type;

      // 3. Filter Status
      const matchStatus = filterValues.status === 'Tất cả' || human.status === filterValues.status;

      return matchCode && matchType && matchStatus;
    });
  }, [humans, filterValues]);


  // === HANDLERS ===
  const handleReload = () => {
    setReloadKey(prev => prev + 1);
  };

  // Nút áp dụng bây giờ chỉ đơn giản là đóng panel, vì input đã filter realtime
  const applyFilter = () => {
    setShowFilter(false);
  };

  // Reset filter về mặc định
  const clearFilter = () => {
      setFilterValues({ code: '', type: 'Tất cả', status: 'Tất cả' });
  }

  const handleViewDetail = (human) => {
    setSelectedHuman(human);
  };

  const handleCreateNew = () => setShowCreateNew(true);
  const handleBack = () => {
    setShowCreateNew(false);
    setSelectedHuman(null);
    handleReload();
  };

  // === LẤY DANH SÁCH ĐỘNG CHO DROPDOWN (Dựa trên toàn bộ dữ liệu đã tải) ===
  const types = useMemo(() => ['Tất cả', ...new Set(humans.map(h => h.type))], [humans]);
  const statuses = useMemo(() => ['Tất cả', ...new Set(humans.map(h => h.status))], [humans]);

  // === SKELETON ROW ===
  const SkeletonRow = () => (
    <tr className="skeleton-row">
      {[...Array(9)].map((_, i) => (
        <td key={i}><div className="skeleton-line"></div></td>
      ))}
    </tr>
  );

  // === RENDER ===
  if (showCreateNew) {
    return (
      <div>
        <div className="detail-nav">
          <button onClick={handleBack} className="back-btn">Quay lại</button>
        </div>
        <CreateNewHuman />
      </div>
    );
  }

  if (selectedHuman) {
    return (
      <div>
        <div className="detail-nav">
          <button onClick={handleBack} className="back-btn">Quay lại</button>
        </div>
        <HumanDetail human={selectedHuman} />
      </div>
    );
  }

  return (
    <div className="human-page">
      {/* HEADER */}
      <div className="human-header">
        <div className="actions">
          <button className={`action-btn filter-btn ${showFilter ? 'active': ''}`} onClick={() => setShowFilter(!showFilter)}>
            🔍 Tìm kiếm
          </button>
          <button className={`action-btn new-btn ${!canEdit ? 'disabled-button' : ''}`} 
          onClick={handleCreateNew}
          disabled={!canEdit}
          title={!canEdit ? "Bạn không có quyền tạo mới nhân sự" : "Tạo mới nhân sự"}>
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
                <label>Mã / Tên</label>
                <input
                  type="text"
                  placeholder="Nhập mã hoặc tên..."
                  value={filterValues.code}
                  onChange={(e) => setFilterValues({ ...filterValues, code: e.target.value })}
                />
              </div>
              <div>
                <label>Lớp nhân sự</label>
                <select
                  value={filterValues.type}
                  onChange={(e) => setFilterValues({ ...filterValues, type: e.target.value })}
                >
                  {types.map((t) => (
                    <option key={t} value={t}>{t}</option>
                  ))}
                </select>
              </div>
              <div>
                <label>Trạng thái</label>
                <select
                  value={filterValues.status}
                  onChange={(e) => setFilterValues({ ...filterValues, status: e.target.value })}
                >
                  {statuses.map((s) => (
                    <option key={s} value={s}>{s}</option>
                  ))}
                </select>
              </div>
              <div className="filter-actions">
                <button onClick={clearFilter} className="clear-btn" style={{marginRight: 'auto', background: 'transparent', color: '#666', border:'none', textDecoration:'underline', cursor:'pointer'}}>Xóa lọc</button>
                <button onClick={applyFilter} className="search-btn">Đóng</button>
              </div>
            </div>
          )}
        </div>
      </div>

      {/* SUMMARY */}
      <div className="summary">
        Đang hiển thị {filteredHumans.length.toLocaleString()} kết quả (trong tổng {humans.length.toLocaleString()} đã tải).
      </div>

      {/* TABLE */}
      <div className="humans-table">
        <div className="table-container" ref={scrollContainerRef}>
          <table role="grid">
            <thead>
              <tr>
                <th>STT</th>
                <th>Họ và tên</th>
                <th>Mã nhân viên</th>
                <th>SĐT</th>
                <th>Lớp nhân sự</th>
                <th>Năng lực</th>
                <th>Ca</th>
                <th style={{textAlign: 'center'}}>Trạng thái</th>
                <th>Chi tiết</th>
              </tr>
            </thead>
            <tbody>
              {/* SKELETON */}
              {initialLoading && humans.length === 0 && [...Array(PAGE_SIZE)].map((_, i) => <SkeletonRow key={i} />)}

              {/* DATA (Render filteredHumans) */}
              {filteredHumans.map((human, index) => (
                <tr
                  key={human.id}
                  // Attach ref to last item of FILTERED list to trigger load more if user scrolls to bottom
                  ref={index === filteredHumans.length - 1 ? lastHumanRef : null}
                  className="human-row"
                >
                  <td>{human.stt}</td>
                  <td><b>{human.name}</b></td>
                  <td>{human.code}</td>
                  <td>{human.phone}</td>
                  <td>{human.type}</td>
                  <td style={{textAlign: 'center'}}>{human.capacities}</td>
                  <td>{human.shift}</td>
                  <td style={{textAlign: 'center'}}>
                    <span
                      className="status-badge-styled"
                      style={{
                        ...getStatusStyle(human.status),
                        padding: '4px 10px',
                        borderRadius: '12px',
                        fontSize: '0.85rem',
                        fontWeight: '600',
                        display: 'inline-block',
                        minWidth: '100px'
                      }}
                    >
                      {human.status}
                    </span>
                  </td>
                  <td style={{textAlign: 'center'}}>
                    <button
                      className="view-btn"
                      onClick={() => handleViewDetail(human)}
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
                  <td colSpan="9" className="loading-cell">
                    <div className="loading-spinner">
                      <div className="spinner"></div>
                      <span>Đang tải thêm dữ liệu...</span>
                    </div>
                  </td>
                </tr>
              )}

              {/* ERROR */}
              {error && !initialLoading && (
                <tr>
                  <td colSpan="9" className="error-message">
                    {error} <button onClick={handleReload}>Thử lại</button>
                  </td>
                </tr>
              )}

              {/* NO DATA FOUND (Sau khi filter) */}
              {!initialLoading && filteredHumans.length === 0 && !error && (
                <tr>
                  <td colSpan="9" className="no-data-message">
                    {humans.length === 0 
                      ? "Chưa có dữ liệu nhân sự." 
                      : "Không tìm thấy kết quả phù hợp với bộ lọc."}
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

export default Human;