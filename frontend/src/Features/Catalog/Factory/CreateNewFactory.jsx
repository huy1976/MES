import React, { useState, useEffect } from 'react';
import { toast } from "react-toastify"; // Import toast for notifications
import "react-toastify/dist/ReactToastify.css";
import './CreateNewFactory.css';
import { createSite } from '../../../api/Enterprise';

const CreateNewFactory = ({ savedState, onSaveState }) => {
  // === STATE BAN ĐẦU ===
  const initialFactoryState = {
    enterprisesId: 'E01',
    code: '',
    name: '',
    areas: []
  };

  // === KHÔI PHỤC STATE TỪ savedState NẾU CÓ ===
  const [factory, setFactory] = useState(
    savedState?.factory || initialFactoryState
  );
  const [factoryList, setFactoryList] = useState(
    savedState?.factoryList || []
  );

  // === EFFECT: SAVE STATE KHI factory HOẶC factoryList THAY ĐỔI ===
  useEffect(() => {
    if (onSaveState) {
      onSaveState({
        factory,
        factoryList,
      });
    }
  }, [factory, factoryList, onSaveState]);

  // === XỬ LÝ FORM ===
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFactory(prev => ({ ...prev, [name]: value }));
  };

  // === XỬ LÝ BẢNG AREAS ĐỘNG ===
  const handleAreaChange = (index, e) => {
    const { name, value } = e.target;
    const updatedAreas = [...factory.areas];
    updatedAreas[index][name] = value;
    setFactory((prev) => ({ ...prev, areas: updatedAreas }));
  };

  const addAreaRow = () => {
    setFactory((prev) => ({
      ...prev,
      areas: [
        ...prev.areas,
        { areaId: '', name: '' },
      ],
    }));
  };

  const removeAreaRow = (index) => {
    const updatedAreas = factory.areas.filter((_, i) => i !== index);
    setFactory((prev) => ({ ...prev, areas: updatedAreas }));
  };

  const handleAddToList = (e) => {
    e.preventDefault();
    if (!factory.enterprisesId || !factory.code || !factory.name) {
      toast.info("Vui lòng điền đầy đủ thông tin nhà máy!", {
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

    // === FILTER AREAS: CHỈ GIỮ LẠI NHỮNG AREA CÓ DỮ LIỆU ===
    const filteredAreas = factory.areas.filter(area => {
      const isAreaIdEmpty = !area.areaId || area.areaId.trim() === '';
      const isNameEmpty = !area.name || area.name.trim() === '';
      return !(isAreaIdEmpty && isNameEmpty);
    });

    const newFactory = { 
      ...factory,
      areas: filteredAreas,
      tempId: Date.now() 
    };
    setFactoryList(prev => [...prev, newFactory]);
    // === RESET FORM SAU KHI THÊM VÀO DANH SÁCH ===
    setFactory(initialFactoryState);
  };

  const handleDeleteFromList = (tempId) => {
    setFactoryList(prev => prev.filter(item => item.tempId !== tempId));
  };

  const handleConfirm = async () => {
    if (factoryList.length === 0) {
        toast.info("Không có nhà máy nào để xác nhận!", {
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

    for(const item of factoryList){
        const areas = item.areas.map((area) => ({
            areaId : area.areaId,
            name : area.name,
      }));
        
        const payload = {
          enterprisesId : item.enterprisesId,
          siteId : item.code,
          name : item.name,
          areas: areas
        }

        try {
          console.log("Đang gửi payload:", JSON.stringify(payload, null, 2));
          await createSite(payload.enterprisesId, payload)
          successes.push(item.code);
          toast.success("Tạo nhà máy thành công!", {
            position: "top-right",
            autoClose: 3000,
            hideProgressBar: false,
            closeOnClick: true,
            pauseOnHover: true,
            draggable: true,
            progress: undefined,
          });
        } catch {
          toast.error("Lỗi: Không thể tạo nhà máy!", {
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
    
    setFactoryList(prev => prev.filter(item => failures.includes(item.code)));
  };

  return (
    <>
          {/* TITLE */}
      <h2 className="title">Danh mục | Nhà máy</h2>

      {/* TABS */}
      <div className="tabs">
        <button className="tab-btn active">Nhà máy</button>
      </div>
    <div className="newfactory-container">
      <form onSubmit={handleAddToList}>
        <div>
          <label>Mã nhà máy</label>
          <input type="text" name="code" value={factory.code} onChange={handleChange} required />
        </div>
        <div>
          <label>Tên nhà máy</label>
          <input type="text" name="name" value={factory.name} onChange={handleChange} required />
        </div>

        <div className="properties-section">
          <h4>Khu vực (Areas)</h4>
          <table className="properties-table">
            <thead>
              <tr>
                <th>ID Khu vực</th>
                <th>Tên khu vực</th>
                <th>Xóa</th>
              </tr>
            </thead>
            <tbody>
              {factory.areas.length === 0 ? (
                <tr>
                  <td colSpan="3" style={{ textAlign: 'center', color: '#999', fontStyle: 'italic' }}>
                    Chưa có khu vực. Bấm "Thêm khu vực" để bắt đầu.
                  </td>
                </tr>
              ) : (
                factory.areas.map((area, index) => (
                  <tr key={index}>
                    <td>
                      <input
                        type="text"
                        name="areaId"
                        value={area.areaId}
                        onChange={(e) => handleAreaChange(index, e)}
                        placeholder="VD: A01"
                      />
                    </td>
                    <td>
                      <input
                        type="text"
                        name="name"
                        value={area.name}
                        onChange={(e) => handleAreaChange(index, e)}
                        placeholder="VD: Khu vực sản xuất A"
                      />
                    </td>
                    <td>
                      <button type="button" className="delete-btn" onClick={() => removeAreaRow(index)}>
                        <i className="fas fa-trash"></i>
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
          <button type="button" onClick={addAreaRow} className="add-area-btn">
            ➕ Thêm khu vực
          </button>
        </div>

        <button type="submit" className="create-factory-btn">Tạo</button>
      </form>

      <div className="factory-list">
        <h2>Danh sách nhà máy chờ xác nhận</h2>
        <div className="table-newfactory-container">
          <table>
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã nhà máy</th>
                <th>Tên nhà máy</th>
                <th>Số khu vực</th>
                <th>Hành động</th>
              </tr>
            </thead>
            <tbody>
              {factoryList.length === 0 ? (
                <tr>
                  <td colSpan="5" style={{ textAlign: 'center' }}>Chưa có nhà máy nào.</td>
                </tr>
              ) : (
                factoryList.map((item, index) => (
                  <tr key={item.tempId}>
                    <td>{index + 1}</td>
                    <td>{item.code}</td>
                    <td>{item.name}</td>
                    <td>{item.areas.length}</td>
                    <td>
                      <button
                        type="button"
                        className="delete-btn"
                        onClick={() => handleDeleteFromList(item.tempId)}
                        aria-label={`Xóa nhà máy ${item.code}`}
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
          disabled={factoryList.length === 0}
        >
          Xác nhận ({factoryList.length})
        </button>
      </div>
    </div>
    </>
  );
};

export default CreateNewFactory;