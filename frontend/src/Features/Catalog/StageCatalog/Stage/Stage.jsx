import React, { useState, useEffect, useRef, useCallback } from 'react';
import CreateNewStage from './CreateNewStage';
import { getProcessSegments, getProcessSegmentDetail } from '../../../../api/ProcessSegment';
import { FaSyncAlt } from 'react-icons/fa';
import './Stage.css';
import StageDetail from './StageDetail';
import { useAuthPermissions } from '../../../../api/UseAuthPermissions';

const PAGE_SIZE = 31;

const Stage = ({ onBack }) => {
  // === STATE ===
  const [stages, setStages] = useState([]);
  const [loading, setLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(true);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [totalItems, setTotalItems] = useState(0);
  const [isFullyLoaded, setIsFullyLoaded] = useState(false);
  const [reloadKey, setReloadKey] = useState(0);

  const { permissions, loading: permLoading, error: permError } = useAuthPermissions(7);
  const canView = permissions.some((p) => p.canView);
  const canEdit = permissions.some((p) => p.canEdit);

  // Filter
  const [showFilter, setShowFilter] = useState(false);
  const [filterValues, setFilterValues] = useState({ code: '', name: '', type: 'Tất cả' });
  const [activeFilters, setActiveFilters] = useState(filterValues);

  // Tạo mới
  const [showCreateNew, setShowCreateNew] = useState(false);

  // Thêm state để lưu công đoạn đang xem chi tiết
  const [selectedStage, setSelectedStage] = useState(null);

  const observer = useRef();
  const scrollContainerRef = useRef(null);

  // === INFINITE SCROLL CALLBACK ===
  const lastStageRef = useCallback(
    (node) => {
      if (loading || initialLoading) return;
      if (observer.current) observer.current.disconnect();
      observer.current = new IntersectionObserver(
        (entries) => {
          if (entries[0].isIntersecting && hasMore && !loading) {
            setPage((prev) => prev + 1);
          }
        },
        { root: scrollContainerRef.current, rootMargin: '300px', threshold: 0.1 }
      );
      if (node) observer.current.observe(node);
    },
    [loading, initialLoading, hasMore]
  );

  // === CHUẨN HÓA DỮ LIỆU ===
  const normalizeStage = (item, index) => {
    const stt = (page - 1) * PAGE_SIZE + index + 1;
    const typeMap = { 0: 'Kéo', 1: 'Tráng men', 2: 'Kéo + Tráng' };
    const type = typeMap[item.segmentType] || 'Khác';
    const typeClass = type === 'Kéo' ? 'production' : type === 'Tráng men' ? 'enamel' : 'combined';
    const requiredCount = item.equipmentSegmentSpecificationCount + item.materialSegmentSpecificationCount + item.personnelSegmentSpecificationCount;
    return {
      id: item.processSegmentId || `STG${stt.toString().padStart(3, '0')}`,
      stt,
      code: item.processSegmentId || '--',
      name: item.name || '--',
      description: item.description || '--',
      type,
      typeClass,
      productionParams: item.productionParameter || '--',
      hierarchyScope: item.hierarchyScope || '--',
      requiredCount: requiredCount,
      duration: item.duration?.valueString || '--',
      status: 'Hoạt động',
      rawData: item,
    };
  };

  // === FETCH DATA (SERVER-SIDE FILTER) ===
  const fetchStages = useCallback(
    async (pageNum, isAppend = false, filtersToApply) => {
      setError(null);
      const isInitial = pageNum === 1 && !isAppend;
      if (isInitial) setInitialLoading(true);
      else setLoading(true);

      const apiFilters = {
        code: filtersToApply.code || null,
        name: filtersToApply.name || null,
        type: filtersToApply.type !== 'Tất cả' ? filtersToApply.type : null,
      };

      try {
        const data = await getProcessSegments(pageNum, PAGE_SIZE, apiFilters);
        const newItems = data.items || [];
        const mapped = newItems.map((item, index) => normalizeStage(item, index));
        const isTrulyFinished = newItems.length === 0 || newItems.length < PAGE_SIZE;
        setHasMore(!isTrulyFinished);
        setIsFullyLoaded(isTrulyFinished);
        setStages((prev) => (isAppend ? [...prev, ...mapped] : mapped));

        if (isInitial) {
          setTotalItems(data.totalItems || 0);
        }
      } catch (err) {
        setError('Không thể tải danh sách công đoạn. Vui lòng thử lại.');
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
      fetchStages(page, true, activeFilters);
    }
  }, [page, isFullyLoaded, fetchStages, activeFilters]);

  // === EFFECT: TẢI MỚI KHI FILTER THAY ĐỔI ===
  useEffect(() => {
    setPage(1);
    setStages([]);
    setIsFullyLoaded(false);
    setHasMore(true);

    if (scrollContainerRef.current) {
      scrollContainerRef.current.scrollTo({ top: 0, behavior: 'auto' });
    }

    fetchStages(1, false, activeFilters);
  }, [activeFilters, fetchStages]);

  // === EFFECT: RESET KHI RELOAD ===
  useEffect(() => {
    if (reloadKey > 0) {
      const defaultFilters = { code: '', name: '', type: 'Tất cả' };
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
  const handleReload = () => setReloadKey((prev) => prev + 1);
  const applyFilter = () => {
    setActiveFilters(filterValues);
    setShowFilter(false);
  };
  const handleCreateNew = () => setShowCreateNew(true);
  const handleBack = () => {
    setShowCreateNew(false);
    handleReload();
  };

  // Handler xem chi tiết
  const handleViewDetail = (stage) => {
    setSelectedStage(stage);
  };
  // Handler quay lại từ trang chi tiết
  const handleBackFromDetail = () => {
    setSelectedStage(null);
  };

  // === SKELETON ROW ===
  const SkeletonRow = () => (
    <tr className="skeleton-row">
      {[...Array(8)].map((_, i) => (
        <td key={i}>
          <div className="skeleton-line"></div>
        </td>
      ))}
    </tr>
  );

  // === RENDER TẠO MỚI (CÓ NÚT QUAY LẠI) ===
  if (showCreateNew) {
    return (
      <div>
        <div className="detail-nav">
          <button onClick={handleBack} className="back-btn">
            &larr; Quay lại danh sách
          </button>
        </div>
        <CreateNewStage />
      </div>
    );
  }

  // === RENDER CHI TIẾT CÔNG ĐOẠN ===
  if (selectedStage) {
    return (
      <div>
        <div className="detail-nav">
          <button onClick={handleBackFromDetail} className="back-btn">
            &larr; Quay lại danh sách
          </button>
        </div>
        <StageDetail stage={selectedStage} />
      </div>
    );
  }

  // === RENDER DANH SÁCH ===
  return (
    <div className="stage-root">
      {/* HEADER ACTIONS */}
      <div className="stage-header">
        <div className="actions">
          <button className="action-btn filter-btn" onClick={() => setShowFilter(!showFilter)}>
            🔍Tìm kiếm
          </button>
          <button className={`action-btn new-btn ${!canEdit ? "disabled-button" : ""}`}
            disabled={!canEdit}
            title= {!canEdit ? "Bạn không có quyền tạo công đoạn" : "Tạo mới công đoạn"}
          onClick={handleCreateNew}>
            ➕Tạo mới
          </button>
          {(initialLoading || error || stages.length > 0) && (
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
                <label>Mã/Tên công đoạn</label>
                <input
                  type="text"
                  placeholder="Mã hoặc tên"
                  value={filterValues.code}
                  onChange={(e) => setFilterValues({ ...filterValues, code: e.target.value })}
                />
              </div>
              <div>
                <label>Tên công đoạn</label>
                <input
                  type="text"
                  placeholder="Nhập tên"
                  value={filterValues.name}
                  onChange={(e) => setFilterValues({ ...filterValues, name: e.target.value })}
                />
              </div>
              <div>
                <label>Loại công đoạn</label>
                <select
                  value={filterValues.type}
                  onChange={(e) => setFilterValues({ ...filterValues, type: e.target.value })}
                >
                  <option value="Tất cả">Tất cả</option>
                  <option value="Sản xuất">Sản xuất</option>
                  <option value="Tráng men">Tráng men</option>
                  <option value="Kéo + Tráng">Kéo + Tráng</option>
                </select>
              </div>
              <div className="filter-actions">
                <button onClick={applyFilter} className="search-btn">
                  Áp dụng
                </button>
                <button onClick={() => setShowFilter(false)} className="close-btn">
                  Đóng
                </button>
              </div>
            </div>
          )}
        </div>
      </div>

      {/* SUMMARY */}
      <div className="summary">
        Đang hiển thị {stages.length.toLocaleString()} / {totalItems.toLocaleString()} công đoạn.
        {isFullyLoaded && stages.length > 0 && ' (Đã tải hết)'}
      </div>

      {/* BẢNG CHÍNH */}
      <div className="stages-table">
        <div className="table-container" ref={scrollContainerRef}>
          <table role="grid">
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã công đoạn</th>
                <th>Tên công đoạn</th>
                <th>Mô tả</th>
                <th>Loại</th>
                <th>Thông số sản xuất</th>
                <th>Yêu cầu công đoạn</th>
                <th>Thời gian</th>
                <th>Chi tiết</th> {/* Thêm cột Chi tiết */}
              </tr>
            </thead>
            <tbody>
              {initialLoading && stages.length === 0 && [...Array(PAGE_SIZE)].map((_, i) => <SkeletonRow key={i} />)}
              {stages.map((stage, index) => (
                <tr
                  key={stage.id}
                  ref={index === stages.length - 1 && hasMore ? lastStageRef : null}
                  className="stage-row"
                >
                  <td>{stage.stt}</td>
                  <td>{stage.code}</td>
                  <td>{stage.name}</td>
                  <td>{stage.description}</td>
                  <td>
                    <span className={`type-badge ${stage.typeClass}`}>{stage.type}</span>
                  </td>
                  <td>{stage.productionParams}</td>
                  <td>{stage.requiredCount}</td>
                  <td>{stage.duration}</td>
                  <td>
                    <button
                      className="view-btn"
                      onClick={() => handleViewDetail(stage)}
                      title="Xem chi tiết"
                    >
                      <i className="fas fa-eye"></i>
                    </button>
                  </td>
                </tr>
              ))}

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

              {error && !initialLoading && (
                <tr>
                  <td colSpan="8" className="error-message">
                    {error}
                    <button onClick={handleReload} className="retry-btn">
                      Thử lại
                    </button>
                  </td>
                </tr>
              )}

              {!initialLoading && !loading && stages.length === 0 && !error && (
                <tr>
                  <td colSpan="8" className="no-data-message">
                    Không tìm thấy công đoạn nào phù hợp.
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

export default Stage;
