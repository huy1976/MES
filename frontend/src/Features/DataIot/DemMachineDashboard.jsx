import React, { useState, useEffect, useMemo, useRef } from 'react';
import axios from 'axios';
import { 
  FaServer, FaEye, FaSyncAlt, FaChartLine, FaExclamationTriangle, 
  FaSearch, FaCalendarAlt, FaTachometerAlt, FaSearchPlus, FaSearchMinus, FaSpinner 
} from 'react-icons/fa';
import './../OrderRequire/OrderRequire.css';
import './DataIotDashboard.css';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import DateInput from '../../common/components/DateInput/DateInput';

const METRIC_VI_MAP = {
  'ActiveTime': 'T.gian hoạt động (phút)',
  'ActualLength': 'Chiều dài thực tế (m)',
  'Angle': 'Góc lệch (°)',
  'ErrorCount': 'Số lần lỗi',
  'FeedbackSpeed': 'Tốc độ thực tế (m/p)',
  'MachineStatus': 'Trạng thái máy',
  'Positon': 'Vị trí', 
  'Position': 'Vị trí',
  'SetLength': 'Chiều dài C.đặt (m)',
  'SetSpeed': 'Tốc độ C.đặt (m/p)',
  'StartCount': 'Số lần chạy',
  'StopCount': 'Số lần dừng',
};

const formatDisplayDate = (date) => `${pad(date.getDate())}/${pad(date.getMonth() + 1)}/${date.getFullYear()} ${pad(date.getHours())}:${pad(date.getMinutes())}`;
const getCleanMetric = (rawName) => {
  if (!rawName) return 'Unknown';
  const cleanName = rawName.includes('/') ? rawName.split('/')[1] : rawName;
  return cleanName;
};

const pad = (n) => n.toString().padStart(2, '0');
const translateMetric = (cleanName) => METRIC_VI_MAP[cleanName] || cleanName;
const formatToLocalInput = (date) => `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
const formatToAPIString = (date) => formatToLocalInput(date) + ':00';

// === API GỌI TỪ BACKEND ===
const fetchDemMachines = async () => {
  try {
    const response = await axios.get('/iot-api/FineWireDrawingMachines');
    return response.data; 
  } catch (error) {
    console.error("Lỗi khi lấy danh sách máy Dem:", error);
    return []; 
  }
};

const fetchMachineDetail = async (machineId, startDt, endDt) => {
  try {
    const totalDays = (endDt - startDt) / (1000 * 60 * 60 * 24);
    let allData = [];

    if (totalDays > 35) {
      let currentStart = new Date(startDt);
      while (currentStart < endDt) {
        let currentEnd = new Date(currentStart);
        currentEnd.setMonth(currentEnd.getMonth() + 1); 
        if (currentEnd > endDt) currentEnd = endDt;

        const url = `/iot-api/FineWireDrawingMachineRecords?MachineId=${machineId}&StartTime=${formatToAPIString(currentStart)}&EndTime=${formatToAPIString(currentEnd)}`;
        const response = await axios.get(url, { timeout: 30000 }); 
        if (response.data && response.data.length > 0) {
          allData = allData.concat(response.data);
        }
        currentStart = currentEnd;
      }
    } else {
      const url = `/iot-api/FineWireDrawingMachineRecords?MachineId=${machineId}&StartTime=${formatToAPIString(startDt)}&EndTime=${formatToAPIString(endDt)}`;
      const response = await axios.get(url, { timeout: 30000 });
      allData = response.data;
    }
    return allData;
  } catch (error) {
    throw new Error('Không thể lấy đủ dữ liệu do khối lượng quá lớn. Thử khoảng thời gian ngắn hơn.');
  }
};

// HÀM XỬ LÝ DỮ LIỆU CHUNG (Dùng cho cả lần đầu tải và khi Zoom in)
const processMachineData = (rawData) => {
  if (!rawData || rawData.length === 0) return { allPoints: [], latest: {}, oee: {A:0, P:0, Q:0, OEE:0} };

  const groupedData = {};
  const latest = {};

  rawData.forEach(item => {
    const cleanId = getCleanMetric(item.metricId || item.name);
    const itemDate = new Date(item.timestamp);
    if (itemDate.getFullYear() < 2000) return;

    const timeNumeric = itemDate.getTime(); 
    latest[cleanId] = item.value;

    if (!groupedData[timeNumeric]) groupedData[timeNumeric] = { timeNumeric: timeNumeric };
    groupedData[timeNumeric][cleanId] = item.value;
  });

  const allProcessedPoints = Object.values(groupedData).sort((a, b) => a.timeNumeric - b.timeNumeric);

  let totalA = 0, totalP = 0, totalQ = 0, validPCount = 0;
  allProcessedPoints.forEach(point => {
    totalA += (point.MachineStatus === 1 || point.MachineStatus === '1') ? 1.0 : 0.0;
    if (point.SetSpeed > 0) {
      totalP += Math.min(1, (point.FeedbackSpeed || 0) / point.SetSpeed);
      validPCount++;
    }
    totalQ += Math.max(0, 1 - ((point.ErrorCount || 0) * 0.02));
  });

  const n = allProcessedPoints.length || 1;
  const avgA = totalA / n;
  const avgP = validPCount > 0 ? (totalP / validPCount) : 0.85;
  const avgQ = totalQ / n;
  
  const oee = {
    A: (avgA * 100).toFixed(1),
    P: (avgP * 100).toFixed(1),
    Q: (avgQ * 100).toFixed(1),
    OEE: (avgA * avgP * avgQ * 100).toFixed(1)
  };

  return { allPoints: allProcessedPoints, latest, oee };
};


// === COMPONENT CHI TIẾT MÁY (CÓ BIỂU ĐỒ) ===
const DemMachineDetail = ({ machine, onBack }) => {
  const [customStart, setCustomStart] = useState(() => {
    const d = new Date(); d.setDate(d.getDate() - 1); return d; 
  });
  const [customEnd, setCustomEnd] = useState(new Date());
  const [activeQuickFilter, setActiveQuickFilter] = useState('day');
  const [queryParams, setQueryParams] = useState({ start: customStart, end: customEnd });

  const [fullData, setFullData] = useState([]);
  
  // STATE MỚI: Lưu trữ dữ liệu tải thêm khi zoom
  const [zoomedData, setZoomedData] = useState(null); 
  const [isZoomFetching, setIsZoomFetching] = useState(false);

  const [zoomDomain, setZoomDomain] = useState(['dataMin', 'dataMax']);
  const chartRef = useRef(null);

  const [latestValues, setLatestValues] = useState({});
  const [selectedMetric, setSelectedMetric] = useState('FeedbackSpeed');
  const [loading, setLoading] = useState(true);
  const [errorMsg, setErrorMsg] = useState(null);
  const [oeeData, setOeeData] = useState({ A: 0, P: 0, Q: 0, OEE: 0 });

  const handleQuickFilter = (range) => {
    const end = new Date();
    const start = new Date();
    if (range === 'day') start.setDate(start.getDate() - 1);
    else if (range === 'week') start.setDate(start.getDate() - 7);
    else if (range === 'month') start.setMonth(start.getMonth() - 1);
    else if (range === 'year') start.setFullYear(start.getFullYear() - 1);

    setCustomStart(start);
    setCustomEnd(end);
    setActiveQuickFilter(range);
    setQueryParams({ start, end }); 
  };

  const handleCustomSearch = () => {
    setActiveQuickFilter('custom');
    setQueryParams({ start: customStart, end: customEnd });
  };

  // 1. TẢI DỮ LIỆU LẦN ĐẦU (KHOẢNG THỜI GIAN LỚN)
  useEffect(() => {
    const loadMainData = async () => {
      setLoading(true);
      setErrorMsg(null);
      try {
        const rawData = await fetchMachineDetail(machine.id || machine.machineId, queryParams.start, queryParams.end);
        const { allPoints, latest, oee } = processMachineData(rawData);
        
        setFullData(allPoints);
        setZoomDomain(['dataMin', 'dataMax']); 
        setZoomedData(null); // Clear dữ liệu zoom cũ
        setLatestValues(latest);
        setOeeData(oee);
        if (!latest[selectedMetric]) setSelectedMetric(Object.keys(latest)[0] || 'FeedbackSpeed');
        
      } catch (error) {
        setErrorMsg(error.message);
      } finally {
        setLoading(false);
      }
    };
    loadMainData();
  }, [machine, queryParams]); 

  // 2. DEBOUNCE FETCH KHI ZOOM (Chỉ gọi API lấy dữ liệu chi tiết sau khi ngừng cuộn chuột)
  useEffect(() => {
    if (zoomDomain[0] === 'dataMin' || zoomDomain[1] === 'dataMax') {
        setZoomedData(null); // Nếu ở mức zoom gốc thì xoá data zoom chi tiết
        return; 
    }

    // Thiết lập timeout (Debounce 800ms)
    const fetchTimer = setTimeout(async () => {
      setIsZoomFetching(true);
      try {
        const startZoomDate = new Date(zoomDomain[0]);
        const endZoomDate = new Date(zoomDomain[1]);
        
        // Gọi API lấy dữ liệu cho khoảng thời gian NHỎ đang được zoom
        const rawZoomData = await fetchMachineDetail(machine.id || machine.machineId, startZoomDate, endZoomDate);
        const { allPoints } = processMachineData(rawZoomData);
        
        setZoomedData(allPoints);
      } catch (error) {
        console.error("Lỗi khi tải dữ liệu zoom chi tiết:", error);
      } finally {
        setIsZoomFetching(false);
      }
    }, 800); // Đợi 800ms sau lần cuộn chuột cuối cùng mới gọi API

    // Xóa timer nếu người dùng tiếp tục cuộn chuột chưa tới 800ms
    return () => clearTimeout(fetchTimer);
  }, [zoomDomain, machine.id, machine.machineId]);


  // 3. XỬ LÝ DỮ LIỆU HIỂN THỊ TRÊN BIỂU ĐỒ
  const visibleChartData = useMemo(() => {
    // Nếu có dữ liệu zoom chi tiết -> Ưu tiên dùng dữ liệu zoom
    let sourceData = (zoomedData && zoomedData.length > 0) ? zoomedData : fullData;
    
    if (!sourceData || sourceData.length === 0) return [];

    let currentMin = zoomDomain[0] === 'dataMin' ? fullData[0]?.timeNumeric : zoomDomain[0];
    let currentMax = zoomDomain[1] === 'dataMax' ? fullData[fullData.length - 1]?.timeNumeric : zoomDomain[1];

    if (!currentMin || !currentMax) return [];

    const buffer = (currentMax - currentMin) * 0.05;
    let visibleData = sourceData.filter(d => d.timeNumeric >= (currentMin - buffer) && d.timeNumeric <= (currentMax + buffer));

    // Nén dữ liệu để UI không bị đơ nếu trong vùng xem vẫn quá nhiều điểm
    if (visibleData.length > 300) {
      const step = Math.ceil(visibleData.length / 300);
      return visibleData.filter((_, index) => index % step === 0);
    }
    
    return visibleData; 
  }, [fullData, zoomedData, zoomDomain]);

  // XỬ LÝ SỰ KIỆN LĂN CHUỘT
  useEffect(() => {
    const element = chartRef.current;
    if (!element) return;

    const handleWheel = (e) => {
      if (fullData.length === 0) return;
      e.preventDefault(); 

      setZoomDomain(prev => {
        let currentMin = prev[0] === 'dataMin' ? fullData[0].timeNumeric : prev[0];
        let currentMax = prev[1] === 'dataMax' ? fullData[fullData.length - 1].timeNumeric : prev[1];

        const range = currentMax - currentMin;
        const zoomAmount = range * 0.15; 

        let newMin, newMax;
        if (e.deltaY < 0) { 
          newMin = currentMin + zoomAmount;
          newMax = currentMax - zoomAmount;
        } else { 
          newMin = currentMin - zoomAmount;
          newMax = currentMax + zoomAmount;
        }

        if (newMax - newMin < 60000) return prev; // Không zoom sâu hơn 1 phút

        const absoluteMin = fullData[0].timeNumeric;
        const absoluteMax = fullData[fullData.length - 1].timeNumeric;

        newMin = Math.max(absoluteMin, newMin);
        newMax = Math.min(absoluteMax, newMax);

        if (newMin <= absoluteMin && newMax >= absoluteMax) {
          return ['dataMin', 'dataMax'];
        }
        return [newMin, newMax];
      });
    };

    element.addEventListener('wheel', handleWheel, { passive: false });
    return () => element.removeEventListener('wheel', handleWheel);
  }, [fullData]);

  const handleResetZoom = () => {
    setZoomDomain(['dataMin', 'dataMax']);
    setZoomedData(null);
  };

  const formatXAxisTick = (timeNumeric) => {
    const d = new Date(timeNumeric);
    const diffDays = (queryParams.end - queryParams.start) / 86400000;
    return diffDays <= 2 ? `${pad(d.getHours())}:${pad(d.getMinutes())}` : 
           diffDays <= 35 ? `${pad(d.getDate())}/${pad(d.getMonth() + 1)}` : 
           `${pad(d.getDate())}/${pad(d.getMonth() + 1)}/${d.getFullYear()}`;
  };

  const CustomTooltip = ({ active, payload, label }) => {
    if (active && payload && payload.length) {
      const d = new Date(label);
      return (
        <div style={{ background: '#fff', padding: '12px', border: '1px solid #ccc', borderRadius: '8px', boxShadow: '0 4px 10px rgba(0,0,0,0.1)' }}>
          <p style={{ margin: '0 0 8px 0', fontWeight: 'bold', color: '#2c3e50', borderBottom: '1px solid #eee', paddingBottom: '4px' }}>
            <FaCalendarAlt style={{marginRight: '6px', color: '#7f8c8d'}}/> {formatDisplayDate(d)}
          </p>
          {payload.map((entry, index) => (
            <p key={index} style={{ margin: 0, color: entry.color, fontWeight: 500, fontSize: '14px' }}>
              {translateMetric(entry.name)} : <strong style={{fontSize: '16px'}}>{Number.isInteger(entry.value) ? entry.value : entry.value.toFixed(2)}</strong>
            </p>
          ))}
        </div>
      );
    }
    return null;
  };

  const getOeeColor = (val) => val >= 85 ? '#2ecc71' : val >= 65 ? '#f39c12' : '#e74c3c';

  return (
    <div className="order-require-container">
      <div className="detail-nav" style={{ marginBottom: '16px' }}>
        <button onClick={onBack} className="back-btn" disabled={loading}>← Quay lại danh sách</button>
      </div>

      <h2 className="title" style={{marginBottom: '20px'}}>Phân tích: {machine.name || machine.id}</h2>

      <div className="filter-panel active" style={{ position: 'relative', transform: 'none', width: '100%', marginBottom: '24px', display: 'flex', flexWrap: 'wrap', gap: '20px', alignItems: 'flex-end', padding: '16px', background: '#fff' }}>
        <div style={{ display: 'flex', gap: '12px', flexWrap: 'wrap' }}>
          <div>
            <label style={{display: 'block', fontSize: '12px', color: '#7f8c8d', marginBottom: '4px', fontWeight: 'bold'}}>Từ ngày:</label>
            <DateInput selectedDate={customStart} onChange={(date) => setCustomStart(date)} />
          </div>
          <div>
            <label style={{display: 'block', fontSize: '12px', color: '#7f8c8d', marginBottom: '4px', fontWeight: 'bold'}}>Đến ngày:</label>
            <DateInput selectedDate={customEnd} onChange={(date) => setCustomEnd(date)} />
          </div>
          <button onClick={handleCustomSearch} disabled={loading} className="action-btn reload-btn" style={{ height: '38px', marginTop: 'auto' }}>
            <FaSearch /> Tra cứu
          </button>
        </div>

        <div style={{ width: '1px', background: '#ecf0f1', height: '40px', margin: '0 10px' }} className="divider-hide-mobile"></div>

        <div className="time-filters" style={{ display: 'flex', gap: '8px', flexWrap: 'wrap', marginTop: 'auto' }}>
          {['day', 'week', 'month', 'year'].map(range => (
            <button key={range} disabled={loading} onClick={() => handleQuickFilter(range)}
              className={activeQuickFilter === range ? 'quick-filter-active' : ''}
              style={{
                padding: '8px 16px', borderRadius: '6px', border: activeQuickFilter === range ? 'none' : '1px solid #bdc3c7',
                backgroundColor: activeQuickFilter === range ? '#3498db' : '#f8f9fa',
                color: activeQuickFilter === range ? '#fff' : '#2c3e50',
                cursor: loading ? 'wait' : 'pointer', fontWeight: 600, fontSize: '13px'
              }}>
              {range === 'day' ? '24 Giờ qua' : range === 'week' ? '7 Ngày qua' : range === 'month' ? '1 Tháng qua' : '1 Năm qua'}
            </button>
          ))}
        </div>
      </div>

      {errorMsg ? (
        <div className="error-message" style={{ padding: '20px', background: '#ffeaa7', borderRadius: '8px' }}>{errorMsg}</div>
      ) : loading ? (
        <div className="loading-spinner"><div className="spinner"></div><span>Đang tính toán dữ liệu tổng quan...</span></div>
      ) : (
        <>
          <div style={{ marginBottom: '24px', background: '#fff', padding: '20px', borderRadius: '12px', border: '1px solid #eee' }}>
            <h3 style={{ margin: '0 0 16px 0', fontSize: '15px', color: '#2c3e50', display: 'flex', alignItems: 'center', gap: '8px' }}>
              <FaTachometerAlt color="#3498db" /> Hiệu suất thiết bị tổng thể (OEE)
            </h3>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: '16px' }}>
              {['OEE', 'A', 'P', 'Q'].map(k => (
                <div key={k} style={{ padding: '16px', borderRadius: '8px', background: '#f8f9fa', borderLeft: `4px solid ${getOeeColor(oeeData[k])}` }}>
                  <div style={{ fontSize: '24px', color: '#7f8c8d', fontWeight: 'bold' }}>{k === 'OEE' ? 'OEE TỔNG' : k}</div>
                  <div style={{ fontSize: '24px', fontWeight: 'bold', color: getOeeColor(oeeData[k]) }}>{oeeData[k]}%</div>
                </div>
              ))}
            </div>
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(180px, 1fr))', gap: '12px', marginBottom: '24px' }}>
            {Object.entries(latestValues).map(([key, value]) => (
              <div key={key} onClick={() => setSelectedMetric(key)}
                style={{ 
                  padding: '16px', background: '#fff', borderRadius: '8px', cursor: 'pointer',
                  border: selectedMetric === key ? '2px solid #3498db' : '1px solid #ecf0f1',
                  boxShadow: selectedMetric === key ? '0 4px 12px rgba(52, 152, 219, 0.2)' : 'none'
                }}>
                <div style={{ fontSize: '11px', color: '#7f8c8d', marginBottom: '4px', fontWeight: 600 }}>{translateMetric(key)}</div>
                <div style={{ fontSize: '18px', fontWeight: 'bold', color: selectedMetric === key ? '#3498db' : '#2c3e50' }}>
                  {typeof value === 'number' ? value.toFixed(2) : value}
                </div>
              </div>
            ))}
          </div>

          <div className="machine-card" style={{ padding: '24px 20px', background: '#fff', position: 'relative' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '16px' }}>
              <h3 style={{ margin: 0, fontSize: '15px' }}>
                <FaChartLine color="#3498db" style={{marginRight: '6px'}}/> 
                Biểu đồ: {translateMetric(selectedMetric)}
                <span style={{ fontSize: '12px', color: '#7f8c8d', fontWeight: 'normal', marginLeft: '12px' }}>
                  (Cuộn chuột trên biểu đồ để Thu/Phóng)
                </span>
              </h3>
              
              {zoomDomain[0] !== 'dataMin' && (
                <button 
                  onClick={handleResetZoom}
                  style={{ padding: '6px 12px', background: '#ecf0f1', border: 'none', borderRadius: '4px', cursor: 'pointer', fontSize: '12px', display: 'flex', alignItems: 'center', gap: '6px', color: '#2c3e50', fontWeight: 'bold' }}
                >
                  <FaSearchMinus /> Khôi phục thu phóng
                </button>
              )}
            </div>

            {/* OVERLAY LOADING KHI ĐANG TẢI DỮ LIỆU ZOOM CHI TIẾT */}
            {isZoomFetching && (
              <div style={{
                position: 'absolute', top: '60px', left: 0, right: 0, bottom: 0, 
                backgroundColor: 'rgba(255,255,255,0.7)', zIndex: 10, 
                display: 'flex', justifyContent: 'center', alignItems: 'center',
                flexDirection: 'column', color: '#3498db', fontWeight: 'bold'
              }}>
                <div className="spinner" style={{marginBottom: '10px', borderColor: '#3498db', borderTopColor: 'transparent'}}></div>
                Đang tải dữ liệu chi tiết khu vực này...
              </div>
            )}

            <div ref={chartRef} style={{ width: '100%', height: '400px' }}>
              <ResponsiveContainer width="100%" height="100%">
                <LineChart data={visibleChartData} margin={{ top: 10, right: 30, left: 10, bottom: 10 }}>
                  <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#ecf0f1" />
                  <XAxis 
                    dataKey="timeNumeric" 
                    type="number" 
                    domain={zoomDomain} 
                    tickFormatter={formatXAxisTick} 
                    tick={{ fontSize: 11 }} 
                    minTickGap={40} 
                  />
                  <YAxis tick={{ fontSize: 11 }} width={55} tickFormatter={v => v.toFixed(1)} />
                  <Tooltip content={<CustomTooltip />} />
                  
                  {/* QUAN TRỌNG: connectNulls={true} GIÚP NỐI LIỀN CÁC ĐIỂM DỮ LIỆU ĐỨT QUÃNG LỚN */}
                  <Line 
                    type="monotone" 
                    dataKey={selectedMetric} 
                    stroke="#3498db" 
                    strokeWidth={2} 
                    dot={false} 
                    activeDot={{ r: 6 }} 
                    connectNulls={true} 
                  />
                </LineChart>
              </ResponsiveContainer>
            </div>
          </div>
        </>
      )}
    </div>
  );
};

// ... Phần Main Component DemMachineDashboard giữ nguyên không đổi ...
const DemMachineDashboard = () => {
  const [machines, setMachines] = useState([]);
  const [loading, setLoading] = useState(true);
  const [selectedMachine, setSelectedMachine] = useState(null);

  const loadData = async () => {
    setLoading(true);
    try {
      const data = await fetchDemMachines();
      setMachines(Array.isArray(data) ? data : []); 
    } catch (error) {
      console.error("Lỗi API", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  if (selectedMachine) {
    return (
      <DemMachineDetail 
        machine={selectedMachine} 
        onBack={() => setSelectedMachine(null)} 
      />
    );
  }

  return (
    <div className="iot-dashboard-container order-require-container">
      <h2 className="title">Giám sát hệ thống máy Dem ({machines.length} máy)</h2>

      <div className="header">
        <div className="actions">
          <button className="action-btn reload-btn" onClick={loadData} disabled={loading}>
            <FaSyncAlt className={loading ? 'spinning' : ''} /> Tải lại danh sách
          </button>
        </div>
      </div>

      <div className="dashboard-content">
        {loading ? (
          <div className="loading-cell">Đang tải danh sách máy...</div>
        ) : machines.length === 0 ? (
          <div className="no-data-message">Không thể kết nối đến hệ thống máy hoặc chưa có máy nào.</div>
        ) : (
          <div className="machine-grid">
            {machines.map((machine, index) => {
              const machineId = machine.id || machine.machineId || `machine-${index}`;
              const machineName = machine.name || machine.machineName || machineId;
              const machineStatus = machine.status || machine.state || 'Đang hoạt động';

              return (
                <div 
                  key={machineId} 
                  className="machine-card" 
                  style={{ cursor: 'pointer' }}
                  onClick={() => setSelectedMachine(machine)} 
                >
                  <div className="card-header">
                    <div className="machine-info">
                      <FaServer className="machine-icon" />
                      <div>
                        <h3>{machineName}</h3>
                        <span className="line-tag">{machineId}</span>
                      </div>
                    </div>
                    <span 
                      className="status" 
                      style={{ backgroundColor: (machineStatus === 'Running' || machineStatus === 'Active') ? '#2ecc71' : '#95a5a6' }}
                    >
                      {machineStatus}
                    </span>
                  </div>
                  
                  <div className="card-body" style={{ textAlign: 'center', padding: '30px 10px' }}>
                    <FaEye style={{ fontSize: '24px', color: '#bdc3c7', marginBottom: '10px' }} />
                    <p style={{ margin: 0, color: '#7f8c8d', fontSize: '13px' }}>Nhấn để xem chi tiết thông số</p>
                  </div>
                </div>
              );
            })}
          </div>
        )}
      </div>
    </div>
  );
};

export default DemMachineDashboard;