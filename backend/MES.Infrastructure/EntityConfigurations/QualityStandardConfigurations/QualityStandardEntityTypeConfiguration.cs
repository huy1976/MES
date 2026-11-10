global using MesMicroservice.Domain.AggregateModels.QualityStandardAggregate;

namespace MesMicroservice.Infrastructure.EntityConfigurations.QualityStandardConfigurations
{
    public class QualityStandardEntityTypeConfiguration : IEntityTypeConfiguration<QualityStandard>
    {
        public void Configure(EntityTypeBuilder<QualityStandard> builder)
        {
            builder.ToTable(nameof(QualityStandard));

            builder.OwnsMany(x => x.Properties, pb =>
            {
                pb.HasKey(p => p.Id);
                pb.WithOwner().HasForeignKey(p => p.QualityStandardId);

                pb.HasIndex(p => new { p.QualityStandardId, p.PropertyId }).IsUnique();

                pb.OwnsOne(p => p.Value);

                pb.Property(p => p.PropertyId)
                    .HasMaxLength(50)
                    .IsRequired();

                pb.Property(p => p.Description)
                    .HasMaxLength(500)
                    .IsRequired();

                pb.Property(p => p.ValueUnitOfMeasure)
                    .HasMaxLength(30)
                    .IsRequired();
            });

            builder.HasMany(x => x.MaterialDefinitions)
                .WithMany(x => x.QualityStandards);

        }
    }
}
