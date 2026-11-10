namespace MesMicroservice.Infrastructure.EntityConfigurations.EquipmentModelConfigurations
{
    public class EquipmentCapabilityEntityTypeConfiguration : IEntityTypeConfiguration<EquipmentCapability>
    {
        public void Configure(EntityTypeBuilder<EquipmentCapability> builder)
        {
            builder.ToTable(nameof(EquipmentCapability));

            builder.HasOne(x => x.Equipment)
                .WithMany(x => x.EquipmentCapabilities)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.OwnsOne(x => x.Quantity);

            builder.OwnsMany(x => x.Properties, pb =>
            {
                pb.HasKey(p => p.Id);
                pb.WithOwner().HasForeignKey(p => p.EquipmentCapabilityId);

                pb.HasIndex(p => new { p.EquipmentCapabilityId, p.PropertyId }).IsUnique();

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
