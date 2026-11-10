import React, { useState, useEffect, useRef, useCallback } from 'react';
import { getSites } from '../../../api/Enterprise';
import CreateNewWorkCenter from './CreateNewWorkCenter';
import { FaSyncAlt } from 'react-icons/fa';
import './WorkCenter.css';
import { useAuthPermissions } from '../../../api/UseAuthPermissions';

const PAGE_SIZE = 31;

const workCenterTypeMap = {
   0 : 'Ô quy trình',
   1 : 'Đơn vị sản xuất',
   2 : 'Dây chuyền sản xuất',
   3 : 'Khu vực lưu trữ'
}

const WorkCenter = () => {
  // === STATE ===
  const [workCenters, setWorkCenters] = useState([]);
  const [loading, setLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(true);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [totalItems, setTotalItems] = useState(0);
  const [isFullyLoaded, setIsFullyLoaded] = useState(false);
  const [reloadKey, setReloadKey] = useState(0);

  // Filter
  const [showFilter, setShowFilter] = useState(false);
  const [filterValues, setFilterValues] = useState({ code: '', name: '', type: '' });
  const [activeFilters, setActiveFilters] = useState(filterValues);

  const { permissions, loading: permLoading, error: permError } = useAuthPermissions(8); // Giả sử pageId cho WorkCenter là 8
  const canView = permissions.some((p) => p.canView);
  const canEdit = permissions.some((p) => p.canEdit);

  // Tạo mới
  const [showCreateNew, setShowCreateNew] = useState(false);

  const observer = useRef();
  const scrollContainerRef = useRef(null);

  // === INFINITE SCROLL CALLBACK ===
  const lastWorkCenterRef = useCallback(
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
  const normalizeWorkCenter = (item, index, siteId) => {
    const stt = (page - 1) * PAGE_SIZE + index + 1;

    
    return {
      id: item.workCenterId || `WC${stt.toString().padStart(3, '0')}`,
      stt,
      code: item.workCenterId || '--',
      name: item.name || '--',
      factory: siteId || '--',
      type: workCenterTypeMap[item.workCenterType] || '--',
      mainEquipment: item.mainEquipmentCount,
      rawData: item,
    };
  };

  // === FLATTEN WORKCENTER DATA ===
  const flattenWorkCenters = (sites) => {
    const flattened = [];
    sites.forEach((site) => {
      if (site.areas && site.areas.length > 0) {
        site.areas.forEach((area) => {
          if (area.workCenters && area.workCenters.length > 0) {
            area.workCenters.forEach((wc) => {
              flattened.push({
                ...wc,
                siteId: site.siteId,
              });
            });
          }
        });
      }
    });
    return flattened;
  };

  // === FETCH DATA (SERVER-SIDE FILTER) ===
  const fetchWorkCenters = useCallback(
    async (pageNum, isAppend = false, filtersToApply) => {
      setError(null);
      const isInitial = pageNum === 1 && !isAppend;
      if (isInitial) setInitialLoading(true);
      else setLoading(true);

      const apiFilters = {
        code: filtersToApply.code || null,
        name: filtersToApply.name || null,
        type: filtersToApply.type && filtersToApply.type !== 'Tất cả' ? filtersToApply.type : null,
      };

      try {
        const data = await getSites(pageNum, PAGE_SIZE, apiFilters);
        const sites = data.items || [];

        // Flatten the nested structure
        const allWorkCenters = flattenWorkCenters(sites);

        // Apply client-side filtering if needed
        let filtered = allWorkCenters;
        if (filtersToApply.code) {
          filtered = filtered.filter(wc => 
            wc.workCenterId?.toLowerCase().includes(filtersToApply.code.toLowerCase()) ||
            wc.name?.toLowerCase().includes(filtersToApply.code.toLowerCase())
          );
        }
        if (filtersToApply.name) {
          filtered = filtered.filter(wc => 
            wc.name?.toLowerCase().includes(filtersToApply.name.toLowerCase())
          );
        }

        const mapped = filtered
          .slice(0, PAGE_SIZE)
          .map((item, index) => normalizeWorkCenter(item, index, item.siteId));

        const isTrulyFinished = filtered.length === 0 || filtered.length < PAGE_SIZE;
        setHasMore(!isTrulyFinished);
        setIsFullyLoaded(isTrulyFinished);

        setWorkCenters(prev => (isAppend ? [...prev, ...mapped] : mapped));

        if (isInitial) {
          setTotalItems(filtered.length);
        }
      } catch (err) {
        setError('Không thể tải danh sách WorkCenter. Vui lòng thử lại.');
        console.error('Fetch error:', err);
      } finally {
        setLoading(false);
        setInitialLoading(false);
      }
    },
    [page]
  );

  // === EFFECT: TẢI THÊM ===
  useEffect(() => {
    if (page > 1 && !isFullyLoaded) {
      fetchWorkCenters(page, true, activeFilters);
    }
  }, [page, isFullyLoaded, fetchWorkCenters, activeFilters]);

  // === EFFECT: TẢI MỚI KHI FILTER THAY ĐỔI ===
  useEffect(() => {
    setPage(1);
    setWorkCenters([]);
    setIsFullyLoaded(false);
    setHasMore(true);

    if (scrollContainerRef.current) {
      scrollContainerRef.current.scrollTo({ top: 0, behavior: 'auto' });
    }

    fetchWorkCenters(1, false, activeFilters);
  }, [activeFilters, fetchWorkCenters]);

  // === EFFECT: RESET KHI RELOAD ===
  useEffect(() => {
    if (reloadKey > 0) {
      const defaultFilters = { code: '', name: '', type: '' };
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

  const handleFilterChange = (e) => {
    const { name, value } = e.target;
    setFilterValues((prev) => ({ ...prev, [name]: value }));
  };

  const handleCreateNew = () => setShowCreateNew(true);
  const handleBack = () => {
    setShowCreateNew(false);
    handleReload();
  };

  // === SKELETON ROW ===
  const SkeletonRow = () => (
    <tr className="skeleton-row">
      {[...Array(6)].map((_, i) => (
        <td key={i}><div className="skeleton-line"></div></td>
      ))}
    </tr>
  );

  // === RENDER TẠO MỚI ===
  if (showCreateNew) {
    return (
      <div>
        <div className="detail-nav">
          <button onClick={handleBack} className="back-btn">← Quay lại</button>
        </div>
        <CreateNewWorkCenter />
      </div>
    );
  }

  // === RENDER DANH SÁCH ===
  return (
    <div className="workcenter-container">
      {/* TITLE */}
      <h2 className="title">Danh mục | WorkCenter</h2>

      {/* TABS */}
      <div className="tabs">
        <button className="tab-btn active" aria-label="Tab WorkCenter">
          WorkCenter
        </button>
      </div>

      {/* HEADER ACTIONS */}
      <div className="workcenter-header">
        <div className="actions">
          <button
            className="action-btn filter-btn"
            onClick={() => setShowFilter(!showFilter)}
            aria-label="Tìm kiếm WorkCenter"
          >
            <span className="icon">🔍</span> Tìm kiếm
          </button>
          <button
            className={`action-btn new-btn ${!canEdit ? 'disabled-button' : ''}`}
            disabled={!canEdit}
            title={!canEdit ? "Bạn không có quyền tạo mới WorkCenter" : "Tạo mới WorkCenter"}
            onClick={handleCreateNew}
            aria-label="Tạo mới WorkCenter"
          >
            <span className="icon">➕</span> Tạo mới
          </button>
          {(initialLoading || error || workCenters.length > 0) && (
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
                <label>Mã WorkCenter</label>
                <input
                  type="text"
                  name="code"
                  placeholder="Tìm kiếm theo Mã hoặc Tên WorkCenter"
                  value={filterValues.code}
                  onChange={handleFilterChange}
                  aria-label="Tìm kiếm theo mã hoặc tên WorkCenter"
                />
              </div>
              <div>
                <label>Tên WorkCenter</label>
                <input
                  type="text"
                  name="name"
                  placeholder="Tìm kiếm theo Tên WorkCenter"
                  value={filterValues.name}
                  onChange={handleFilterChange}
                  aria-label="Tìm kiếm theo tên WorkCenter"
                />
              </div>
              <div>
                <label>Loại WorkCenter</label>
                <select
                  name="type"
                  value={filterValues.type}
                  onChange={handleFilterChange}
                  aria-label="Lọc theo loại WorkCenter"
                >
                  <option value="Tất cả">Tất cả</option>
                  <option value="Kéo dây">Kéo dây</option>
                  <option value="Tráng men">Tráng men</option>
                </select>
              </div>
              <div className="filter-actions">
                <button className="search-btn" onClick={applyFilter} aria-label="Áp dụng bộ lọc">Tìm kiếm</button>
                <button className="close-btn" onClick={() => setShowFilter(false)} aria-label="Đóng bộ lọc">Đóng</button>
              </div>
            </div>
          )}
        </div>
      </div>

      {/* SUMMARY */}
      <div className="summary">
        Đang hiển thị {workCenters.length.toLocaleString()} / {totalItems.toLocaleString()} WorkCenter.
        {isFullyLoaded && workCenters.length > 0 && " (Đã tải hết)"}
      </div>

      {/* BẢNG CHÍNH */}
      <div className="workcenters-table">
        <div className="table-container" ref={scrollContainerRef}>
          <table role="grid">
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã WorkCenter</th>
                <th>Tên WorkCenter</th>
                <th>Xưởng</th>
                <th>Loại WorkCenter</th>
                <th>Số thiết bị chính</th>
              </tr>
            </thead>
            <tbody>
              {/* SKELETON */}
              {initialLoading && workCenters.length === 0 && [...Array(PAGE_SIZE)].map((_, i) => <SkeletonRow key={i} />)}

              {/* DỮ LIỆU */}
              {workCenters.map((wc, index) => (
                <tr
                  key={wc.id}
                  ref={index === workCenters.length - 1 && hasMore ? lastWorkCenterRef : null}
                  className="workcenter-row"
                >
                  <td>{wc.stt}</td>
                  <td>{wc.code}</td>
                  <td>{wc.name}</td>
                  <td>{wc.factory}</td>
                  <td>{wc.type}</td>
                  <td>{wc.mainEquipment}</td>
                </tr>
              ))}

              {/* LOADING MORE */}
              {loading && !initialLoading && (
                <tr>
                  <td colSpan="6" className="loading-cell">
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
                  <td colSpan="6" className="error-message">
                    {error}
                    <button onClick={handleReload} className="retry-btn">Thử lại</button>
                  </td>
                </tr>
              )}

              {/* NO DATA */}
              {!initialLoading && !loading && workCenters.length === 0 && !error && (
                <tr>
                  <td colSpan="6" className="no-data-message">
                    Không tìm thấy WorkCenter nào phù hợp.
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

export default WorkCenter;