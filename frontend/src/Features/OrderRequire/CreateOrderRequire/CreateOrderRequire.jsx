import React, { useState, useEffect, useRef, useMemo } from 'react';
import './CreateOrderRequire.css';
import { createManufacturingOrder } from '../../../api/ManufacturingOrders';
import { getOperationsDefinitions } from '../../../api/OperationsDefinition';
import { getCustomerIds } from '../../../api/BusinessPartner';
import { 
  getMaterialDefinitionIds, 
  getMaterialNameandUnit, 
  getQualityStandardByMaterialId 
} from '../../../api/MaterialDefinition';
import toast from 'react-hot-toast';
import debounce from 'lodash/debounce';

const CreateOrderRequire = ({ onCancel, onSuccess }) => {
  const [formData, setFormData] = useState({
    code: '',
    productCode: '',
    productName: '',
    quantity: '',
    deliveryDate: '',
    company: '',
    department: '',
    qualityStandard: '',
    unit: '',
    createdBy: '00001',
    isHighPriority: false,
  });
  const [submitError, setSubmitError] = useState('');
  const [submitSuccess, setSubmitSuccess] = useState('');

  const [pendingOrders, setPendingOrders] = useState([]);
  const [errors, setErrors] = useState({});
  const [isLoading, setIsLoading] = useState(false);
  const [isFetchingDetail, setIsFetchingDetail] = useState(false);

  const [customerList, setCustomerList] = useState([]);
  const [materialList, setMaterialList] = useState([]);
  const [formulaList, setFormulaList] = useState([]);
  const [qualityList, setQualityList] = useState([]);

  const firstInputRef = useRef(null);
  const detailRequestIdRef = useRef(0);
  useEffect(() => {
    const load = async () => {
      try {
        const [customers, materials] = await Promise.all([getCustomerIds(), getMaterialDefinitionIds()]);
        if (Array.isArray(customers)) setCustomerList(customers);
        if (Array.isArray(materials)) setMaterialList(materials);
      } catch { toast.error('Lỗi tải dữ liệu'); }
    };
    load();
  }, []);
  const handleChange = (e) => {
  const { name, value, type, checked } = e.target;

  setFormData(prev => ({
    ...prev,
    [name]: type === 'checkbox' ? checked : value
  }));

  if (errors[name]) {
    setErrors(prev => ({ ...prev, [name]: undefined }));
  }
};
const validateForm = () => {
    const e = {};
    if (!formData.code.trim()) e.code = 'Bắt buộc';
    if (!formData.productCode.trim()) e.productCode = 'Bắt buộc';
    const quantity = Number(formData.quantity);
    if (!Number.isInteger(quantity) || quantity <= 0) {e.quantity = 'Số lượng phải là số nguyên > 0';}
    if (!formData.deliveryDate) e.deliveryDate = 'Bắt buộc';
    if (!formData.company.trim()) e.company = 'Bắt buộc';
    if (pendingOrders.some(o => o.code === formData.code.trim())) e.code = 'Mã đã tồn tại';
    return e;
  };
  useEffect(() => firstInputRef.current?.focus(), []);

  // LOAD DATA HỆ THỐNG
  

  const fetchAllDetails = useMemo(
  () =>
    debounce(async (materialId, requestId) => {
      if (!materialId) return;

      setIsFetchingDetail(true);

      try {
        const [detail, formulas, qualities] = await Promise.all([
          getMaterialNameandUnit(materialId),
          getOperationsDefinitions(materialId),
          getQualityStandardByMaterialId(materialId),
        ]);

        if (requestId !== detailRequestIdRef.current) return;

        setFormData(prev => ({
          ...prev,
          productName: detail?.name || '',
          unit: detail?.unitOfMeasure || '',
          department: formulas?.[0] || '',
          qualityStandard: qualities?.[0] || '',
        }));

        setFormulaList(formulas || []);
        setQualityList(qualities || []);
      } catch {
        toast.error('Lỗi tải thông tin sản phẩm');
      } finally {
        if (requestId === detailRequestIdRef.current) {
          setIsFetchingDetail(false);
        }
      }
    }, 600),
  []
);

  useEffect(() => {
  const requestId = ++detailRequestIdRef.current;

  setFormData(prev => ({
    ...prev,
    productName: '',
    unit: '',
    department: '',
    qualityStandard: '',
  }));
  setFormulaList([]);
  setQualityList([]);

  if (!formData.productCode) {
    fetchAllDetails.cancel();
    setIsFetchingDetail(false);
    return;
  }
  fetchAllDetails(formData.productCode, requestId);
  return () => {
    fetchAllDetails.cancel();
  };
}, [formData.productCode, fetchAllDetails]);
  const handleAddOrder = (e) => {
    e.preventDefault();
    const errs = validateForm();
    if (Object.keys(errs).length > 0) { setErrors(errs); return; }

    setPendingOrders(prev => [
  ...prev,
  {
    ...formData,
    code: formData.code.trim(),
    productCode: formData.productCode.trim(),
    company: formData.company.trim(),
    createdBy: formData.createdBy.trim(),
  },
]);

    setFormData(prev => ({ ...prev, code: '', quantity: '' })); 
    firstInputRef.current?.focus();
    toast.success('Đã thêm vào danh sách');
  };

  const handleFinalSubmit = async () => {
  if (pendingOrders.length === 0) return;

  setIsLoading(true);

  let successCount = 0;
  const failedOrders = [];

  for (const order of pendingOrders) {
    try {
      const payload = {
  manufacturingOrderId: order.code.trim(),
  materialDefinitionId: order.productCode.trim(),
  quantity: Number(order.quantity),
  unit: order.unit.trim(),
  dueDate: new Date(`${order.deliveryDate}T00:00:00.000Z`).toISOString(),
  customerId: order.company.trim(),
  operationsDefinitionId: order.department.trim(),
  qualityStandardId: order.qualityStandard.trim(),
  createdById: order.createdBy.trim(),
  priority: order.isHighPriority ? 100 : 1,
};

      const result = await createManufacturingOrder(payload);

      if (result?.errorCode) {
        throw result;
      }

      successCount++;
    } catch {
      failedOrders.push(order);
    }
  }

  setPendingOrders(failedOrders);
  setIsLoading(false);

  if (failedOrders.length > 0) {
  const failedCodes = failedOrders.map(order => order.code).join(', ');
  setSubmitError(`Lệnh lỗi: ${failedCodes}`);
} else {
  setSubmitError('');
}

if (successCount > 0) {
  setSubmitSuccess(`Đã tạo ${successCount} yêu cầu!`);
} else {
  setSubmitSuccess('');
}
};

  return (
    <div className="create-order-container">
      <h2 className="title">Yêu cầu sản xuất</h2>
      
      <form onSubmit={handleAddOrder} className="create-order-form">
        <div className="form-group">
          <label className="form-label">Mã yêu cầu</label>
          <input ref={firstInputRef} type="text" name="code" value={formData.code} onChange={handleChange} className={`form-input ${errors.code ? 'error' : ''}`} placeholder="YCSX..." />
          {errors.code && <p className="form-error">{errors.code}</p>}
        </div>

        <div className="form-group">
          <label className="form-label">Mã sản phẩm</label>
          <select name="productCode" value={formData.productCode} onChange={handleChange} className={`form-input ${errors.productCode ? 'error' : ''}`}>
            <option value="">-- Chọn mã --</option>
            {materialList.map(id => <option key={id} value={id}>{id}</option>)}
          </select>
          {errors.productCode && <p className="form-error">{errors.productCode}</p>}
        </div>

        <div className="form-group">
          <label className="form-label">Tên sản phẩm</label>
          <input type="text" value={formData.productName} readOnly className="form-input read-only" />
        </div>

        <div className="form-group">
          <label className="form-label">Số lượng & Đơn vị</label>
          <div style={{ display: 'flex', gap: '8px' }}>
            <input type="number" name="quantity" value={formData.quantity} onChange={handleChange} className={`form-input ${errors.quantity ? 'error' : ''}`} style={{ flex: 1 }}/>
            {errors.quantity && <p className="form-error">{errors.quantity}</p>}
            <input type="text" value={formData.unit} readOnly className="form-input read-only" style={{ width: '65px' }} />
          </div>
        </div>

        <div className="form-group">
          <label className="form-label">Công thức</label>
          <select name="department" value={formData.department} onChange={handleChange} className="form-input">
            {formulaList.map(f => <option key={f} value={f}>{f}</option>)}
          </select>
        </div>

        <div className="form-group">
          <label className="form-label">Tiêu chuẩn</label>
          <select name="qualityStandard" value={formData.qualityStandard} onChange={handleChange} className="form-input">
            {qualityList.map(q => <option key={q} value={q}>{q}</option>)}
          </select>
        </div>

        <div className="form-group">
          <label className="form-label">Hạn giao</label>
          <input type="date" name="deliveryDate" value={formData.deliveryDate} onChange={handleChange} className={`form-input ${errors.deliveryDate ? 'error' : ''}`}/>{errors.deliveryDate && <p className="form-error">{errors.deliveryDate}</p>}
        </div>
        <div className="form-group">
  <label className="form-label">Độ ưu tiên</label>
  <label style={{ display: 'flex', alignItems: 'center', gap: '8px', height: '38px' }}>
    <input
      type="checkbox"
      name="isHighPriority"
      checked={formData.isHighPriority}
      onChange={handleChange}
    />
    Đơn hàng ưu tiên cao
  </label>
</div>
        <div className="form-group">
          <label className="form-label">Khách hàng</label>
          <select name="company" value={formData.company} onChange={handleChange} className={`form-input ${errors.company ? 'error' : ''}`}>
            <option value="">-- Khách hàng --</option>
            {customerList.map(id => <option key={id} value={id}>{id}</option>)}
          </select>
          {errors.company && <p className="form-error">{errors.company}</p>}
        </div>

        <div className="form-group">
          <label className="form-label">Người tạo</label>
          <input type="text" name="createdBy" value={formData.createdBy} onChange={handleChange} className="form-input" />
        </div>
        <div className="form-group btn-add-wrapper">
          <label className="form-label">&nbsp;</label>
          <button type="submit" className="mini-add-btn" title="Thêm vào danh sách chờ">
             + Thêm vào danh sách
          </button>
        </div>
      </form>
      {/* Nút thêm nhanh đặt trong Grid để không tốn diện tích */}
        
      {/* DANH SÁCH HIỂN THỊ PHÍA DƯỚI */}
      <div className="pending-section">
        <div className="pending-header">
          <h3 className="pending-title">Danh sách chờ ({pendingOrders.length})</h3>
          
          {pendingOrders.length > 0 && (
            <button type="button" onClick={handleFinalSubmit} className={`action-btn submit-btn ${isLoading ? 'loading' : ''}`} disabled={isLoading}>
              {isLoading ? 'Đang xử lý...' : 'Xác nhận & Tạo yêu cầu'}
            </button>
          )}
        </div>
          {submitError && <p className="form-error">{submitError}</p>}
          {submitSuccess && <p className="success-text">{submitSuccess}</p>}
        <div className="pending-list">
          {pendingOrders.map((order) => (
            <div key={order.code} className="order-badge">
              <span className="badge-code">{order.code}</span>
              <span className="badge-info">({order.quantity} {order.unit})</span>
              {order.isHighPriority && (
  <span style={{ color: '#dc2626', fontWeight: 700, marginLeft: 8 }}>
    Ưu tiên
  </span>
)}
              <button type="button" className="remove-badge" onClick={() => setPendingOrders(pendingOrders.filter(o => o.code !== order.code))}>✕</button>
            </div>
          ))}
          {pendingOrders.length === 0 && <p className="empty-text">Chưa có đơn hàng nào trong danh sách chờ...</p>}
        </div>
      </div>
    </div>
  );
};

export default CreateOrderRequire;