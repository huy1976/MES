namespace MesMicroservice.Infrastructure.EntityConfigurations.SegmentModelConfigurations
{
    public class PersonnelSegmentSpecificationEntityTypeConfiguration : IEntityTypeConfiguration<PersonnelSegmentSpecification>
    {
        public void Configure(EntityTypeBuilder<PersonnelSegmentSpecification> builder)
        {
            builder.ToTable(nameof(PersonnelSegmentSpecification));

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.OwnsOne(x => x.Quantity);

            builder.OwnsMany(x => x.Properties, pb =>
            {
                pb.HasKey(p => p.Id);
                pb.WithOwner().HasForeignKey(p => p.PersonnelSegmentSpecificationId);

                pb.HasIndex(p => new { p.PersonnelSegmentSpecificationId, p.PropertyId }).IsUnique();

                pb.OwnsOne(p => p.Value);

                pb.Property(p => p.PropertyId).HasMaxLength(50).IsRequired();
                pb.Property(p => p.Description).HasMaxLength(500).IsRequired();
                pb.Property(p => p.ValueUnitOfMeasure).HasMaxLength(30).IsRequired();
            });

            builder.HasOne(x => x.ProcessSegment)
                .WithMany(x => x.PersonnelSegmentSpecifications)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.PersonnelClass)
                .WithMany(x => x.PersonnelSegmentSpecifications)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
