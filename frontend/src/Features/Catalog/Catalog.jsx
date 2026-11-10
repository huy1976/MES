import React, { Component, useState } from 'react';
import './Catalog.css';
import Material from './Material/Material';
import Equipment from './Equipment/Equipment';
import HumanResource from './HumanResource/HumanResource';
import SupplierAndCustomer from './SupplierAndCustomer/SupplierAndCustomer';
import WorkTime from './WorkTime/WorkTime';
import Factory from './Factory/Factory';
import WorkCenter from './WorkCenter/WorkCenter';
import Formula from './Formula/Formula';
import StageCatalog from './StageCatalog/StageCatalog';
import BOM from './BOM/BOM';
import Quality from './Quality/Quality';
import ProductionParameter from './ProductionParameter/ProductionParameter';

const Catalog = () => {
  const [selectedItem, setSelectedItem] = useState(null);

  const items = [
    { id: '1', icon: '📦', title: 'Vật tư', desc: 'Quản lý sản phẩm và nguyên vật liệu', component: <Material /> },
    { id: '2', icon: '⚙️', title: 'Thiết bị', desc: 'Theo dõi thiết bị và tình trạng thiết bị', component: <Equipment /> },
    { id: '3', icon: '👥', title: 'Nhân sự', desc: 'Thông tin và phân công nhân viên', component: <HumanResource /> },
    { id: '4', icon: '📋', title: 'Khách hàng/Nhà cung cấp', desc: 'Quản lý khách hàng và nhà cung cấp', component: <SupplierAndCustomer /> },
    { id: '5', icon: '⏰', title: 'Ca làm việc', desc: 'Thiết lập lịch ca làm việc', component: <WorkTime /> },
    { id: '6', icon: '🏭', title: 'Nhà máy', desc: 'Thông tin và vị trí nhà máy', component: <Factory /> },
    { id: '7', icon: '🛠️', title: 'WorkCenter', desc: 'Quản lý dây chuyền sản xuất', component: <WorkCenter /> },
    { id: '8', icon: '🔧', title: 'Phương thức sản xuất', desc: 'Cấu hình quy trình sản xuất', component: <Formula /> },
    { id: '9', icon: '🔗', title: 'Công đoạn', desc: 'Các bước trong quy trình sản xuất', component: <StageCatalog /> },
    { id: '10', icon: '📊', title: 'BOM', desc: 'Định mức nguyên vật liệu', component: <BOM /> },
    { id: '11', icon: '✅', title: 'Chất lượng', desc: 'Quản lý kiểm tra chất lượng', component: <Quality /> },
    { id: '12', icon: '📈', title: 'Thống số sản xuất', desc: 'Quản lý các công thức sản xuất của công đoạn', component: <ProductionParameter /> },
  ];

  const handleItemClick = (item) => {
    setSelectedItem(item.component || null);
  };

  return (
    <div className="catalog-container">
      {!selectedItem ? (
        items.map((item) => (
          <div key={item.id} className="catalog-item" onClick={() => handleItemClick(item)}>
            <span className="item-icon">{item.icon}</span>
            <div className="item-content">
              <h3>{item.title}</h3>
              <p>{item.desc}</p>
            </div>
          </div>
        ))
      ) : (
        <div className="catalog-detail-view">
          {selectedItem}
        </div>
      )}
    </div>
  );
};

export default Catalog;