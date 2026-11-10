import React, { useState, useEffect } from 'react';
import { toast } from "react-toastify"; // Import toast for notifications
import "react-toastify/dist/ReactToastify.css";
import './CreateNewWorkCenter.css';
import { getSiteIds, getAreaIds, createWorkCenter} from '../../../api/Enterprise';

const workCenterTypeMap = {
    'Ô quy trình' : 0,
    'Đơn vị sản xuất' : 1,
    'Dây chuyền sản xuất' : 2,
    'Khu vực lưu trữ' : 3
}

const CreateNewWorkCenter = ({ savedState, onSaveState }) => {
  // === STATE BAN ĐẦU ===
  const initialWorkCenterState = {
    enterprisesId: 'E01',
    siteId: '',
    areaId: '',
    code: '',
    name: '',
    type: '',
    workUnits: []
  };

  // === KHÔI PHỤC STATE TỪ savedState NẾU CÓ ===
  const [workCenter, setWorkCenter] = useState(
    savedState?.workCenter || initialWorkCenterState
  );
  const [workCenterList, setWorkCenterList] = useState(
    savedState?.workCenterList || []
  );

  const [siteIds, setSiteIds] = useState([]);
  const [areaIds, setAreaIds] = useState([]);

  useEffect(() => {
      const GetApi = async () => {
        try {
          const siteIdList = await getSiteIds();
          const AreaIdList = await getAreaIds();

          setSiteIds(siteIdList);
          setAreaIds(AreaIdList);
        } catch (error) {
          console.error(error);
          setSiteIds([]);
          setAreaIds([]);
        }
      };
      GetApi();
    }, []);

  // === EFFECT: SAVE STATE KHI workCenter HOẶC workCenterList THAY ĐỔI ===
  useEffect(() => {
    if (onSaveState) {
      onSaveState({
        workCenter,
        workCenterList,
      });
    }
  }, [workCenter, workCenterList, onSaveState]);

  // === XỬ LÝ FORM ===
  const handleChange = (e) => {
    const { name, value } = e.target;
    setWorkCenter(prev => ({ ...prev, [name]: value }));
  };

  // === XỬ LÝ BẢNG WORK UNITS ĐỘNG ===
  const handleWorkUnitChange = (index, e) => {
    const { name, value } = e.target;
    const updatedWorkUnits = [...workCenter.workUnits];
    updatedWorkUnits[index][name] = value;
    setWorkCenter((prev) => ({ ...prev, workUnits: updatedWorkUnits }));
  };

  const addWorkUnitRow = () => {
    setWorkCenter((prev) => ({
      ...prev,
      workUnits: [
        ...prev.workUnits,
        { workUnitId: '', name: '' },
      ],
    }));
  };

  const removeWorkUnitRow = (index) => {
    const updatedWorkUnits = workCenter.workUnits.filter((_, i) => i !== index);
    setWorkCenter((prev) => ({ ...prev, workUnits: updatedWorkUnits }));
  };

  const handleAddToList = (e) => {
    e.preventDefault();
    if (!workCenter.enterprisesId || !workCenter.code || !workCenter.name) {
        toast.info("Vui lòng điền đầy đủ thông tin WorkCenter!", {
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

    // === FILTER WORK UNITS: CHỈ GIỮ LẠI NHỮNG WORK UNIT CÓ DỮ LIỆU ===
    const filteredWorkUnits = workCenter.workUnits.filter(unit => {
      const isWorkUnitIdEmpty = !unit.workUnitId || unit.workUnitId.trim() === '';
      const isNameEmpty = !unit.name || unit.name.trim() === '';
      return !(isWorkUnitIdEmpty && isNameEmpty);
    });

    const newWorkCenter = { 
      ...workCenter,
      workUnits: filteredWorkUnits,
      tempId: Date.now() 
    };
    setWorkCenterList(prev => [...prev, newWorkCenter]);
    // === RESET FORM SAU KHI THÊM VÀO DANH SÁCH ===
    setWorkCenter(initialWorkCenterState);
  };

  const handleDeleteFromList = (tempId) => {
    setWorkCenterList(prev => prev.filter(item => item.tempId !== tempId));
  };

  const handleConfirm = async () => {
    if (workCenterList.length === 0) {
        toast.info("Không có WorkCenter nào để xác nhận!", {
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

    for (const item of workCenterList) {
      const workUnits = item.workUnits.map((unit) => ({
        workUnitId: unit.workUnitId,
        name: unit.name,
      }));

      const payload = {
        workCenterId: item.code,
        name: item.name,
        workCenterType: workCenterTypeMap[item.type],
        workUnits: workUnits
      };

      try {
        console.log("Đang gửi payload:", JSON.stringify(payload, null, 2));
        await createWorkCenter(item.enterprisesId, item.siteId, item.areaId, payload);
        successes.push(item.code);
        toast.success("Tạo WorkCenter thành công!", {
        position: "top-right",
        autoClose: 3000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
      });
      } catch (error) {
      toast.error("Lỗi: Không thể tạo WorkCenter!", {
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
    setWorkCenterList(prev => prev.filter(item => failures.includes(item.code)));
  };

  return (
    <>
      {/* TITLE */}
      <h2 className="title">Danh mục | WorkCenter</h2>

      {/* TABS */}
      <div className="tabs">
        <button className="tab-btn active">WorkCenter</button>
      </div>

      <div className="newworkcenter-container">
        <form onSubmit={handleAddToList}>
          <div>
            <label>Mã WorkCenter</label>
            <input type="text" name="code" value={workCenter.code} onChange={handleChange} required />
          </div>
          <div>
            <label>Tên WorkCenter</label>
            <input type="text" name="name" value={workCenter.name} onChange={handleChange} required />
          </div>
          <div>
            <label>Loại WorkCenter</label>
            <select name="type" value={workCenter.type} onChange={handleChange}>
              <option value="">Chọn loại</option>
              <option value="Ô quy trình">Ô quy trình</option>
              <option value="Đơn vị sản xuất">Đơn vị sản xuất</option>
              <option value="Dây chuyền sản xuất">Dây chuyền sản xuất</option>
              <option value="Khu vực lưu trữ">Khu vực lưu trữ</option>
            </select>
          </div>
          <div>
            <label>Mã nhà máy</label>
              <select name="siteId" value={workCenter.siteId} onChange={handleChange} required>
                <option value="" disabled>Chọn mã nhà máy</option>
                {siteIds.map((id, item) => (
                  <option key={item} value={id}>{id}</option>
                ))}
              </select>
          </div>
          <div>
            <label>Mã khu vực</label>
            <select name="areaId" value={workCenter.areaId} onChange={handleChange} required>
                <option value="" disabled>Chọn mã khu vực</option>
                {areaIds.map((id, item) => (
                  <option key={item} value={id}>{id}</option>
                ))}
           </select>
          </div>

          <div className="properties-section">
            <h4>Đơn vị làm việc (Work Units)</h4>
            <table className="properties-table">
              <thead>
                <tr>
                  <th>ID Đơn vị làm việc</th>
                  <th>Tên đơn vị làm việc</th>
                  <th>Xóa</th>
                </tr>
              </thead>
              <tbody>
                {workCenter.workUnits.length === 0 ? (
                  <tr>
                    <td colSpan="3" style={{ textAlign: 'center', color: '#999', fontStyle: 'italic' }}>
                      Chưa có đơn vị làm việc. Bấm "Thêm đơn vị làm việc" để bắt đầu.
                    </td>
                  </tr>
                ) : (
                  workCenter.workUnits.map((unit, index) => (
                    <tr key={index}>
                      <td>
                        <input
                          type="text"
                          name="workUnitId"
                          value={unit.workUnitId}
                          onChange={(e) => handleWorkUnitChange(index, e)}
                          placeholder="VD: WU01"
                        />
                      </td>
                      <td>
                        <input
                          type="text"
                          name="name"
                          value={unit.name}
                          onChange={(e) => handleWorkUnitChange(index, e)}
                          placeholder="VD: Đơn vị làm việc 1"
                        />
                      </td>
                      <td>
                        <button type="button" className="delete-btn" onClick={() => removeWorkUnitRow(index)}>
                          <i className="fas fa-trash"></i>
                        </button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
            <button type="button" onClick={addWorkUnitRow} className="add-area-btn">
              ➕ Thêm đơn vị làm việc
            </button>
          </div>

          <button type="submit" className="create-workcenter-btn">Tạo</button>
        </form>

        <div className="workcenter-list">
          <h2>Danh sách WorkCenter chờ xác nhận</h2>
          <div className="table-newworkcenter-container">
            <table>
              <thead>
                <tr>
                  <th>STT</th>
                  <th>Mã WorkCenter</th>
                  <th>Tên WorkCenter</th>
                  <th>Nhà máy</th>
                  <th>Khu vực</th>
                  <th>Loại WorkCenter</th>
                  <th>Số đơn vị làm việc</th>
                  <th>Hành động</th>
                </tr>
              </thead>
              <tbody>
                {workCenterList.length === 0 ? (
                  <tr>
                    <td colSpan="8" style={{ textAlign: 'center' }}>Chưa có WorkCenter nào.</td>
                  </tr>
                ) : (
                  workCenterList.map((item, index) => (
                    <tr key={item.tempId}>
                      <td>{index + 1}</td>
                      <td>{item.code}</td>
                      <td>{item.name}</td>
                      <td>{item.siteId}</td>
                      <td>{item.areaId}</td>
                      <td>{item.type}</td>
                      <td>{item.workUnits.length}</td>
                      <td>
                        <button
                          type="button"
                          className="delete-btn"
                          onClick={() => handleDeleteFromList(item.tempId)}
                          aria-label={`Xóa WorkCenter ${item.code}`}
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
            disabled={workCenterList.length === 0}
          >
            Xác nhận ({workCenterList.length})
          </button>
        </div>
      </div>
    </>
  );
};

export default CreateNewWorkCenter;