global using MesMicroservice.Domain.AggregateModels.BomAggregate;

namespace MesMicroservice.Application.Commands.BillOfMaterials
{
    public class CreateBillOfMaterialCommandHandler : IRequestHandler<CreateBillOfMaterialCommand, bool>
    {
        private readonly IBillOfMaterialRepository _billOfMaterialRepository;
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

        public CreateBillOfMaterialCommandHandler(IBillOfMaterialRepository billOfMaterialRepository, IMaterialDefinitionRepository materialDefinitionRepository)
        {
            _billOfMaterialRepository = billOfMaterialRepository;
            _materialDefinitionRepository = materialDefinitionRepository;
        }

        public async Task<bool> Handle(CreateBillOfMaterialCommand request, CancellationToken cancellationToken)
        {
            var materialDefinition = await _materialDefinitionRepository.GetAsync(request.MaterialDefinitionId)
                ?? throw new ResourceNotFoundException(nameof(MaterialDefinition), request.MaterialDefinitionId);

            var Quantity = new Value(request.ValueString, request.ValueType);

            var billOfMaterial = new BillOfMaterial(
                billId: request.BillOfMaterialId,
                description: request.Description ?? "--",
                materialUse: request.MaterialUse ?? EMaterialUse.Consumable,
                assemblyType: request.AssemblyType ?? EAssemblyType.None,
                assemblyRelationship: request.AssemblyRelationship ?? EAssemblyRelationship.None,
                quantity: Quantity,
                valueUnitOfMeasure: request.ValueUnitOfMeasure,
                materialDefinition: materialDefinition);

            await _billOfMaterialRepository.Add(billOfMaterial);

            return await _billOfMaterialRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
