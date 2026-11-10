using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

namespace MesMicroservice.Infrastructure.EntityConfigurations.HierarchyModelConfigurations;
public class AreaEntityTypeConfiguration : IEntityTypeConfiguration<Area>
{
    public void Configure(EntityTypeBuilder<Area> builder)
    {
        builder.ToTable("Areas");
        builder
            .Property(x => x.Id)
            .UseHiLo("areaeq", ApplicationDbContext.DEFAULT_SCHEMA);

        builder.HasMany(x => x.WorkCenters).WithOne(x => (Area?)x.Parent);
    }
}
