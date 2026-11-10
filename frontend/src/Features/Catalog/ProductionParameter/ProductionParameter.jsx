import React, { useState, useEffect, useRef, useCallback } from 'react';
import CreateNewProductionParameter from './CreateNewProductionParameter';
import { getProductionParameters } from '../../../api/ProductionParameter';
import { FaSyncAlt, FaEye, FaEyeSlash } from 'react-icons/fa';
import './ProductionParameter.css';
import { useAuthPermissions } from '../../../api/UseAuthPermissions';

const PAGE_SIZE = 31;

const statusMap = {
  InProduction: { label: 'Đang sản xuất', class: 'in-production' },
  Available: { label: 'Khả dụng', class: 'available' },
  Unavailable: { label: 'Không khả dụng', class: 'unavailable' },
};

// Tối ưu: để ngoài component
const normalizeParameter = (item, pageNum, index) => {
  const stt = (pageNum - 1) * PAGE_SIZE + index + 1;
  const statusInfo = statusMap[item.status] || { label: 'Không xác định', class: 'unknown' };
  const endDate =
    item.validTo === '0001-01-01T00:00:00' || !item.validTo
      ? 'Vô thời hạn'
      : item.validTo.split('T')[0];

  return {
    id: item.productionParameterId || `PP${stt.toString().padStart(3, '0')}`,
    stt,
    code: item.productionParameterId || '--',
    status: statusInfo.label,
    statusClass: statusInfo.class,
    startDate: item.validFrom?.split('T')[0] || '--',
    endDate,
    note: item.description || '--',
    rawData: item,
  };
};

const ProductionParameter = () => {
  const [parameters, setParameters] = useState([]);
  const [totalItems, setTotalItems] = useState(0);
  const [isFullyLoaded, setIsFullyLoaded] = useState(false);
  const [reloadKey, setReloadKey] = useState(0);

  const [showFilter, setShowFilter] = useState(false);
  const [filterValues, setFilterValues] = useState({ code: '', status: 'Tất cả' });
  const [activeFilters, setActiveFilters] = useState(filterValues);

  const { permissions, loading: permLoading, error: permError } = useAuthPermissions(7);
  const canView = permissions.some((p) => p.canView);
  const canEdit = permissions.some((p) => p.canEdit);

  const [showCreateNew, setShowCreateNew] = useState(false);
  const [selectedParamId, setSelectedParamId] = useState(null);

  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(true);
  const [hasMore, setHasMore] = useState(true);
  const [error, setError] = useState(null);

  const observer = useRef();
  const scrollContainerRef = useRef(null);

  const lastParameterRef = useCallback(
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
          threshold: 0.1,
        }
      );

      if (node) observer.current.observe(node);
    },
    [loading, initialLoading, hasMore]
  );

  const fetchParameters = useCallback(
    async (pageNum, isAppend = false, filtersToApply) => {
      setError(null);
      const isInitial = pageNum === 1 && !isAppend;

      if (isInitial) setInitialLoading(true);
      else setLoading(true);

      const apiFilters = {
        code: filtersToApply.code || null,
        status: filtersToApply.status === 'Tất cả' ? null : filtersToApply.status,
      };

      try {
        const data = await getProductionParameters(pageNum, PAGE_SIZE, apiFilters);
        const newItems = data.items || [];

        const mapped = newItems.map((item, index) =>
          normalizeParameter(item, pageNum, index)
        );

        const finished = newItems.length === 0 || newItems.length < PAGE_SIZE;
        setHasMore(!finished);
        setIsFullyLoaded(finished);

        setParameters((prev) => (isAppend ? [...prev, ...mapped] : mapped));

        if (isInitial) {
          setTotalItems(data.totalItems || 0);
        }
      } catch (err) {
        console.error('Fetch Production Parameters Error:', err);
        setError('Không thể tải danh sách thông số sản xuất. Vui lòng thử lại.');
      } finally {
        setLoading(false);
        setInitialLoading(false);
      }
    },
    []
  );

  useEffect(() => {
    if (page > 1 && !isFullyLoaded) {
      fetchParameters(page, true, activeFilters);
    }
  }, [page, isFullyLoaded, fetchParameters, activeFilters]);

  useEffect(() => {
    setPage(1);
    setParameters([]);
    setIsFullyLoaded(false);
    setHasMore(true);

    if (scrollContainerRef.current) {
      scrollContainerRef.current.scrollTo({ top: 0, behavior: 'auto' });
    }

    fetchParameters(1, false, activeFilters);
  }, [activeFilters, fetchParameters]);

  useEffect(() => {
    if (reloadKey > 0) {
      const defaultFilters = { code: '', status: 'Tất cả' };
      setFilterValues(defaultFilters);
      setActiveFilters(defaultFilters);
    }
  }, [reloadKey]);

  useEffect(() => {
    return () => {
      if (observer.current) observer.current.disconnect();
    };
  }, []);

  const handleReload = () => setReloadKey((prev) => prev + 1);
  const applyFilter = () => {
    setActiveFilters(filterValues);
    setShowFilter(false);
  };

  const handleViewDetail = (id) =>
    setSelectedParamId((prev) => (prev === id ? null : id));

  const handleCreateNew = () => setShowCreateNew(true);
  const handleBack = () => {
    setShowCreateNew(false);
    handleReload();
  };

  const SkeletonRow = () => (
    <tr className="skeleton-row">
      {[...Array(7)].map((_, i) => (
        <td key={i}>
          <div className="skeleton-line"></div>
        </td>
      ))}
    </tr>
  );

  if (showCreateNew) {
    return (
      <div>
        <div className="detail-nav">
          <button onClick={handleBack} className="back-btn">
            Quay lại
          </button>
        </div>
        <CreateNewProductionParameter />
      </div>
    );
  }

  return (
    <div className="parameter-root">
      <h2 className="title">Danh mục | Thông số sản xuất</h2>

      <div className="tabs">
        <button className="tab-btn active">Thông số sản xuất</button>
      </div>

      <div className="parameter-header">
        <div className="actions">
          <button className="action-btn filter-btn" onClick={() => setShowFilter(!showFilter)}>
            🔍Tìm kiếm
          </button>

          <button className={`action-btn new-btn ${!canEdit ? "disabled-button" : ""}`}
          disabled={!canEdit}
          title={!canEdit ? "Bạn không có quyền tạo thông số" : "Tạo mới thông số"}
          onClick={handleCreateNew}>
            ➕Tạo mới
          </button>

          {(initialLoading || error || parameters.length > 0) && (
            <button
              className="action-btn reload-btn"
              onClick={handleReload}
              disabled={loading || initialLoading}
            >
              <FaSyncAlt className={loading || initialLoading ? 'spinning' : ''} />
              Tải lại
            </button>
          )}

          {showFilter && (
            <div className="filter-panel active">
              <div>
                <label>Mã thông số</label>
                <input
                  type="text"
                  placeholder="Nhập mã thông số"
                  value={filterValues.code}
                  onChange={(e) =>
                    setFilterValues({ ...filterValues, code: e.target.value })
                  }
                />
              </div>

              <div>
                <label>Tình trạng</label>
                <select
                  value={filterValues.status}
                  onChange={(e) =>
                    setFilterValues({ ...filterValues, status: e.target.value })
                  }
                >
                  <option value="Tất cả">Tất cả</option>
                  <option value="Đang sản xuất">Đang sản xuất</option>
                  <option value="Khả dụng">Khả dụng</option>
                  <option value="Không khả dụng">Không khả dụng</option>
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

      <div className="summary">
        Đang hiển thị {parameters.length.toLocaleString()} /{' '}
        {totalItems.toLocaleString()} thông số sản xuất.
        {isFullyLoaded && parameters.length > 0 && ' (Đã tải hết)'}
      </div>

      <div className="parameters-table">
        <div className="table-container" ref={scrollContainerRef}>
          <table role="grid">
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã thông số</th>
                <th>Tình trạng</th>
                <th>Áp dụng từ</th>
                <th>Kết thúc</th>
                <th>Ghi chú</th>
                <th>Chi tiết</th>
              </tr>
            </thead>

            <tbody>
              {initialLoading &&
                parameters.length === 0 &&
                [...Array(PAGE_SIZE)].map((_, i) => <SkeletonRow key={i} />)}

              {parameters.map((param, index) => {
                const properties = param.rawData?.properties || [];
                const isSelected = selectedParamId === param.id;

                return (
                  <React.Fragment key={param.id}>
                    <tr
                      ref={index === parameters.length - 1 && hasMore ? lastParameterRef : null}
                      className={`parameter-row ${isSelected ? 'selected' : ''}`}
                    >
                      <td>{param.stt}</td>
                      <td>{param.code}</td>
                      <td>
                        <span className={`status-badge ${param.statusClass}`}>
                          {param.status}
                        </span>
                      </td>
                      <td>{param.startDate}</td>
                      <td>{param.endDate}</td>
                      <td>{param.note}</td>
                      <td>
                        <button
                          className="view-btn"
                          onClick={() => handleViewDetail(param.id)}
                          title="Xem chi tiết"
                        >
                          {isSelected ? <FaEyeSlash /> : <FaEye />}
                        </button>
                      </td>
                    </tr>

                    {isSelected && (
                      <tr>
                        <td colSpan="7">
                          <div className="detail-properties-table">
                            <h3>Chi tiết thông số: {param.code}</h3>
                            <table role="grid">
                              <thead>
                                <tr>
                                  <th>STT</th>
                                  <th>Tên thông số</th>
                                  <th>Giá trị</th>
                                  <th>Đơn vị</th>
                                  <th>Mô tả</th>
                                </tr>
                              </thead>
                              <tbody>
                                {properties.length > 0 ? (
                                  properties.map((prop, idx) => (
                                    <tr key={prop.propertyId || idx}>
                                      <td>{idx + 1}</td>
                                      <td>{prop.name || '--'}</td>
                                      <td>{prop.valueString || '--'}</td>
                                      <td>{prop.valueUnitOfMeasure || '--'}</td>
                                      <td>{prop.description || '--'}</td>
                                    </tr>
                                  ))
                                ) : (
                                  <tr>
                                    <td colSpan="5" className="no-properties">
                                      Không có thông số chi tiết.
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
                );
              })}

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

              {error && !initialLoading && (
                <tr>
                  <td colSpan="7" className="error-message">
                    {error}
                    <button onClick={handleReload} className="retry-btn">
                      Thử lại
                    </button>
                  </td>
                </tr>
              )}

              {!initialLoading && !loading && parameters.length === 0 && !error && (
                <tr>
                  <td colSpan="7" className="no-data-message">
                    Không tìm thấy thông số sản xuất nào phù hợp.
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

export default ProductionParameter;
