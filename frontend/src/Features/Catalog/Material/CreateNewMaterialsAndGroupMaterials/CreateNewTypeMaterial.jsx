import React, { useState, useEffect } from 'react';
import { toast } from "react-toastify"; // Import toast for notifications
import "react-toastify/dist/ReactToastify.css";
import './CreateNewMaterial.css';
import { createMaterialClass, getMaterialClassPropertyNames } from '../../../../api/MaterialClasses';

// === STATE BAN ĐẦU ===
const initialTypeMaterialState = {
  code: '',
  name: '',
  quantity: '',
  note: '',
  properties: []
};
// Helper function để xác định valueType
// 0=Boolean, 1=Integer, 2=Decimal, 3=String
const getValueType = (value) => {
  if (typeof value !== 'string') return 3;
  const trimmedValue = value.trim();
  if (trimmedValue === '') return 3;

  // Type 0: Boolean
  if (trimmedValue.toLowerCase() === 'true' || trimmedValue.toLowerCase() === 'false') {
    return 0;
  }

  // Kiểm tra xem có phải là số không
  if (isNaN(Number(trimmedValue))) {
    return 3;
  }

  // Nó LÀ một số. Giờ kiểm tra là integer hay decimal.
  if (trimmedValue.includes('.')) {
    return 2;
  }

  return 1;
};
// === HÀM HELPER ===
const createMaterialClassPayload = (item) => {
  const dynamicProperties = item.properties
    .filter(p => p.name && p.name.trim() !== '')
    .map((p) => {
      const valueStr = p.valueString || '';
      return {
        name: p.name || 'N/A',
        description: p.description || 'N/A',
        valueString: valueStr,
        valueType: getValueType(valueStr),
        valueUnitOfMeasure: p.valueUnitOfMeasure || 'N/A',
      };
    });

  return {
    materialClassId: item.code,
    name: item.name,
    description: item.note,
    assemblyType: 0,
    assemblyRelationship: 0,
    properties: dynamicProperties
  };
};

const CreateNewTypeMaterial = ({ savedState, onSaveState }) => {
  // === KHÔI PHỤC STATE TỪ savedState NẾU CÓ ===
  const [typeMaterial, setTypeMaterial] = useState(
    savedState?.typeMaterial || initialTypeMaterialState
  );
  const [materialList, setMaterialList] = useState(
    savedState?.materialList || []
  );

  // === STATE MỚI ĐỂ LƯU TÊN THUỘC TÍNH ===
    const [propertyNames, setPropertyNames] = useState([]);

      // === EFFECT MỚI ĐỂ TẢI TÊN THUỘC TÍNH ===
      useEffect(() => {
        const fetchPropertyNames = async () => {
          try {
            const names = await getMaterialClassPropertyNames();
            setPropertyNames(names || []);
          } catch (error) {
            console.error('Không thể tải danh sách tên thuộc tính:', error);
            setPropertyNames([]);
          }
        };
        fetchPropertyNames();
      }, []);
  

  // === EFFECT: SAVE STATE KHI typeMaterial HOẶC materialList THAY ĐỔI ===
  useEffect(() => {
    if (onSaveState) {
      onSaveState({
        typeMaterial,
        materialList,
      });
    }
  }, [typeMaterial, materialList, onSaveState]);

  // === XỬ LÝ FORM ===
  const handleChange = (e) => {
    const { name, value } = e.target;
    setTypeMaterial(prev => ({ ...prev, [name]: value }));
  };

 // === XỬ LÝ BẢNG THUỘC TÍNH ĐỘNG ===
  const handlePropertyChange = (index, e) => {
    const { name, value } = e.target;
    const updatedProperties = [...typeMaterial.properties];
    updatedProperties[index][name] = value;
    setTypeMaterial((prev) => ({ ...prev, properties: updatedProperties }));
  };

  const addPropertyRow = () => {
    setTypeMaterial((prev) => ({
      ...prev,
      properties: [
        ...prev.properties,
        { name: '', description: '', valueString: '', valueUnitOfMeasure: '' },
      ],
    }));
  };

  const removePropertyRow = (index) => {
    const updatedProperties = typeMaterial.properties.filter((_, i) => i !== index);
    setTypeMaterial((prev) => ({ ...prev, properties: updatedProperties }));
  };


  const handleAddToList = (e) => {
    e.preventDefault();
    if (!typeMaterial.code || !typeMaterial.name) {
        toast.info("Vui lòng điền đầy đủ Mã và Tên nhóm vật tư!", {
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

    const newItem = { ...typeMaterial, tempId: Date.now() };
    setMaterialList(prevList => [...prevList, newItem]);
    // === RESET FORM SAU KHI THÊM VÀO DANH SÁCH ===
    setTypeMaterial(initialTypeMaterialState);
  };

  const handleDeleteFromList = (tempIdToDelete) => {
    setMaterialList(prevList =>
      prevList.filter(item => item.tempId !== tempIdToDelete)
    );
  };

  const handleConfirm = async () => {
    if (materialList.length === 0) {
        toast.info("Không có nhóm vật tư  nào để xác nhận!", {
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

    for (const item of materialList) {
      const payload = createMaterialClassPayload(item);

      try {
        console.log('Đang gửi:', item.code);
        await createMaterialClass(payload);
        successes.push(item.code);
        toast.success("Tạo nhóm vật tư thành công!", {
        position: "top-right",
        autoClose: 3000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
      });


      } catch (error) {
        toast.error("Lỗi: Không thể tạo nhóm vật tư!", {
        position: "top-right",
        autoClose: 3000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
      });
        failures.push(item.code);
      }
    }

    // === GIỮ LẠI CÁC ITEM THẤT BẠI ===
    setMaterialList((prev) => prev.filter((item) => failures.includes(item.code)));
  };

  return (
    <div className="newmaterial-container">
      <form onSubmit={handleAddToList}>
        <div>
          <label>Mã nhóm vật tư</label>
          <input type="text" name="code" value={typeMaterial.code} onChange={handleChange} required />
        </div>
        <div>
          <label>Tên nhóm vật tư</label>
          <input type="text" name="name" value={typeMaterial.name} onChange={handleChange} required />
        </div>
        <div>
          <label>Ghi chú</label>
          <input type="text" name="note" value={typeMaterial.note} onChange={handleChange} />
        </div>

         <div className="properties-section">
          <h4>Thuộc tính bổ sung</h4>
          <table className="properties-table">
            <thead>
              <tr>
                <th>Tên thuộc tính</th>
                <th>Mô tả</th>
                <th>Giá trị</th>
                <th>Đơn vị</th>
                <th>Xóa</th>
              </tr>
            </thead>
            <tbody>
              {typeMaterial.properties.length === 0 ? (
                <tr>
                  <td colSpan="5" style={{ textAlign: 'center', color: '#999', fontStyle: 'italic' }}>
                    Chưa có thuộc tính. Bấm "Thêm thuộc tính" để bắt đầu.
                  </td>
                </tr>
              ) : (
                typeMaterial.properties.map((prop, index) => (
                  <tr key={index}>
                    <td>
                      <select
                        name="name"
                        value={prop.name}
                        onChange={(e) => handlePropertyChange(index, e)}
                      >
                        <option value="">Chọn thuộc tính</option>
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
                        name="description"
                        value={prop.description}
                        onChange={(e) => handlePropertyChange(index, e)}
                        placeholder="Mô tả thuộc tính"
                      />
                    </td>
                    <td>
                      <input
                        type="text"
                        name="valueString"
                        value={prop.valueString}
                        onChange={(e) => handlePropertyChange(index, e)}
                        placeholder="VD: 5 hoặc 10.5 hoặc true"
                      />
                    </td>
                    <td>
                      <input
                        type="text"
                        name="valueUnitOfMeasure"
                        value={prop.valueUnitOfMeasure}
                        onChange={(e) => handlePropertyChange(index, e)}
                        placeholder="VD: mm"
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


        <button type="submit" className="create-material-btn">Tạo</button>
      </form>

      <div className="material-list">
        <h2>Danh sách nhóm vật tư chờ xác nhận</h2>
        <div className="table-newmaterial-container">
          <table>
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã nhóm</th>
                <th>Tên nhóm</th>
                <th>Số vật tư</th>
                <th>Ghi chú</th>
                <th>Hành động</th>
              </tr>
            </thead>
            <tbody>
              {materialList.length === 0 ? (
                <tr>
                  <td colSpan="6" style={{ textAlign: 'center' }}>Chưa có nhóm vật tư nào.</td>
                </tr>
              ) : (
                materialList.map((item, index) => (
                  <tr key={item.tempId}>
                    <td>{index + 1}</td>
                    <td>{item.code}</td>
                    <td>{item.name}</td>
                    <td>{item.quantity}</td>
                    <td>{item.note || '-'}</td>
                    <td>
                      <button
                        type="button"
                        className="delete-btn"
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

export default CreateNewTypeMaterial;
