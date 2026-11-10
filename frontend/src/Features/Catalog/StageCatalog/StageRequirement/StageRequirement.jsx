import React, { useState, useEffect } from 'react';
import CreateNewEquipment from './CreateNewEquipment';
import CreateNewHuman from './CreateNewHuman';
import CreateNewMaterial from './CreateNewMaterial';
import { FaSyncAlt, FaChevronDown, FaChevronUp, FaPlus, FaSearch, FaEye, FaEraser } from 'react-icons/fa'; // Thêm icon FaEraser
import './StageRequirement.css';
import {
  getMaterialSegmentSpecifications,
  getEquipmentSegmentSpecifications,
  getPersonnelSegmentSpecifications
} from '../../../../api/StageRequirement';

import {useAuthPermissions} from '../../../../api/UseAuthPermissions';

// === COMPONENT HIỂN THỊ TỪNG PHẦN ===
const RequirementSection = ({
  title,
  type,
  data,
  isVisible,
  onToggle,
  onFilterToggle,
  showFilter,
  filterValues,
  onFilterChange,
  onApplyFilter,
  onClearFilter, // <--- Nhận thêm props này
  onCreate,
  showDetails,
  onToggleDetails,
  columns,
  renderRow,
  loading,
  canEdit,
  canView
}) => {
  return (
    <div className="requirement-section">
      {/* HEADER */}
      <div className="section-header" onClick={() => onToggle(type)}>
        <h3>{title} <span className="count-badge">{data.length}</span></h3>
        <button className="toggle-icon-btn">
          {isVisible ? <FaChevronUp /> : <FaChevronDown />}
        </button>
      </div>

      {isVisible && (
        <div className="section-body">
          {/* ACTIONS & FILTER */}
          <div className="stage-header">
            <div className="actions">
              <button className={`action-btn filter-btn ${showFilter ? 'active' : ''}`} onClick={() => onFilterToggle(type)}>
                <FaSearch /> Tìm kiếm
              </button>
              <button className={`action-btn new-btn ${canEdit ? '' : 'disabled-button'}`}
                disabled={!canEdit}
                title={!canEdit ? "Bạn không có quyền tạo mới yêu cầu công đoạn" : "Tạo mới yêu cầu công đoạn"}
              onClick={() => onCreate(type)}>
                <FaPlus /> Tạo mới
              </button>

              {/* FILTER PANEL */}
              {showFilter && (
                <div className="filter-panel active">
                  <div className="filter-group">
                    <label>Mã yêu cầu / Mô tả</label> {/* Đổi nhãn cho rõ ràng */}
                    <input
                      type="text"
                      name="code"
                      placeholder="Nhập mã yêu cầu..."
                      value={filterValues.code}
                      onChange={(e) => onFilterChange(type, e)}
                    />
                  </div>
                  <div className="filter-group">
                    <label>Mã công đoạn</label>
                    <input
                      type="text"
                      name="stageCode"
                      placeholder="Nhập mã công đoạn..."
                      value={filterValues.stageCode}
                      onChange={(e) => onFilterChange(type, e)}
                    />
                  </div>
                  <div className="filter-group">
                    <label>Phân loại</label>
                    <select
                      name={type === 'human' ? 'role' : 'type'}
                      value={type === 'human' ? filterValues.role : filterValues.type}
                      onChange={(e) => onFilterChange(type, e)}
                    >
                      <option value="Tất cả">Tất cả</option>
                      {type === 'equipment' && (
                        <>
                          <option value="Drawing Machine">Drawing Machine</option>
                          <option value="Máy kéo">Máy kéo</option>
                          <option value="Máy ép">Máy ép</option>
                        </>
                      )}
                      {type === 'human' && (
                        <>
                          <option value="Van hanh">Vận hành</option>
                          <option value="Bảo trì">Bảo trì</option>
                          <option value="Thi công">Thi công</option>
                        </>
                      )}
                      {type === 'material' && (
                        <>
                          <option value="Nguyen Vat Lieu">Nguyên vật liệu</option>
                          <option value="Thành phẩm">Thành phẩm</option>
                          <option value="Bán thành phẩm">Bán thành phẩm</option>
                        </>
                      )}
                    </select>
                  </div>
                  <div className="filter-actions">
                    {/* Nút Xóa Lọc mới */}
                    <button className="clear-btn" onClick={() => onClearFilter(type)} title="Xóa điều kiện lọc và hiện tất cả">
                       <FaEraser /> Xóa lọc
                    </button>
                    <div className="right-actions">
                        <button className="search-btn" onClick={() => onApplyFilter(type)}>Áp dụng</button>
                        <button className="close-btn" onClick={() => onFilterToggle(type)}>Đóng</button>
                    </div>
                  </div>
                </div>
              )}
            </div>
          </div>

          {/* TABLE */}
          <div className="requirements-table">
            <div className="table-container">
              <table role="grid">
                <thead>
                  <tr>
                    <th style={{ width: '60px' }} className="text-center sticky-col">STT</th>
                    {columns.map((col, idx) => (
                      <th key={idx} className={col.className}>{col.name}</th>
                    ))}
                    <th style={{ width: '80px' }} className="text-center sticky-col-right">Chi tiết</th>
                  </tr>
                </thead>
                <tbody>
                  {loading ? (
                    [...Array(3)].map((_, i) => (
                      <tr key={i} className="skeleton-row">
                        <td colSpan={columns.length + 2}><div className="skeleton-line"></div></td>
                      </tr>
                    ))
                  ) : data.length === 0 ? (
                    <tr>
                      <td colSpan={columns.length + 2} className="no-data-message">
                        Không tìm thấy dữ liệu phù hợp với bộ lọc.
                      </td>
                    </tr>
                  ) : (
                    data.map((item, index) => (
                      <React.Fragment key={item.id}>
                        <tr className="data-row">
                          <td className="text-center">{index + 1}</td>
                          {renderRow(item)}
                          <td className="text-center">
                            <button className="view-btn" onClick={() => onToggleDetails(type, item.id)} title="Xem chi tiết">
                              <FaEye />
                            </button>
                          </td>
                        </tr>
                        {showDetails[item.id] && (
                          <tr className="detail-row">
                            <td colSpan={columns.length + 2}>
                              <div className="sub-table-wrapper">
                                <h4>Thông số kỹ thuật chi tiết:</h4>
                                <div className="sub-table-container">
                                  {item.details && item.details.length > 0 ? (
                                    <table className="sub-table">
                                      <thead>
                                        <tr>
                                          <th>Tên thuộc tính</th>
                                          <th>Giá trị</th>
                                          <th>Đơn vị</th>
                                          <th>Mô tả thêm</th>
                                        </tr>
                                      </thead>
                                      <tbody>
                                        {item.details.map((d, i) => (
                                          <tr key={i}>
                                            <td>{d.attributeName || d.criteriaName}</td>
                                            <td>{d.value}</td>
                                            <td>{d.unit}</td>
                                            <td>{d.description}</td>
                                          </tr>
                                        ))}
                                      </tbody>
                                    </table>
                                  ) : (
                                    <div className="no-detail-box">Chưa có cấu hình chi tiết cho mục này.</div>
                                  )}
                                </div>
                              </div>
                            </td>
                          </tr>
                        )}
                      </React.Fragment>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

const StageRequirement = () => {
  // === STATE ===
  const [data, setData] = useState({ equipment: [], human: [], material: [] });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [reloadKey, setReloadKey] = useState(0);
  const { permissions, loading: permLoading, error: permError } = useAuthPermissions(7);
  const canView = permissions.some((p) => p.canView);
  const canEdit = permissions.some((p) => p.canEdit);
  const [showCreateNew, setShowCreateNew] = useState({ equipment: false, human: false, material: false });
  const [showFilter, setShowFilter] = useState({ equipment: false, human: false, material: false });
  const [isVisible, setIsVisible] = useState({ equipment: true, human: true, material: true });
  const [showDetails, setShowDetails] = useState({ equipment: {}, human: {}, material: {} });

  // State cho giá trị đang nhập trong form
  const [filterValues, setFilterValues] = useState({
    equipment: { code: '', stageCode: '', type: 'Tất cả' },
    human: { code: '', stageCode: '', role: 'Tất cả' },
    material: { code: '', stageCode: '', type: 'Tất cả' },
  });
  
  // State cho bộ lọc ĐANG ĐƯỢC ÁP DỤNG thực tế
  const [activeFilters, setActiveFilters] = useState({
    equipment: { code: '', stageCode: '', type: 'Tất cả' },
    human: { code: '', stageCode: '', role: 'Tất cả' },
    material: { code: '', stageCode: '', type: 'Tất cả' },
  });

  // === FETCH DATA (Giữ nguyên) ===
  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      setError(null);
      try {
        const [materialRes, equipmentRes, personnelRes] = await Promise.all([
          getMaterialSegmentSpecifications(),
          getEquipmentSegmentSpecifications(),
          getPersonnelSegmentSpecifications(),
        ]);

        const mapProps = (props, isHuman = false) => (props || []).map(p => ({
            id: p.propertyId || Math.random(),
            attributeName: p.name,
            criteriaName: p.name,
            value: isHuman && p.valueString === 'true' ? 'Có' : p.valueString,
            unit: p.valueUnitOfMeasure === 'None' ? '' : p.valueUnitOfMeasure,
            description: p.description
        }));

        // Mapping Equipment
        const eqData = (equipmentRes?.items || []).map(item => ({
          id: item.equipmentSegmentSpecificationId,
          code: item.equipmentSegmentSpecificationId, // Đây là Mã Yêu Cầu
          stageCode: item.processSegmentId,
          type: item.equipmentClassName,
          description: item.description,
          function: item.equipmentUse, 
          quantity: item.quantity?.valueString || '0',
          unit: item.valueUnitOfMeasure,
          details: mapProps(item.properties)
        }));

        // Mapping Human
        const huData = (personnelRes?.items || []).map(item => ({
          id: item.personnelSegmentSpecificationId,
          code: item.personnelSegmentSpecificationId, // Đây là Mã Yêu Cầu
          stageCode: item.processSegmentId,
          role: item.personnelClassName,
          description: item.description,
          usage: item.personnelUse,
          quantity: item.quantity?.valueString || '0',
          unit: item.valueUnitOfMeasure,
          details: mapProps(item.properties, true)
        }));

        // Mapping Material
        const maData = (materialRes?.items || []).map(item => ({
          id: item.materialSegmentSpecificationId,
          code: item.materialSegmentSpecificationId, // Đây là Mã Yêu Cầu
          stageCode: item.processSegmentId,
          type: item.materialClassName,
          material: item.materialDefinitionName,
          description: item.description,
          assemblyType: item.assemblyType,
          assemblyRelation: item.assemblyRelationship,
          usage: item.materialUse,
          quantity: item.quantity?.valueString || '0',
          unit: item.valueUnitOfMeasure,
          details: mapProps(item.properties)
        }));

        setData({ equipment: eqData, human: huData, material: maData });

      } catch (err) {
        console.error("Error fetching requirements:", err);
        setError("Không thể tải dữ liệu. Vui lòng kiểm tra kết nối.");
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [reloadKey]);

  // === HANDLERS ===
  const handleReload = () => setReloadKey(prev => prev + 1);
  const handleToggleSection = (section) => setIsVisible(prev => ({ ...prev, [section]: !prev[section] }));
  
  // Create New Handlers (Giữ nguyên)
  const handleCreateNew = (section) => {
    setShowCreateNew(prev => ({ ...prev, [section]: true }));
    setIsVisible({ equipment: false, human: false, material: false }); 
  };
  const handleBack = () => {
    setShowCreateNew({ equipment: false, human: false, material: false });
    setIsVisible({ equipment: true, human: true, material: true });
    handleReload();
  };

  // --- LOGIC FILTER & SEARCH ---
  
  const handleFilterToggle = (section) => setShowFilter(prev => ({ ...prev, [section]: !prev[section] }));
  
  // 1. Khi người dùng gõ vào ô input -> Cập nhật filterValues (chưa lọc ngay)
  const handleFilterChange = (section, e) => {
    const { name, value } = e.target;
    setFilterValues(prev => ({ ...prev, [section]: { ...prev[section], [name]: value } }));
  };

  // 2. Khi bấm "Áp dụng" -> Đẩy giá trị từ filterValues sang activeFilters
  const handleApplyFilter = (section) => {
    setActiveFilters(prev => ({ ...prev, [section]: filterValues[section] }));
    setShowFilter(prev => ({ ...prev, [section]: false })); // Đóng panel sau khi áp dụng
  };

  // 3. Khi bấm "Xóa lọc" -> Reset cả filterValues và activeFilters về rỗng/mặc định
  const handleClearFilter = (section) => {
    const defaultState = section === 'human' 
        ? { code: '', stageCode: '', role: 'Tất cả' }
        : { code: '', stageCode: '', type: 'Tất cả' };

    // Reset form nhập liệu
    setFilterValues(prev => ({ ...prev, [section]: defaultState }));
    // Reset bộ lọc đang áp dụng (để hiện lại tất cả dữ liệu ngay lập tức)
    setActiveFilters(prev => ({ ...prev, [section]: defaultState }));
    
    // Có thể chọn đóng panel luôn hoặc giữ nguyên. Ở đây mình giữ nguyên để user thấy đã xóa.
    // setShowFilter(prev => ({ ...prev, [section]: false })); 
  };

  const handleToggleDetails = (section, id) => {
    setShowDetails(prev => ({ ...prev, [section]: { ...prev[section], [id]: !prev[section][id] } }));
  };

  // === HÀM LỌC DỮ LIỆU CHÍNH ===
  const filterData = (sectionName, items) => {
    const filters = activeFilters[sectionName];

    // Nếu không có filter nào được set, trả về toàn bộ (Performance optimization)
    if (!filters.code && !filters.stageCode && (filters.type === 'Tất cả' || filters.role === 'Tất cả')) {
        return items;
    }

    return items.filter(item => {
      // Chuẩn hóa chuỗi tìm kiếm (chữ thường, bỏ khoảng trắng thừa)
      const searchCode = filters.code.toLowerCase().trim();
      const searchStage = filters.stageCode.toLowerCase().trim();

      // Lọc theo CODE (Mã yêu cầu) - Ưu tiên tìm trong Code, nếu muốn tìm cả mô tả thì để cả 2
      const matchCode = !searchCode || 
        (item.code && item.code.toLowerCase().includes(searchCode)) || 
        (item.description && item.description.toLowerCase().includes(searchCode));

      // Lọc theo Stage Code
      const matchStage = !searchStage || 
        (item.stageCode && item.stageCode.toLowerCase().includes(searchStage));

      // Lọc theo Type/Role
      let matchType = true;
      if (sectionName === 'equipment' && filters.type !== 'Tất cả') matchType = item.type === filters.type;
      else if (sectionName === 'human' && filters.role !== 'Tất cả') matchType = item.role === filters.role;
      else if (sectionName === 'material' && filters.type !== 'Tất cả') matchType = item.type === filters.type;

      return matchCode && matchStage && matchType;
    });
  };
  
  // Render logic (Giữ nguyên phần return CreateNew)
  if (showCreateNew.equipment) return <><div className="detail-nav"><button onClick={handleBack} className="back-btn">← Quay lại danh sách</button></div><CreateNewEquipment /></>;
  if (showCreateNew.human) return <><div className="detail-nav"><button onClick={handleBack} className="back-btn">← Quay lại danh sách</button></div><CreateNewHuman /></>;
  if (showCreateNew.material) return <><div className="detail-nav"><button onClick={handleBack} className="back-btn">← Quay lại danh sách</button></div><CreateNewMaterial /></>;

  // Columns Definitions (Giữ nguyên)
  const equipmentColumns = [
    { name: 'Mã yêu cầu', className: 'text-center' },
    { name: 'Mã công đoạn', className: 'text-center' },
    { name: 'Loại thiết bị', className: 'text-center' },
    { name: 'Mô tả thiết bị', className: 'text-center' },
    { name: 'Chức năng', className: 'text-center' },
    { name: 'Số lượng', className: 'text-center' },
    { name: 'Đơn vị', className: 'text-center' }
  ];

  const humanColumns = [
    { name: 'Mã yêu cầu', className: 'text-center' },
    { name: 'Mã công đoạn', className: 'text-center' },
    { name: 'Vai trò', className: 'text-center' },
    { name: 'Mô tả công việc', className: 'text-center' },
    { name: 'Cách dùng', className: 'text-center' },
    { name: 'Số lượng', className: 'text-center' },
    { name: 'Đơn vị', className: 'text-center' }
  ];

  const materialColumns = [
    { name: 'Mã yêu cầu', className: 'text-center' },
    { name: 'Mã công đoạn', className: 'text-center' },
    { name: 'Loại vật liệu', className: 'text-center' },
    { name: 'Tên vật liệu', className: 'text-center' },
    { name: 'Mô tả', className: 'text-center' },
    { name: 'Kiểu lắp', className: 'text-center' },
    { name: 'Quan hệ', className: 'text-center' },
    { name: 'Cách dùng', className: 'text-center' },
    { name: 'Số lượng', className: 'text-center' },
    { name: 'Đơn vị', className: 'text-center' }
  ];

  return (
    <div className="stage-req-root">
      <div className="main-header">
        <button className="action-btn reload-btn" onClick={handleReload} disabled={loading}>
          <FaSyncAlt className={loading ? 'spinning' : ''} /> Tải dữ liệu
        </button>
      </div>
      {error && <div className="error-message">{error}</div>}

      {/* TABLE 1: EQUIPMENT */}
      <RequirementSection 
        title="Yêu cầu thiết bị"
        type="equipment"
        data={filterData('equipment', data.equipment)}
        isVisible={isVisible.equipment}
        loading={loading}
        onToggle={handleToggleSection}
        onFilterToggle={handleFilterToggle}
        showFilter={showFilter.equipment}
        filterValues={filterValues.equipment}
        onFilterChange={handleFilterChange}
        onApplyFilter={handleApplyFilter}
        onClearFilter={handleClearFilter} // TRUYỀN HÀM CLEAR
        onCreate={handleCreateNew}
        showDetails={showDetails.equipment}
        onToggleDetails={handleToggleDetails}
        columns={equipmentColumns}
        renderRow={(req) => (
          <>
            <td className="text-center">{req.code}</td>
            <td className="text-center">{req.stageCode}</td>
            <td className="text-center"><span className="type-badge equipment">{req.type}</span></td>
            <td className="text-center description-cell">{req.description}</td>
            <td className="text-center">{req.function}</td>
            <td className="text-center font-bold">{req.quantity}</td>
            <td className="text-center">{req.unit}</td>
          </>
        )}
        canEdit={canEdit}
        canView={canView}
      />

      {/* TABLE 2: HUMAN */}
      <RequirementSection 
        title="Yêu cầu nhân sự"
        type="human"
        data={filterData('human', data.human)}
        isVisible={isVisible.human}
        loading={loading}
        onToggle={handleToggleSection}
        onFilterToggle={handleFilterToggle}
        showFilter={showFilter.human}
        filterValues={filterValues.human}
        onFilterChange={handleFilterChange}
        onApplyFilter={handleApplyFilter}
        onClearFilter={handleClearFilter} // TRUYỀN HÀM CLEAR
        onCreate={handleCreateNew}
        showDetails={showDetails.human}
        onToggleDetails={handleToggleDetails}
        columns={humanColumns}
        renderRow={(req) => (
          <>
            <td className="text-center">{req.code}</td>
            <td className="text-center">{req.stageCode}</td>
            <td className="text-center"><span className="type-badge human">{req.role}</span></td>
            <td className="text-center description-cell">{req.description}</td>
            <td className="text-center">{req.usage}</td>
            <td className="text-center font-bold">{req.quantity}</td>
            <td className="text-center">{req.unit}</td>
          </>
        )}
        canEdit={canEdit}
        canView={canView}
      />

      {/* TABLE 3: MATERIAL */}
      <RequirementSection 
        title="Yêu cầu vật liệu"
        type="material"
        data={filterData('material', data.material)}
        isVisible={isVisible.material}
        loading={loading}
        onToggle={handleToggleSection}
        onFilterToggle={handleFilterToggle}
        showFilter={showFilter.material}
        filterValues={filterValues.material}
        onFilterChange={handleFilterChange}
        onApplyFilter={handleApplyFilter}
        onClearFilter={handleClearFilter} // TRUYỀN HÀM CLEAR
        onCreate={handleCreateNew}
        showDetails={showDetails.material}
        onToggleDetails={handleToggleDetails}
        columns={materialColumns}
        renderRow={(req) => (
          <>
            <td className="text-center">{req.code}</td>
            <td className="text-center">{req.stageCode}</td>
            <td className="text-center"><span className="type-badge material">{req.type}</span></td>
            <td className="text-center font-bold">{req.material}</td>
            <td className="text-center description-cell">{req.description}</td>
            <td className="text-center">{req.assemblyType}</td>
            <td className="text-center">{req.assemblyRelation}</td>
            <td className="text-center">{req.usage}</td>
            <td className="text-center font-bold">{req.quantity}</td>
            <td className="text-center">{req.unit}</td>
          </>
        )}
        canEdit={canEdit}
        canView={canView}
      />
    </div>
  );
};

export default StageRequirement;