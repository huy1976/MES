namespace MesMicroservice.Infrastructure.EntityConfigurations.OrderModelConfigurations
{
    public class WorkOrderProgressRecordEntityTypeConfiguration
        : IEntityTypeConfiguration<WorkOrderProgressRecord>
    {
        public void Configure(EntityTypeBuilder<WorkOrderProgressRecord> builder)
        {
            builder.ToTable(nameof(WorkOrderProgressRecord));

            builder.HasKey(x => x.Id);

        }
    }
}
