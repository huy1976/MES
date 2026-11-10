namespace MesMicroservice.Domain.AggregateModels.BusinessPartnerAggregate
{
    public class BusinessPartner : Resource, IAggregateRoot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public EBusinessPartnerType BusinessPartnerType { get; private set; }
        public ICollection<BusinessPartnerProperty> Properties { get; private set; } = new List<BusinessPartnerProperty>(); 
        public ICollection<ManufacturingOrder> ManufacturingOrders { get; set; } = new List<ManufacturingOrder>(); 

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private BusinessPartner() : base() { }
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public BusinessPartner(string businessPartnerId, string name, string description, EBusinessPartnerType businessPartnerType, ICollection<BusinessPartnerProperty> properties) : base(businessPartnerId)
        {
            Name = name;
            Description = description;
            BusinessPartnerType = businessPartnerType;
            Properties = properties;
        }

    }
}
