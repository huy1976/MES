import React, { useState } from 'react';
import Equipments from './Equipments';
import MachineCapacity from './MachineCapacity/MachineCapacity';
import GroupEquipments from './GroupEquipments';
import './Equipment.css';

const Equipment = () => {
  const [activeTab, setActiveTab] = useState('equipments');

  return (
    <div className="equipment-container">
      <h2 className="title">Danh mục | {activeTab === 'equipments' ? 'Thiết bị' : activeTab === 'machine-capacity' ? 'Năng lực của máy' : 'Nhóm thiết bị'}</h2>
      <div className="tabs">
        <button
          className={activeTab === 'equipments' ? 'active' : ''}
          onClick={() => setActiveTab('equipments')}
        >
          Thiết bị
        </button>
        <button
          className={activeTab === 'machine-capacity' ? 'active' : ''}
          onClick={() => setActiveTab('machine-capacity')}
        >
          Năng lực của máy
        </button>
        <button
          className={activeTab === 'group-equipments' ? 'active' : ''}
          onClick={() => setActiveTab('group-equipments')}
        >
          Nhóm thiết bị
        </button>
      </div>
      {activeTab === 'equipments' ? <Equipments /> : activeTab === 'machine-capacity' ? <MachineCapacity /> : <GroupEquipments />}
    </div>
  );
};

export default Equipment;