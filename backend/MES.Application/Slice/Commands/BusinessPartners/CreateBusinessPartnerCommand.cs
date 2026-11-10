
namespace MesMicroservice.Application.Commands.BusinessPartners;

[DataContract]
public class CreateBusinessPartnerCommand : IRequest<bool>
{
    [DataMember]
    public string BusinessPartnerId { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Description { get; private set; }
    [DataMember]
    public EBusinessPartnerType BusinessPartnerType { get; private set; }

    [DataMember]
    public List<SavePropertyViewModel> Properties { get; set; }

    public CreateBusinessPartnerCommand(string businessPartnerId, string name, string description, EBusinessPartnerType businessPartnerType, List<SavePropertyViewModel> properties)
    {
        BusinessPartnerId = businessPartnerId;
        Name = name;
        Description = description;
        BusinessPartnerType = businessPartnerType;
        Properties = properties;
    }
}
