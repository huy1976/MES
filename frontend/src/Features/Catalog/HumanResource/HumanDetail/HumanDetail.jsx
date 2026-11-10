// src/components/HumanDetail/HumanDetail.js
import React from 'react';
import './HumanDetail.css';

const HumanDetail = ({ human }) => {
  const {
    name,
    code,
    phone,
    type,
    department,
    capacities,
    shift,
    status,
    raw,
  } = human;
  
  // === LẤY TẤT CẢ PROPERTIES TRONG personnelCapability ===
  const capProps = raw?.personnelCapabilities[0].properties || [];
console.log('capProps', capProps);
  // Chuẩn hóa tất cả properties (không lọc ca làm việc)
  const allCapabilities = capProps
    .map((prop, index) => ({
      stt: index + 1,
      name: prop.name,
      description: prop.description,
      value: prop.valueString === 'true' ? 'Có' : 'Không',
    }))
    .sort((a, b) => a.stt - b.stt); // Sắp xếp theo thứ tự gốc

  return (
    <div className="human-detail-container">
      {/* ==================== HEADER THÔNG TIN CƠ BẢN ==================== */}
      <div className="detail-header">
        <h2>Thông tin chi tiết nhân sự</h2>
        <div className="info-table-wrapper">
          <table className="info-table">
            <thead>
              <tr>
                <th>Họ và tên</th>
                <th>Mã nhân viên</th>
                <th>Số điện thoại</th>
                <th>Lớp nhân sự</th>
                <th>Số năng lực</th>
                <th>Ca làm việc</th>
                <th>Trạng thái</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td><strong>{name}</strong></td>
                <td>{code}</td>
                <td>{phone}</td>
                <td>{type}</td>
                <td><strong>{capacities}</strong></td>
                <td>{shift}</td>
                <td>
                  <span className={`status-badge status-${status.replace(/\s+/g, '-').toLowerCase()}`}>
                    {status}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      {/* ==================== BẢNG TẤT CẢ NĂNG LỰC (BAO GỒM CA LÀM VIỆC) ==================== */}
      <div className="content">
        <div className="capacities-section">
          <h3>Tất cả năng lực & kỹ năng</h3>
          {allCapabilities.length > 0 ? (
            <div className="table-wrapper">
              <table className="capacities-table">
                <thead>
                  <tr>
                    <th style={{ width: '60px' }}>STT</th>
                    <th style={{ width: '180px' }}>Tên năng lực</th>
                    <th>Mô tả</th>
                    <th style={{ width: '80px' }}>Khả năng</th>
                  </tr>
                </thead>
                <tbody>
                  {allCapabilities.map((cap) => (
                    <tr key={cap.stt} className={cap.name.includes('shift') ? 'shift-row' : ''}>
                      <td className="text-center">{cap.stt}</td>
                      <td>
                        <strong>{cap.name}</strong>
                        {cap.name.includes('shift') && (
                          <span className="shift-label"> (Ca làm việc)</span>
                        )}
                      </td>
                      <td>{cap.description}</td>
                      <td className="text-center">
                        <span
                          className={`ability-badge ${cap.value === 'Có' ? 'yes' : 'no'}`}
                        >
                          {cap.value}
                        </span>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : (
            <p className="no-data">Không có dữ liệu năng lực.</p>
          )}
        </div>
      </div>
    </div>
  );
};

export default HumanDetail;