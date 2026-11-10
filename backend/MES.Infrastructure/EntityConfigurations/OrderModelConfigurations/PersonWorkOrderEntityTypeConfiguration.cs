namespace MesMicroservice.Infrastructure.EntityConfigurations.OrderModelConfigurations
{
    public class PersonWorkOrderEntityTypeConfiguration : IEntityTypeConfiguration<PersonWorkOrder>
    {
        public void Configure(EntityTypeBuilder<PersonWorkOrder> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Person)
                .WithMany(x => x.PersonWorkOrders)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.WorkOrder)
                .WithMany(x => x.PersonWorkOrders)
                .HasForeignKey(x => x.WorkOrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
