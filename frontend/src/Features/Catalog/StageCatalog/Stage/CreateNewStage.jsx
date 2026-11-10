import React, { useState, useEffect } from 'react';
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import './CreateNewStage.css';

import { createProcessSegment } from '../../../../api/ProcessSegment';
import { getProductionParameterIds } from '../../../../api/ProductionParameter';
import { getMaterialDefinitionIdList } from '../../../../api/MaterialDefinition';

// Các loại cũ chỉ dùng để hiển thị đẹp.
// Nếu có loại mới, giao diện sẽ hiển thị trực tiếp giá trị người dùng nhập.
const segmentTypeDisplayMap = {
  Drawing: 'Kéo',
  Enameling: 'Tráng men',
  Both: 'Kéo + Tráng men',
};

// Helper xác định valueType
const getValueType = (value) => {
  if (typeof value !== 'string') return 3;

  const trimmed = value.trim();

  if (trimmed === '') return 3;

  if (
    trimmed.toLowerCase() === 'true' ||
    trimmed.toLowerCase() === 'false'
  ) {
    return 0;
  }

  if (isNaN(Number(trimmed))) return 3;
  if (trimmed.includes('.')) return 2;

  return 1;
};

// Chuẩn hóa dữ liệu vật liệu.
// API có thể trả:
//   "MAT01"
// hoặc:
//   { id: "MAT01", name: "Dây đồng..." }
const getMaterialId = (item) => {
  if (typeof item === 'string') return item;

  return (
    item?.id ??
    item?.resourceId ??
    item?.materialDefinitionId ??
    ''
  );
};

const getMaterialLabel = (item) => {
  if (typeof item === 'string') return item;

  const id = getMaterialId(item);
  const name = item?.name ?? '';

  if (id && name) return `${id} - ${name}`;
  return id || name || 'Không xác định';
};

// State ban đầu
const initialStageState = {
  code: '',
  name: '',
  description: '',
  segmentType: '',
  productionParameterId: '',
  outputMaterialDefinitionId: '',
  hierarchyScope: '',
  durationValueString: '',
  durationUnit: '',
};

const CreateNewStage = () => {
  const [stage, setStage] = useState(initialStageState);
  const [stageList, setStageList] = useState([]);

  const [productionParameterIds, setProductionParameterIds] = useState([]);
  const [materialDefinitionIds, setMaterialDefinitionIds] = useState([]);

  // Tải danh sách thông số sản xuất và vật liệu từ backend
  useEffect(() => {
    const getApi = async () => {
      try {
        const [parameterIds, materialIds] = await Promise.all([
          getProductionParameterIds(),
          getMaterialDefinitionIdList(),
        ]);

        setProductionParameterIds(
          Array.isArray(parameterIds) ? parameterIds : []
        );

        setMaterialDefinitionIds(
          Array.isArray(materialIds) ? materialIds : []
        );

        console.log("Production parameter data:", parameterIds);
        console.log("Material definition data:", materialIds);
      } catch (error) {
        console.error("Không thể tải dữ liệu tạo công đoạn:", error);

        setProductionParameterIds([]);
        setMaterialDefinitionIds([]);

        toast.error("Không thể tải danh sách thông số sản xuất hoặc vật liệu!", {
          position: "top-right",
          autoClose: 3000,
        });
      }
    };

    getApi();
  }, []);

  const handleChange = (e) => {
    const { name, value } = e.target;

    setStage((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleAddToList = (e) => {
    e.preventDefault();

    if (
      !stage.code.trim() ||
      !stage.name.trim() ||
      !stage.segmentType.trim() ||
      !stage.productionParameterId ||
      !stage.outputMaterialDefinitionId ||
      !stage.hierarchyScope.trim()
    ) {
      toast.info(
        "Vui lòng điền đầy đủ Mã, Tên, Loại công đoạn, Thông số sản xuất, Vật liệu đầu ra và Hierarchy Scope!",
        {
          position: "top-right",
          autoClose: 3000,
          hideProgressBar: false,
          closeOnClick: true,
          pauseOnHover: true,
          draggable: true,
        }
      );

      return;
    }

    const newStage = {
      ...stage,
      code: stage.code.trim(),
      name: stage.name.trim(),
      description: stage.description.trim(),
      segmentType: stage.segmentType.trim(),
      hierarchyScope: stage.hierarchyScope.trim(),
      tempId: Date.now(),
    };

    setStageList((prev) => [...prev, newStage]);
    setStage(initialStageState);
  };

  const handleDeleteFromList = (tempId) => {
    setStageList((prev) =>
      prev.filter((item) => item.tempId !== tempId)
    );
  };

  const handleConfirm = async () => {
    if (stageList.length === 0) {
      toast.info("Không có công đoạn nào để xác nhận!", {
        position: "top-right",
        autoClose: 3000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
      });

      return;
    }

    const failures = [];

    for (const item of stageList) {
      const durationStr = item.durationValueString || '0';

      const payload = {
        processSegmentId: item.code,
        name: item.name,
        description: item.description,

        // SegmentType động: gửi string thẳng xuống backend
        segmentType: item.segmentType.trim(),

        operationsType: 0,
        hierarchyScope: item.hierarchyScope,
        durationValueString: durationStr,
        valueType: getValueType(durationStr),
        valueUnitOfMeasure: item.durationUnit,

        productionParameterId: item.productionParameterId,
        outputMaterialDefinitionId: item.outputMaterialDefinitionId,
      };

      console.log("Create ProcessSegment payload:", payload);

      try {
        await createProcessSegment(payload);

        toast.success(`Tạo công đoạn ${item.code} thành công!`, {
          position: "top-right",
          autoClose: 3000,
          hideProgressBar: false,
          closeOnClick: true,
          pauseOnHover: true,
          draggable: true,
        });
      } catch (error) {
        console.error(
          `Lỗi khi tạo công đoạn ${item.code}:`,
          error?.response?.data ?? error
        );

        const backendMessage =
          error?.response?.data?.title ||
          error?.response?.data?.message ||
          "Không thể tạo công đoạn!";

        toast.error(`Lỗi: ${backendMessage}`, {
          position: "top-right",
          autoClose: 4000,
          hideProgressBar: false,
          closeOnClick: true,
          pauseOnHover: true,
          draggable: true,
        });

        failures.push(item.code);
      }
    }

    // Chỉ giữ lại các công đoạn tạo thất bại để người dùng thử lại
    setStageList((prev) =>
      prev.filter((item) => failures.includes(item.code))
    );
  };

  const getSegmentTypeDisplay = (segmentType) => {
    return segmentTypeDisplayMap[segmentType] || segmentType || 'N/A';
  };

  return (
    <div className="newstage-container">
      <form onSubmit={handleAddToList}>
        <div>
          <label>Mã công đoạn</label>
          <input
            type="text"
            name="code"
            value={stage.code}
            onChange={handleChange}
            required
          />
        </div>

        <div>
          <label>Tên công đoạn</label>
          <input
            type="text"
            name="name"
            value={stage.name}
            onChange={handleChange}
            required
          />
        </div>

        <div>
          <label>Mô tả</label>
          <input
            type="text"
            name="description"
            value={stage.description}
            onChange={handleChange}
          />
        </div>

        <div>
          <label>Loại công đoạn</label>

          <input
            type="text"
            name="segmentType"
            value={stage.segmentType}
            onChange={handleChange}
            list="segmentTypeSuggestions"
            placeholder="VD: Drawing, Enameling, Packaging..."
            required
          />

          <datalist id="segmentTypeSuggestions">
            <option value="Drawing">Kéo</option>
            <option value="Enameling">Tráng men</option>
            <option value="Both">Kéo + Tráng men</option>
          </datalist>
        </div>

        <div>
          <label>Thông số sản xuất (ID)</label>

          <select
            name="productionParameterId"
            value={stage.productionParameterId}
            onChange={handleChange}
            required
          >
            <option value="" disabled>
              Chọn Thông số sản xuất
            </option>

            {productionParameterIds.map((item, index) => {
              const value =
                typeof item === 'string'
                  ? item
                  : item?.id ?? item?.resourceId ?? '';

              const label =
                typeof item === 'string'
                  ? item
                  : item?.name
                    ? `${value} - ${item.name}`
                    : value;

              return (
                <option key={`${value}-${index}`} value={value}>
                  {label}
                </option>
              );
            })}
          </select>
        </div>

        <div>
          <label>Vật liệu đầu ra</label>

          <select
            name="outputMaterialDefinitionId"
            value={stage.outputMaterialDefinitionId}
            onChange={handleChange}
            required
          >
            <option value="" disabled>
              Chọn vật liệu đầu ra
            </option>

            {materialDefinitionIds.map((item, index) => {
              const materialId = getMaterialId(item);

              return (
                <option
                  key={`${materialId}-${index}`}
                  value={materialId}
                >
                  {getMaterialLabel(item)}
                </option>
              );
            })}
          </select>
        </div>

        <div>
          <label>Hierarchy Scope</label>

          <input
            type="text"
            name="hierarchyScope"
            value={stage.hierarchyScope}
            onChange={handleChange}
            placeholder="VD: Ent, Site, Area"
            required
          />
        </div>

        <div>
          <label>Thời lượng</label>

          <input
            type="number"
            name="durationValueString"
            value={stage.durationValueString}
            onChange={handleChange}
            placeholder="VD: 10"
          />
        </div>

        <div>
          <label>Đơn vị (Thời gian)</label>

          <input
            type="text"
            name="durationUnit"
            value={stage.durationUnit}
            onChange={handleChange}
            placeholder="VD: s, min, h"
          />
        </div>

        <button type="submit" className="create-stage-btn">
          Tạo
        </button>
      </form>

      <div className="stage-list">
        <h2>Danh sách công đoạn đã tạo</h2>

        <div className="table-newstage-container">
          <table>
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã</th>
                <th>Tên</th>
                <th>Mô tả</th>
                <th>Loại</th>
                <th>Thông số SX (ID)</th>
                <th>Vật liệu đầu ra</th>
                <th>Scope</th>
                <th>Thời lượng</th>
                <th>Đơn vị</th>
                <th>Xóa</th>
              </tr>
            </thead>

            <tbody>
              {stageList.length === 0 ? (
                <tr>
                  <td colSpan="11" style={{ textAlign: 'center' }}>
                    Chưa có công đoạn nào.
                  </td>
                </tr>
              ) : (
                stageList.map((item, index) => (
                  <tr key={item.tempId}>
                    <td>{index + 1}</td>
                    <td>{item.code}</td>
                    <td>{item.name}</td>
                    <td>{item.description}</td>
                    <td>{getSegmentTypeDisplay(item.segmentType)}</td>
                    <td>{item.productionParameterId}</td>
                    <td>{item.outputMaterialDefinitionId}</td>
                    <td>{item.hierarchyScope}</td>
                    <td>{item.durationValueString}</td>
                    <td>{item.durationUnit}</td>

                    <td>
                      <button
                        type="button"
                        className="delete-item-btn"
                        onClick={() =>
                          handleDeleteFromList(item.tempId)
                        }
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
          onClick={handleConfirm}
          disabled={stageList.length === 0}
        >
          Xác nhận ({stageList.length})
        </button>
      </div>
    </div>
  );
};

export default CreateNewStage;
