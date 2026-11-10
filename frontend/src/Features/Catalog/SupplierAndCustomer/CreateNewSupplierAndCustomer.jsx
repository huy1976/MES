import React, { useState, useEffect } from 'react';
import { toast } from "react-toastify"; // Import toast for notifications
import "react-toastify/dist/ReactToastify.css";
import './CreateNewSupplierAndCustomer.css';
import { createBusinesses, getBusinessPartnerPropertyNames } from '../../../api/Business';

// === MAPPING VÀ CONSTANTS ===
const roleToTypeMap = {
  "Nhà cung cấp": 0,
  "Khách hàng": 1,
  "Tất cả": 2
};

const roleList = [
  { label: "Nhà cung cấp", value: "Nhà cung cấp" },
  { label: "Khách hàng", value: "Khách hàng" },
  { label: "Tất cả", value: "Tất cả" }
];

// === STATE BAN ĐẦU ===
const initialPartnerState = {
  name: '',
  code: '',
  role: '',
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

const CreateNewSupplierAndCustomer = ({ savedState, onSaveState }) => {
  // === KHÔI PHỤC STATE TỪ savedState NẾU CÓ ===
  const [partner, setPartner] = useState(
    savedState?.partner || initialPartnerState
  );
  const [partnerList, setPartnerList] = useState(
    savedState?.partnerList || []
  );

  const [propertyNames, setPropertyNames] = useState([]);

  useEffect(() => {
      const GetApi = async () => {
        try {
          // TODO: Replace with actual API calls
          const names = await getBusinessPartnerPropertyNames();

          setPropertyNames(names);
        } catch (error) {
          console.error(error);
        }
      };
      GetApi();
    }, []);

  // === EFFECT: SAVE STATE KHI partner HOẶC partnerList THAY ĐỔI ===
  useEffect(() => {
    if (onSaveState) {
      onSaveState({
        partner,
        partnerList,
      });
    }
  }, [partner, partnerList, onSaveState]);

  // === XỬ LÝ FORM CHÍNH ===
  const handleChange = (e) => {
    const { name, value } = e.target;
    setPartner(prev => ({ ...prev, [name]: value }));
  };

  // === XỬ LÝ BẢNG THUỘC TÍNH ĐỘNG ===
  const handlePropertyChange = (index, e) => {
    const { name, value } = e.target;
    const updatedProperties = [...partner.properties];
    updatedProperties[index][name] = value;
    setPartner((prev) => ({ ...prev, properties: updatedProperties }));
  };

  const addPropertyRow = () => {
    setPartner((prev) => ({
      ...prev,
      properties: [
        ...prev.properties,
        { name: '', description: '', valueString: '' },
      ],
    }));
  };

  const removePropertyRow = (index) => {
    const updatedProperties = partner.properties.filter((_, i) => i !== index);
    setPartner((prev) => ({ ...prev, properties: updatedProperties }));
  };

  const handleAddToList = (e) => {
    e.preventDefault();
    if (!partner.name || !partner.code || !partner.role) {
        toast.info("Vui lòng điền đầy đủ Tên, Mã và Vai trò của đối tác!", {
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

    // === LỌC CÁC PROPERTIES TRỐNG ===
    const filteredProperties = partner.properties.filter(prop => {
      const isNameEmpty = !prop.name || prop.name.trim() === '';
      const isDescEmpty = !prop.description || prop.description.trim() === '';
      const isValueEmpty = !prop.valueString || prop.valueString.trim() === '';
      return !(isNameEmpty && isDescEmpty && isValueEmpty);
    });

    const newPartner = { 
      ...partner, 
      properties: filteredProperties,
      tempId: Date.now() 
    };
    setPartnerList(prev => [...prev, newPartner]);
    setPartner(initialPartnerState);
  };

  const handleDeleteFromList = (tempId) => {
    setPartnerList(prev => prev.filter(item => item.tempId !== tempId));
  };

  // === HÀM LẤY GIÁ TRỊ PROPERTY ===
  const getPropertyValue = (item, propName) => {
    const prop = item.properties.find(p => p.name === propName);
    return prop?.valueString || '';
  };

  const handleConfirm = async () => {
    if (partnerList.length === 0) {
        toast.info("Không có đối tác nào để xác nhận!", {
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

    for (const item of partnerList) {
      const properties = item.properties.map((prop) => ({
        description: prop.description || "",
        name: prop.name,
        valueString: prop.valueString,
        valueType: getValueType(prop.valueString),
        valueUnitOfMeasure: ""
      }));

      const partnerType = roleToTypeMap[item.role] !== undefined ? roleToTypeMap[item.role] : 0;
      const payload = {
        businessPartnerId: item.code,
        name: item.name,
        description: "",
        businessPartnerType: partnerType,
        properties: properties,
      };

      try {
        console.log("Đang gửi payload:", JSON.stringify(payload, null, 2));
        await createBusinesses(payload);
        successes.push(item.code);
        toast.success("Tạo đối tác thành công!", {
        position: "top-right",
        autoClose: 3000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
      });
      } catch (error) {
        toast.error("Lỗi: Không thể tạo đối tác!", {
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
    setPartnerList(prev => prev.filter(item => failures.includes(item.code)));
  };

  return (
    <>
      {/* TITLE */}
      <h2 className="title">Danh mục | Khách hàng/Nhà cung cấp</h2>

      {/* TABS */}
      <div className="tabs">
        <button className="tab-btn active">Khách hàng/Nhà cung cấp</button>
      </div>

      <div className="newpartner-container">
        <form onSubmit={handleAddToList}>
          <div>
            <label>Tên đối tác</label>
            <input type="text" name="name" value={partner.name} onChange={handleChange} required />
          </div>
          <div>
            <label>Mã đối tác</label>
            <input type="text" name="code" value={partner.code} onChange={handleChange} required />
          </div>
          <div>
            <label>Vai trò</label>
            <select name="role" value={partner.role} onChange={handleChange} required>
              <option value="" disabled>Chọn vai trò</option>
              {roleList.map((role) => (
                <option key={role.value} value={role.value}>{role.label}</option>
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
                  <th>Xóa</th>
                </tr>
              </thead>
              <tbody>
                {partner.properties.length === 0 ? (
                  <tr>
                    <td colSpan="4" style={{ textAlign: 'center', color: '#999', fontStyle: 'italic' }}>
                      Chưa có thuộc tính. Bấm "Thêm thuộc tính" để bắt đầu.
                    </td>
                  </tr>
                ) : (
                  partner.properties.map((prop, index) => (
                    <tr key={index}>
                      <td>
                        <select
                          name="name"
                          value={prop.name}
                          onChange={(e) => handlePropertyChange(index, e)}
                        >
                          <option value="" disabled>Chọn thuộc tính</option>
                          {propertyNames.map((name) => (
                            <option key={name} value={name}>{name}</option>
                          ))}
                        </select>
                      </td>
                      <td>
                        <input
                          type="text"
                          name="description"
                          value={prop.description}
                          onChange={(e) => handlePropertyChange(index, e)}
                          placeholder="Mô tả (tùy chọn)"
                        />
                      </td>
                      <td>
                        <input
                          type="text"
                          name="valueString"
                          value={prop.valueString}
                          onChange={(e) => handlePropertyChange(index, e)}
                          placeholder="Nhập giá trị"
                          required
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

          <button type="submit" className="create-partner-btn">Tạo</button>
        </form>

        <div className="partner-list">
          <h2>Danh sách đối tác và nhà cung cấp đã tạo</h2>
          <div className="table-newpartner-container">
            <table>
              <thead>
                <tr>
                  <th>STT</th>
                  <th>Tên đối tác</th>
                  <th>Mã đối tác</th>
                  <th>Vai trò</th>
                  <th>Địa chỉ</th>
                  <th>Số điện thoại</th>
                  <th>Email</th>
                  <th>Số thuộc tính</th>
                  <th>Hành động</th>
                </tr>
              </thead>
              <tbody>
                {partnerList.length === 0 ? (
                  <tr>
                    <td colSpan="9" style={{ textAlign: 'center' }}>Chưa có đối tác nào.</td>
                  </tr>
                ) : (
                  partnerList.map((item, index) => (
                    <tr key={item.tempId}>
                      <td>{index + 1}</td>
                      <td>{item.name}</td>
                      <td>{item.code}</td>
                      <td>{item.role}</td>
                      <td>{getPropertyValue(item, 'address')}</td>
                      <td>{getPropertyValue(item, 'Phone')}</td>
                      <td>{getPropertyValue(item, 'email')}</td>
                      <td>{item.properties.length}</td>
                      <td>
                        <button 
                          type="button" 
                          className="delete-btn" 
                          onClick={() => handleDeleteFromList(item.tempId)}
                          aria-label={`Xóa đối tác ${item.code}`}
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
            disabled={partnerList.length === 0}
          >
            Xác nhận ({partnerList.length})
          </button>
        </div>
      </div>
    </>
  );
};

export default CreateNewSupplierAndCustomer;
