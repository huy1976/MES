import React, { useState, useEffect, useRef, useCallback, useMemo } from 'react';
import { CreateNewHumanCapacity } from '../Create';
import { getPersonelCapability } from '../../../../api/PersonCapability';
import { getPerson } from '../../../../api/Person';
import { FaSyncAlt } from 'react-icons/fa';
import './HumanCapacity.css';
import { useAuthPermissions } from '../../../../api/UseAuthPermissions';

const PAGE_SIZE = 31;

const HumanCapacity = () => {
  // === STATE ===
  const [capacities, setCapacities] = useState([]);
  const [personsMap, setPersonsMap] = useState({});
  const [loading, setLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(true);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [totalItems, setTotalItems] = useState(0);
  const [isFullyLoaded, setIsFullyLoaded] = useState(false);
  const [reloadKey, setReloadKey] = useState(0);

  // Xem chi tiết
  const [selectedCapacityId, setSelectedCapacityId] = useState(null);

  const { permissions, loading: permLoading, error: permError } = useAuthPermissions(7);
  const canView = permissions.some((p) => p.canView);
  const canEdit = permissions.some((p) => p.canEdit);

  // Filter
  const [showFilter, setShowFilter] = useState(false);
  const [filterValues, setFilterValues] = useState({
    code: '',
    employeeName: '',
    startDate: '',
    endDate: '',
  });
  const [activeFilters, setActiveFilters] = useState(filterValues);

  // Tạo mới
  const [showCreateNew, setShowCreateNew] = useState(false);

  const observer = useRef();
  const scrollContainerRef = useRef(null);

  // === TẢI NHÂN VIÊN (MAP) ===
  useEffect(() => {
    const fetchPersons = async () => {
      try {
        const data = await getPerson(1, 200); // Đủ để map
        const map = {};
        data.items.forEach(p => {
          if (p.personId && p.name) map[p.personId] = p.name;
        });
        setPersonsMap(map);
      } catch (err) {
        console.error('Lỗi tải nhân viên:', err);
      }
    };
    fetchPersons();
  }, []);

  // === INFINITE SCROLL CALLBACK ===
  const lastCapacityRef = useCallback(
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

  // === CHUẨN HÓA DỮ LIỆU ===
  const normalizeCapacity = (item, index) => {
    const stt = (page - 1) * PAGE_SIZE + index + 1;
    const capProps = item.properties || [];

    const personId = item.person?.personId || item.personId;
    const employeeName = item.personName || '--';

    const code = item.personnelCapabilityId || '--';
    const quantity = item.quantity?.valueString || '1';
    const unit = item.valueUnitOfMeasure || 'Người';
    const startDate = item.validFrom?.split('T')[0] || '--';
    const endDate = item.validTo && item.validTo !== '0001-01-01T00:00:00' ? item.validTo.split('T')[0] : '--';

    const details = [];

    // === CA LÀM ===
    const shifts = [
      { name: 'shift1', label: 'Ca 1', desc: 'Làm việc được trong ca 1' },
      { name: 'shift2', label: 'Ca 2', desc: 'Làm việc được trong ca 2' },
      { name: 'shift3', label: 'Ca 3', desc: 'Làm việc được trong ca 3' },
    ];

    shifts.forEach(shift => {
      const prop = capProps.find(p => p.name === shift.name);
      if (prop && prop.valueString === 'true') {
        details.push({
          id: details.length + 1,
          name: shift.label,
          value: 'Có',
          unit: '--',
          description: shift.desc,
          type: 'shift'
        });
      }
    });

    // === KỸ NĂNG ===
    const skills = [
      { name: 'draw', label: 'Vận hành máy kéo', desc: 'Khả năng vận hành máy kéo' },
      { name: 'enamel', label: 'Vận hành máy tráng', desc: 'Khả năng vận hành máy tráng' },
      { name: 'drawenamel', label: 'Máy kéo-tráng liên tục', desc: 'Khả năng vận hành máy kéo tráng liên tục' },
    ];

    skills.forEach(skill => {
      const prop = capProps.find(p => p.name === skill.name);
      if (prop && prop.valueString === 'true') {
        details.push({
          id: details.length + 1,
          name: skill.label,
          value: 'Có',
          unit: '--',
          description: skill.desc,
          type: 'skill'
        });
      }
    });

    return {
      id: item.personnelCapabilityId || `CAP${stt.toString().padStart(3, '0')}`,
      stt,
      code,
      employeeName,
      quantity,
      unit,
      startDate,
      endDate,
      details,
      rawData: item,
    };
  };

  // === FETCH DATA (SERVER-SIDE FILTER) ===
  const fetchCapacities = useCallback(
    async (pageNum, isAppend = false, filtersToApply) => {
      setError(null);
      const isInitial = pageNum === 1 && !isAppend;
      if (isInitial) setInitialLoading(true);
      else setLoading(true);

      const apiFilters = {
        code: filtersToApply.code || null,
        employeeName: filtersToApply.employeeName || null,
        startDate: filtersToApply.startDate || null,
        endDate: filtersToApply.endDate || null,
      };

      try {
        const data = await getPersonelCapability(pageNum, PAGE_SIZE, apiFilters);
        const newItems = data.items || [];

        const mapped = newItems.map((item, index) => normalizeCapacity(item, index));

        const isTrulyFinished = newItems.length === 0 || newItems.length < PAGE_SIZE;
        setHasMore(!isTrulyFinished);
        setIsFullyLoaded(isTrulyFinished);

        setCapacities(prev => (isAppend ? [...prev, ...mapped] : mapped));

        if (isInitial) {
          setTotalItems(data.totalItems || 0);
        }
      } catch (err) {
        setError('Không thể tải dữ liệu năng lực. Vui lòng thử lại.');
        console.error('Fetch error:', err);
      } finally {
        setLoading(false);
        setInitialLoading(false);
      }
    },
    [personsMap]
  );

  // === EFFECT: TẢI THÊM ===
  useEffect(() => {
    if (page > 1 && !isFullyLoaded) {
      fetchCapacities(page, true, activeFilters);
    }
  }, [page, isFullyLoaded, fetchCapacities, activeFilters]);

  // === EFFECT: TẢI MỚI KHI FILTER THAY ĐỔI ===
  useEffect(() => {
    setPage(1);
    setCapacities([]);
    setIsFullyLoaded(false);
    setHasMore(true);

    if (scrollContainerRef.current) {
      scrollContainerRef.current.scrollTo({ top: 0, behavior: 'auto' });
    }

    fetchCapacities(1, false, activeFilters);
  }, [activeFilters, fetchCapacities]);

  // === EFFECT: RESET KHI RELOAD ===
  useEffect(() => {
    if (reloadKey > 0) {
      const defaultFilters = { code: '', employeeName: '', startDate: '', endDate: '' };
      setFilterValues(defaultFilters);
      setActiveFilters(defaultFilters);
    }
  }, [reloadKey]);

  // === CLEANUP ===
  useEffect(() => {
    return () => {
      if (observer.current) observer.current.disconnect();
    };
  }, []);

  // === HANDLERS ===
  const handleReload = () => {
    setReloadKey(prev => prev + 1);
  };

  const applyFilter = () => {
    setActiveFilters(filterValues);
    setShowFilter(false);
  };

  const handleViewDetail = (capacityId) => {
    setSelectedCapacityId(selectedCapacityId === capacityId ? null : capacityId);
  };

  const handleCreateNew = () => setShowCreateNew(true);
  const handleBack = () => {
    setShowCreateNew(false);
    setSelectedCapacityId(null);
    handleReload();
  };

  // === SKELETON ROW ===
  const SkeletonRow = () => (
    <tr className="skeleton-row">
      {[...Array(8)].map((_, i) => (
        <td key={i}><div className="skeleton-line"></div></td>
      ))}
    </tr>
  );

  // === RENDER TẠO MỚI ===
  if (showCreateNew) {
    return (
      <div>
        <div className="detail-nav">
          <button onClick={handleBack} className="back-btn">Quay lại</button>
        </div>
        <CreateNewHumanCapacity />
      </div>
    );
  }

  // === RENDER DANH SÁCH ===
  return (
    <div className="human-capacity-page">
      {/* HEADER */}
      <div className="human-capacity-header">
        <div className="actions">
          <button className="action-btn filter-btn" onClick={() => setShowFilter(!showFilter)}>
            🔍Tìm kiếm
          </button>
          <button className={`action-btn new-btn ${!canEdit ? 'disabled-button' : ''}`} 
          onClick={handleCreateNew} 
          disabled={!canEdit} 
          title={!canEdit ? "Bạn không có quyền tạo mới năng lực nhân sự" : "Tạo mới năng lực nhân sự"}>
            ➕Tạo mới
          </button>
          {(initialLoading || error || capacities.length > 0) && (
            <button
              className="action-btn reload-btn"
              onClick={handleReload}
              disabled={loading || initialLoading}
            >
              <FaSyncAlt className={loading || initialLoading ? 'spinning' : ''} />
              Tải lại
            </button>
          )}

          {/* FILTER PANEL */}
          {showFilter && (
            <div className="filter-panel active">
              <div>
                <label>Mã năng lực</label>
                <input
                  type="text"
                  placeholder="Nhập mã"
                  value={filterValues.code}
                  onChange={(e) => setFilterValues({ ...filterValues, code: e.target.value })}
                />
              </div>
              <div>
                <label>Tên nhân viên</label>
                <input
                  type="text"
                  placeholder="Nhập tên"
                  value={filterValues.employeeName}
                  onChange={(e) => setFilterValues({ ...filterValues, employeeName: e.target.value })}
                />
              </div>
              <div>
                <label>Thời gian bắt đầu</label>
                <input
                  type="date"
                  value={filterValues.startDate}
                  onChange={(e) => setFilterValues({ ...filterValues, startDate: e.target.value })}
                />
              </div>
              <div>
                <label>Thời gian kết thúc</label>
                <input
                  type="date"
                  value={filterValues.endDate}
                  onChange={(e) => setFilterValues({ ...filterValues, endDate: e.target.value })}
                />
              </div>
              <div className="filter-actions">
                <button onClick={applyFilter} className="search-btn">Áp dụng</button>
                <button onClick={() => setShowFilter(false)} className="close-btn">Đóng</button>
              </div>
            </div>
          )}
        </div>
      </div>

      {/* SUMMARY */}
      <div className="summary">
        Đang hiển thị {capacities.length.toLocaleString()} / {totalItems.toLocaleString()} năng lực nhân sự.
        {isFullyLoaded && capacities.length > 0 && " (Đã tải hết)"}
      </div>

      {/* TABLE */}
      <div className="human-capacity-table">
        <div className="table-container" ref={scrollContainerRef}>
          <table role="grid">
            <thead>
              <tr>
                <th>STT</th>
                <th>Tên nhân viên</th>
                <th>Mã năng lực</th>
                <th>Số lượng</th>
                <th>Đơn vị</th>
                <th>Bắt đầu</th>
                <th>Kết thúc</th>
                <th>Chi tiết</th>
              </tr>
            </thead>
            <tbody>
              {/* SKELETON */}
              {initialLoading && capacities.length === 0 && [...Array(PAGE_SIZE)].map((_, i) => <SkeletonRow key={i} />)}

              {/* DATA */}
              {capacities.map((capacity, index) => (
                <React.Fragment key={capacity.id}>
                  <tr
                    ref={index === capacities.length - 1 && hasMore ? lastCapacityRef : null}
                    className="capacity-row"
                  >
                    <td>{capacity.stt}</td>
                    <td>{capacity.employeeName}</td>
                    <td>{capacity.code}</td>
                    <td>{capacity.quantity}</td>
                    <td>{capacity.unit}</td>
                    <td>{capacity.startDate}</td>
                    <td>{capacity.endDate}</td>
                    <td>
                      <button
                        className="view-btn"
                        onClick={() => handleViewDetail(capacity.id)}
                        title="Xem chi tiết"
                      >
                        <i className={`fas ${selectedCapacityId === capacity.id ? 'fa-eye-slash' : 'fa-eye'}`}></i>
                      </button>
                    </td>
                  </tr>

                  {/* BẢNG CHI TIẾT */}
                  {selectedCapacityId === capacity.id && (
                    <tr>
                      <td colSpan="8">
                        <div className="details-table">
                          <h3>Chi tiết năng lực: {capacity.employeeName}</h3>
                          <table role="grid">
                            <thead>
                              <tr>
                                <th>STT</th>
                                <th>Tên năng lực</th>
                                <th>Giá trị</th>
                                <th>Đơn vị</th>
                                <th>Mô tả</th>
                              </tr>
                            </thead>
                            <tbody>
                              {capacity.details.length > 0 ? (
                                capacity.details.map((detail, i) => (
                                  <tr key={detail.id}>
                                    <td>{i + 1}</td>
                                    <td>{detail.name}</td>
                                    <td>
                                      <span className={`value-badge ${detail.type}`}>
                                        {detail.value}
                                      </span>
                                    </td>
                                    <td>{detail.unit}</td>
                                    <td>{detail.description}</td>
                                  </tr>
                                ))
                              ) : (
                                <tr>
                                  <td colSpan="5" className="no-details">
                                    Không có chi tiết năng lực
                                  </td>
                                </tr>
                              )}
                            </tbody>
                          </table>
                        </div>
                      </td>
                    </tr>
                  )}
                </React.Fragment>
              ))}

              {/* LOADING MORE */}
              {loading && !initialLoading && (
                <tr>
                  <td colSpan="8" className="loading-cell">
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
                  <td colSpan="8" className="error-message">
                    {error}
                    <button onClick={handleReload} className="retry-btn">Thử lại</button>
                  </td>
                </tr>
              )}

              {/* NO DATA */}
              {!initialLoading && !loading && capacities.length === 0 && !error && (
                <tr>
                  <td colSpan="8" className="no-data-message">
                    Không tìm thấy năng lực nào phù hợp.
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

export default HumanCapacity;