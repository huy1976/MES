namespace MesMicroservice.Application.Commands.Enterprises;

public class UpdateEnterpriseCommandHandler : IRequestHandler<UpdateEnterpriseCommand, bool>
{
    private readonly IEnterpriseRepository _enterpriseRepository;

    public UpdateEnterpriseCommandHandler(IEnterpriseRepository enterpriseRepository)
    {
        _enterpriseRepository = enterpriseRepository;
    }

    public async Task<bool> Handle(UpdateEnterpriseCommand request, CancellationToken cancellationToken)
    {
        var enterprise = await _enterpriseRepository.GetAsync(request.EnterpriseId) ?? throw new ResourceNotFoundException(nameof(Enterprise), request.EnterpriseId);

        enterprise.Update(request.HierarchyModelId, request.Name);
        _enterpriseRepository.Update(enterprise);

        return await _enterpriseRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
