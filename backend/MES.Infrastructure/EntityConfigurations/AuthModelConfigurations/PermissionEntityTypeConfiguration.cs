namespace MesMicroservice.Infrastructure.EntityConfigurations.AuthModelConfigurations
{
    public class PermissionEntityTypeConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable(nameof(Permission));

            builder.HasKey(x => x.PermissionId);

            builder.Property(x => x.PageId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(250);

        }
    }

}
