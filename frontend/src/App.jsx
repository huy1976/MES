import React, { useState } from 'react';
// 1. Import AuthProvider và useAuth
import { AuthProvider, useAuth } from './common/components/Login/AuthContext'; 
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

// Import các component khác
import Login from './common/components/Login/LoginScreen';
import Sidebar from './common/components/Sidebar/Sidebar';
import Dashboard from './Features/Dashboard/Dashboard';
import OrderRequire from './Features/OrderRequire/OrderRequire';
import ProductionPlan from './Features/ProductionPlan/ProductionPlan';
import ProductionSchedule from './Features/ProductionSchedule/ProductionSchedule';
import Catalog from './Features/Catalog/Catalog';
import ProgressAndProblem from './Features/ProgressAndProblem/ProgressAndProblem';
import HumanResourceManagement from './Features/HumanResourceManagement/HumanResourceManagement';
import SettingPopup from './common/components/Popup/Setting';
import AIChatBox from './common/components/ChatBox/AIChatBox';
import DemMachineDashboard from './Features/DataIot/DemMachineDashboard';
import './App.css';

// Component chính chứa logic điều hướng (được tách ra)
function AppContent() {
  // Lấy trạng thái xác thực từ Context
  const { isAuthenticated } = useAuth(); 
  const [showSettingPopup, setShowSettingPopup] = useState(false);
  // Giữ lại logic điều hướng và re-render cho Sidebar
  const [currentId, setCurrentId] = useState(1); // Mặc định ID=1 sau login
  const [reloadKey, setReloadKey] = useState(0); // Key để trigger reload

  const handleMenuClick = (id) => {
    if (id === 8) {
      setShowSettingPopup(true);
      return; 
    }
    if (id === currentId) {
      // Nếu click vào mục đã active, tăng reloadKey để trigger re-render
      setReloadKey(prev => prev + 1);
    } else {
      // Nếu click vào mục khác, cập nhật currentId và reset reloadKey
      setCurrentId(id);
      setReloadKey(0);
    }
  };

  // ⚠️ SỬ DỤNG isAuthenticated TỪ CONTEXT
  if (!isAuthenticated) {
    return (
      <div className="App">
        {/* Component Login không cần onLoginSuccess nữa, vì nó gọi setAuthToken trong Context */}
        <Login /> 
      </div>
    );
  }

  // Giao diện sau khi đăng nhập
  return (
    <div className="App">
      <Sidebar activeId={currentId} onMenuClick={handleMenuClick} />
      <main className="main-content">
        {
          currentId === 2 ? <OrderRequire key={`order-require-${reloadKey}`} /> :
          currentId === 3 ? <ProductionPlan key={`production-plan-${reloadKey}`} /> :
          currentId === 4 ? <ProductionSchedule key={`production-schedule-${reloadKey}`} /> :
          currentId === 5 ? <HumanResourceManagement key={`hr-management-${reloadKey}`} /> :
          currentId === 6 ? <ProgressAndProblem key={`progress-problem-${reloadKey}`} /> :
          currentId === 7 ? <Catalog key={`catalog-${reloadKey}`} /> :
          currentId === 9 ? <DemMachineDashboard key={`dem-machine-${reloadKey}`} /> :
          <Dashboard key={`dashboard-${reloadKey}`} currentId={currentId} />
        }
      </main>
      <SettingPopup
      isOpen={showSettingPopup}
      onClose={() => setShowSettingPopup(false)} />
      <AIChatBox />
    </div>

  );
}

// Component App bọc toàn bộ nội dung trong AuthProvider
function App() {
  return (
    <AuthProvider> 
      <AppContent />
      <ToastContainer position="top-right" autoClose={3000} />
    </AuthProvider>
  );
}

export default App;