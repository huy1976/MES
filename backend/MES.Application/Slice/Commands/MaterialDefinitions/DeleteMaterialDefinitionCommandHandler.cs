namespace MesMicroservice.Application.Commands.MaterialDefinitions;

public class DeleteMaterialDefinitionCommandHandler : IRequestHandler<DeleteMaterialDefinitionCommand, bool>
{
    private readonly IMaterialDefinitionRepository _materialDefinitionRepository;
    private readonly IResourceRelationshipNetworkRepository _relationshipRepository;

    public DeleteMaterialDefinitionCommandHandler(IMaterialDefinitionRepository materialDefinitionRepository, IResourceRelationshipNetworkRepository relationshipRepository)
    {
        _materialDefinitionRepository = materialDefinitionRepository;
        _relationshipRepository = relationshipRepository;
    }

    public async Task<bool> Handle(DeleteMaterialDefinitionCommand request, CancellationToken cancellationToken)
    {
        var productMaterialRelationship = await _relationshipRepository.GetAsync("ProductMaterialRelationshipId")
            ?? throw new ResourceNotFoundException(nameof(ResourceRelationshipNetwork), "ProductMaterialRelationshipId");

        var plasticProductMoldRelationship = await _relationshipRepository.GetAsync("PlasticProductMoldRelationshipId")
            ?? throw new ResourceNotFoundException(nameof(ResourceRelationshipNetwork), "PlasticProductMoldRelationshipId");

        productMaterialRelationship.RemoveConnectionsByResourceId(request.MaterialDefinitionId);
        plasticProductMoldRelationship.RemoveConnectionsByResourceId(request.MaterialDefinitionId);

        await _relationshipRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        await _materialDefinitionRepository.DeleteAsync(request.MaterialDefinitionId);
        return await _materialDefinitionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
