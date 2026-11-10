import React, { useState, useEffect } from 'react';
import { toast } from "react-toastify"; // Import toast for notifications
import "react-toastify/dist/ReactToastify.css";
import './CreateNewMaterial.css';
import { getProcessSegmentIds } from '../../../../api/ProcessSegment';
import { getMaterialDefinitionIdList } from '../../../../api/MaterialDefinition';
import { getMaterialSegmentSpecificationPropertyNames, createMaterialSegmentSpecification } from '../../../../api/StageRequirement';

// Khởi tạo state ban đầu đầy đủ các trường
const initialMaterialState = {
  code: '',
  stageCode: '',
  type: '',
  description: '',
  assemblyType: '',       
  assemblyRelation: '',   
  usage: '',              
  quantity: '',
  unit: '',
  properties: [],
};

// Helper function để xác định valueType
const getValueType = (value) => {
  if (typeof value !== 'string') return 3;
  const trimmedValue = value.trim();
  if (trimmedValue === '') return 3;
  if (trimmedValue.toLowerCase() === 'true' || trimmedValue.toLowerCase() === 'false') return 0;
  if (isNaN(Number(trimmedValue))) return 3;
  if (trimmedValue.includes('.')) return 2;
  return 1;
};

const CreateNewMaterial = ({ savedState, onSaveState }) => {
  // === STATE QUẢN LÝ ===
  const [material, setMaterial] = useState(
    savedState?.material || initialMaterialState
  );
  const [materialList, setMaterialList] = useState(
    savedState?.materialList || []
  );

  // === STATE DATA API ===
  const [materialDefinitionIds, setMaterialDefinitionIds] = useState([]);
  const [processSegmentIds, setProcessSegmentIds] = useState([]);
  const [propertyNames, setPropertyNames] = useState([]);

  // === LOAD API BAN ĐẦU ===
  useEffect(() => {
    const GetApi = async () => {
      try {
        const [names, pIds, mIds] = await Promise.all([
            getMaterialSegmentSpecificationPropertyNames(),
            getProcessSegmentIds(),
            getMaterialDefinitionIdList()
        ]);

        setPropertyNames(names || []);
        setProcessSegmentIds(pIds || []);
        setMaterialDefinitionIds(mIds || []);
      } catch (error) {
        console.error("Lỗi tải dữ liệu ban đầu:", error);
      }
    };
    GetApi();
  }, []);

  // === LƯU TRẠNG THÁI ===
  useEffect(() => {
    if (onSaveState) {
      onSaveState({
        material,
        materialList,
      });
    }
  }, [material, materialList, onSaveState]);

  // === HANDLE INPUT ===
  const handleChange = (e) => {
    const { name, value } = e.target;
    setMaterial((prev) => ({ ...prev, [name]: value }));
  };

  const handlePropertyChange = (index, e) => {
    const { name, value } = e.target;
    const updatedProperties = [...material.properties];
    updatedProperties[index][name] = value;
    setMaterial((prev) => ({ ...prev, properties: updatedProperties }));
  };

  const addPropertyRow = () => {
    setMaterial((prev) => ({
      ...prev,
      properties: [
        ...prev.properties,
        { name: '', value: '', unit: '', description: '' },
      ],
    }));
  };

  const removePropertyRow = (index) => {
    const updatedProperties = material.properties.filter((_, i) => i !== index);
    setMaterial((prev) => ({ ...prev, properties: updatedProperties }));
  };

  // === THÊM VÀO DANH SÁCH TẠM ===
  const handleAddToList = (e) => {
    e.preventDefault();
    if (!material.code || !material.stageCode) {
       toast.info("Vui lòng điền đầy đủ Mã yêu cầu và Mã công đoạn!", {
                    position: "top-right",
                    autoClose: 3000,
                    hideProgressBar: false,
                    closeOnClick: true,
                    pauseOnHover: true,
                    draggable: true,
                    progress: undefined,
                  });
      return;
    }

    const filteredProperties = material.properties.filter((prop) => {
      return prop.name && prop.name.trim() !== '';
    });

    const newMaterial = {
      ...material,
      properties: filteredProperties,
      tempId: Date.now(),
    };

    setMaterialList((prevList) => [...prevList, newMaterial]);
    setMaterial(initialMaterialState); // Reset form sau khi thêm vào list
  };

  const handleDeleteFromList = (tempId) => {
    setMaterialList((prevList) => prevList.filter((item) => item.tempId !== tempId));
  };

  // === XÁC NHẬN GỬI API (ĐÃ CẬP NHẬT LOGIC CLEAR DATA) ===
  const handleConfirm = async () => {
    if (materialList.length === 0) {
        toast.info("Không có vật liệu nào để xác nhận!", {
                    position: "top-right",
                    autoClose: 3000,
                    hideProgressBar: false,
                    closeOnClick: true,
                    pauseOnHover: true,
                    draggable: true,
                    progress: undefined,
                  });
      return;
    }

    const successes = [];
    const failures = [];

    // Duyệt và gửi từng item
    for (const item of materialList) {
      
      const dynamicProperties = item.properties.map((p) => {
        const valueStr = p.value || '';
        return {
          propertyId: p.name,
          name: p.name || 'N/A',
          description: p.description || '',
          valueString: valueStr,
          valueType: getValueType(valueStr),
          valueUnitOfMeasure: p.unit || '',
        };
      });

      const materialSegmentSpec = {
        materialSegmentSpecificationId: item.code,
        description: item.description,
        assemblyType: parseInt(item.assemblyType) || 0,
        assemblyRelationship: parseInt(item.assemblyRelation) || 0,
        materialUse: parseInt(item.usage) || 0,
        materialDefinitionId: item.type,
        properties: dynamicProperties,
      };

      const payload = {
        processSegmentId: item.stageCode,
        materialSegmentSpecifications: [materialSegmentSpec]
      };

      console.log(`Sending payload for ${item.code}:`, JSON.stringify(payload, null, 2));

      try {
        await createMaterialSegmentSpecification(payload);
        successes.push(item.code);
      } catch (error) {
        console.error(`Lỗi khi tạo ${item.code}:`, error);
        failures.push(item.code);
      }
    }

    // === XỬ LÝ KẾT QUẢ SAU KHI CHẠY XONG VÒNG LẶP ===
    if (failures.length === 0) {
      // TRƯỜNG HỢP 1: THÀNH CÔNG TOÀN BỘ (Success 100%)
        toast.success("Tạo yêu cầu vật liệu thành công!", {
                    position: "top-right",
                    autoClose: 3000,
                    hideProgressBar: false,
                    closeOnClick: true,
                    pauseOnHover: true,
                    draggable: true,
                    progress: undefined,
              });
      
      // 1. Xóa dữ liệu hiển thị ở bảng
      setMaterialList([]); 
      
      // 2. Clear dữ liệu trong các input (reset về ban đầu)
      setMaterial(initialMaterialState);

    } else {
      // TRƯỜNG HỢP 2: CÓ LỖI XẢY RA
      // Giữ lại danh sách để người dùng biết cái nào lỗi mà sửa/gửi lại
      let message = `⚠️ Hoàn tất có lỗi!\n`;
      message += `- Thành công: ${successes.length}\n`;
      message += `- Thất bại: ${failures.length} (Mã: ${failures.join(', ')})`;
      toast.error("Lỗi: Không thể tạo yêu cầu vật liệu!", {
                    position: "top-right",
                    autoClose: 3000,
                    hideProgressBar: false,
                    closeOnClick: true,
                    pauseOnHover: true,
                    draggable: true,
                    progress: undefined,
                  });
      
      // Tùy chọn: Có thể lọc danh sách chỉ giữ lại những cái thất bại
      // setMaterialList(prev => prev.filter(item => failures.includes(item.code)));
    }
  };

  return (
    <div className="newmaterial-container">
      {/* --- FORM NHẬP LIỆU --- */}
      <form onSubmit={handleAddToList}>
        <div>
          <label>Mã yêu cầu (*)</label>
          <input type="text" name="code" value={material.code} onChange={handleChange} required />
        </div>
        
        <div>
          <label>Mã công đoạn (*)</label>
          <select name="stageCode" value={material.stageCode} onChange={handleChange} required>
            <option value="" disabled>Chọn Mã công đoạn</option>
            {processSegmentIds.map((id, index) => (
              <option key={index} value={id}>{id}</option>
            ))}
          </select>
        </div>

        <div>
          <label>Loại vật liệu (*)</label>
          <select name="type" value={material.type} onChange={handleChange} required>
            <option value="" disabled>Chọn loại vật liệu</option>
            {materialDefinitionIds.map((item) => (
              <option key={item.id} value={item.id}>{item.name}</option>
            ))}
          </select>
        </div>

        {/* --- CÁC TRƯỜNG BỔ SUNG --- */}
        <div>
          <label>Mô tả</label>
          <input type="text" name="description" value={material.description} onChange={handleChange} />
        </div>
        <div>
          <label>Kiểu lắp ghép (Số)</label>
          <input type="number" name="assemblyType" value={material.assemblyType} onChange={handleChange} placeholder="VD: 0, 1, 2" />
        </div>
        <div>
          <label>Quan hệ lắp ghép (Số)</label>
          <input type="number" name="assemblyRelation" value={material.assemblyRelation} onChange={handleChange} placeholder="VD: 0, 1, 2" />
        </div>
        <div>
          <label>Cách dùng (Số)</label>
          <input type="number" name="usage" value={material.usage} onChange={handleChange} placeholder="VD: 5" />
        </div>
        <div>
          <label>Số lượng</label>
          <input type="text" name="quantity" value={material.quantity} onChange={handleChange} />
        </div>
        <div>
          <label>Đơn vị</label>
          <input type="text" name="unit" value={material.unit} onChange={handleChange} />
        </div>

        {/* --- BẢNG THUỘC TÍNH ĐỘNG --- */}
        <div className="properties-section">
          <h4>Chi tiết thuộc tính</h4>
          <table className="properties-table">
            <thead>
              <tr>
                <th>Tên thuộc tính</th>
                <th>Giá trị</th>
                <th>Đơn vị</th>
                <th>Mô tả</th>
                <th>Xóa</th>
              </tr>
            </thead>
            <tbody>
              {material.properties.length === 0 ? (
                <tr>
                  <td colSpan="5" style={{ textAlign: 'center', color: '#999', fontStyle: 'italic' }}>
                    Chưa có thuộc tính. Bấm "Thêm thuộc tính" để bắt đầu.
                  </td>
                </tr>
              ) : (
                material.properties.map((prop, index) => (
                  <tr key={index}>
                    <td>
                      <select
                        name="name"
                        value={prop.name}
                        onChange={(e) => handlePropertyChange(index, e)}
                      >
                        <option value="" disabled>Chọn thuộc tính</option>
                        {propertyNames.map((name) => (
                          <option key={name} value={name}>
                            {name}
                          </option>
                        ))}
                      </select>
                    </td>
                    <td>
                      <input
                        type="text"
                        name="value"
                        value={prop.value}
                        onChange={(e) => handlePropertyChange(index, e)}
                        placeholder="Giá trị"
                      />
                    </td>
                    <td>
                      <input
                        type="text"
                        name="unit"
                        value={prop.unit}
                        onChange={(e) => handlePropertyChange(index, e)}
                        placeholder="Đơn vị"
                      />
                    </td>
                    <td>
                      <input
                        type="text"
                        name="description"
                        value={prop.description}
                        onChange={(e) => handlePropertyChange(index, e)}
                        placeholder="Mô tả"
                      />
                    </td>
                    <td>
                      <button type="button" className="delete-btn" onClick={() => removePropertyRow(index)}>
                        <i className="fas fa-trash"></i>
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
          <button type="button" onClick={addPropertyRow} className="add-property-btn">
            ➕ Thêm thuộc tính
          </button>
        </div>

        <button type="submit" className="create-material-btn">
          Thêm vào danh sách
        </button>
      </form>

      {/* --- DANH SÁCH TẠM --- */}
      <div className="material-list">
        <h2>Danh sách vật liệu chờ xác nhận</h2>
        <div className="table-newmaterial-container">
          <table>
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã yêu cầu</th>
                <th>Mã công đoạn</th>
                <th>Loại vật liệu</th>
                <th>Mô tả</th>
                <th>Kiểu LG</th>
                <th>Quan hệ LG</th>
                <th>Cách dùng</th>
                <th>Số lượng</th>
                <th>Đơn vị</th>
                <th>Số thuộc tính</th>
                <th>Hành động</th>
              </tr>
            </thead>
            <tbody>
              {materialList.length === 0 ? (
                <tr>
                  <td colSpan="12" style={{ textAlign: 'center' }}>
                    Chưa có vật liệu nào.
                  </td>
                </tr>
              ) : (
                materialList.map((item, index) => (
                  <tr key={item.tempId}>
                    <td>{index + 1}</td>
                    <td>{item.code}</td>
                    <td>{item.stageCode}</td>
                    <td>{item.type}</td>
                    <td>{item.description}</td>
                    <td>{item.assemblyType}</td>
                    <td>{item.assemblyRelation}</td>
                    <td>{item.usage}</td>
                    <td>{item.quantity}</td>
                    <td>{item.unit}</td>
                    <td>{item.properties.length}</td>
                    <td>
                      <button
                        type="button"
                        className="delete-item-btn"
                        onClick={() => handleDeleteFromList(item.tempId)}
                      >
                        Xóa
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
        
        {/* Nút xác nhận gửi API */}
        <button
          type="button"
          className="confirm-btn"
          onClick={handleConfirm}
          disabled={materialList.length === 0}
        >
          Xác nhận ({materialList.length})
        </button>
      </div>
    </div>
  );
};

export default CreateNewMaterial;