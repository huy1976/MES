import api from "./Axios";
export const getEnterprises = async (pageIndex = 1, pageSize = 30) => {
  const response = await api.get('Enterprises/GetEnterprises', {
    params: {
      pageIndex,
      pageSize
    }
  });
  return response.data;
};

export const getSites = async (pageIndex = 1, pageSize = 30) => {
  const response = await api.get('Enterprises/GetSites', {
    params: {
      pageIndex,
      pageSize
    }
  });
  return response.data;
};

export const createSite = async (enterpriseId, data) => {
  const response = await api.post(`Enterprises/${enterpriseId}`, data);
  return response.data;
}

export const createWorkCenter = async (enterpriseId, siteId, areaId, data) => {
  const response = await api.post(`Enterprises/${enterpriseId}/${siteId}/${areaId}`, data);
  return response.data;
}

export const getEnterpriseIds = async () => {
  const response = await api.get('Enterprises/Ids');
  return response.data;
}

export const getSiteIds = async () => {
  const response = await api.get('Enterprises/GetSiteIds');
  return response.data;
}

export const getAreaIds = async () => {
  const response = await api.get('Enterprises/GetAreaIds');
  return response.data;
}