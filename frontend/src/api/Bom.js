import api from "./Axios";
export const getBillOfMaterials = async (pageIndex = 1, pageSize = 30) => {
  const response = await api.get('BillOfMaterial/GetBillOfMaterials', {
    params: {
      pageIndex,
      pageSize
    }
  });
  return response.data;
};
export const createBillOfMaterial = async (bomData) => {
  const response = await api.post('BillOfMaterial/CreateBillOfMaterial', bomData);
  return response.data;
}

export const getBillOfMaterialIds = async () => {
  const response = await api.get('BillOfMaterial/GetBillOfMaterialIds');
  return response.data;
};