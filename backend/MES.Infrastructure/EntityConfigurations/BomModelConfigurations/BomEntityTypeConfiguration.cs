global using MesMicroservice.Domain.AggregateModels.BomAggregate;

namespace MesMicroservice.Infrastructure.EntityConfigurations.BomModelConfigurations
{
    public class BomEntityTypeConfiguration : IEntityTypeConfiguration<BillOfMaterial>
    {
        public void Configure(EntityTypeBuilder<BillOfMaterial> builder)
        {
            builder.ToTable(nameof(BillOfMaterial));

            builder.OwnsOne(x => x.Quantity);

            builder.HasOne(x => x.MaterialDefinition)
                .WithMany(x => x.BillOfMaterials)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
