import api from "./Axios";
export const getProcessSegments = async (pageIndex = 1, pageSize = 30) => {
  const response = await api.get('ProcessSegment/GetProcessSegments', {
    params: {
      pageIndex,
      pageSize
    }
  });
  return response.data;
};
export const createProcessSegment = async (SegmentData) => {
  const response = await api.post('ProcessSegment/CreateProcessSegment', SegmentData);
  return response.data;
}
export const getProcessSegmentDetail = async (processSegmentId) => {
  const response = await api.get(
    `ProcessSegment/GetProcessSegmentById/${processSegmentId}`
  );
  return response.data;
}

export const getProcessSegmentIds = async () => {
  const response = await api.get('ProcessSegment/GetProcessSegmentIds');
  return response.data;
}

export const getEquipmentSegmentSpecificationsByProcessSegmentId = async (processSegmentId) => {
  const response = await api.get(
    `ProcessSegment/GetEquipmentSegmentSpecificationsByProcessSegmentId/${processSegmentId}`
  );
  return response.data;
}

export const getMaterialSegmentSpecificationsByProcessSegmentId = async (processSegmentId) => {
  const response = await api.get(
    `ProcessSegment/GetMaterialSegmentSpecificationsByProcessSegmentId/${processSegmentId}`
  );
  return response.data;
} 

export const getPersonnelSegmentSpecificationsByProcessSegmentId = async (processSegmentId) => {
  const response = await api.get(
    `ProcessSegment/GetPersonnelSegmentSpecificationsByProcessSegmentId/${processSegmentId}`
  );
  return response.data;
}