import React, { useState, useEffect } from 'react';
import { toast } from "react-toastify"; // Import toast for notifications
import "react-toastify/dist/ReactToastify.css";
import './CreateNewEquipment.css';
import { getProcessSegmentIds } from '../../../../api/ProcessSegment';
import { getEquipmentClassIds } from '../../../../api/EquipmentClasses';
import { getEquipmentSegmentSpecificationPropertyNames, createEquipmentSegmentSpecification } from '../../../../api/StageRequirement';

const initialEquipmentState = {
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

const CreateNewEquipment = ({ savedState, onSaveState }) => {
  const [equipment, setEquipment] = useState(
    savedState?.equipment || initialEquipmentState
  );
  const [equipmentList, setEquipmentList] = useState(
    savedState?.equipmentList || []
  );

  const [equipmentClassIdList, setEquipmentClassIdList] = useState([]);
  const [processSegmentIds, setProcessSegmentIds] = useState([]);

  const [propertyNames, setPropertyNames] = useState([]);

   // === EFFECT MỚI ĐỂ TẢI TÊN THUỘC TÍNH ===
          useEffect(() => {
            const GetApi = async () => {
              try {
                const segmentIds = await getProcessSegmentIds();
                const equipmentClassIds = await getEquipmentClassIds();
                const propertyNames = await getEquipmentSegmentSpecificationPropertyNames();

                setEquipmentClassIdList(equipmentClassIds);
                setProcessSegmentIds(segmentIds);
                setPropertyNames(propertyNames);
              } catch (error) {
                console.error(error);
                setProcessSegmentIds([]);
                setEquipmentClassIdList([]);
                setPropertyNames([]);
              }
            };
            GetApi();
          }, []); 

  useEffect(() => {
    if (onSaveState) {
      onSaveState({
        equipment,
        equipmentList,
      });
    }
  }, [equipment, equipmentList, onSaveState]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setEquipment((prev) => ({ ...prev, [name]: value }));
  };

  const handlePropertyChange = (index, e) => {
    const { name, value } = e.target;
    const updatedProperties = [...equipment.properties];
    updatedProperties[index][name] = value;
    setEquipment((prev) => ({ ...prev, properties: updatedProperties }));
  };

  const addPropertyRow = () => {
    setEquipment((prev) => ({
      ...prev,
      properties: [
        ...prev.properties,
        { attributeName: '', value: '', unit: '', description: '' },
      ],
    }));
  };

  const removePropertyRow = (index) => {
    const updatedProperties = equipment.properties.filter((_, i) => i !== index);
    setEquipment((prev) => ({ ...prev, properties: updatedProperties }));
  };

  const handleAddToList = (e) => {
    e.preventDefault();
    if (!equipment.code || !equipment.stageCode || !equipment.type) {
        toast.info("Vui lòng điền đầy đủ Mã yêu cầu, Mã công đoạn và Loại thiết bị!", {
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

    const filteredProperties = equipment.properties.filter((prop) => {
      const isNameEmpty = !prop.attributeName || prop.attributeName.trim() === '';
      const isValueEmpty = !prop.value || prop.value.trim() === '';
      const isUnitEmpty = !prop.unit || prop.unit.trim() === '';
      const isDescEmpty = !prop.description || prop.description.trim() === '';
      return !(isNameEmpty && isValueEmpty && isUnitEmpty && isDescEmpty);
    });

    const newEquipment = {
      ...equipment,
      properties: filteredProperties,
      tempId: Date.now(),
    };

    setEquipmentList((prevList) => [...prevList, newEquipment]);
    setEquipment(initialEquipmentState);
  };

  const handleDeleteFromList = (tempId) => {
    setEquipmentList((prevList) => prevList.filter((item) => item.tempId !== tempId));
  };

  const handleConfirm = async () => {
    if (equipmentList.length === 0) {
        toast.info("Không có thiết bị nào để xác nhận!", {
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

    const equipmentSegmentSpecifications = [];

    for (const item of equipmentList) {
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

      const equipmentSegmentSpecification = {
        equipmentSegmentSpecificationId: item.code,
        description: item.description,
        equipmentUse: 0,
        equipmentClassId: item.type,
        properties: dynamicProperties,
        quantity: quantityValue,
        valueUnitOfMeasure: item.unit || 'N/A',
      };

      equipmentSegmentSpecifications.push(equipmentSegmentSpecification);
    }

    const payload = {
      processSegmentId: equipmentList[0].stageCode,
      equipmentSegmentSpecifications: equipmentSegmentSpecifications,
    };

    console.log(payload);
    
    try {
      await createEquipmentSegmentSpecification(payload);
      toast.success("Tạo yêu cầu thiết bị thành công!", {
        position: "top-right",
        autoClose: 3000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
      });
      setEquipmentList([]);
    } catch (error) {
      toast.error("Lỗi: Không thể tạo yêu cầu thiết bị!", {
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
    <div className="newequipment-container">
      <form onSubmit={handleAddToList}>
        <div>
          <label>Mã yêu cầu</label>
          <input type="text" name="code" value={equipment.code} onChange={handleChange} required />
        </div>
        <div>
          <label>Mã công đoạn</label>
          <select name="stageCode" value={equipment.stageCode} onChange={handleChange} required>
                <option value="" disabled>Chọn Mã công đoạn</option>
                {processSegmentIds.map((id, item) => (
                  <option key={item} value={id}>{id}</option>
                ))}
         </select>
        </div>
        <div>
          <label>Loại thiết bị</label>
          <select name="type" value={equipment.type} onChange={handleChange} required>
            <option value="" disabled>Chọn loại thiết bị</option>
            {equipmentClassIdList.map((item) => (
              <option key={item.id} value={item.id}>{item.description}</option>
            ))}
          </select>
        </div>
        <div>
          <label>Mô tả</label>
          <input type="text" name="description" value={equipment.description} onChange={handleChange} />
        </div>
        <div>
          <label>Số lượng</label>
          <input type="text" name="quantity" value={equipment.quantity} onChange={handleChange} />
        </div>
        <div>
          <label>Đơn vị</label>
          <input type="text" name="unit" value={equipment.unit} onChange={handleChange} />
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
              {equipment.properties.length === 0 ? (
                <tr>
                  <td colSpan="5" style={{ textAlign: 'center', color: '#999', fontStyle: 'italic' }}>
                    Chưa có thuộc tính. Bấm "Thêm thuộc tính" để bắt đầu.
                  </td>
                </tr>
              ) : (
                equipment.properties.map((prop, index) => (
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

        <button type="submit" className="create-equipment-btn">
          Tạo
        </button>
      </form>

      <div className="equipment-list">
        <h2>Danh sách thiết bị đã tạo</h2>
        <div className="table-newequipment-container">
          <table>
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã yêu cầu</th>
                <th>Mã công đoạn</th>
                <th>Loại thiết bị</th>
                <th>Mô tả</th>
                <th>Số lượng</th>
                <th>Đơn vị</th>
                <th>Số thuộc tính</th>
                <th>Hành động</th>
              </tr>
            </thead>
            <tbody>
              {equipmentList.length === 0 ? (
                <tr>
                  <td colSpan="10" style={{ textAlign: 'center' }}>
                    Chưa có thiết bị nào.
                  </td>
                </tr>
              ) : (
                equipmentList.map((item, index) => (
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
          disabled={equipmentList.length === 0}
        >
          Xác nhận ({equipmentList.length})
        </button>
      </div>
    </div>
  );
};

export default CreateNewEquipment;