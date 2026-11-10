import React, { useState } from 'react';
import Stage from './Stage/Stage';
import StageRequirement from './StageRequirement/StageRequirement';
import './Stagecatalog.css';
import { useAuthPermissions } from '../../../api/UseAuthPermissions';

const Stagecatalog = () => {
  const [activeTab, setActiveTab] = useState('stage');
  const { permissions, loading: permLoading, error: permError } = useAuthPermissions(8);
  const canView = permissions.some((p) => p.canView);
  const canEdit = permissions.some((p) => p.canEdit);

  return (
    <div className="stagecatalog-container">
      <h2 className="title">Danh mục | {activeTab === 'stage' ? 'Công đoạn' : 'Yêu cầu công đoạn'}</h2>
      <div className="tabs">
        <button
          className={activeTab === 'stage' ? 'active' : ''}
          onClick={() => setActiveTab('stage')}
        >
          Công đoạn
        </button>
        <button
          className={activeTab === 'stage-requirement' ? 'active' : ''}
          onClick={() => setActiveTab('stage-requirement')}
        >
          Yêu cầu công đoạn
        </button>
      </div>
      {activeTab === 'stage' ? <Stage /> : <StageRequirement />}
    </div>
  );
};

export default Stagecatalog;