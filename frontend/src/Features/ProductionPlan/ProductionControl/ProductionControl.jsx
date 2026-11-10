import React, { useState, useEffect, useCallback } from 'react';
import { FaArrowLeft, FaClipboardList, FaTimes, FaLayerGroup, FaSpinner } from 'react-icons/fa';
import './ProductionControl.css';
import {
  getWorkOrdersByStatus2,
  scheduleWorkOrders,
  confirmListWorkOrders,
  getCurrentProductionSchedule,
  rescheduleUrgentOrder
} from '../../../api/WorkOrdersPlan';
import AcceptProductionOrder from './AcceptProductionOrder';

  const getWorkOrderNumber = (workOrderId) => {
  const match = String(workOrderId || '').match(/\d+/);
  return match ? Number(match[0]) : 0;
};

  const ProductionControl = ({ onBack }) => {
  const [workOrders, setWorkOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [confirming, setConfirming] = useState(false);
  const [selectedOrders, setSelectedOrders] = useState([]);
  const [selectedAlgorithm, setSelectedAlgorithm] = useState('TabuSearch');
  const [scheduledResult, setScheduledResult] = useState(null);
  const [isReschedulingUrgent, setIsReschedulingUrgent] = useState(false);
  const ALGORITHMS = [
    { value: 'GeneticAlgorithm', label: 'Genetic Algorithm' },
    { value: 'TabuSearch', label: 'Tabu Search' },
    { value: 'BruteForce', label: 'Brute Force' },
  ];
  const [currentSchedule, setCurrentSchedule] = useState([]);
  const [currentSchedulePreview, setCurrentSchedulePreview] = useState(null);
  const loadProductionData = useCallback(async (showLoading = true) => {
  try {
    if (showLoading) setLoading(true);

    const [pendingData, currentData] = await Promise.all([
      getWorkOrdersByStatus2(),
      getCurrentProductionSchedule()
    ]);

    setWorkOrders(Array.isArray(pendingData) ? pendingData : []);
    setCurrentSchedule(Array.isArray(currentData) ? currentData : []);
  } catch (error) {
    console.error("Lỗi tải dữ liệu:", error);
    setWorkOrders([]);
    setCurrentSchedule([]);
  } finally {
    if (showLoading) setLoading(false);
  }
}, []);
  useEffect(() => {
  loadProductionData();
}, [loadProductionData]);

  const handleSelectOrder = (order) => {
    if (!selectedOrders.find(o => o.id === order.id)) {
      setSelectedOrders((prev) => [...prev, order]);
    }
  };
  const getAlgorithmType = () => {
  switch (selectedAlgorithm) {
    case 'GeneticAlgorithm':
      return 2;
    case 'TabuSearch':
      return 3;
    case 'BruteForce':
      return 4;
    default:
      return 3;
  }
};

  const normalizeCurrentSchedule = (items = []) => {
  return items.map((row) => {
    const equipmentId = row.equipmentId || row.equipments?.[0]?.equipmentId || '';

    const personShift12 =
      row.personShift12 ||
      row.persons?.find(p => p.shift === 'Ca12')?.personId ||
      row.persons?.[0]?.personId ||
      '';

    const personShift3 =
      row.personShift3 ||
      row.persons?.find(p => p.shift === 'Ca3')?.personId ||
      row.persons?.[1]?.personId ||
      '';

    return {
      ...row,
      startTime: row.startTime || row.start,
      endTime: row.endTime || row.end,
      materialDefinition: row.materialDefinition || row.materialDefinitionId,
      manufacturingOrderId: row.manufacturingOrderId || row.manufacturingOrderCode,
      equipmentId,
      personShift12,
      personShift3,
      boMs: row.boMs || row.bOMs || []
    };
  });
};
  const handleDeselectOrder = (id) => {
    setSelectedOrders((prev) => prev.filter(item => item.id !== id));
  };
  const handleRescheduleUrgentOrder = async () => {
  if (!selectedOrders || selectedOrders.length === 0) {
    alert('Vui lòng chọn đơn hàng quan trọng cần chen vào lịch.');
    return;
  }

  try {
    setIsReschedulingUrgent(true);

    const result = await rescheduleUrgentOrder(selectedOrders, getAlgorithmType());

    setScheduledResult(result);
    setSelectedOrders([]);
  } catch (error) {
    console.error(error);
    alert(
      error?.response?.data?.message ||
      error?.response?.data ||
      'Điều độ chen đơn quan trọng thất bại.'
    );
  } finally {
    setIsReschedulingUrgent(false);
  }
};
  const handlePlanProduction = async () => {
    if (selectedOrders.length === 0) return;
  let algorithmType;

  switch (selectedAlgorithm) {
    case 'GeneticAlgorithm':
      algorithmType = 2;
      break;
    case 'TabuSearch':
      algorithmType = 3;
      break;
    case 'BruteForce':
      algorithmType = 4;
      break;
    default:
      algorithmType = 3;
  }


    setSubmitting(true);
    try {
      const responseData = await scheduleWorkOrders(selectedOrders, algorithmType);
      console.log("Kết quả lập lịch:", responseData);
      setScheduledResult(responseData);
      setSelectedOrders([]);
    } catch (error) {
      console.error("Lỗi lập lịch:", error);
      alert("Có lỗi xảy ra khi gửi yêu cầu lập lịch. Vui lòng thử lại.");
    } finally {
      setSubmitting(false);
    }
  };

  const handleConfirmResult = async () => {
    if (!scheduledResult) {
      console.error("Không có kết quả lập lịch để xác nhận.");
      return;
    }

    setConfirming(true);
    try {
      const allWorkOrders = Object.values(scheduledResult).flat();
      await confirmListWorkOrders(allWorkOrders);
      alert("Kế hoạch sản xuất đã được lưu thành công!");
      setScheduledResult(null);
      onBack();
    } catch (error) {
      console.error("Lỗi khi xác nhận/lưu kế hoạch:", error);
      alert("Có lỗi xảy ra khi lưu kế hoạch. Vui lòng thử lại.");
    } finally {
      setConfirming(false);
    }
  };

  const availableOrders = workOrders
  .filter((order) => !selectedOrders.some((selected) => selected.id === order.id))
  .sort((a, b) => {
    const numberA = getWorkOrderNumber(a.workOrderId);
    const numberB = getWorkOrderNumber(b.workOrderId);

    return numberB - numberA;
  });
  if (currentSchedulePreview) {
  return (
    <AcceptProductionOrder
      data={currentSchedulePreview}
      onBack={() => setCurrentSchedulePreview(null)}
      onConfirm={() => setCurrentSchedulePreview(null)}
      isConfirming={false}
      readOnly={true}
    />
  );
}

  if (scheduledResult) {
    return (
      <AcceptProductionOrder
        data={scheduledResult}
        onBack={() => setScheduledResult(null)}
        onConfirm={handleConfirmResult}
        isConfirming={confirming}
      />
    );
  }

  return (
    <div className="production-control-container fade-in">
      <div className="control-header-actions">
        <button className="back-btn" onClick={onBack} disabled={submitting}>
          <FaArrowLeft /> Quay lại
        </button>
        <button
  className="plan-btn"
  style={{ width: 'auto', padding: '0 18px', height: '38px' }}
  onClick={() => setCurrentSchedulePreview(normalizeCurrentSchedule(currentSchedule))}
  disabled={currentSchedule.length === 0}
>
  Xem lịch hiện tại ({currentSchedule.length})
</button>
        <h2 className="page-title">Điều độ sản xuất</h2>
      </div>

      <div className="panel top-section">
        <div className="panel-header">
          <h3><FaLayerGroup /> Danh sách chờ điều độ ({availableOrders.length})</h3>
        </div>

        <div className="table-wrapper custom-scrollbar">
          {loading ? (
            <div className="loading-state">
              <div className="spinner"></div>
              <p>Đang tải dữ liệu...</p>
            </div>
          ) : (
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
                  <th>Thông số sản xuất</th>
                  <th>Thông số chất lượng</th>
                  <th>Máy</th>
                  <th>Nhân sự</th>
                  <th>BOM</th>
                </tr>
              </thead>
              <tbody>
                {availableOrders.length > 0 ? (
                  availableOrders.map((row, index) => (
                    <tr
                      key={row.id}
                      onClick={() => !submitting && handleSelectOrder(row)}
                      className={`table-row-animate ${submitting ? 'disabled-row' : ''}`}
                    >
                      <td>{index + 1}</td>
                      <td className="font-bold">{row.workOrderId || '--'}</td>
                      <td>{row.segmentName || '--'}</td>
                      <td>{row.manufacturingOrderCode || '--'}</td>
                      <td>{row.materialDefinitionId || '--'}</td>
                      <td>{row.productionTarget ? `${row.productionTarget} ${row.unit}` : '--'}</td>
                      <td>{row.start === "0001-01-01T00:00:00" ? "--" : row.start}</td>
                      <td>{row.end === "0001-01-01T00:00:00" ? "--" : row.end}</td>
                      <td className="truncate-text" title={row.productionParamId || ''}>{row.productionParamId || ''}</td>
                      <td className="truncate-text" title={row.qualityParamId || ''}>{row.qualityParamId || ''}</td>
                      <td className="truncate-text" title={row.equipments?.join(', ')}>
                        {Array.isArray(row.equipments) ? row.equipments.join(', ') : '--'}
                      </td>
                      <td className="truncate-text" title={row.persons?.join(', ')}>
                        {Array.isArray(row.persons) ? row.persons.join(', ') : '--'}
                      </td>
                      <td className="truncate-text" title={row.boMs?.join(', ')}>
                        {Array.isArray(row.boMs) ? row.boMs.join(', ') : ''}
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan="13" className="empty-state">
                      {workOrders.length > 0 ? "Tất cả đơn hàng đã được chọn." : "Không có dữ liệu hiển thị."}
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          )}
        </div>
      </div>

      <div className={`panel bottom-section ${selectedOrders.length > 0 ? 'active' : ''}`}>
        <div className="bottom-header">
          <h3>Đã chọn ({selectedOrders.length})</h3>
          <div className={`action-controls ${selectedOrders.length === 0 ? 'disabled' : ''}`}>
            <select
              className="algo-select"
              value={selectedAlgorithm}
              onChange={(e) => setSelectedAlgorithm(e.target.value)}
              disabled={selectedOrders.length === 0 || submitting}
            >
              {ALGORITHMS.map(algo => (
                <option key={algo.value} value={algo.value}>{algo.label}</option>
              ))}
            </select>
            <button
              className="plan-btn"
              onClick={handlePlanProduction}
              disabled={selectedOrders.length === 0 || submitting}
            >
              {submitting ? (
                <>
                  <FaSpinner className="fa-spin" /> Đang xử lý...
                </>
              ) : (
                <>
                  <FaClipboardList /> Lên kế hoạch
                </>
              )}
            </button>
            <button
  className="plan-btn"
  onClick={handleRescheduleUrgentOrder}
  disabled={isReschedulingUrgent || selectedOrders.length === 0}
>
  {isReschedulingUrgent ? 'Đang điều độ lại...' : 'Chen đơn hàng quan trọng'}
</button>
          </div>
        </div>

        <div className="selected-list-container custom-scrollbar">
          {selectedOrders.length === 0 ? (
            <p className="placeholder-text">Chọn đơn hàng từ bảng trên để đưa vào danh sách điều độ.</p>
          ) : (
            <div className="tags-wrapper">
              {selectedOrders.map((order, idx) => (
                <div
                  key={order.id}
                  className="order-tag pop-in"
                  style={{ animationDelay: `${idx * 0.05}s` }}
                  onClick={() => !submitting && handleDeselectOrder(order.id)}
                >
                  <span>{order.workOrderId}</span>
                  {!submitting && <FaTimes className="remove-icon" />}
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default ProductionControl;
