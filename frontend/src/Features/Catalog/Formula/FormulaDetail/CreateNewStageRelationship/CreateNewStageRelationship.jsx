import React, { useState } from 'react';
import './CreateNewStageRelationship.css';
import { createOperationRelationship } from '../../../../../api/OperationsDefinition';

// Định nghĩa danh sách tùy chọn để dùng chung (tránh hardcode nhiều chỗ)
const DEPENDENCY_OPTIONS = [
  { value: 0, label: 'Kết thúc - Bắt đầu' }, // FinishToStart
  { value: 1, label: 'Bắt đầu - Bắt đầu' },  // StartToStart
  { value: 2, label: 'Kết thúc - Kết thúc' },// FinishToFinish
  { value: 3, label: 'Bắt đầu - Kết thúc' }  // StartToFinish
];

const CreateNewStageRelationship = ({ formulaCode, onComplete, onBack }) => {
  // === STATE FORM NHẬP LIỆU ===
  const [stageRelationship, setStageRelationship] = useState({
    fromOperation: '',
    toOperation: '',
    description: '',
    dependencyType: '', // Sẽ lưu giá trị số (dạng string hoặc number từ input)
    coefficient: '',
    unit: '',
  });

  // === STATE DANH SÁCH TẠM ===
  const [stageRelationshipList, setStageRelationshipList] = useState([]);

  // === XỬ LÝ NHẬP LIỆU ===
  const handleChange = (e) => {
    const { name, value } = e.target;
    setStageRelationship(prev => ({ ...prev, [name]: value }));
  };

  // === THÊM VÀO DANH SÁCH TẠM (Client Side) ===
  const handleAddToList = (e) => {
    e.preventDefault();

    // Validate: Kiểm tra xem dependencyType có rỗng không (check != '' vì 0 là giá trị hợp lệ)
    if (!stageRelationship.fromOperation || !stageRelationship.toOperation || stageRelationship.dependencyType === '') {
      alert("Vui lòng nhập đủ Từ công đoạn, Đến công đoạn và Loại phụ thuộc.");
      return;
    }

    const newItem = {
      ...stageRelationship,
      // Chuyển sang kiểu số (Int) ngay lúc thêm vào list để đảm bảo nhất quán
      dependencyType: parseInt(stageRelationship.dependencyType),
      tempId: Date.now(),
    };

    setStageRelationshipList(prev => [...prev, newItem]);

    // Reset form
    setStageRelationship({
      fromOperation: '',
      toOperation: '',
      description: '',
      dependencyType: '', // Reset về rỗng
      coefficient: '',
      unit: '',
    });
  };

  // === XÓA KHỎI DANH SÁCH TẠM ===
  const handleDelete = (tempId) => {
    setStageRelationshipList(prev => prev.filter(item => item.tempId !== tempId));
  };

  //Helper để lấy tên hiển thị tiếng Việt từ số ID
  const getDependencyLabel = (value) => {
    const option = DEPENDENCY_OPTIONS.find(opt => opt.value === value);
    return option ? option.label : value;
  };

  // === GỬI API (XÁC NHẬN) ===
  const handleConfirm = async () => {
    if (stageRelationshipList.length === 0) {
      alert("Danh sách trống. Vui lòng thêm quan hệ trước khi xác nhận.");
      return;
    }

    const successes = [];
    const failures = [];

    for (const item of stageRelationshipList) {
      // 1. Chuẩn bị Payload
      const payload = {
        operationsDefinitionId: formulaCode || "string",
        fromSegmentId: item.fromOperation,
        toSegmentId: item.toOperation,
        description: item.description,
        dependencyType: item.dependencyType, // Đã là số 0, 1, 2, 3 từ lúc thêm vào list
        dependencyFactor: {
          valueString: item.coefficient ? item.coefficient.toString() : "0",
          valueType: 2
        },
        valueUnitOfMeasure: item.unit
      };

      try {
        console.log("Sending payload:", JSON.stringify(payload, null, 2));
        await createOperationRelationship(payload);
        successes.push(`${item.fromOperation}->${item.toOperation}`);
      } catch (error) {
        console.error("Error creating relationship:", error);
        failures.push(`${item.fromOperation}->${item.toOperation}`);
      }
    }

    // === XỬ LÝ KẾT QUẢ ===
    if (failures.length === 0) {
      alert(`✅ Đã tạo thành công ${successes.length} quan hệ công đoạn!`);
      setStageRelationshipList([]);
      if (onComplete) onComplete();
    } else {
      const message = `⚠️ Hoàn tất nhưng có lỗi!\n\n- Thành công: ${successes.length}\n- Thất bại: ${failures.length} (${failures.join(', ')})\n\nVui lòng kiểm tra lại các mục thất bại.`;
      alert(message);
    }
  };

  return (
    <div className="newstagerelationship-container">

      {/* --- FORM NHẬP LIỆU --- */}
      <div className="form-container">
        <form onSubmit={handleAddToList}>
          <div>
            <label>Từ Công đoạn</label>
            <input type="text" name="fromOperation" value={stageRelationship.fromOperation} onChange={handleChange} placeholder="VD: SEG01" required />
          </div>
          <div>
            <label>Đến Công đoạn</label>
            <input type="text" name="toOperation" value={stageRelationship.toOperation} onChange={handleChange} placeholder="VD: SEG02" required />
          </div>
          <div>
            <label>Mô tả</label>
            <textarea name="description" value={stageRelationship.description} onChange={handleChange} />
          </div>
          
          {/* --- SELECT LOẠI PHỤ THUỘC (Đã sửa) --- */}
          <div>
            <label>Loại Phụ thuộc</label>
            <select name="dependencyType" value={stageRelationship.dependencyType} onChange={handleChange} required>
              <option value="">Chọn loại phụ thuộc</option>
              {DEPENDENCY_OPTIONS.map((opt) => (
                <option key={opt.value} value={opt.value}>
                  {opt.label}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label>Hệ số</label>
            <input type="number" step="0.1" name="coefficient" value={stageRelationship.coefficient} onChange={handleChange} placeholder="VD: 30" />
          </div>
          <div>
            <label>Đơn vị</label>
            <input type="text" name="unit" value={stageRelationship.unit} onChange={handleChange} placeholder="VD: Phút, Giờ..." />
          </div>
          
          <button type="submit" className="create-stagerelationship-btn">Thêm vào danh sách</button>
        </form>
      </div>

      {/* --- DANH SÁCH TẠM --- */}
      <div className="stagerelationship-list">
        <h2>Danh sách Quan hệ chờ xác nhận</h2>
        <div className="table-newstagerelationship-container">
          <table>
            <thead>
              <tr>
                <th>STT</th>
                <th>Từ Công đoạn</th>
                <th>Đến Công đoạn</th>
                <th>Mô tả</th>
                <th>Loại Phụ thuộc</th>
                <th>Hệ số</th>
                <th>Đơn vị</th>
                <th>Xóa</th>
              </tr>
            </thead>
            <tbody>
              {stageRelationshipList.length === 0 ? (
                 <tr><td colSpan="8" style={{textAlign: 'center', color: '#888'}}>Chưa có dữ liệu</td></tr>
              ) : (
                stageRelationshipList.map((item, index) => (
                  <tr key={item.tempId}>
                    <td>{index + 1}</td>
                    <td>{item.fromOperation}</td>
                    <td>{item.toOperation}</td>
                    <td>{item.description}</td>
                    {/* Hiển thị Text thay vì số 0,1,2,3 */}
                    <td>{getDependencyLabel(item.dependencyType)}</td>
                    <td>{item.coefficient}</td>
                    <td>{item.unit}</td>
                    <td>
                      <button
                        className="delete-btn"
                        onClick={() => handleDelete(item.tempId)}
                        aria-label="Xóa"
                      >
                        <span className="delete-icon"><i className="fas fa-trash"></i></span>
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
        
        <button 
          type="button" 
          className="confirm-btn" 
          onClick={handleConfirm}
          disabled={stageRelationshipList.length === 0}
        >
          Xác nhận ({stageRelationshipList.length})
        </button>
      </div>
    </div>
  );
};

export default CreateNewStageRelationship;