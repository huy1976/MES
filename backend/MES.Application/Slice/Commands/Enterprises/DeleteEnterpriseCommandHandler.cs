using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

namespace MesMicroservice.Application.Commands.Enterprises;

public class DeleteEnterpriseCommandHandler : IRequestHandler<DeleteEnterpriseCommand, bool>
{
    private readonly IEnterpriseRepository _enterpriseRepository;

    public DeleteEnterpriseCommandHandler(IEnterpriseRepository enterpriseRepository)
    {
        _enterpriseRepository = enterpriseRepository;
    }

    public async Task<bool> Handle(DeleteEnterpriseCommand request, CancellationToken cancellationToken)
    {
        await _enterpriseRepository.DeleteAsync(request.EnterpriseId);

        return await _enterpriseRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
