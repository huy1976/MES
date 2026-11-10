namespace MesMicroservice.Infrastructure.EntityConfigurations.OperationModelConfigurations
{
    public class OperationsMaterialBillEntityTypeConfiguration : IEntityTypeConfiguration<OperationsMaterialBill>
    {
        public void Configure(EntityTypeBuilder<OperationsMaterialBill> builder)
        {
            builder.ToTable(nameof(OperationsMaterialBill));

            builder.HasOne(x => x.OperationsDefinition)
                .WithOne(x => x.OperationsMaterialBill)
                .HasForeignKey<OperationsMaterialBill>(x => x.OperationsDefinitionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
