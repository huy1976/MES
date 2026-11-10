import React, { useState, useEffect, useRef, useCallback, useMemo } from 'react';
import MaterialDetail from '../MaterialDetail/MaterialDetail';
import { CreateNewMaterial } from '../CreateNewMaterialsAndGroupMaterials';
import { getMaterials } from '../../../../api/Materials';
import './Materials.css';
import { FaSyncAlt } from 'react-icons/fa';
import { useAuthPermissions } from '../../../../api/UseAuthPermissions';

const PAGE_SIZE = 31;

const Materials = () => {
  // === STATE ===
  const [materials, setMaterials] = useState([]);
  const [loading, setLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(true);
  const [error, setError] = useState(null);

  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [totalItems, setTotalItems] = useState(0);
  const [isFullyLoaded, setIsFullyLoaded] = useState(false);
  const [reloadKey, setReloadKey] = useState(0);

  const [selectedMaterial, setSelectedMaterial] = useState(null);

  const { permissions, loading: permLoading, error: permError } = useAuthPermissions(7);
  const canView = permissions.some((p) => p.canView);
  const canEdit = permissions.some((p) => p.canEdit);

  // Filter Client-side
  const [showFilter, setShowFilter] = useState(false);
  const [filterValues, setFilterValues] = useState({
    search: '',
    type: 'Tất cả'
  });

  const [showCreateNew, setShowCreateNew] = useState(false);
  const [savedCreateMaterialState, setSavedCreateMaterialState] = useState({
    material: { type: '', code: '', name: '', unit: 'Kg', properties: [] },
    materialList: [],
  });

  const observer = useRef();
  const scrollContainerRef = useRef(null);

  // === HELPER: ÁNH XẠ DỮ LIỆU TỪ API SANG TIẾNG VIỆT ===
  const mapMaterialType = (rawType) => {
    if (!rawType) return 'Không xác định';
    const normalized = rawType.toLowerCase().trim();
    
    if (normalized === 'nguyen vat lieu') return 'Nguyên vật liệu';
    if (normalized === 'thanh pham') return 'Thành phẩm';
    if (normalized === 'ban thanh pham') return 'Bán thành phẩm';
    
    return rawType; // Trả về gốc nếu không khớp case nào
  };

  // === HELPER: MÀU SẮC LOẠI VẬT TƯ (Cập nhật check theo tiếng Việt có dấu) ===
  const getTypeStyle = (typeName) => {
    // Chuẩn hóa chuỗi để so sánh (lowercase)
    const normalized = typeName ? typeName.toLowerCase().trim() : '';

    if (normalized.includes('nguyên vật liệu')) {
      // Màu Đỏ (Red)
      return { 
        backgroundColor: '#fee2e2', 
        color: '#b91c1c', 
        border: '1px solid #b91c1c' 
      };
    } else if (normalized.includes('bán thành phẩm')) {
      // Màu Xanh Dương (Blue)
      return { 
        backgroundColor: '#e0f2fe', 
        color: '#0369a1', 
        border: '1px solid #0369a1' 
      };
    } else if (normalized.includes('thành phẩm')) {
      // Màu Xanh Lá (Green)
      return { 
        backgroundColor: '#dcfce7', 
        color: '#15803d', 
        border: '1px solid #15803d' 
      };
    } else {
      // Mặc định (Gray)
      return { 
        backgroundColor: '#f3f4f6', 
        color: '#374151', 
        border: '1px solid #9ca3af' 
      };
    }
  };

  // === INFINITE SCROLL CALLBACK ===
  const lastMaterialRef = useCallback(
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

  // === FETCH API ===
  const fetchMaterials = useCallback(
    async (pageNum, isAppend = false) => {
      setError(null);
      const isInitial = pageNum === 1 && !isAppend;
      if (isInitial) setInitialLoading(true);
      else setLoading(true);

      const apiFilters = {
        search: null,
        materialClassName: null
      };

      try {
        const data = await getMaterials(pageNum, PAGE_SIZE, apiFilters);
        const newItems = data.items || [];

        const mapped = newItems.map((item, index) => {
          // Xử lý thông số
          const spec = item.properties?.length > 0
            ? item.properties
                .map(prop => {
                  const desc = prop.description ? `${prop.description}: ` : '';
                  return `${desc}${prop.valueString || ''}`;
                })
                .join(', ')
            : 'Không có thông số';
          
          return {
            id: item.materialDefinitionId || `temp-id-${(pageNum - 1) * PAGE_SIZE + index + 1}`,
            // SỬ DỤNG HÀM MAP ĐỂ HIỂN THỊ TIẾNG VIỆT
            type: mapMaterialType(item.materialClassName),
            code: item.materialDefinitionId || 'N/A',
            name: item.name || 'Không tên',
            unit: item.primaryUnit || 'N/A',
            spec,
            propertiesCount: item.properties?.length || 0,
            operationsCount: item.operationsDefinitions?.length || 0,
            standardsCount: item.qualityStandards?.length || 0,
            billOfMaterials: item.billOfMaterials || [],
            operationsDefinitions: item.operationsDefinitions || [],
            qualityStandards: item.qualityStandards || [],
            rawData: item,
          };
        });

        const isTrulyFinished = newItems.length === 0 || newItems.length < PAGE_SIZE;
        setHasMore(!isTrulyFinished);
        setIsFullyLoaded(isTrulyFinished);
        setMaterials(prev => (isAppend ? [...prev, ...mapped] : mapped));

        if (isInitial) {
          setTotalItems(data.totalItems || 0);
        }

      } catch (err) {
        setError('Không thể tải dữ liệu. Vui lòng thử lại.');
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
      fetchMaterials(page, true);
    }
  }, [page, isFullyLoaded, fetchMaterials]);

  // === EFFECT: RELOAD ===
  useEffect(() => {
    setPage(1);
    setMaterials([]); 
    setIsFullyLoaded(false);
    setHasMore(true);

    if (scrollContainerRef.current) {
      scrollContainerRef.current.scrollTo({ top: 0, behavior: 'auto' });
    }

    fetchMaterials(1, false);
  }, [reloadKey, fetchMaterials]);

  // === FILTER LOGIC ===
  // 1. Tự động lấy danh sách loại vật tư (lúc này đã là tiếng Việt) để đưa vào dropdown
  const uniqueTypes = useMemo(() => {
    return ['Tất cả', ...new Set(materials.map(m => m.type))];
  }, [materials]);

  // 2. Lọc dữ liệu
  const filteredMaterials = useMemo(() => {
    return materials.filter((item) => {
      // Lọc theo loại
      if (filterValues.type !== 'Tất cả') {
        if (item.type !== filterValues.type) return false;
      }
      // Lọc theo từ khóa
      if (filterValues.search) {
        const keyword = filterValues.search.toLowerCase();
        const codeMatch = item.code?.toLowerCase().includes(keyword);
        const nameMatch = item.name?.toLowerCase().includes(keyword);
        if (!codeMatch && !nameMatch) return false;
      }
      return true;
    });
  }, [materials, filterValues]);

  // === HANDLERS ===
  const handleReload = () => setReloadKey(prev => prev + 1);

  const applyFilter = () => setShowFilter(false);
  
  const clearFilter = () => setFilterValues({ search: '', type: 'Tất cả' });

  const handleViewDetail = (material) => setSelectedMaterial(material);
  const handleBackToList = () => setSelectedMaterial(null);
  const handleCreateNew = () => setShowCreateNew(true);
  const handleBack = () => setShowCreateNew(false);
  const handleSaveCreateMaterialState = (state) => setSavedCreateMaterialState(state);

  const SkeletonRow = () => (
    <tr className="skeleton-row">
      {[...Array(9)].map((_, i) => (
        <td key={i}><div className="skeleton-line"></div></td>
      ))}
    </tr>
  );

  // === RENDER ===
  if (selectedMaterial) {
    return (
      <div>
        <div className="detail-nav">
          <button onClick={handleBackToList} className="back-btn">
            Quay lại danh sách
          </button>
        </div>
        <MaterialDetail material={selectedMaterial} />
      </div>
    );
  }

  if (showCreateNew) {
    return (
      <div>
        <div className="detail-nav">
          <button onClick={handleBack} className="back-btn">Quay lại</button>
        </div>
        <CreateNewMaterial 
          savedState={savedCreateMaterialState}
          onSaveState={handleSaveCreateMaterialState}
        />
      </div>
    );
  }

  return (
    <div className="materials-page">
      {/* HEADER */}
      <div className="materials-header">
        <div className="actions">
          <button className={`action-btn filter-btn ${showFilter ? 'active' : ''}`} onClick={() => setShowFilter(!showFilter)}>
            🔍 Tìm kiếm
          </button>
          <button className={`action-btn new-btn ${!canEdit ? 'disabled-button' : ''}`}
          onClick={handleCreateNew}
          disabled={!canEdit}
          title={!canEdit ? "Bạn không có quyền tạo mới vật tư" : "Tạo mới vật tư"}
          >
            ➕ Tạo mới
          </button>
          
          {(initialLoading || error || materials.length > 0) && (
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
                <label>Mã hoặc tên vật tư</label>
                <input
                  type="text"
                  placeholder="Nhập mã hoặc tên..."
                  value={filterValues.search}
                  onChange={(e) => setFilterValues({ ...filterValues, search: e.target.value })}
                />
              </div>
              <div>
                <label>Loại vật tư</label>
                <select
                  value={filterValues.type}
                  onChange={(e) => setFilterValues({ ...filterValues, type: e.target.value })}
                >
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
        {filteredMaterials.length !== materials.length 
          ? `Tìm thấy ${filteredMaterials.length} trên ${materials.length} vật tư đã tải.` 
          : `Đang hiển thị ${materials.length.toLocaleString()} vật tư.`
        }
        {isFullyLoaded && materials.length > 0 && " (Đã tải hết)"}
      </div>

      {/* TABLE */}
      <div className="materials-table">
        <div className="table-container" ref={scrollContainerRef}>
          <table role="grid">
            <thead>
              <tr>
                <th>STT</th>
                <th style={{textAlign: 'center'}}>Loại vật tư</th>
                <th>Mã vật tư</th>
                <th>Tên vật tư</th>
                <th>Đơn vị</th>
                <th style={{textAlign: 'center'}}>Số thông số</th>
                <th style={{textAlign: 'center'}}>Số công thức</th>
                <th style={{textAlign: 'center'}}>Số TCCL</th>
                <th style={{textAlign: 'center'}}>Chi tiết</th>
              </tr>
            </thead>
            <tbody>
              {/* SKELETON */}
              {initialLoading && materials.length === 0 && [...Array(PAGE_SIZE)].map((_, i) => <SkeletonRow key={i} />)}

              {/* DATA */}
              {filteredMaterials.map((material, index) => (
                <tr
                  key={material.id}
                  ref={index === filteredMaterials.length - 1 ? lastMaterialRef : null}
                  className="material-row"
                >
                  <td>{index + 1}</td>
                  <td style={{textAlign: 'center'}}>
                    <span 
                      className="status-badge-styled" 
                      style={getTypeStyle(material.type)}
                    >
                      {material.type}
                    </span>
                  </td>
                  <td>{material.code}</td>
                  <td><b>{material.name}</b></td>
                  <td>{material.unit}</td>
                  <td style={{textAlign: 'center'}}>{material.propertiesCount}</td>
                  <td style={{textAlign: 'center'}}>{material.operationsCount}</td>
                  <td style={{textAlign: 'center'}}>{material.standardsCount}</td>
                  <td style={{textAlign: 'center'}}>
                    <button
                      className="view-btn"
                      onClick={() => handleViewDetail(material)}
                      title="Xem chi tiết"
                    >
                      <i className="fas fa-eye"></i>
                    </button>
                  </td>
                </tr>
              ))}

              {/* LOADING & MESSAGES */}
              {loading && !initialLoading && (
                <tr>
                  <td colSpan="9" className="loading-cell">
                    <div className="loading-spinner">
                      <div className="spinner"></div>
                      <span>Đang tải thêm...</span>
                    </div>
                  </td>
                </tr>
              )}

              {error && !initialLoading && (
                <tr>
                  <td colSpan="9" className="error-message">
                    {error} <button onClick={handleReload}>Thử lại</button>
                  </td>
                </tr>
              )}

              {!initialLoading && filteredMaterials.length === 0 && !error && (
                <tr>
                  <td colSpan="9" className="no-data-message">
                    {materials.length === 0 
                      ? "Chưa có dữ liệu vật tư."
                      : "Không tìm thấy vật tư nào phù hợp."}
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

export default Materials;