import React, { useState, useMemo, useEffect } from 'react';
import { debounce } from 'lodash';
import styles from './ProgressAndProblem.module.scss';
import { DndContext, closestCenter } from '@dnd-kit/core';
import { arrayMove, SortableContext, useSortable, verticalListSortingStrategy } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import ContentContainer from '../../common/components/ContentContainer';
import CreateButton from '../../common/components/Button/CreateButton';
import { Label } from '../../common/components/Label';
import FormGroup from "../../common/components/FormGroup";
import { getWorkOrderProcess, patchWorkOrderStatus } from '../../api/WorkOrderProcess';
import { FaFileExcel, FaSync, FaGripVertical, FaSearch, FaTimesCircle, FaFilter, FaChevronDown, FaHistory} from 'react-icons/fa';
import ProgressUpdateModal from './ProgressUpdateModal';
import ProgressHistory from './ProgressHistory';
import {useAuthPermissions} from '../../api/UseAuthPermissions'
import { useProductionProgressRealtime } from '../../realtime/useProductionProgressRealtime';

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

const formatDate = (isoString) => {
    if (!isoString || String(isoString).startsWith('0001-01-01')) return '--';

    const date = new Date(isoString);
    if (isNaN(date.getTime())) return '--';

    return date.toLocaleString('vi-VN', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        hour12: true
    });
};

const formatQuantity = (valStr) => (valStr ? parseFloat(valStr).toString() : '0');
const uniqueJoin = (items = []) => {
    const values = items
        .map(x => String(x || '').trim())
        .filter(Boolean);

    return [...new Set(values)].join(', ') || '--';
};

const getEquipmentLabel = (equipment) => {
    if (!equipment) return '';

    if (typeof equipment === 'string') return equipment;

    return (
        equipment.equipmentId ||
        equipment.EquipmentId ||
        equipment.resourceId ||
        equipment.ResourceId ||
        equipment.equipmentName ||
        equipment.EquipmentName ||
        equipment.code ||
        equipment.Code ||
        ''
    );
};

const getPersonLabel = (person) => {
    if (!person) return '';

    if (typeof person === 'string') return person;

    return (
        person.personId ||
        person.PersonId ||
        person.resourceId ||
        person.ResourceId ||
        person.name ||
        person.Name ||
        ''
    );
};
const ProgressCircle = ({ percentage }) => {
    const safePercentage = isNaN(percentage) ? 0 : percentage;
    return (
        <div className={styles.progressCircle} style={{ background: `conic-gradient(#3498db ${safePercentage * 3.6}deg, #e0e0e0 0deg)` }}>
            <div className={styles.progressInner}><span className={styles.progressPercentage}>{safePercentage.toFixed(0)}%</span></div>
        </div>
    );
};
const distinctMaterials = (materials = []) => {
    const map = new Map();

    materials.forEach((m) => {
        const id = m.materialDefinitionId || m.id || '--';
        const unit = m.valueUnitOfMeasure || m.unit || '';
        const key = `${id}_${unit}`;

        if (!map.has(key)) {
            map.set(key, {
                id,
                qty: formatQuantity(m.quantity?.valueString ?? m.qty),
                unit,
                requiredQtyPerDay: m.requireRawComponentQuantityPerDay ?? m.requiredQtyPerDay ?? 0,
                actualQtyPerDay: m.actualRawComponentQuantityPerDay ?? m.actualQtyPerDay ?? 0,
                totalUsedQty: m.actualRawComponentQuantity ?? m.totalUsedQty ?? 0
            });
        }
    });

    return Array.from(map.values());
};
const SkeletonOrder = () => (
    <div className={`${styles.orderItem} ${styles.skeletonItem}`}>
        <div className={styles.skeletonHandle}></div>
        <div className={styles.orderInfo}><div className={`${styles.skeletonLine} ${styles.skeletonTitle}`}></div></div>
        <div className={styles.orderResources}><div className={styles.skeletonLine}></div></div>
        <div className={styles.orderStats}><div className={styles.skeletonLine}></div></div>
        <div className={styles.orderProgress}><div className={styles.skeletonCircle}></div></div>
        <div className={styles.shimmerWrapper}></div>
    </div>
);
const toNumber = (value) => {
    const number = Number(String(value ?? 0).replace(',', '.'));
    return Number.isFinite(number) ? number : 0;
};
const mergeMaterials = (materials = []) => {
    const map = new Map();

    materials.forEach((m) => {
        const id = m.materialDefinitionId || m.id || '--';
        const unit = m.valueUnitOfMeasure || m.unit || '';
        const key = `${id}_${unit}`;

        const qty = toNumber(m.quantity?.valueString ?? m.qty);
        const requiredQtyPerDay = toNumber(m.requireRawComponentQuantityPerDay ?? m.requiredQtyPerDay);
        const actualQtyPerDay = toNumber(m.actualRawComponentQuantityPerDay ?? m.actualQtyPerDay);
        const totalUsedQty = toNumber(m.actualRawComponentQuantity ?? m.totalUsedQty);

        if (!map.has(key)) {
            map.set(key, {
                id,
                unit,
                qty: 0,
                requiredQtyPerDay: 0,
                actualQtyPerDay: 0,
                totalUsedQty: 0
            });
        }

        const current = map.get(key);
        current.qty += qty;
        current.requiredQtyPerDay += requiredQtyPerDay;
        current.actualQtyPerDay += actualQtyPerDay;
        current.totalUsedQty += totalUsedQty;
    });

    return Array.from(map.values()).map(m => ({
        ...m,
        qty: m.qty.toString(),
        requiredQtyPerDay: m.requiredQtyPerDay.toString(),
        actualQtyPerDay: m.actualQtyPerDay.toString(),
        totalUsedQty: m.totalUsedQty.toString()
    }));
};

const SortableOrderItem = ({ order, onUpdateClick, onStatusChange, onShowHistory, canEdit, canView}) => {
    const { attributes, listeners, setNodeRef, transform, transition } = useSortable({ id: order.id });
    const style = { transform: CSS.Transform.toString(transform), transition };
    const percent = order.progressPercentage > 0 ? order.progressPercentage : (order.total > 0 ? (order.completed / order.total) * 100 : 0);
    return (
        <div ref={setNodeRef} style={style} {...attributes} className={styles.orderItem}>
            <div className={styles.dragHandle} {...listeners}><FaGripVertical /></div>
            <div className={styles.orderInfo}>
                <h3 className={styles.orderCode}>{order.code} <small style={{fontSize: '0.7em', color: '#94a3b8', fontWeight: 500}}>#{order.moCode}</small></h3>
                <div className={styles.infoGrid}>
                    <span>Sản phẩm:</span> <strong>{order.productId} ({order.unit})</strong>
                    <span>Ngày bắt đầu theo kế hoạch:</span> <strong>{order.planStart}</strong>
                    <span>Ngày kết thúc theo kế hoạch:</span> <strong>{order.planEnd}</strong>
                    <span>Ngày bắt đầu thực tế:</span> <strong>{order.actualStart}</strong>
                    <span>Ngày kết thúc thực tế:</span> <strong>{order.actualEnd}</strong>
                </div>
                    <div className={styles.itemActions}>
                        {order.status === 'ReadyForProduction' && (
                            <CreateButton
                                className={styles.startButton}
                                disabled={!canEdit}
                                title={!canEdit ? "Bạn không có quyền chỉnh sửa" : "Bắt đầu sản xuất"}
                                onClick={() => { 
                                    if (window.confirm(`Bắt đầu lệnh ${order.code}?`)) 
                                        onStatusChange(order, 5);
                                }}
                            >
                                Bắt đầu sản xuất
                            </CreateButton>
                        )}

                        {order.status === 'InProduction' && (
                            <>
                                <CreateButton
                                    className={styles.completeButton}
                                    disabled={!canEdit}
                                    title={!canEdit ? "Bạn không có quyền chỉnh sửa" : "Hoàn thành lệnh sản xuất"}
                                    onClick={() => { 
                                        if(window.confirm(`Hoàn thành lệnh ${order.code}?`)) 
                                            onStatusChange(order, 6); 
                                    }}
                                >
                                    Hoàn thành
                                </CreateButton>
                                
                                <CreateButton 
                                    className={styles.updateButton} 
                                    disabled={!canEdit}
                                    title={!canEdit ? "Bạn không có quyền chỉnh sửa" : "Cập nhật tiến độ sản xuất"}
                                    onClick={() => onUpdateClick(order)}
                                >
                                    Cập nhật tiến độ
                                </CreateButton>
                            </>
                        )}

                        {order.status === 'Completed' && (
                            <span className={styles.completedText}>
                                Đã đóng lệnh
                            </span>
                        )}

                        <CreateButton
                            className={styles.historyButton}
                            onClick={() => onShowHistory(order)}
                        >
                            <FaHistory/>
                            Lịch sử
                        </CreateButton>
                    </div>
            </div>
            <div className={styles.orderResources}>
                <div className={styles.resourceBlock}>
                    <span>Nhân viên vận hành</span>
                    <strong title={order.employees}>{order.employees}</strong>
                </div>
                <div className={styles.resourceBlock}>
                    <span>Nguyên vật liệu </span>
                    {/* Phần render này đã đúng, nhưng cần dữ liệu từ props order.materials */}
                    {order.materials && order.materials.length > 0 ? (
                        <div style={{fontWeight: '900'}}>
                            {order.materials.map((mat, idx) => (
                            <div key={idx}>
                                {/* {mat.id} ({mat.unit})  -   SL: {mat.totalUsedQty}/{mat.qty}  -   YC: {mat.actualQtyPerDay}/{mat.requiredQtyPerDay} {mat.unit}/ngày */}
                                {mat.id} ({mat.unit})  -   SL: {mat.qty}  -   YC: {mat.requiredQtyPerDay} {mat.unit}/ngày
                            </div>))}
                        </div>
                    ) : (<span style={{fontStyle:'italic', color: '#ccc'}}>Không có NVL</span>)}
                </div>
            </div>
            <div className={styles.orderStats}>
                <div className={styles.resourceBlock}>
                    <span>Thiết bị</span>
                    <strong title={order.machine}>{order.machine}</strong>
                </div>
                <div><span>Sản xuất thành công</span><strong>{order.completed} / {order.total}</strong></div>
                <div><span>Sản lượng yêu cầu theo ngày</span><strong>{order.productionTargetByDay}</strong></div>
                <div><span>Số sản phẩm lỗi</span><strong>{order.errorQuantity}</strong></div>
            </div>
            <div className={styles.orderProgress}>
                <span className={`${styles.statusBadge} ${styles[order.statusClass] || styles.defaultBadge}`}>{order.statusText}</span>
                <ProgressCircle percentage={percent} />
            </div>
        </div>
    );
};

const ProgressAndProblem = () => {
    const [orders, setOrders] = useState([]);
    const [loading, setLoading] = useState(true);
    const [statusFilter, setStatusFilter] = useState("all");
    const [inputValue, setInputValue] = useState(''); 
    const [searchTerm, setSearchTerm] = useState('');
    const [reloadKey, setReloadKey] = useState(0);

    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedOrder, setSelectedOrder] = useState(null);
    const [activeTab, setActiveTab] = useState('tien-do');
    const [showHistoryModal, setShowHistoryModal] = useState(false);
    const [historyOrder, setHistoryOrder] = useState(null);

    const { permissions, loading: permLoading, error: permError } = useAuthPermissions(6);
    const safePermissions = permissions || [];
    const canView = safePermissions.some((p) => p.canView);
    const canEdit = safePermissions.some((p) => p.canEdit);

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

        console.log("REALTIME PROGRESS RECEIVED IN ProgressAndProblem:", {
            workOrderId,
            producedQuantity,
            errorQuantity,
            progressPercentage,
            status
        });

        setOrders((prevOrders) => {
            let matched = false;

            const nextOrders = prevOrders.map((order) => {
                const currentId = String(order.workOrderId ?? "").trim();

                if (currentId !== workOrderId) {
                    return order;
                }

                matched = true;

                const statusInfo = STATUS_MAPPING[status] || {
                    label: status,
                    className: 'default'
                };

                return {
                    ...order,

                    // Các field UI đang render trực tiếp
                    completed: producedQuantity,
                    errorQuantity: errorQuantity,
                    progressPercentage: progressPercentage,

                    // Cập nhật trạng thái nếu backend gửi về
                    status: status || order.status,
                    statusText: status ? statusInfo.label : order.statusText,
                    statusClass: status ? statusInfo.className : order.statusClass,

                    // Giữ thêm field gốc để modal/history nếu cần dùng
                    originalData: {
                        ...order.originalData,
                        producedQuantity,
                        errorQuantity,
                        progressPercentage,
                        status: status || order.originalData?.status
                    },

                    lastRealtimeAt: new Date().toISOString()
                };
            });

            // if (!matched) {
            //     console.warn(
            //         "Realtime DTO không match WorkOrder nào trên trang:",
            //         workOrderId,
            //         "Danh sách hiện có:",
            //         prevOrders.map(x => x.workOrderId)
            //     );
            // }

            return nextOrders;
        });
    }
});

    const fetchData = async () => {
        setLoading(true);
        try {
            const data = await getWorkOrderProcess();
            if (data && Array.isArray(data)) {
                const mappedOrders = data.map(item => {
                    const statusInfo = STATUS_MAPPING[item.status] || { label: item.status, className: 'default' };
                    const employeeStr = uniqueJoin((item.persons || []).map(getPersonLabel));
                    const machineName = uniqueJoin((item.equipments || []).map(getEquipmentLabel));
                    
                    // --- [FIX]: Thêm lại logic map materials ---
                    const materialList = distinctMaterials(item.materials || []);
                    // -------------------------------------------

                    return {
                        id: item.id.toString(),
                        workOrderId: item.workOrderId, 
                        manufacturingOrderId: item.manufacturingOrderId,
                        code: item.workOrderId || '--',
                        moCode: item.manufacturingOrderCode,
                        productId: item.materialDefinitionId || '--',
                        planStart: formatDate(item.planedStart),
                        planEnd: formatDate(item.planedEnd),
                        actualStart: formatDate(item.actualStartDate),
                        actualEnd: formatDate(item.actualEndDate),
                        machine: machineName,
                        total: item.productionTarget || 0,
                        completed: item.producedQuantity || 0,
                        productionTargetByDay: item.productionTargetByDay || 0,
                        errorQuantity: item.errorQuantity || 0,
                        progressPercentage: item.progressPercentage || 0,
                        unit: item.unitOfMeasure || 'Đv',
                        status: item.status,
                        statusText: statusInfo.label,
                        statusClass: statusInfo.className,
                        employees: employeeStr,
                        materials: materialList, // --- [FIX]: Trả về materials ---
                        originalData: item
                    };
                });
                setOrders(mappedOrders);
            }
        } catch (error) {
            console.error("Lỗi fetch data:", error);
        } finally {
            setLoading(false);
        }
    };

    

    useEffect(() => { setOrders([]); fetchData(); }, [reloadKey]);

    const handleReload = () => setReloadKey(prev => prev + 1);

    const handleUpdateStatus = async (order, newStatus) => {
        try {
            const payload = {
                manufacturingOrderId: String(order.moCode), 
                workOrderId: String(order.workOrderId),
                workOrderStatus: newStatus 
            };
            await patchWorkOrderStatus(payload);
            await fetchData(); 
        } catch (error) {
            alert("Có lỗi khi cập nhật trạng thái.");
        }
    };

    const handleOpenModal = (order) => { 
        setSelectedOrder(order);
        setActiveTab('tien-do'); 
        setIsModalOpen(true); 
    };

    const handleCloseModal = () => { 
        setIsModalOpen(false); 
        setSelectedOrder(null); 
    };

    const handleShowHistory = (order) => {
        setHistoryOrder(order);
        setShowHistoryModal(true);
    };

    const handleCloseHistory = () => {
        setShowHistoryModal(false);
        setHistoryOrder(null);
    };

    const updateSearchTerm = useMemo(() => debounce((value) => setSearchTerm(value.toLowerCase()), 300), []);
    const handleSearchChange = (e) => {
        const value = e.target.value;
        setInputValue(value);
        updateSearchTerm(value);
    };
    const handleClearSearch = () => {
        setInputValue('');
        setSearchTerm('');
        updateSearchTerm.cancel(); 
    };

    const filteredOrders = useMemo(() => {
        return orders.filter(order => {
            const statusMatch = statusFilter === 'all' || order.status === statusFilter;
            const searchTermMatch =
    !searchTerm ||
    order.code.toLowerCase().includes(searchTerm) ||
    order.moCode?.toLowerCase().includes(searchTerm) ||
    order.productId.toLowerCase().includes(searchTerm) ||
    order.machine.toLowerCase().includes(searchTerm) ||
    order.employees.toLowerCase().includes(searchTerm);
            return statusMatch && searchTermMatch;
        });
    }, [orders, statusFilter, searchTerm]);

    const handleDragEnd = (event) => {
    const { active, over } = event;

    if (!over || active.id === over.id) return;

    setOrders((currentOrders) => {
        const oldIndex = currentOrders.findIndex(item => item.id === active.id);
        const newIndex = currentOrders.findIndex(item => item.id === over.id);

        if (oldIndex < 0 || newIndex < 0) return currentOrders;

        return arrayMove(currentOrders, oldIndex, newIndex);
    });
};

    return (
        <div className={styles.pageContainer}>
             <h2 className="title">Tiến độ & Sự cố | Tiến độ sản xuất</h2>
            <ContentContainer className={styles.mainContent}>
                <div className={styles.actionBar}>
                    <FormGroup className={styles.filters}>
                        <Label>Bộ lọc & tìm kiếm</Label>
                        <div className={styles.filterWrapper}>
                            <FaFilter className={styles.filterIcon} />
                            <select className={styles.customSelect} value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)}>
                                <option value="all">Tất cả trạng thái</option>
                                <option value="InProduction">Đang thực thi</option>
                                <option value="ReadyForProduction">Chờ sản xuất</option>
                                <option value="Completed">Đã hoàn thành</option>
                            </select>
                            <FaChevronDown className={styles.arrowIcon} />
                        </div>
                        <div className={styles.searchWrapper}>
                            <FaSearch className={styles.searchIcon} />
                            <input type="text" className={styles.customSearchInput} placeholder="Tìm kiếm..." value={inputValue} onChange={handleSearchChange} />
                            {inputValue && <FaTimesCircle className={styles.clearIcon} onClick={handleClearSearch} />}
                        </div>
                    </FormGroup>
                    <div className={styles.topActions}>
                        <CreateButton><FaFileExcel /> Xuất Excel</CreateButton>
                        <button className={styles.reloadBtn} onClick={handleReload} disabled={loading}>
                            <FaSync className={loading ? styles.spinning : ''} /> {loading ? '...' : 'Tải lại'}
                        </button>
                    </div>
                </div>

                {loading && orders.length === 0 ? (
                    <div className={styles.orderList}>{[1, 2, 3].map((i) => <SkeletonOrder key={i} />)}</div>
                ) : (
                    <DndContext collisionDetection={closestCenter} onDragEnd={handleDragEnd}>
                        <div className={styles.orderList}>
                            <SortableContext items={filteredOrders} strategy={verticalListSortingStrategy}>
                                {filteredOrders.length > 0 ? (
                                    filteredOrders.map(order => 
                                        <SortableOrderItem 
                                        key={order.id} 
                                        order={order} 
                                        onUpdateClick={handleOpenModal} 
                                        onStatusChange={handleUpdateStatus} 
                                        onShowHistory={handleShowHistory} 
                                        canEdit={canEdit}
                                        canView={canView}/>
                                    )
                                ) : (
                                    <div className={styles.emptyState}><p>Không có dữ liệu</p></div>
                                )}
                            </SortableContext>
                        </div>
                    </DndContext>
                )}
            </ContentContainer>

            <ProgressUpdateModal
                isOpen={isModalOpen}
                selectedOrder={selectedOrder}
                activeTab={activeTab}
                setActiveTab={setActiveTab}
                onClose={handleCloseModal}
                onSuccess={fetchData} 
                onUpdateStatus={handleUpdateStatus}
            />

                        {/* ProgressHistory Modal */}
            {showHistoryModal && (
                <div className={styles.overlay} onClick={handleCloseHistory}>
                    <div style = {{position: 'relative', display: 'flex', maxHeight: '90vh'}}onClick={e => e.stopPropagation()} >
                        <button className={styles.closeButton} onClick={handleCloseHistory}>&times;</button>
                        <ProgressHistory order={historyOrder} onClose={handleCloseHistory} />
                    </div>
                </div>
            )}
        </div>
    );
};

export default ProgressAndProblem;