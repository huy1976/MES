import React, { useState, useEffect, useRef, useCallback, useMemo } from 'react';
import { CreateNewGroupHuman } from '../Create';
import { getPersonGroups } from '../../../../api/PersonGroup';
import { getPerson } from '../../../../api/Person';
import { FaSyncAlt } from 'react-icons/fa';
import './GroupHuman.css';
import { useAuthPermissions } from '../../../../api/UseAuthPermissions';
const PAGE_SIZE = 31;

const GroupHuman = () => {
  // === STATE ===
  const [groups, setGroups] = useState([]);
  const [personsMap, setPersonsMap] = useState({});
  const [loading, setLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(true);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [totalItems, setTotalItems] = useState(0);
  const [isFullyLoaded, setIsFullyLoaded] = useState(false);
  const [reloadKey, setReloadKey] = useState(0);

  // Chi tiết mở rộng
  const [expandedRow, setExpandedRow] = useState(null);

  // Filter
  const [showFilter, setShowFilter] = useState(false);
  const [filterValues, setFilterValues] = useState({ code: '', name: '' });
  const [activeFilters, setActiveFilters] = useState(filterValues);

  // Tạo mới
  const [showCreateNew, setShowCreateNew] = useState(false);

  const observer = useRef();
  const scrollContainerRef = useRef(null);

  const { permissions, loading: permLoading, error: permError } = useAuthPermissions(7);
  const canView = permissions.some((p) => p.canView);
  const canEdit = permissions.some((p) => p.canEdit);

  // === TẢI NHÂN VIÊN (MAP) ===
  useEffect(() => {
    const fetchPersons = async () => {
      try {
        const data = await getPerson(1, 200);
        const map = {};
        data.items.forEach(p => {
          if (p.personId && p.name) map[p.personId] = p.name;
        });
        setPersonsMap(map);
      } catch (err) {
        console.error('Lỗi tải nhân viên:', err);
      }
    };
    fetchPersons();
  }, []);

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

  // === CHUẨN HÓA DỮ LIỆU ===
  const normalizeGroup = (item, index) => {
    const stt = (page - 1) * PAGE_SIZE + index + 1;
    const persons = item.persons || [];
    const props = item.properties || [];

    return {
      id: item.personnelClassId || `GRP${stt.toString().padStart(3, '0')}`,
      stt,
      groupId: item.personnelClassId,
      code: item.personnelClassId || '--',
      name: item.name || '--',
      memberCount: persons.length,
      description: item.description || '--',
      members: persons.map((personId, i) => ({
        id: i + 1,
        personId,
        name: personsMap[personId] || personId || '--',
      })),
      properties: props,
      rawData: item,
    };
  };

  // === FETCH DATA (SERVER-SIDE FILTER) ===
  const fetchGroups = useCallback(
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
        const data = await getPersonGroups(pageNum, PAGE_SIZE, apiFilters);
        const newItems = data.items || [];

        const mapped = newItems.map((item, index) => normalizeGroup(item, index));

        const isTrulyFinished = newItems.length === 0 || newItems.length < PAGE_SIZE;
        setHasMore(!isTrulyFinished);
        setIsFullyLoaded(isTrulyFinished);

        setGroups(prev => (isAppend ? [...prev, ...mapped] : mapped));

        if (isInitial) {
          setTotalItems(data.totalItems || 0);
        }
      } catch (err) {
        setError('Không thể tải danh sách nhóm nhân sự. Vui lòng thử lại.');
        console.error('Fetch error:', err);
      } finally {
        setLoading(false);
        setInitialLoading(false);
      }
    },
    [personsMap]
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
          <button onClick={handleBack} className="back-btn">Quay lại</button>
        </div>
        <CreateNewGroupHuman />
      </div>
    );
  }

  // === RENDER DANH SÁCH ===
  return (
    <div className="group-human-page">
      {/* HEADER */}
      <div className="group-human-header">
        <div className="actions">
          <button className="action-btn filter-btn" onClick={() => setShowFilter(!showFilter)}>
            🔍Tìm kiếm
          </button>
          <button className={`action-btn new-btn ${!canEdit ? 'disabled-button' : ''}`}
          disabled={!canEdit}
          title={!canEdit ? "Bạn không có quyền tạo mới nhóm nhân sự" : "Tạo mới nhóm nhân sự"}
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
                <label>Mã nhóm</label>
                <input
                  type="text"
                  placeholder="Nhập mã"
                  value={filterValues.code}
                  onChange={(e) => setFilterValues({ ...filterValues, code: e.target.value })}
                />
              </div>
              <div>
                <label>Tên nhóm</label>
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
        Đang hiển thị {groups.length.toLocaleString()} / {totalItems.toLocaleString()} nhóm nhân sự.
        {isFullyLoaded && groups.length > 0 && " (Đã tải hết)"}
      </div>

      {/* TABLE */}
      <div className="groups-table">
        <div className="table-container" ref={scrollContainerRef}>
          <table role="grid">
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã nhóm</th>
                <th>Tên nhóm</th>
                <th>Số nhân viên</th>
                <th>Mô tả</th>
                <th>Chi tiết</th>
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
                    <td>{group.name}</td>
                    <td>{group.memberCount}</td>
                    <td>{group.description}</td>
                    <td>
                      <button
                        className="view-btn"
                        onClick={() => toggleExpandRow(group.id)}
                        title="Xem chi tiết"
                      >
                        <i className={`fas ${expandedRow === group.id ? 'fa-eye-slash' : 'fa-eye'}`}></i>
                      </button>
                    </td>
                  </tr>

                  {/* BẢNG CHI TIẾT */}
                  {expandedRow === group.id && (
                    <tr>
                      <td colSpan="6">
                        <div className="details-table">
                          <h3>Chi tiết nhóm: {group.name}</h3>

                          {/* Bảng thành viên */}
                          <h4 className="section-title">Thành viên ({group.memberCount})</h4>
                          <table role="grid" className="sub-table">
                            <thead>
                              <tr>
                                <th>STT</th>
                                <th>Mã nhân viên</th>
                                <th>Tên nhân viên</th>
                              </tr>
                            </thead>
                            <tbody>
                              {group.members.length > 0 ? (
                                group.members.map((member, i) => (
                                  <tr key={member.id}>
                                    <td>{i + 1}</td>
                                    <td>{member.personId}</td>
                                    <td>{member.name}</td>
                                  </tr>
                                ))
                              ) : (
                                <tr>
                                  <td colSpan="3" className="no-data">
                                    Không có thành viên
                                  </td>
                                </tr>
                              )}
                            </tbody>
                          </table>

                          {/* Bảng thuộc tính */}
                          {group.properties.length > 0 && (
                            <>
                              <h4 className="section-title">Thuộc tính</h4>
                              <table role="grid" className="sub-table">
                                <thead>
                                  <tr>
                                    <th>STT</th>
                                    <th>Tên</th>
                                    <th>Giá trị</th>
                                    <th>Mô tả</th>
                                  </tr>
                                </thead>
                                <tbody>
                                  {group.properties.map((prop, i) => (
                                    <tr key={prop.propertyId || i}>
                                      <td>{i + 1}</td>
                                      <td>{prop.name}</td>
                                      <td>
                                        <span className={`value-badge ${prop.valueString?.toLowerCase()}`}>
                                          {prop.valueString || '--'}
                                        </span>
                                      </td>
                                      <td>{prop.description || '--'}</td>
                                    </tr>
                                  ))}
                                </tbody>
                              </table>
                            </>
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
              {!initialLoading && !loading && groups.length === 0 && !error && (
                <tr>
                  <td colSpan="6" className="no-data-message">
                    Không tìm thấy nhóm nhân sự nào phù hợp.
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

export default GroupHuman;