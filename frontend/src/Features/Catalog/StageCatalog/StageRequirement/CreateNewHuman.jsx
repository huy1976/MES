import React, { useState, useEffect } from 'react';
import { toast } from "react-toastify"; // Import toast for notifications
import "react-toastify/dist/ReactToastify.css";
import './CreateNewHuman.css';
import { getProcessSegmentIds } from '../../../../api/ProcessSegment';
import { getPersonnelSegmentSpecificationPropertyNames, createPersonnelSegmentSpecification } from '../../../../api/StageRequirement';
import { getPersonnelClassIds } from '../../../../api/PersonGroup';
const initialHumanState = {
  code: '',
  stageCode: '',
  type: '',
  description: '',
  quantity: '',
  unit: '',
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

const CreateNewHuman = ({ savedState, onSaveState }) => {
  const [human, setHuman] = useState(
    savedState?.human || initialHumanState
  );
  const [humanList, setHumanList] = useState(
    savedState?.humanList || []
  );

  const [processSegmentIds, setProcessSegmentIds] = useState([]);
  const [personnelClassIds, setPersonnelClassIds] = useState([]);

  const [propertyNames, setPropertyNames] = useState([]);

  // === FETCH DATA FROM API ===
  useEffect(() => {
    const GetApi = async () => {
      try {
        const segmentIds = await getProcessSegmentIds();
        const personnelClassIds = await getPersonnelClassIds();
        const propertyNames = await getPersonnelSegmentSpecificationPropertyNames();

        setPersonnelClassIds(personnelClassIds);
        setProcessSegmentIds(segmentIds);
        setPropertyNames(propertyNames);
      } catch (error) {
        console.error(error);
        setProcessSegmentIds([]);
        setPersonnelClassIds([]);
        setPropertyNames([]);
      }
    };
    GetApi();
  }, []);

  useEffect(() => {
    if (onSaveState) {
      onSaveState({
        human,
        humanList,
      });
    }
  }, [human, humanList, onSaveState]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setHuman((prev) => ({ ...prev, [name]: value }));
  };

  const handlePropertyChange = (index, e) => {
    const { name, value } = e.target;
    const updatedProperties = [...human.properties];
    updatedProperties[index][name] = value;
    setHuman((prev) => ({ ...prev, properties: updatedProperties }));
  };

  const addPropertyRow = () => {
    setHuman((prev) => ({
      ...prev,
      properties: [
        ...prev.properties,
        { criteriaName: '', value: '', unit: '', description: '' },
      ],
    }));
  };

  const removePropertyRow = (index) => {
    const updatedProperties = human.properties.filter((_, i) => i !== index);
    setHuman((prev) => ({ ...prev, properties: updatedProperties }));
  };

  const handleAddToList = (e) => {
    e.preventDefault();
    if (!human.code || !human.stageCode || !human.type) {
       toast.info("Vui lòng điền đầy đủ Mã yêu cầu, Mã công đoạn và Vai trò nhân sự!", {
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

    const filteredProperties = human.properties.filter((prop) => {
      const isNameEmpty = !prop.name || prop.name.trim() === '';
      const isValueEmpty = !prop.value || prop.value.trim() === '';
      const isUnitEmpty = !prop.unit || prop.unit.trim() === '';
      const isDescEmpty = !prop.description || prop.description.trim() === '';
      return !(isNameEmpty && isValueEmpty && isUnitEmpty && isDescEmpty);
    });

    const newHuman = {
      ...human,
      properties: filteredProperties,
      tempId: Date.now(),
    };

    setHumanList((prevList) => [...prevList, newHuman]);
    setHuman(initialHumanState);
  };

  const handleDeleteFromList = (tempId) => {
    setHumanList((prevList) => prevList.filter((item) => item.tempId !== tempId));
  };

  const handleConfirm = async () => {
    if (humanList.length === 0) {
        toast.info("Không có nhân sự nào để xác nhận!", {
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

    const personnelSegmentSpecifications = [];

    for (const item of humanList) {
      const dynamicProperties = item.properties.map((p) => {
        const valueStr = p.value || '';
        return {
          name: p.name || 'N/A',
          description: p.description || 'N/A',
          valueString: valueStr,
          valueType: getValueType(valueStr),
          valueUnitOfMeasure: p.unit || 'N/A',
        };
      });

      const quantityValue = {
        valueString: item.quantity || '',
        valueType: 1,
      };

      const personnelSegmentSpecification = {
        personnelSegmentSpecificationId: item.code,
        description: item.description,
        personnelUse: 0,
        personClassId: item.type,
        properties: dynamicProperties,
        quantity: quantityValue,
        valueUnitOfMeasure: item.unit || 'N/A',
      };

      personnelSegmentSpecifications.push(personnelSegmentSpecification);
    }

    const payload = {
      processSegmentId: humanList[0].stageCode,
      personnelSegmentSpecifications: personnelSegmentSpecifications,
    };

    try {
      await createPersonnelSegmentSpecification(payload);
       toast.success("Tạo yêu cầu nhân sự thành công!", {
              position: "top-right",
              autoClose: 3000,
              hideProgressBar: false,
              closeOnClick: true,
              pauseOnHover: true,
              draggable: true,
              progress: undefined,
        });
      setHumanList([]);
    } catch (error) {
      toast.error("Lỗi: Không thể tạo yêu cầu nhân sự!", {
              position: "top-right",
              autoClose: 3000,
              hideProgressBar: false,
              closeOnClick: true,
              pauseOnHover: true,
              draggable: true,
              progress: undefined,
            });
    }
  };

  return (
    <div className="newhuman-container">
      <form onSubmit={handleAddToList}>
        <div>
          <label>Mã yêu cầu</label>
          <input type="text" name="code" value={human.code} onChange={handleChange} required />
        </div>
        <div>
          <label>Mã công đoạn</label>
          <select name="stageCode" value={human.stageCode} onChange={handleChange} required>
            <option value="" disabled>Chọn Mã công đoạn</option>
            {processSegmentIds.map((id, index) => (
              <option key={index} value={id}>{id}</option>
            ))}
          </select>
        </div>
        <div>
          <label>Loại nhân sự</label>
          <select name="type" value={human.type} onChange={handleChange} required>
            <option value="" disabled>Chọn loại nhân sự</option>
            {personnelClassIds.map((item) => (
              <option key={item.id} value={item.id}>{item.name}</option>
            ))}
          </select>
        </div>
        <div>
          <label>Mô tả</label>
          <input type="text" name="description" value={human.description} onChange={handleChange} />
        </div>
        <div>
          <label>Số lượng</label>
          <input type="text" name="quantity" value={human.quantity} onChange={handleChange} />
        </div>
        <div>
          <label>Đơn vị</label>
          <input type="text" name="unit" value={human.unit} onChange={handleChange} />
        </div>

        <div className="properties-section">
          <h4>Chi tiết thuộc tính</h4>
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
              {human.properties.length === 0 ? (
                <tr>
                  <td colSpan="5" style={{ textAlign: 'center', color: '#999', fontStyle: 'italic' }}>
                    Chưa có thuộc tính. Bấm "Thêm thuộc tính" để bắt đầu.
                  </td>
                </tr>
              ) : (
                human.properties.map((prop, index) => (
                  <tr key={index}>
                    <td>
                     <select
                        name="name"
                        value={prop.name}
                        onChange={(e) => handlePropertyChange(index, e)}
                      >
                        <option value="" disabled selected >Chọn thuộc tính</option>
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
                        placeholder="Mô tả"
                      />
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

        <button type="submit" className="create-human-btn">
          Tạo
        </button>
      </form>

      <div className="human-list">
        <h2>Danh sách nhân sự đã tạo</h2>
        <div className="table-newhuman-container">
          <table>
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã yêu cầu</th>
                <th>Mã công đoạn</th>
                <th>Loại nhân sự</th>
                <th>Mô tả</th>
                <th>Số lượng</th>
                <th>Đơn vị</th>
                <th>Số thuộc tính</th>
                <th>Hành động</th>
              </tr>
            </thead>
            <tbody>
              {humanList.length === 0 ? (
                <tr>
                  <td colSpan="10" style={{ textAlign: 'center' }}>
                    Chưa có nhân sự nào.
                  </td>
                </tr>
              ) : (
                humanList.map((item, index) => (
                  <tr key={item.tempId}>
                    <td>{index + 1}</td>
                    <td>{item.code}</td>
                    <td>{item.stageCode}</td>
                    <td>{item.type}</td>
                    <td>{item.description}</td>
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
        <button
          type="button"
          className="confirm-btn"
          onClick={handleConfirm}
          disabled={humanList.length === 0}
        >
          Xác nhận ({humanList.length})
        </button>
      </div>
    </div>
  );
};

export default CreateNewHuman;