using MesMicroservice.Domain.AggregateModels.UserAggregate;

namespace MesMicroservice.Infrastructure.EntityConfigurations.AuthModelConfigurations
{
    public class TokenEntityTypeConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Role)
                .IsRequired();

            builder.Property(x => x.Expires)
                .IsRequired();

            builder.Property(x => x.IsRevoked)
                .IsRequired();

        }
    }
}
