import React from 'react';
import { FaHome, FaFileAlt, FaClipboardList, FaCalendarAlt, FaChartLine, FaDatabase, FaUsers, FaCog, FaMicrochip } from 'react-icons/fa';
import './Sidebar.css';
import BKlogo from './../../../assets/bk_logo.png';

const Sidebar = ({ activeId, onMenuClick }) => {
  const menuItems = [
    { id: 1, icon: <FaHome />, label: 'Tổng quan' },
    { id: 2, icon: <FaFileAlt />, label: 'Yêu cầu sản xuất' },
    { id: 3, icon: <FaClipboardList />, label: 'Kế hoạch sản xuất' },
    { id: 4, icon: <FaCalendarAlt />, label: 'Lịch sản xuất' },
    { id: 5, icon: <FaUsers />, label: 'Kế hoạch tài nguyên' },
    { id: 6, icon: <FaChartLine />, label: 'Tiến độ & sự cố' },
    { id: 7, icon: <FaDatabase />, label: 'Danh mục' },
    { id: 9, icon: <FaMicrochip />, label: 'Giám sát máy Dem' },
    { id: 8, icon: <FaCog />, label: 'Cài đặt' },
  ];

  const handleMenuClickLocal = (id) => {
    onMenuClick(id); // Always call onMenuClick, even if the item is already active
  };

  return (
    <div className="sidebar">
      <div className="sidebar-header">
        <div className="logo">
          <img src={BKlogo} className="logo-img" alt="ảnh bách khoa" />
        </div>
        <h2 className="portal-title">MES Portal</h2>
      </div>
      <nav className="sidebar-nav">
        <ul>
          {menuItems.map((item) => (
            <li key={item.id} data-id={item.id} className={activeId === item.id ? 'active' : ''}>
              <button onClick={() => handleMenuClickLocal(item.id)}>
                <span className="icon">{item.icon}</span>
                <span className="label">{item.label}</span>
              </button>
            </li>
          ))}
        </ul>
      </nav>
    </div>
  );
};

export default Sidebar;