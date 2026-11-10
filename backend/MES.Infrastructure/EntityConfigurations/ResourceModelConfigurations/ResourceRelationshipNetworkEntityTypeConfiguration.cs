using MesMicroservice.Domain.AggregateModels.ResourceRelationshipNetworkAggregate;

namespace MesMicroservice.Infrastructure.EntityConfigurations.ResourceModelConfigurations;
public class ResourceRelationshipNetworkEntityTypeConfiguration : IEntityTypeConfiguration<ResourceRelationshipNetwork>
{
    public void Configure(EntityTypeBuilder<ResourceRelationshipNetwork> builder)
    {
        builder.HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .UseHiLo("resourcerelationshipnetworkeq", ApplicationDbContext.DEFAULT_SCHEMA);

        builder.HasIndex(x => x.ResourceRelationshipNetworkId).IsUnique();
        builder.Property(x => x.ResourceRelationshipNetworkId).HasMaxLength(50).IsRequired();

        builder.OwnsMany(x => x.Connections, cb =>
        {
            cb.HasKey(c => c.Id);
            cb.Property(c => c.Id).UseHiLo("resourcenetworkconnectioneq", ApplicationDbContext.DEFAULT_SCHEMA);

            cb.HasIndex(c => c.ConnectionId).IsUnique();

            cb.HasOne(c => c.FromResource).WithMany().HasForeignKey(c => c.FromResourceId).IsRequired().OnDelete(DeleteBehavior.NoAction);
            cb.HasOne(c => c.ToResource).WithMany().HasForeignKey(c => c.ToResourceId).IsRequired().OnDelete(DeleteBehavior.NoAction);

            cb.OwnsMany(x => x.Properties, pb =>
            {
                pb.HasKey(p => p.Id);
                pb.WithOwner().HasForeignKey(p => p.ConnectionId);

                pb.HasIndex(p => new { p.ConnectionId, p.PropertyId }).IsUnique();

                pb.OwnsOne(p => p.Value);

                pb.Property(p => p.PropertyId).HasMaxLength(50).IsRequired();
                pb.Property(p => p.Description).HasMaxLength(500).IsRequired();
                pb.Property(p => p.ValueUnitOfMeasure).HasMaxLength(30).IsRequired();
            });
        });
    }
}
