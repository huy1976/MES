import React, { useState, useEffect, useRef, useCallback } from 'react';
import { CreateNewTypeMaterial } from '../CreateNewMaterialsAndGroupMaterials';
import { getMaterialClasses } from '../../../../api/MaterialClasses';
import './GroupMaterials.css'; // Sẽ dùng file CSS ở mục 2
import { FaSyncAlt } from 'react-icons/fa'; // <-- IMPORT ICON
import { useAuthPermissions } from '../../../../api/UseAuthPermissions';

const PAGE_SIZE = 31;

const GroupMaterials = () => {
  // === STATE ===
  const [groups, setGroups] = useState([]); // Dữ liệu hiển thị (Đã lọc từ server)
  const [loading, setLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(true);
  const [error, setError] = useState(null);

  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [totalItems, setTotalItems] = useState(0); // Tổng số items TỪ API (sau khi lọc)
  const [isFullyLoaded, setIsFullyLoaded] = useState(false);
  const [reloadKey, setReloadKey] = useState(0);

  const [selectedGroupId, setSelectedGroupId] = useState(null);

  const [showFilter, setShowFilter] = useState(false);
  const [filterValues, setFilterValues] = useState({ code: '', name: '' });
  // activeFilters là "bộ lọc" đang được API sử dụng
  const [activeFilters, setActiveFilters] = useState(filterValues);

  const { permissions, loading: permLoading, error: permError } = useAuthPermissions(7);
  const canView = permissions.some((p) => p.canView);
  const canEdit = permissions.some((p) => p.canEdit);


  // Tạo mới
  const [showCreateNew, setShowCreateNew] = useState(false);

  const observer = useRef();
  const scrollContainerRef = useRef(null);

  // === INFINITE SCROLL CALLBACK ===
  const lastGroupRef = useCallback(
    (node) => {
      if (loading || initialLoading) return;
      if (observer.current) observer.current.disconnect();

      observer.current = new IntersectionObserver(
        (entries) => {
          if (entries[0].isIntersecting && hasMore && !loading) {
            setPage((prev) => prev + 1); // Tăng PageIndex
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

  // === FETCH DATA (ĐÃ SỬA: GỬI FILTER LÊN API) ===
  const fetchGroups = useCallback(
    async (pageNum, isAppend = false, filtersToApply) => {
      setError(null);
      const isInitial = pageNum === 1 && !isAppend;
      if (isInitial) setInitialLoading(true);
      else setLoading(true);

      // === LOGIC MỚI: GỬI FILTER LÊN API ===
      // (Giả định API hỗ trợ các tham số này, tương tự getMaterials)
      const apiFilters = {
        code: filtersToApply.code || null,
        name: filtersToApply.name || null
      };
      // ====================================

      try {
        // (Giả định getMaterialClasses chấp nhận tham số filters)
        const data = await getMaterialClasses(pageNum, PAGE_SIZE, apiFilters);
        const newItems = data.items || [];

        const mapped = newItems.map((item, index) => {
          // STT dựa trên trang và page size
          const stt = (pageNum - 1) * PAGE_SIZE + index + 1;
          return {
            id: item.materialClassId || `temp-id-${stt}`, // Dùng ID thật nếu có
            stt: stt,
            code: item.materialClassId || 'N/A',
            name: item.name || 'Không xác định',
            count: item.materialDefinitions?.length || 0,
            description: item.description || 'Không có mô tả',
            properties: item.properties || [],
            rawData: item,
          };
        });

        const isTrulyFinished = newItems.length === 0 || newItems.length < PAGE_SIZE;
        setHasMore(!isTrulyFinished);
        setIsFullyLoaded(isTrulyFinished);
        
        // Cập nhật 'groups' (danh sách hiển thị)
        setGroups(prev => (isAppend ? [...prev, ...mapped] : mapped));
        
        // Chỉ cập nhật totalItems ở lần tải đầu tiên (trang 1)
        if (isInitial) {
          setTotalItems(data.totalItems || 0);
        }

      } catch (err) {
        setError('Không thể tải dữ liệu nhóm vật tư. Vui lòng thử lại.');
        console.error('Fetch error:', err);
      } finally {
        setLoading(false);
        setInitialLoading(false);
      }
    },
    [] // Không cần dependency
  );

  // === EFFECT (MỚI): GỌI KHI 'page' THAY ĐỔI (CHỈ ĐỂ TẢI THÊM) ===
  useEffect(() => {
    // Chỉ xử lý khi TẢI THÊM (cuộn)
    if (page > 1 && !isFullyLoaded) {
      fetchGroups(page, true, activeFilters); // Tải thêm với BỘ LỌC HIỆN TẠI
    }
  }, [page, isFullyLoaded, fetchGroups, activeFilters]);

  // === EFFECT (MỚI): GỌI KHI 'activeFilters' THAY ĐỔI (ĐỂ TẢI MỚI) ===
  useEffect(() => {
    // Xử lý khi TẢI MỚI (lần đầu, hoặc khi filter, hoặc khi reload)
    // Reset lại list và page, sau đó fetch
    setPage(1); 
    setGroups([]); // Xóa dữ liệu cũ
    setIsFullyLoaded(false);
    setHasMore(true);
    
    // Cuộn lên đầu
    if (scrollContainerRef.current) {
      scrollContainerRef.current.scrollTo({ top: 0, behavior: 'auto' });
    }
    
    fetchGroups(1, false, activeFilters); // Tải trang 1 với BỘ LỌC MỚI

  }, [activeFilters, fetchGroups]); // CHỈ phụ thuộc 'activeFilters'

  // === EFFECT (MỚI): GỌI KHI 'reloadKey' THAY ĐỔI (ĐỂ RESET) ===
  useEffect(() => {
    // Chỉ chạy khi reloadKey thay đổi (user nhấn Tải lại)
    if (reloadKey > 0) {
      const defaultFilters = { code: '', name: '' };
      setFilterValues(defaultFilters);
      setActiveFilters(defaultFilters); // <-- VIỆC NÀY SẼ KÍCH HOẠT useEffect[activeFilters]
    }
  }, [reloadKey]); // CHỈ phụ thuộc 'reloadKey'

  // Giữ nguyên useEffect cleanup
  useEffect(() => {
    return () => {
      if (observer.current) observer.current.disconnect();
    };
  }, []);

  // === HANDLERS ===
  const handleReload = () => {
    setReloadKey(prev => prev + 1); // Kích hoạt useEffect "RESET"
  };

  const applyFilter = () => {
    setActiveFilters(filterValues); // Kích hoạt useEffect "TẢI MỚI"
    setShowFilter(false);
  };

  const handleViewDetail = (groupId) => {
    setSelectedGroupId(selectedGroupId === groupId ? null : groupId);
  };

  const handleCreateNew = () => setShowCreateNew(true);
  const handleBack = () => {
    setShowCreateNew(false);
    // Tải lại nếu có thể đã tạo mới
    handleReload();
  };

  // === LẤY THUỘC TÍNH (giữ nguyên) ===
  const getAttributes = (group) => {
    if (!group.properties) return [];
    return group.properties.map((prop, index) => ({
      id: index + 1,
      name: prop.name || 'Thuộc tính không xác định',
      min: prop.valueString?.split(' - ')[0] || 'N/A',
      max: prop.valueString?.split(' - ')[1] || prop.valueString || 'N/A',
      unit: prop.valueUnitOfMeasure || 'None',
    }));
  };

  // === SKELETON (giống Materials) ===
  const SkeletonRow = () => (
    <tr className="skeleton-row">
      {[...Array(6)].map((_, i) => (
        <td key={i}><div className="skeleton-line"></div></td>
      ))}
    </tr>
  );

  // === RENDER TẠO MỚI (giữ nguyên) ===
  if (showCreateNew) {
    return (
      <div>
        <div className="detail-nav">
          <button onClick={handleBack} className="back-btn">Quay lại</button>
        </div>
        <CreateNewTypeMaterial />
      </div>
    );
  }

  // === MAIN LIST (Render chính) ===
  return (
    <div className="group-materials-page">
      {/* HEADER (giống Materials) */}
      <div className="group-materials-header">
        <div className="actions">
          <button className="action-btn filter-btn" onClick={() => setShowFilter(!showFilter)}>
            🔍 Tìm kiếm
          </button>
          <button className={`action-btn new-btn ${!canEdit ? 'disabled-button' : ''}`}
          onClick={handleCreateNew}
          disabled={!canEdit}
          title={!canEdit ? "Bạn không có quyền tạo mới nhóm vật tư" : "Tạo mới nhóm vật tư"}
          >
            ➕ Tạo mới
          </button>
          {(initialLoading || error || groups.length > 0) && (
            <button
              className="action-btn reload-btn"
              onClick={handleReload}
              disabled={loading || initialLoading}
            >
              <FaSyncAlt className={loading || initialLoading ? 'spinning' : ''} />
              Tải lại
            </button>
          )}

          {/* FILTER PANEL (giống Materials) */}
          {showFilter && (
            <div className="filter-panel active">
              <div>
                <label>Mã nhóm vật tư</label>
                <input
                  type="text"
                  placeholder="Nhập mã nhóm"
                  value={filterValues.code}
                  onChange={(e) => setFilterValues({ ...filterValues, code: e.target.value })}
                />
              </div>
              <div>
                <label>Tên nhóm vật tư</label>
                <input
                  type="text"
                  placeholder="Nhập tên nhóm"
                  value={filterValues.name}
                  onChange={(e) => setFilterValues({ ...filterValues, name: e.target.value })}
                />
              </div>
              <div className="filter-actions">
                <button onClick={applyFilter} className="search-btn">Tìm kiếm</button>
                <button onClick={() => setShowFilter(false)} className="close-btn">Đóng</button>
              </div>
            </div>
          )}
        </div>
      </div>

      {/* === SUMMARY (giống Materials) === */}
      <div className="summary">
        Đang hiển thị {groups.length.toLocaleString()} / {totalItems.toLocaleString()} nhóm vật tư.
        {isFullyLoaded && groups.length > 0 && " (Đã tải hết)"}
      </div>

      {/* TABLE (giống Materials) */}
      <div className="group-materials-table">
        <div className="table-container" ref={scrollContainerRef}>
          <table role="grid">
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã nhóm</th>
                <th>Tên nhóm</th>
                <th>Số lượng vật tư</th>
                <th>Mô tả</th>
                <th>Chi tiết</th>
              </tr>
            </thead>
            <tbody>
              {/* SKELETON */}
              {initialLoading && groups.length === 0 && [...Array(PAGE_SIZE)].map((_, i) => <SkeletonRow key={i} />)}

              {/* DỮ LIỆU */}
              {groups.map((group, index) => (
                <React.Fragment key={group.id}>
                  <tr
                    ref={index === groups.length - 1 && hasMore ? lastGroupRef : null}
                    className="group-row"
                  >
                    <td>{group.stt}</td>
                    <td>{group.code}</td>
                    <td>{group.name}</td>
                    <td>{group.count}</td>
                    <td className="desc-cell">{group.description}</td>
                    <td>
                      <button
                        className="view-btn"
                        onClick={() => handleViewDetail(group.id)}
                        title="Xem chi tiết"
                      >
                        <i className={`fas ${selectedGroupId === group.id ? 'fa-eye-slash' : 'fa-eye'}`}></i>
                      </button>
                    </td>
                  </tr>

                  {/* CHI TIẾT THUỘC TÍNH (giữ nguyên logic) */}
                  {selectedGroupId === group.id && (
                    <tr>
                      <td colSpan="6">
                        <div className="attributes-table">
                          <h3>Thuộc tính của {group.name}</h3>
                          <table role="grid">
                            <thead>
                              <tr>
                                <th>STT</th>
                                <th>Tên thuộc tính</th>
                                <th>Tối thiểu</th>
                                <th>Tối đa</th>
                                <th>Đơn vị</th>
                              </tr>
                            </thead>
                            <tbody>
                              {getAttributes(group).length > 0 ? (
                                getAttributes(group).map((attr) => (
                                  <tr key={attr.id}>
                                    <td>{attr.id}</td>
                                    <td>{attr.name}</td>
                                    <td>{attr.min}</td>
                                    <td>{attr.max}</td>
                                    <td>{attr.unit}</td>
                                  </tr>
                                ))
                              ) : (
                                <tr>
                                  <td colSpan="5" className="no-attributes">
                                    Không có thuộc tính
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

              {/* LOADING MORE (SPINNER) */}
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

              {/* END OF DATA */}
              {isFullyLoaded && groups.length > 0 && (
                <tr>
                  <td colSpan="6" className="end-message">
                    {/* (Đã dời thông báo lên Summary) */}
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
              {!initialLoading && !loading && groups.length === 0 && !error && (
                <tr>
                  <td colSpan="6" className="no-data-message">
                    Không tìm thấy nhóm vật tư nào phù hợp.
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

export default GroupMaterials;