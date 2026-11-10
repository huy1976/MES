namespace MesMicroservice.Application.Commands.Equipments;

public class DeleteEquipmentCommandHandler : IRequestHandler<DeleteEquipmentCommand, bool>
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IResourceRelationshipNetworkRepository _relationshipRepository;

    public DeleteEquipmentCommandHandler(IEquipmentRepository equipmentRepository, IResourceRelationshipNetworkRepository relationshipRepository)
    {
        _equipmentRepository = equipmentRepository;
        _relationshipRepository = relationshipRepository;
    }

    public async Task<bool> Handle(DeleteEquipmentCommand request, CancellationToken cancellationToken)
    {
        var machineMoldRelationship = await _relationshipRepository.GetAsync("MachineMoldRelationshipId")
            ?? throw new ResourceNotFoundException(nameof(ResourceRelationshipNetwork), "MachineMoldRelationshipId");

        var plasticProductMoldRelationship = await _relationshipRepository.GetAsync("PlasticProductMoldRelationshipId")
            ?? throw new ResourceNotFoundException(nameof(ResourceRelationshipNetwork), "PlasticProductMoldRelationshipId");

        machineMoldRelationship.RemoveConnectionsByResourceId(request.EquipmentId);
        plasticProductMoldRelationship.RemoveConnectionsByResourceId(request.EquipmentId);

        await _relationshipRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        await _equipmentRepository.DeleteAsync(request.EquipmentId);
        return await _equipmentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
