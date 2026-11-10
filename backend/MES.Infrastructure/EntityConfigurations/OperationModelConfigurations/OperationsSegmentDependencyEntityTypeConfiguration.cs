namespace MesMicroservice.Infrastructure.EntityConfigurations.OperationModelConfigurations
{
    public class OperationsSegmentDependencyEntityTypeConfiguration : IEntityTypeConfiguration<OperationsSegmentDependency>
    {
        public void Configure(EntityTypeBuilder<OperationsSegmentDependency> builder)
        {
            builder.ToTable(nameof(OperationsSegmentDependency));

            builder.Property(x => x.Description)
                .HasMaxLength(500);
            builder.Property(x => x.FromSegmentId).IsRequired().HasMaxLength(200);
            builder.Property(x => x.ToSegmentId).IsRequired().HasMaxLength(200);

            builder.OwnsOne(x => x.DependencyFactor);
        }
    }
}
