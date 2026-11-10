using MesMicroservice.Domain.AggregateModels.MaterialClassAggregate;

namespace MesMicroservice.Infrastructure.EntityConfigurations.MaterialModelConfigurations;

public class MaterialClassEntityTypeConfiguration : IEntityTypeConfiguration<MaterialClass>
{
    public void Configure(EntityTypeBuilder<MaterialClass> builder)
    {
        builder.ToTable(nameof(MaterialClass));

        builder.Property(x => x.Name).HasMaxLength(250).IsRequired();

        builder.OwnsMany(x => x.Properties, pb =>
        {
            pb.HasKey(p => p.Id);
            pb.WithOwner().HasForeignKey(p => p.MaterialClassId);

            pb.HasIndex(p => new { p.MaterialClassId, p.PropertyId }).IsUnique();

            pb.OwnsOne(p => p.Value);

            pb.Property(p => p.PropertyId).HasMaxLength(50).IsRequired();
            pb.Property(p => p.Description).HasMaxLength(500).IsRequired();
            pb.Property(p => p.ValueUnitOfMeasure).HasMaxLength(30).IsRequired();
        });
    }
}
