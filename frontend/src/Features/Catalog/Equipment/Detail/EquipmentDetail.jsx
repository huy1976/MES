import React, { useState } from 'react';
import './EquipmentDetail.css';

const EquipmentDetail = ({ equipment }) => {
  const [activeTab, setActiveTab] = useState('specifications');
  
  // Chuyển đổi properties thành attributes
  const attributes = (equipment.rawData.properties || []).map((prop, index) => ({
    id: index + 1,
    name: prop.description,
    value: prop.valueString,
    unit: prop.valueUnitOfMeasure === 'None' ? '' : prop.valueUnitOfMeasure,
  }));

  // Chuyển đổi equipmentCapability.properties thành capabilities
  const capabilities = (equipment.rawData.equipmentCapabilities || []).map((prop, index) => ({
    id: index + 1,
    name: prop.description,
    value: prop.quantity.valueString,
    unit: prop.valueUnitOfMeasure === 'None' ? '' : prop.valueUnitOfMeasure,
  }));

  // === HELPER: MÀU SẮC TRẠNG THÁI ===
  const getStatusStyle = (statusName) => {
    switch (statusName) {
      case 'Đang chạy': // Xanh lá
        return { backgroundColor: '#dcfce7', color: '#15803d', border: '1px solid #15803d' };
      case 'Khả dụng': // Xanh dương
        return { backgroundColor: '#e0f2fe', color: '#0369a1', border: '1px solid #0369a1' };
      case 'Đã được phân công': // Cam
        return { backgroundColor: '#ffedd5', color: '#c2410c', border: '1px solid #c2410c' };
      case 'Bị sự cố': // Đỏ
        return { backgroundColor: '#fee2e2', color: '#b91c1c', border: '1px solid #b91c1c' };
      case 'Đang bảo trì': // Vàng/Nâu
        return { backgroundColor: '#fef3c7', color: '#b45309', border: '1px solid #b45309' };
      default: // Xám
        return { backgroundColor: '#f3f4f6', color: '#374151', border: '1px solid #9ca3af' };
    }
  };
  
  return (
    <div className="equipment-detail-container">
      {/* Header */}
      <div className="detail-header">
        <h2>Thông tin chi tiết của {equipment.code}</h2>
        <table className="info-row">
          <thead>
            <tr>
              <th>STT</th>
              <th>Loại thiết bị</th>
              <th>Mã thiết bị</th>
              <th>Tên thiết bị</th>
              <th>Model</th>
              <th>Nhà sản xuất</th>
              <th>Trạng thái</th>
              <th>Vị trí lắp đặt</th>
              
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>{equipment.stt}</td>
              <td>{equipment.type}</td>
              <td>{equipment.code}</td>
              <td><strong>{equipment.name}</strong></td>
              <td>{equipment.model}</td>
              <td>{equipment.manufacturer}</td>
              <td>
                {/* ÁP DỤNG STYLE BADGE MỚI */}
                <span
                  className="status-badge-styled"
                  style={getStatusStyle(equipment.status)}
                >
                  {equipment.status}
                </span>
              </td>
              <td>{equipment.workCenter}</td>
             
            </tr>
          </tbody>
        </table>
      </div>

      {/* Tabs */}
      <div className="tabs">
        <button
          className={`tab ${activeTab === 'specifications' ? 'active' : ''}`}
          onClick={() => setActiveTab('specifications')}
          aria-label="Xem thuộc tính thiết bị"
        >
          Thuộc tính ({attributes.length})
        </button>
        <button
          className={`tab ${activeTab === 'capabilities' ? 'active' : ''}`}
          onClick={() => setActiveTab('capabilities')}
          aria-label="Xem năng lực thiết bị"
        >
          Năng lực ({capabilities.length})
        </button>
      </div>

      {/* Content */}
      <div className="content">
        {/* === Tab: THUỘC TÍNH === */}
        {activeTab === 'specifications' && (
          <div className="attributes-section">
            <h3>Thuộc tính thiết bị</h3>
            {attributes.length === 0 ? (
              <div className="empty-state"><p>Không có thuộc tính nào.</p></div>
            ) : (
              <table className="attributes-table">
                <thead>
                  <tr>
                    <th>STT</th>
                    <th>Tên thuộc tính</th>
                    <th>Giá trị</th>
                    <th>Đơn vị</th>
                  </tr>
                </thead>
                <tbody>
                  {attributes.map((attr) => (
                    <tr key={attr.id}>
                      <td data-label="STT">{attr.id}</td>
                      <td data-label="Tên thuộc tính"><strong>{attr.name}</strong></td>
                      <td data-label="Giá trị" className="value-cell">{attr.value}</td>
                      <td data-label="Đơn vị">{attr.unit}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>
        )}

        {/* === Tab: NĂNG LỰC === */}
        {activeTab === 'capabilities' && (
          <div className="attributes-section">
            <h3>Năng lực thiết bị</h3>
            {capabilities.length === 0 ? (
              <div className="empty-state"><p>Không có năng lực nào.</p></div>
            ) : (
              <table className="attributes-table">
                <thead>
                  <tr>
                    <th>STT</th>
                    <th>Tên năng lực</th>
                    <th>Giá trị</th>
                    <th>Đơn vị</th>
                  </tr>
                </thead>
                <tbody>
                  {capabilities.map((cap) => (
                    <tr key={cap.id}>
                      <td data-label="STT">{cap.id}</td>
                      <td data-label="Tên năng lực"><strong>{cap.name}</strong></td>
                      <td data-label="Giá trị" className="value-cell">{cap.value}</td>
                      <td data-label="Đơn vị">{cap.unit}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default EquipmentDetail;