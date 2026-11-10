using MesMicroservice.Domain.AggregateModels.WorkCenterOutputAggregate;

namespace MesMicroservice.Infrastructure.EntityConfigurations.HierarchyModelConfigurations;
public class WorkCenterOutputEntityTypeConfiguration : IEntityTypeConfiguration<WorkCenterOutput>
{
    public void Configure(EntityTypeBuilder<WorkCenterOutput> builder)
    {
        builder.HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .UseHiLo("workcenteroutputeq", ApplicationDbContext.DEFAULT_SCHEMA);

        builder.HasIndex(x => new { x.WorkCenterId, x.MaterialDefinitionId }).IsUnique();

        builder.HasOne(x => x.MaterialDefinition).WithMany().HasForeignKey(x => x.MaterialDefinitionId);
        builder.HasOne(x => x.Unit).WithMany();

        builder.Property(x => x.Output).HasPrecision(30, 8).IsRequired();
    }
}
