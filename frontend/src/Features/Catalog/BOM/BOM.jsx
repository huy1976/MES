import React, { useState, useEffect, useRef, useCallback } from 'react';
import CreateNewBOM from './CreateNewBOM';
import { getBillOfMaterials } from '../../../api/Bom';
import { FaSyncAlt } from 'react-icons/fa';
import './BOM.css';
import { useAuthPermissions } from '../../../api/UseAuthPermissions';

const PAGE_SIZE = 31;

const BOM = () => {
  // === STATE ===
  const [boms, setBoms] = useState([]);
  const [loading, setLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(true);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [totalItems, setTotalItems] = useState(0);
  const [isFullyLoaded, setIsFullyLoaded] = useState(false);
  const [reloadKey, setReloadKey] = useState(0);

  const { permissions, loading: permLoading, error: permError } = useAuthPermissions(7); // Giả sử pageId cho BOM là 8
  const canView = permissions.some((p) => p.canView);
  const canEdit = permissions.some((p) => p.canEdit);

  // Filter
  const [showFilter, setShowFilter] = useState(false);
  const [filterValues, setFilterValues] = useState({ code: '', material: '' });
  const [activeFilters, setActiveFilters] = useState(filterValues);

  // Tạo mới
  const [showCreateNew, setShowCreateNew] = useState(false);

  const observer = useRef();
  const scrollContainerRef = useRef(null);

  // === INFINITE SCROLL CALLBACK ===
  const lastBomRef = useCallback(
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
  const normalizeBOM = (item, index) => {
    const stt = (page - 1) * PAGE_SIZE + index + 1;

    return {
      id: item.billOfMaterialId || `BOM${stt.toString().padStart(4, '0')}`,
      stt,
      code: item.billOfMaterialId || '--',
      material: item.materialDefinition || '--',
      description: item.description || '--',
      quantity: parseFloat(item.quantity?.valueString || '0'),
      unit: item.valueUnitOfMeasure || '--',
      rawData: item,
    };
  };

  // === FETCH DATA (SERVER-SIDE FILTER) ===
  const fetchBOMs = useCallback(
    async (pageNum, isAppend = false, filtersToApply) => {
      setError(null);
      const isInitial = pageNum === 1 && !isAppend;
      if (isInitial) setInitialLoading(true);
      else setLoading(true);

      const apiFilters = {
        code: filtersToApply.code || null,
        material: filtersToApply.material || null,
      };

      try {
        const data = await getBillOfMaterials(pageNum, PAGE_SIZE, apiFilters);
        const newItems = data.items || [];

        const mapped = newItems.map((item, index) => normalizeBOM(item, index));

        const isTrulyFinished = newItems.length === 0 || newItems.length < PAGE_SIZE;
        setHasMore(!isTrulyFinished);
        setIsFullyLoaded(isTrulyFinished);

        setBoms(prev => (isAppend ? [...prev, ...mapped] : mapped));

        if (isInitial) {
          setTotalItems(data.totalItems || 0);
        }
      } catch (err) {
        setError('Không thể tải danh sách BOM. Vui lòng thử lại.');
        console.error('Fetch BOM error:', err);
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
      fetchBOMs(page, true, activeFilters);
    }
  }, [page, isFullyLoaded, fetchBOMs, activeFilters]);

  // === EFFECT: TẢI MỚI KHI FILTER THAY ĐỔI ===
  useEffect(() => {
    setPage(1);
    setBoms([]);
    setIsFullyLoaded(false);
    setHasMore(true);

    if (scrollContainerRef.current) {
      scrollContainerRef.current.scrollTo({ top: 0, behavior: 'auto' });
    }

    fetchBOMs(1, false, activeFilters);
  }, [activeFilters, fetchBOMs]);

  // === EFFECT: RESET KHI RELOAD ===
  useEffect(() => {
    if (reloadKey > 0) {
      const defaultFilters = { code: '', material: '' };
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

  const handleCreateNew = () => setShowCreateNew(true);
  const handleBack = () => {
    setShowCreateNew(false);
    handleReload(); // Hàm này sẽ kích hoạt tải lại dữ liệu
  };

  // === SKELETON ROW ===
  const SkeletonRow = () => (
    <tr className="skeleton-row">
      {[...Array(6)].map((_, i) => (
        <td key={i}><div className="skeleton-line"></div></td>
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
        <CreateNewBOM />
      </div>
    );
  }

  // === RENDER DANH SÁCH ===
  return (
    <div className="bom-root">
      <h2 className="title">Danh mục | BOM (Bill of Materials)</h2>

      <div className="tabs">
        <button className="tab-btn active">BOM</button>
      </div>

      <div className="bom-header">
        <div className="actions">
          <button className="action-btn filter-btn" onClick={() => setShowFilter(!showFilter)}>
            🔍Tìm kiếm
          </button>
          <button className={`action-btn new-btn ${!canEdit ? "disabled-button" : ""}`}
          disabled={!canEdit}
          title={!canEdit ? "Bạn không có quyền tạo mới BOM" : "Tạo mới BOM"}
          onClick={handleCreateNew}>
            ➕Tạo mới
          </button>
          {(initialLoading || error || boms.length > 0) && (
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
                <label>Mã/Tên vật liệu</label>
                <input
                  type="text"
                  placeholder="Mã BOM hoặc vật liệu"
                  value={filterValues.code}
                  onChange={(e) => setFilterValues({ ...filterValues, code: e.target.value })}
                />
              </div>
              <div>
                <label>Vật liệu đầu vào</label>
                <input
                  type="text"
                  placeholder="Nhập tên vật liệu"
                  value={filterValues.material}
                  onChange={(e) => setFilterValues({ ...filterValues, material: e.target.value })}
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

      <div className="summary">
        Đang hiển thị {boms.length.toLocaleString()} / {totalItems.toLocaleString()} BOM.
        {isFullyLoaded && boms.length > 0 && " (Đã tải hết)"}
      </div>

      <div className="boms-table">
        <div className="table-container" ref={scrollContainerRef}>
          <table role="grid">
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã BOM</th>
                <th>Vật liệu đầu vào</th>
                <th>Số lượng</th>
                <th>Đơn vị</th>
                <th>Mô tả</th>
              </tr>
            </thead>
            <tbody>
              {initialLoading && boms.length === 0 && [...Array(PAGE_SIZE)].map((_, i) => <SkeletonRow key={i} />)}

              {boms.map((bom, index) => (
                <tr
                  key={bom.id}
                  ref={index === boms.length - 1 && hasMore ? lastBomRef : null}
                  className="bom-row"
                >
                  <td>{bom.stt}</td>
                  <td>{bom.code}</td>
                  <td>{bom.material}</td>
                  <td>{bom.quantity}</td>
                  <td>{bom.unit}</td>
                  <td>{bom.description}</td>
                </tr>
              ))}

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

              {error && !initialLoading && (
                <tr>
                  <td colSpan="6" className="error-message">
                    {error}
                    <button onClick={handleReload} className="retry-btn">Thử lại</button>
                  </td>
                </tr>
              )}

              {!initialLoading && !loading && boms.length === 0 && !error && (
                <tr>
                  <td colSpan="6" className="no-data-message">
                    Không tìm thấy BOM nào phù hợp.
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

export default BOM;
