namespace MesMicroservice.Infrastructure.EntityConfigurations.PersonModelConfigurations
{
    public class PersonnelClassEntityTypeConfiguration : IEntityTypeConfiguration<PersonnelClass>
    {
        public void Configure(EntityTypeBuilder<PersonnelClass> builder)
        {
            builder.ToTable(nameof(PersonnelClass));

            builder.OwnsMany(x => x.Properties, pb =>
            {
                pb.HasKey(p => p.Id);
                pb.WithOwner().HasForeignKey(p => p.PersonClassId);

                pb.HasIndex(p => new { p.PersonClassId, p.PropertyId }).IsUnique();

                pb.OwnsOne(p => p.Value);

                pb.Property(p => p.PropertyId).HasMaxLength(50).IsRequired();
                pb.Property(p => p.Description).HasMaxLength(500).IsRequired();
                pb.Property(p => p.ValueUnitOfMeasure).HasMaxLength(30).IsRequired();
            });

            builder.Property(x => x.Name).HasMaxLength(250).IsRequired();

        }

    }
}
