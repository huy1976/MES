import api from "./Axios";
export const getMaterialSegmentSpecifications = async (pageIndex = 1, pageSize = 30) => {
  const response = await api.get('ProcessSegment/GetMaterialSegmentSpecifications', {
    params: {
      pageIndex,
      pageSize
    }
  });
  return response.data;
};
export const getPersonnelSegmentSpecifications = async (pageIndex = 1, pageSize = 30) => {
  const response = await api.get('ProcessSegment/GetPersonnelSegmentSpecifications', {
    params: {
      pageIndex,
      pageSize
    }
  });
  return response.data;
};
export const getEquipmentSegmentSpecifications = async (pageIndex = 1, pageSize = 30) => {
  const response = await api.get('ProcessSegment/GetEquipmentSegmentSpecifications', {
    params: {
      pageIndex,
      pageSize
    }
  });
  return response.data;
};

export const getEquipmentSegmentSpecificationPropertyNames = async () => {
  const response = await api.get('ProcessSegment/GetEquipmentSegmentSpecificationPropertyNames');
  return response.data;
}

export const createEquipmentSegmentSpecification = async (specificationData) => {
  const response = await api.post('ProcessSegment/AddEquipmentSegmentSpecifications', specificationData);
  return response.data;
}

export const getPersonnelSegmentSpecificationPropertyNames = async () => {
  const response = await api.get('ProcessSegment/GetPersonnelSegmentSpecificationPropertyNames');
  return response.data;
}

export const createPersonnelSegmentSpecification = async (specificationData) => {
  const response = await api.post('ProcessSegment/AddPersonnelSegmentSpecifications', specificationData);
  return response.data;
}

export const getMaterialSegmentSpecificationPropertyNames = async () => {
  const response = await api.get('ProcessSegment/GetMaterialSegmentSpecificationPropertyNames');
  return response.data;
}

export const createMaterialSegmentSpecification = async (specificationData) => {
  const response = await api.post('ProcessSegment/AddMaterialSegmentSpecifications', specificationData);
  return response.data;
}