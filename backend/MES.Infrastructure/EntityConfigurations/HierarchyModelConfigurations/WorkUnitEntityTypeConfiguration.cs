using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

namespace MesMicroservice.Infrastructure.EntityConfigurations.HierarchyModelConfigurations;
public class WorkUnitEntityTypeConfiguration : IEntityTypeConfiguration<WorkUnit>
{
    public void Configure(EntityTypeBuilder<WorkUnit> builder)
    {
        builder.ToTable("WorkUnits");
        builder
            .Property(x => x.Id)
            .UseHiLo("workuniteq", ApplicationDbContext.DEFAULT_SCHEMA);
    }
}
