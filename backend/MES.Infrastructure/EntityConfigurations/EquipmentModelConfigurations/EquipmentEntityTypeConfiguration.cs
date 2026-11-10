namespace MesMicroservice.Infrastructure.EntityConfigurations.EquipmentModelConfigurations;
public class EquipmentEntityTypeConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable(nameof(Equipment));

        builder.OwnsMany(x => x.Properties, pb =>
        {
            pb.HasKey(p => p.Id);
            pb.WithOwner().HasForeignKey(p => p.EquipmentId);

            pb.HasIndex(p => new { p.EquipmentId, p.PropertyId }).IsUnique();

            pb.OwnsOne(p => p.Value);

            pb.Property(p => p.PropertyId).HasMaxLength(50).IsRequired();
            pb.Property(p => p.Description).HasMaxLength(500).IsRequired();
            pb.Property(p => p.ValueUnitOfMeasure).HasMaxLength(30).IsRequired();
        });

        builder.HasOne(x => x.HierarchyModel).WithMany(x => x.AssociatedEquipments);
        builder.HasOne(x => x.EquipmentClass).WithMany(x => x.Equipments).IsRequired().OnDelete(DeleteBehavior.NoAction);

        builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
    }
}
