using MesMicroservice.Domain.AggregateModels.ManufacucturingRecordAggregate;

namespace MesMicroservice.Infrastructure.EntityConfigurations.OrderModelConfigurations;
public class ManufacturingRecordEntityTypeConfiguration : IEntityTypeConfiguration<ManufacturingRecord>
{
    public void Configure(EntityTypeBuilder<ManufacturingRecord> builder)
    {
        builder.HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .UseHiLo("manufacturingrecordeq", ApplicationDbContext.DEFAULT_SCHEMA);

        builder.HasIndex(x => new { x.WorkOrderId, x.StartTime }).IsUnique();

        builder.HasOne(x => x.WorkOrder)
            .WithMany(x => x.ManufacturingRecords)
            .HasForeignKey(x => x.WorkOrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.OutputMaterialDefinition).WithMany().OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(x => x.Equipments).WithMany();

        builder.Property(x => x.Defects).HasPrecision(30, 8).IsRequired();
        builder.Property(x => x.Output).HasPrecision(30, 8).IsRequired();
    }
}
