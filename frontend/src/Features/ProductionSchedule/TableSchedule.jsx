import React, { useState, useMemo, useEffect } from 'react';
import {
  Search, ArrowUpDown, ArrowUp, ArrowDown, AlertCircle
} from 'lucide-react';
import './TableSchedule.css';
import { getManufacturingOrders } from '../../api/WorkOrdersPlan';

const TableSchedule = ({ data, scheduleMode = 'plan' }) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [sortConfig, setSortConfig] = useState({ key: 'rawStart', direction: 'asc' });
  const [rawData, setRawData] = useState(Array.isArray(data) ? data : []);
  const [isLoading, setIsLoading] = useState(!Array.isArray(data));
  const [error, setError] = useState(null);

  const statusMap = {
    PendingScheduling: 'Chờ xử lý',
    Scheduled: 'Đang điều độ',
    Planned: 'Đã lên kế hoạch',
    InProduction: 'Đang thực thi',
    ReadyForProduction: 'Chờ sản xuất',
    Completed: 'Đã hoàn thành',
    Cancelled: 'Đã hủy',
    delayed: 'Trễ tiến độ',
  };

  const numericStatusMap = {
    2: 'PendingScheduling',
    3: 'Planned',
    4: 'ReadyForProduction',
    5: 'InProduction',
    6: 'Completed',
    7: 'Cancelled',
  };

  const statusColors = {
    'Chờ xử lý': '#95a5a6',
    'Đang điều độ': '#C11DEB',
    'Đã lên kế hoạch': '#8b5cf6',
    'Đang thực thi': '#3498db',
    'Chờ sản xuất': '#f39c12',
    'Đã hoàn thành': '#2ecc71',
    'Đã hủy': '#e74c3c',
    'Trễ tiến độ': '#ef4444',
  };

  useEffect(() => {
    if (Array.isArray(data)) {
      setRawData(data);
      setIsLoading(false);
      setError(null);
      return;
    }

    const fetchData = async () => {
      setIsLoading(true);
      try {
        const response = await getManufacturingOrders();
        setRawData(Array.isArray(response) ? response : []);
      } catch (err) {
        console.error(err);
        setError('Không thể tải dữ liệu bảng.');
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, [data]);

  const hasValidDate = (value) => {
    if (!value) return false;
    const text = String(value);
    if (text.startsWith('0001-01-01')) return false;

    const date = new Date(value);
    return !isNaN(date.getTime());
  };

  const formatDate = (value) => {
    if (!hasValidDate(value)) return '---';

    return new Date(value).toLocaleString('vi-VN', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const normalizeStatus = (status) => {
    if (typeof status === 'number') {
      return numericStatusMap[status] || String(status);
    }

    return status || 'PendingScheduling';
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

  const getEquipmentNames = (item) => {
    const list = item.equipments || item.Equipments || [];

    if (Array.isArray(list) && list.length > 0) {
      const names = list
        .map(e => {
          if (typeof e === 'string') return e;

          return (
            e.equipmentName ||
            e.EquipmentName ||
            e.equipmentId ||
            e.EquipmentId ||
            e.resourceId ||
            e.ResourceId
          );
        })
        .filter(Boolean);

      if (names.length > 0) return names.join(', ');
    }

    return item.equipmentId || item.EquipmentId || item.machine || item.assignedMachineCode || '---';
  };

  const getPersonNames = (item) => {
    const list = item.persons || item.Persons || [];

    if (Array.isArray(list) && list.length > 0) {
      const names = list
        .map(p => {
          if (typeof p === 'string') return p;

          return (
            p.personId ||
            p.PersonId ||
            p.resourceId ||
            p.ResourceId ||
            p.name ||
            p.Name
          );
        })
        .filter(Boolean);

      if (names.length > 0) return names.join(', ');
    }

    return [item.personShift12, item.personShift3].filter(Boolean).join(', ') || '---';
  };

  const processedData = useMemo(() => {
    return rawData.map(item => {
      const statusKey = normalizeStatus(item.status || item.Status || item.workOrderStatus || item.WorkOrderStatus);
      const statusLabel = statusMap[statusKey] || statusKey;

      const plannedStart = getPlannedStart(item);
      const plannedEnd = getPlannedEnd(item);
      const actualStart = getActualStart(item);
      const actualEnd = getActualEnd(item);

      const selectedStart = scheduleMode === 'actual' ? actualStart : plannedStart;
      const selectedEnd = scheduleMode === 'actual' ? actualEnd : plannedEnd;

      const hasStart = hasValidDate(selectedStart);
      const hasEnd = hasValidDate(selectedEnd);

      let startDate = formatDate(selectedStart);
      let endDate = formatDate(selectedEnd);

      if (scheduleMode === 'actual') {
        if (!hasStart) {
          startDate = 'Chưa bắt đầu';
          endDate = '---';
        } else if (!hasEnd && statusKey === 'InProduction') {
          endDate = 'Đang chạy';
        }
      }

      return {
        id: item.workOrderId || item.WorkOrderId || item.id,
        code: item.workOrderId || item.WorkOrderId || item.code || '---',
        rawStart: hasStart ? new Date(selectedStart) : null,
        rawEnd: hasEnd ? new Date(selectedEnd) : null,
        startDate,
        endDate,
        segmentId: item.segmentId || item.SegmentId || item.segmentName || item.workCenter || '---',
        machine: getEquipmentNames(item),
        target: `${Number(item.productionTarget || item.ProductionTarget || 0).toLocaleString()} ${item.unit || item.unitOfMeasure || ''}`,
        status: statusLabel,
        productionParamId: item.productionParamId || item.productionParameterId || item.ProductionParameterId || '---',
        qualityParamId: item.qualityParamId || item.qualityStandardId || item.QualityStandardId || '---',
        personIds: getPersonNames(item),
      };
    });
  }, [rawData, scheduleMode]);

  const filteredAndSortedSchedules = useMemo(() => {
    let result = [...processedData];

    if (searchTerm) {
      const lowerTerm = searchTerm.toLowerCase();

      result = result.filter(item =>
        item.code.toLowerCase().includes(lowerTerm) ||
        item.machine.toLowerCase().includes(lowerTerm) ||
        item.segmentId.toLowerCase().includes(lowerTerm) ||
        item.productionParamId.toLowerCase().includes(lowerTerm) ||
        item.qualityParamId.toLowerCase().includes(lowerTerm) ||
        item.personIds.toLowerCase().includes(lowerTerm) ||
        item.status.toLowerCase().includes(lowerTerm)
      );
    }

    result.sort((a, b) => {
      let aValue = a[sortConfig.key];
      let bValue = b[sortConfig.key];

      if (sortConfig.key === 'rawStart' || sortConfig.key === 'rawEnd') {
        aValue = aValue ? aValue.getTime() : 0;
        bValue = bValue ? bValue.getTime() : 0;
      }

      if (typeof aValue === 'string') aValue = aValue.toLowerCase();
      if (typeof bValue === 'string') bValue = bValue.toLowerCase();

      if (aValue < bValue) return sortConfig.direction === 'asc' ? -1 : 1;
      if (aValue > bValue) return sortConfig.direction === 'asc' ? 1 : -1;

      return 0;
    });

    return result;
  }, [processedData, searchTerm, sortConfig]);

  const handleSort = (key) => {
    setSortConfig(prev => ({
      key,
      direction: prev.key === key && prev.direction === 'asc' ? 'desc' : 'asc',
    }));
  };

  const getSortIcon = (columnKey) => {
    if (sortConfig.key !== columnKey) {
      return <ArrowUpDown size={14} className="sort-icon text-gray-300" />;
    }

    return sortConfig.direction === 'asc'
      ? <ArrowUp size={14} className="sort-icon active" />
      : <ArrowDown size={14} className="sort-icon active" />;
  };

  const getStatusBadge = (status) => {
    const color = statusColors[status] || '#95a5a6';

    return (
      <span
        style={{
          backgroundColor: color,
          color: '#fff',
          padding: '4px 8px',
          borderRadius: '4px',
          fontSize: '12px',
          fontWeight: 'bold',
          display: 'inline-block',
          minWidth: '90px',
          textAlign: 'center',
        }}
      >
        {status}
      </span>
    );
  };

  if (isLoading) return <div className="table-loading">Đang tải dữ liệu...</div>;
  if (error) return <div className="table-error"><AlertCircle size={20} /> {error}</div>;

  return (
    <div className="table-schedule-container">
      <div className="table-toolbar">
        <div className="search-box">
          <Search size={18} className="search-icon" />
          <input
            type="text"
            placeholder="Tìm kiếm lệnh, nhân sự, máy, thông số..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>

        <div className="table-stats">
          Chế độ: <strong>{scheduleMode === 'actual' ? 'Thực tế' : 'Kế hoạch'}</strong>
          {' | '}
          Tổng: <strong>{filteredAndSortedSchedules.length}</strong> bản ghi
        </div>
      </div>

      <div className="table-wrapper">
        <table>
          <thead>
            <tr>
              <th onClick={() => handleSort('code')} className="sortable">
                Lệnh SX {getSortIcon('code')}
              </th>
              <th onClick={() => handleSort('rawStart')} className="sortable">
                {scheduleMode === 'actual' ? 'Bắt đầu thực tế' : 'Bắt đầu kế hoạch'} {getSortIcon('rawStart')}
              </th>
              <th onClick={() => handleSort('rawEnd')} className="sortable">
                {scheduleMode === 'actual' ? 'Kết thúc thực tế' : 'Kết thúc kế hoạch'} {getSortIcon('rawEnd')}
              </th>
              <th onClick={() => handleSort('segmentId')} className="sortable">
                Công đoạn {getSortIcon('segmentId')}
              </th>
              <th onClick={() => handleSort('productionParamId')} className="sortable">
                TS Sản xuất {getSortIcon('productionParamId')}
              </th>
              <th onClick={() => handleSort('qualityParamId')} className="sortable">
                TS Chất lượng {getSortIcon('qualityParamId')}
              </th>
              <th onClick={() => handleSort('personIds')} className="sortable">
                Nhân sự {getSortIcon('personIds')}
              </th>
              <th onClick={() => handleSort('machine')} className="sortable">
                Máy {getSortIcon('machine')}
              </th>
              <th>Mục tiêu</th>
              <th onClick={() => handleSort('status')} className="sortable">
                Trạng thái {getSortIcon('status')}
              </th>
            </tr>
          </thead>

          <tbody>
            {filteredAndSortedSchedules.length > 0 ? (
              filteredAndSortedSchedules.map(item => (
                <tr key={item.id} className="table-row">
                  <td className="font-medium text-primary">{item.code}</td>
                  <td>{item.startDate}</td>
                  <td>{item.endDate}</td>
                  <td><span className="segment-tag">{item.segmentId}</span></td>
                  <td>{item.productionParamId}</td>
                  <td>{item.qualityParamId}</td>
                  <td><div className="cell-truncate" title={item.personIds}>{item.personIds}</div></td>
                  <td><div className="cell-truncate" title={item.machine}>{item.machine}</div></td>
                  <td>{item.target}</td>
                  <td>{getStatusBadge(item.status)}</td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="10" className="empty-state">Không tìm thấy dữ liệu phù hợp</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default TableSchedule;