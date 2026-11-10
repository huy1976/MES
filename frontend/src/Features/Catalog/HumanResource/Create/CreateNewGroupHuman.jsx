import React, { useEffect, useState } from 'react';
import { toast } from "react-toastify"; // Import toast for notifications
import "react-toastify/dist/ReactToastify.css";
import './CreateNewGroupHuman.css';
import { createPersonnelClass, getPersonnelClassPropertyNames } from '../../../../api/PersonGroup';

const initialGroupState = {
  code: '',
  name: '',
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


const CreateNewGroupHuman = ({ savedState, onSaveState }) => {
  const [group, setGroup] = useState(
    savedState?.group || initialGroupState
  );
  const [groupList, setGroupList] = useState(
    savedState?.groupList || []
  );
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [propertyNames, setPropertyNames] = useState([]);

  // === FETCH DỮ LIỆU TỪ API ===
  useEffect(() => {
    const GetApi = async () => {
      try {
        const names = await getPersonnelClassPropertyNames();
        setPropertyNames(names || []);
      } catch (error) {
        console.error(error);
      }
    };
    GetApi();
  }, []);

  // === LƯU STATE QUA PROPS ===
  useEffect(() => {
    if (onSaveState) {
      onSaveState({
        group,
        groupList,
      });
    }
  }, [group, groupList, onSaveState]);

  // === HÀM XỬ LÝ FORM CHÍNH ===
  const handleChange = (e) => {
    const { name, value } = e.target;
    setGroup(prev => ({ ...prev, [name]: value }));
  };

  // === HÀM XỬ LÝ "TABLE NHỎ" (THUỘC TÍNH) ===
  const handlePropertyChange = (index, e) => {
    const { name, value } = e.target;
    const updatedProperties = [...group.properties];
    updatedProperties[index][name] = value;
    setGroup(prev => ({ ...prev, properties: updatedProperties }));
  };

  const addPropertyRow = () => {
    setGroup(prev => ({
      ...prev,
      properties: [
        ...prev.properties,
        { name: '', description: '', valueString: '', valueUnitOfMeasure: '' }
      ]
    }));
  };

  const removePropertyRow = (index) => {
    const updatedProperties = group.properties.filter((_, i) => i !== index);
    setGroup(prev => ({ ...prev, properties: updatedProperties }));
  };

  // === HÀM XỬ LÝ NÚT CHÍNH ===
  const handleSubmit = (e) => {
    e.preventDefault();
    if (!group.code || !group.name) {
      toast.info("Vui lòng điền đầy đủ Mã và Tên nhóm nhân sự!", {
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

    const filteredProperties = group.properties.filter(prop => {
      const isNameEmpty = !prop.name || prop.name.trim() === '';
      const isValueEmpty = !prop.valueString || prop.valueString.trim() === '';
      const isUnitEmpty = !prop.valueUnitOfMeasure || prop.valueUnitOfMeasure.trim() === '';
      return !(isNameEmpty && isValueEmpty && isUnitEmpty);
    });

    const newGroup = {
      ...group,
      properties: filteredProperties,
      tempId: Date.now()
    };
    setGroupList(prev => [...prev, newGroup]);
    setGroup(initialGroupState);
  };

  const handleDelete = (tempId) => {
    setGroupList(prev => prev.filter(item => item.tempId !== tempId));
  };

  const handleConfirm = async () => {
    if (groupList.length === 0) {
        toast.info("Không có nhóm nhân sự nào để xác nhận!", {
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
    setIsSubmitting(true);

    const successes = [];
    const failures = [];

    for (const item of groupList) {
      const dynamicProperties = item.properties.map(p => ({
        name: p.name || 'N/A',
        description: p.description || 'N/A',
        valueString: p.valueString || 'N/A',
        valueType: getValueType(p.valueString),
        valueUnitOfMeasure: p.valueUnitOfMeasure || 'N/A'
      }));

      const payload = {
        personnelClassId: item.code,
        name: item.name,
        description: item.description,
        properties: dynamicProperties || []
      };

      try {
        console.log("Đang gửi payload:", payload);
        await createPersonnelClass(payload);
        successes.push(item.code);
        toast.success("Tạo nhóm nhân sự thành công!", {
        position: "top-right",
        autoClose: 3000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
      });
      } catch (error) {
        toast.error("Lỗi: Không thể tạo nhóm nhân sự!", {
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

    setGroupList((prev) => prev.filter((item) => failures.includes(item.code)));
    setIsSubmitting(false);
  };

  return (
    <div className="newgrouphuman-container">
      <form onSubmit={handleSubmit}>
        <div>
          <label>Mã nhóm nhân sự</label>
          <input type="text" name="code" value={group.code} onChange={handleChange} required />
        </div>
        <div>
          <label>Tên nhóm nhân sự</label>
          <input type="text" name="name" value={group.name} onChange={handleChange} required />
        </div>
        <div>
          <label>Mô tả</label>
          <input type="text" name="description" value={group.description} onChange={handleChange} />
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
              {group.properties.length === 0 ? (
                <tr>
                  <td colSpan="5" style={{ textAlign: 'center', color: '#999', fontStyle: 'italic' }}>
                    Chưa có thuộc tính. Bấm "Thêm thuộc tính" để bắt đầu.
                  </td>
                </tr>
              ) : (
                group.properties.map((prop, index) => (
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
                        placeholder="VD: Nam"
                      />
                    </td>
                    <td>
                      <input
                        type="text"
                        name="valueUnitOfMeasure"
                        value={prop.valueUnitOfMeasure}
                        onChange={(e) => handlePropertyChange(index, e)}
                        placeholder="VD: người"
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

        <button type="submit" className="create-group-btn">Tạo</button>
      </form>

      <div className="group-list">
        <h2>Danh sách nhóm nhân sự đã tạo</h2>
        <div className="table-newgroup-container">
          <table>
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã nhóm</th>
                <th>Tên nhóm</th>
                <th>Số thuộc tính</th>
                <th>Mô tả</th>
                <th>Xóa</th>
              </tr>
            </thead>
            <tbody>
              {groupList.length === 0 ? (
                <tr>
                  <td colSpan="6" style={{ textAlign: 'center' }}>Chưa có nhóm nào.</td>
                </tr>
              ) : (
                groupList.map((item, index) => (
                  <tr key={item.tempId}>
                    <td>{index + 1}</td>
                    <td>{item.code}</td>
                    <td>{item.name}</td>
                    <td>{item.properties.length}</td>
                    <td>{item.description}</td>
                    <td>
                      <button
                        type="button"
                        className="delete-btn"
                        onClick={() => handleDelete(item.tempId)}
                        aria-label={`Xóa nhóm ${item.code}`}
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
          disabled={isSubmitting || groupList.length === 0}
        >
          {isSubmitting ? 'Đang gửi...' : `Xác nhận (${groupList.length})`}
        </button>
      </div>
    </div>
  );
};

export default CreateNewGroupHuman;