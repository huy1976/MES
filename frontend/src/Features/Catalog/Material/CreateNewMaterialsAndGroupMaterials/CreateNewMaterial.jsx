import React, { useState, useEffect, useRef } from 'react';
import { toast } from "react-toastify"; // Import toast for notifications
import "react-toastify/dist/ReactToastify.css";
import './CreateNewMaterial.css';
import { createMaterial } from '../../../../api/Materials';
import { getMaterialDefinitionPropertyNames } from '../../../../api/MaterialDefinition';

// Map hiển thị loại vật tư
const typeDisplayMap = {
  NVL: 'Nguyên vật liệu',
  TP: 'Thành phẩm',
  BTP: 'Bán thành phẩm',
};

// State ban đầu
const initialMaterialState = {
  type: '',
  code: '',
  name: '',
  unit: 'Kg',
  properties: [],
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

const CreateNewMaterial = ({ savedState, onSaveState }) => {
  // === KHÔI PHỤC STATE TỪ savedState NẾU CÓ ===
  const [material, setMaterial] = useState(
    savedState?.material || initialMaterialState
  );
  const [materialList, setMaterialList] = useState(
    savedState?.materialList || []
  );

  // === STATE MỚI ĐỂ LƯU TÊN THUỘC TÍNH ===
  const [propertyNames, setPropertyNames] = useState([]);

  // === [FIX] REF ĐỂ LƯU TRẠNG THÁI CỦA LẦN SAVE TRƯỚC ĐÓ ===
  const lastSavedStateRef = useRef('');

  // === EFFECT MỚI ĐỂ TẢI TÊN THUỘC TÍNH ===
  useEffect(() => {
    const fetchPropertyNames = async () => {
      try {
        const names = await getMaterialDefinitionPropertyNames();
        setPropertyNames(names || []);
      } catch (error) {
        console.error('Không thể tải danh sách tên thuộc tính:', error);
        setPropertyNames([]);
      }
    };
    fetchPropertyNames();
  }, []);

  // === [FIX] EFFECT: CHỈ SAVE KHI DỮ LIỆU THỰC SỰ THAY ĐỔI ===
  useEffect(() => {
    const currentData = { material, materialList };
    const currentDataString = JSON.stringify(currentData);

    if (lastSavedStateRef.current !== currentDataString) {
      if (onSaveState) {
        onSaveState(currentData);
      }
      lastSavedStateRef.current = currentDataString;
    }
  }, [material, materialList, onSaveState]);

  // === XỬ LÝ FORM CHÍNH ===
  const handleChange = (e) => {
    const { name, value } = e.target;
    setMaterial((prev) => ({ ...prev, [name]: value }));
  };

  // === XỬ LÝ BẢNG THUỘC TÍNH ĐỘNG ===
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
        { name: '', description: '', valueString: '', valueUnitOfMeasure: '' },
      ],
    }));
  };

  const removePropertyRow = (index) => {
    const updatedProperties = material.properties.filter((_, i) => i !== index);
    setMaterial((prev) => ({ ...prev, properties: updatedProperties }));
  };

  const handleAddToList = (e) => {
    e.preventDefault();
    if (!material.type || !material.code || !material.name) {
       toast.info("Vui lòng điền đầy đủ Loại, Mã và Tên vật tư!", {
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

    const filteredProperties = material.properties.filter(prop => {
      const isNameEmpty = !prop.name || prop.name.trim() === '';
      const isDescEmpty = !prop.description || prop.description.trim() === '';
      const isValueEmpty = !prop.valueString || prop.valueString.trim() === '';
      const isUnitEmpty = !prop.valueUnitOfMeasure || prop.valueUnitOfMeasure.trim() === '';
      return !(isNameEmpty && isDescEmpty && isValueEmpty && isUnitEmpty);
    });

    const newMaterial = {
      ...material,
      properties: filteredProperties,
      tempId: Date.now()
    };

    setMaterialList((prevList) => [...prevList, newMaterial]);
    setMaterial(initialMaterialState);
  };

  const handleDeleteFromList = (tempId) => {
    setMaterialList((prevList) => prevList.filter((item) => item.tempId !== tempId));
  };

  // === HÀM XÁC NHẬN (CÓ THÊM LOG) ===
  const handleConfirm = async () => {
    if (materialList.length === 0) {
        toast.info("Không có vật tư nào để xác nhận!", {
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
      const dynamicProperties = item.properties.map((p) => {
        const valueStr = p.valueString || '';
        return {
          name: p.name || 'N/A',
          description: p.description || 'N/A',
          valueString: valueStr,
          valueType: getValueType(valueStr),
          valueUnitOfMeasure: p.valueUnitOfMeasure || 'N/A',
        };
      });

      const payload = {
        materialDefinitionId: item.code,
        name: item.name,
        primaryUnit: item.unit,
        materialClass: item.type,
        assemblyType: 0,
        assemblyRelationship: 0,
        properties: dynamicProperties,
      };

      // --- LOG DỮ LIỆU TẠI ĐÂY ---
      console.log(`%c[API Request] Payload for ${item.code}:`, 'color: blue; font-weight: bold;', payload);
      // --------------------------

      try {
        await createMaterial(payload);
        successes.push(item.code);
        toast.success("Tạo vật tư thành công!", {
        position: "top-right",
        autoClose: 3000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
      });
      } catch (error) {
        toast.error("Lỗi: Không thể tạo vật tư!", {
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
    setMaterialList((prev) => prev.filter((item) => failures.includes(item.code)));
  };

  return (
    <div className="newmaterial-container">
      <form onSubmit={handleAddToList}>
        <div>
          <label>Loại vật tư</label>
          <select name="type" value={material.type} onChange={handleChange} required>
            <option value="">Chọn loại</option>
            <option value="NVL">Nguyên vật liệu</option>
            <option value="TP">Thành phẩm</option>
            <option value="BTP">Bán thành phẩm</option>
          </select>
        </div>

        <div>
          <label>Mã vật tư</label>
          <input type="text" name="code" value={material.code} onChange={handleChange} required />
        </div>

        <div>
          <label>Tên vật tư</label>
          <input type="text" name="name" value={material.name} onChange={handleChange} required />
        </div>

        <div>
          <label>Đơn vị</label>
          <select name="unit" value={material.unit} onChange={handleChange}>
            <option value="Kg">Kg</option>
            <option value="Lồng">Lồng</option>
            <option value="Phuy">Phuy</option>
            <option value="Cái">Cái</option>
            <option value="Không">Không</option>
          </select>
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

        <button type="submit" className="create-material-btn">
          Tạo
        </button>
      </form>

      <div className="material-list">
        <h2>Danh sách vật tư đã tạo</h2>
        <div className="table-newmaterial-container">
          <table>
            <thead>
              <tr>
                <th>STT</th>
                <th>Loại</th>
                <th>Mã</th>
                <th>Tên</th>
                <th>Đơn vị</th>
                <th>Số thuộc tính</th>
                <th>Hành động</th>
              </tr>
            </thead>
            <tbody>
              {materialList.length === 0 ? (
                <tr>
                  <td colSpan="7" style={{ textAlign: 'center' }}>
                    Chưa có vật tư nào.
                  </td>
                </tr>
              ) : (
                materialList.map((item, index) => (
                  <tr key={item.tempId}>
                    <td>{index + 1}</td>
                    <td>{typeDisplayMap[item.type] || item.type}</td>
                    <td>{item.code}</td>
                    <td>{item.name}</td>
                    <td>{item.unit}</td>
                    <td>{item.properties.length}</td>
                    <td>
                      <button type="button" className="delete-item-btn" onClick={() => handleDeleteFromList(item.tempId)}>
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

export default CreateNewMaterial;