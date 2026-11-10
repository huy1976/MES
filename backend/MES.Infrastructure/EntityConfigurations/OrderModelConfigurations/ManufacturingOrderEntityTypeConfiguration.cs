namespace MesMicroservice.Infrastructure.EntityConfigurations.OrderModelConfigurations;
public class ManufacturingOrderEntityTypeConfiguration : IEntityTypeConfiguration<ManufacturingOrder>
{
    public void Configure(EntityTypeBuilder<ManufacturingOrder> builder)
    {
        builder.ToTable(nameof(ManufacturingOrder));

        builder.Property(s => s.Id)
            .UseHiLo("manufacturingorderseq", ApplicationDbContext.DEFAULT_SCHEMA);

        builder.Property(m => m.ManufacturingOrderId)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(m => m.ManufacturingOrderId)
            .IsUnique();

        builder.Property(m => m.Quantity)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(m => m.Unit)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(m => m.AvailableDate)
            .IsRequired();

        builder.Property(m => m.DueDate)
            .IsRequired();

        builder.Property(m => m.PlanedStartDate);
        builder.Property(m => m.PlanedEnDate);

        builder.Property(m => m.Priority)
            .HasDefaultValue(0);

        builder.Property(m => m.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(m => m.CreateDate)
            .HasDefaultValueSql("GETUTCDATE()");

        // 🔸 Relationships
        builder.HasOne(m => m.MaterialDefinition)
            .WithMany(m => m.ManufacturingOrders)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Customer)
            .WithMany(m => m.ManufacturingOrders)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.OperationsDefinition)
            .WithMany(m => m.ManufacturingOrders)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.QualityStandard)
            .WithMany(m => m.ManufacturingOrders)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.CreatedBy)
            .WithMany(m => m.ManufacturingOrders)
            .OnDelete(DeleteBehavior.Restrict);

        
    }
}
