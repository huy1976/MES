import React, { useState, useEffect } from 'react';
import './CreateProductionOrder.css';
import {
  FaArrowLeft,
  FaPlus,
  FaCalendarAlt,
  FaCheck,
  FaSpinner,
  FaCut,
  FaUndo,
} from 'react-icons/fa';

import {
  getManufacturingOrdersByStatus1,
  createWorkOrdersForManufacturingOrder,
  getQuantityWorkOrders,
  addListWorkOrders,
} from '../../../api/GetManufacturingOrdersByStatus';

const getApiPayload = (value) => value?.response?.data ?? value?.data ?? value;

const getApiErrorMessage = (error, fallbackMessage) => {
  const payload = getApiPayload(error);

  if (payload?.message) return payload.message;
  if (payload?.detail) return payload.detail;
  if (typeof payload === 'string') return payload;
  if (error?.message) return error.message;

  return fallbackMessage;
};

const assertSuccessfulResponse = (result) => {
  const payload = getApiPayload(result);

  if (payload?.errorCode || payload?.success === false || payload?.isSuccess === false) {
    throw payload;
  }

  return payload;
};

const normalizeArrayResponse = (value) => {
  const payload = getApiPayload(value);

  if (Array.isArray(payload)) return payload;
  if (Array.isArray(payload?.items)) return payload.items;
  if (Array.isArray(payload?.result)) return payload.result;
  if (Array.isArray(payload?.results)) return payload.results;
  if (Array.isArray(payload?.data)) return payload.data;

  return [];
};

const normalizeWorkOrderResponse = (result) => {
  const payload = assertSuccessfulResponse(result);

  if (!payload) return [];
  if (Array.isArray(payload)) return payload.flat().filter(Boolean);

  const nestedList = normalizeArrayResponse(payload);
  if (nestedList.length > 0) return nestedList.flat().filter(Boolean);

  return [payload];
};

const toNumber = (value) => {
  const numberValue = Number(value);
  if (Number.isFinite(numberValue)) return numberValue;

  const parsedValue = Number.parseFloat(value);
  return Number.isFinite(parsedValue) ? parsedValue : 0;
};

const getNextWorkOrderNumber = (value) => {
  const payload = getApiPayload(value);

  const explicitNext =
    payload?.nextWorkOrderNumber ??
    payload?.nextNumber ??
    payload?.next;

  if (explicitNext !== undefined) {
    const nextValue = Number(explicitNext);
    return Number.isFinite(nextValue) && nextValue > 0 ? Math.floor(nextValue) : 1;
  }

  let counterValue = 0;

  if (Array.isArray(payload)) {
    counterValue = payload.length;
  } else if (typeof payload === 'number' || typeof payload === 'string') {
    counterValue = Number(payload);
  } else if (payload && typeof payload === 'object') {
    counterValue = Number(
      payload.quantity ??
      payload.count ??
      payload.total ??
      payload.totalCount ??
      payload.value ??
      0
    );
  }

  return Number.isFinite(counterValue) && counterValue >= 0
    ? Math.floor(counterValue) + 1
    : 1;
};

const formatWorkOrderId = (number) => `LSX${number}`;

const formatDate = (dateValue) => {
  if (!dateValue) return 'Chưa có hạn';

  const date = new Date(dateValue);
  if (Number.isNaN(date.getTime())) return 'Ngày không hợp lệ';

  return date.toLocaleDateString('vi-VN');
};

const getFailedWorkOrderIds = (result) => {
  const payload = getApiPayload(result);
  const failedItems =
    payload?.failedWorkOrderIds ??
    payload?.failedIds ??
    payload?.failedItems ??
    payload?.errors;

  if (!Array.isArray(failedItems)) return [];

  return failedItems
    .map((item) => {
      if (typeof item === 'string') return item;
      return item?.workOrderId ?? item?.id ?? item?.code;
    })
    .filter(Boolean);
};

const CreateProductionOrder = ({ onBack }) => {
  const [selectedRequests, setSelectedRequests] = useState([]);
  const [createdOrders, setCreatedOrders] = useState([]);
  const [pendingRequests, setPendingRequests] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [isCreating, setIsCreating] = useState(false);
  const [isApproving, setIsApproving] = useState(false);
  const [workOrderCounter, setWorkOrderCounter] = useState(null);

  const [isSplitting, setIsSplitting] = useState(false);
  const [splitTargetWo, setSplitTargetWo] = useState(null);
  const [splitAmount, setSplitAmount] = useState('');

  const [actionError, setActionError] = useState('');
  const [actionSuccess, setActionSuccess] = useState('');

  useEffect(() => {
    let isMounted = true;

    const fetchInitialData = async () => {
      try {
        setLoading(true);

        const [pendingData, initialCounter] = await Promise.all([
          getManufacturingOrdersByStatus1(),
          getQuantityWorkOrders(),
        ]);

        if (!isMounted) return;

        const pendingPayload = assertSuccessfulResponse(pendingData);
        const counterPayload = assertSuccessfulResponse(initialCounter);

        setPendingRequests(normalizeArrayResponse(pendingPayload));
        setWorkOrderCounter(getNextWorkOrderNumber(counterPayload));
      } catch (err) {
        if (!isMounted) return;

        console.error('Lỗi tải dữ liệu:', getApiPayload(err));
        setError(getApiErrorMessage(err, 'Không thể tải dữ liệu. Vui lòng thử lại.'));
      } finally {
        if (isMounted) setLoading(false);
      }
    };

    fetchInitialData();

    return () => {
      isMounted = false;
    };
  }, []);

  const clearActionMessages = () => {
    setActionError('');
    setActionSuccess('');
  };

  const handleSelect = (id) => {
    if (!id || isCreating || isApproving) return;

    clearActionMessages();

    setSelectedRequests((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
    );
  };

  const handleCreateOrders = async () => {
    const selectedIds = [...selectedRequests];

    if (selectedIds.length === 0 || workOrderCounter === null || isCreating) return;

    clearActionMessages();
    setIsCreating(true);

    try {
      const results = await Promise.allSettled(
        selectedIds.map(async (id) => {
          const result = await createWorkOrdersForManufacturingOrder(id);
          const workOrders = normalizeWorkOrderResponse(result);

          if (workOrders.length === 0) {
            throw new Error(`Yêu cầu ${id} không trả về lệnh sản xuất.`);
          }

          return { id, workOrders };
        })
      );

      let currentCounter = workOrderCounter;
      const successfulIds = [];
      const failedMessages = [];
      const remappedWOs = [];

      results.forEach((result, index) => {
        const requestId = selectedIds[index];

        if (result.status === 'fulfilled') {
          successfulIds.push(result.value.id);

          result.value.workOrders.forEach((wo) => {
            remappedWOs.push({
              ...wo,
              workOrderId: formatWorkOrderId(currentCounter),
              productionTarget: toNumber(wo.productionTarget),
              bomIds: Array.isArray(wo.bomIds) ? [...wo.bomIds] : wo.bomIds,
            });

            currentCounter += 1;
          });

          return;
        }

        failedMessages.push(
          `${requestId}: ${getApiErrorMessage(result.reason, 'Không tạo được lệnh sản xuất')}`
        );
      });

      if (remappedWOs.length > 0) {
        const successfulIdSet = new Set(successfulIds);

        setCreatedOrders((prev) => [...prev, ...remappedWOs]);
        setWorkOrderCounter(currentCounter);
        setPendingRequests((prev) =>
          prev.filter((request) => !successfulIdSet.has(request.manufacturingOrderId))
        );
        setSelectedRequests((prev) => prev.filter((id) => !successfulIdSet.has(id)));
        setActionSuccess(`Đã tạo ${remappedWOs.length} lệnh sản xuất.`);
      }

      if (failedMessages.length > 0) {
        setActionError(`Yêu cầu lỗi: ${failedMessages.join(' | ')}`);
      }
    } catch (err) {
      console.error('Lỗi khi tạo lệnh sản xuất:', getApiPayload(err));
      setActionError(getApiErrorMessage(err, 'Lỗi khi tạo lệnh sản xuất.'));
    } finally {
      setIsCreating(false);
    }
  };

  const handleStartSplit = (wo) => {
    if (!wo || isApproving) return;

    clearActionMessages();
    setSplitTargetWo(wo);
    setSplitAmount('');
    setIsSplitting(true);
  };

  const handleConfirmSplit = () => {
    if (!splitTargetWo || workOrderCounter === null) return;

    const splitQty = Number(splitAmount);
    const originalQty = Number(splitTargetWo.productionTarget);

    if (!Number.isFinite(splitQty) || !Number.isFinite(originalQty) || splitQty <= 0 || splitQty >= originalQty) {
      alert('Số lượng tách không hợp lệ.');
      return;
    }

    const newId = formatWorkOrderId(workOrderCounter);
    const newSplitWo = {
      ...splitTargetWo,
      workOrderId: newId,
      productionTarget: splitQty,
      originOrderId: splitTargetWo.workOrderId,
      bomIds: Array.isArray(splitTargetWo.bomIds) ? [...splitTargetWo.bomIds] : splitTargetWo.bomIds,
    };

    setCreatedOrders((prevOrders) => {
      const targetExists = prevOrders.some((wo) => wo.workOrderId === splitTargetWo.workOrderId);
      if (!targetExists) return prevOrders;

      return prevOrders
        .map((wo) =>
          wo.workOrderId === splitTargetWo.workOrderId
            ? { ...wo, productionTarget: originalQty - splitQty }
            : wo
        )
        .concat(newSplitWo);
    });

    setWorkOrderCounter((prev) => prev + 1);
    setIsSplitting(false);
    setSplitTargetWo(null);
    setSplitAmount('');
    setActionSuccess(`Đã tách ${splitQty} ${splitTargetWo.unit} từ ${splitTargetWo.workOrderId}.`);
  };

  const handleMergeBack = (childWo) => {
    if (!childWo?.originOrderId || isApproving) return;

    clearActionMessages();

    const parentExists = createdOrders.some((wo) => wo.workOrderId === childWo.originOrderId);
    if (!parentExists) {
      setActionError(`Không tìm thấy lệnh gốc ${childWo.originOrderId} để gộp lại.`);
      return;
    }

    setCreatedOrders((prevOrders) =>
      prevOrders
        .map((wo) => {
          if (wo.workOrderId === childWo.originOrderId) {
            return {
              ...wo,
              productionTarget: toNumber(wo.productionTarget) + toNumber(childWo.productionTarget),
            };
          }

          return wo;
        })
        .filter((wo) => wo.workOrderId !== childWo.workOrderId)
    );

    setActionSuccess(`Đã gộp ${childWo.workOrderId} về ${childWo.originOrderId}.`);
  };

  const handleApproveOrders = async () => {
    if (createdOrders.length === 0 || isApproving) return;

    clearActionMessages();
    setIsApproving(true);

    try {
      const ordersToSubmit = createdOrders.map(({ originOrderId, ...rest }) => ({
        ...rest,
        productionTarget: toNumber(rest.productionTarget),
      }));

      const result = await addListWorkOrders(ordersToSubmit);
      const payload = assertSuccessfulResponse(result);
      const failedIds = getFailedWorkOrderIds(payload);

      if (failedIds.length > 0) {
        const failedIdSet = new Set(failedIds);

        setCreatedOrders((prev) => prev.filter((order) => failedIdSet.has(order.workOrderId)));
        setActionError(`Lệnh lỗi: ${failedIds.join(', ')}`);
        return;
      }

      setCreatedOrders([]);
      setActionSuccess(`Duyệt thành công ${ordersToSubmit.length} lệnh sản xuất.`);
    } catch (err) {
      console.error('Lỗi khi duyệt:', getApiPayload(err));

      const failedCodes = createdOrders.map((order) => order.workOrderId).join(', ');
      const message = getApiErrorMessage(err, 'Lỗi khi duyệt lệnh. Vui lòng kiểm tra lại dữ liệu.');

      setActionError(`Lệnh lỗi: ${failedCodes}. ${message}`);
    } finally {
      setIsApproving(false);
    }
  };

  if (loading) {
    return (
      <div className="loading-state">
        <FaSpinner className="spinning" /> Đang tải dữ liệu...
      </div>
    );
  }

  if (error) {
    return <div className="error-state">{error}</div>;
  }

  const splitQtyPreview = Number.isFinite(Number(splitAmount)) ? Number(splitAmount) : 0;
  const splitTargetQty = Number.isFinite(Number(splitTargetWo?.productionTarget))
    ? Number(splitTargetWo.productionTarget)
    : 0;

  return (
    <div className="create-po-container">
      <div className="create-po-header">
        <button onClick={onBack} className="back-btn">
          <FaArrowLeft /> Quay lại
        </button>
        <h2>Quản lý Tạo Lệnh Sản Xuất</h2>
      </div>

      {actionError && <div className="error-state">{actionError}</div>}
      {actionSuccess && <div className="success-state">{actionSuccess}</div>}

      <div className="create-po-content">
        <div className="left-panel">
          <h3>Yêu cầu đang chờ ({pendingRequests.length})</h3>

          <div className="request-list">
            {pendingRequests.length === 0 && (
              <div className="empty-state">Không có yêu cầu đang chờ.</div>
            )}

            {pendingRequests.map((req) => {
              const requestId = req.manufacturingOrderId;

              return (
                <div
                  key={requestId}
                  className={`request-item ${selectedRequests.includes(requestId) ? 'selected' : ''}`}
                  onClick={() => handleSelect(requestId)}
                >
                  <div className="request-info">
                    <strong>{requestId}</strong>
                    <span>
                      {req.materialDefinitionName} - {req.quantity} {req.unit}
                    </span>
                    <small>
                      <FaCalendarAlt /> {formatDate(req.dueDate)}
                    </small>
                  </div>
                </div>
              );
            })}
          </div>

          <button
            className="create-btn"
            onClick={handleCreateOrders}
            disabled={selectedRequests.length === 0 || isCreating || isApproving || workOrderCounter === null}
          >
            {isCreating ? <FaSpinner className="spinning" /> : <FaPlus />} Tạo Lệnh
          </button>
        </div>

        <div className="right-panel">
          <h3>Lệnh sản xuất đã tạo ({createdOrders.length})</h3>

          <div className="order-results">
            {createdOrders.length === 0 && (
              <div className="empty-state">Chưa có lệnh sản xuất nào.</div>
            )}

            {createdOrders.map((wo) => (
              <div
                key={wo.workOrderId}
                className={`order-card ${wo.originOrderId ? 'child-order' : ''}`}
              >
                <div className="order-header">
                  <strong>{wo.workOrderId}</strong>

                  <div className="order-plan-actions">
                    <button
                      className="split-btn"
                      onClick={() => handleStartSplit(wo)}
                      disabled={toNumber(wo.productionTarget) <= 1 || isApproving}
                      title="Tách lệnh"
                    >
                      <FaCut />
                    </button>

                    {wo.originOrderId && (
                      <button
                        className="merge-btn"
                        onClick={() => handleMergeBack(wo)}
                        disabled={isApproving}
                        title="Gộp lại lệnh gốc"
                      >
                        <FaUndo /> Gộp
                      </button>
                    )}
                  </div>
                </div>

                <div className="order-body">
                  <p>YCSX: {wo.manufacturingOrderId}</p>
                  <p>Sản phẩm: {wo.materialDefinitionId}</p>
                  <p className="qty-highlight">
                    Số lượng: <strong>{wo.productionTarget} {wo.unit}</strong>
                  </p>
                  {wo.originOrderId && (
                    <small className="origin-tag">Tách từ: {wo.originOrderId}</small>
                  )}
                </div>
              </div>
            ))}
          </div>

          {createdOrders.length > 0 && (
            <button
              className="approve-btn"
              onClick={handleApproveOrders}
              disabled={isApproving || isCreating}
            >
              {isApproving ? <FaSpinner className="spinning" /> : <FaCheck />} Duyệt & Gửi
            </button>
          )}
        </div>
      </div>

      {isSplitting && splitTargetWo && (
        <div className="split-modal-overlay">
          <div className="split-modal">
            <h3>Tách Lệnh: {splitTargetWo.workOrderId}</h3>
            <p>Số lượng hiện tại: {splitTargetWo.productionTarget} {splitTargetWo.unit}</p>

            <div className="split-input-group">
              <label>Số lượng tách ra:</label>
              <input
                type="number"
                min="0"
                max={splitTargetWo.productionTarget}
                value={splitAmount}
                onChange={(e) => setSplitAmount(e.target.value)}
                placeholder="Ví dụ: 50"
              />
            </div>

            <div className="split-preview">
              <span>Lệnh gốc còn: {splitTargetQty - splitQtyPreview}</span>
              <span>Lệnh mới: {splitQtyPreview}</span>
            </div>

            <div className="split-actions">
              <button
                className="cancel-btn"
                onClick={() => {
                  setIsSplitting(false);
                  setSplitTargetWo(null);
                  setSplitAmount('');
                }}
              >
                Hủy
              </button>
              <button className="confirm-split-btn" onClick={handleConfirmSplit}>
                Xác nhận Tách
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default CreateProductionOrder;
