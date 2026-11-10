namespace MesMicroservice.Infrastructure.EntityConfigurations.OrderModelConfigurations
{
    public class WorkOrderErrorRecordEntityTypeConfiguration : IEntityTypeConfiguration<WorkOrderErrorRecord>
    {
        public void Configure(EntityTypeBuilder<WorkOrderErrorRecord> builder)
        {
            builder.ToTable(nameof(WorkOrderErrorRecord));

            builder.HasKey(x => x.Id);

        }
    }
}
