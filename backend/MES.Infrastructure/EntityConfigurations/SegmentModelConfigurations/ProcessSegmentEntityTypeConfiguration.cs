global using MesMicroservice.Domain.AggregateModels.SegmentAggregate;

namespace MesMicroservice.Infrastructure.EntityConfigurations.SegmentModelConfigurations
{
    public class ProcessSegmentEntityTypeConfiguration : IEntityTypeConfiguration<ProcessSegment>
    {
        public void Configure(EntityTypeBuilder<ProcessSegment> builder)
        {
            builder.ToTable(nameof(ProcessSegment));

            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.OwnsOne(x => x.Duration);

            builder.HasOne(x => x.ProductionParameter)
                .WithMany(x => x.ProcessSegments)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
