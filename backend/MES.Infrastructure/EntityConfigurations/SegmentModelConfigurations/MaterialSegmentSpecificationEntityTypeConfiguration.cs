namespace MesMicroservice.Infrastructure.EntityConfigurations.SegmentModelConfigurations
{
    public class MaterialSegmentSpecificationEntityTypeConfiguration : IEntityTypeConfiguration<MaterialSegmentSpecification>
    {
        public void Configure(EntityTypeBuilder<MaterialSegmentSpecification> builder)
        {
            builder.ToTable(nameof(MaterialSegmentSpecification));

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.OwnsOne(x => x.Quantity);

            builder.OwnsMany(x => x.Properties, pb =>
            {
                pb.HasKey(p => p.Id);
                pb.WithOwner().HasForeignKey(p => p.MaterialSegmentSpecificationId);

                pb.HasIndex(p => new { p.MaterialSegmentSpecificationId, p.PropertyId }).IsUnique();

                pb.OwnsOne(p => p.Value);

                pb.Property(p => p.PropertyId).HasMaxLength(50).IsRequired();
                pb.Property(p => p.Description).HasMaxLength(500).IsRequired();
                pb.Property(p => p.ValueUnitOfMeasure).HasMaxLength(30).IsRequired();
            });

            builder.HasOne(x => x.ProcessSegment)
                .WithMany(x => x.MaterialSegmentSpecifications)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.MaterialDefinition)
                .WithMany(x => x.MaterialSegmentSpecifications)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
