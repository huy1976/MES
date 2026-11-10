import React, { useState, useEffect, useRef, useCallback } from 'react';
import { CreateMachineCapacity } from '../Create';
import { getEquipmentCapabilities } from '../../../../api/EquipmentCapabilities';
import { FaSyncAlt } from 'react-icons/fa';
import './MachineCapacity.css';
import { useAuthPermissions } from '../../../../api/UseAuthPermissions';

const PAGE_SIZE = 31;

const capabilityTypeMap = {
    0: 'Sẵn sàng',
    1: 'Không thể sử dụng',
    2: 'Đã cam kết',
}

const MachineCapacity = () => {
  // === STATE ===
  const [groups, setGroups] = useState([]); // Dữ liệu hiển thị (đã lọc)
  const [loading, setLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(true);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [totalItems, setTotalItems] = useState(0);
  const [isFullyLoaded, setIsFullyLoaded] = useState(false);
  const [reloadKey, setReloadKey] = useState(0);

  // Xem chi tiết
  const [selectedGroupId, setSelectedGroupId] = useState(null);

  // Filter
  const [showFilter, setShowFilter] = useState(false);
  const [filterValues, setFilterValues] = useState({ code: '', equipment: '' });
  const [activeFilters, setActiveFilters] = useState(filterValues);

  // Tạo mới
  const [showCreateNew, setShowCreateNew] = useState(false);

  const observer = useRef();
  const scrollContainerRef = useRef(null);

  const { permissions, loading: permLoading, error: permError } = useAuthPermissions(7);
  const canView = permissions.some((p) => p.canView);
  const canEdit = permissions.some((p) => p.canEdit);

  // === INFINITE SCROLL CALLBACK ===
  const lastGroupRef = useCallback(
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

  // === FETCH DATA (SERVER-SIDE FILTER) ===
  const fetchGroups = useCallback(
    async (pageNum, isAppend = false, filtersToApply) => {
      setError(null);
      const isInitial = pageNum === 1 && !isAppend;
      if (isInitial) setInitialLoading(true);
      else setLoading(true);

      const apiFilters = {
        code: filtersToApply.code || null,
        equipment: filtersToApply.equipment || null
      };

      try {
        const data = await getEquipmentCapabilities(pageNum, PAGE_SIZE, apiFilters);
        const newItems = data.items || [];

        const mapped = newItems.map((item, index) => {
  const stt = (pageNum - 1) * PAGE_SIZE + index + 1;
  return {
    id: item.equipmentCapabilityId || `EQP${stt.toString().padStart(3, '0')}`,
    stt,
    code: item.equipmentCapabilityId || `EQP${stt.toString().padStart(3, '0')}`,
    equipment: item.equipmentName || '--',
    type: capabilityTypeMap[item.capabilityType] || 'Không xác định',
    quantity: item.quantity?.valueString || '--',
    unit: item.valueUnitOfMeasure || '--',
    startTime: item.validFrom?.split('T')[0] || '--',
    
    // --- DÒNG ĐÃ CHỈNH SỬA ---
    endTime: item.validTo?.startsWith('0001-01-01') ? '--' : (item.validTo?.split('T')[0] || '--'),
    // -------------------------

    note: item.description || '--',
    equipmentCapability: item,
    rawData: item,
  };
});

        const isTrulyFinished = newItems.length === 0 || newItems.length < PAGE_SIZE;
        setHasMore(!isTrulyFinished);
        setIsFullyLoaded(isTrulyFinished);

        setGroups(prev => (isAppend ? [...prev, ...mapped] : mapped));

        if (isInitial) {
          setTotalItems(data.totalItems || 0);
        }
      } catch (err) {
        setError('Không thể tải dữ liệu năng lực máy. Vui lòng thử lại.');
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
      fetchGroups(page, true, activeFilters);
    }
  }, [page, isFullyLoaded, fetchGroups, activeFilters]);

  // === EFFECT: TẢI MỚI KHI FILTER THAY ĐỔI ===
  useEffect(() => {
    setPage(1);
    setGroups([]);
    setIsFullyLoaded(false);
    setHasMore(true);

    if (scrollContainerRef.current) {
      scrollContainerRef.current.scrollTo({ top: 0, behavior: 'auto' });
    }

    fetchGroups(1, false, activeFilters);
  }, [activeFilters, fetchGroups]);

  // === EFFECT: RESET KHI RELOAD ===
  useEffect(() => {
    if (reloadKey > 0) {
      const defaultFilters = { code: '', equipment: '' };
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

  const handleViewDetail = (groupId) => {
    setSelectedGroupId(selectedGroupId === groupId ? null : groupId);
  };

  const handleCreateNew = () => setShowCreateNew(true);
  const handleBack = () => {
    setShowCreateNew(false);
    handleReload();
  };

  // === LẤY CAPACITIES ===
  const getCapacities = (group) => {
    return (group.equipmentCapability?.properties || []).map((prop, index) => ({
      id: index + 1,
      name: prop.name || 'Không tên',
      value: prop.valueString || '--',
      unit: prop.valueUnitOfMeasure === 'None' ? '' : prop.valueUnitOfMeasure || '--',
      description: prop.description || '--',
    }));
  };

  // === SKELETON ROW ===
  const SkeletonRow = () => (
    <tr className="skeleton-row">
      {[...Array(10)].map((_, i) => (
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
        <CreateMachineCapacity />
      </div>
    );
  }

  // === RENDER DANH SÁCH ===
  return (
    <div className="machine-capacity-page">
      {/* HEADER */}
      <div className="machine-capacity-header">
        <div className="actions">
          <button className="action-btn filter-btn" onClick={() => setShowFilter(!showFilter)}>
           🔍Tìm kiếm
          </button>
          <button className={`action-btn new-btn ${!canEdit ? 'disabled-button' : ''}`}
          disabled={!canEdit}
          title={!canEdit ? "Bạn không có quyền tạo mới năng lực máy" : "Tạo mới năng lực máy"}
          onClick={handleCreateNew}>
            ➕Tạo mới
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

          {/* FILTER PANEL */}
          {showFilter && (
            <div className="filter-panel active">
              <div>
                <label>Mã năng lực</label>
                <input
                  type="text"
                  placeholder="Nhập mã năng lực"
                  value={filterValues.code}
                  onChange={(e) => setFilterValues({ ...filterValues, code: e.target.value })}
                />
              </div>
              <div>
                <label>Thiết bị</label>
                <input
                  type="text"
                  placeholder="Nhập tên thiết bị"
                  value={filterValues.equipment}
                  onChange={(e) => setFilterValues({ ...filterValues, equipment: e.target.value })}
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

      {/* SUMMARY */}
      <div className="summary">
        Đang hiển thị {groups.length.toLocaleString()} / {totalItems.toLocaleString()} năng lực máy.
        {isFullyLoaded && groups.length > 0 && " (Đã tải hết)"}
      </div>

      {/* TABLE */}
      <div className="machine-capacity-table">
        <div className="table-container" ref={scrollContainerRef}>
          <table role="grid">
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã năng lực</th>
                <th>Thiết bị</th>
                <th>Loại năng lực</th>
                <th>Số lượng</th>
                <th>Đơn vị</th>
                <th>Thời gian bắt đầu</th>
                <th>Thời gian kết thúc</th>
                <th>Ghi chú</th>
                <th>Xem thêm</th>
              </tr>
            </thead>
            <tbody>
              {/* SKELETON */}
              {initialLoading && groups.length === 0 && [...Array(PAGE_SIZE)].map((_, i) => <SkeletonRow key={i} />)}

              {/* DATA */}
              {groups.map((group, index) => (
                <React.Fragment key={group.id}>
                  <tr
                    ref={index === groups.length - 1 && hasMore ? lastGroupRef : null}
                    className="group-row"
                  >
                    <td>{group.stt}</td>
                    <td>{group.code}</td>
                    <td>{group.equipment}</td>
                    <td>{group.type}</td>
                    <td>{group.quantity}</td>
                    <td>{group.unit}</td>
                    <td>{group.startTime}</td>
                    <td>{group.endTime}</td>
                    <td className="note-cell">{group.note}</td>
                    <td>
                      <button
                        className="view-btn"
                        onClick={() => handleViewDetail(group.id)}
                        title="Xem chi tiết năng lực"
                      >
                        <i className={`fas ${selectedGroupId === group.id ? 'fa-eye-slash' : 'fa-eye'}`}></i>
                      </button>
                    </td>
                  </tr>

                  {/* CHI TIẾT NĂNG LỰC */}
                  {selectedGroupId === group.id && (
                    <tr>
                      <td colSpan="10">
                        <div className="capacities-table">
                          <h3>Năng lực chi tiết của {group.equipment}</h3>
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
                              {getCapacities(group).length > 0 ? (
                                getCapacities(group).map((cap) => (
                                  <tr key={cap.id}>
                                    <td>{cap.id}</td>
                                    <td>{cap.name}</td>
                                    <td>{cap.value}</td>
                                    <td>{cap.unit}</td>
                                    <td>{cap.description}</td>
                                  </tr>
                                ))
                              ) : (
                                <tr>
                                  <td colSpan="5" className="no-attributes">
                                    Không có năng lực chi tiết
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
                  <td colSpan="10" className="loading-cell">
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
                  <td colSpan="10" className="error-message">
                    {error}
                    <button onClick={handleReload} className="retry-btn">Thử lại</button>
                  </td>
                </tr>
              )}

              {/* NO DATA */}
              {!initialLoading && !loading && groups.length === 0 && !error && (
                <tr>
                  <td colSpan="10" className="no-data-message">
                    Không tìm thấy năng lực máy nào phù hợp.
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

export default MachineCapacity;