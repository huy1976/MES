import React, { useEffect, useState } from 'react';
import { toast } from "react-toastify"; // Import toast for notifications
import "react-toastify/dist/ReactToastify.css";
import './CreateMachineCapacity.css';
import { getEquipmentCapabilityPropertyNames, getEquipmentCapabilityUnits,
  getEquipmentCapabilityPropertyUnits, createEquipmentCapability} from '../../../../api/EquipmentCapabilities';
import { getEquipmentIds } from '../../../../api/Equipments';

const capabilityTypeMap = {
     'Sẵn sàng' : 0,
     'Không thể sử dụng' : 1,
     'Đã cam kết' : 2,
};

const capabilityTypeList = [
    'Sẵn sàng',
    'Không thể sử dụng',
    'Đã cam kết',
]

const initialCapacityState = {
  code: '',
  equipmentId: '',
  type: '',
  quantity: {
     valueString: '',
     valueType: 1
  },
  unit: '',
  startTime: '',
  endTime: '',
  description: '',
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

// Helper function để định dạng datetime-local sang ISO format
const formatDateTime = (datetimeString) => {
  if (!datetimeString) return null;
  const date = new Date(datetimeString);
  return date.toISOString(); // "2025-10-22T07:54:00.000Z"
};

const CreateMachineCapacity = ({ savedState, onSaveState }) => {
  const [capacity, setCapacity] = useState(
    savedState?.capacity || initialCapacityState);
  const [capacityList, setCapacityList] = useState(
    savedState?.capacityList || []
  );

  const [equipmentIdList, setEquipmentIdList] = useState([]);
  const [equipmentCapabilityUnitList, setEquipmentCapabilityUnitList] = useState([]);
  const [equipmentCapabilityPropertyUnitList, setEquipmentCapabilityPropertyUnitList] = useState([]);

  // === STATE MỚI ĐỂ LƯU TÊN THUỘC TÍNH ===
  const [propertyNames, setPropertyNames] = useState([]);



useEffect(() => {
  const GetApi = async () => {
      try {
        const names = await getEquipmentCapabilityPropertyNames();
        const equipmentIds = await getEquipmentIds();
        const equipmentCapabilityUnits = await getEquipmentCapabilityUnits();
        const equipmentCapabilityPropertyUnits = await getEquipmentCapabilityPropertyUnits();

        const equipmentIdList = equipmentIds.flatMap(equipment => equipment.id);
        setEquipmentCapabilityUnitList(equipmentCapabilityUnits);
        setEquipmentIdList(equipmentIdList);
        setEquipmentCapabilityPropertyUnitList(equipmentCapabilityPropertyUnits);
        setPropertyNames(names || []);
      }catch (error) {
          console.error(error);
      }




  };
  GetApi();
},[])



   useEffect(() => {
      if (onSaveState) {
        onSaveState({
          capacity,
          capacityList,
        });
      }
    }, [capacity, capacityList, onSaveState]);


  const handleChange = (e) => {
    const { name, value } = e.target;
    setCapacity(prev => ({ ...prev, [name]: value }));
  };

  const handlePropertyChange = (index, e) => {
    const { name, value } = e.target;
    const updatedProperties = [...capacity.properties];
    updatedProperties[index][name] = value;
    setCapacity((prev) => ({ ...prev, properties: updatedProperties }));
  };

  const addPropertyRow = () => {
    setCapacity((prev) => ({
      ...prev,
      properties: [
        ...prev.properties,
        { name: '', description: '', valueString: '', valueUnitOfMeasure: '' },
      ],
    }));
  };

  const removePropertyRow = (index) => {
    const updatedProperties = capacity.properties.filter((_, i) => i !== index);
    setCapacity((prev) => ({ ...prev, properties: updatedProperties }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!capacity.code || !capacity.equipmentId || !capacity.type) {
        toast.info("Vui lòng điền đầy đủ Mã, Thiết bị và Loại năng lực!", {
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

    const filteredProperties = capacity.properties.filter(prop => {
      const isNameEmpty = !prop.name || prop.name.trim() === '';
      const isValueEmpty = !prop.valueString || prop.valueString.trim() === '';
      const isUnitEmpty = !prop.valueUnitOfMeasure || prop.valueUnitOfMeasure.trim() === '';
      return !(isNameEmpty && isValueEmpty && isUnitEmpty);
    });

    const newCapacity = {
      ...capacity,
      properties: filteredProperties,
      tempId: Date.now()
    };
    setCapacityList(prev => [...prev, newCapacity]);
    setCapacity(initialCapacityState);
  };

  const handleDelete = (tempId) => {
    setCapacityList(prev => prev.filter(item => item.tempId !== tempId));
  };

  const handleConfirm = async () => {
    if (capacityList.length === 0) {
        toast.info("Không có năng lực nào để xác nhận!", {
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

    for (const item of capacityList) {
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

      const quantityValue = {
        valueString: item.quantity || '',
        valueType: 1,
      } 

      const payload = {
        equipmentCapabilityId: item.code,
        equipmentId: item.equipmentId,
        capabilityType : capabilityTypeMap[item.type],
        hierarchyScope : '--',
        quantity: quantityValue,
        valueUnitOfMeasure: item.unit,
        validFrom: formatDateTime(item.startTime, true),
        validTo: formatDateTime(item.endTime, false),
        description: item.description || '',
        properties: dynamicProperties,
      };

      console.log('Gửi payload:', payload); 
              try {
                    await createEquipmentCapability(payload);
                    successes.push(item.code);
                    toast.success("Tạo năng lực thiết bị thành công!", {
                        position: "top-right",
                        autoClose: 3000,
                        hideProgressBar: false,
                        closeOnClick: true,
                        pauseOnHover: true,
                        draggable: true,
                        progress: undefined,
                      });
                  } catch (error) {
                    toast.error("Lỗi: Không thể tạo năng lực thiết bị!", {
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
    setCapacityList((prev) => prev.filter((item) => failures.includes(item.code)));
  };

  return (
    <div className="newcapacity-container">
      <form onSubmit={handleSubmit}>
        <div>
          <label>Mã năng lực</label>
          <input type="text" name="code" value={capacity.code} onChange={handleChange} required />
        </div>
        <div>
          <label>Mã Thiết bị</label>
          <select name="equipmentId" value={capacity.equipmentId} onChange={handleChange} required>
          <option value="" disabled selected >Chọn mã thiết bị</option>
            {equipmentIdList.map((equipmentId, index) => (
              <option key={index} value={equipmentId}>{equipmentId}</option>
            ))}
          </select>
        </div>
        <div>
          <label>Loại năng lực</label>
          <select name="type" value={capacity.type} onChange={handleChange} required>
            <option value="" disabled selected >Chọn loại</option>
            {capabilityTypeList.map((type, index) => (
              <option key={index} value={type}>{type}</option>
            ))}
          </select>
        </div>
        <div>
          <label>Số lượng</label>
          <input type="number" name="quantity" value={capacity.quantity} onChange={handleChange} min="0" />
        </div>
        <div>
          <label>Đơn vị</label>
          <select name="unit" value={capacity.unit} onChange={handleChange} required>
            <option value="" disabled selected >Chọn đơn vị</option>
            {equipmentCapabilityUnitList.map((unit, index) => (
              <option key={index} value={unit}>{unit}</option>
            ))}
          </select>
        </div>
        <div>
          <label>Thời gian bắt đầu</label>
          <input type="datetime-local" name="startTime" value={capacity.startTime} onChange={handleChange} />
        </div>
        <div>
          <label>Thời gian kết thúc</label>
          <input type="datetime-local" name="endTime" value={capacity.endTime} onChange={handleChange} />
        </div>
        <div>
          <label>Ghi chú</label>
          <input type="text" name="description" value={capacity.description} onChange={handleChange} />
        </div>

        <div className="properties-section">
          <h4>Chi tiết năng lực</h4>
          <table className="properties-table">
            <thead>
              <tr>
                <th>Tên</th>
                <th>Mô tả</th>
                <th>Giá trị</th>
                <th>Đơn vị</th>
                <th>Xóa</th>
              </tr>
            </thead>
            <tbody>
              {capacity.properties.length === 0 ? (
                <tr>
                  <td colSpan="5" style={{ textAlign: 'center', color: '#999', fontStyle: 'italic' }}>
                    Chưa có chi tiết. Bấm "Thêm chi tiết" để bắt đầu.
                  </td>
                </tr>
              ) : (
                capacity.properties.map((prop, index) => (
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
                        placeholder="Mô tả thuộc tính"
                      />
                    </td>
                    <td>
                      <input
                        type="text"
                        name="valueString"
                        value={prop.valueString}
                        onChange={(e) => handlePropertyChange(index, e)}
                        placeholder="VD: 100"
                      />
                    </td>
                   <td>
                      <select
                        name="valueUnitOfMeasure"
                        value={prop.valueUnitOfMeasure}
                        onChange={(e) => handlePropertyChange(index, e)}
                      >
                        <option value="" disabled selected >Chọn đơn vị</option>
                        {equipmentCapabilityPropertyUnitList.map((unit) => (
                          <option key={unit} value={unit}>
                            {unit}
                          </option>
                        ))}
                      </select>
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
            ➕ Thêm chi tiết
          </button>
        </div>

        <button type="submit" className="create-capacity-btn">Tạo</button>
      </form>

      <div className="capacity-list">
        <h2>Danh sách năng lực chờ xác nhận</h2>
        <div className="table-newcapacity-container">
          <table>
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã năng lực</th>
                <th>Mã Thiết bị</th>
                <th>Loại năng lực</th>
                <th>Số lượng</th>
                <th>Đơn vị</th>
                <th>Thời gian bắt đầu</th>
                <th>Thời gian kết thúc</th>
                <th>Ghi chú</th>
                <th>Hành động</th>
              </tr>
            </thead>
            <tbody>
              {capacityList.length === 0 ? (
                <tr>
                  <td colSpan="10" style={{ textAlign: 'center' }}>Chưa có năng lực nào.</td>
                </tr>
              ) : (
                capacityList.map((item, index) => (
                  <tr key={item.tempId}>
                    <td>{index + 1}</td>
                    <td>{item.code}</td>
                    <td>{item.equipmentId}</td>
                    <td>{item.type}</td>
                    <td>{item.quantity}</td>
                    <td>{item.unit}</td>
                    <td>{item.startTime}</td>
                    <td>{item.endTime}</td>
                    <td>{item.description}</td>
                    <td>
                      <button
                        type="button"
                        className="delete-btn"
                        onClick={() => handleDelete(item.tempId)}
                        aria-label={`Xóa năng lực ${item.code}`}
                      >
                        <i className="fas fa-trash"></i>
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
          disabled={capacityList.length === 0}
        >
          Xác nhận ({capacityList.length})
        </button>
      </div>
    </div>
  );
};

export default CreateMachineCapacity;