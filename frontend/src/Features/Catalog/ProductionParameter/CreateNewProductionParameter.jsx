import React, { useState, useEffect } from 'react';
import { toast } from "react-toastify"; // Import toast for notifications
import "react-toastify/dist/ReactToastify.css";
import './CreateNewProductionParameter.css';
import {
  getProductionParameterPropertyNames,
  createProductionParameter,
} from '../../../api/ProductionParameter';

// Initial state
const initialParameterState = {
  code: '',
  startDate: '',
  endDate: '',
  parameterCount: '',
  note: '',
  properties: [],
};

// Helper: determine value type
const getValueType = (value) => {
  if (typeof value !== 'string') return 3;
  const trimmed = value.trim();
  if (trimmed === '') return 3;
  if (trimmed.toLowerCase() === 'true' || trimmed.toLowerCase() === 'false') return 0;
  if (isNaN(Number(trimmed))) return 3;
  if (trimmed.includes('.')) return 2;
  return 1;
};

const CreateNewProductionParameter = () => {
  const [parameter, setParameter] = useState(initialParameterState);
  const [parameterList, setParameterList] = useState([]);
  const [propertyNames, setPropertyNames] = useState([]);

  // Load property names on mount
  useEffect(() => {
    const fetchPropertyNames = async () => {
      try {
        const names = await getProductionParameterPropertyNames();
        setPropertyNames(names || []);
      } catch (error) {
        console.error('Không thể tải danh sách tên thuộc tính:', error);
        setPropertyNames([]);
      }
    };

    fetchPropertyNames();
  }, []);

  // Handle main form fields
  const handleParameterChange = (e) => {
    const { name, value } = e.target;
    setParameter((prev) => ({ ...prev, [name]: value }));
  };

  // Handle property row input
  const handlePropertyChange = (index, e) => {
    const { name, value } = e.target;
    const updated = [...parameter.properties];
    updated[index][name] = value;
    setParameter((prev) => ({ ...prev, properties: updated }));
  };

  // Add property row
  const addPropertyRow = () => {
    setParameter((prev) => ({
      ...prev,
      properties: [
        ...prev.properties,
        { name: '', description: '', valueString: '', valueUnitOfMeasure: '' },
      ],
    }));
  };

  // Remove property row
  const removePropertyRow = (index) => {
    const updated = parameter.properties.filter((_, i) => i !== index);
    setParameter((prev) => ({ ...prev, properties: updated }));
  };

  // Add parameter to list
  const handleAddToList = (e) => {
    e.preventDefault();

    if (!parameter.code) {
      toast.info("Vui lòng điền đầy đủ Mã thông số sản xuất!", {
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

    // Remove empty properties
    const filteredProperties = parameter.properties.filter((p) => {
      const emptyName = !p.name || p.name.trim() === '';
      const emptyValue = !p.valueString || p.valueString.trim() === '';
      return !(emptyName && emptyValue);
    });

    const newParameter = {
      ...parameter,
      properties: filteredProperties,
      tempId: Date.now(),
    };

    setParameterList((prev) => [...prev, newParameter]);
    setParameter(initialParameterState);
  };

  // Remove parameter from right-side table
  const handleDeleteFromList = (tempId) => {
    setParameterList((prev) => prev.filter((item) => item.tempId !== tempId));
  };

  // Send all parameters to API
  const handleConfirm = async () => {
    if (parameterList.length === 0) {
        toast.info("Không có thông số nào để xác nhận!", {
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

    for (const item of parameterList) {
      const dynamicProperties = item.properties.map((p) => {
        const value = p.valueString || '';
        return {
          name: p.name || 'N/A',
          description: p.description || 'N/A',
          valueString: value,
          valueType: getValueType(value),
          valueUnitOfMeasure: p.valueUnitOfMeasure || 'N/A',
        };
      });

      const validFrom = item.startDate ? `${item.startDate}T00:00:00.000Z` : null;
      const validTo = item.endDate ? `${item.endDate}T23:59:59.999Z` : null;

      const payload = {
        productionParameterId: item.code,
        description: item.note,
        validFrom,
        validTo,
        properties: dynamicProperties,
      };

      try {
        await createProductionParameter(payload);
        successes.push(item.code);
        toast.success("Tạo thông số thành công!", {
        position: "top-right",
        autoClose: 3000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
      });
      } catch (error) {
        toast.error("Lỗi: Không thể tạo thông số!", {
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

    // Keep only failed items
    setParameterList((prev) => prev.filter((item) => failures.includes(item.code)));
  };

  return (
    <div className="newparameter-container">
      {/* Left Column – Form */}
      <form onSubmit={handleAddToList}>
        <div>
          <label>Mã thông số sản xuất</label>
          <input
            type="text"
            name="code"
            value={parameter.code}
            onChange={handleParameterChange}
            required
          />
        </div>

        <div>
          <label>Thời gian áp dụng</label>
          <input
            type="date"
            name="startDate"
            value={parameter.startDate}
            onChange={handleParameterChange}
          />
        </div>

        <div>
          <label>Thời gian kết thúc</label>
          <input
            type="date"
            name="endDate"
            value={parameter.endDate}
            onChange={handleParameterChange}
          />
        </div>

        <div>
          <label>Ghi chú</label>
          <input
            type="text"
            name="note"
            value={parameter.note}
            onChange={handleParameterChange}
          />
        </div>

        {/* Dynamic Properties */}
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
              {parameter.properties.length === 0 ? (
                <tr>
                  <td colSpan="5" style={{ textAlign: 'center', color: '#999', fontStyle: 'italic' }}>
                    Chưa có thuộc tính. Bấm "Thêm thuộc tính" để bắt đầu.
                  </td>
                </tr>
              ) : (
                parameter.properties.map((prop, index) => (
                  <tr key={index}>
                    <td>
                      <select
                        name="name"
                        value={prop.name}
                        onChange={(e) => handlePropertyChange(index, e)}
                      >
                        <option value="">Chọn TT</option>
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
                      <button
                        type="button"
                        className="delete-btn"
                        onClick={() => removePropertyRow(index)}
                      >
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

        <button type="submit" className="create-parameter-btn">
          Tạo
        </button>
      </form>

      {/* Right Column – List */}
      <div className="parameter-list">
        <h2>Danh sách thông số sản xuất đã tạo</h2>
        <div className="table-newparameter-container">
          <table>
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã thông số</th>
                <th>Thời gian áp dụng</th>
                <th>Thời gian kết thúc</th>
                <th>Số thuộc tính</th>
                <th>Ghi chú</th>
                <th>Hành động</th>
              </tr>
            </thead>
            <tbody>
              {parameterList.length === 0 ? (
                <tr>
                  <td colSpan="7" style={{ textAlign: 'center' }}>
                    Chưa có thông số nào.
                  </td>
                </tr>
              ) : (
                parameterList.map((item, index) => (
                  <tr key={item.tempId}>
                    <td>{index + 1}</td>
                    <td>{item.code}</td>
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
          disabled={parameterList.length === 0}
        >
          Xác nhận ({parameterList.length})
        </button>
      </div>
    </div>
  );
};

export default CreateNewProductionParameter;
