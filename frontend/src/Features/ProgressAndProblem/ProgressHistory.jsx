import React, { useState, useEffect } from 'react';
// IMPORT THÊM CÁC ICON MỚI TẠI ĐÂY
import { 
    FaSpinner, 
    FaCalendarAlt, // Ngày cập nhật
    FaTools, // Thiết bị
    FaUser, // Người cập nhật, Người báo cáo, Người xử lý
    FaTachometerAlt, // Trạng thái/Tiến độ
    FaRulerCombined, // Đơn vị
    FaWarehouse, // SL sản xuất
    FaCheckCircle, // SL hoàn thành thêm (thành công)
    FaTimesCircle, // SL lỗi thêm (lỗi)
    FaExclamationTriangle, // Sự cố, Mức độ ảnh hưởng
    FaChartBar, // Tổng SL Hợp lệ
    FaStickyNote // Ghi chú
} from 'react-icons/fa'; 

import styles from './ProgressHistory.module.scss';
import { getWorkOrderProgressHistory, getWorkOrderErrorHistory } from '../../api/WorkOrderProcess';

// Component nhỏ hiển thị thanh tiến trình
const ProgressBar = ({ percentage }) => {
    const progress = Math.max(0, Math.min(100, parseFloat(percentage)));
    const color = progress < 50 ? '#f59e0b' : progress < 100 ? '#3b82f6' : '#10b981'; // Yellow, Blue, Green

    return (
        <div className={styles.progressBarContainer}>
            <div className={styles.progressBar} style={{ width: `${progress}%`, backgroundColor: color }}></div>
            <span className={styles.percentageText}>{progress.toFixed(2)}%</span>
        </div>
    );
};

const ProgressHistory = ({ order, onBack }) => {
    const [loading, setLoading] = useState(true);
    const [activeTab, setActiveTab] = useState('tien-do');
    const [ProgressHistoryData, setProgressHistoryData] = useState([]);
    const [ErrorHistoryData, setErrorHistoryData] = useState([]);

    const statusMap = {
        5: 'Đang thực thi',
        6: 'Hoàn thành'
    }

    const formatDate = (isoString) => {
        if (!isoString) return '--';
        const date = new Date(isoString);
        // Định dạng 05/12/2025 20:03 (24h)
        return date.toLocaleString('vi-VN', { 
            day: '2-digit', 
            month: '2-digit', 
            year: 'numeric', 
            hour: '2-digit', 
            minute: '2-digit', 
            hour12: false
        }); 
    };
    
    // Hàm này dùng để định dạng thời gian cho tab Sự cố
    const formatDateTimeForError = (isoString) => {
        if (!isoString || isoString === "1970-01-01T00:00:00") return '--';
        return formatDate(isoString);
    };


    useEffect(() => {
        const fetchHistory = async () => {
            setLoading(true);
            try {
                const progressData = await getWorkOrderProgressHistory(order.workOrderId);
                const incidentData = await getWorkOrderErrorHistory(order.workOrderId);

                setProgressHistoryData(progressData);
                setErrorHistoryData(incidentData);
            } catch (error) {
                console.error("Lỗi fetch history:", error);
                setProgressHistoryData([]);
                setErrorHistoryData([]);
            } finally {
                setLoading(false);
            }
        };

        fetchHistory();
    }, [order]);

    return (
        <div className={styles.historyContainer}>
            {/* Header: Bỏ nút Back, chỉ giữ tiêu đề và khoảng trống để căn giữa */}
            <div className={styles.historyHeader}>
                {/* Khoảng trống để cân bằng vị trí của nút Đóng (X) ở bên phải, giúp tiêu đề historyTitle được căn giữa chính xác. */}
                <div style={{ width: '24px' }}></div> 
                
                <h3 className={styles.historyTitle}>Lịch sử cập nhật [{order?.code}]</h3>
                
                {/* Hiện tại, giữ nguyên div trống để căn giữa tiêu đề */}
                <div style={{ width: '24px' }}></div> 
            </div>

            {/* Tab Navigation */}
            <div className={styles.tabNavigation}>
                <button 
                    className={`${styles.tabButton} ${activeTab === 'tien-do' ? styles.activeTab : ''}`}
                    onClick={() => setActiveTab('tien-do')}
                >
                    Tiến độ
                </button>
                <button 
                    className={`${styles.tabButton} ${activeTab === 'su-co' ? styles.activeTab : ''}`}
                    onClick={() => setActiveTab('su-co')}
                >
                    Sự cố
                </button>
            </div>

            <div className={styles.historyContent}>
                {loading ? (
                    <div className={styles.loadingSpinner}>
                        <FaSpinner className={styles.spinning} />
                        <p>Đang tải lịch sử...</p>
                    </div>
                ) : (
                    <>
                        {/* Tab Tiến độ */}
                        {activeTab === 'tien-do' && (
                            ProgressHistoryData.length > 0 ? (
                                <table className={styles.historyTable}>
                                    <thead>
                                        <tr>
                                            {/* Thêm Icon */}
                                            <th><FaCalendarAlt /> Ngày cập nhật</th>
                                            <th><FaTools /> Thiết bị</th>
                                            <th><FaUser /> Người cập nhật</th>
                                            <th><FaTachometerAlt /> Trạng thái</th>
                                            <th><FaRulerCombined /> Đơn vị</th>
                                            <th className={styles.alignRight}><FaWarehouse /> SL sản xuất</th>
                                            <th className={styles.alignRight}><FaCheckCircle className={styles.deltaSuccess} /> SL hoàn thành thêm</th>
                                            <th className={styles.alignRight}><FaTimesCircle className={styles.deltaError} /> SL lỗi thêm</th>
                                            <th className={styles.alignRight}><FaTimesCircle className={styles.deltaError} /> Tổng SL (Lỗi)</th>
                                            <th className={styles.alignRight}><FaChartBar /> Tổng SL (Hợp lệ)</th>
                                            <th className={styles.progressColumn}><FaChartBar /> Phần trăm HT</th>
                                            <th><FaStickyNote /> Ghi chú</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {ProgressHistoryData.map((record) => (
                                            <tr key={record.id}>
                                                <td>{formatDate(record.timestamp)}</td>
                                                <td>{record.equipment}</td>
                                                <td>{record.updatedBy}</td>
                                                <td><span className={styles.statusBadge}>{statusMap[record.status]}</span></td>
                                                <td>{record.unitOfMeasure}</td>
                                                <td className={styles.alignRight}>{record.producedQuantity}</td>
                                                <td className={`${styles.alignRight} ${styles.deltaSuccess}`}>{record.deltaQuantity}</td>
                                                <td className={`${styles.alignRight} ${styles.deltaError}`}>{record.deltaErrorQuantity}</td>
                                                <td className={styles.alignRight}>{record.errorQuantity}</td>
                                                <td className={styles.alignRight}><strong>{record.totalQuantity}</strong></td>
                                                <td className={styles.progressColumn}>
                                                    <ProgressBar percentage={record.progressPercentage} />
                                                </td>
                                                <td>{record.notes}</td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            ) : (
                                <div className={styles.emptyHistory}>
                                    <p>Chưa có lịch sử tiến độ cho lệnh này</p>
                                </div>
                            )
                        )}

                        {/* Tab Sự cố */}
                        {activeTab === 'su-co' && (
                            ErrorHistoryData.length > 0 ? (
                                <table className={styles.historyTable}>
                                    <thead>
                                        <tr>
                                            {/* Thêm Icon */}
                                            <th><FaExclamationTriangle /> Mã sự cố</th>
                                            <th><FaExclamationTriangle /> Tên sự cố</th>
                                            <th><FaExclamationTriangle /> Mức độ ảnh hưởng</th>
                                            <th><FaCalendarAlt /> Thời gian bắt đầu</th>
                                            <th><FaCalendarAlt /> Thời gian kết thúc</th>
                                            <th><FaUser /> Người báo cáo</th>
                                            <th><FaUser /> Người xử lý</th>
                                            <th><FaStickyNote /> Ghi chú</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {ErrorHistoryData.map((record) => (
                                            <tr key={record.id}>
                                                <td>{record.errorId}</td>
                                                <td>{record.errorName}</td>
                                                <td>
                                                    {/* Badge màu động dựa trên giá trị */}
                                                    <span className={
                                                        record.downtimeType === 'Ảnh hưởng lớn' ? styles.badgeDanger :
                                                        record.downtimeType === 'Ảnh hưởng trung bình' ? styles.badgeWarning :
                                                        styles.badgeSuccess
                                                    }>
                                                        {record.downtimeType}
                                                    </span>
                                                </td>
                                                <td>{formatDateTimeForError(record.start)}</td>
                                                <td>{formatDateTimeForError(record.end)}</td>
                                                <td>{formatDateTimeForError(record.reportedBy)}</td>
                                                <td>{formatDateTimeForError(record.handledBy)}</td>
                                                <td>{record.notes}</td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            ) : (
                                <div className={styles.emptyHistory}>
                                    <p>Chưa có lịch sử sự cố cho lệnh này</p>
                                </div>
                            )
                        )}
                    </>
                )}
            </div>
        </div>
    );
};

export default ProgressHistory;