namespace MesMicroservice.Application.Commands.Equipments
{
    public class AddEquipmentCapabilityPropertiesCommandHandler : IRequestHandler<AddEquipmentCapabilityPropertiesCommand,bool>
    {
        private readonly IEquipmentCapabilityRepository _equipmentCapabilityRepository;

        public AddEquipmentCapabilityPropertiesCommandHandler(IEquipmentCapabilityRepository equipmentCapabilityRepository)
        {
            _equipmentCapabilityRepository = equipmentCapabilityRepository;
        }

        public async Task<bool> Handle(AddEquipmentCapabilityPropertiesCommand request, CancellationToken cancellationToken)
        {
            var equipmentCa = await _equipmentCapabilityRepository.GetAsync(request.EquipmentCapabilityId);

            var properties = request.Properties.Select(x => new EquipmentCapabilityProperty(
                x.PropertyId,
                x.Description,
                new Value(x.ValueString, x.ValueType),
                x.ValueUnitOfMeasure,
                x.Name))
                .ToList();

            if (equipmentCa is null)
            {
                throw new ResourceNotFoundException(nameof(EquipmentCapabilityProperty), request.EquipmentCapabilityId);
            }

            equipmentCa.AddProperties(properties);

            return await _equipmentCapabilityRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
