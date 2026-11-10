import React, { useState, useCallback, useEffect, useMemo } from 'react';
import Chart from 'react-apexcharts';
import { 
  Calendar, 
  ChevronLeft, 
  ChevronRight, 
  RefreshCw, 
  Filter, 
  BarChart2, 
  List,
  AlertCircle
} from 'lucide-react';
import './ProductionSchedule.css';
import TableSchedule from './TableSchedule';
import { getWorkOrderProcess, getWorkOrderErrorHistory } from '../../api/WorkOrderProcess';
import { useProductionProgressRealtime } from '../../realtime/useProductionProgressRealtime';
const MS_PER_DAY = 1000 * 60 * 60 * 24;

const ProductionSchedule = () => {
  const [activeTab, setActiveTab] = useState('gantt');
  const [filterStatus, setFilterStatus] = useState('');
  const [date, setDate] = useState({ 
    month: new Date().getMonth() + 1, 
    year: new Date().getFullYear() 
  });
  const [currentDate, setCurrentDate] = useState(new Date());
  const [workOrders, setWorkOrders] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const [zoomRange, setZoomRange] = useState(null);
  const [scheduleMode, setScheduleMode] = useState('plan'); // plan | actual
useProductionProgressRealtime({
  onProgress: (dto) => {
    const workOrderId = String(
      dto.workOrderId ?? dto.WorkOrderId ?? ""
    ).trim();

    const producedQuantity = Math.floor(
      Number(dto.producedQuantity ?? dto.ProducedQuantity ?? 0)
    );

    const errorQuantity = Math.floor(
      Number(dto.errorQuantity ?? dto.ErrorQuantity ?? 0)
    );

    const progressPercentage = Number(
      dto.progressPercentage ?? dto.ProgressPercentage ?? 0
    );

    const status = dto.status ?? dto.Status ?? "";

    // console.log("UPDATE UI FOR:", {
    //   workOrderId,
    //   producedQuantity,
    //   errorQuantity,
    //   progressPercentage,
    //   status
    // });

    setWorkOrders((prev) =>
      prev.map((x) => {
        const currentId = String(
          x.workOrderId ?? x.WorkOrderId ?? ""
        ).trim();

        if (currentId !== workOrderId) return x;

        return {
          ...x,

          // Các field chuẩn đang dùng cho realtime
          producedQuantity,
          errorQuantity,
          progressPercentage,
          status,

          // Thêm các alias để tránh frontend đang render bằng tên khác
          ProducedQuantity: producedQuantity,
          ErrorQuantity: errorQuantity,
          ProgressPercentage: progressPercentage,
          Status: status,

          productionSuccessQuantity: producedQuantity,
          successQuantity: producedQuantity,
          completedQuantity: producedQuantity,
          totalProducedQuantity: producedQuantity,

          productErrorQuantity: errorQuantity,
          defectiveQuantity: errorQuantity,

          lastRealtimeAt: new Date().toISOString()
        };
      })
    );
  }
});



  useEffect(() => {
    const updateDaily = () => setCurrentDate(new Date());
    updateDaily();
    const interval = setInterval(updateDaily, MS_PER_DAY);
    return () => clearInterval(interval);
  }, []);

  const fetchWorkOrders = useCallback(async () => {
    setIsLoading(true);
    setError(null);
    try {
      const allData = await getWorkOrderProcess();
      const dataWithErrors = await Promise.all(allData.map(async (item) => {
        try {
          const errors = await getWorkOrderErrorHistory(item.workOrderId);
          return { ...item, errors: errors || [] };
        } catch (err) {
          return { ...item, errors: [] };
        }
      }));
      setWorkOrders(dataWithErrors);
    } catch (err) {
      console.error(err);
      setError("Không thể tải dữ liệu tiến độ sản xuất.");
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchWorkOrders();
  }, [fetchWorkOrders]);

  const handlePrevMonth = () => setDate(prev => prev.month === 1 ? { month: 12, year: prev.year - 1 } : { ...prev, month: prev.month - 1 });
  const handleNextMonth = () => setDate(prev => prev.month === 12 ? { month: 1, year: prev.year + 1 } : { ...prev, month: prev.month + 1 });
  const handleToday = () => {
    const today = new Date();
    setDate({ month: today.getMonth() + 1, year: today.getFullYear() });
  };

  useEffect(() => {
  setZoomRange(null);
}, [date.month, date.year, scheduleMode]);

  const hexToRgba = (hex, alpha) => {
  if (!/^#([A-Fa-f0-9]{3}){1,2}$/.test(hex)) return hex;
  let c = hex.substring(1).split('');
  if (c.length === 3) c = [c[0], c[0], c[1], c[1], c[2], c[2]];
  c = '0x' + c.join('');
  return 'rgba(' + [(c >> 16) & 255, (c >> 8) & 255, c & 255].join(',') + ',' + alpha + ')';
};

const getStatusBaseColor = (s) => {
  switch (s) {
    case 'InProduction': return '#3b82f6';
    case 'ReadyForProduction': return '#eab308';
    case 'PendingScheduling': return '#94a3b8';
    case 'Planned': return '#8b5cf6';
    case 'Completed': return '#10b981';
    case 'delayed': return '#ef4444';
    default: return '#9ca3af';
  }
};

const getDisplayStatus = (item, end, mode) => {
  const status = item.status || item.Status || 'ReadyForProduction';

  if (mode === 'actual') return status;

  const keepOriginalColorStatuses = [
    'Completed',
    'ReadyForProduction',
    'PendingScheduling',
    'Planned'
  ];

  if (keepOriginalColorStatuses.includes(status)) {
    return status;
  }

  if (end < currentDate) {
    return 'delayed';
  }

  return status;
};

const hasValidDate = (value) => {
  if (!value) return false;
  const text = String(value);
  if (text.startsWith('0001-01-01')) return false;

  const date = new Date(value);
  return !isNaN(date.getTime());
};
const getPlannedStart = (item) =>
  item.plannedStartTime ||
  item.planedStart ||
  item.planStart ||
  item.Start ||
  item.start ||
  item.startTime;

const getPlannedEnd = (item) =>
  item.plannedEndTime ||
  item.planedEnd ||
  item.planEnd ||
  item.End ||
  item.end ||
  item.endTime;

const getActualStart = (item) =>
  item.actuallyStartTime ||
  item.actualStartTime ||
  item.actualStartDate ||
  item.ActuallyStartTime ||
  item.ActualStartTime ||
  item.ActualStartDate;

const getActualEnd = (item) =>
  item.actuallyEndTime ||
  item.actualEndTime ||
  item.actualEndDate ||
  item.ActuallyEndTime ||
  item.ActualEndTime ||
  item.ActualEndDate;
const getScheduleRange = (item, mode) => {
  if (mode === 'actual') {
  const actualStart = getActualStart(item);
  const actualEnd = getActualEnd(item);
  const status = item.status || item.Status;

  if (!hasValidDate(actualStart)) return null;

  if (!hasValidDate(actualEnd) && status !== 'InProduction') {
    return null;
  }

  return {
    start: actualStart,
    end: hasValidDate(actualEnd) ? actualEnd : new Date().toISOString(),
    type: 'actual'
  };
}

  const plannedStart = getPlannedStart(item);
  const plannedEnd = getPlannedEnd(item);

  if (!hasValidDate(plannedStart) || !hasValidDate(plannedEnd)) return null;

  return {
    start: plannedStart,
    end: plannedEnd,
    type: 'plan'
  };
};

const isBadEquipmentName = (value) => {
  const text = String(value || '').trim();
  return !text || /^\d{12,}(\.\d+)?$/.test(text);
};

const getEquipmentNames = (item) => {
  const equipments = item.equipments || item.Equipments || [];

  if (Array.isArray(equipments) && equipments.length > 0) {
    const names = equipments
      .map(eq => {
        if (typeof eq === 'string') return eq;

        return (
          eq.equipmentId ||
          eq.EquipmentId ||
          eq.resourceId ||
          eq.ResourceId ||
          eq.code ||
          eq.Code ||
          eq.name ||
          eq.Name ||
          eq.equipmentName ||
          eq.EquipmentName
        );
      })
      .filter(name => !isBadEquipmentName(name));

    if (names.length > 0) return names;
  }

  const fallback =
    item.equipmentId ||
    item.EquipmentId ||
    item.machineId ||
    item.machine ||
    item.assignedMachineCode;

  return [isBadEquipmentName(fallback) ? 'Chưa gán máy' : fallback];
};
  const tasks = useMemo(() => {
  const machineRows = new Map();

  workOrders.forEach(item => {
    const range = getScheduleRange(item, scheduleMode);
    if (!range) return;

    const start = new Date(range.start);
    const end = new Date(range.end);

    if (isNaN(start.getTime()) || isNaN(end.getTime())) {
      console.warn(`Invalid dates for workOrder ${item.workOrderId}:`, range.start, range.end);
      return;
    }

    if (start >= end) {
      console.warn(`Start date >= end date for workOrder ${item.workOrderId}`);
      return;
    }

    const durationDays = (end - start) / MS_PER_DAY;
    const safeDuration = durationDays > 0 ? durationDays : 0.1;
    const targetPerDay = (item.productionTarget || 0) / safeDuration;

    const status = getDisplayStatus(item, end, scheduleMode);
    const equipmentNames = getEquipmentNames(item);

    const progressValue = Number(item.progressPercentage || item.ProgressPercentage || 0);
    const validProgress = !isNaN(progressValue)
      ? Math.min(Math.max(progressValue, 0), 100)
      : 0;

    const errorsList = Array.isArray(item.errors) ? item.errors : [];

    equipmentNames.forEach(rowName => {
      if (!machineRows.has(rowName)) {
        machineRows.set(rowName, []);
      }

      machineRows.get(rowName).push({
        ...item,
        taskStart: start,
        taskEnd: end,
        taskStatus: status,
        taskProgress: validProgress,
        targetPerDay,
        errors: errorsList
      });
    });
  });

  // Tab thực tế: tách track để các lệnh cùng máy bị chồng thời gian không đè lên nhau
  if (scheduleMode === 'actual') {
    const trackedRows = [];

    machineRows.forEach((items, machineName) => {
      const tracks = [];

      const sortedItems = [...items].sort(
        (a, b) => a.taskStart.getTime() - b.taskStart.getTime()
      );

      sortedItems.forEach(item => {
        const startMs = item.taskStart.getTime();
        const endMs = item.taskEnd.getTime();

        let trackIndex = tracks.findIndex(lastEndMs => startMs >= lastEndMs);

        if (trackIndex === -1) {
          tracks.push(endMs);
          trackIndex = tracks.length - 1;
        } else {
          tracks[trackIndex] = endMs;
        }

        const laneName = `${machineName} - Dòng ${trackIndex + 1}`;

        let lane = trackedRows.find(row => row.name === laneName);
        if (!lane) {
          lane = { name: laneName, items: [] };
          trackedRows.push(lane);
        }

        lane.items.push(item);
      });
    });

    return trackedRows.sort((a, b) => a.name.localeCompare(b.name));
  }

  // Tab kế hoạch: giữ theo máy như lịch đã điều độ
  return Array.from(machineRows.entries())
    .map(([name, items]) => ({
      name,
      items: items.sort((a, b) => a.taskStart.getTime() - b.taskStart.getTime())
    }))
    .sort((a, b) => a.name.localeCompare(b.name));
}, [workOrders, currentDate, scheduleMode]);

  const chartSeries = useMemo(() => {
    const backgroundData = [];
    const progressData = [];
    const errorData = [];

    tasks.forEach(taskRow => {
      taskRow.items.forEach(item => {
        if (filterStatus && item.taskStatus !== filterStatus) return;
        
        const baseColor = getStatusBaseColor(item.taskStatus);
        const startMs = item.taskStart.getTime();
        const endMs = item.taskEnd.getTime();
        
        backgroundData.push({
          x: taskRow.name,
          y: [startMs, endMs],
          fillColor: hexToRgba(baseColor, 0.15),
          apiData: item,
          type: 'bg'
        });
        
        if (scheduleMode === 'plan' && item.taskProgress > 0) {
  const progEnd = startMs + (endMs - startMs) * (Math.min(item.taskProgress, 100) / 100);
  progressData.push({
    x: taskRow.name,
    y: [startMs, progEnd],
    fillColor: baseColor,
    apiData: item,
    type: 'fg'
  });
}

        // FIX #3: Safe error data processing
        if (Array.isArray(item.errors) && item.errors.length > 0) {
          item.errors.forEach(err => {
            if (err.start && err.end) {
              const errStart = new Date(err.start).getTime();
              const errEnd = new Date(err.end).getTime();
              if (!isNaN(errStart) && !isNaN(errEnd) && errStart < errEnd) {
                errorData.push({
                  x: taskRow.name,
                  y: [errStart, errEnd],
                  fillColor: '#dc2626',
                  errorInfo: err,
                  apiData: item,
                  type: 'error'
                });
              }
            }
          });
        }
      });
    });

    const series = [
  { name: scheduleMode === 'plan' ? 'Kế hoạch' : 'Thực tế', data: backgroundData }
];

if (scheduleMode === 'plan') {
  series.push({ name: 'Tiến độ', data: progressData });
}

series.push({ name: 'Sự cố', data: errorData });

return series;
  }, [tasks, filterStatus, scheduleMode]);

  const chartOptions = useMemo(() => {
    const monthStart = new Date(date.year, date.month - 1, 1).getTime();
    const monthEnd = new Date(date.year, date.month, 0, 23, 59, 59).getTime();

    return {
      chart: { 
        type: 'rangeBar', 
        fontFamily: 'Inter, sans-serif',
        toolbar: { show: true, autoSelected: 'zoom' },
        zoom: { enabled: true, type: 'x', autoScaleYaxis: true },
        events: {
          zoomed: (_, { xaxis }) => {
            if (xaxis) {
              setZoomRange({
                min: xaxis.min,
                max: xaxis.max
              });
            }
          }
        },
        animations: { enabled: false }
      },
      plotOptions: { 
        bar: { 
          horizontal: true, 
          barHeight: '75%', 
          rangeBarGroupRows: true, 
          rangeBarOverlap: true,
          borderRadius: 4
        } 
      },
      dataLabels: {
        enabled: true,
        formatter: (val, opts) => {
          const d = opts.w.config.series[opts.seriesIndex].data[opts.dataPointIndex];
          if (d.type === 'bg') {
  if (scheduleMode === 'actual') {
    return `${d.apiData.workOrderId}`;
  }

  const progress = (d.apiData.taskProgress || 0).toFixed(2);
  return `${d.apiData.workOrderId} (${progress}%)`;
}
          return '';
        },
        style: { fontSize: '10px', fontWeight: 600, colors: ['#334155'] }
      },
      xaxis: { 
        type: 'datetime', 
        min: zoomRange?.min ?? monthStart,
        max: zoomRange?.max ?? monthEnd,
        position: 'top',
        labels: { datetimeUTC: false, style: { fontSize: '11px' } }
      },
      tooltip: {
        shared: false,
        intersect: true,
        custom: ({ seriesIndex, dataPointIndex, w }) => {
          const d = w.config.series[seriesIndex].data[dataPointIndex];
          const { apiData, type, errorInfo } = d;

          // Xử lý Tooltip riêng cho thanh màu đỏ (Sự cố đơn lẻ)
          if (type === 'error') {
            return `
              <div style="padding:12px; border-radius:8px; background:#fff; min-width:220px; box-shadow: 0 10px 15px -3px rgba(0,0,0,0.1);">
                <div style="display:flex; align-items:center; color:#dc2626; font-weight:bold; margin-bottom:8px; gap:6px;">
                   <span style="font-size:16px;">⚠️</span> CHI TIẾT SỰ CỐ
                </div>
                <div style="font-size:12px; color:#1e293b;">
                  <div style="margin-bottom:4px;"><b>Mã lỗi:</b> ${errorInfo.errorId || 'N/A'}</div>
                  <div style="margin-bottom:4px;"><b>Tên lỗi:</b> ${errorInfo.errorName}</div>
                  <div style="margin-bottom:4px;"><b>Bắt đầu:</b> ${new Date(errorInfo.start).toLocaleTimeString()}</div>
                  <div style="margin-bottom:4px;"><b>Kết thúc:</b> ${new Date(errorInfo.end).toLocaleTimeString()}</div>
                  <div style="color:#64748b; font-style:italic; margin-top:4px; border-top:1px solid #f1f5f9; padding-top:4px;">
                    Lệnh: ${apiData.workOrderId}
                  </div>
                </div>
              </div>`;
          }

          const color = getStatusBaseColor(apiData.taskStatus);
          const progressFormatted = (apiData.taskProgress || 0).toFixed(2);
          const formattedTargetDay = new Intl.NumberFormat('vi-VN').format(apiData.targetPerDay || 0);

          // Xử lý danh sách lỗi để hiển thị trong Tooltip tổng
          const errorListHtml = apiData.errors && apiData.errors.length > 0 
            ? `<div style="margin-top:10px; border-top:1px dashed #fee2e2; padding-top:8px;">
                 <div style="color:#b91c1c; font-weight:bold; font-size:11px; margin-bottom:4px; display:flex; align-items:center; gap:4px;">
                    <span style="width:6px; height:6px; background:#dc2626; border-radius:50%;"></span>
                    LỊCH SỬ SỰ CỐ (${apiData.errors.length})
                 </div>
                 <div style="max-height:100px; overflow-y:auto;">
                   ${apiData.errors.map(err => `
                     <div style="font-size:10px; color:#475569; margin-bottom:3px; background:#fff5f5; padding:4px; border-radius:4px;">
                        • <b>${err.errorName}:</b> ${new Date(err.start).toLocaleTimeString()} - ${new Date(err.end).toLocaleTimeString()}
                     </div>
                   `).join('')}
                 </div>
               </div>`
            : '';

          return `
            <div style="padding:12px; min-width:280px; border-radius:8px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); background:#fff;">
              <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:8px; border-left: 4px solid ${color}; padding-left: 8px;">
                <span style="font-weight:800; color:#1e293b; font-size:14px;">${apiData.workOrderId}</span>
                <span style="background:${color}22; color:${color}; padding:2px 8px; border-radius:10px; font-size:10px; font-weight:bold;">${apiData.taskStatus}</span>
              </div>
              <div style="font-size:12px; line-height:1.6; color:#475569;">
                <b>Sản phẩm:</b> ${apiData.materialDefinitionId || 'N/A'}<br/>
                <div style="background: #f8fafc; padding: 4px; border-radius: 4px; margin: 4px 0;">
                   <b>Cần sản xuất/ngày:</b> ${formattedTargetDay} ${apiData.unitOfMeasure || ''}
                </div>
                <b>Tiến độ tổng:</b>
                <div style="margin-top:4px; display:flex; align-items:center; gap:8px;">
                  <div style="flex:1; height:6px; background:#e2e8f0; border-radius:3px; overflow:hidden;">
                    <div style="width:${Math.min(apiData.taskProgress, 100)}%; height:100%; background:${color};"></div>
                  </div>
                  <span style="font-weight:bold; color:#1e293b;">${progressFormatted}%</span>
                </div>
                ${errorListHtml}
                <hr style="margin:8px 0; border:0; border-top:1px solid #f1f5f9;"/>
                <div style="font-size:11px; color:#94a3b8;">
                  Bắt đầu: ${apiData.taskStart.toLocaleString('vi-VN')}<br/>
                  Kết thúc: ${apiData.taskEnd.toLocaleString('vi-VN')}
                </div>
              </div>
            </div>`;
        }
      }
    };
  }, [date.month, date.year, tasks, zoomRange, scheduleMode]);

  if (isLoading) return <div className="loading-wrapper"><div className="loader"></div><p>Đang đồng bộ lệnh sản xuất...</p></div>;

  return (
    <div className="ps-container">
      <div className="ps-header">
        <h1 className="title">Quản Lý Lệnh Sản Xuất</h1>
        <div className="ps-actions">
          <div className="view-toggles">
            <button className={`toggle-btn ${activeTab === 'gantt' ? 'active' : ''}`} onClick={() => setActiveTab('gantt')}><BarChart2 size={18} /></button>
            <button className={`toggle-btn ${activeTab === 'table' ? 'active' : ''}`} onClick={() => setActiveTab('table')}><List size={18} /></button>
          </div>
          <button className="action-btn reload-btn" onClick={fetchWorkOrders}><RefreshCw size={18} /></button>
        </div>
      </div>

      <div className="ps-toolbar">
      <div className="view-toggles">
  <button
    className={`toggle-btn ${scheduleMode === 'plan' ? 'active' : ''}`}
    onClick={() => setScheduleMode('plan')}
  >
    Kế hoạch
  </button>

  <button
    className={`toggle-btn ${scheduleMode === 'actual' ? 'active' : ''}`}
    onClick={() => setScheduleMode('actual')}
  >
    Thực tế
  </button>
</div>
        <div className="toolbar-left">
          <div className="filter-dropdown">
            <Filter size={16} />
            <select value={filterStatus} onChange={e => setFilterStatus(e.target.value)}>
              <option value="">Tất cả trạng thái</option>
              <option value="InProduction">Đang thực thi</option>
              <option value="ReadyForProduction">Chờ sản xuất</option>
              <option value="Completed">Đã hoàn thành</option>
              <option value="delayed">Trễ tiến độ</option>
            </select>
          </div>
        </div>
        <div className="toolbar-center">
          <button onClick={handlePrevMonth} className="nav-btn"><ChevronLeft /></button>
          <div className="date-indicator">
             <Calendar size={18} style={{marginRight: 8}} />
             Tháng {date.month}, {date.year}
          </div>
          <button onClick={handleNextMonth} className="nav-btn"><ChevronRight /></button>
          <button onClick={handleToday} className="today-btn">Hôm nay</button>
        </div>
        <div className="toolbar-right">
          <div style={{display:'flex', gap:'12px', fontSize:'11px'}}>
            <span>
  <i style={{background:'#3b82f6', width:10, height:10, display:'inline-block', borderRadius:2, marginRight:4}}></i>
  {scheduleMode === 'plan' ? 'Tiến độ' : 'Thực tế'}
</span>
            <span><i style={{background:'#dc2626', width:10, height:10, display:'inline-block', borderRadius:2, marginRight:4}}></i> Sự cố</span>
          </div>
        </div>
      </div>

      <div className="ps-content">
        {activeTab === 'gantt' ? (
          <div className="gantt-wrapper">
            <Chart 
              options={chartOptions} 
              series={chartSeries} 
              type="rangeBar" 
              height={Math.max(500, tasks.length * 70)} 
              width="100%" 
            />
          </div>
        ) : (
          <TableSchedule data={workOrders} scheduleMode={scheduleMode} />
        )}
      </div>
    </div>
  );
};

export default ProductionSchedule;