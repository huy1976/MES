import React, { useState, useEffect } from 'react';
import { toast } from "react-toastify"; // Import toast for notifications
import "react-toastify/dist/ReactToastify.css";
import './CreateNewFormula.css';
import { getMaterialDefinitionIds } from '../../../../api/MaterialDefinition'; ;
import { getBillOfMaterialIds } from '../../../../api/Bom';
import { getProcessSegmentIds } from '../../../../api/ProcessSegment';
import { createOperationsDefinition } from '../../../../api/OperationsDefinition';

// Helper function để định dạng datetime-local sang ISO format
const formatDateTime = (datetimeString) => {
  if (!datetimeString) return null;
  const date = new Date(datetimeString);
  return date.toISOString(); // "2025-10-22T07:54:00.000Z"
};

const CreateNewFormula = ({ savedState, onSaveState }) => {
  // === STATE BAN ĐẦU ===
  const initialFormulaState = {
    code: '',
    name: '',
    materialId: '',
    version: '',
    operationsType: 0,
    availableDate: '',
    expiryDate: '',
    hierarchyScope: '--',
    workDefinitionId: '--',
    bomIds: [],
    segmentId: [],
  };

  // === KHÔI PHỤC STATE TỪ savedState NẾU CÓ ===
  const [formula, setFormula] = useState(
    savedState?.formula || initialFormulaState
  );
  const [formulaList, setFormulaList] = useState(
    savedState?.formulaList || []
  );

  const [materialDefinitionId, setMaterialDefinitionId] = useState([]);
  const [billOfMaterialIds, setBillOfMaterialIds] = useState([]);
  const [processSegmentIds, setProcessSegmentIds] = useState([]);

    // === EFFECT MỚI ĐỂ TẢI TÊN THUỘC TÍNH ===
      useEffect(() => {
        const GetApi = async () => {
          try {
            const materialIds = await getMaterialDefinitionIds();
            const bomIds =  await getBillOfMaterialIds();
            const segmentIds = await getProcessSegmentIds();

            setProcessSegmentIds(segmentIds);
            setBillOfMaterialIds(bomIds);
            setMaterialDefinitionId(materialIds);
          } catch (error) {
            console.error(error);
            setMaterialDefinitionId([]);
            setBillOfMaterialIds([]);
            setProcessSegmentIds([]);
          }
        };
        GetApi();
      }, []); 


  // === EFFECT: SAVE STATE KHI formula HOẶC formulaList THAY ĐỔI ===
  useEffect(() => {
    if (onSaveState) {
      onSaveState({
        formula,
        formulaList,
      });
    }
  }, [formula, formulaList, onSaveState]);

  // === XỬ LÝ FORM ===
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormula(prev => ({ ...prev, [name]: value }));
  };

  // === XỬ LÝ BẢNG BOM IDS ĐỘNG ===
  const handleBomIdChange = (index, value) => {
    const updatedBomIds = [...formula.bomIds];
    updatedBomIds[index] = value;
    setFormula(prev => ({ ...prev, bomIds: updatedBomIds }));
  };

  const addBomIdRow = () => {
    setFormula(prev => ({
      ...prev,
      bomIds: [...prev.bomIds, ''],
    }));
  };

  const removeBomIdRow = (index) => {
    const updatedBomIds = formula.bomIds.filter((_, i) => i !== index);
    setFormula(prev => ({ ...prev, bomIds: updatedBomIds }));
  };

  // === XỬ LÝ BẢNG SEGMENT ID ĐỘNG ===
  const handleSegmentIdChange = (index, value) => {
    const updatedSegmentId = [...formula.segmentId];
    updatedSegmentId[index] = value;
    setFormula(prev => ({ ...prev, segmentId: updatedSegmentId }));
  };

  const addSegmentIdRow = () => {
    setFormula(prev => ({
      ...prev,
      segmentId: [...prev.segmentId, ''],
    }));
  };

  const removeSegmentIdRow = (index) => {
    const updatedSegmentId = formula.segmentId.filter((_, i) => i !== index);
    setFormula(prev => ({ ...prev, segmentId: updatedSegmentId }));
  };

  const handleAddToList = (e) => {
    e.preventDefault();
    if (!formula.code || !formula.name || !formula.materialId) {
      toast.info("Vui lòng điền đầy đủ thông tin công thức!", {
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

    // === FILTER: CHỈ GIỮ LẠI NHỮNG ID CÓ DỮ LIỆU ===
    const filteredBomIds = formula.bomIds.filter(id => id && id.trim() !== '');
    const filteredSegmentIds = formula.segmentId.filter(id => id && id.trim() !== '');

    const newFormula = { 
      ...formula,
      bomIds: filteredBomIds,
      segmentId: filteredSegmentIds,
      tempId: Date.now() 
    };
    setFormulaList(prev => [...prev, newFormula]);
    // === RESET FORM SAU KHI THÊM VÀO DANH SÁCH ===
    setFormula(initialFormulaState);
  };

  const handleDeleteFromList = (tempId) => {
    setFormulaList(prev => prev.filter(item => item.tempId !== tempId));
  };

  const handleConfirm = async () => {
    if (formulaList.length === 0) {
        toast.info("Không có công thức nào để xác nhận!", {
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

    for (const item of formulaList) {
      const payload = {
        operationsDefinitionId: item.code,
        description: item.name,
        materialId: item.materialId,
        version: item.version,
        operationsType: item.operationsType,
        validFrom: formatDateTime(item.availableDate),
        validTo: formatDateTime(item.expiryDate),
        hierarchyScope: item.hierarchyScope,
        workDefinitionId: item.workDefinitionId,
        bomIds: item.bomIds,
        segmentIds: item.segmentId,
      };

      try {
        console.log("Đang gửi payload:", JSON.stringify(payload, null, 2));
        createOperationsDefinition(payload);  
        successes.push(item.code);
        toast.success("Tạo công thức thành công!", {
        position: "top-right",
        autoClose: 3000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
      });
      } catch (error) {
        toast.error("Lỗi: Không thể tạo công thức!", {
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

    setFormulaList(prev => prev.filter(item => failures.includes(item.code)));
  };

  return (
    <>
      {/* TITLE */}
      <h2 className="title">Danh mục | Công thức</h2>

      {/* TABS */}
      <div className="tabs">
        <button className="tab-btn active">Công thức</button>
      </div>

      <div className="newformula-container">
        <form onSubmit={handleAddToList}>
          <div>
            <label>Mã Công thức</label>
            <input type="text" name="code" value={formula.code} onChange={handleChange} required />
          </div>
          <div>
            <label>Tên Công thức</label>
            <input type="text" name="name" value={formula.name} onChange={handleChange} required />
          </div>
          <div>
            <label>Sản phẩm đầu ra</label>
            <select name="materialId" value={formula.materialId} onChange={handleChange} required>
                <option value="" disabled>Chọn sản phẩm</option>
                {materialDefinitionId.map((materialid, item) => (
                  <option key={item} value={materialid}>{materialid}</option>
                ))}
            </select>
          </div>
          <div>
            <label>Phiên bản</label>
            <input type="text" name="version" value={formula.version} onChange={handleChange} />
          </div>
          <div>
            <label>Ngày Khả dụng</label>
            <input type="date" name="availableDate" value={formula.availableDate} onChange={handleChange} />
          </div>
          <div>
            <label>Ngày Hết hạn</label>
            <input type="date" name="expiryDate" value={formula.expiryDate} onChange={handleChange} />
          </div>

          {/* === BẢNG BOM IDS ĐỘNG === */}
          <div className="properties-section">
            <h4>BOM IDs</h4>
            <table className="properties-table">
              <thead>
                <tr>
                  <th>BOM ID</th>
                  <th>Xóa</th>
                </tr>
              </thead>
              <tbody>
                {formula.bomIds.length === 0 ? (
                  <tr>
                    <td colSpan="2" style={{ textAlign: 'center', color: '#999', fontStyle: 'italic' }}>
                      Chưa có BOM ID. Bấm "Thêm BOM ID" để bắt đầu.
                    </td>
                  </tr>
                ) : (
                  formula.bomIds.map((bomId, index) => (
                    <tr key={index}>
                      <td>
                        <select
                          type="text"
                          value={bomId}
                          onChange={(e) => handleBomIdChange(index, e.target.value)}
                          placeholder="VD: BOM001"
                        >
                          <option value="" disabled selected >Chọn BOMId</option>
                          {billOfMaterialIds.map((id) => (
                            <option key={id} value={id}>
                              {id}
                            </option>
                          ))}
                        </select>
                      </td>
                      <td>
                        <button type="button" className="delete-btn" onClick={() => removeBomIdRow(index)}>
                          <i className="fas fa-trash"></i>
                        </button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
            <button type="button" onClick={addBomIdRow} className="add-row-btn">
              ➕ Thêm BOM ID
            </button>
          </div>

          {/* === BẢNG SEGMENT ID ĐỘNG === */}
          <div className="properties-section">
            <h4>Segment IDs</h4>
            <table className="properties-table">
              <thead>
                <tr>
                  <th>Segment ID</th>
                  <th>Xóa</th>
                </tr>
              </thead>
              <tbody>
                {formula.segmentId.length === 0 ? (
                  <tr>
                    <td colSpan="2" style={{ textAlign: 'center', color: '#999', fontStyle: 'italic' }}>
                      Chưa có Segment ID. Bấm "Thêm Segment ID" để bắt đầu.
                    </td>
                  </tr>
                ) : (
                  formula.segmentId.map((segId, index) => (
                    <tr key={index}>
                      <td>
                        <select
                          type="text"
                          value={segId}
                          onChange={(e) => handleSegmentIdChange(index, e.target.value)}
                          placeholder="VD: SEG001"
                        >
                          <option value="" disabled selected >Chọn SegmentId</option>
                          {processSegmentIds.map((id) => (
                            <option key={id} value={id}>
                              {id}
                            </option>
                          ))}    
                        </select>
                      </td>
                      <td>
                        <button type="button" className="delete-btn" onClick={() => removeSegmentIdRow(index)}>
                          <i className="fas fa-trash"></i>
                        </button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
            <button type="button" onClick={addSegmentIdRow} className="add-row-btn">
              ➕ Thêm Segment ID
            </button>
          </div>

          <button type="submit" className="create-formula-btn">Tạo</button>
        </form>

        <div className="formula-list">
          <h2>Danh sách công thức chờ xác nhận</h2>
          <div className="table-newformula-container">
            <table>
              <thead>
                <tr>
                  <th>STT</th>
                  <th>Mã Công thức</th>
                  <th>Tên Công thức</th>
                  <th>Sản phẩm đầu ra</th>
                  <th>Phiên bản</th>
                  <th>Ngày Khả dụng</th>
                  <th>Ngày Hết hạn</th>
                  <th>BOM IDs</th>
                  <th>Segment IDs</th>
                  <th>Hành động</th>
                </tr>
              </thead>
              <tbody>
                {formulaList.length === 0 ? (
                  <tr>
                    <td colSpan="13" style={{ textAlign: 'center' }}>Chưa có công thức nào.</td>
                  </tr>
                ) : (
                  formulaList.map((item, index) => (
                    <tr key={item.tempId}>
                      <td>{index + 1}</td>
                      <td>{item.code}</td>
                      <td>{item.name}</td>
                      <td>{item.materialId}</td>
                      <td>{item.version}</td>
                      <td>{item.availableDate}</td>
                      <td>{item.expiryDate}</td>
                      <td>{item.bomIds.join(', ')}</td>
                      <td>{item.segmentId.join(', ')}</td>
                      <td>
                        <button
                          type="button"
                          className="delete-btn"
                          onClick={() => handleDeleteFromList(item.tempId)}
                          aria-label={`Xóa Công thức ${item.code}`}
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
            disabled={formulaList.length === 0}
          >
            Xác nhận ({formulaList.length})
          </button>
        </div>
      </div>
    </>
  );
};

export default CreateNewFormula;