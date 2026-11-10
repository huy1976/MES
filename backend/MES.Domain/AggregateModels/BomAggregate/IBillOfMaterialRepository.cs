namespace MesMicroservice.Domain.AggregateModels.BomAggregate
{
    public interface IBillOfMaterialRepository : IRepository<BillOfMaterial>
    {
        public Task<BillOfMaterial> Add(BillOfMaterial billOfMaterial);
        public Task<BillOfMaterial?> GetAsync(string billOfMaterialId);

        public Task<IEnumerable<BillOfMaterial>> GetBillOfMaterials();
        public Task<List<BillOfMaterial>> GetListByIdAsync(List<string> billOfMaterialIds);
        public BillOfMaterial Update(BillOfMaterial billOfMaterial);
        public Task<bool> ExistsAsync(string billOfMaterialId);
        public Task DeleteAsync(string billOfMaterialId);
    }
}
