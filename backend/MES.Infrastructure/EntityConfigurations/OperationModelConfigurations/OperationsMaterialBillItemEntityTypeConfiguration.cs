namespace MesMicroservice.Infrastructure.EntityConfigurations.OperationModelConfigurations
{
    public class OperationsMaterialBillItemEntityTypeConfiguration : IEntityTypeConfiguration<OperationsMaterialBillItem>
    {
        public void Configure(EntityTypeBuilder<OperationsMaterialBillItem> builder)
        {
            builder.ToTable(nameof(OperationsMaterialBillItem));

            builder.OwnsOne(x => x.Quantity);
            builder.Property(x => x.BillId).IsRequired();
            builder.Property(x => x.MaterialDefinitionId).IsRequired();

            builder.HasOne(x => x.OperationsMaterialBill)
                .WithMany(x => x.Items)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
