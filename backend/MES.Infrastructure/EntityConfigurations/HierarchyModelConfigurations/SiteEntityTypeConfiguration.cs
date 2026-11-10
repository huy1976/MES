using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

namespace MesMicroservice.Infrastructure.EntityConfigurations.HierarchyModelConfigurations;
public class SiteEntityTypeConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.ToTable("Sites");
        builder
            .Property(x => x.Id)
            .UseHiLo("siteeq", ApplicationDbContext.DEFAULT_SCHEMA);

        builder.HasMany(x => x.Areas).WithOne(x => (Site?)x.Parent);
    }
}
