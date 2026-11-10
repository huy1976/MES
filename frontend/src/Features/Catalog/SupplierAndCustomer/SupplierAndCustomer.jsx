import React, { useState, useEffect, useRef, useCallback } from 'react';
import CreateNewSupplierAndCustomer from './CreateNewSupplierAndCustomer';
import { getBusinesses } from '../../../api/Business'; // Đảm bảo đường dẫn đúng
import { FaSyncAlt } from 'react-icons/fa';
import './SupplierAndCustomer.css';
import { useAuthPermissions } from '../../../api/UseAuthPermissions';

const PAGE_SIZE = 31;

const SupplierAndCustomer = ({ onBack }) => {
  // === STATE ===
  const [partners, setPartners] = useState([]);
  const [loading, setLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(true);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [totalItems, setTotalItems] = useState(0);
  const [isFullyLoaded, setIsFullyLoaded] = useState(false);
  const [reloadKey, setReloadKey] = useState(0);

  const { permissions, loading: permLoading, error: permError } = useAuthPermissions(7); // Giả sử pageId cho SupplierAndCustomer là 7
  const canView = permissions.some((p) => p.canView);
  const canEdit = permissions.some((p) => p.canEdit);

  // Filter
  const [showFilter, setShowFilter] = useState(false);
  const [filterValues, setFilterValues] = useState({ code: '', role: 'Tất cả', contact: '' });
  const [activeFilters, setActiveFilters] = useState(filterValues);

  // Tạo mới
  const [showCreateNew, setShowCreateNew] = useState(false);

  const observer = useRef();
  const scrollContainerRef = useRef(null);

  // === INFINITE SCROLL CALLBACK ===
  const lastPartnerRef = useCallback(
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

  // === CHUẨN HÓA DỮ LIỆU (ĐÃ CHỈNH SỬA ĐỂ THÊM EMAIL) ===
  const normalizePartner = (item, index) => {
    const stt = (page - 1) * PAGE_SIZE + index + 1;
    const props = item.properties || [];

    // Tìm các thuộc tính dựa trên tên (không phân biệt hoa thường)
    const phoneProp = props.find(p => p.name.toLowerCase() === 'phone');
    const addressProp = props.find(p => p.name.toLowerCase() === 'address');
    const contactProp = props.find(p => p.name.toLowerCase() === 'contact');
    // Thêm logic lấy Email
    const emailProp = props.find(p => p.name.toLowerCase() === 'email');

    const role = item.businessPartnerType === 0 ? 'Khách hàng' : 'Nhà cung cấp';

    return {
      id: item.businessPartnerId || `PART${stt.toString().padStart(3, '0')}`,
      stt,
      code: item.businessPartnerId || '--',
      name: item.name || '--',
      role,
      address: addressProp?.valueString?.trim() || '--',
      phone: phoneProp?.valueString?.trim() || '--',
      email: emailProp?.valueString?.trim() || '--', // Thêm trường email vào object
      rawData: item,
    };
  };

  // === FETCH DATA (SERVER-SIDE FILTER) ===
  const fetchPartners = useCallback(
    async (pageNum, isAppend = false, filtersToApply) => {
      setError(null);
      const isInitial = pageNum === 1 && !isAppend;
      if (isInitial) setInitialLoading(true);
      else setLoading(true);

      const apiFilters = {
        code: filtersToApply.code || null,
        role: filtersToApply.role === 'Tất cả' ? null : filtersToApply.role,
        contact: filtersToApply.contact || null,
      };

      try {
        const data = await getBusinesses(pageNum, PAGE_SIZE, apiFilters);
        const newItems = data.items || [];

        const mapped = newItems.map((item, index) => normalizePartner(item, index));

        const isTrulyFinished = newItems.length === 0 || newItems.length < PAGE_SIZE;
        setHasMore(!isTrulyFinished);
        setIsFullyLoaded(isTrulyFinished);

        setPartners(prev => (isAppend ? [...prev, ...mapped] : mapped));

        if (isInitial) {
          setTotalItems(data.totalItems || 0);
        }
      } catch (err) {
        setError('Không thể tải danh sách đối tác. Vui lòng thử lại.');
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
      fetchPartners(page, true, activeFilters);
    }
  }, [page, isFullyLoaded, fetchPartners, activeFilters]);

  // === EFFECT: TẢI MỚI KHI FILTER THAY ĐỔI ===
  useEffect(() => {
    setPage(1);
    setPartners([]);
    setIsFullyLoaded(false);
    setHasMore(true);

    if (scrollContainerRef.current) {
      scrollContainerRef.current.scrollTo({ top: 0, behavior: 'auto' });
    }

    fetchPartners(1, false, activeFilters);
  }, [activeFilters, fetchPartners]);

  // === EFFECT: RESET KHI RELOAD ===
  useEffect(() => {
    if (reloadKey > 0) {
      const defaultFilters = { code: '', role: 'Tất cả', contact: '' };
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
    handleReload();
  };

  // === SKELETON ROW ===
  const SkeletonRow = () => (
    <tr className="skeleton-row">
      {/* Tăng số lượng cột loading lên 7 để khớp với bảng */}
      {[...Array(7)].map((_, i) => (
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
        <CreateNewSupplierAndCustomer />
      </div>
    );
  }

  // === RENDER DANH SÁCH ===
  return (
    <div className="supplier-and-customer-container">
      {/* TITLE */}
      <h2 className="title">Danh mục | Khách hàng/Nhà cung cấp</h2>

      {/* TABS */}
      <div className="tabs">
        <button className="tab-btn active">Khách hàng/Nhà cung cấp</button>
      </div>

      {/* HEADER ACTIONS */}
      <div className="supplier-header">
        <div className="actions">
          <button className="action-btn filter-btn" onClick={() => setShowFilter(!showFilter)}>
            🔍Tìm kiếm
          </button>
          <button className={`action-btn new-btn ${!canEdit ? 'disabled-button' : ''}`}
          onClick={handleCreateNew} disabled={!canEdit} title={!canEdit ? "Bạn không có quyền tạo mới đối tác" : "Tạo mới đối tác"}>
            ➕Tạo mới
          </button>
          {(initialLoading || error || partners.length > 0) && (
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
                <label>Mã/Tên đối tác</label>
                <input
                  type="text"
                  placeholder="Nhập mã hoặc tên"
                  value={filterValues.code}
                  onChange={(e) => setFilterValues({ ...filterValues, code: e.target.value })}
                />
              </div>
              <div>
                <label>Vai trò</label>
                <select
                  value={filterValues.role}
                  onChange={(e) => setFilterValues({ ...filterValues, role: e.target.value })}
                >
                  <option value="Tất cả">Tất cả</option>
                  <option value="Nhà cung cấp">Nhà cung cấp</option>
                  <option value="Khách hàng">Khách hàng</option>
                </select>
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
        Đang hiển thị {partners.length.toLocaleString()} / {totalItems.toLocaleString()} đối tác.
        {isFullyLoaded && partners.length > 0 && " (Đã tải hết)"}
      </div>

      {/* BẢNG CHÍNH */}
      <div className="partners-table">
        <div className="table-container" ref={scrollContainerRef}>
          <table role="grid">
            <thead>
              <tr>
                <th>STT</th>
                <th>Tên đối tác</th>
                <th>Mã đối tác</th>
                <th>Vai trò</th>
                <th>Địa chỉ</th>
                <th>Số điện thoại</th>
                <th>Email</th> {/* Thêm cột Email */}
              </tr>
            </thead>
            <tbody>
              {/* SKELETON */}
              {initialLoading && partners.length === 0 && [...Array(PAGE_SIZE)].map((_, i) => <SkeletonRow key={i} />)}

              {/* DỮ LIỆU */}
              {partners.map((partner, index) => (
                <tr
                  key={partner.id}
                  ref={index === partners.length - 1 && hasMore ? lastPartnerRef : null}
                  className="partner-row"
                >
                  <td>{partner.stt}</td>
                  <td>{partner.name}</td>
                  <td>{partner.code}</td>
                  <td>
                    <span className={`role-badge ${partner.role === 'Khách hàng' ? 'customer' : 'supplier'}`}>
                      {partner.role}
                    </span>
                  </td>
                  <td>{partner.address}</td>
                  <td>{partner.phone}</td>
                  <td>{partner.email}</td> {/* Hiển thị dữ liệu Email */}
                </tr>
              ))}

              {/* LOADING MORE */}
              {loading && !initialLoading && (
                <tr>
                  <td colSpan="7" className="loading-cell"> {/* colSpan cập nhật thành 7 */}
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
                  <td colSpan="7" className="error-message"> {/* colSpan cập nhật thành 7 */}
                    {error}
                    <button onClick={handleReload} className="retry-btn">Thử lại</button>
                  </td>
                </tr>
              )}

              {/* NO DATA */}
              {!initialLoading && !loading && partners.length === 0 && !error && (
                <tr>
                  <td colSpan="7" className="no-data-message"> {/* colSpan cập nhật thành 7 */}
                    Không tìm thấy đối tác nào phù hợp.
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

export default SupplierAndCustomer;