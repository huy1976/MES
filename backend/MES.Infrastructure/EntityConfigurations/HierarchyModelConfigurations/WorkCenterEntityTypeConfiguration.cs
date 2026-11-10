using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

namespace MesMicroservice.Infrastructure.EntityConfigurations.HierarchyModelConfigurations;
public class WorkCenterEntityTypeConfiguration : IEntityTypeConfiguration<WorkCenter>
{
    public void Configure(EntityTypeBuilder<WorkCenter> builder)
    {
        builder.ToTable("WorkCenters");
        builder
            .Property(x => x.Id)
            .UseHiLo("workcentereq", ApplicationDbContext.DEFAULT_SCHEMA);

        builder.HasMany(x => x.WorkUnits).WithOne(x => (WorkCenter?)x.Parent);
        builder.HasMany(x => x.Outputs).WithOne(x => x.WorkCenter).HasForeignKey(x => x.WorkCenterId);

        builder.Property(x => x.WorkCenterType).IsRequired();
    }
}
