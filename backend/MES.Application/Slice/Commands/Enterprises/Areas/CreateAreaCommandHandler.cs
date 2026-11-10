using MesMicroservice.Application.Exceptions;
using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

namespace MesMicroservice.Application.Commands.Enterprises.Areas;

public class CreateAreaCommandHandler : IRequestHandler<CreateAreaCommand, bool>
{
    private readonly IEnterpriseRepository _enterpriseRepository;

    public CreateAreaCommandHandler(IEnterpriseRepository enterpriseRepository)
    {
        _enterpriseRepository = enterpriseRepository;
    }

    public async Task<bool> Handle(CreateAreaCommand request, CancellationToken cancellationToken)
    {
        var enterprise = await _enterpriseRepository.GetAsync(request.EnterpriseId) ?? throw new ResourceNotFoundException(nameof(Enterprise), request.EnterpriseId);
        var site = enterprise.Sites.Find(x => x.AbsolutePath == $"{request.EnterpriseId}/{request.SiteId}") ?? throw new ResourceNotFoundException(nameof(Site), request.SiteId);
        var area = new Area(request.AreaId, request.Name);

        site.AddArea(area);
        return await _enterpriseRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
