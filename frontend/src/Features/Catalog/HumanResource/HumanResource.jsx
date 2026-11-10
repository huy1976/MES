import React, { useState } from 'react';
import Human from './Human/Human';
import HumanCapacity from './HumanCapacity/HumanCapacity';
import GroupHuman from './GroupHuman';
import './HumanResource.css';

const HumanResource = () => {
  const [activeTab, setActiveTab] = useState('human');

  return (
    <div className="human-resource-container">
      <h2 className="title">Danh mục | {activeTab === 'human' ? 'Nhân sự' : activeTab === 'human-capacity' ? 'Năng lực nhân sự' : 'Nhóm nhân sự'}</h2>
      <div className="tabs">
        <button
          className={activeTab === 'human' ? 'active' : ''}
          onClick={() => setActiveTab('human')}
        >
          Nhân sự
        </button>
        <button
          className={activeTab === 'human-capacity' ? 'active' : ''}
          onClick={() => setActiveTab('human-capacity')}
        >
          Năng lực nhân sự
        </button>
        <button
          className={activeTab === 'group-human' ? 'active' : ''}
          onClick={() => setActiveTab('group-human')}
        >
          Nhóm nhân sự
        </button>
      </div>
      {activeTab === 'human' ? <Human /> : activeTab === 'human-capacity' ? <HumanCapacity /> : <GroupHuman />}
    </div>
  );
};

export default HumanResource;