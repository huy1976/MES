namespace MesMicroservice.Infrastructure.EntityConfigurations.OperationModelConfigurations
{
    public class MaterialSpecificationEntityTypeConfiguration : IEntityTypeConfiguration<MaterialSpecification>
    {
        public void Configure(EntityTypeBuilder<MaterialSpecification> builder)
        {
            builder.ToTable(nameof(MaterialSpecification));

            builder.Property(x => x.MaterialDefinitionId).IsRequired().HasMaxLength(200);
            builder.Property(x => x.MaterialSegmentSpecificationId).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(1000);
            builder.OwnsOne(x => x.Quantity);

            builder.OwnsMany(x => x.Properties, pb =>
            {
                pb.HasKey(p => p.Id);
                pb.WithOwner().HasForeignKey(p => p.MaterialSpecificationId);

                pb.HasIndex(p => new { p.MaterialSpecificationId, p.PropertyId }).IsUnique();

                pb.OwnsOne(p => p.Value);

                pb.Property(p => p.PropertyId).HasMaxLength(50).IsRequired();
                pb.Property(p => p.Description).HasMaxLength(500).IsRequired();
                pb.Property(p => p.ValueUnitOfMeasure).HasMaxLength(30).IsRequired();
            });

            builder.HasOne(x => x.OperationsSegment)
                .WithMany(x => x.MaterialSpecifications)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
