global using MesMicroservice.Domain.AggregateModels.ParameterAggregate;

namespace MesMicroservice.Infrastructure.EntityConfigurations.ProductionParameterConfigurations
{
    public class ProductionParameterEntityTypeConfiguration : IEntityTypeConfiguration<ProductionParameter>
    {
        public void Configure(EntityTypeBuilder<ProductionParameter> builder)
        {
            builder.ToTable(nameof(ProductionParameter));

            builder.OwnsMany(x => x.Properties, pb =>
            {
                pb.HasKey(p => p.Id);
                pb.WithOwner().HasForeignKey(p => p.ProductionParameterId);

                pb.HasIndex(p => new { p.ProductionParameterId, p.PropertyId }).IsUnique();

                pb.OwnsOne(p => p.Value);

                pb.Property(p => p.PropertyId)
                    .HasMaxLength(50)
                    .IsRequired();

                pb.Property(p => p.Description)
                    .HasMaxLength(500)
                    .IsRequired();

                pb.Property(p => p.ValueUnitOfMeasure)
                    .HasMaxLength(30)
                    .IsRequired();
            });
        }
    }
}
