import React, { useMemo, useState } from 'react';
import Chart from 'react-apexcharts';
import { 
  FaArrowLeft, 
  FaCheckCircle, 
  FaFileExport, 
  FaSpinner, 
  FaList, 
  FaChartBar 
} from 'react-icons/fa';
import './ProductionControl.css';

// Hàm helper định dạng ngày
const formatDate = (dateString) => {
  if (!dateString || dateString === "0001-01-01T00:00:00") return "--";
  return new Date(dateString).toLocaleString('vi-VN', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit'
  });
};

const AcceptProductionOrder = ({ data, onBack, onConfirm, isConfirming, readOnly = false }) => {
  const [activeTab, setActiveTab] = useState('table'); // Mặc định hiển thị bảng

  // 1. Chuẩn hóa dữ liệu đầu vào
  const scheduledOrders = useMemo(() => {
    if (!data) return [];
    if (Array.isArray(data)) return data;
    return Object.values(data).flat();
  }, [data]);

  // 2. Tính toán Min/Max Date để trục X hiển thị đúng tỉ lệ
  const { minDate, maxDate } = useMemo(() => {
    if (scheduledOrders.length === 0) return { minDate: undefined, maxDate: undefined };
    
    let min = Infinity;
    let max = -Infinity;

    scheduledOrders.forEach(o => {
        const s = new Date(o.startTime).getTime();
        const e = new Date(o.endTime).getTime();
        if (!isNaN(s) && s < min) min = s;
        if (!isNaN(e) && e > max) max = e;
    });

    // Thêm buffer 1 giờ trước và sau
    const BUFFER = 60 * 60 * 1000; 
    return { 
        minDate: min !== Infinity ? min - BUFFER : undefined, 
        maxDate: max !== -Infinity ? max + BUFFER : undefined 
    };
  }, [scheduledOrders]);

  // 3. Xử lý dữ liệu cho Chart (Track logic)
  const { series: ganttSeries, height: chartHeight } = useMemo(() => {
    if (scheduledOrders.length === 0) return { series: [], height: 350 };

    const map = new Map();
    const trackManager = new Map();

    // Sắp xếp theo thời gian bắt đầu
    const sortedOrders = [...scheduledOrders].sort((a, b) => new Date(a.startTime) - new Date(b.startTime));

    sortedOrders.forEach(order => {
      const machineId = order.equipmentId || 'Chưa gán máy';
      const start = new Date(order.startTime).getTime();
      const end = new Date(order.endTime).getTime();
      
      if (isNaN(start) || isNaN(end) || start >= end) return;

      // Logic chia dòng nếu trùng giờ trên cùng 1 máy
      if (!trackManager.has(machineId)) trackManager.set(machineId, []);
      const tracks = trackManager.get(machineId);

      let assignedTrack = -1;
      for (let i = 0; i < tracks.length; i++) {
        if (start >= tracks[i]) { 
          tracks[i] = end;
          assignedTrack = i;
          break;
        }
      }
      
      if (assignedTrack === -1) {
        tracks.push(end);
        assignedTrack = tracks.length - 1;
      }

      const uniqueRowId = `${machineId}_${assignedTrack}`;
      
      if (!map.has(uniqueRowId)) {
        map.set(uniqueRowId, { 
          name: machineId, 
          data: [] 
        });
      }

      map.get(uniqueRowId).data.push({
        x: machineId,
        y: [start, end],
        fillColor: order.isLateInSchedule ? '#ef4444' : '#3b82f6', // Đỏ nếu trễ, Xanh nếu OK
        orderData: order 
      });
    });

    const finalSeries = Array.from(map.values()).map(item => ({
        name: item.name,
        data: item.data
    })).sort((a, b) => a.name.localeCompare(b.name));

    // Tính chiều cao dynamic
    const height = Math.max(400, finalSeries.length * 60 + 100);

    return { series: finalSeries, height };
  }, [scheduledOrders]);

  // 4. Cấu hình Chart Options
  const chartOptions = useMemo(() => ({
    chart: {
      type: 'rangeBar',
      height: chartHeight,
      fontFamily: 'Inter, sans-serif',
      toolbar: { show: true },
      animations: { enabled: false }
    },
    plotOptions: {
      bar: {
        horizontal: true,
        barHeight: '65%',
        rangeBarGroupRows: true,
        borderRadius: 4
      }
    },
    // Tạo viền trắng để tách các thanh dính liền
    stroke: {
      width: 2,
      colors: ['#ffffff']
    },
    // Hiển thị WorkOrderId ở giữa thanh
    dataLabels: {
      enabled: true,
      textAnchor: 'middle',
      style: {
        colors: ['#fff'],
        fontSize: '11px',
        fontWeight: 'bold',
      },
      formatter: function(val, opts) {
        const seriesIndex = opts.seriesIndex;
        const dataPointIndex = opts.dataPointIndex;
        const w = opts.w;
        const data = w.config.series[seriesIndex].data[dataPointIndex];
        return data.orderData ? data.orderData.workOrderId : '';
      },
      dropShadow: { enabled: true, top: 1, left: 1, blur: 1, color: '#000', opacity: 0.45 }
    },
    xaxis: {
      type: 'datetime',
      min: minDate,
      max: maxDate,
      labels: { 
        datetimeUTC: false, // [QUAN TRỌNG] Giữ nguyên giờ Local (VN)
        datetimeFormatter: { year: 'yyyy', month: 'MM/yyyy', day: 'dd/MM HH:mm' },
        style: { fontSize: '12px' }
      },
      tooltip: { enabled: true } 
    },
    yaxis: {
        labels: {
            style: { fontSize: '13px', fontWeight: 600 },
            offsetX: 0,
            maxWidth: 200
        }
    },
    grid: {
      xaxis: { lines: { show: true } },
      yaxis: { lines: { show: true } },
      padding: { left: 10, right: 10 }
    },
tooltip: {
  enabled: true,
  shared: false,
  intersect: false, // Để tooltip xuất hiện mượt mà hơn khi di chuyển chuột
  followCursor: true, // [QUAN TRỌNG] Cho phép tooltip chạy theo con trỏ
  
  // Tắt cấu hình fixed cũ
  fixed: {
    enabled: false, 
  },

  x: { format: 'dd/MM/yyyy HH:mm' },
  
  custom: ({ seriesIndex, dataPointIndex, w }) => {
    const data = w.config.series[seriesIndex].data[dataPointIndex];
    const order = data.orderData;
    const color = data.fillColor;
    
    // Lưu ý: Đã xóa pointer-events: none để tooltip phản hồi tốt hơn
    return `
      <div class="custom-tooltip" style="
          padding: 12px; 
          border: 1px solid #e2e8f0; 
          background: rgba(255, 255, 255, 0.98); 
          box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1); 
          border-radius: 8px;
          min-width: 300px; 
          font-family: 'Inter', sans-serif;
          position: relative;
      ">
        <div style="
            border-left: 4px solid ${color}; 
            padding-left: 10px; 
            margin-bottom: 10px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        ">
          <div>
            <div style="font-weight: 700; font-size: 15px; color: #1e293b;">${order.workOrderId}</div>
            <div style="font-size: 12px; color: ${color}; font-weight: 600; margin-top: 2px;">
              ${order.isLateInSchedule ? '⚠ Trễ tiến độ' : '✓ Đúng tiến độ'}
            </div>
          </div>
          <div style="background: ${color}20; color: ${color}; font-size: 11px; padding: 2px 6px; border-radius: 4px; font-weight: bold;">
             ${order.status || 'Scheduled'}
          </div>
        </div>
        
        <div style="font-size: 13px; line-height: 1.6; color: #475569;">
          <div style="display: flex; justify-content: space-between; gap: 10px;">
            <span>Yêu cầu:</span> <strong style="color: #334155;">${order.manufacturingOrderId}</strong>
          </div>
          <div style="display: flex; justify-content: space-between; gap: 10px;">
            <span>Sản phẩm:</span> <strong style="color: #334155; text-align: right;">${order.materialDefinition}</strong>
          </div>
          <div style="display: flex; justify-content: space-between; gap: 10px;">
            <span>Mục tiêu:</span> <strong style="color: #334155;">${order.productionTarget} ${order.unit}</strong>
          </div>
          
          <div style="margin-top: 8px; border-top: 1px dashed #cbd5e1; padding-top: 8px;">
            <div style="display: flex; justify-content: space-between; margin-bottom: 4px;">
              <span>Bắt đầu:</span> <span>${formatDate(order.startTime)}</span>
            </div>
            <div style="display: flex; justify-content: space-between;">
              <span>Kết thúc:</span> <span>${formatDate(order.endTime)}</span>
            </div>
            <div style="display: flex; justify-content: space-between;">
              <span>Thời hạn:</span> <span>${formatDate(order.dueDate)}</span>
            </div>
          </div>
        </div>
      </div>
    `;
  }
}
  }), [chartHeight, minDate, maxDate]);

  return (
    <div className="production-control-container fade-in">
      <div className="control-header-actions">
        <div style={{ display: 'flex', alignItems: 'center', gap: '15px' }}>
            <button className="back-btn" onClick={onBack} disabled={isConfirming}>
            <FaArrowLeft /> Điều độ lại
            </button>
            <h2 className="page-title" style={{ margin: 0 }}>Kết quả điều độ sản xuất</h2>
        </div>

        {/* Các nút chuyển đổi View */}
        <div className="view-toggles" style={{ background: '#e2e8f0', padding: '4px', borderRadius: '8px', display: 'flex', gap: '5px' }}>
            <button 
                className={`toggle-btn ${activeTab === 'table' ? 'active' : ''}`}
                style={{ 
                    padding: '8px 12px', border: 'none', borderRadius: '6px', cursor: 'pointer', 
                    background: activeTab === 'table' ? '#fff' : 'transparent',
                    color: activeTab === 'table' ? '#3b82f6' : '#64748b',
                    boxShadow: activeTab === 'table' ? '0 1px 2px rgba(0,0,0,0.1)' : 'none',
                    display: 'flex', alignItems: 'center', gap: '5px', fontWeight: '600'
                }}
                onClick={() => setActiveTab('table')}
            >
                <FaList /> Danh sách
            </button>
            <button 
                className={`toggle-btn ${activeTab === 'gantt' ? 'active' : ''}`}
                style={{ 
                    padding: '8px 12px', border: 'none', borderRadius: '6px', cursor: 'pointer', 
                    background: activeTab === 'gantt' ? '#fff' : 'transparent',
                    color: activeTab === 'gantt' ? '#3b82f6' : '#64748b',
                    boxShadow: activeTab === 'gantt' ? '0 1px 2px rgba(0,0,0,0.1)' : 'none',
                    display: 'flex', alignItems: 'center', gap: '5px', fontWeight: '600'
                }}
                onClick={() => setActiveTab('gantt')}
            >
                <FaChartBar /> Biểu đồ Gantt
            </button>
        </div>
      </div>

      <div className="panel top-section" style={{ height: 'auto', minHeight: '70vh' }}>
        <div className="panel-header success-header">
          <h3 style={{ color: '#27ae60' }}>
            <FaCheckCircle /> Đã lập lịch thành công ({scheduledOrders.length} LSX)
          </h3>
        </div>

        <div className="content-wrapper" style={{ padding: '10px' }}>
            {activeTab === 'table' ? (
                /* === TABLE VIEW === */
                <div className="table-wrapper custom-scrollbar">
                <table className="production-table">
                    <thead>
                    <tr>
                        <th>STT</th>
                        <th>Lệnh sản xuất</th>
                        <th>Công đoạn</th>
                        <th>Yêu cầu sản xuất</th>
                        <th>Sản phẩm</th>
                        <th>Mục tiêu</th>
                        <th>Ngày bắt đầu</th>
                        <th>Ngày kết thúc</th>
                        <th>Thông số</th>
                        <th>Chất lượng</th>
                        <th>Máy</th>
                        <th>Nhân sự</th>
                        <th>BOM</th>
                    </tr>
                    </thead>
                    <tbody>
                    {scheduledOrders.length > 0 ? (
                        scheduledOrders.map((row, index) => {
                        const isLate = row.isLateInSchedule;
                        const rowStyle = isLate ? { backgroundColor: '#fff0f0' } : {};
                        const dateStyle = isLate
                            ? { color: '#e74c3c', fontWeight: 'bold' }
                            : { color: '#0984e3', fontWeight: '600' };
                        const persons = [row.personShift12, row.personShift3].filter(Boolean).join(', ');
                        return (
                            <tr key={index} className="table-row-animate" style={rowStyle}>
                            <td>{index + 1}</td>
                            <td className="font-bold">{row.workOrderId || '--'}</td>
                            <td title={row.workCenter}>{row.segmentId || '--'}</td>
                            <td>{row.manufacturingOrderId || '--'}</td>
                            <td>{row.materialDefinition || '--'}</td>
                            <td>{row.productionTarget ? `${row.productionTarget} ${row.unit}` : '--'}</td>
                            <td style={{ color: '#0984e3', fontWeight: '600' }}>{formatDate(row.startTime)}</td>
                            <td style={dateStyle}>{formatDate(row.endTime)}</td>
                            <td>{row.productionParameterId || '--'}</td>
                            <td>{row.qualityStandardId || '--'}</td>
                            <td className="truncate-text font-bold">{row.equipmentId || '--'}</td>
                            <td className="truncate-text" title={persons}>{persons || '--'}</td>
                            <td className="truncate-text" title={row.boMs || ''}>{row.boMs || '--'}</td>
                            </tr>
                        );
                        })
                    ) : (
                        <tr>
                        <td colSpan="13" className="empty-state">Không có dữ liệu kết quả trả về.</td>
                        </tr>
                    )}
                    </tbody>
                </table>
                </div>
            ) : (
                /* === GANTT CHART VIEW === */
                <div className="gantt-wrapper" style={{ width: '100%', overflowX: 'hidden' }}>
                   {scheduledOrders.length > 0 ? (
                       <Chart 
                           options={chartOptions}
                           series={ganttSeries}
                           type="rangeBar"
                           height={chartHeight}
                           width="100%"
                       />
                   ) : (
                       <div className="empty-state" style={{ textAlign: 'center', padding: '40px' }}>
                           Không có dữ liệu để hiển thị biểu đồ.
                       </div>
                   )}
                </div>
            )}
        </div>
      </div>
            {!readOnly && (
        <div
          className="panel bottom-section active"
          style={{
            minHeight: '80px',
            flexDirection: 'row',
            justifyContent: 'flex-end',
            paddingRight: '20px',
            alignItems: 'center'
          }}
        >
          <button
            className="plan-btn"
            style={{ width: 'auto', padding: '0 30px', height: '40px' }}
            onClick={onConfirm}
            disabled={isConfirming}
          >
            {isConfirming ? (
              <>
                <FaSpinner className="fa-spin" /> Đang lưu...
              </>
            ) : (
              <>
                <FaFileExport /> Xác nhận & Lưu kế hoạch
              </>
            )}
          </button>
        </div>
      )}
    </div>
  );
};
export default AcceptProductionOrder;