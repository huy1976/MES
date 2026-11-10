using MesMicroservice.Domain.AggregateModels.MaterialClassAggregate;

namespace MesMicroservice.Application.Commands.MaterialClasses;

public class DeleteMaterialClassCommandHandler : IRequestHandler<DeleteMaterialClassCommand, bool>
{
    private readonly IMaterialClassRepository _materialClassRepository;

    public DeleteMaterialClassCommandHandler(IMaterialClassRepository materialClassRepository)
    {
        _materialClassRepository = materialClassRepository;
    }

    public async Task<bool> Handle(DeleteMaterialClassCommand request, CancellationToken cancellationToken)
    {
        await _materialClassRepository.Delete(request.MaterialClassId);

        return await _materialClassRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
