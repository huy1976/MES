global using MesMicroservice.Domain.AggregateModels.BusinessPartnerAggregate;

namespace MesMicroservice.Infrastructure.EntityConfigurations.BusinessPartnerConfigurations
{
    public class BusinessPartnerEntityTypeConfiguration : IEntityTypeConfiguration<BusinessPartner>
    {
        public void Configure(EntityTypeBuilder<BusinessPartner> builder)
        {
            builder.ToTable(nameof(BusinessPartner));

            builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(500);

            builder.OwnsMany(x => x.Properties, pb =>
            {
                pb.HasKey(p => p.Id);
                pb.WithOwner().HasForeignKey(p => p.BusinessPartnerId);

                pb.HasIndex(p => new { p.BusinessPartnerId, p.PropertyId }).IsUnique();

                pb.OwnsOne(p => p.Value);

                pb.Property(p => p.PropertyId).HasMaxLength(50).IsRequired();
                pb.Property(p => p.Description).HasMaxLength(500).IsRequired();
                pb.Property(p => p.ValueUnitOfMeasure).HasMaxLength(30).IsRequired();

            });
        }
    }
}
