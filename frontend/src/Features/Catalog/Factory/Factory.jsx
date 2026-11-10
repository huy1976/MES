import React, { useState, useEffect, useRef, useCallback } from 'react';
import CreateNewFactory from './CreateNewFactory';
import { getSites } from '../../../api/Enterprise';
import { FaSyncAlt } from 'react-icons/fa';
import './Factory.css';
import { useAuthPermissions } from '../../../api/UseAuthPermissions';

const PAGE_SIZE = 31;

const Factory = () => {
  // === STATE ===
  const [factories, setFactories] = useState([]);
  const [loading, setLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(true);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [totalItems, setTotalItems] = useState(0);
  const [isFullyLoaded, setIsFullyLoaded] = useState(false);
  const [reloadKey, setReloadKey] = useState(0);

  const { permissions, loading: permLoading, error: permError } = useAuthPermissions(7); // Giả sử pageId cho Factory là 7
  const canView = permissions.some((p) => p.canView);
  const canEdit = permissions.some((p) => p.canEdit);

  // Chi tiết mở rộng
  const [expandedRow, setExpandedRow] = useState(null);

  // Filter
  const [showFilter, setShowFilter] = useState(false);
  const [filterValues, setFilterValues] = useState({ code: '', name: '', manager: '' });
  const [activeFilters, setActiveFilters] = useState(filterValues);

  // Tạo mới
  const [showCreateNew, setShowCreateNew] = useState(false);

  const observer = useRef();
  const scrollContainerRef = useRef(null);

  // === INFINITE SCROLL CALLBACK ===
  const lastFactoryRef = useCallback(
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
  const normalizeFactory = (item, index) => {
    const stt = (page - 1) * PAGE_SIZE + index + 1;
    const areas = item.areas || [];
    const workCenters = areas.flatMap(a => a.workCenters || []);
    const areaCount = areas.length;
    const workCenterCount = workCenters.length;

    return {
      id: item.siteId || `FAC${stt.toString().padStart(3, '0')}`,
      stt,
      code: item.siteId || '--',
      name: item.name || '--',
      location: item.absolutePath || '--',
      areaCount,
      workCenterCount,
      areas: areas.map(area => ({
        id: area.areaId,
        name: area.name,
        workCenters: (area.workCenters || []).map(wc => ({
          workCenterId: wc.workCenterId,
          name: wc.name,
          type: wc.workCenterType === 2 ? 'Sản xuất' : 'Khác',
          typeClass: wc.workCenterType === 2 ? 'production' : 'other'
        }))
      })),
      rawData: item,
    };
  };

  // === FETCH DATA (SERVER-SIDE FILTER) ===
  const fetchFactories = useCallback(
    async (pageNum, isAppend = false, filtersToApply) => {
      setError(null);
      const isInitial = pageNum === 1 && !isAppend;
      if (isInitial) setInitialLoading(true);
      else setLoading(true);

      const apiFilters = {
        code: filtersToApply.code || null,
        name: filtersToApply.name || null,
      };

      try {
        const data = await getSites(pageNum, PAGE_SIZE, apiFilters);
        const newItems = data.items || [];

        const mapped = newItems.map((item, index) => normalizeFactory(item, index));

        const isTrulyFinished = newItems.length === 0 || newItems.length < PAGE_SIZE;
        setHasMore(!isTrulyFinished);
        setIsFullyLoaded(isTrulyFinished);

        setFactories(prev => (isAppend ? [...prev, ...mapped] : mapped));

        if (isInitial) {
          setTotalItems(data.totalItems || 0);
        }
      } catch (err) {
        setError('Không thể tải danh sách xưởng sản xuất. Vui lòng thử lại.');
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
      fetchFactories(page, true, activeFilters);
    }
  }, [page, isFullyLoaded, fetchFactories, activeFilters]);

  // === EFFECT: TẢI MỚI KHI FILTER THAY ĐỔI ===
  useEffect(() => {
    setPage(1);
    setFactories([]);
    setIsFullyLoaded(false);
    setHasMore(true);

    if (scrollContainerRef.current) {
      scrollContainerRef.current.scrollTo({ top: 0, behavior: 'auto' });
    }

    fetchFactories(1, false, activeFilters);
  }, [activeFilters, fetchFactories]);

  // === EFFECT: RESET KHI RELOAD ===
  useEffect(() => {
    if (reloadKey > 0) {
      const defaultFilters = { code: '', name: '' };
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

  const toggleExpandRow = (id) => {
    setExpandedRow(expandedRow === id ? null : id);
  };

  const handleCreateNew = () => setShowCreateNew(true);
  const handleBack = () => {
    setShowCreateNew(false);
    setExpandedRow(null);
    handleReload();
  };

  // === SKELETON ROW ===
  const SkeletonRow = () => (
    <tr className="skeleton-row">
      {[...Array(7)].map((_, i) => (
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
        <CreateNewFactory />
      </div>
    );
  }

  // === RENDER DANH SÁCH ===
  return (
    <div className="factory-container">
      {/* TITLE */}
      <h2 className="title">Danh mục | Nhà máy</h2>

      {/* TABS */}
      <div className="tabs">
        <button className="tab-btn active">Nhà máy</button>
      </div>

      {/* HEADER ACTIONS */}
      <div className="factory-header">
        <div className="actions">
          <button className="action-btn filter-btn" onClick={() => setShowFilter(!showFilter)}>
            🔍Tìm kiếm
          </button>
          <button className={`action-btn new-btn ${!canEdit ? 'disabled-button' : ''}`} 
          onClick={handleCreateNew} disabled={!canEdit} 
          title={!canEdit ? "Bạn không có quyền tạo mới nhà máy" : "Tạo mới nhà máy"}>
            ➕Tạo mới
          </button>
          {(initialLoading || error || factories.length > 0) && (
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
                <label>Mã nhà máy</label>
                <input
                  type="text"
                  placeholder="Nhập mã"
                  value={filterValues.code}
                  onChange={(e) => setFilterValues({ ...filterValues, code: e.target.value })}
                />
              </div>
              <div>
                <label>Tên nhà máy</label>
                <input
                  type="text"
                  placeholder="Nhập tên"
                  value={filterValues.name}
                  onChange={(e) => setFilterValues({ ...filterValues, name: e.target.value })}
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
        Đang hiển thị {factories.length.toLocaleString()} / {totalItems.toLocaleString()} xưởng sản xuất.
        {isFullyLoaded && factories.length > 0 && " (Đã tải hết)"}
      </div>

      {/* BẢNG CHÍNH */}
      <div className="factories-table">
        <div className="table-container" ref={scrollContainerRef}>
          <table role="grid">
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã nhà máy</th>
                <th>Tên nhà máy</th>
                <th>Địa điểm</th>
                <th>Số Area</th>
                <th>Số WorkCenter</th>
                <th>Chi tiết</th>
              </tr>
            </thead>
            <tbody>
              {/* SKELETON */}
              {initialLoading && factories.length === 0 && [...Array(PAGE_SIZE)].map((_, i) => <SkeletonRow key={i} />)}

              {/* DỮ LIỆU */}
              {factories.map((factory, index) => (
                <React.Fragment key={factory.id}>
                  <tr
                    ref={index === factories.length - 1 && hasMore ? lastFactoryRef : null}
                    className="factory-row"
                  >
                    <td>{factory.stt}</td>
                    <td>{factory.code}</td>
                    <td>{factory.name}</td>
                    <td>{factory.location}</td>
                    <td>{factory.areaCount}</td>
                    <td>{factory.workCenterCount}</td>
                    <td>
                      <button
                        className="view-btn"
                        onClick={() => toggleExpandRow(factory.id)}
                        title="Xem chi tiết"
                      >
                        <i className={`fas ${expandedRow === factory.id ? 'fa-eye-slash' : 'fa-eye'}`}></i>
                      </button>
                    </td>
                  </tr>

                  {/* BẢNG CHI TIẾT */}
                  {expandedRow === factory.id && (
                    <tr>
                      <td colSpan="7">
                        <div className="details-table">
                          <h3>Chi tiết nhà máy: {factory.name}</h3>

                          {factory.areas.length > 0 ? (
                            factory.areas.map((area) => (
                              <div key={area.id} className="area-section">
                                <h4 className="section-title">
                                  Khu vực: {area.name} ({area.workCenters.length} WorkCenter)
                                </h4>
                                <table role="grid" className="sub-table">
                                  <thead>
                                    <tr>
                                      <th>STT</th>
                                      <th>Mã WC</th>
                                      <th>Tên WorkCenter</th>
                                      <th>Loại</th>
                                    </tr>
                                  </thead>
                                  <tbody>
                                    {area.workCenters.map((wc, i) => (
                                      <tr key={wc.workCenterId}>
                                        <td>{i + 1}</td>
                                        <td>{wc.workCenterId}</td>
                                        <td>{wc.name}</td>
                                        <td>
                                          <span className={`value-badge ${wc.typeClass}`}>
                                            {wc.type}
                                          </span>
                                        </td>
                                      </tr>
                                    ))}
                                  </tbody>
                                </table>
                              </div>
                            ))
                          ) : (
                            <p className="no-data">Không có khu vực</p>
                          )}
                        </div>
                      </td>
                    </tr>
                  )}
                </React.Fragment>
              ))}

              {/* LOADING MORE */}
              {loading && !initialLoading && (
                <tr>
                  <td colSpan="7" className="loading-cell">
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
                  <td colSpan="7" className="error-message">
                    {error}
                    <button onClick={handleReload} className="retry-btn">Thử lại</button>
                  </td>
                </tr>
              )}

              {/* NO DATA */}
              {!initialLoading && !loading && factories.length === 0 && !error && (
                <tr>
                  <td colSpan="7" className="no-data-message">
                    Không tìm thấy nhà máy nào phù hợp.
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

export default Factory;