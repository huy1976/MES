import React, { useState, useEffect } from 'react'; // 1. Import useEffect
import CreateNewStageRelationship from './CreateNewStageRelationship/CreateNewStageRelationship';
import { getFormualaDetail } from '../../../../api/FormulaDetail';
import './FormulaDetail.css';
import {useAuthPermissions} from '../../../../api/UseAuthPermissions';

const FormulaDetail = ({ formula, onBack }) => {
  const [showCreateNewRelation, setShowCreateNewRelation] = useState(false);
  
  // 2. State lưu trữ dữ liệu chi tiết từ API
  const [detailData, setDetailData] = useState(null);

  const { permissions, loading: permLoading, error: permError } = useAuthPermissions(7); // Giả sử pageId cho Formula là 9
  const canView = permissions.some((p) => p.canView);
  const canEdit = permissions.some((p) => p.canEdit);

  // 3. Gọi API lấy chi tiết khi có mã công thức
  useEffect(() => {
    const fetchDetail = async () => {
      if (formula && formula.code) {
        try {
          // Gọi API với tham số là formula.code (operationId)
          const response = await getFormualaDetail(formula.code);
          // Giả sử API trả về data trực tiếp theo format JSON bạn đưa
          setDetailData(response); 
        } catch (error) {
          console.error("Lỗi khi lấy chi tiết công thức:", error);
        }
      }
    };

    fetchDetail();
  }, [formula]);

  const handleCreateNewRelation = () => setShowCreateNewRelation(true);
  
  const handleBackFromCreateRelation = () => setShowCreateNewRelation(false);

  const relationsList = formula.relations || [];

  return (
    <div className="formula-detail-wrapper">
      {showCreateNewRelation ? (  
        <>
          <div className="detail-nav-back">
             <button onClick={handleBackFromCreateRelation} className="back-btn-simple">
              <i className="fas fa-chevron-left"></i> Quay lại chi tiết
            </button>
          </div>
          <CreateNewStageRelationship 
            formulaCode={formula.code}
            onComplete={() => setShowCreateNewRelation(false)}
            onBack={handleBackFromCreateRelation}
          />
        </>
      ) : (
        <>
          {/* --- Nút Quay lại danh sách chính --- */}
          <div className="detail-nav-back">
            <button onClick={onBack} className="back-btn-simple">
              <i className="fas fa-chevron-left"></i> Quay lại danh sách
            </button>
          </div>

          <div className="formula-detail-card">
            {/* === HEADER: THÔNG TIN CHUNG === */}
            <div className="detail-section header-section">
              <h2 className="section-title text-center">Thông tin chi tiết của công thức {formula.code}</h2>
              <div className="info-table-container">
                <table className="info-table">
                  <thead>
                    <tr>
                      <th>Mã công thức</th><th>Tên công thức</th><th>Sản phẩm đầu ra</th><th>Mã sản phẩm</th><th>Phiên bản</th><th>Ngày khả dụng</th><th>Ngày hết hạn</th><th>Mã BOM</th><th>Mã Công đoạn</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td>{formula.code}</td>
                      <td>{formula.name}</td>
                      <td>{formula.outputProduct}</td>
                      <td>{formula.productCode}</td>
                      <td>{formula.version}</td>
                      <td>{formula.availableDate}</td>
                      <td>{formula.expiryDate || '-'}</td>
                      {/* Hiển thị tóm tắt mã BOM từ props ban đầu hoặc load từ detailData nếu muốn đồng bộ */}
                      <td>{Array.isArray(formula.bomCode) && formula.bomCode.length > 0 ? formula.bomCode.map(b => b.bomId).join(', ') : formula.bomCode || '-'}</td>
                      <td>{formula.operationCode}</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>

            {/* === BODY: BOM & CÔNG ĐOẠN (DATA TỪ API) === */}
            <div className="detail-body-grid">
              
              {/* Cột Trái: BOM (Lấy từ detailData.bomDTOs) */}
              <div className="detail-box">
                <div className="box-header"><h3>BOM</h3></div>
                <div className="box-content scrollable">
                  {detailData && detailData.bomDTOs && detailData.bomDTOs.length > 0 ? (
                    detailData.bomDTOs.map((bom, index) => (
                      <div key={index} className="detail-item-card">
                        <div className="card-row">
                            <span className="value">Mã BOM: {bom.bomId}</span>
                        </div>
                        <div className="card-row">
                            <span className="value">Vật liệu: {bom.rawMaterialId}</span>
                        </div>
                        <div className="card-row">
                            <span className="value">Số lượng: {bom.quantity} {bom.unitOfMeasure}</span>
                        </div>
                      </div>
                    ))
                  ) : (
                    <p className="no-data-text">
                        {detailData ? "Không có thông tin BOM" : "Đang tải dữ liệu..."}
                    </p>
                  )}
                </div>
              </div>

              {/* Cột Phải: CÔNG ĐOẠN (Lấy từ detailData.segmentDTOs) */}
              <div className="detail-box">
                <div className="box-header">
                  <h3>Công đoạn</h3>
                </div>
                <div className="box-content scrollable">
                  {detailData && detailData.segmentDTOs && detailData.segmentDTOs.length > 0 ? (
                    detailData.segmentDTOs.map((seg, index) => (
                      <div key={index} className="detail-item-card">
                        <div className="card-row">
                          <span className="value">Mã Công đoạn: {seg.segmentId}</span>
                        </div>
                        <div className="card-row">
                          <span className="value">Mô tả: {seg.description || '--'}</span>
                        </div>
                        <div className="card-row">
                          <span className="value">Loại công đoạn: {seg.typeSegment || '--'}</span>
                        </div>
                      </div>
                    ))
                  ) : (
                    <p className="no-data-text">
                        {detailData ? "Không có thông tin công đoạn" : "Đang tải dữ liệu..."}
                    </p>
                  )}
                </div>
              </div>
            </div>

            {/* === FOOTER: QUAN HỆ CÔNG ĐOẠN (Giữ nguyên) === */}
            <div className="detail-section footer-section">
              <div className="section-header-row">
                <h3>Quan hệ công đoạn</h3>
                <button className={`btn-create-relation ${!canEdit ? "disabled-button" : ""}`} 
                disabled={!canEdit}
                title={!canEdit ? "Bạn không có quyền tạo mới Quan hệ công đoạn" : "Tạo mới Quan hệ công đoạn"}
                onClick={handleCreateNewRelation}>Tạo quan hệ</button>
              </div>
              <div className="relation-table-container">
                <table className="relation-table">
                  <thead>
                    <tr>
                      <th>STT</th>
                      <th>Từ công đoạn</th>
                      <th>Đến công đoạn</th>
                      <th>Mô tả</th>
                      <th>Loại phụ thuộc</th>
                      <th>Hệ số</th>
                      <th>Đơn vị</th>
                    </tr>
                  </thead>
                  <tbody>
                    {relationsList.length > 0 ? (
                      relationsList.map((rel, index) => (
                        <tr key={rel.id || index}>
                          <td>{index + 1}</td>
                          <td>{rel.fromOperation}</td>
                          <td>{rel.toOperation}</td>
                          <td>{rel.description}</td>
                          <td>{rel.dependencyType}</td>
                          <td>{rel.coefficient}</td>
                          <td>{rel.unit}</td>
                        </tr>
                      ))
                    ) : (
                      <tr>
                        <td colSpan="7" className="no-data-text" style={{padding: '20px'}}>
                          Chưa có quan hệ công đoạn nào.
                        </td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </>
      )}
    </div>
  );
};

export default FormulaDetail;