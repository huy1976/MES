import api from "./Axios";
export const getBusinesses = async (pageIndex = 1, pageSize = 30) => {
  const response = await api.get('BusinessPartner/GetBusinessPartners', {
    params: {
      pageIndex,
      pageSize
    }
  });
  return response.data;
};
export const createBusinesses = async (BuisnessData) => {
  const response = await api.post('BusinessPartner', BuisnessData);
  return response.data;
}

export const getBusinessPartnerPropertyNames = async () => {
  const response = await api.get('BusinessPartner/GetBusinessPartnerPropertyNames');
  return response.data;
}
