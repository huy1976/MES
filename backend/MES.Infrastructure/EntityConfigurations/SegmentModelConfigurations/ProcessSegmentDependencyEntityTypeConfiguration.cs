namespace MesMicroservice.Infrastructure.EntityConfigurations.SegmentModelConfigurations
{
    public class ProcessSegmentDependencyEntityTypeConfiguration : IEntityTypeConfiguration<ProcessSegmentDependency>
    {
        public void Configure(EntityTypeBuilder<ProcessSegmentDependency> builder)
        {
            builder.ToTable(nameof(ProcessSegmentDependency));

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.FromSegmentId).IsRequired().HasMaxLength(200);
            builder.Property(x => x.ToSegmentId).IsRequired().HasMaxLength(200);


            builder.OwnsOne(x => x.DependencyFactor);
        }
    }
}
