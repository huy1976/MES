namespace MesMicroservice.Infrastructure.EntityConfigurations.EquipmentModelConfigurations;
public class EquipmentClassEntityTypeConfiguration : IEntityTypeConfiguration<EquipmentClass>
{
    public void Configure(EntityTypeBuilder<EquipmentClass> builder)
    {
        builder.ToTable(nameof(EquipmentClass));

        builder.Property(x => x.Name).HasMaxLength(250).IsRequired();

        builder.OwnsMany(x => x.Properties, pb =>
        {
            pb.HasKey(p => p.Id);
            pb.WithOwner().HasForeignKey(p => p.EquipmentClassId);

            pb.HasIndex(p => new { p.EquipmentClassId, p.PropertyId }).IsUnique();

            pb.OwnsOne(p => p.Value);

            pb.Property(p => p.PropertyId).HasMaxLength(50).IsRequired();
            pb.Property(p => p.Description).HasMaxLength(500).IsRequired();
            pb.Property(p => p.ValueUnitOfMeasure).HasMaxLength(30).IsRequired();
        });

    }
}
