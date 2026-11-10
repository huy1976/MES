using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

namespace MesMicroservice.Application.Commands.Enterprises;

public class CreateEnterpriseCommandHandler : IRequestHandler<CreateEnterpriseCommand, bool>
{
    private readonly IEnterpriseRepository _enterpriseRepository;

    public CreateEnterpriseCommandHandler(IEnterpriseRepository enterpriseRepository)
    {
        _enterpriseRepository = enterpriseRepository;
    }

    public async Task<bool> Handle(CreateEnterpriseCommand request, CancellationToken cancellationToken)
    {
        var enterprise = new Enterprise(request.EnterpriseId, request.Name);
        await _enterpriseRepository.Add(enterprise);

        return await _enterpriseRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
