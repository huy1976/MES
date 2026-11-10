import React, { useState, useMemo } from 'react';
import CreateNewWorkTime from './CreateNewWorkTime';
import './WorkTime.css';
import { useAuthPermissions } from '../../../api/UseAuthPermissions';

const WorkTime = () => {
const [shifts] = useState([
  { id: 1, code: 'CA001', name: 'Ca 1', startTime: '06:00', endTime: '14:00', duration: '8 giờ' },
  { id: 2, code: 'CA002', name: 'Ca 2', startTime: '14:00', endTime: '22:00', duration: '8 giờ' },
  { id: 3, code: 'CA003', name: 'Ca 3', startTime: '22:00', endTime: '06:00', duration: '8 giờ' },
]);

  const [showCreateNew, setShowCreateNew] = useState(false);
  const [showFilter, setShowFilter] = useState(false);
  const [filterValues, setFilterValues] = useState({ code: '', name: 'Tất cả' });

  const { permissions, loading: permLoading, error: permError } = useAuthPermissions(9);
  const canView = permissions.some((p) => p.canView);
  const canEdit = permissions.some((p) => p.canEdit);

  const handleCreateNew = () => {
    setShowCreateNew(true);
    setShowFilter(false);
  };

  const handleBack = () => {
    setShowCreateNew(false);
    setShowFilter(false);
  };

  const toggleFilter = () => {
    setShowFilter(!showFilter);
  };

  const handleFilterChange = (e) => {
    const { name, value } = e.target;
    setFilterValues((prev) => ({ ...prev, [name]: value }));
  };

  const applyFilter = () => {
    setShowFilter(false);
  };

  const filteredShifts = useMemo(() => {
    let result = [...shifts];

    if (filterValues.code) {
      result = result.filter((shift) =>
        shift.code.toLowerCase().includes(filterValues.code.toLowerCase())
      );
    }

    if (filterValues.name !== 'Tất cả') {
      result = result.filter((shift) => shift.name === filterValues.name);
    }

    return result;
  }, [shifts, filterValues]);

  return (
    <div className="work-time-container">
      {showCreateNew ? (
        <>
          <div className="detail-nav">
            <button onClick={handleBack} className="back-btn" aria-label="Quay lại danh sách">
              ← Quay lại
            </button>
          </div>
          <CreateNewWorkTime />
        </>
      ) : (
        <>
          <h2 className="title">Danh mục | Ca làm việc</h2>
          <div className="actions">
            <button
              className="action-btn filter-btn"
              onClick={toggleFilter}
              aria-label="Tìm kiếm ca làm việc"
            >
              <span className="icon">🔍</span> Tìm kiếm
            </button>
            <button
              className={`action-btn new-btn ${!canEdit ? "disabled-button" : ""}`}
              onClick={handleCreateNew}
              aria-label="Tạo mới ca làm việc"
              title={!canEdit ? "Bạn không có quyền tạo ca làm việc" : "Tạo mới ca làm việc"}
              disabled={!canEdit}
            >
              <span className="icon">➕</span> Tạo mới
            </button>
            {showFilter && (
              <div className="filter-panel active">
                <div>
                  <label>Mã ca</label>
                  <input
                    type="text"
                    name="code"
                    placeholder="Tìm kiếm theo Mã ca"
                    value={filterValues.code}
                    onChange={handleFilterChange}
                    aria-label="Tìm kiếm theo mã ca"
                  />
                </div>
                <div>
                  <label>Tên ca</label>
                  <select
                    name="name"
                    value={filterValues.name}
                    onChange={handleFilterChange}
                    aria-label="Lọc theo tên ca"
                  >
                    <option value="Tất cả">Tất cả</option>
                    <option value="Ca sáng">Ca sáng</option>
                    <option value="Ca chiều">Ca chiều</option>
                    <option value="Ca tối">Ca tối</option>
                    <option value="Ca đêm">Ca đêm</option>
                    <option value="Ca hành chính">Ca hành chính</option>
                  </select>
                </div>
                <button className="search-btn" onClick={applyFilter} aria-label="Áp dụng bộ lọc">
                  Tìm kiếm
                </button>
                <button className="close-btn" onClick={toggleFilter} aria-label="Đóng bộ lọc">
                  Đóng
                </button>
              </div>
            )}
          </div>
          <div className="shifts-table">
            <div className="table-container">
              <table role="grid">
                <thead>
                  <tr>
                    <th>STT</th>
                    <th>Mã ca</th>
                    <th>Tên ca</th>
                    <th>Thời gian bắt đầu</th>
                    <th>Thời gian kết thúc</th>
                    <th>Thời gian kéo dài</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredShifts.map((shift) => (
                    <tr key={shift.id}>
                      <td>{shift.id}</td>
                      <td>{shift.code}</td>
                      <td>{shift.name}</td>
                      <td>{shift.startTime}</td>
                      <td>{shift.endTime}</td>
                      <td>{shift.duration}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </>
      )}
    </div>
  );
};

export default WorkTime;