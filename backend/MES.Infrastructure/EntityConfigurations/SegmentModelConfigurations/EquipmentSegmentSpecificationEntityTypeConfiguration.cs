namespace MesMicroservice.Infrastructure.EntityConfigurations.SegmentModelConfigurations
{
    public class EquipmentSegmentSpecificationEntityTypeConfiguration : IEntityTypeConfiguration<EquipmentSegmentSpecification>
    {
        public void Configure(EntityTypeBuilder<EquipmentSegmentSpecification> builder)
        {
            builder.ToTable(nameof(EquipmentSegmentSpecification));

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.OwnsOne(x => x.Quantity);

            builder.OwnsMany(x => x.Properties, pb =>
            {
                pb.HasKey(p => p.Id);
                pb.WithOwner().HasForeignKey(p => p.EquipmentSegmentSpecificationId);

                pb.HasIndex(p => new { p.EquipmentSegmentSpecificationId, p.PropertyId }).IsUnique();

                pb.OwnsOne(p => p.Value);

                pb.Property(p => p.PropertyId).HasMaxLength(50).IsRequired();
                pb.Property(p => p.Description).HasMaxLength(500).IsRequired();
                pb.Property(p => p.ValueUnitOfMeasure).HasMaxLength(30).IsRequired();
            });

            builder.HasOne(x => x.ProcessSegment)
                .WithMany(x => x.EquipmentSegmentSpecifications)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.EquipmentClass)
                .WithMany(x => x.EquipmentSegmentSpecifications)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
