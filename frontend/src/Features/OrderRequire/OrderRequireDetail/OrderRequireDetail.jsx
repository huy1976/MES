import React, { useState, useEffect } from 'react';
import './OrderRequireDetail.css';
import ManufacturingOrderDetail from '../../../api/ManufacturingOrderDetail';

const OrderRequireDetail = ({ order }) => {
  const [subOrders, setSubOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Map trạng thái API → tiếng Việt
  const statusMap = {
    PendingScheduling: 'Đang chờ điều độ',
    Scheduled: 'Đang điều độ',
    InProduction: 'Đang thực thi',
    ReadyForProduction: 'Chờ sản xuất',
    Completed: 'Đã hoàn thành',
    Cancelled: 'Đã hủy',
    Pending: 'Chờ xử lý' 
  };

  const statusColors = {
    'Chờ xử lý': '#95a5a6',
    'Đang chờ điều độ': '#C11DEB',
    'Đang thực thi': '#3498db',
    'Chờ sản xuất': '#f39c12',
    'Đã hoàn thành': '#2ecc71',
    'Đã hủy': '#e74c3c',
  };

  useEffect(() => {
    if (!order?.code) return;

    const fetchSubOrders = async () => {
      try {
        setLoading(true);
        setError(null);
        const response = await ManufacturingOrderDetail.getOrderDetail(order.code);
        
        const workOrders = response.data.value.workOrderDTOs || [];
        
        const formattedSubOrders = workOrders.map((wo, index) => ({
          id: wo.workOrderId,
          processCode: wo.segmentId,
          process: wo.segmentName,
          startDate: formatDate(wo.start),
          endDate: formatDate(wo.end),
          machine: wo.equipments?.length > 0 
              ? wo.equipments.map(e => e.equipmentId).join(', ') 
              : 'Chưa phân bổ',
          product: response.data.value.materialDefinitionName,
          productionSpec: wo.productionParamId,
          label: 'N/A',
          quantity: `${wo.productionTarget} ${wo.unit}`,
          qualitySpec: wo.qualityParamId,
          material: wo.materials?.map(m => m.materialDefinitionId).join(', ') || '',
          status: statusMap[wo.status] || wo.status,
        }));

        setSubOrders(formattedSubOrders);
      } catch (err) {
        console.error('Error fetching order detail:', err);
        setError('Không thể tải chi tiết lệnh sản xuất');
      } finally {
        setLoading(false);
      }
    };

    fetchSubOrders();
  }, [order?.code]);

  // Helper functions
  const formatDate = (dateString) => {
    if (!dateString || dateString.includes('0001-01-01')) return 'Chưa xác định';
    return new Date(dateString).toLocaleDateString('vi-VN');
  };

  const getProcessName = (segmentId) => {
    const names = {
      'KEO01': 'Kéo dây',
      'TRANG01': 'Tráng men',
      'MENHS01': 'Men HS',
      // Thêm nếu cần
    };
    return names[segmentId] || segmentId;
  };

  return (
    <div className="order-detail-container">
      <h2 className="detail-title">Thông tin chi tiết lệnh sản xuất: {order.code}</h2>

      {/* Main Info - Từ props */}
      {/* Main Info - ĐÃ ĐỒNG BỘ HOÀN TOÀN VỚI BẢNG DANH SÁCH */}
<div className="main-info">
  <div className="header-row">
    <span>STT</span>
    <span>Mã YCSX</span>
    <span>Mã SP</span>
    <span>Tên sản phẩm</span>
    <span>Số lượng</span>
    <span>Ngày tạo</span>
    <span>Hạn giao</span>
    <span>Khách hàng</span>
    <span>Công đoạn</span>
    <span>Tiêu chuẩn</span>
    <span>Trạng thái</span>
    <span>Người tạo</span>
  </div>
  <div className="data-row">
    <span>{order.id}</span>
    <span><strong>{order.code}</strong></span>
    <span>{order.productCode}</span>
    <span>{order.productName}</span>
    <span>{order.quantity}</span>
    <span>{order.createdDate}</span>
    <span>{order.deliveryDate}</span>
    <span>{order.company}</span>
    <span>{order.department}</span>
    <span>{order.unit}</span>
    <span>
      <span 
        className="status" 
        style={{ 
          backgroundColor: statusColors[order.status] || '#95a5a6',
          padding: '6px 12px',
          borderRadius: '20px',
          fontSize: '11px',
          fontWeight: '600',
          color: 'white',
          display: 'inline-block',
          minWidth: '90px',
          textAlign: 'center'
        }}
      >
        {order.status}
      </span>
    </span>
    <span>{order.createdBy}</span>
  </div>
</div>

      {/* Sub Orders */}
      <div className="sub-orders-section">
        <h3 className="sub-title">Các lệnh sản xuất chi tiết ({subOrders.length})</h3>

        {loading && (
          <div className="loading-skeleton">
            {[1, 2, 3].map((i) => (
              <div key={i} className="skeleton-card">
                <div className="skeleton-header skeleton-line"></div>
                <div className="skeleton-body">
                  <div className="skeleton-line short"></div>
                  <div className="skeleton-line"></div>
                  <div className="skeleton-line medium"></div>
                  <div className="skeleton-line short"></div>
                </div>
              </div>
            ))}
          </div>
        )}

        {error && (
          <div className="error-message">
            {error}
            <button onClick={() => window.location.reload()} className="retry-btn">
              Thử lại
            </button>
          </div>
        )}

        {!loading && !error && subOrders.length === 0 && (
          <div className="no-data-message">Chưa có lệnh sản xuất chi tiết</div>
        )}

        {!loading && !error && subOrders.length > 0 && (
          <div className="sub-orders">
            {subOrders.map((subOrder) => (
              <div key={subOrder.id} className="sub-order-card">
                <div className="sub-order-header">
                  {subOrder.id}
                  <span className="status-badge" style={{ backgroundColor: statusColors[subOrder.status] }}>
                    {subOrder.status}
                  </span>
                </div>
                <div className="sub-order-details">
                  <div><strong>Mã công đoạn:</strong> {subOrder.processCode}</div>
                  <div><strong>Tên công đoạn:</strong> {subOrder.process}</div>
                  <div><strong>Sản phẩm:</strong> {subOrder.product}</div>
                  <div><strong>Số lượng:</strong> {subOrder.quantity}</div>
                  <div><strong>Máy:</strong> {subOrder.machine}</div>
                  <div><strong>Thông số SX:</strong> {subOrder.productionSpec}</div>
                  <div><strong>Tiêu chuẩn CL:</strong> {subOrder.qualitySpec}</div>
                  <div><strong>Nguyên liệu:</strong> {subOrder.material}</div>
                  <div><strong>Ngày bắt đầu:</strong> {subOrder.startDate}</div>
                  <div><strong>Ngày kết thúc:</strong> {subOrder.endDate}</div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default OrderRequireDetail;