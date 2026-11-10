using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

namespace MesMicroservice.Infrastructure.EntityConfigurations.HierarchyModelConfigurations;
public class HierarchyModelEntityTypeConfiguration : IEntityTypeConfiguration<HierarchyModel>
{
    public void Configure(EntityTypeBuilder<HierarchyModel> builder)
    {
        builder.HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .UseHiLo("hierarchymodeleq", ApplicationDbContext.DEFAULT_SCHEMA);

        builder.HasIndex(x => x.HierarchyModelId);
        builder.Property(x => x.HierarchyModelId).HasMaxLength(50).IsRequired();

        builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
    }
}
