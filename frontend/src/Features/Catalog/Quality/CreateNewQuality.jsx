import React, { useState, useEffect } from 'react';
import { toast } from "react-toastify"; // Import toast for notifications
import "react-toastify/dist/ReactToastify.css";
import './CreateNewQuality.css';
import { getMaterialDefinitionIdsByFinishedProductTP } from '../../../api/QualityStandard';
import {
  getQualityStandardPropertyNames,
  createQualityStandard,
} from '../../../api/QualityStandard';

/* ========================================
   INITIAL STATE
======================================== */
const initialQualityState = {
  code: '',
  productCode: '',
  startDate: '',
  endDate: '',
  note: '',
  properties: [],
};

/* ========================================
   HELPER FUNCTIONS
======================================== */
const getValueType = (value) => {
  if (typeof value !== 'string') return 3;

  const trimmed = value.trim();
  if (trimmed === '') return 3;
  if (trimmed.toLowerCase() === 'true' || trimmed.toLowerCase() === 'false')
    return 0;
  if (isNaN(Number(trimmed))) return 3;
  if (trimmed.includes('.')) return 2;

  return 1;
};

/* ========================================
   MAIN COMPONENT
======================================== */
const CreateNewQuality = () => {
  const [quality, setQuality] = useState(initialQualityState);
  const [qualityList, setQualityList] = useState([]);

  const [propertyNames, setPropertyNames] = useState([]);
  const [materialNames, setMaterialNames] = useState([]);

  /* ========================================
     LOAD PROPERTY NAMES + MATERIAL NAMES
  ======================================== */
  useEffect(() => {
    const fetchData = async () => {
      try {
        const [qNames, mNames] = await Promise.all([
          getQualityStandardPropertyNames(),
          getMaterialDefinitionIdsByFinishedProductTP(),
        ]);

        setPropertyNames(qNames || []);
        setMaterialNames(mNames || []);
      } catch (error) {
        console.error('Không thể tải dữ liệu ban đầu:', error);
        setPropertyNames([]);
        setMaterialNames([]);
      }
    };

    fetchData();
  }, []);

  /* ========================================
     INPUT HANDLERS
  ======================================== */
  const handleQualityChange = (e) => {
    const { name, value } = e.target;
    setQuality((prev) => ({ ...prev, [name]: value }));
  };

  const handlePropertyChange = (index, e) => {
    const { name, value } = e.target;

    const updated = [...quality.properties];
    updated[index][name] = value;

    setQuality((prev) => ({ ...prev, properties: updated }));
  };

  const addPropertyRow = () => {
    setQuality((prev) => ({
      ...prev,
      properties: [
        ...prev.properties,
        {
          name: '',
          description: '',
          valueString: '',
          valueUnitOfMeasure: '',
        },
      ],
    }));
  };

  const removePropertyRow = (index) => {
    const updated = quality.properties.filter((_, i) => i !== index);
    setQuality((prev) => ({ ...prev, properties: updated }));
  };

  /* ========================================
     ADD QUALITY TO TEMP LIST
  ======================================== */
  const handleAddToList = (e) => {
    e.preventDefault();

    if (!quality.code || !quality.productCode) {
      toast.info("Vui lòng điền đầy đủ Mã tiêu chuẩn và Mã sản phẩm!", {
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

    const filteredProps = quality.properties.filter((p) => {
      const emptyName = !p.name || p.name.trim() === '';
      const emptyValue = !p.valueString || p.valueString.trim() === '';
      return !(emptyName && emptyValue);
    });

    const newQuality = {
      ...quality,
      properties: filteredProps,
      tempId: Date.now(),
    };

    setQualityList((prev) => [...prev, newQuality]);
    setQuality(initialQualityState);
  };

  const handleDeleteFromList = (tempId) => {
    setQualityList((prev) => prev.filter((q) => q.tempId !== tempId));
  };

  /* ========================================
     CONFIRM SUBMIT TO SERVER
  ======================================== */
  const handleConfirm = async () => {
    if (qualityList.length === 0) {
        toast.info("Không có tiêu chuẩn nào để xác nhận!", {
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

    for (const item of qualityList) {
      const dynamicProperties = item.properties.map((p) => {
        const val = p.valueString || '';

        return {
          name: p.name || 'N/A',
          description: p.description || 'N/A',
          valueString: val,
          valueType: getValueType(val),
          valueUnitOfMeasure: p.valueUnitOfMeasure || 'N/A',
        };
      });

      const payload = {
        qualityStandardId: item.code,
        description: item.note,
        validFrom: item.startDate ? `${item.startDate}T00:00:00.000Z` : null,
        validTo: item.endDate ? `${item.endDate}T23:59:59.999Z` : null,
        properties: dynamicProperties,
        materialDefinitionIds: [item.productCode],
      };

      try {
        await createQualityStandard(payload);
        successes.push(item.code);
        toast.success("Tạo tiêu chuẩn thành công!", {
        position: "top-right",
        autoClose: 3000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
      });
      } catch (err) {
        toast.error("Lỗi: Không thể tạo tiêu chuẩn!", {
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

    setQualityList((prev) => prev.filter((item) => failures.includes(item.code)));
  };

  /* ========================================
     RENDER
  ======================================== */
  return (
    <div className="newquality-container">
      {/* FORM ==================== */}
      <form onSubmit={handleAddToList}>
        <div>
          <label>Mã tiêu chuẩn</label>
          <input
            type="text"
            name="code"
            value={quality.code}
            onChange={handleQualityChange}
            required
          />
        </div>

        <div>
  <label>Mã sản phẩm</label>
  <select
    name="productCode"
    value={quality.productCode}
    onChange={handleQualityChange}
    required
  >
    <option value="">Chọn mã sản phẩm</option>
    {/* SỬA ĐOẠN NÀY */}
    {materialNames.map((m) => (
      <option key={m.materialId} value={m.materialId}>
        {m.materialName} {/* Hoặc hiển thị {m.materialId} - {m.materialName} tùy bạn */}
      </option>
    ))}
  </select>
</div>

        <div>
          <label>Thời gian áp dụng</label>
          <input
            type="date"
            name="startDate"
            value={quality.startDate}
            onChange={handleQualityChange}
          />
        </div>

        <div>
          <label>Thời gian kết thúc</label>
          <input
            type="date"
            name="endDate"
            value={quality.endDate}
            onChange={handleQualityChange}
          />
        </div>

        <div>
          <label>Ghi chú</label>
          <input
            type="text"
            name="note"
            value={quality.note}
            onChange={handleQualityChange}
          />
        </div>

        {/* PROPERTIES ==================== */}
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
              {quality.properties.length === 0 ? (
                <tr>
                  <td colSpan="5" style={{ color: '#999', fontStyle: 'italic' }}>
                    Chưa có thuộc tính. Bấm "Thêm thuộc tính".
                  </td>
                </tr>
              ) : (
                quality.properties.map((prop, index) => (
                  <tr key={index}>
                    <td>
                      <select
                        name="name"
                        value={prop.name}
                        onChange={(e) => handlePropertyChange(index, e)}
                      >
                        <option value="">Chọn thuộc tính</option>
                        {propertyNames.map((p) => (
                          <option key={p} value={p}>
                            {p}
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
                        placeholder="Mô tả"
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
                      <button
                        type="button"
                        className="delete-btn"
                        onClick={() => removePropertyRow(index)}
                      >
                        <i className="fas fa-trash" />
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>

          <button
            type="button"
            onClick={addPropertyRow}
            className="add-property-btn"
          >
            ➕ Thêm thuộc tính
          </button>
        </div>

        <button type="submit" className="create-quality-btn">
          Tạo
        </button>
      </form>

      {/* LIST ==================== */}
      <div className="quality-list">
        <h2>Danh sách tiêu chuẩn chất lượng đã tạo</h2>

        <div className="table-newquality-container">
          <table>
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã tiêu chuẩn</th>
                <th>Mã sản phẩm</th>
                <th>Bắt đầu</th>
                <th>Kết thúc</th>
                <th>Số thuộc tính</th>
                <th>Ghi chú</th>
                <th>Hành động</th>
              </tr>
            </thead>

            <tbody>
              {qualityList.length === 0 ? (
                <tr>
                  <td colSpan="8" style={{ textAlign: 'center' }}>
                    Chưa có tiêu chuẩn nào.
                  </td>
                </tr>
              ) : (
                qualityList.map((item, index) => (
                  <tr key={item.tempId}>
                    <td>{index + 1}</td>
                    <td>{item.code}</td>
                    <td>{item.productCode}</td>
                    <td>{item.startDate}</td>
                    <td>{item.endDate}</td>
                    <td>{item.properties.length}</td>
                    <td>{item.note}</td>
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
          disabled={qualityList.length === 0}
        >
          Xác nhận ({qualityList.length})
        </button>
      </div>
    </div>
  );
};

export default CreateNewQuality;
