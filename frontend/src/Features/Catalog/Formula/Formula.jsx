import React, { useState, useEffect, useRef, useCallback } from 'react';
import CreateNewFormula from './CreateNewFormula';
import { getOperations } from '../../../api/Operation';
import { FaSyncAlt } from 'react-icons/fa';
import './Formula.css';
import FormulaDetail from './FormulaDetail/FormulaDetail';
import { useAuthPermissions } from '../../../api/UseAuthPermissions';

const PAGE_SIZE = 31;

const segmentTypeMap = {
  0 : 'Kéo dây',
  1 : 'Tráng men',
  2 : 'Cả hai'
};
const dependencyTypeMap = {
  0: 'Finish-Start',
  1: 'Start-Start',
  2: 'Finish-Finish',
  3: 'Start-Finish'
};

const Formula = ({ onBack }) => {
  // === STATE ===
  const [formulas, setFormulas] = useState([]);
  const [loading, setLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(true);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [totalItems, setTotalItems] = useState(0);
  const [isFullyLoaded, setIsFullyLoaded] = useState(false);
  const [reloadKey, setReloadKey] = useState(0);

  const { permissions, loading: permLoading, error: permError } = useAuthPermissions(7); // Giả sử pageId cho Formula là 9
  const canView = permissions.some((p) => p.canView);
  const canEdit = permissions.some((p) => p.canEdit);

  // --- THAY ĐỔI 1: State để lưu công thức đang được chọn xem chi tiết ---
  const [selectedFormula, setSelectedFormula] = useState(null); 

  // Filter
  const [showFilter, setShowFilter] = useState(false);
  const [filterValues, setFilterValues] = useState({
    code: '', productCode: '', bomCode: '', operationCode: '', availableDate: '', expiryDate: ''
  });
  const [activeFilters, setActiveFilters] = useState(filterValues);

  // Tạo mới
  const [showCreateNew, setShowCreateNew] = useState(false);

  const observer = useRef();
  const scrollContainerRef = useRef(null);

  // ... (Giữ nguyên logic Infinite Scroll: lastFormulaRef, normalizeFormula, fetchFormulas, useEffects) ...
  // === INFINITE SCROLL CALLBACK ===
  const lastFormulaRef = useCallback(
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
  // --- Thêm map loại phụ thuộc ở đầu file Formula.js (gần segmentTypeMap) ---
const dependencyTypeMap = {
  0: 'Finish-Start',
  1: 'Start-Start',
  2: 'Finish-Finish',
  3: 'Start-Finish'
};

// --- Cập nhật hàm normalizeFormula bên trong component Formula ---
const normalizeFormula = (item, index) => {
    const stt = (page - 1) * PAGE_SIZE + index + 1;
    const segments = item.operationsSegments || [];
    const boms = item.billOfMaterials || [];
    // Lấy danh sách quan hệ
    const dependencies = item.operationsSegmentDependencies || [];

    return {
      id: item.operationId || `FORM${stt.toString().padStart(3, '0')}`,
      stt,
      code: item.operationId || '--',
      name: item.description || '--',
      outputProduct: item.materialDefinitionName || '--',
      productCode: item.materialDefinitionId || '--',
      version: item.version || '--',
      availableDate: item.validFrom?.split('T')[0] || '--',
      expiryDate: item.validTo?.includes('0001') ? '--' : item.validTo?.split('T')[0],
      bomCode: boms.map(b => ({
        bomId: b.billOfMaterialId,
      })) || '--',
      operationCode: item.operationId || '--',
      
      // Xử lý Segments
      segments: segments.map(seg => ({
        segmentId: seg.segmentId,
        description: seg.description,
        type: segmentTypeMap[seg.segmentType],
        typeClass: item.operationsType
      })),

      // --- MỚI: Xử lý Relations (Quan hệ công đoạn) ---
      relations: dependencies.map(dep => ({
        id: dep.operationsSegmentDependencyId,
        fromOperation: dep.fromSegmentId,
        toOperation: dep.toSegmentId,
        description: dep.description || '--',
        // Map loại dependency từ số sang chữ
        dependencyType: dependencyTypeMap[dep.dependencyType] || dep.dependencyType, 
        // Lấy giá trị hệ số
        coefficient: dep.dependencyFactor?.valueString || 0, 
        unit: dep.valueUnitOfMeasure || ''
      })),
      
      rawData: item,
    };
  };

  const fetchFormulas = useCallback(
    async (pageNum, isAppend = false, filtersToApply) => {
      setError(null);
      const isInitial = pageNum === 1 && !isAppend;
      if (isInitial) setInitialLoading(true);
      else setLoading(true);

      const apiFilters = { ...filtersToApply }; // Copy filters

      try {
        const data = await getOperations(pageNum, PAGE_SIZE, apiFilters);
        const newItems = data.items || [];
        const mapped = newItems.map((item, index) => normalizeFormula(item, index));
        const isTrulyFinished = newItems.length === 0 || newItems.length < PAGE_SIZE;
        
        setHasMore(!isTrulyFinished);
        setIsFullyLoaded(isTrulyFinished);
        setFormulas(prev => (isAppend ? [...prev, ...mapped] : mapped));

        if (isInitial) setTotalItems(data.totalItems || 0);
      } catch (err) {
        setError('Không thể tải danh sách công thức. Vui lòng thử lại.');
        console.error('Fetch error:', err);
      } finally {
        setLoading(false);
        setInitialLoading(false);
      }
    },
    []
  );

  useEffect(() => {
    if (page > 1 && !isFullyLoaded) fetchFormulas(page, true, activeFilters);
  }, [page, isFullyLoaded, fetchFormulas, activeFilters]);

  useEffect(() => {
    setPage(1); setFormulas([]); setIsFullyLoaded(false); setHasMore(true);
    if (scrollContainerRef.current) scrollContainerRef.current.scrollTo({ top: 0, behavior: 'auto' });
    fetchFormulas(1, false, activeFilters);
  }, [activeFilters, fetchFormulas]);

  useEffect(() => {
    if (reloadKey > 0) {
      const defaultFilters = { code: '', productCode: '', bomCode: '', operationCode: '', availableDate: '', expiryDate: '' };
      setFilterValues(defaultFilters); setActiveFilters(defaultFilters);
    }
  }, [reloadKey]);

  useEffect(() => {
    return () => { if (observer.current) observer.current.disconnect(); };
  }, []);

  // === HANDLERS ===
  const handleReload = () => setReloadKey(prev => prev + 1);
  const applyFilter = () => { setActiveFilters(filterValues); setShowFilter(false); };
  
  // --- THAY ĐỔI 2: Hàm xử lý khi click nút Xem ---
  const handleViewDetail = (formula) => {
    setSelectedFormula(formula);
  };

  // --- THAY ĐỔI 3: Hàm xử lý quay lại từ trang chi tiết ---
  const handleBackFromDetail = () => {
    setSelectedFormula(null);
  };

  const handleCreateNew = () => setShowCreateNew(true);
  const handleBackFromCreate = () => {
    setShowCreateNew(false);
    handleReload();
  };

  // === RENDER TẠO MỚI ===
  if (showCreateNew) {
    return (
      <div>
        <div className="detail-nav">
          <button onClick={handleBackFromCreate} className="back-btn">Quay lại</button>
        </div>
        <CreateNewFormula />
      </div>
    );
  }

  // --- THAY ĐỔI 4: RENDER TRANG CHI TIẾT (Che lấp toàn bộ bảng) ---
  if (selectedFormula) {
    return (
      <FormulaDetail 
        formula={selectedFormula} 
        onBack={handleBackFromDetail} 
      />
    );
  }

  // === RENDER DANH SÁCH (Chỉ hiển thị khi không có selectedFormula) ===
  const SkeletonRow = () => (
    <tr className="skeleton-row">{[...Array(11)].map((_, i) => <td key={i}><div className="skeleton-line"></div></td>)}</tr>
  );

  return (
    <div className="formula-root">
      {/* TITLE */}
      <h2 className="title">Danh mục | Phương thức sản xuất</h2>
      <div className="tabs"><button className="tab-btn active">Phương thức sản xuất</button></div>

      {/* HEADER ACTIONS */}
      <div className="formula-header">
        <div className="actions">
          <button className="action-btn filter-btn" onClick={() => setShowFilter(!showFilter)}>🔍Tìm kiếm</button>
          <button className={`action-btn new-btn ${!canEdit ? 'disabled-button' : ''}`} 
          onClick={handleCreateNew} disabled={!canEdit} 
          title={!canEdit ? "Bạn không có quyền tạo mới Công thức" : "Tạo mới Công thức"}
          >➕Tạo mới</button>
          {(initialLoading || error || formulas.length > 0) && (
            <button className="action-btn reload-btn" onClick={handleReload} disabled={loading || initialLoading}>
              <FaSyncAlt className={loading || initialLoading ? 'spinning' : ''} /> Tải lại
            </button>
          )}

          {/* FILTER PANEL */}
          {showFilter && (
             <div className="filter-panel active">
                {/* ... (Giữ nguyên code phần input filter) ... */}
                {/* Rút gọn code hiển thị ở đây để tập trung vào logic chính */}
                 <div><label>Mã/Tên Phương thức sản xuất</label><input type="text" value={filterValues.code} onChange={(e) => setFilterValues({ ...filterValues, code: e.target.value })} /></div>
                 {/* ... các input khác ... */}
                 <div className="filter-actions">
                    <button onClick={applyFilter} className="search-btn">Áp dụng</button>
                    <button onClick={() => setShowFilter(false)} className="close-btn">Đóng</button>
                 </div>
             </div>
          )}
        </div>
      </div>

      <div className="summary">
        Đang hiển thị {formulas.length.toLocaleString()} / {totalItems.toLocaleString()} công thức.
        {isFullyLoaded && formulas.length > 0 && " (Đã tải hết)"}
      </div>

      {/* BẢNG CHÍNH */}
      <div className="formulas-table">
        <div className="table-container" ref={scrollContainerRef}>
          <table role="grid">
            <thead>
              <tr>
                <th>STT</th><th>Mã phương thức sản xuất</th><th>Tên phương thức sản xuất</th><th>Sản phẩm đầu ra</th><th>Mã SP</th><th>Phiên bản</th><th>Ngày khả dụng</th><th>Ngày hết hạn</th><th>Mã BOM</th><th>Mã công đoạn</th><th>Chi tiết</th>
              </tr>
            </thead>
            <tbody>
              {initialLoading && formulas.length === 0 && [...Array(PAGE_SIZE)].map((_, i) => <SkeletonRow key={i} />)}
              {formulas.map((formula, index) => (
                <tr key={formula.id} ref={index === formulas.length - 1 && hasMore ? lastFormulaRef : null} className="formula-row">
                  <td>{formula.stt}</td>
                  <td>{formula.code}</td>
                  <td>{formula.name}</td>
                  <td>{formula.outputProduct}</td>
                  <td>{formula.productCode}</td>
                  <td>{formula.version}</td>
                  <td>{formula.availableDate}</td>
                  <td>{formula.expiryDate}</td>
                  <td>{formula.bomCode.length > 0 ? formula.bomCode.map(b => b.bomId).join(', ') : '--'}</td>
                  <td>{formula.segments.length > 0 ? formula.segments.map(seg => seg.segmentId).join(', ') : formula.operationCode}</td>
                  <td>
                    {/* --- THAY ĐỔI 5: Nút View gọi hàm handleViewDetail --- */}
                    <button className="view-btn" onClick={() => handleViewDetail(formula)} title="Xem chi tiết">
                      <i className="fas fa-eye"></i>
                    </button>
                  </td>
                </tr>
              ))}
              {loading && !initialLoading && <tr><td colSpan="11" className="loading-cell"><div className="loading-spinner"><div className="spinner"></div><span>Đang tải thêm...</span></div></td></tr>}
              {error && !initialLoading && <tr><td colSpan="11" className="error-message">{error}<button onClick={handleReload} className="retry-btn">Thử lại</button></td></tr>}
              {!initialLoading && !loading && formulas.length === 0 && !error && <tr><td colSpan="11" className="no-data-message">Không tìm thấy công thức nào phù hợp.</td></tr>}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};

export default Formula;