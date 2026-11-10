global using MesMicroservice.Domain.AggregateModels.SegmentAggregate;

namespace MesMicroservice.Application.Commands.SegmentModels
{
    public class CreateProcessSegmentCommandHandler : IRequestHandler<CreateProcessSegmentCommand, bool>
    {
        private readonly IProcessSegmentRepository _processSegmentRepository;
        private readonly IProductionParameterRepository _productionParameterRepository;
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

        public CreateProcessSegmentCommandHandler(IProcessSegmentRepository processSegmentRepository, IProductionParameterRepository productionParameterRepository, IMaterialDefinitionRepository materialDefinitionRepository)
        {
            _processSegmentRepository = processSegmentRepository;
            _productionParameterRepository = productionParameterRepository;
            _materialDefinitionRepository = materialDefinitionRepository;
        }

        public async Task<bool> Handle(CreateProcessSegmentCommand request, CancellationToken cancellationToken)
        {
            MesMicroservice.Algorithm.Services.SegmentDuration.GetHours(
                request.SegmentType, request.DurationValueString, request.ValueUnitOfMeasure);

            var materialDefinition = await _materialDefinitionRepository.GetAsync(request.OutputMaterialDefinitionId);
            if (materialDefinition is null)
            {
                throw new ResourceNotFoundException(nameof(MaterialDefinition), request.OutputMaterialDefinitionId);
            }

            var productionParameter = await _productionParameterRepository.GetAsync(request.ProductionParameterId);
            if (productionParameter is null)
            {
                throw new ResourceNotFoundException(nameof(ProductionParameter), request.ProductionParameterId);
            }

            var processSegment = new ProcessSegment(processSegmentId: request.ProcessSegmentId,
                                                    name: request.Name,
                                                    description: request.Description ?? "--",
                                                    segmentType: request.SegmentType,
                                                    operationsType: request.OperationsType,
                                                    hierarchyScope: request.HierarchyScope ?? "",
                                                    duration: new Value(request.DurationValueString ?? "--", request.ValueType ?? EValueType.String),
                                                    valueUnitOfMeasure: request.ValueUnitOfMeasure ?? "None",
                                                    productionParameter: productionParameter,
                                                    materialId: request.OutputMaterialDefinitionId);

            await _processSegmentRepository.Add(processSegment);

            return await _processSegmentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
