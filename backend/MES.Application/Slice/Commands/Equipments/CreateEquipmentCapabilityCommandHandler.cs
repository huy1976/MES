global using MesMicroservice.Domain.AggregateModels.EquipmentCapabilityAggregate;

namespace MesMicroservice.Application.Commands.Equipments
{
    public class CreateEquipmentCapabilityCommandHandler : IRequestHandler<CreateEquipmentCapabilityCommand, bool>
    {
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IEquipmentCapabilityRepository _equipmentCapabilityRepository;

        public CreateEquipmentCapabilityCommandHandler(IEquipmentRepository equipmentRepository, IEquipmentCapabilityRepository equipmentCapabilityRepository)
        {
            _equipmentRepository = equipmentRepository;
            _equipmentCapabilityRepository = equipmentCapabilityRepository;
        }

        public async Task<bool> Handle(CreateEquipmentCapabilityCommand request, CancellationToken cancellationToken)
        {
            var equipment = await _equipmentRepository.GetAsync(request.EquipmentId);
            if (equipment is null)
            {
                throw new ResourceNotFoundException($"Equipment with ID {request.EquipmentId} not found.");
            }

            var properties = request.Properties.Select(x => new EquipmentCapabilityProperty(
                x.PropertyId,
                x.Description,
                new Value(x.ValueString, x.ValueType),
                x.ValueUnitOfMeasure,
                x.Name))
                .ToList();

            var capability = new EquipmentCapability(equipmentCapabilityId: request.EquipmentCapabilityId,
                                                     description: request.Description ?? "--",
                                                     hierarchyScope: request.HierarchyScope ?? "--",
                                                     capabilityType: request.CapabilityType,
                                                     validFrom: request.ValidFrom ?? DateTime.UtcNow,
                                                     validTo: request.ValidTo ?? DateTime.MinValue,
                                                     quantity: request.Quantity,
                                                     valueUnitOfMeasure: request.ValueUnitOfMeasure,
                                                     properties: properties,
                                                     equipment: equipment);

            await _equipmentCapabilityRepository.Add(capability);

            return await _equipmentCapabilityRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
