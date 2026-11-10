using MesMicroservice.Domain.AggregateModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MesMicroservice.Infrastructure.EntityConfigurations.ResourceModelConfigurations
{
    public class ResourceEntityTypeConfiguration : IEntityTypeConfiguration<Resource>
    {
        public void Configure(EntityTypeBuilder<Resource> builder)
        {
            builder.HasKey(x => x.Id);


            builder.Property(x => x.Id)
                   .UseIdentityColumn();

            builder.HasIndex(x => x.ResourceId).IsUnique();
            builder.Property(x => x.ResourceId)
                   .HasMaxLength(50)
                   .IsRequired();
        }
    }
}
