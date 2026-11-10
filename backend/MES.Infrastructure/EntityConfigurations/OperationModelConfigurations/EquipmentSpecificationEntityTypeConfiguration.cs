namespace MesMicroservice.Infrastructure.EntityConfigurations.OperationModelConfigurations
{
    public class EquipmentSpecificationEntityTypeConfiguration : IEntityTypeConfiguration<EquipmentSpecification>
    {
        public void Configure(EntityTypeBuilder<EquipmentSpecification> builder)
        {
            builder.ToTable(nameof(EquipmentSpecification));

            builder.Property(x => x.EquipmentClassId).IsRequired().HasMaxLength(200);
            builder.Property(x => x.EquipmentSegmentSpecificationId).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(1000);
            builder.OwnsOne(x => x.Quantity);

            builder.OwnsMany(x => x.Properties, pb =>
            {
                pb.HasKey(p => p.Id);
                pb.WithOwner().HasForeignKey(p => p.EquipmentSpecificationId);

                pb.HasIndex(p => new { p.EquipmentSpecificationId, p.PropertyId }).IsUnique();

                pb.OwnsOne(p => p.Value);

                pb.Property(p => p.PropertyId).HasMaxLength(50).IsRequired();
                pb.Property(p => p.Description).HasMaxLength(500).IsRequired();
                pb.Property(p => p.ValueUnitOfMeasure).HasMaxLength(30).IsRequired();
            });

            builder.HasOne(x => x.OperationsSegment)
                .WithMany(x => x.EquipmentSpecifications)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
