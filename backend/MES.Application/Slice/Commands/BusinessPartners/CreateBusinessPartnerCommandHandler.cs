


namespace MesMicroservice.Application.Commands.BusinessPartners;

public class CreateBusinessPartnerCommandHandler : IRequestHandler<CreateBusinessPartnerCommand, bool>
{
    private readonly IBusinessPartnerRepository _businessPartnerRepository;
    public CreateBusinessPartnerCommandHandler(IBusinessPartnerRepository businessPartnerRepository)
    {
        _businessPartnerRepository = businessPartnerRepository;
    }

    public async Task<bool> Handle(CreateBusinessPartnerCommand request, CancellationToken cancellationToken)
    {
        var properties = request.Properties.ConvertAll(x => new BusinessPartnerProperty(
            x.PropertyId,
            x.Description,
            new Value(x.ValueString, x.ValueType),
            x.ValueUnitOfMeasure,
            x.Name)).ToList();

        var businessPartner = new BusinessPartner(
            businessPartnerId: request.BusinessPartnerId,
            name: request.Name,
            description: request.Description ?? "--",
            businessPartnerType: request.BusinessPartnerType,
            properties: properties
            );

        await _businessPartnerRepository.Add(businessPartner);

        return await _businessPartnerRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

    }
}

