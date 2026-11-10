// File: ProgressUpdateModal.jsx
import React, { useState, useEffect } from 'react';
// Import API trực tiếp vào Modal
import { postWorkOrderProgressRecord, postWorkOrderErrorRecord, getReviewNewScheduleTimeForWo } from '../../api/WorkOrderProcess';
// Import UI Components
import CreateButton from '../../common/components/Button/CreateButton';
import FormGroup from '../../common/components/FormGroup';
import { Label } from '../../common/components/Label';
import styles from './ProgressUpdateModal.module.scss'; 

const STATUS_MAPPING = {
    Active: { label: 'Hoạt động', className: 'active' },
    Pending: { label: 'Chờ xử lý', className: 'pending' },
    PendingScheduling: { label: 'Chờ lập kế hoạch', className: 'pendingScheduling' },
    Planned: { label: 'Đã lên kế hoạch', className: 'planned' },
    ReadyForProduction: { label: 'Chờ sản xuất', className: 'readyForProduction' },
    InProduction: { label: 'Đang thực thi', className: 'inProduction' },
    Completed: { label: 'Đã hoàn thành', className: 'completed' },
    Cancelled: { label: 'Đã hủy', className: 'cancelled' },
    Available: { label: 'Sẵn sàng', className: 'available' }
};

// --- Helper Functions ---
// Hàm này vẫn giữ nguyên, dùng để chuyển đổi từ ISO String (UTC từ server) sang Local Time String (dành cho input)
const toDateTimeLocal = (isoString) => {
    if (!isoString) return '';
    const date = new Date(isoString);
    if (isNaN(date.getTime())) return '';
    
    const offset = date.getTimezoneOffset() * 60000;
    const localTime = new Date(date.getTime() - offset).toISOString().slice(0, 16);
    return localTime;
};

// Hàm này đã sửa: chuyển từ Local Time String (input) sang Chuỗi ISO KÈM MÚI GIỜ (để gửi lên server)
const getISOString = (dateTimeLocal) => {
    if (!dateTimeLocal) return null;
    
    // 1. Tách chuỗi input (ví dụ: "2025-11-29T13:00") thành các thành phần
    const parts = dateTimeLocal.match(/(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2})/);
    if (!parts) return null;

    // Lấy các thành phần: Năm, Tháng, Ngày, Giờ, Phút (Lưu ý: Tháng là index 1)
    const year = parseInt(parts[1], 10);
    const month = parseInt(parts[2], 10) - 1; // Tháng (0-11)
    const day = parseInt(parts[3], 10);
    const hour = parseInt(parts[4], 10);
    const minute = parseInt(parts[5], 10);

    // 2. TẠO ĐỐI TƯỢNG DATE BẰNG CÁCH DÙNG CÚ PHÁP LOCAL (YYYY, MM, DD, HH, mm)
    // Cú pháp này bắt buộc tạo ra đối tượng Date ở múi giờ LOCAL (13:25)
    const date = new Date(year, month, day, hour, minute, 0, 0); 
    
    // 3. Tính toán Offset múi giờ địa phương (theo phút)
    const offsetMinutes = date.getTimezoneOffset(); 
    const sign = offsetMinutes < 0 ? '+' : '-';
    
    // 4. Chuyển đổi offset phút sang định dạng HH:mm
    const absOffset = Math.abs(offsetMinutes);
    const offsetHours = Math.floor(absOffset / 60).toString().padStart(2, '0');
    const offsetMins = (absOffset % 60).toString().padStart(2, '0');
    
    // 5. Lấy phần ngày giờ cơ bản (YYYY-MM-DDTHH:mm:ss.sss)
    // Lúc này, date.toISOString() sẽ trả về UTC 06:25Z (Đúng)
    const baseISO = date.toISOString(); 
    
    // 6. Kết hợp: Cắt bỏ đuôi 'Z' và thêm Offset múi giờ Local (13:25+07:00)
    // Chúng ta đã sửa lại phép tính offset, nên kết quả cuối cùng sẽ là 13:25+07:00
    // LƯU Ý: Tuy nhiên, để đảm bảo code chạy ổn định và theo tiêu chuẩn hơn, ta nên sử dụng các thành phần Local trực tiếp, không dựa vào date.toISOString()
    
    // --- KHUYẾN NGHỊ: CÁCH TẠO CHUỖI ISO LOCAL TƯỜNG MINH ---
    const YYYY = year.toString();
    const MM = (month + 1).toString().padStart(2, '0');
    const DD = day.toString().padStart(2, '0');
    const HH = hour.toString().padStart(2, '0');
    const mm = minute.toString().padStart(2, '0');
    const ss = '00';
    const sss = '000'; // Có thể bỏ nếu server không yêu cầu milliseconds
    
    return `${YYYY}-${MM}-${DD}T${HH}:${mm}:${ss}.${sss}${sign}${offsetHours}:${offsetMins}`;
};

// Helper format hiển thị ngày giờ cho bảng Review
const formatDisplayDate = (isoString) => {
    if (!isoString) return '--';
    const date = new Date(isoString);
    // Sử dụng .toLocaleString để hiển thị giờ địa phương
    return date.toLocaleString('vi-VN', { day: '2-digit', month: '2-digit', hour: '2-digit', minute: '2-digit' });
};

// State khởi tạo cho Sự cố
const initialIncidentState = {
    errorId: '', 
    errorName: '', 
    downtimeType: 0, 
    notes: '', 
    handledBy: '', 
    reportedBy: '', 
    start: '', // SẼ LƯU LOCAL TIME STRING
    end: '', // SẼ LƯU LOCAL TIME STRING
    workOrderId: '', 
    isUpdateTime: false, 
    isSchedule: false 
};

const ProgressUpdateModal = ({
    isOpen,
    selectedOrder,
    activeTab,
    setActiveTab,
    onClose,
    onSuccess,      
    onUpdateStatus, 
}) => {
    // === LOCAL STATE QUẢN LÝ DỮ LIỆU FORM ===
    const [loading, setLoading] = useState(false);

    // State cho chức năng Review Dời lịch
    const [scheduleReviewData, setScheduleReviewData] = useState(null);
    const [isReviewLoading, setIsReviewLoading] = useState(false);

    // 1. State cho Tab Tiến độ
    const [progressData, setProgressData] = useState({
        deltaQuantity: '0',
        deltaErrorQuantity: '0',
        deltaRawConsumedQuatity: '0',
        selectedEmployee: '',
        selectedDate: '' // SẼ LƯU LOCAL TIME STRING
    });

    // 2. State cho Tab Sự cố
    const [incidentData, setIncidentData] = useState(initialIncidentState);

    // === EFFECT: Reset form khi mở Modal ===
    useEffect(() => {
        if (isOpen && selectedOrder) {
            // 💡 ĐIỂM SỬA 1: Lấy thời gian hiện tại và chuyển sang Local Time Format (YYYY-MM-DDThh:mm)
            const nowISOTime = new Date().toISOString();
            const localTimeFormat = toDateTimeLocal(nowISOTime); 
            
            // Bỏ đoạn tính offset thủ công và sử dụng helper function để đảm bảo đúng format
            // const now = new Date();
            // const offsetMs = now.getTimezoneOffset() * 60000;
            // const localISOTime = new Date(now.getTime() - offsetMs).toISOString().slice(0, 16); // BỎ ĐOẠN CODE NÀY

            const defaultPerson = selectedOrder.originalData?.persons?.[0]?.personId || '';

            // Reset Form Tiến độ
            setProgressData({
                deltaQuantity: '',
                deltaErrorQuantity: '0',
                deltaRawConsumedQuatity: '0',
                selectedEmployee: defaultPerson,
                selectedDate: localTimeFormat // Lưu chuỗi Local Time Format
            });

            // Reset Form Sự cố
            setIncidentData({
                ...initialIncidentState,
                workOrderId: selectedOrder.workOrderId,
                start: localTimeFormat, // Lưu chuỗi Local Time Format
                end: '', 
                reportedBy: defaultPerson,
                handledBy: defaultPerson
            });
            
            // Reset review data
            setScheduleReviewData(null);
        }
    }, [isOpen, selectedOrder]);

    if (!isOpen) return null;

    // === HANDLERS CHO TAB TIẾN ĐỘ ===
    const handleProgressChange = (field, value) => {
        setProgressData(prev => ({ ...prev, [field]: value }));
    };

    const handleSaveProgress = async () => {
        const qty = parseFloat(progressData.deltaQuantity);
        const errorQty = parseFloat(progressData.deltaErrorQuantity);
        const rawConsumedQty = parseFloat(progressData.deltaRawConsumedQuatity);

        if (isNaN(qty) || qty <= 0) return alert("Vui lòng nhập số lượng sản xuất thêm hợp lệ (> 0).");
        if (isNaN(errorQty) || errorQty < 0) return alert("Vui lòng nhập số lượng lỗi hợp lệ (>= 0).");
        if (!progressData.selectedEmployee) return alert("Vui lòng chọn nhân viên thực hiện.");
        if (!progressData.selectedDate) return alert("Vui lòng chọn thời gian cập nhật.");

        setLoading(true);
        try {
            const payload = {
                workOrderId: String(selectedOrder.workOrderId),
                deltaQuantityProduced: qty,
                deltaQuantityError: errorQty,
                deltaRawConsumedQuantity: rawConsumedQty,
                recordedBy: progressData.selectedEmployee,
                // Chuyển Local Time String sang ISO String (UTC) khi gửi lên API
                timestamp: getISOString(progressData.selectedDate),
            };

            await postWorkOrderProgressRecord(payload);
            alert("Cập nhật tiến độ thành công!");
            onSuccess(); 
            onClose();   
        } catch (error) {
            console.error("Lỗi save progress:", error);
            alert("Có lỗi khi lưu tiến độ: " + (error.response?.data?.message || error.message));
        } finally {
            setLoading(false);
        }
    };

    // === HANDLERS CHO TAB SỰ CỐ ===
    const handleIncidentChange = (field, value) => {
        // [UPDATE] Logic tự động set boolean khi đổi loại ảnh hưởng
        if (field === 'downtimeType') {
            const type = value;
            let newIsUpdateTime = false;
            let newIsSchedule = false;

            if (type === 1) {
                // Loại 1: Mặc định tick "Cập nhật thời gian"
                newIsUpdateTime = true;
            } else if (type === 2) {
                // Loại 2: Mặc định tick cả hai
                newIsUpdateTime = true;
                newIsSchedule = true;
            }
            // Loại 0: Cả hai là false

            setIncidentData(prev => ({ 
                ...prev, 
                [field]: value,
                isUpdateTime: newIsUpdateTime,
                isSchedule: newIsSchedule
            }));

            // Reset dữ liệu review nếu không phải loại 1
            if (value !== 1) {
                setScheduleReviewData(null);
            }
        } else {
            setIncidentData(prev => ({ ...prev, [field]: value }));
        }
    };

    // [UPDATE] Hàm xử lý checkbox: Nhận e.target.checked (true/false)
    const handleCheckboxChange = (field, e) => {
        handleIncidentChange(field, e.target.checked);
    };

    // Hàm tính toán thời gian chênh lệch (giờ)
    const calculateDurationInHours = () => {
        if (!incidentData.start || !incidentData.end) return 0;
        
        // Chú ý: Khi tạo Date object từ Local Time String, nó sẽ được hiểu là giờ địa phương.
        const start = new Date(incidentData.start);
        const end = new Date(incidentData.end);
        
        const diffMs = end - start;
        if (diffMs <= 0) return 0;
        
        const hours = diffMs / (1000 * 60 * 60);
        return hours;
    };

    // Hàm gọi API xem trước lịch
    const handleCheckSchedule = async () => {
        if (!selectedOrder?.workOrderId) return;

        const duration = calculateDurationInHours();
        if (duration <= 0) {
            alert("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
            return;
        }
        
        setIsReviewLoading(true);
        try {
            const data = await getReviewNewScheduleTimeForWo(selectedOrder.workOrderId, duration);
            setScheduleReviewData(data); 
        } catch (error) {
            console.error("Lỗi lấy thông tin điều độ:", error);
            alert("Không thể lấy thông tin điều độ lại.");
        } finally {
            setIsReviewLoading(false);
        }
    };

    const handleSaveIncident = async () => {
        if (!incidentData.errorName || !incidentData.start) {
            return alert("Vui lòng nhập tên sự cố và thời gian bắt đầu.");
        }

        setLoading(true);
        try {
            // [UPDATE] Logic kiểm tra hiển thị để đảm bảo dữ liệu gửi đi chính xác
            const showIsUpdateTime = incidentData.downtimeType !== 0;
            const showIsSchedule = incidentData.downtimeType === 2 || incidentData.downtimeType === 3;

            const payload = {
                ...incidentData,
                workOrderId: String(selectedOrder.workOrderId),
                // 💡 ĐIỂM SỬA 3: Chuyển đổi từ Local Time String trong state sang ISO String (UTC) khi gửi đi
                start: getISOString(incidentData.start),
                end: incidentData.end ? getISOString(incidentData.end) : getISOString(new Date(0).toISOString()),
                
                // [UPDATE] Ép giá trị boolean: Nếu UI ẩn (show... = false) thì gửi false, ngược lại gửi giá trị trong state
                isUpdateTime: showIsUpdateTime ? (incidentData.isUpdateTime === true) : false,
                isSchedule: showIsSchedule ? (incidentData.isSchedule === true) : false
            };

            await postWorkOrderErrorRecord(payload);
            alert("Ghi nhận sự cố thành công!");
            onSuccess(); 
            onClose();   
        } catch (error) {
            console.error("Lỗi save incident:", error);
            alert("Có lỗi khi ghi nhận sự cố: " + (error.response?.data?.message || error.message));
        } finally {
            setLoading(false);
        }
    };

    // === LOGIC GIAO DIỆN ===
    // 💡 ĐIỂM SỬA 2: Lấy trực tiếp từ state vì state đã lưu Local Time String
    // BỎ toDateTimeLocal() vì dữ liệu trong state đã là Local Time String
    const incidentStartDateTimeLocal = incidentData.start; 
    const incidentEndDateTimeLocal = incidentData.end; 
    
    const durationInHours = calculateDurationInHours();
    const isDurationValid = durationInHours > 0;

    const downtimeType = incidentData.downtimeType;
    let isUpdateTimeLabel = 'Có cập nhật lại thời gian không?';
    let isScheduleLabel = 'Có điều độ lại không?';

    if (downtimeType === 2 || downtimeType === 3) {
        isUpdateTimeLabel = 'Có điều độ lại không?';
        isScheduleLabel = 'Có điều độ lại không?';
    }
    const showIsUpdateTime = downtimeType !== 0;
    const showIsSchedule = downtimeType === 2 || downtimeType === 3;

    return (
        <div className={styles.overlay} onClick={onClose}>
            <div className={styles.modal} onClick={(e) => e.stopPropagation()}>
                <button className={styles.closeButton} onClick={onClose}>&times;</button>
                <div className={styles.modalHeader}>
                    <button className={`${styles.modalTab} ${activeTab === 'tien-do' ? styles.active : ''}`} onClick={() => setActiveTab('tien-do')}>Tiến độ</button>
                    <button className={`${styles.modalTab} ${activeTab === 'su-co' ? styles.active : ''}`} onClick={() => setActiveTab('su-co')}>Sự cố</button>
                </div>

                {/* --- TAB TIẾN ĐỘ --- */}
                {activeTab === 'tien-do' && (
                    <div className={styles.modalBody}>
                        <h4 className={styles.incidentTitle}>Ghi nhận tiến độ cho {selectedOrder?.code}</h4>
                        <div className={styles.infoBox}>
                             <div className={styles.twoColGrid}>
                                <FormGroup>
                                    <Label>Trạng thái</Label>
                                    <div className={STATUS_MAPPING[selectedOrder?.status]?.className}>
                                        {selectedOrder?.statusText}
                                    </div>
                                </FormGroup>
                                <FormGroup><Label>Sản phẩm</Label><div style={{fontWeight: 600, fontSize: '14px'}}>{selectedOrder?.productId || '--'}</div></FormGroup>
                             </div>
                             <div style={{borderTop: '1px dashed #cbd5e1', margin: '4px 0'}}></div>
                             <div className={styles.twoColGrid}>
                                <FormGroup><Label>Mã lệnh (LSX)</Label><input type="text" value={selectedOrder?.code || ''} disabled /></FormGroup>
                                <FormGroup><Label>Mã MO</Label><input type="text" value={selectedOrder?.moCode || ''} disabled /></FormGroup>
                             </div>
                        </div>

                        <div className={styles.inputSection}>
                            <div className={styles.twoColGrid}>
                                <FormGroup>
                                    <Label>Thời gian cập nhật</Label>
                                    {/* Lấy trực tiếp từ state (Local Time String) */}
                                    <input type="datetime-local" value={progressData.selectedDate} onChange={(e) => handleProgressChange('selectedDate', e.target.value)} />
                                </FormGroup>
                                <FormGroup>
                                    <Label>Nhân viên thực hiện</Label>
                                    <select className={styles.customSelect} value={progressData.selectedEmployee} onChange={(e) => handleProgressChange('selectedEmployee', e.target.value)}>
                                        <option value="">-- Chọn nhân viên --</option>
                                        {selectedOrder?.originalData?.persons?.map((p, idx) => (
                                            <option key={idx} value={p.personId}>{p.personId} - {p.className} ({p.shift})</option>
                                        ))}
                                    </select>
                                </FormGroup>
                            </div>
                            <div className={styles.twoColGrid}>
                                <FormGroup>
                                    <Label>Số lượng hoàn thành thêm</Label> 
                                    <input type="number" placeholder="Nhập số lượng..." value={progressData.deltaQuantity} onChange={(e) => handleProgressChange('deltaQuantity', e.target.value)} min="0" style={{fontWeight: 'bold', color: '#3b82f6'}} />
                                </FormGroup>
                                <FormGroup>
                                    <Label>Số lượng bị lỗi</Label> 
                                    <input type="number" placeholder="Nhập số lượng..." value={progressData.deltaErrorQuantity} onChange={(e) => handleProgressChange('deltaErrorQuantity', e.target.value)} min="0" style={{fontWeight: 'bold', color: '#ef4444'}} />
                                </FormGroup>
                                {/* <FormGroup>
                                    <Label>Số lượng nguyên liệu tiêu hao</Label>
                                    <input type="number" placeholder="Nhập số lượng..." value={progressData.deltaRawConsumedQuantity} onChange={(e) => handleProgressChange('deltaRawConsumedQuantity', e.target.value)} min="0" style={{fontWeight: 'bold', color: '#10b981'}} />
                                </FormGroup> */}
                                <FormGroup>
                                    <Label>Tiến độ lũy kế (Hiện tại)</Label> 
                                    <div style={{padding: '10px 12px', backgroundColor: '#f1f5f9', borderRadius: '6px', fontSize: '14px', color: '#64748b'}}>
                                        <strong>{selectedOrder?.completed + selectedOrder?.errorQuantity}</strong> / {selectedOrder?.total} {selectedOrder?.unit}
                                    </div>
                                </FormGroup>
                            </div>
                        </div>
                    </div>
                )}
                
                {/* --- TAB SỰ CỐ --- */}
                {activeTab === 'su-co' && (
                    <div className={styles.incidentBody}>
                        <h4 className={styles.incidentTitle}>Ghi nhận sự cố cho {selectedOrder?.code}</h4>
                        <div className={styles.incidentFormGrid}>
                            <FormGroup>
                                <Label>Mã sự cố</Label>
                                <input type="text" placeholder="ERROR-001" value={incidentData.errorId || ''} onChange={(e) => handleIncidentChange('errorId', e.target.value)} />
                            </FormGroup>
                            <FormGroup>
                                <Label>Tên sự cố</Label>
                                <input type="text" placeholder="VD: Máy cắt hỏng" value={incidentData.errorName || ''} onChange={(e) => handleIncidentChange('errorName', e.target.value)} />
                            </FormGroup>
                            <FormGroup>
                                <Label>Mức độ ảnh hưởng</Label>
                                <select className={styles.customSelect} value={incidentData.downtimeType} onChange={(e) => handleIncidentChange('downtimeType', parseInt(e.target.value))}>
                                    <option value={0}>Ảnh hưởng ít</option>
                                    <option value={1}>Ảnh hưởng tương đối (Dời lịch)</option>
                                    <option value={2}>Ảnh hưởng nhiều (Dời lịch và Điều độ)</option>
                                </select>
                            </FormGroup>

                            {/* --- PHẦN REVIEW DỜI LỊCH --- */}
                            {incidentData.downtimeType === 1 && (
                                <div style={{ gridColumn: '1 / span 3', marginTop: '-10px', marginBottom: '10px' }}>
                                    <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginBottom: '10px' }}>
                                        <button 
                                            type="button"
                                            onClick={handleCheckSchedule}
                                            disabled={isReviewLoading || !isDurationValid}
                                            style={{
                                                padding: '6px 12px',
                                                backgroundColor: (isReviewLoading || !isDurationValid) ? '#94a3b8' : '#eab308', 
                                                color: 'white',
                                                border: 'none',
                                                borderRadius: '4px',
                                                cursor: (isReviewLoading || !isDurationValid) ? 'not-allowed' : 'pointer',
                                                fontSize: '13px',
                                                fontWeight: 600,
                                                transition: 'background-color 0.2s'
                                            }}
                                        >
                                            {isReviewLoading ? 'Đang kiểm tra...' : `Dời lịch (Xem trước)`}
                                        </button>
                                        <span style={{ fontSize: '12px', color: '#64748b', fontStyle: 'italic' }}>
                                            {!isDurationValid 
                                                ? "(Vui lòng nhập 'Thời gian kết thúc' hợp lệ để xem trước)" 
                                                : `(Dự kiến dời: ${parseFloat(durationInHours.toFixed(2))} giờ)`
                                            }
                                        </span>
                                    </div>

                                    {/* Bảng Dữ Liệu */}
                                    {scheduleReviewData && scheduleReviewData.length > 0 && (
                                        <div style={{ 
                                            display: 'flex',
                                            justifyContent: 'center',
                                            border: '1px solid #e2e8f0', 
                                            borderRadius: '6px',
                                            padding: '0', 
                                            backgroundColor: '#fff',
                                            marginTop: '10px',
                                            width: '100%'
                                        }}>
                                            <table style={{ 
                                                width: 'auto',      
                                                fontSize: '12px', 
                                                borderCollapse: 'collapse', 
                                                textAlign: 'center'
                                            }}>
                                                <thead style={{ backgroundColor: '#f8fafc', color: '#475569' }}>
                                                    <tr>
                                                        <th style={{ padding: '8px 12px', borderBottom: '1px solid #e2e8f0', whiteSpace: 'nowrap' }}>
                                                            Mã lệnh
                                                        </th>
                                                        <th style={{ padding: '8px 12px', borderBottom: '1px solid #e2e8f0', whiteSpace: 'nowrap' }}>
                                                            Bắt đầu (Mới / Cũ)
                                                        </th>
                                                        <th style={{ padding: '8px 12px', borderBottom: '1px solid #e2e8f0', whiteSpace: 'nowrap' }}>
                                                            Kết thúc (Mới / Cũ)
                                                        </th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    {scheduleReviewData.map((item, idx) => (
                                                        <tr key={idx} style={{ borderBottom: '1px solid #f1f5f9' }}>
                                                            <td style={{ padding: '6px 12px', fontWeight: 600, verticalAlign: 'middle' }}>
                                                                {item.workOrderId}
                                                            </td>
                                                            <td style={{ padding: '6px 12px', verticalAlign: 'middle' }}>
                                                                <div style={{ color: '#2563eb', fontWeight: 500, whiteSpace: 'nowrap' }}>
                                                                    {formatDisplayDate(item.newStart)}
                                                                </div>
                                                                <div style={{ color: '#94a3b8', textDecoration: 'line-through', fontSize: '11px', marginTop: '2px', whiteSpace: 'nowrap' }}>
                                                                    ({formatDisplayDate(item.oldStart)})
                                                                </div>
                                                            </td>
                                                            <td style={{ padding: '6px 12px', verticalAlign: 'middle' }}>
                                                                <div style={{ color: '#2563eb', fontWeight: 500, whiteSpace: 'nowrap' }}>
                                                                    {formatDisplayDate(item.newEnd)}
                                                                </div>
                                                                <div style={{ color: '#94a3b8', textDecoration: 'line-through', fontSize: '11px', marginTop: '2px', whiteSpace: 'nowrap' }}>
                                                                    ({formatDisplayDate(item.oldEnd)})
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    ))}
                                                </tbody>
                                            </table>
                                        </div>
                                    )}
                                </div>
                            )}

                            <FormGroup>
                                <Label>Thời gian bắt đầu xảy ra</Label>
                                {/* 💡 ĐIỂM SỬA 3 (Sử dụng trực tiếp giá trị input): Bỏ getISOString() */}
                                <input 
                                    type="datetime-local" 
                                    value={incidentStartDateTimeLocal} 
                                    onChange={(e) => handleIncidentChange('start', e.target.value)} 
                                />
                            </FormGroup>
                            
                            <FormGroup>
                                <Label>Thời gian kết thúc</Label>
                                {/* 💡 ĐIỂM SỬA 3 (Sử dụng trực tiếp giá trị input): Bỏ getISOString() */}
                                <input 
                                    type="datetime-local" 
                                    value={incidentEndDateTimeLocal} 
                                    onChange={(e) => handleIncidentChange('end', e.target.value)} 
                                />
                            </FormGroup>

                            <FormGroup>
                                <Label>Mã lệnh SX liên quan</Label>
                                <input type="text" value={incidentData.workOrderId || ''} onChange={(e) => handleIncidentChange('workOrderId', e.target.value)} />
                            </FormGroup>
                            <FormGroup>
                                <Label>Người xử lý</Label>
                                <input type="text" value={incidentData.handledBy || ''} onChange={(e) => handleIncidentChange('handledBy', e.target.value)} />
                            </FormGroup>
                            <FormGroup>
                                <Label>Người báo cáo</Label>
                                <input type="text" value={incidentData.reportedBy || ''} onChange={(e) => handleIncidentChange('reportedBy', e.target.value)} />
                            </FormGroup>
                            
                            {/* Checkbox Logic */}
                            {showIsUpdateTime ? (
                                <FormGroup>
                                    <Label className={styles.checkboxLabel}>
                                        <input 
                                            type="checkbox" 
                                            checked={incidentData.isUpdateTime === true} 
                                            onChange={(e) => handleCheckboxChange('isUpdateTime', e)} 
                                        />
                                        {isUpdateTimeLabel}
                                    </Label>
                                </FormGroup>
                            ) : <div/>}
                            
                            {showIsSchedule ? (
                                <FormGroup>
                                    <Label className={styles.checkboxLabel}>
                                        <input 
                                            type="checkbox" 
                                            checked={incidentData.isSchedule === true} 
                                            onChange={(e) => handleCheckboxChange('isSchedule', e)} 
                                        />
                                        {isScheduleLabel}
                                    </Label>
                                </FormGroup>
                            ) : (showIsUpdateTime && !showIsSchedule ? <div/> : null)}
                            
                            <FormGroup style={{gridColumn: '1 / span 3'}}>
                                <Label>Ghi chú</Label>
                                <textarea rows="3" placeholder="Mô tả chi tiết..." value={incidentData.notes || ''} onChange={(e) => handleIncidentChange('notes', e.target.value)}></textarea>
                            </FormGroup>
                        </div>
                    </div>
                )}
                
                {/* --- FOOTER --- */}
                <div className={styles.modalFooter}>
                    {activeTab === 'su-co' && <button className={styles.viewHistoryBtn}>Xem lịch sử sự cố</button>}
                    {activeTab === 'su-co' && <div style={{flexGrow: 1}}/>}

                    {activeTab === 'tien-do' && (
                        <>
                            <CreateButton className={styles.updateModalBtn} onClick={handleSaveProgress} disabled={loading}>
                                {loading ? 'Đang lưu...' : 'Cập nhật'}
                            </CreateButton>
                            <CreateButton className={styles.completeModalBtn} onClick={() => {
                                if (window.confirm(`Xác nhận hoàn tất và đóng lệnh ${selectedOrder?.code}?`)) onUpdateStatus(selectedOrder, 6);
                            }}>Hoàn tất lệnh</CreateButton>
                            <CreateButton className={styles.pauseModalBtn}>Tạm dừng</CreateButton>
                        </>
                    )}
                    
                    {activeTab === 'su-co' && (
                        <>
                            <CreateButton className={styles.cancelBtn} onClick={onClose}>Hủy</CreateButton>
                            <CreateButton className={styles.saveBtn} onClick={handleSaveIncident} disabled={loading}>
                                {loading ? 'Đang lưu...' : 'Lưu sự cố'}
                            </CreateButton>
                        </>
                    )}
                </div>
            </div>
        </div>
    );
};

export default ProgressUpdateModal;