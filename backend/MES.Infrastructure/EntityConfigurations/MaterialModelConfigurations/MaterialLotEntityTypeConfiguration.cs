namespace MesMicroservice.Infrastructure.EntityConfigurations.MaterialModelConfigurations;
public class MaterialLotEntityTypeConfiguration : IEntityTypeConfiguration<MaterialLot>
{
    public void Configure(EntityTypeBuilder<MaterialLot> builder)
    {
        builder.ToTable(nameof(MaterialLot));

        builder.HasOne(x => x.Unit).WithMany(x => x.MaterialLots);
        builder.HasOne(x => x.MaterialDefinition).WithMany(x => x.MaterialLots).IsRequired().OnDelete(DeleteBehavior.NoAction);

        builder.Property(x => x.Quantity).HasPrecision(30, 8).IsRequired();

        builder.OwnsMany(x => x.Properties, pb =>
        {
            pb.HasKey(p => p.Id);
            pb.WithOwner().HasForeignKey(p => p.MaterialLotId);

            pb.HasIndex(p => new { p.MaterialLotId, p.PropertyId }).IsUnique();

            pb.OwnsOne(p => p.Value);

            pb.Property(p => p.PropertyId).HasMaxLength(50).IsRequired();
            pb.Property(p => p.Description).HasMaxLength(500).IsRequired();
            pb.Property(p => p.ValueUnitOfMeasure).HasMaxLength(30).IsRequired();
        });
    }
}
