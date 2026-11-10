import React, { useState } from 'react';
import Materials from './Materials';
import GroupMaterials from './GroupMaterials';
import './Material.css';

const Material = () => {
  const [activeTab, setActiveTab] = useState('materials');

  return (
    <div className="material-container">
      <h2 className="title">Danh mục | {activeTab === 'materials' ? 'Vật tư' : 'Nhóm vật tư'}</h2>
      <div className="tabs">
        <button
          className={activeTab === 'materials' ? 'active' : ''}
          onClick={() => setActiveTab('materials')}
        >
          Vật tư
        </button>
        <button
          className={activeTab === 'material-group' ? 'active' : ''}
          onClick={() => setActiveTab('material-group')}
        >
          Nhóm vật tư
        </button>
      </div>
      {activeTab === 'materials' ? <Materials /> : <GroupMaterials />}
    </div>
  );
};

export default Material;