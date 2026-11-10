// api/OperationsDefinition.js
import api from "./Axios";

export const getOperationsDefinitions = async (materialId) => {
  const response = await api.get(
    `OperationsDefinition/GetOperationsDefinitionsByMaterialId/${materialId}`
  );
  return response.data;
};

export const createOperationsDefinition = async (operationsDefinitions) => {
  const response = await api.post('OperationsDefinition/CreateOperationsDefinition', operationsDefinitions);
  return response.data;
}
export const createOperationRelationship = async (relationship) => {
  try {
    const response = await api.post('OperationsDefinition/CreateOperationSegmentDependency', relationship);
    return response.data; 
  } catch (error) {
    console.error("Lỗi khi tạo quan hệ công đoạn:", error);
    throw error;
  }
}
