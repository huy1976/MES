namespace MesMicroservice.Domain.AggregateModels.ManufacucturingRecordAggregate;
public interface IManufacturingRecordRepository: IRepository<ManufacturingRecord>
{
    public ManufacturingRecord Add(ManufacturingRecord record);

    public Task<IEnumerable<ManufacturingRecord>> GetAllAsync();
}
