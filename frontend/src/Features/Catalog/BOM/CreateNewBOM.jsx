import React, { useState, useEffect } from 'react';
import { toast } from "react-toastify"; // Import toast for notifications
import "react-toastify/dist/ReactToastify.css";
import './CreateNewBOM.css';
import { createBillOfMaterial } from '../../../api/Bom';
import { getMaterialDefinitionIdsByFinishedProductNVL } from '../../../api/QualityStandard';

const initialBomState = {
  code: '',
  materialDefinitionId: '',
  quantity: '',
  unit: '',
  description: '',
};

const getValueType = (value) => {
  if (typeof value !== 'string') return 3;
  const trimmedValue = value.trim();
  if (trimmedValue === '') return 3;
  if (trimmedValue.toLowerCase() === 'true' || trimmedValue.toLowerCase() === 'false') return 0;
  if (isNaN(Number(trimmedValue))) return 3;
  if (trimmedValue.includes('.')) return 2;
  return 1;
};

const CreateNewBOM = () => {
  const [bom, setBom] = useState(initialBomState);
  const [bomList, setBomList] = useState([]);
  
  // Sửa tên state thành materials để thể hiện nó chứa danh sách object
  const [materials, setMaterials] = useState([]);

  useEffect(() => {
    const fetchMaterials = async () => {
      try {
        const data = await getMaterialDefinitionIdsByFinishedProductNVL();
        // data trả về là mảng object [{ materialId, materialName }, ...]
        setMaterials(data || []);
      } catch (error) {
        console.error('Không thể tải danh sách vật liệu:', error);
        setMaterials([]);
      }
    };
    fetchMaterials();
  }, []);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setBom(prev => ({ ...prev, [name]: value }));
  };

  const handleAddToList = (e) => {
    e.preventDefault();
    if (!bom.code || !bom.materialDefinitionId) {
        toast.info("Vui lòng điền đầy đủ Mã BOM và Vật liệu đầu vào!", {
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
    const newBom = { ...bom, tempId: Date.now() };
    setBomList(prev => [...prev, newBom]);
    setBom(initialBomState);
  };

  const handleDeleteFromList = (tempId) => {
    setBomList(prev => prev.filter(item => item.tempId !== tempId));
  };

  const handleConfirm = async () => {
    if (bomList.length === 0) {
        toast.info("Không có BOM nào để xác nhận!", {
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

    for (const item of bomList) {
      const quantityStr = item.quantity || '';
      const payload = {
        billOfMaterialId: item.code,
        description: item.description,
        materialUse: 0,
        assemblyType: 0,
        assemblyRelationship: 0,
        valueString: quantityStr,
        valueType: getValueType(quantityStr),
        valueUnitOfMeasure: item.unit,
        materialDefinitionId: item.materialDefinitionId, // Đây sẽ là ID
      };
      try {
        await createBillOfMaterial(payload);
        successes.push(item.code);
        toast.success("Tạo BOM thành công!", {
        position: "top-right",
        autoClose: 3000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
      });

      } catch (error) {
        toast.error("Lỗi: Không thể tạo BOM!", {
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

    setBomList(prev => prev.filter(item => failures.includes(item.code)));
  };

  return (
    <div className="newbom-container">
      <form onSubmit={handleAddToList}>
        <div>
          <label>Mã BOM</label>
          <input 
            type="text" 
            name="code" 
            value={bom.code} 
            onChange={handleChange} 
            aria-label="Mã BOM"
            required 
          />
        </div>
        <div>
          <label>Vật liệu đầu vào</label>
          <select
            name="materialDefinitionId"
            value={bom.materialDefinitionId}
            onChange={handleChange}
            aria-label="Vật liệu đầu vào"
            required
          >
            <option value="">Chọn vật liệu</option>
            {/* Sửa lại phần map để lấy đúng trường ID và Name */}
            {materials.map((item) => (
              <option key={item.materialId} value={item.materialId}>
                {item.materialName}
              </option>
            ))}
          </select>
        </div>
        <div>
          <label>Số lượng</label>
          <input 
            type="number" 
            name="quantity" 
            value={bom.quantity} 
            onChange={handleChange} 
            aria-label="Số lượng"
            required
          />
        </div>
        <div>
          <label>Đơn vị</label>
          <input 
            type="text" 
            name="unit" 
            value={bom.unit} 
            onChange={handleChange} 
            aria-label="Đơn vị" 
            required
          />
        </div>
        <div>
          <label>Mô tả</label>
          <input 
            type="text" 
            name="description" 
            value={bom.description} 
            onChange={handleChange} 
            aria-label="Mô tả" 
          />
        </div>
        <button type="submit" className="create-bom-btn">Tạo</button>
      </form>

      <div className="bom-list">
        <h2>Danh sách BOM đã tạo</h2>
        <div className="table-newbom-container">
          <table>
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã BOM</th>
                <th>Vật liệu đầu vào (ID)</th>
                <th>Số lượng</th>
                <th>Đơn vị</th>
                <th>Mô tả</th>
                <th>Xóa</th>
              </tr>
            </thead>
            <tbody>
              {bomList.length === 0 ? (
                <tr>
                  <td colSpan="7" style={{ textAlign: 'center' }}>Chưa có BOM nào.</td>
                </tr>
              ) : (
                bomList.map((item, index) => (
                  <tr key={item.tempId}>
                    <td>{index + 1}</td>
                    <td>{item.code}</td>
                    <td>{item.materialDefinitionId}</td>
                    <td>{item.quantity}</td>
                    <td>{item.unit}</td>
                    <td>{item.description}</td>
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
        <button 
          type="button" 
          className="confirm-btn"
          onClick={handleConfirm}
          disabled={bomList.length === 0}
        >
          Xác nhận ({bomList.length})
        </button>
      </div>
    </div>
  );
};

export default CreateNewBOM;