namespace MesMicroservice.Infrastructure.EntityConfigurations.OperationModelConfigurations
{
    public class PersonnelSpecificationEntityTypeConfiguration : IEntityTypeConfiguration<PersonnelSpecification>
    {
        public void Configure(EntityTypeBuilder<PersonnelSpecification> builder)
        {
            builder.ToTable(nameof(PersonnelSpecification));

            builder.Property(x => x.PersonnelClassId).IsRequired().HasMaxLength(200);
            builder.Property(x => x.PersonnelSegmentSpecificationId).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(1000);
            builder.OwnsOne(x => x.Quantity);

            builder.OwnsMany(x => x.Properties, pb =>
            {
                pb.HasKey(p => p.Id);
                pb.WithOwner().HasForeignKey(p => p.PersonnelSpecificationId);

                pb.HasIndex(p => new { p.PersonnelSpecificationId, p.PropertyId }).IsUnique();

                pb.OwnsOne(p => p.Value);

                pb.Property(p => p.PropertyId).HasMaxLength(50).IsRequired();
                pb.Property(p => p.Description).HasMaxLength(500).IsRequired();
                pb.Property(p => p.ValueUnitOfMeasure).HasMaxLength(30).IsRequired();
            });

            builder.HasOne(x => x.OperationsSegment)
                .WithMany(x => x.PersonnelSpecifications)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
