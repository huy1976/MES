namespace MesMicroservice.Infrastructure.EntityConfigurations.MaterialModelConfigurations
{
    public class MaterialSubLotEntityTypeConfiguration : IEntityTypeConfiguration<MaterialSubLot>
    {
        public void Configure(EntityTypeBuilder<MaterialSubLot> builder)
        {
            builder.ToTable(nameof(MaterialSubLot));

            builder.HasOne(x => x.Unit).WithMany(x => x.MaterialSubLots);
            builder.HasOne(x => x.MaterialDefinition).WithMany(x => x.MaterialSubLots).IsRequired().OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.ParentMaterialLot).WithMany(x => x.MaterialSubLots).IsRequired().OnDelete(DeleteBehavior.NoAction);
            builder.Property(x => x.Quantity).HasPrecision(30, 8).IsRequired();
        }
    }
}
    