import React, { useState, useEffect } from 'react';
import { toast } from "react-toastify"; // Import toast for notifications
import "react-toastify/dist/ReactToastify.css";
import './CreateNewEquipments.css';
import { createEquipment, getEquipmentPropertyNames} from '../../../../api/Equipments';
import { getEnterprises } from '../../../../api/Enterprise';
import { getEquipmentClassIds } from '../../../../api/EquipmentClasses';

const statusDisplayMap = {
         'Hoạt động' : 0,
         'Chờ xử lý' : 1,
         'Chờ lập kế hoạch' : 2,
         'Đã lên kế hoạch' : 3,
         'Chờ sản xuất' : 4,
         'Đang thực thi' : 5,    
         'Đã hoàn thành' : 6,
         'Đã hủy' : 7,
         'Sẵn sàng' : 8
};

const status = [
        'Hoạt động',
        'Chờ xử lý',
        'Chờ lập kế hoạch',
        'Đã lên kế hoạch',
        'Chờ sản xuất',
        'Đang thực thi',    
        'Đã hoàn thành',
        'Đã hủy',
        'Sẵn sàng'  
]

// === STATE BAN ĐẦU ===
const initialEquipmentState = {
  type: '',
  code: '',
  name: '',
  status: '',
  absolutePath: '',
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



const CreateNewEquipments = ({ savedState, onSaveState }) => {
  // === KHÔI PHỤC STATE TỪ savedState NẾU CÓ ===
  const [equipment, setEquipment] = useState(
    savedState?.equipment || initialEquipmentState
  );
  const [equipmentList, setEquipmentList] = useState(
    savedState?.equipmentList || []
  );

  const [equipmentClassIdList, setEquipmentClassIdList] = useState([]);

  const [absolutePathList, setAbsolutePathList] = useState(
    savedState?.absolutePathList || []
  );
  
  // === STATE MỚI ĐỂ LƯU TÊN THUỘC TÍNH ===
  const [propertyNames, setPropertyNames] = useState([]);

  const [model, setModel] = useState('');
  
  // === EFFECT MỚI ĐỂ TẢI TÊN THUỘC TÍNH ===
    useEffect(() => {
      const GetApi = async () => {
        try {
          const names = await getEquipmentPropertyNames();
          const enterprises = await getEnterprises();
          const equipmentClassIds = await getEquipmentClassIds();

      
          const absolutePathList = enterprises.items
            .flatMap(ent => ent.sites)
            .flatMap(site => site.areas)
            .flatMap(area => area.workCenters)
            .map(workcenter => workcenter.absolutePath);

          setAbsolutePathList(absolutePathList);
          setPropertyNames(names || []);
          setEquipmentClassIdList(equipmentClassIds || []);
        } catch (error) {
          console.error(error);
          setPropertyNames([]);
          setAbsolutePathList([]);
          setEquipmentClassIdList([]);
        }
      };
      GetApi();
    }, []);

  // === EFFECT: SAVE STATE KHI equipment HOẶC equipmentList THAY ĐỔI ===
  useEffect(() => {
    if (onSaveState) {
      onSaveState({
        equipment,
        equipmentList,
      });
    }
  }, [equipment, equipmentList, onSaveState]);

  // === XỬ LÝ FORM ===
  const handleChange = (e) => {
    const { name, value } = e.target;
    setEquipment(prev => ({ ...prev, [name]: value }));
  };

   // === XỬ LÝ BẢNG THUỘC TÍNH ĐỘNG ===
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
        { name: '', description: '', valueString: '', valueUnitOfMeasure: '' },
      ],
    }));
  };

  const removePropertyRow = (index) => {
    const updatedProperties = equipment.properties.filter((_, i) => i !== index);
    setEquipment((prev) => ({ ...prev, properties: updatedProperties }));
  };

  const handleAddToList = (e) => {
    e.preventDefault();
    if (!equipment.type || !equipment.code || !equipment.name) {
      toast.info("Vui lòng điền đầy đủ Loại, Mã và Tên thiết bị!", {
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

   const filteredProperties = equipment.properties.filter(prop => {
      const isNameEmpty = !prop.name || prop.name.trim() === '';
      const isDescEmpty = !prop.description || prop.description.trim() === '';
      const isValueEmpty = !prop.valueString || prop.valueString.trim() === '';
      const isUnitEmpty = !prop.valueUnitOfMeasure || prop.valueUnitOfMeasure.trim() === '';
      return !(isNameEmpty && isDescEmpty && isValueEmpty && isUnitEmpty);
    });
    
    const model = filteredProperties.find(prop => prop.name === 'model')?.valueString || '';
    
    setModel(model);

    const newEquipment = { 
      ...equipment, 
      properties: filteredProperties,
      tempId: Date.now() 
    };
    setEquipmentList(prev => [...prev, newEquipment]);
    // === RESET FORM SAU KHI THÊM VÀO DANH SÁCH ===
    setEquipment(initialEquipmentState);
  };

  const handleDeleteFromList = (tempId) => {
    setEquipmentList(prev => prev.filter(item => item.tempId !== tempId));
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

    const successes = [];
    const failures = [];

    for (const item of equipmentList) {
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
        equipmentId: item.code,
        name: item.name,
        equipmentStatus: statusDisplayMap[item.status],
        equipmentClass: item.type,
        properties: dynamicProperties,
        absolutePath: item.absolutePath || 'N/A',
      };
      console.log('Gửi payload:', payload); 
        try {
              await createEquipment(payload);
              successes.push(item.code);
              toast.success("Tạo thiết bị thành công!", {
                position: "top-right",
                autoClose: 3000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
              });
            } catch (error) {
              toast.error("Lỗi: Không thể tạo thiết bị!", {
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
    setEquipmentList((prev) => prev.filter((item) => failures.includes(item.code)));
  };

  return (
    <div className="newequipment-container">
      <form onSubmit={handleAddToList}>
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
          <label>Mã thiết bị</label>
          <input type="text" name="code" value={equipment.code} onChange={handleChange} required />
        </div>
        <div>
          <label>Tên thiết bị</label>
          <input type="text" name="name" value={equipment.name} onChange={handleChange} required />
        </div>
        <div>
          <label>Trạng thái</label>
          <select name="status" value={equipment.status} onChange={handleChange}>
          <option value="" disabled selected >Chọn trạng thái</option>
            {status.map((status, index) => (
              <option key={index} value={status}>{status}</option>
            ))}
          </select>
        </div>
        <div>
          <label>WorkCenter</label>
           <select name="absolutePath" value={equipment.absolutePath} onChange={handleChange}>
            <option value="" disabled selected >Chọn WorkCenter</option>
            {absolutePathList.map((path, index) => (
              <option key={index} value={path}>{path}</option>
            ))}
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


        <button type="submit" className="create-equipment-btn">Tạo</button>
      </form>

      <div className="equipment-list">
        <h2>Danh sách thiết bị chờ xác nhận</h2>
        <div className="table-newequipment-container">
          <table>
            <thead>
              <tr>
                <th>STT</th>
                <th>Loại thiết bị</th>
                <th>Mã thiết bị</th>
                <th>Tên thiết bị</th>
                <th>Model</th>
                <th>Trạng thái</th>
                <th>WorkCenter</th>
                <th>Số thuộc tính</th>
                <th>Số năng lực</th>
                <th>Hành động</th>
              </tr>
            </thead>
            <tbody>
              {equipmentList.length === 0 ? (
                <tr>
                  <td colSpan="12" style={{ textAlign: 'center' }}>Chưa có thiết bị nào.</td>
                </tr>
              ) : (
                equipmentList.map((item, index) => (
                  <tr key={item.tempId}>
                    <td>{index + 1}</td>
                    <td>{item.type}</td>
                    <td>{item.code}</td>
                    <td>{item.name}</td>
                    <td>{model}</td>
                    <td>
                      <span
                        className="status-btn"
                        style={{
                          backgroundColor: statusDisplayMap[item.status]?.color || '#767676',
                        }}
                      >
                        {statusDisplayMap[item.status]?.label || item.status}
                      </span>
                    </td>
                    <td>{item.absolutePath}</td>
                    <td>{item.properties.length}</td>
                    <td>{item.capacities || 0}</td>
                    <td>
                      <button
                        type="button" 
                        className="delete-btn"
                        onClick={() => handleDeleteFromList(item.tempId)}
                        aria-label={`Xóa thiết bị ${item.code}`}
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
          disabled={equipmentList.length === 0}
        >
          Xác nhận ({equipmentList.length})
        </button>
      </div>
    </div>
  );
};

export default CreateNewEquipments;