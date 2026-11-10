namespace MesMicroservice.Infrastructure.EntityConfigurations.OrderModelConfigurations;
public class WorkOrderEntityTypeConfiguration : IEntityTypeConfiguration<WorkOrder>
{
    public void Configure(EntityTypeBuilder<WorkOrder> builder)
    {
        builder.ToTable(nameof(WorkOrder));

        builder.Property(w => w.Id)
            .UseHiLo("workorderseq", ApplicationDbContext.DEFAULT_SCHEMA);

        builder.Property(w => w.WorkOrderId)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(w => w.WorkOrderId).IsUnique();

        builder.Property(w => w.ProductionTarget)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(w => w.Unit)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(w => w.DueDate)
            .IsRequired();

        builder.Property(w => w.Duration)
            .IsRequired();

        builder.Property(w => w.WorkOrderStatus)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(w => w.SetupTime).IsRequired();
        builder.Property(w => w.TeardownTime).IsRequired();
        builder.Property(w => w.RunTime).IsRequired();

        builder.Property(w => w.SequenceNumber);

        // 🔸 Relationships
        builder.HasOne(w => w.OperationsSegment)
            .WithMany(m => m.WorkOrders)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.WorkCenter)
            .WithMany(w => w.WorkOrders)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.ManufacturingOrder)
            .WithMany(m => m.WorkOrders)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(w => w.MaterialWorkOrders)
            .WithOne(x => x.WorkOrder)
            .HasForeignKey(x => x.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(w => w.EquipmentWorkOrders)
            .WithOne(x => x.WorkOrder)
            .HasForeignKey(x => x.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(w => w.PersonWorkOrders)
            .WithOne(x => x.WorkOrder)
            .HasForeignKey(x => x.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);


    }
}
