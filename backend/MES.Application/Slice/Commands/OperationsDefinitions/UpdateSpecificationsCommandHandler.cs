namespace MesMicroservice.Application.Commands.OperationsDefinitions
{
    public class UpdateSpecificationsCommandHandler : IRequestHandler<UpdateSpecificationsCommand, bool>
    {
        private readonly IOperationsDefinitionRepository _operationsDefinitionRepository;
        private readonly IProcessSegmentRepository _processSegmentRepository;

        public UpdateSpecificationsCommandHandler(
            IOperationsDefinitionRepository operationsDefinitionRepository,
            IProcessSegmentRepository processSegmentRepository)
        {
            _operationsDefinitionRepository = operationsDefinitionRepository;
            _processSegmentRepository = processSegmentRepository;
        }

        public async Task<bool> Handle(UpdateSpecificationsCommand request, CancellationToken cancellationToken)
        {
            var operationsDefinitions = await _operationsDefinitionRepository.GetAllAsync();

            // Cache các ProcessSegment cần thiết để tránh nhiều truy vấn DB
            var allSegmentIds = operationsDefinitions
                .SelectMany(od => od.OperationsSegments.Select(os => os.SegmentId))
                .Distinct()
                .ToList();

            var processSegments = await _processSegmentRepository.GetListByIdAsync(allSegmentIds);
            var processSegmentDict = processSegments.ToDictionary(ps => ps.ResourceId);

            foreach (var operationsDefinition in operationsDefinitions)
            {
                foreach (var operationsSegment in operationsDefinition.OperationsSegments)
                {
                    if (!processSegmentDict.TryGetValue(operationsSegment.SegmentId, out var processSegment))
                        throw new ResourceNotFoundException(nameof(ProcessSegment), operationsSegment.SegmentId);

                    SyncEquipmentSpecifications(operationsSegment, processSegment);
                    SyncMaterialSpecifications(operationsSegment, processSegment);
                    SyncPersonnelSpecifications(operationsSegment, processSegment);
                }
            }

            return await _operationsDefinitionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }

        private static void SyncEquipmentSpecifications(OperationsSegment operationsSegment, ProcessSegment processSegment)
        {
            foreach (var equipmentSegSpec in processSegment.EquipmentSegmentSpecifications)
            {
                bool exists = operationsSegment.EquipmentSpecifications
                    .Any(es => es.EquipmentSegmentSpecificationId == equipmentSegSpec.ResourceId);

                if (!exists)
                {
                    var newSpec = new EquipmentSpecification(
                        equipmentSegmentSpecificationId: equipmentSegSpec.ResourceId,
                        equipmentClassId: equipmentSegSpec.EquipmentClass.ResourceId,
                        description: equipmentSegSpec.Description,
                        equipmentUse: equipmentSegSpec.EquipmentUse,
                        quantity: equipmentSegSpec.Quantity,
                        valueUnitOfMeasure: equipmentSegSpec.ValueUnitOfMeasure,
                        properties: equipmentSegSpec.Properties
                            .Select(p => new EquipmentSpecificationProperty(
                                propertyId: p.PropertyId,
                                description: p.Description,
                                value: p.Value,
                                valueUnitOfMeasure: p.ValueUnitOfMeasure,
                                name: p.Name))
                            .ToList(),
                        operationsSegment: operationsSegment
                    );

                    operationsSegment.EquipmentSpecifications.Add(newSpec);
                }
            }
        }

        private static void SyncMaterialSpecifications(OperationsSegment operationsSegment, ProcessSegment processSegment)
        {
            foreach (var materialSegSpec in processSegment.MaterialSegmentSpecifications)
            {
                bool exists = operationsSegment.MaterialSpecifications
                    .Any(ms => ms.MaterialSegmentSpecificationId == materialSegSpec.ResourceId);

                if (!exists)
                {
                    var newSpec = new MaterialSpecification(
                        materialSegmentSpecificationId: materialSegSpec.ResourceId,
                        materialDefinitionId: materialSegSpec.MaterialDefinition.ResourceId,
                        description: materialSegSpec.Description,
                        assemblyType: materialSegSpec.AssemblyType,
                        assemblyRelationship: materialSegSpec.AssemblyRelationship,
                        materialUse: materialSegSpec.MaterialUse,
                        quantity: materialSegSpec.Quantity,
                        valueUnitOfMeasure: materialSegSpec.ValueUnitOfMeasure,
                        properties: materialSegSpec.Properties
                            .Select(p => new MaterialSpecificationProperty(
                                propertyId: p.PropertyId,
                                description: p.Description,
                                value: p.Value,
                                valueUnitOfMeasure: p.ValueUnitOfMeasure,
                                name: p.Name))
                            .ToList(),
                        operationsSegment: operationsSegment
                    );

                    operationsSegment.MaterialSpecifications.Add(newSpec);
                }
            }
        }

        private static void SyncPersonnelSpecifications(OperationsSegment operationsSegment, ProcessSegment processSegment)
        {
            foreach (var personnelSegSpec in processSegment.PersonnelSegmentSpecifications)
            {
                bool exists = operationsSegment.PersonnelSpecifications
                    .Any(ps => ps.PersonnelSegmentSpecificationId == personnelSegSpec.ResourceId);

                if (!exists)
                {
                    var newSpec = new PersonnelSpecification(
                        personnelSegmentSpecificationId: personnelSegSpec.ResourceId,
                        personnelClassId: personnelSegSpec.PersonnelClass.ResourceId,
                        description: personnelSegSpec.Description,
                        personnelUse: personnelSegSpec.PersonnelUse,
                        quantity: personnelSegSpec.Quantity,
                        valueUnitOfMeasure: personnelSegSpec.ValueUnitOfMeasure,
                        properties: personnelSegSpec.Properties
                            .Select(p => new PersonnelSpecificationProperty(
                                propertyId: p.PropertyId,
                                description: p.Description,
                                value: p.Value,
                                valueUnitOfMeasure: p.ValueUnitOfMeasure,
                                name: p.Name))
                            .ToList(),
                        operationsSegment: operationsSegment
                    );

                    operationsSegment.PersonnelSpecifications.Add(newSpec);
                }
            }
        }
    }
}
