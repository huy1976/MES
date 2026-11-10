global using MesMicroservice.Domain.AggregateModels.OperationAggregate;

namespace MesMicroservice.Infrastructure.EntityConfigurations.OperationModelConfigurations
{
    public class OperationsDefinitionEntityTypeConfiguration : IEntityTypeConfiguration<OperationsDefinition>
    {
        public void Configure(EntityTypeBuilder<OperationsDefinition> builder)
        {
            builder.ToTable(nameof(OperationsDefinition));

            builder.Property(x => x.Description).HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.Version).HasMaxLength(50).IsRequired();
            
            builder.HasOne(x => x.MaterialDefinition).WithMany(x => x.OperationsDefinitions).IsRequired().OnDelete(DeleteBehavior.NoAction);


        }
    }
}
