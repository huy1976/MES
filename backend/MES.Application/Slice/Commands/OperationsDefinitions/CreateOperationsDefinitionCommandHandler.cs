global using MesMicroservice.Domain.AggregateModels.OperationAggregate;

namespace MesMicroservice.Application.Commands.OperationsDefinitions
{
    public class CreateOperationsDefinitionCommandHandler : IRequestHandler<CreateOperationsDefinitionCommand, bool>
    {
        private readonly IOperationsDefinitionRepository _operationsDefinitionRepository;
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;
        private readonly IBillOfMaterialRepository _billOfMaterialRepository;
        private readonly IProcessSegmentRepository _processSegmentRepository;

        public CreateOperationsDefinitionCommandHandler(IOperationsDefinitionRepository operationsDefinitionRepository, IMaterialDefinitionRepository materialDefinitionRepository, IBillOfMaterialRepository billOfMaterialRepository, IProcessSegmentRepository processSegmentRepository)
        {
            _operationsDefinitionRepository = operationsDefinitionRepository;
            _materialDefinitionRepository = materialDefinitionRepository;
            _billOfMaterialRepository = billOfMaterialRepository;
            _processSegmentRepository = processSegmentRepository;
        }

        public async Task<bool> Handle(CreateOperationsDefinitionCommand request, CancellationToken cancellationToken)
        {
            var dependencies = request.Dependencies ?? new List<OperationDependencyInput>();
            MesMicroservice.Application.Services.SegmentSequencePlanner.Calculate(
                request.SegmentIds,
                dependencies.Select(d => new KeyValuePair<string, string>(d.FromSegmentId, d.ToSegmentId)));

            var materialDefinition = await _materialDefinitionRepository.GetAsync(request.MaterialId);
            if (materialDefinition is null)
            {
                throw new ResourceNotFoundException(nameof(MaterialDefinition), request.MaterialId);
            }

            var operationsDefinition = new OperationsDefinition(operationId: request.OperationsDefinitionId,
                                                                version: request.Version,
                                                                description: request.Description,
                                                                operationsType: request.OperationsType ?? EOperationsType.Production,
                                                                hierarchyScope: request.HierarchyScope ?? "--",
                                                                workDefinitionId: request.WorkDefinitionId ?? "--",
                                                                validFrom: request.ValidFrom,
                                                                validTo: request.ValidTo ?? DateTime.MinValue,
                                                                materialDefinition: materialDefinition,
                                                                operationsSegments: new List<OperationsSegment>(),
                                                                operationsMaterialBill:null!);

            
            operationsDefinition.OperationsMaterialBill = await CreateOperationsMaterialBill(materialDefinition, operationsDefinition, request);

            foreach (var operationsSegment in await CreateOperationsSegments(materialDefinition, operationsDefinition, request))
            {
                operationsDefinition.OperationsSegments.Add(operationsSegment);
            }

            foreach (var input in dependencies)
            {
                operationsDefinition.OperationsSegmentDependencies.Add(new OperationsSegmentDependency(
                    fromSegmentId: input.FromSegmentId.Trim(),
                    toSegmentId: input.ToSegmentId.Trim(),
                    description: "Configured production route",
                    dependencyType: EDependencyType.FinishToStart,
                    dependencyFactor: null,
                    valueUnitOfMeasure: "None",
                    isActive: true,
                    operationsDefinition: operationsDefinition));
            }

            await _operationsDefinitionRepository.Add(operationsDefinition);

            return await _operationsDefinitionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }

        private async Task<OperationsMaterialBill> CreateOperationsMaterialBill(MaterialDefinition materialDefinition, OperationsDefinition operationsDefinition, 
                                                                                CreateOperationsDefinitionCommand request)
        {
            var operationsMaterialBill = new OperationsMaterialBill(description: $"Tong hop nguyen vat lieu de san xuat san pham {materialDefinition.Name}",
                                                                    operationsDefinition: operationsDefinition,
                                                                    items: new List<OperationsMaterialBillItem>());

            var boms = new List<BillOfMaterial>();
            foreach (var billId in request.BomIds)
            {
                var bom = await _billOfMaterialRepository.GetAsync(billId);
                if (bom is null)
                {
                    throw new ResourceNotFoundException(nameof(BillOfMaterial), billId);
                }

                boms.Add(bom);
            }

            var operationsMaterialBillItems = new List<OperationsMaterialBillItem>();
            foreach (var bom in boms)
            {
                var item = new OperationsMaterialBillItem(billId: bom.ResourceId,
                                                          materialDefinitionId: bom.MaterialDefinition.ResourceId,
                                                          description: bom.Description,
                                                          materialUse: bom.MaterialUse,
                                                          assemblyType: bom.AssemblyType,
                                                          assemblyRelationship: bom.AssemblyRelationship,
                                                          quantity: bom.Quantity,
                                                          valueUnitOfMeasure: bom.ValueUnitOfMeasure,
                                                          operationsMaterialBill: operationsMaterialBill);

                operationsMaterialBillItems.Add(item);
            }
            operationsMaterialBill.Items = operationsMaterialBillItems;

            return operationsMaterialBill;
        }

        private async Task<List<OperationsSegment>> CreateOperationsSegments (MaterialDefinition materialDefinition, OperationsDefinition operationsDefinition,
                                                            CreateOperationsDefinitionCommand request)
        {
            var segments = new List<ProcessSegment>();
            foreach (var segmentId in request.SegmentIds)
            {
                var segment = await _processSegmentRepository.GetAsync(segmentId);
                if (segment is null)
                {
                    throw new ResourceNotFoundException(nameof(ProcessSegment), segmentId);
                }

                segments.Add(segment);
            }

            var operationsSegments = new List<OperationsSegment>();
            foreach (var segment in segments)
            {
                var operationsSegment = new OperationsSegment(segmentId: segment.ResourceId,
                                                              productionParameterId: segment.ProductionParameter.ResourceId,
                                                              description: segment.Description,
                                                              operationsType: segment.OperationsType,
                                                              segmentType: segment.SegmentType,
                                                              hierarchyScope: segment.HierarchyScope,
                                                              duration: segment.Duration,
                                                              valueUnitOfMeasure: segment.ValueUnitOfMeasure,
                                                              workDefinitionId: operationsDefinition.WorkDefinitionId,
                                                              operationsDefinition: operationsDefinition,
                                                              equipmentSpecifications: new List<EquipmentSpecification>(),
                                                              personnelSpecifications: new List<PersonnelSpecification>(),
                                                              materialSpecifications: new List<MaterialSpecification>(),
                                                              materialId: segment.OutputMaterialDefinitionId);

                CreateEquipmentSpecifications(segment, operationsSegment);

                CreatePersonSpecifications(segment, operationsSegment);

                CreateMaterialSpecifications(segment, operationsSegment);


                operationsSegments.Add(operationsSegment);
            }

            return operationsSegments;
        }

        private void CreateEquipmentSpecifications(ProcessSegment segment, OperationsSegment operationsSegment)
        {
            var equipmentSpecifications = segment.EquipmentSegmentSpecifications
                .Select(es => new EquipmentSpecification(equipmentSegmentSpecificationId: es.ResourceId,
                                                         equipmentClassId: es.EquipmentClass.ResourceId,
                                                         description: es.Description,
                                                         equipmentUse: es.EquipmentUse,
                                                         quantity: es.Quantity,
                                                         valueUnitOfMeasure: es.ValueUnitOfMeasure,
                                                         properties: es.Properties
                                                            .Select(p => new EquipmentSpecificationProperty(propertyId: p.PropertyId,
                                                                                                            description: p.Description,
                                                                                                            value: p.Value,
                                                                                                            valueUnitOfMeasure: p.ValueUnitOfMeasure,
                                                                                                            name: p.Name))
                                                            .ToList(),
                                                         operationsSegment: operationsSegment))
                .ToList();

            foreach (var equipmentSpecification in equipmentSpecifications)
            {
                operationsSegment.EquipmentSpecifications.Add(equipmentSpecification);
            }
        }

        private void CreatePersonSpecifications(ProcessSegment segment, OperationsSegment operationsSegment)
        {
            var personSpecifications = segment.PersonnelSegmentSpecifications
                .Select(es => new PersonnelSpecification(personnelSegmentSpecificationId: es.ResourceId,
                                                         personnelClassId: es.PersonnelClass.ResourceId,
                                                         description: es.Description,
                                                         personnelUse: es.PersonnelUse,
                                                         quantity: es.Quantity,
                                                         valueUnitOfMeasure: es.ValueUnitOfMeasure,
                                                         properties: es.Properties
                                                            .Select(p => new PersonnelSpecificationProperty(propertyId: p.PropertyId,
                                                                                                            description: p.Description,
                                                                                                            value: p.Value,
                                                                                                            valueUnitOfMeasure: p.ValueUnitOfMeasure,
                                                                                                            name: p.Name))
                                                            .ToList(),
                                                         operationsSegment: operationsSegment))
                .ToList();

            foreach (var personnelSpecification in personSpecifications)
            {
                operationsSegment.PersonnelSpecifications.Add(personnelSpecification);
            }
        }

        private void CreateMaterialSpecifications(ProcessSegment segment, OperationsSegment operationsSegment)
        {
            var materialSpecifications = segment.MaterialSegmentSpecifications
                .Select(es => new MaterialSpecification(materialSegmentSpecificationId: es.ResourceId,
                                                         materialDefinitionId: es.MaterialDefinition.ResourceId,
                                                         description: es.Description,
                                                         assemblyType: es.AssemblyType,
                                                         assemblyRelationship: es.AssemblyRelationship,
                                                         materialUse: es.MaterialUse,
                                                         quantity: es.Quantity,
                                                         valueUnitOfMeasure: es.ValueUnitOfMeasure,
                                                         properties: es.Properties
                                                            .Select(p => new MaterialSpecificationProperty(propertyId: p.PropertyId,
                                                                                                            description: p.Description,
                                                                                                            value: p.Value,
                                                                                                            valueUnitOfMeasure: p.ValueUnitOfMeasure,
                                                                                                            name: p.Name))
                                                            .ToList(),
                                                         operationsSegment: operationsSegment))
                .ToList();

            foreach (var materialSpecification in materialSpecifications)
            {
                operationsSegment.MaterialSpecifications.Add(materialSpecification);
            }
        }



    }
}
