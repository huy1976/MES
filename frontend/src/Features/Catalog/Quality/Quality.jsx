import React, { useState, useEffect, useRef, useCallback } from 'react';
import CreateNewQuality from './CreateNewQuality';
// Import thêm hàm API mới getMaterialDefinitionIdsByFinishedProductTP
import { 
  getQualityStandards, 
  createMaterialInfoQualityStandard, 
  getMaterialDefinitionIdsByFinishedProductTP 
} from '../../../api/QualityStandard';
import { FaSyncAlt, FaEdit, FaEye, FaEyeSlash } from 'react-icons/fa';
import './Quality.css';
import { useAuthPermissions } from '../../../api/UseAuthPermissions';

const PAGE_SIZE = 31;

const Quality = () => {
  const [qualities, setQualities] = useState([]);
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

  const [showFilter, setShowFilter] = useState(false);
  const [filterValues, setFilterValues] = useState({ code: '', productCode: '' });
  const [activeFilters, setActiveFilters] = useState(filterValues);

  const [expandedRow, setExpandedRow] = useState(null);
  const [showCreateNew, setShowCreateNew] = useState(false);

  // --- NEW: State cho Modal Edit Material ---
  const [showEditModal, setShowEditModal] = useState(false);
  const [editingItem, setEditingItem] = useState(null);
  const [savingMaterial, setSavingMaterial] = useState(false);
  
  // State mới để quản lý danh sách chọn
  const [availableMaterials, setAvailableMaterials] = useState([]); // Danh sách từ API
  const [selectedMaterialIds, setSelectedMaterialIds] = useState([]); // Danh sách ID đã tick chọn
  const [loadingMaterials, setLoadingMaterials] = useState(false); // Loading khi lấy list
  const [searchTerm, setSearchTerm] = useState(''); // Từ khóa tìm kiếm trong modal
  // ------------------------------------------

  const observer = useRef();
  const scrollContainerRef = useRef(null);

  // ... (Giữ nguyên logic scroll/pagination cũ) ...
  const lastQualityRef = useCallback(
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

  const normalizeQuality = (item, index) => {
    const stt = (page - 1) * PAGE_SIZE + index + 1;
    const details = (item.properties || []).map((prop, i) => ({
      id: i + 1,
      name: prop.name || 'N/A',
      value: prop.valueString || 'N/A',
      unit: prop.valueUnitOfMeasure || 'None',
      description: prop.description || '',
    }));

    const productCode = (item.materialDefinitions || []).join(', ') || 'N/A';
    const status = item.status === 8 ? 'Khả dụng' : 'Không đạt';

    return {
      id: item.qualityStandardId || `QLT${stt.toString().padStart(3, '0')}`,
      stt,
      code: item.qualityStandardId || 'N/A',
      productCode,
      status,
      statusClass: status === 'Khả dụng' ? 'status-ok' : 'status-fail',
      startDate: item.validFrom ? new Date(item.validFrom).toLocaleDateString('vi-VN') : 'N/A',
      endDate: item.validTo && !item.validTo.startsWith('0001') ? new Date(item.validTo).toLocaleDateString('vi-VN') : 'Vô thời hạn',
      note: item.description || 'Không có ghi chú',
      details,
      rawData: item,
    };
  };

  const fetchQualities = useCallback(async (pageNum, isAppend = false, filtersToApply) => {
    setError(null);
    const isInitial = pageNum === 1 && !isAppend;
    if (isInitial) setInitialLoading(true);
    else setLoading(true);

    const apiFilters = {
      code: filtersToApply.code || null,
      productCode: filtersToApply.productCode || null,
    };

    try {
      const data = await getQualityStandards(pageNum, PAGE_SIZE, apiFilters);
      const newItems = data.items || [];
      const mapped = newItems.map((item, index) => normalizeQuality(item, index));
      const finished = newItems.length === 0 || newItems.length < PAGE_SIZE;

      setHasMore(!finished);
      setIsFullyLoaded(finished);
      setQualities((prev) => (isAppend ? [...prev, ...mapped] : mapped));
      if (isInitial) setTotalItems(data.totalItems || 0);
    } catch (err) {
      console.error('Fetch Error:', err);
      setError('Không thể tải tiêu chuẩn chất lượng. Vui lòng thử lại.');
    } finally {
      setLoading(false);
      setInitialLoading(false);
    }
  }, []);

  useEffect(() => {
    if (page > 1 && !isFullyLoaded) {
      fetchQualities(page, true, activeFilters);
    }
  }, [page, isFullyLoaded, fetchQualities, activeFilters]);

  useEffect(() => {
    setPage(1);
    setQualities([]);
    setIsFullyLoaded(false);
    setHasMore(true);
    if (scrollContainerRef.current) scrollContainerRef.current.scrollTo({ top: 0 });
    fetchQualities(1, false, activeFilters);
  }, [activeFilters, fetchQualities]);

  useEffect(() => {
    if (reloadKey > 0) {
      const defaultFilters = { code: '', productCode: '' };
      setFilterValues(defaultFilters);
      setActiveFilters(defaultFilters);
      setExpandedRow(null);
    }
  }, [reloadKey]);

  useEffect(() => {
    return () => {
      if (observer.current) observer.current.disconnect();
    };
  }, []);

  const handleReload = () => setReloadKey((p) => p + 1);
  const applyFilter = () => { setActiveFilters(filterValues); setShowFilter(false); };
  const toggleExpandRow = (id) => setExpandedRow(expandedRow === id ? null : id);
  const handleCreateNew = () => setShowCreateNew(true);
  const handleBack = () => { setShowCreateNew(false); setExpandedRow(null); handleReload(); };

  // --- NEW: Các hàm xử lý Edit Material ---

  // Hàm lấy danh sách nguyên vật liệu từ API
  const fetchMaterialsList = async () => {
    setLoadingMaterials(true);
    try {
      const data = await getMaterialDefinitionIdsByFinishedProductTP();
      setAvailableMaterials(data || []);
    } catch (err) {
      console.error("Lỗi lấy danh sách Material:", err);
      alert("Không thể tải danh sách sản phẩm.");
    } finally {
      setLoadingMaterials(false);
    }
  };

  // 1. Mở Modal
  const handleEditClick = (quality) => {
    setEditingItem(quality);
    
    // Lấy các mã đã có sẵn trong quality để tick sẵn vào checkbox
    const currentIds = quality.rawData.materialDefinitions || [];
    setSelectedMaterialIds(currentIds);
    
    setSearchTerm(''); // Reset ô tìm kiếm
    setShowEditModal(true);
    
    // Gọi API lấy danh sách chọn
    fetchMaterialsList();
  };

  // 2. Đóng Modal
  const handleCloseModal = () => {
    setShowEditModal(false);
    setEditingItem(null);
    setSelectedMaterialIds([]);
    setAvailableMaterials([]);
    setSavingMaterial(false);
  };

  // 3. Xử lý khi tick vào checkbox
  const handleCheckboxChange = (materialId) => {
    setSelectedMaterialIds(prev => {
      if (prev.includes(materialId)) {
        // Nếu đã có thì bỏ chọn
        return prev.filter(id => id !== materialId);
      } else {
        // Chưa có thì thêm vào
        return [...prev, materialId];
      }
    });
  };

  // 4. Lọc danh sách hiển thị theo ô tìm kiếm
  const filteredMaterials = availableMaterials.filter(item => {
    const searchLower = searchTerm.toLowerCase();
    return (
      (item.materialId && item.materialId.toLowerCase().includes(searchLower)) ||
      (item.materialName && item.materialName.toLowerCase().includes(searchLower))
    );
  });

  // 5. Gọi API lưu dữ liệu (Gửi từng request một)
  const handleSaveMaterials = async () => {
    if (!editingItem) return;

    setSavingMaterial(true);
    try {
      // Yêu cầu: Gửi lần lượt từng materialId được chọn
      // Chúng ta sử dụng Promise.all để gửi song song (nhanh hơn) 
      // hoặc vòng lặp for (chậm hơn nhưng tuần tự). 
      // Dưới đây dùng Promise.all để tối ưu hiệu suất nhưng vẫn đảm bảo đúng format payload từng request.
      
      const requests = selectedMaterialIds.map(matId => {
        const payload = {
          qualityStandardId: editingItem.rawData.qualityStandardId,
          materialDefinitionIds: [matId] // Mảng chỉ chứa 1 phần tử
        };
        
        console.log(`Đang gửi payload cho ${matId}:`, payload);
        return createMaterialInfoQualityStandard(payload);
      });

      await Promise.all(requests);
      
      alert(`Đã cập nhật ${selectedMaterialIds.length} mã sản phẩm thành công!`);
      handleCloseModal();
      handleReload(); 
    } catch (error) {
      console.error('Update Material Error:', error);
      alert('Đã có lỗi xảy ra khi cập nhật một số mã sản phẩm.');
    } finally {
      setSavingMaterial(false);
    }
  };
  // ----------------------------------------

  const SkeletonRow = () => (
    <tr className="skeleton-row">
      {[...Array(8)].map((_, i) => <td key={i}><div className="skeleton-line"></div></td>)}
    </tr>
  );

  if (showCreateNew) {
    return (
      <div>
        <div className="detail-nav">
          <button onClick={handleBack} className="back-btn">&larr; Quay lại danh sách</button>
        </div>
        <CreateNewQuality />
      </div>
    );
  }

  return (
    <div className="quality-root">
      {/* ... (Phần Header, Filter, Tabs giữ nguyên) ... */}
      <h2 className="title">Danh mục | Tiêu chuẩn chất lượng</h2>
      <div className="tabs"><button className="tab-btn active">Tiêu chuẩn chất lượng</button></div>
      <div className="quality-header">
         <div className="actions">
            <button className="action-btn filter-btn" onClick={() => setShowFilter(!showFilter)}>🔍Tìm kiếm</button>
            <button className={`action-btn new-btn ${!canEdit ? "disabled-button" : ""}`}
            disabled={!canEdit}
            title={!canEdit ? "Bạn không có quyền tạo tiêu chuẩn chất lượng" : "Tạo mới tiêu chuẩn chất lượng"}
            onClick={handleCreateNew}>➕Tạo mới</button>
            {(initialLoading || error || qualities.length > 0) && (
              <button className="action-btn reload-btn" onClick={handleReload} disabled={loading || initialLoading}>
                <FaSyncAlt className={loading || initialLoading ? 'spinning' : ''} /> Tải lại
              </button>
            )}
            
            {showFilter && (
                <div className="filter-panel active">
                    <div>
                        <label>Mã/Mô tả tiêu chuẩn</label>
                        <input type="text" placeholder="Tìm kiếm theo Mã hoặc Mô tả" value={filterValues.code} onChange={(e) => setFilterValues({ ...filterValues, code: e.target.value })} />
                    </div>
                    <div>
                        <label>Mã sản phẩm</label>
                        <input type="text" placeholder="Tìm theo Mã sản phẩm" value={filterValues.productCode} onChange={(e) => setFilterValues({ ...filterValues, productCode: e.target.value })} />
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
        Đang hiển thị {qualities.length.toLocaleString()} / {totalItems.toLocaleString()} tiêu chuẩn chất lượng.
        {isFullyLoaded && qualities.length > 0 && ' (Đã tải hết)'}
      </div>

      <div className="qualities-table">
        <div className="table-container" ref={scrollContainerRef}>
          <table>
            <thead>
              <tr>
                <th>STT</th><th>Mã tiêu chuẩn</th><th>Mã sản phẩm</th><th>Tình trạng</th><th>Thời gian áp dụng</th><th>Thời gian kết thúc</th><th>Ghi chú</th><th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {initialLoading && qualities.length === 0 && [...Array(PAGE_SIZE)].map((_, i) => <SkeletonRow key={i} />)}
              
              {qualities.map((quality, index) => (
                <React.Fragment key={quality.id}>
                  <tr ref={index === qualities.length - 1 && hasMore ? lastQualityRef : null} className="quality-row">
                    <td>{quality.stt}</td>
                    <td>{quality.code}</td>
                    <td>{quality.productCode}
                      <button className="edit-btn" title="Sửa Mã sản phẩm" onClick={() => handleEditClick(quality)}>
                        <FaEdit />
                      </button>
                    </td>
                    <td><span className={`status-badge ${quality.statusClass}`}>{quality.status}</span></td>
                    <td>{quality.startDate}</td>
                    <td>{quality.endDate}</td>
                    <td>{quality.note}</td>
                    <td>
                      <button className="view-btn" onClick={() => toggleExpandRow(quality.id)} title="Xem chi tiết">
                        {expandedRow === quality.id ? <FaEyeSlash /> : <FaEye />}
                      </button>
                    </td>
                  </tr>
                  
                  {expandedRow === quality.id && (
                    <tr>
                      <td colSpan="8">
                        <div className="details-table">
                          <h3>Chi tiết tiêu chuẩn: {quality.code}</h3>
                           <table className="sub-table">
                            <thead><tr><th>STT</th><th>Tên tiêu chuẩn</th><th>Giá trị</th><th>Đơn vị</th><th>Mô tả</th></tr></thead>
                            <tbody>
                              {quality.details.length > 0 ? (
                                quality.details.map((detail) => (
                                  <tr key={detail.id}>
                                    <td>{detail.id}</td><td>{detail.name}</td><td>{detail.value}</td><td>{detail.unit}</td><td>{detail.description}</td>
                                  </tr>
                                ))
                              ) : (
                                <tr><td colSpan="5" style={{ textAlign: 'center', fontStyle: 'italic', color: '#777' }}>Không có chi tiết tiêu chuẩn.</td></tr>
                              )}
                            </tbody>
                          </table>
                        </div>
                      </td>
                    </tr>
                  )}
                </React.Fragment>
              ))}
              
               {/* ... (Các phần loading row, error row giữ nguyên) ... */}
               {loading && !initialLoading && (
                <tr><td colSpan="8" className="loading-cell"><div className="loading-spinner"><div className="spinner"></div><span>Đang tải thêm...</span></div></td></tr>
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* --- NEW: Modal Edit Material (CHECKBOX VERSION) --- */}
      {showEditModal && (
        <div className="modal-overlay">
          <div className="modal-content">
            <div className="modal-header">
              Chọn Mã sản phẩm cho: {editingItem?.code}
            </div>
            
            <div className="modal-body">
              {/* Ô tìm kiếm */}
              <input 
                type="text" 
                className="search-input"
                placeholder="Tìm kiếm mã hoặc tên sản phẩm..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
              />

              {/* Danh sách Checkbox */}
              <div className="material-list-container">
                {loadingMaterials ? (
                  <div className="modal-loading">Đang tải danh sách...</div>
                ) : filteredMaterials.length > 0 ? (
                  filteredMaterials.map((item) => (
                    <div key={item.materialId} className="material-item">
                      <input 
                        type="checkbox" 
                        id={`mat-${item.materialId}`}
                        checked={selectedMaterialIds.includes(item.materialId)}
                        onChange={() => handleCheckboxChange(item.materialId)}
                      />
                      <label htmlFor={`mat-${item.materialId}`}>
                        <span className="mat-id">{item.materialId}</span>
                        <span className="mat-name">{item.materialName}</span>
                      </label>
                    </div>
                  ))
                ) : (
                  <div className="modal-loading">Không tìm thấy sản phẩm nào.</div>
                )}
              </div>
              
              <div style={{ marginTop: '10px', fontSize: '13px', color: '#555' }}>
                 Đã chọn: <b>{selectedMaterialIds.length}</b> sản phẩm.
              </div>
            </div>

            <div className="modal-footer">
              <button 
                className="cancel-btn" 
                onClick={handleCloseModal}
                disabled={savingMaterial}
              >
                Hủy bỏ
              </button>
              <button 
                className="save-btn" 
                onClick={handleSaveMaterials}
                disabled={savingMaterial}
              >
                {savingMaterial ? 'Đang lưu...' : 'Lưu thay đổi'}
              </button>
            </div>
          </div>
        </div>
      )}
      {/* -------------------------------- */}

    </div>
  );
};

export default Quality;