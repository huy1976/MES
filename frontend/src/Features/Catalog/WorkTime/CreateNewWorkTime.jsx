import React, { useState } from 'react';
import './CreateNewWorkTime.css';

const CreateNewWorkTime = () => {
  const [shift, setShift] = useState({
    code: '',
    name: '',
    startTime: '',
    endTime: '',
    duration: ''
  });
  const [shiftList, setShiftList] = useState([]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setShift(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    setShiftList(prev => [...prev, { ...shift, id: prev.length + 1 }]);
    setShift({
      code: '',
      name: '',
      startTime: '',
      endTime: '',
      duration: ''
    });
    console.log('Shift added:', shift);
  };

  const handleDelete = (id) => {
    setShiftList(prev => prev.filter(item => item.id !== id));
  };

  return (
    <div className="newshift-container">
      <div className="form-container">
        <form onSubmit={handleSubmit}>
          <div>
            <label>Mã ca</label>
            <input type="text" name="code" value={shift.code} onChange={handleChange} />
          </div>
          <div>
            <label>Tên ca</label>
            <input type="text" name="name" value={shift.name} onChange={handleChange} />
          </div>
          <div>
            <label>Thời gian bắt đầu</label>
            <input type="time" name="startTime" value={shift.startTime} onChange={handleChange} />
          </div>
          <div>
            <label>Thời gian kết thúc</label>
            <input type="time" name="endTime" value={shift.endTime} onChange={handleChange} />
          </div>
          <div>
            <label>Thời gian kéo dài</label>
            <input type="text" name="duration" value={shift.duration} onChange={handleChange} />
          </div>
          <button type="submit" className="create-shift-btn">Tạo</button>
        </form>
      </div>

      <div className="shift-list">
        <h2>Danh sách ca làm việc đã tạo</h2>
        <div className="table-newshift-container">
          <table>
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã ca</th>
                <th>Tên ca</th>
                <th>Thời gian bắt đầu</th>
                <th>Thời gian kết thúc</th>
                <th>Thời gian kéo dài</th>
                <th>Xóa</th>
              </tr>
            </thead>
            <tbody>
              {shiftList.map((item) => (
                <tr key={item.id}>
                  <td>{item.id}</td>
                  <td>{item.code}</td>
                  <td>{item.name}</td>
                  <td>{item.startTime}</td>
                  <td>{item.endTime}</td>
                  <td>{item.duration}</td>
                  <td>
                    <button
                      className="delete-btn"
                      onClick={() => handleDelete(item.id)}
                      aria-label={`Xóa ca ${item.code}`}
                    >
                      <span className="delete-icon"><i className="fas fa-trash"></i></span>
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <button type="button" className="confirm-btn">Xác nhận</button>
      </div>
    </div>
  );
};

export default CreateNewWorkTime;