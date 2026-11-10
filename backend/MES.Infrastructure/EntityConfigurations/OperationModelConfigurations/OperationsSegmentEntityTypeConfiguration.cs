namespace MesMicroservice.Infrastructure.EntityConfigurations.OperationModelConfigurations
{
    public class OperationsSegmentEntityTypeConfiguration : IEntityTypeConfiguration<OperationsSegment>
    {
        public void Configure(EntityTypeBuilder<OperationsSegment> builder)
        {
            builder.ToTable(nameof(OperationsSegment));

            builder.Property(x => x.SegmentId).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(1000);
            builder.Property(x => x.ProductionParameterId).IsRequired();
            builder.OwnsOne(x => x.Duration);

            builder.HasOne(x => x.OperationsDefinition)
                .WithMany(x => x.OperationsSegments)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
