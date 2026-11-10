namespace MesMicroservice.Infrastructure.EntityConfigurations.OrderModelConfigurations
{
    public class MaterialWorkOrderEntityTypeConfiguration : IEntityTypeConfiguration<MaterialWorkOrder>
    {
        public void Configure(EntityTypeBuilder<MaterialWorkOrder> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsOne(s => s.Quantity);

            builder.HasOne(x => x.MaterialDefinition)
                .WithMany(x => x.MaterialWorkOrders)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.WorkOrder)
                .WithMany(x => x.MaterialWorkOrders)
                .HasForeignKey(x => x.WorkOrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
