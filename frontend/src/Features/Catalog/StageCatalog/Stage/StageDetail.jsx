import React, { useState, useEffect} from 'react';
import './StageDetail.css';
import { getEquipmentSegmentSpecificationsByProcessSegmentId,
  getMaterialSegmentSpecificationsByProcessSegmentId,
  getPersonnelSegmentSpecificationsByProcessSegmentId
 } from '../../../../api/ProcessSegment';

const StageDetail = ({ stage }) => {
  const [activeTab, setActiveTab] = useState('equipment'); // 'equipment' | 'human' | 'material'
  const [showDetails, setShowDetails] = useState({ equipment: {}, human: {}, material: {} });

  const [equipmentSegmentSpecifications, setEquipmentSegmentSpecifications] = useState([]);
  const [humanSegmentSpecifications, setHumanSegmentSpecifications] = useState([]);
  const [materialSegmentSpecifications, setMaterialSegmentSpecifications] = useState([]);
  const [loading, setLoading] = useState(true); // Thêm state loading

  // === EFFECT MỚI ĐỂ TẢI TÊN THUỘC TÍNH ===
          useEffect(() => {
            const GetApi = async () => {
              setLoading(true); // Bắt đầu loading
              try {
                const equipmentSpecs = await getEquipmentSegmentSpecificationsByProcessSegmentId(stage.code);
                setEquipmentSegmentSpecifications(equipmentSpecs.items);
                const humanSpecs = await getPersonnelSegmentSpecificationsByProcessSegmentId(stage.code);
                setHumanSegmentSpecifications(humanSpecs.items);
                const materialSpecs = await getMaterialSegmentSpecificationsByProcessSegmentId(stage.code);
                setMaterialSegmentSpecifications(materialSpecs.items);
              } catch (error) {
                console.error(error);
                setEquipmentSegmentSpecifications([]);
                setHumanSegmentSpecifications([]);
                setMaterialSegmentSpecifications([]);
              } finally {
                setLoading(false); // Kết thúc loading
              }
            };
            GetApi();
          }, [stage.code]); // Đảm bảo reload khi stage thay đổi

  const toggleDetails = (section, uniqueKey) => {
    setShowDetails((prev) => ({
      ...prev,
      [section]: { ...prev[section], [uniqueKey]: !prev[section][uniqueKey] },
    }));
  };

  // Skeleton cho bảng
  const SkeletonTable = ({ cols = 8, rows = 6 }) => (
    <table className="attributes-table">
      <tbody>
        {[...Array(rows)].map((_, r) => (
          <tr key={r} className="skeleton-row">
            {[...Array(cols)].map((_, c) => (
              <td key={c}><div className="skeleton-line"></div></td>
            ))}
          </tr>
        ))}
      </tbody>
    </table>
  );

  if (loading) {
    return (
      <div className="stage-detail-container">
        <div className="detail-header">
          <h2>Thông tin chi tiết của {stage.code}</h2>
        </div>
        <div className="loading-spinner" style={{ textAlign: 'center', margin: '32px 0' }}>
          <div className="spinner"></div>
          <span>Đang tải dữ liệu chi tiết...</span>
        </div>
        {/* Skeleton cho từng tab */}
        <div className="tabs">
          <button className="tab active">Thiết bị</button>
          <button className="tab">Nhân sự</button>
          <button className="tab">Vật liệu</button>
        </div>
        <div className="content">
          <SkeletonTable cols={9} rows={6} />
        </div>
      </div>
    );
  }

  return (
    <div className="stage-detail-container">
      {/* Header */}
      <div className="detail-header">
        <h2>Thông tin chi tiết của {stage.code}</h2>
        <table className="info-row">
          <thead>
            <tr>
              <th>Mã công đoạn</th>
              <th>Tên công đoạn</th>
              <th>Mô tả</th>
              <th>Loại công đoạn</th>
              <th>Thông số sản xuất</th>
              <th>Yêu cầu công đoạn</th>
              <th>Thời gian thực hiện</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>{stage.code}</td>
              <td>{stage.name}</td>
              <td>{stage.description}</td>
              <td>{stage.type}</td>
              <td>{stage.productionParams}</td>
              <td>{stage.requiredCount}</td>
              <td>{stage.duration}</td>
            </tr>
          </tbody>
        </table>
      </div>

      {/* Tabs */}
      <div className="tabs">
        <button
          className={`tab ${activeTab === 'equipment' ? 'active' : ''}`}
          onClick={() => setActiveTab('equipment')}
        >
          Thiết bị
        </button>
        <button
          className={`tab ${activeTab === 'human' ? 'active' : ''}`}
          onClick={() => setActiveTab('human')}
        >
          Nhân sự
        </button>
        <button
          className={`tab ${activeTab === 'material' ? 'active' : ''}`}
          onClick={() => setActiveTab('material')}
        >
          Vật liệu
        </button>
      </div>

      {/* Content */}
      <div className="content">
        {/* === Tab: THIẾT BỊ === */}
        {activeTab === 'equipment' && (
          <div className="attributes-section">
            <h3>Yêu cầu thiết bị</h3>
            <table className="attributes-table">
              <thead>
                <tr>
                  <th>STT</th>
                  <th>Mã yêu cầu</th>
                  <th>Mã công đoạn</th>
                  <th>Loại thiết bị</th>
                  <th>Mô tả</th>
                  <th>Chức năng thiết bị</th>
                  <th>Số lượng</th>
                  <th>Đơn vị</th>
                  <th>Chi tiết</th>
                </tr>
              </thead>
              <tbody>
                {equipmentSegmentSpecifications.map((attr, index) => (
                  <React.Fragment key={`equipment-${index}`}>
                    <tr>
                      <td>{index + 1}</td>
                      <td>{attr.equipmentSegmentSpecificationId}</td>
                      <td>{attr.processSegmentId}</td>
                      <td>{attr.equipmentClassName}</td>
                      <td>{attr.description}</td>
                      <td>Sản xuất</td>
                      <td>{attr.quantity.valueString}</td>
                      <td>{attr.valueUnitOfMeasure}</td>
                      <td>
                        <button
                          className="view-btn"
                          onClick={() => toggleDetails('equipment', `equipment-${index}`)}
                          aria-label={`Xem chi tiết yêu cầu ${attr.requestCode}`}
                        >
                          <span className="view-icon"><i className="fas fa-eye"></i></span>
                        </button>
                      </td>
                    </tr>
                    {showDetails.equipment[`equipment-${index}`] && (
                      <tr>
                        <td colSpan="10">
                          <div className="sub-table-container">
                            <table>
                              <thead>
                                <tr>
                                  <th>STT</th>
                                  <th>Tên thuộc tính</th>
                                  <th>Giá trị</th>
                                  <th>Đơn vị</th>
                                  <th>Mô tả</th>
                                </tr>
                              </thead>
                              <tbody>
                                {attr.properties.map((detail, detailIdx) => (
                                  <tr key={detail.id}>
                                    <td>{detailIdx + 1}</td>
                                    <td>{detail.name}</td>
                                    <td>{detail.valueString}</td>
                                    <td>{detail.valueUnitOfMeasure}</td>
                                    <td>{detail.description}</td>
                                  </tr>
                                ))}
                              </tbody>
                            </table>
                          </div>
                        </td>
                      </tr>
                    )}
                  </React.Fragment>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {/* === Tab: NHÂN SỰ === */}
        {activeTab === 'human' && (
          <div className="attributes-section">
            <h3>Yêu cầu nhân sự</h3>
            <table className="attributes-table">
              <thead>
                <tr>
                  <th>STT</th>
                  <th>Mã yêu cầu</th>
                  <th>Mã công đoạn</th>
                  <th>Vai trò nhân sự</th>
                  <th>Mô tả</th>
                  <th>Cách dùng nhân sự</th>
                  <th>Số lượng</th>
                  <th>Đơn vị</th>
                  <th>Chi tiết</th>
                </tr>
              </thead>
              <tbody>
                {humanSegmentSpecifications.map((attr, index) => (
                  <React.Fragment key={`human-${index}`}>
                    <tr>
                      <td>{index + 1}</td>
                      <td>{attr.personnelSegmentSpecificationId}</td>
                      <td>{attr.processSegmentId}</td>
                      <td>{attr.personnelClassName}</td>
                      <td>{attr.description}</td>
                      <td>Vận hành</td>
                      <td>{attr.quantity.valueString}</td>
                      <td>{attr.valueUnitOfMeasure}</td>
                      <td>
                        <button
                          className="view-btn"
                          onClick={() => toggleDetails('human', `human-${index}`)}
                          aria-label={`Xem chi tiết yêu cầu ${attr.requestCode}`}
                        >
                          <span className="view-icon"><i className="fas fa-eye"></i></span>
                        </button>
                      </td>
                    </tr>
                    {showDetails.human[`human-${index}`] && (
                      <tr>
                        <td colSpan="10">
                          <div className="sub-table-container">
                            <table>
                              <thead>
                                <tr>
                                  <th>STT</th>
                                  <th>Tên tiêu chí</th>
                                  <th>Giá trị</th>
                                  <th>Đơn vị</th>
                                  <th>Mô tả</th>
                                </tr>
                              </thead>
                              <tbody>
                                {attr.properties.map((detail, detailIdx) => (
                                  <tr key={detail.id}>
                                    <td>{detailIdx + 1}</td>
                                    <td>{detail.name}</td>
                                    <td>{detail.valueString}</td>
                                    <td>{detail.valueUnitOfMeasure}</td>
                                    <td>{detail.description}</td>
                                  </tr>
                                ))}
                              </tbody>
                            </table>
                          </div>
                        </td>
                      </tr>
                    )}
                  </React.Fragment>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {/* === Tab: VẬT LIỆU === */}
        {activeTab === 'material' && (
          <div className="attributes-section">
            <h3>Yêu cầu vật liệu</h3>
            <table className="attributes-table">
              <thead>
                <tr>
                  <th>STT</th>
                  <th>Mã yêu cầu</th>
                  <th>Mã công đoạn</th>
                  <th>Loại vật liệu</th>
                  <th>Vật liệu</th>
                  <th>Mô tả</th>
                  <th>Kiểu lắp ghép</th>
                  <th>Quan hệ lắp ghép</th>
                  <th>Cách dùng</th>
                  <th>Số lượng</th>
                  <th>Đơn vị</th>
                  <th>Chi tiết</th>
                </tr>
              </thead>
              <tbody>
                {materialSegmentSpecifications.map((attr, index) => (
                  <React.Fragment key={`material-${index}`}>
                    <tr>
                      <td>{index + 1}</td>
                      <td>{attr.materialSegmentSpecificationId}</td>
                      <td>{attr.processSegmentId}</td>
                      <td>{attr.materialClassName}</td>
                      <td>{attr.materialDefinitionName}</td>
                      <td>{attr.description}</td>
                      <td>{attr.assemblyType}</td>
                      <td>{attr.assemblyRelationship}</td>
                      <td>{attr.materialUse}</td>
                      <td>{attr.quantity.valueString}</td>
                      <td>{attr.valueUnitOfMeasure}</td>
                      <td>
                        <button
                          className="view-btn"
                          onClick={() => toggleDetails('material', `material-${index}`)}
                          aria-label={`Xem chi tiết yêu cầu ${attr.requestCode}`}
                        >
                          <span className="view-icon"><i className="fas fa-eye"></i></span>
                        </button>
                      </td>
                    </tr>
                    {showDetails.material[`material-${index}`] && (
                      <tr>
                        <td colSpan="13">
                          <div className="sub-table-container">
                            <table>
                              <thead>
                                <tr>
                                  <th>STT</th>
                                  <th>Tên thuộc tính</th>
                                  <th>Giá trị</th>
                                  <th>Đơn vị</th>
                                  <th>Mô tả</th>
                                </tr>
                              </thead>
                              <tbody>
                                {attr.properties.map((detail, detailIdx) => (
                                  <tr key={detail.id}>
                                    <td>{detailIdx + 1}</td>
                                    <td>{detail.name}</td>
                                    <td>{detail.valueString}</td>
                                    <td>{detail.valueUnitOfMeasure}</td>
                                    <td>{detail.description}</td>
                                  </tr>
                                ))}
                              </tbody>
                            </table>
                          </div>
                        </td>
                      </tr>
                    )}
                  </React.Fragment>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
};

export default StageDetail;