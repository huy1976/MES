import React, { useState, useEffect, useCallback, useMemo } from 'react';
import './Dashboard.css';
import {
  Chart as ChartJS,
  ArcElement, Tooltip, Legend, CategoryScale, LinearScale, BarElement, Title,
} from 'chart.js';
import { Doughnut, Bar } from 'react-chartjs-2';
import ChartDataLabels from 'chartjs-plugin-datalabels'; 
import { GetMOReport } from '../../api/Dashboard'; 

ChartJS.register(ArcElement, Tooltip, Legend, CategoryScale, LinearScale, BarElement, Title, ChartDataLabels);

// --- COMPONENT SKELETON LOADING ---
const DashboardSkeleton = () => {
  return (
    <div className="dashboard-root">
      <div className="skeleton-box" style={{ width: '200px', height: '32px', margin: '10px auto 15px', display: 'block' }}></div>
      <div className="time-filter">
          <div className="skeleton-box" style={{ width: '180px', height: '30px', borderRadius:'4px' }}></div>
      </div>
      <div className="top-row">
        {[1, 2, 3].map((item) => (
          <div className="card small" key={item}>
            <div className="skeleton-box sk-header"></div> 
            <div className="card-body">
              <div className="kpis" style={{width: '100%', marginBottom: '15px'}}>
                  <div className="kpi"><div className="skeleton-box sk-text" style={{height:'20px', width:'50%'}}></div></div>
                  <div className="kpi"><div className="skeleton-box sk-text" style={{height:'20px', width:'50%'}}></div></div>
              </div>
              <div className="skeleton-box sk-circle"></div> 
            </div>
          </div>
        ))}
        <div className="card large-oee">
          <div className="skeleton-box sk-header"></div>
          <div className="card-body oee-body">
              <div className="skeleton-box sk-circle" style={{ width: '140px', height: '140px' }}></div>
              <div className="oee-legend" style={{width: '80%', display:'flex', gap:'10px', justifyContent:'center'}}>
                  <div className="skeleton-box" style={{width:'60px', height:'20px'}}></div>
                  <div className="skeleton-box" style={{width:'60px', height:'20px'}}></div>
                  <div className="skeleton-box" style={{width:'60px', height:'20px'}}></div>
              </div>
          </div>
        </div>
      </div>
      <div className="middle-row" style={{ marginTop: '10px', alignItems:'center' }}>
          <div className="skeleton-box" style={{ width: '300px', height: '30px', marginRight:'auto' }}></div>
          <div className="skeleton-box" style={{ width: '150px', height: '30px' }}></div>
      </div>
      <div className="bottom-row">
        {[1, 2].map((item) => (
          <div className="card wide" key={item}>
            <div className="skeleton-box sk-header"></div>
            <div className="card-body">
              <div className="skeleton-box sk-chart-bar"></div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

// --- MAIN DASHBOARD COMPONENT ---
export default function Dashboard() {
  const [oeePeriod, setOeePeriod] = useState('ngay');
  const [incidentPeriod, setIncidentPeriod] = useState('ngay');
  const [dashboardData, setDashboardData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [machineChartData, setMachineChartData] = useState(null); 
  const [incidentChartData, setIncidentChartData] = useState(null);
  const [machineFilter, setMachineFilter] = useState('enamelMachines');
  const [showMaterialModal, setShowMaterialModal] = useState(false);
  // --- 1. OPTIONS CHO CHART ---
  const donutOptionsWithLabels = {
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false }, 
      tooltip: { enabled: true },
      datalabels: {
        color: '#fff',
        font: { weight: 'bold', size: 10 },
        formatter: (value, ctx) => {
          let sum = 0;
          let dataArr = ctx.chart.data.datasets[0].data;
          dataArr.map(data => { sum += data; });
          
          if (sum === 0) return "0%";
          let percentageVal = (value * 100 / sum);
          
          if (percentageVal < 3) return "";
          return percentageVal.toFixed(0) + "%";
        },
        display: true 
      }
    }
  };

    // --- BỔ SUNG: Hàm tính màu cho OEE ---
  const getOeeColor = (oeeRate) => {
    if (oeeRate >= 85.0) {
      return '#27ae60'; // Xanh lá
    } else if (oeeRate >= 65.0) {
      return '#f39c12'; // Vàng/Cam
    } else {
      return '#c0392b'; // Đỏ
    }
  };
// --- BỔ SUNG: Hàm lấy Text Đánh giá OEE ---
  const getOeeRatingText = (oeeRate) => {
    if (oeeRate >= 85.0) {
      return 'Rất Tốt';
    } else if (oeeRate >= 65.0) {
      return 'Chấp nhận được';
    } else {
      return 'Cần cải thiện';
    }
  };
  
  const oeeLegendData = [
    { color: '#27ae60', label: 'Rất Tốt (≥ 85%)' },
    { color: '#f39c12', label: 'Chấp nhận (65% - 85%)' },
    { color: '#c0392b', label: 'Cần cải thiện (< 65%)' }
  ];

  // --- CẬP NHẬT: Thêm barThickness để cột biểu đồ bớt "gầy" ---
  const barOptions = {
    plugins: { 
        legend: { display: false },
        datalabels: { display: false } 
    },
    maintainAspectRatio: false,
    scales: {
      y: { beginAtZero: true, ticks: { font: { size: 10 } } }, 
      x: { ticks: { font: { size: 10 } } }
    },
    datasets: { 
        bar: { 
            barThickness: 40,    // Độ dày cột cố định
            maxBarThickness: 60  // Độ dày tối đa
        } 
    }
  };

  // --- 2. XỬ LÝ DỮ LIỆU ---
  const transformDashboardData = (rawData) => {
    if (!rawData) return {};

    // --- MO Report ---
    const totalMOs = rawData.moReport.totalMOs;
    const completedMOs = rawData.moReport.completedMOs;
    const pendingMOs = totalMOs - completedMOs;

    const prodData = {
      labels: ['Hoàn thành', 'Chưa hoàn thành'],
      datasets: [{
        data: [completedMOs, pendingMOs],
        backgroundColor: ['#0b5f8a', '#ef6c2f'],
        hoverOffset: 6,
      }],
      kpis: {
        total: totalMOs,
        completed: completedMOs,
        rate: `${rawData.moReport.completionRate}%`
      }
    };

    // --- Material Items Report ---
    const totalFinished = rawData.materialReport.totalFinishedMaterials;
    const cuFinished = rawData.materialReport.cuFinishedMaterials;
    const alFinished = rawData.materialReport.alFinishedMaterials;

    const itemsData = {
      labels: ['Dây đồng', 'Dây nhôm'],
      datasets: [{ 
        data: [cuFinished, alFinished], 
        backgroundColor: ['#8fd77d', '#333'], 
        hoverOffset: 6 
      }],
      kpis: {
        total: totalFinished,
        dayao: cuFinished,
        daynhom: alFinished
      }
    };

    // --- Raw Material Usage Report ---
    const rawList = rawData.rawMaterialReport.rawMaterialsInMOs || [];
    const totalRawMaterial = rawData.rawMaterialReport.totalRawMaterial;

    const sortedMaterials = [...rawList].sort((a, b) => b.quantity - a.quantity);
    
    const MAX_CHART_ITEMS = 6; 

    let chartLabels = [];
    let chartDataValues = [];
    let displayList = sortedMaterials; 

    if (sortedMaterials.length > MAX_CHART_ITEMS) {
        const topMaterials = sortedMaterials.slice(0, MAX_CHART_ITEMS - 1); 
        const otherMaterials = sortedMaterials.slice(MAX_CHART_ITEMS - 1);
        const otherQuantity = otherMaterials.reduce((sum, m) => sum + m.quantity, 0);

        chartLabels = topMaterials.map(m => m.materialName);
        chartDataValues = topMaterials.map(m => m.quantity);

        chartLabels.push('Khác');
        chartDataValues.push(otherQuantity);
        
        displayList = sortedMaterials; 

    } else {
        chartLabels = sortedMaterials.map(m => m.materialName);
        chartDataValues = sortedMaterials.map(m => m.quantity);
    }


    const extendedColors = [
        '#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#AF19FF', 
        '#ef6c2f', '#ff6384', '#36a2eb', '#cc65fe', '#ffce56', 
        '#4bc0c0', '#9966ff'
    ];

    const materialData = {
      labels: chartLabels,
      datasets: [{ 
        data: chartDataValues, 
        backgroundColor: extendedColors.slice(0, chartLabels.length), 
        hoverOffset: 6 
      }],
      kpis: {
        totalRawMaterial: totalRawMaterial,
        fullList: displayList 
      }
    };

    // --- OEE Report ---
    const { availability, performance, quality, oee } = rawData.oeeReport;
    const oeeData = {
      labels: ['Tỷ lệ sẵn sàng (A)', 'Tỷ lệ hiệu suất (P)', 'Tỷ lệ chất lượng (Q)'], 
      datasets: [{
        data: [availability, performance, quality],
        backgroundColor: ['#4aa3ff', '#2b6cd6', '#9aa7b0']
      }],
      overallOEE: oee 
    };
    
    return { prodData, itemsData, materialData, oeeData };
  };
  
  const getMachineChartData = (machineReport, filter) => {
    if (!machineReport) return null;
    const machineTypeData = machineReport[filter] || [];
    let labelKey = '';
    let labelText = 'Sản lượng';

    if (filter === 'drawMachines') {
        labelKey = 'totalDraws';
        labelText = 'Sản lượng Máy Kéo';
    } else if (filter === 'enamelMachines') {
        labelKey = 'totalEnamels';
        labelText = 'Sản lượng Máy Tráng Men';
    } else if (filter === 'drawEnamelMachines') { 
        labelKey = 'totalDrawEnamels'; 
        labelText = 'Sản lượng Máy Kéo Tráng Men';
    } else return null;

    return {
      labels: machineTypeData.map(m => m.machineId),
      datasets: [{ 
        label: labelText, 
        data: machineTypeData.map(m => m[labelKey]), 
        backgroundColor: '#4da6ff' 
      }]
    };
  };

  // --- 3. API & EFFECTS ---
  const fetchDashboardData = useCallback(async (periodString) => {
    setLoading(true);
    let periodValue = 0;
    if (periodString === 'ngay') periodValue = 0;
    else if (periodString === 'tuan') periodValue = 1;
    else if (periodString === 'thang') periodValue = 2;
    else if (periodString === 'tatca') periodValue = 4;


    try {
      const response = await GetMOReport(periodValue); 
      setDashboardData(response);

      const incidentChart = {
        labels: response.errorReport.errorsInMOs.map(e => e.errorType),
        datasets: [{ 
          label: 'Số lần', 
          data: response.errorReport.errorsInMOs.map(e => e.errorCount), 
          backgroundColor: '#f48b8b' 
        }]
      };
      setIncidentChartData(incidentChart);

    } catch (error) {
      console.error(`Lỗi khi tải dữ liệu dashboard:`, error);
      setDashboardData(null); 
    } finally {
      setLoading(false);
    }
  }, []); 

  useEffect(() => {
    fetchDashboardData(oeePeriod);
  }, [oeePeriod, fetchDashboardData]);
  
  useEffect(() => {
    if (dashboardData && dashboardData.machineReport) {
        const data = getMachineChartData(dashboardData.machineReport, machineFilter);
        setMachineChartData(data);
    }
  }, [dashboardData, machineFilter]);
  
  const isActive = (currentPeriod, p) => currentPeriod === p ? 'active' : '';
  const isMachineActive = (currentFilter, filter) => currentFilter === filter ? 'active' : '';
  
  const { prodData, itemsData, materialData, oeeData } = useMemo(() => {
    return transformDashboardData(dashboardData);
  }, [dashboardData]);


  // --- 4. RENDER VỚI SKELETON LOADING ---
  if (loading) {
    return <DashboardSkeleton />;
  }

  // --- 5. RENDER CHÍNH ---
  return (
    <div className="dashboard-root">
      <h1 className="title">Tổng quan</h1>

      <div className="time-filter">
        <div className="periods">
          <button className={`period ${isActive(oeePeriod, 'ngay')}`} onClick={() => setOeePeriod('ngay')}>Ngày</button>
          <button className={`period ${isActive(oeePeriod, 'tuan')}`} onClick={() => setOeePeriod('tuan')}>Tuần</button>
          <button className={`period ${isActive(oeePeriod, 'thang')}`} onClick={() => setOeePeriod('thang')}>Tháng</button>
          <button className={`period ${isActive(oeePeriod, 'tatca')}`} onClick={() => setOeePeriod('tatca')}>Tất cả</button>
        </div>
      </div>

      <div className="top-row">
        {/* 1. Yêu cầu sản xuất */}
        <div className="card small">
          <div className="card-header">Số lượng Yêu cầu sản xuất</div>
          <div className="card-body">
            {prodData && prodData.kpis ? (
              <>
                <div className="kpis">
                  <div className="kpi"><div className="kpi-value">{prodData.kpis.total}</div><div className="kpi-label">Tổng</div></div>
                  <div className="kpi"><div className="kpi-value">{prodData.kpis.completed}</div><div className="kpi-label">Hoàn thành</div></div>
                  <div className="kpi"><div className="kpi-value">{prodData.kpis.rate}</div><div className="kpi-label">Tỷ lệ</div></div>
                </div>
                <div className="chart-donut">
                  <Doughnut data={prodData} options={donutOptionsWithLabels} />
                </div>
              </>
            ) : <div className="loading-state">Chưa có dữ liệu</div>}
          </div>
        </div>

        {/* 2. Mặt hàng sản xuất */}
        <div className="card small">
          <div className="card-header">Số lượng mặt hàng sản xuất</div>
          <div className="card-body">
            {itemsData && itemsData.kpis ? (
              <>
                <div className="kpis">
                  <div className="kpi"><div className="kpi-value">{itemsData.kpis.total}</div><div className="kpi-label">Tổng</div></div>
                  <div className="kpi"><div className="kpi-value">{itemsData.kpis.dayao}</div><div className="kpi-label">Dây đồng</div></div>
                  <div className="kpi"><div className="kpi-value">{itemsData.kpis.daynhom}</div><div className="kpi-label">Dây nhôm</div></div>
                </div>
                <div className="chart-donut">
                  <Doughnut data={itemsData} options={donutOptionsWithLabels} />
                </div>
              </>
            ) : <div className="loading-state">Chưa có dữ liệu</div>}
          </div>
        </div>

        {/* 3. Lượng nguyên vật liệu (FULL LIST & CHART) */}
        <div className="card medium">
          <div className="card-header">Lượng nguyên vật liệu tiêu thụ</div>
          <div className="card-body" style={{ display: 'flex', flexDirection: 'row', gap: '10px', alignItems: 'center' }}>
            {materialData && materialData.kpis ? (
              <>
                {/* List bên trái */}
                <div className="material-list-container" style={{ flex: 1, height: '100%', paddingRight: '15px' }}>
                  <div style={{ fontSize: '20px', fontWeight: 'bold', marginBottom: '8px', borderBottom: '1px solid #eee', paddingBottom: '4px' }}>
                    Tổng nguyên vật liệu đã tiêu thụ: {materialData.kpis.totalRawMaterial}
                  </div>
                  
                  {/* List Top 8 */}
                  {materialData.kpis.fullList.slice(0, 8).map((item, index) => (
                    <div key={index} style={{ display: 'flex', justifyContent: 'space-between', fontSize: '15px', marginBottom: '6px' }}>
                      <span title={item.materialId} style={{ whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis', maxWidth: '75%' }}>
                        {index + 1}. {item.materialId}
                      </span>
                      <span style={{ fontWeight: 'bold' }}>{item.quantity} {item.unit}</span>
                    </div>
                  ))}

                  {/* MỚI: Nút bấm xem thêm */}
                  {materialData.kpis.fullList.length > 8 && (
                    <div 
                      className="see-more-link"
                      onClick={() => setShowMaterialModal(true)} // Bật Modal
                      title="Nhấn để xem danh sách đầy đủ"
                    >
                      Và {materialData.kpis.fullList.length - 8} mục khác... (Xem tất cả)
                    </div>
                  )}
                </div>

                {/* Chart bên phải */}
                <div className="chart-donut" style={{ flex: 1 }}>
                  <Doughnut data={materialData} options={donutOptionsWithLabels} />
                </div>
              </>
            ) : <div className="loading-state">Chưa có dữ liệu</div>}
          </div>
        </div>

        {/* 4. OEE tổng - LARGE OEE */}

      </div>
      
      {/* Bottom Rows */}
      <div className="middle-row">
        <div className="machine-filters">
          <button className={`filter ${isMachineActive(machineFilter, 'drawMachines')}`} onClick={() => setMachineFilter('drawMachines')}>Máy kéo</button>
          <button className={`filter ${isMachineActive(machineFilter, 'enamelMachines')}`} onClick={() => setMachineFilter('enamelMachines')}>Máy tráng men</button>
          <button className={`filter ${isMachineActive(machineFilter, 'drawEnamelMachines')}`} onClick={() => setMachineFilter('drawEnamelMachines')}>Máy kéo tráng men</button>
        </div>
        <div className="periods small">
        </div>
      </div>

      <div className="bottom-row">
        <div className="card wide">
          <div className="card-header">Thống kê các sự cố phát sinh</div>
          <div className="card-body chart">
            {incidentChartData ? (
              <div className="chart-bar">
                <Bar data={incidentChartData} options={barOptions} />
              </div>
            ) : <div className="loading-state">Chưa có dữ liệu</div>}
          </div>
        </div>
        <div className="card wide">
          <div className="card-header">Thống kê số lượng sản xuất của loại máy</div>
          <div className="card-body chart">
            {machineChartData ? (
              <div className="chart-bar">
                <Bar data={machineChartData} options={barOptions} />
              </div>
            ) : <div className="loading-state">Chưa có dữ liệu</div>}
          </div>
        </div>
      </div>
      {/* --- MỚI: MODAL COMPONENT (Đặt ở cuối cùng, trước thẻ đóng div dashboard-root) --- */}
      {showMaterialModal && materialData && (
        <div className="modal-overlay" onClick={() => setShowMaterialModal(false)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}> 
            {/* stopPropagation để click vào modal không bị đóng */}
            
            <div className="modal-header">
              <span>Chi tiết Nguyên vật liệu tiêu thụ</span>
              <button className="close-btn" onClick={() => setShowMaterialModal(false)}>✕</button>
            </div>

            <div className="modal-body">
              {materialData.kpis.fullList.map((item, index) => (
                <div key={index} className="modal-list-item">
                  <span className="modal-item-name">
                    {index + 1}. {item.materialId}
                  </span>
                  <span className="modal-item-qty">{item.quantity} {item.unit}</span>
                </div>
              ))}
            </div>
            
          </div>
        </div>
      )}
      
    </div>
  );
}