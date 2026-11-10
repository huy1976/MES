namespace MesMicroservice.Infrastructure.EntityConfigurations.OrderModelConfigurations
{
    public class EquipmentWorkOrderEntityTypeConfiguration : IEntityTypeConfiguration<EquipmentWorkOrder>
    {
        public void Configure(EntityTypeBuilder<EquipmentWorkOrder> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Equipment)
                .WithMany(x => x.EquipmentWorkOrders)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.WorkOrder)
                .WithMany(x => x.EquipmentWorkOrders)
                .HasForeignKey(x => x.WorkOrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
