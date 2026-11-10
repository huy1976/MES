using MesMicroservice.Domain.AggregateModels.MaterialDefinitionAggregate;

namespace MesMicroservice.Infrastructure.EntityConfigurations.MaterialModelConfigurations;
public class MaterialDefinitionEntityTypeConfiguration : IEntityTypeConfiguration<MaterialDefinition>
{
    public void Configure(EntityTypeBuilder<MaterialDefinition> builder)
    {
        builder.ToTable(nameof(MaterialDefinition));

        builder.OwnsMany(x => x.Properties, pb =>
        {
            pb.HasKey(p => p.Id);
            pb.WithOwner().HasForeignKey(p => p.MaterialDefinitionId);

            pb.HasIndex(p => new { p.MaterialDefinitionId, p.PropertyId }).IsUnique();

            pb.OwnsOne(p => p.Value);

            pb.Property(p => p.PropertyId).HasMaxLength(50).IsRequired();
            pb.Property(p => p.Description).HasMaxLength(500).IsRequired();
            pb.Property(p => p.ValueUnitOfMeasure).HasMaxLength(30).IsRequired();
        });

        builder.HasOne(x => x.MaterialClass).WithMany(x => x.MaterialDefinitions).IsRequired().OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(x => x.SecondaryUnits).WithOne().HasForeignKey(x => x.MaterialDefinitionId).OnDelete(DeleteBehavior.NoAction);

        builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
        builder.Property(x => x.PrimaryUnit).IsRequired();
    }
}
