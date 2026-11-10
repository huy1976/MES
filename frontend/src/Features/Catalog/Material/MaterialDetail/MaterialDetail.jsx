import React, { useEffect, useState } from "react";
import './MaterialDetail.css';
import { getMaterialById, createMaterialUnit} from '../../../../api/MaterialDefinition';
import { toast } from "react-toastify"; // Import toast for notifications
import "react-toastify/dist/ReactToastify.css";
import {useAuthPermissions} from '../../../../api/UseAuthPermissions';

const MaterialDetail = ({ material }) => {
    const [activeTab, setActiveTab] = useState('formula');
    const [materialDetails, setMaterialDetails] = useState(null);

    // Công thức đang chọn
    const [selectedOperationId, setSelectedOperationId] = useState(null);

    const { permissions, loading: permLoading, error: permError } = useAuthPermissions(7); // Giả sử pageId cho Material là 10
    const canView = permissions.some((p) => p.canView);
    const canEdit = permissions.some((p) => p.canEdit);
    // === State cho CreateUnit ===
    const [materialUnit, setMaterialUnit] = useState({
        materialDefinitionId: material?.code || '',
        unitId: '',
        unitName: '',
        conversionValueToPrimaryUnit: 0,
    });
    const [materialUnitList, setMaterialUnitList] = useState([]);

    useEffect(() => {
        const fetchData = async () => {
            try {
                if (material && material.code) {
                    const data = await getMaterialById(material.code);
                    setMaterialDetails(data);
                } else {
                    setMaterialDetails({ operationDTOs: [] });
                }
            } catch (error) {
                console.error("Lỗi khi tải dữ liệu vật tư:", error);
                setMaterialDetails({ operationDTOs: [] });
            }
        };

        fetchData();
    }, [material.code]);

    // Set default công thức sau khi load API
    useEffect(() => {
        if (materialDetails?.operationDTOs?.length > 0) {
            setSelectedOperationId(materialDetails.operationDTOs[0].operationId);
        }
    }, [materialDetails]);

    // === HELPER: MÀU SẮC LOẠI VẬT TƯ (Thêm mới) ===
    const getTypeStyle = (typeName) => {
        const normalized = typeName ? typeName.toLowerCase().trim() : '';

        if (normalized.includes('nguyên vật liệu')) {
            return { backgroundColor: '#fee2e2', color: '#b91c1c', border: '1px solid #b91c1c' };
        } else if (normalized.includes('bán thành phẩm')) {
            return { backgroundColor: '#e0f2fe', color: '#0369a1', border: '1px solid #0369a1' };
        } else if (normalized.includes('thành phẩm')) {
            return { backgroundColor: '#dcfce7', color: '#15803d', border: '1px solid #15803d' };
        } else {
            return { backgroundColor: '#f3f4f6', color: '#374151', border: '1px solid #9ca3af' };
        }
    };

    if (!materialDetails) {
        return <div>Đang tải dữ liệu vật tư...</div>;
    }

    if (!material || !material.rawData) {
        return <div className="error">Không có dữ liệu vật tư</div>;
    }

    const {
        code = 'N/A',
        name = 'Không tên',
        type = 'Không xác định',
        unit = 'N/A',
        spec = 'Không có thông số',
        operationsDefinitions = [],
        qualityStandards = [],
        rawData,
    } = material;

    const properties = rawData.properties || [];
    const materialClassName = rawData.materialClassId || 'Không xác định';

    const formulaCount = materialDetails.operationDTOs?.length || 0;
    const qualityCount = qualityStandards.length;
    const propertyCount = properties.length;

    const operationsTypeMap = {
        "Production": 'Sản xuất',
        "Maintenance": 'Bảo trì',
        "Inspection": 'Kiểm tra',
        "Setup": 'Thiết lập',
    };

    // Lấy operation đang được chọn
    const selectedOperation = materialDetails.operationDTOs?.find(
        op => op.operationId === selectedOperationId
    );

    const boms = selectedOperation?.bomDTOs || [];
    const currentSegments = selectedOperation?.segmentDTOs || [];

    const getValueDisplay = (prop) => {
        if (prop.valueString !== undefined && prop.valueString !== null) return prop.valueString;
        if (prop.valueNumber !== undefined && prop.valueNumber !== null) return prop.valueNumber.toLocaleString('vi-VN');
        if (prop.valueBoolean !== undefined && prop.valueBoolean !== null) return prop.valueBoolean ? 'Có' : 'Không';
        return '—';
    };

    const getUnitDisplay = (prop) => {
        return prop.valueUnitOfMeasure && prop.valueUnitOfMeasure !== 'None' ? prop.valueUnitOfMeasure : '';
    };

    // ========================
    // RENDER TAB FORMULA
    // ========================
    const renderFormula = () => {
        if (!materialDetails.operationDTOs || materialDetails.operationDTOs.length === 0) {
            return <div className="empty-state"><p>Không có công thức sản xuất</p></div>;
        }

        return (
            <div className="formula-content">
                {/* Chọn công thức */}
                <div className="formula-select-bar">
                    <div className="formula-select-group">
                        <label className="select-label">Chọn Phương thức</label>
                        <select
                            className="formula-select-dropdown"
                            value={selectedOperationId || ''}
                            onChange={(e) => setSelectedOperationId(e.target.value)}
                        >
                            {materialDetails.operationDTOs.map((op) => (
                                <option key={op.operationId} value={op.operationId}>
                                    {op.operationId}
                                </option>
                            ))}
                        </select>
                    </div>
                    <div className="formula-name-display">
                        <label className="select-label">Tên Phương thức</label>
                        <span className="name-value">{selectedOperation?.description || '—'}</span>
                    </div>
                </div>

                {/* GRID 2 cột */}
                <div className="formula-details-grid">
                    {/* BOM */}
                    <div className="bom-column">
                        <h3 className="column-title">BOM</h3>
                        <div className="card-scroll-area">
                            {boms.length > 0 ? (
                                boms.map((bom, index) => (
                                    <div key={index} className="formula-card bom-card">
                                        <div><strong>Mã BOM:</strong> <span>{bom.bomId || '—'}</span></div>
                                        <div><strong>Nguyên liệu đầu vào:</strong> <span>{bom.rawMaterialId || '—'}</span></div>
                                        <div><strong>Số lượng:</strong> <span>
                                            {bom.quantity && bom.unitOfMeasure ? `${bom.quantity} ${bom.unitOfMeasure}` : '—'}
                                        </span></div>
                                    </div>
                                ))
                            ) : (
                                <div className="empty-state"><p>Không có BOM</p></div>
                            )}
                        </div>
                    </div>

                    {/* SEGMENTS */}
                    <div className="process-column">
                        <h3 className="column-title">Công đoạn</h3>
                        <div className="card-scroll-area">
                            {currentSegments.length > 0 ? (
                                currentSegments.map((segment, index) => (
                                    <div key={index} className="formula-card process-card">
                                        <div><strong>Mã Công đoạn:</strong> <span>{segment.segmentId || '—'}</span></div>
                                        <div><strong>Mô tả:</strong> <span>{segment.description || '—'}</span></div>
                                        <div><strong>Loại Công đoạn:</strong> <span>
                                            {operationsTypeMap[segment.typeSegment] || '—'}
                                        </span></div>
                                    </div>
                                ))
                            ) : (
                                <div className="empty-state"><p>Không có công đoạn</p></div>
                            )}
                        </div>
                    </div>
                </div>
            </div>
        );
    };

    const renderQuality = () => {
        if (qualityStandards.length === 0) {
            return <div className="empty-state"><p>Không có tiêu chuẩn chất lượng</p></div>;
        }
        return (
            <div className="quality-standards-section">
                <h3>Tiêu chuẩn chất lượng</h3>
                <table className="quality-table">
                    <thead>
                        <tr>
                            <th>Tên tiêu chuẩn</th>
                            <th>Mô tả thuộc tính</th>
                            <th>Giá trị</th>
                        </tr>
                    </thead>
                    <tbody>
                        {qualityStandards.map((standard) =>
                            standard.properties?.length > 0 ? (
                                standard.properties.map((prop, propIdx) => (
                                    <tr key={`${standard.qualityStandardId}-${propIdx}`}>
                                        <td><strong>{standard.qualityStandardId || 'Không xác định'}</strong></td>
                                        <td>{prop.description || 'N/A'}</td>
                                        <td>{getValueDisplay(prop)}</td>
                                    </tr>
                                ))
                            ) : (
                                <tr key={standard.qualityStandardId}>
                                    <td><strong>{standard.qualityStandardId || 'Không xác định'}</strong></td>
                                    <td colSpan="2">Không có thuộc tính</td>
                                </tr>
                            )
                        )}
                    </tbody>
                </table>
            </div>
        );
    };

    const renderProperties = () => {
        if (properties.length === 0) {
            return <div className="empty-state"><p>Không có thuộc tính nào được định nghĩa.</p></div>;
        }
        return (
            <div className="properties-section">
                <h3>Thuộc tính vật tư ({propertyCount})</h3>
                <table className="properties-table">
                    <thead>
                        <tr>
                            <th>STT</th>
                            <th>Tên thuộc tính</th>
                            <th>Mô tả</th>
                            <th>Giá trị</th>
                            <th>Đơn vị</th>
                            <th>Kiểu dữ liệu</th>
                        </tr>
                    </thead>
                    <tbody>
                        {properties.map((prop, index) => (
                            <tr key={index}>
                                <td>{index + 1}</td>
                                <td className="prop-name"><strong>{prop.name || '—'}</strong></td>
                                <td className="prop-desc">{prop.description || '—'}</td>
                                <td className="prop-value">
                                    <span className="value-highlight">{getValueDisplay(prop)}</span>
                                </td>
                                <td className="prop-unit">{getUnitDisplay(prop) || '—'}</td>
                                <td>
                                    <span className={`data-type-badge type-${prop.valueType}`}>
                                        {['', 'Số nguyên', 'Số thực', 'Chuỗi', 'Logic', 'Ngày giờ', 'Danh sách'][prop.valueType] || 'Không rõ'}
                                    </span>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        );
    };

    // === XỬ LÝ FORM CREATE UNIT ===
    const handleUnitChange = (e) => {
        const { name, value } = e.target;
        setMaterialUnit((prev) => ({
            ...prev,
            [name]: name === 'conversionValueToPrimaryUnit' ? parseFloat(value) || 0 : value
        }));
    };

    const handleAddUnitToList = (e) => {
        e.preventDefault();
        if (!materialUnit.unitId || materialUnit.conversionValueToPrimaryUnit === 0) {
            toast.info("Vui lòng điền đầy đủ Mã đơn vị và Giá trị quy đổi!", {
                position: "top-right",
                autoClose: 3000,
            });
            return;
        }

        const newUnit = {
            ...materialUnit,
            tempId: Date.now()
        };

        setMaterialUnitList((prevList) => [...prevList, newUnit]);
        setMaterialUnit({
            materialDefinitionId: material?.code || '',
            unitId: '',
            unitName: '',
            conversionValueToPrimaryUnit: 0,
        });
    };

    const handleDeleteUnitFromList = (tempId) => {
        setMaterialUnitList((prevList) => prevList.filter((item) => item.tempId !== tempId));
    };

    const handleConfirmUnits = async () => {
        if (materialUnitList.length === 0) {
            toast.info("Không có đơn vị nào để xác nhận!", {
                position: "top-right",
                autoClose: 3000,
            });
            return;
        }

        for (const item of materialUnitList) {

            const payload = {
                materialDefinitionId: item.materialDefinitionId,
                unitId: item.unitId,
                unitName : item.unitName,
                conversionValueToPrimaryUnit: item.conversionValueToPrimaryUnit,
            }

            try {
                await createMaterialUnit(item.materialDefinitionId, payload);
                toast.success("Thêm đơn vị thành công!", {
                        position: "top-right",
                        autoClose: 3000,
                        hideProgressBar: false,
                        closeOnClick: true,
                        pauseOnHover: true,
                        draggable: true,
                        progress: undefined,
                      });
            }
            catch {
                toast.error("Lỗi: Không thể thêm đơn vị!", {
                        position: "top-right",
                        autoClose: 3000,
                        hideProgressBar: false,
                        closeOnClick: true,
                        pauseOnHover: true,
                        draggable: true,
                        progress: undefined,
                      });
            }

        }
        setMaterialUnitList([]);
    };

    const renderCreateUnit = () => {
        return (
            <div className="newunit-container">
                <form onSubmit={handleAddUnitToList}>
                    <div className="form-group">
                        <label>Mã vật tư</label>
                        <input 
                            type="text" 
                            value={material?.code || ''} 
                            disabled 
                        />
                    </div>

                    <div className="form-group">
                        <label>Mã đơn vị quy đổi</label>
                        <input 
                            type="text" 
                            name="unitId" 
                            value={materialUnit.unitId} 
                            onChange={handleUnitChange}
                            required 
                        />
                    </div>

                    <div className="form-group">
                        <label>Tên đơn vị quy đổi</label>
                        <input 
                            type="text" 
                            name="unitName" 
                            value={materialUnit.unitName} 
                            onChange={handleUnitChange}
                            required 
                        />
                    </div>

                    <div className="form-group">
                        <label>Giá trị quy đổi so với đơn vị chính</label>
                        <input 
                            type="number" 
                            name="conversionValueToPrimaryUnit" 
                            value={materialUnit.conversionValueToPrimaryUnit} 
                            onChange={handleUnitChange}
                            placeholder="VD: 1000"
                            step="0.01"
                            required 
                        />
                    </div>

                    <button type="submit" className={`create-unit-btn ${!canEdit ? "disabled-button" : ""}`}
                    disabled={!canEdit}
                    title={!canEdit ? "Bạn không có quyền thêm đơn vị" : "Thêm đơn vị"}
                    >
                        Thêm đơn vị
                    </button>
                </form>

                <div className="unit-list">
                    <h3>Danh sách đơn vị đã thêm</h3>
                    <div className="table-unit-container">
                        <table>
                            <thead>
                                <tr>
                                    <th>STT</th>
                                    <th>Mã đơn vị</th>
                                    <th>Tên đơn vị</th>
                                    <th>Giá trị quy đổi</th>
                                    <th>Hành động</th>
                                </tr>
                            </thead>
                            <tbody>
                                {materialUnitList.length === 0 ? (
                                    <tr>
                                        <td colSpan="5" style={{ textAlign: 'center' }}>
                                            Chưa có đơn vị nào.
                                        </td>
                                    </tr>
                                ) : (
                                    materialUnitList.map((item, index) => (
                                        <tr key={item.tempId}>
                                            <td>{index + 1}</td>
                                            <td>{item.unitId}</td>
                                            <td>{item.unitName}</td>
                                            <td>{item.conversionValueToPrimaryUnit}</td>
                                            <td>
                                                <button 
                                                    type="button" 
                                                    className="delete-item-btn" 
                                                    onClick={() => handleDeleteUnitFromList(item.tempId)}
                                                >
                                                    Xóa
                                                </button>
                                            </td>
                                        </tr>
                                    ))
                                )}
                            </tbody>
                        </table>
                    </div>

                    <button
                        type="button"
                        className="confirm-btn"
                        onClick={handleConfirmUnits}
                        disabled={materialUnitList.length === 0}
                    >
                        Xác nhận ({materialUnitList.length})
                    </button>
                </div>
            </div>
        );
    };








    return (
        <div className="material-detail-container">
            {/* HEADER */}
            <div className="detail-header">
                <h2>Thông tin chi tiết của {code}</h2>
                <table className="info-row">
                    <thead>
                        <tr>
                            <th>Loại vật tư</th>
                            <th>Mã vật tư</th>
                            <th>Tên vật tư</th>
                            <th>Đơn vị</th>
                            <th>Thông số kỹ thuật</th>
                            <th>Số phương thức</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            {/* CẬP NHẬT PHẦN NÀY: ÁP DỤNG MÀU SẮC */}
                            <td>
                                <span className="status-badge-styled" style={getTypeStyle(type)}>
                                    {type}
                                </span>
                            </td>
                            <td><strong>{code}</strong></td>
                            <td>{name}</td>
                            <td>{unit}</td>
                            <td>{spec}</td>
                            <td>{formulaCount}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
            {/* TABS */}
            <div className="tabs">
                <button className={`tab ${activeTab === 'formula' ? 'active' : ''}`} onClick={() => setActiveTab('formula')}>
                    Phương thức ({formulaCount})
                </button>
                <button className={`tab ${activeTab === 'quality' ? 'active' : ''}`} onClick={() => setActiveTab('quality')}>
                    Chất lượng ({qualityCount})
                </button>
                <button className={`tab ${activeTab === 'properties' ? 'active' : ''}`} onClick={() => setActiveTab('properties')}>
                    Thuộc tính ({propertyCount})
                </button>
                <button className={`tab ${activeTab === 'createUnit' ? 'active' : ''}`} onClick={() => setActiveTab('createUnit')}>
                    Thêm đơn vị quy đổi
                </button>
            </div>

            {/* CONTENT */}
            <div className="content">
                {activeTab === 'formula' && renderFormula()}
                {activeTab === 'quality' && renderQuality()}
                {activeTab === 'properties' && renderProperties()}
                {activeTab === 'createUnit' && renderCreateUnit()}
            </div>
        </div>
    );
};

export default MaterialDetail;