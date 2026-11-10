namespace MesMicroservice.Infrastructure.EntityConfigurations.PersonModelConfigurations
{
    public class PersonnelCapabilityEntityTypeConfiguration : IEntityTypeConfiguration<PersonnelCapability>
    {
        public void Configure(EntityTypeBuilder<PersonnelCapability> builder) 
        {
            builder.ToTable(nameof(PersonnelCapability));

            builder.HasOne(x => x.Person)
                .WithMany(x => x.PersonnelCapabilities)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.OwnsOne(x => x.Quantity);

            builder.OwnsMany(x => x.Properties, pb =>
            {
                pb.HasKey(p => p.Id);
                pb.WithOwner().HasForeignKey(p => p.CapabilityId);

                pb.HasIndex(p => new { p.CapabilityId, p.PropertyId }).IsUnique();

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
