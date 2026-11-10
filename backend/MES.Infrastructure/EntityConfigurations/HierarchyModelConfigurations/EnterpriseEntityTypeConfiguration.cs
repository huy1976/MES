using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

namespace MesMicroservice.Infrastructure.EntityConfigurations.HierarchyModelConfigurations;
public class EnterpriseEntityTypeConfiguration : IEntityTypeConfiguration<Enterprise>
{
    public void Configure(EntityTypeBuilder<Enterprise> builder)
    {
        builder.ToTable("Enterprises");
        builder
            .Property(x => x.Id)
            .UseHiLo("enterpriseeq", ApplicationDbContext.DEFAULT_SCHEMA);

        builder.HasMany(x => x.Sites).WithOne(x => (Enterprise?)x.Parent);
    }
}
