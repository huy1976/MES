import React, { useState, useEffect } from 'react';
import { toast } from "react-toastify"; // Import toast for notifications
import "react-toastify/dist/ReactToastify.css";
import './CreateNewHuman.css';
import { getPersonnelClassIds } from '../../../../api/PersonGroup';
import { getPersonPropertyNames, createPerson } from '../../../../api/Person';

const statusDisplayMap = {
  'Đang làm việc': '#149117',
  'Nghỉ việc': '#767676',
  'Tạm nghỉ': '#F03B28'
};

const initialHumanState = {
  name: '',
  code: '',
  type: '',
  shift: '',
  status: '',
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

  // Nếu bắt đầu bằng 0 và độ dài > 1, coi như chuỗi (điện thoại, mã số, v.v.)
  if (trimmedValue.startsWith('0') && trimmedValue.length > 1) {
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

  const [personnelClassIdList, setPersonnelClassIdList] = useState([]);

  const [propertyNames, setPropertyNames] = useState([]);


    // === FETCH DỮ LIỆU TỪ API ===
    useEffect(() => {
      const GetApi = async () => {
        try {
          const names = await getPersonPropertyNames();
          const personnelClassIds = await getPersonnelClassIds();

          setPropertyNames(names || []);
          setPersonnelClassIdList(personnelClassIds || []);
        } catch (error) {
          console.error(error);
          setPropertyNames([]);
          setPersonnelClassIdList([]);
        }
      };
      GetApi();
    }, []);
  // === EFFECT: SAVE STATE KHI human HOẶC humanList THAY ĐỔI ===
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
    setHuman(prev => ({ ...prev, [name]: value }));
  };

  // === XỬ LÝ BẢNG THUỘC TÍNH ĐỘNG ===
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
        { name: '', description: '', valueString: '', valueUnitOfMeasure: '' },
      ],
    }));
  };

  const removePropertyRow = (index) => {
    const updatedProperties = human.properties.filter((_, i) => i !== index);
    setHuman((prev) => ({ ...prev, properties: updatedProperties }));
  };

  const handleAddToList = (e) => {
    e.preventDefault();
    if (!human.name || !human.code || !human.type) {
      toast.info("Vui lòng điền đầy đủ Họ tên, Mã nhân viên và Lớp nhân sự!", {
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

    const filteredProperties = human.properties.filter(prop => {
      const isNameEmpty = !prop.name || prop.name.trim() === '';
      const isDescEmpty = !prop.description || prop.description.trim() === '';
      const isValueEmpty = !prop.valueString || prop.valueString.trim() === '';
      const isUnitEmpty = !prop.valueUnitOfMeasure || prop.valueUnitOfMeasure.trim() === '';
      return !(isNameEmpty && isDescEmpty && isValueEmpty && isUnitEmpty);
    });

    const newHuman = { 
      ...human, 
      properties: filteredProperties,
      tempId: Date.now() 
    };
    setHumanList(prev => [...prev, newHuman]);
    // === RESET FORM SAU KHI THÊM VÀO DANH SÁCH ===
    setHuman(initialHumanState);
  };

  const handleDelete = (tempId) => {
    setHumanList(prev => prev.filter(item => item.tempId !== tempId));
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

    const successes = [];
    const failures = [];

    for (const item of humanList) {
      try {
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

        // === GỌI API TẠO NHÂN SỰ TẠI ĐÂY ===
        const payload = {
          name: item.name,
          personId: item.code,
          personClass: item.type,
          properties: dynamicProperties,
          description: '--'
        };
        console.log('Gửi payload:', payload);
        
        createPerson(payload);
        successes.push(item.code);
        toast.success("Tạo nhân sự thành công!", {
        position: "top-right",
        autoClose: 3000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
      });
      } catch (error) {
        toast.error("Lỗi: Không thể tạo nhân sự!", {
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
    setHumanList((prev) => prev.filter((item) => failures.includes(item.code)));
  };

  return (
    <div className="newhuman-container">
      <form onSubmit={handleAddToList}>
        <div>
          <label>Họ và tên</label>
          <input type="text" name="name" value={human.name} onChange={handleChange} required />
        </div>
        <div>
          <label>Mã nhân viên</label>
          <input type="text" name="code" value={human.code} onChange={handleChange} required />
        </div>
        <div>
          <label>Lớp nhân sự</label>
          <select name="type" value={human.type} onChange={handleChange} required>
           <option value="" disabled>Chọn lớp nhân sự</option>
           {personnelClassIdList.map((item) => (
              <option key={item.id} value={item.id}>{item.name}</option>
            ))}
          </select>
        </div>
        <div>
          <label>Ca làm việc</label>
          <select name="shift" value={human.shift} onChange={handleChange}>
            <option value="">Chọn ca làm việc</option>
            <option value="Ca 1">Ca 1</option>
            <option value="Ca 2">Ca 2</option>
            <option value="Ca 3">Ca 3</option>
            <option value="Ca ngày">Ca ngày</option>
          </select>
        </div>
        <div>
          <label>Trạng thái</label>
          <select name="status" value={human.status} onChange={handleChange}>
            <option value="">Chọn trạng thái</option>
            <option value="Đang làm việc">Đang làm việc</option>
            <option value="Nghỉ việc">Nghỉ việc</option>
            <option value="Tạm nghỉ">Tạm nghỉ</option>
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
                        <option value="" disabled selected>Chọn thuộc tính</option>
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

        <button type="submit" className="create-human-btn">Tạo</button>
      </form>

      <div className="human-list">
        <h2>Danh sách nhân sự đã tạo</h2>
        <div className="table-newhuman-container">
          <table>
            <thead>
              <tr>
                <th>STT</th>
                <th>Họ và tên</th>
                <th>Mã nhân viên</th>
                <th>Lớp nhân sự</th>
                <th>Ca làm việc</th>
                <th>Trạng thái</th>
                <th>Xóa</th>
              </tr>
            </thead>
            <tbody>
              {humanList.length === 0 ? (
                <tr>
                  <td colSpan="8" style={{ textAlign: 'center' }}>Chưa có nhân sự nào.</td>
                </tr>
              ) : (
                humanList.map((item, index) => (
                  <tr key={item.tempId}>
                    <td>{index + 1}</td>
                    <td>{item.name}</td>
                    <td>{item.code}</td>
                    <td>{item.type}</td>
                    <td>{item.shift}</td>
                    <td>
                      <span
                        className="status-btn"
                        style={{
                          backgroundColor: statusDisplayMap[item.status] || '#767676',
                        }}
                      >
                        {item.status}
                      </span>
                    </td>
                    <td>
                      <button
                        type="button"
                        className="delete-btn"
                        onClick={() => handleDelete(item.tempId)}
                        aria-label={`Xóa nhân sự ${item.code}`}
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
          disabled={humanList.length === 0}
        >
          Xác nhận ({humanList.length})
        </button>
      </div>
    </div>
  );
};

export default CreateNewHuman;