namespace MesMicroservice.Domain.AggregateModels.MaterialLotAggregate;
public interface IMaterialLotRepository : IRepository<MaterialLot>
{
    public Task<MaterialLot> Add(MaterialLot materialLot);
    public Task<MaterialLot?> GetAsync(string materialLotId);

    public Task<IEnumerable<MaterialLot>> GetMaterialLots();
    public MaterialLot Update(MaterialLot materialLot);
    public Task<bool> ExistsAsync(string materialLotId);
    public Task DeleteAsync(string materialLotId);
}
