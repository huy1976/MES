namespace MesMicroservice.Application.Commands.OperationsDefinitions
{
    public class CreateOperationSegmentDependencyCommandHandler : IRequestHandler<CreateOperationSegmentDependencyCommand, bool>
    {
        private readonly IProcessSegmentDependencyRepository _processSegmentDependencyRepository;
        private readonly IOperationsDefinitionRepository _operationsDefinitionRepository;

        public CreateOperationSegmentDependencyCommandHandler(IProcessSegmentDependencyRepository processSegmentDependencyRepository, IOperationsDefinitionRepository operationsDefinitionRepository)
        {
            _processSegmentDependencyRepository = processSegmentDependencyRepository;
            _operationsDefinitionRepository = operationsDefinitionRepository;
        }

        public async Task<bool> Handle(CreateOperationSegmentDependencyCommand request, CancellationToken cancellationToken)
        {
            var operationsDefinition = await _operationsDefinitionRepository.GetAsync(request.OperationsDefinitionId);
            if (operationsDefinition == null)
            {
                throw new ResourceNotFoundException($"Operations Definition with ID {request.OperationsDefinitionId} not found.");
            }

            // Validate the complete graph before changing tracked entities or saving it.
            var dependencies = operationsDefinition.OperationsSegmentDependencies
                .Where(d => d.IsActive)
                .Select(d => new KeyValuePair<string, string>(d.FromSegmentId, d.ToSegmentId))
                .Append(new KeyValuePair<string, string>(request.FromSegmentId, request.ToSegmentId));

            MesMicroservice.Application.Services.SegmentSequencePlanner.Calculate(
                operationsDefinition.OperationsSegments.Select(s => s.SegmentId),
                dependencies);

            var dependency = new OperationsSegmentDependency(fromSegmentId: request.FromSegmentId,
                                                             toSegmentId: request.ToSegmentId,
                                                             description: request.Description ?? "--",
                                                             dependencyType: request.DependencyType??EDependencyType.FinishToStart,
                                                             dependencyFactor: request.DependencyFactor ?? null,
                                                             valueUnitOfMeasure: request.ValueUnitOfMeasure ?? "None",
                                                             isActive: true,
                                                             operationsDefinition: operationsDefinition);

            operationsDefinition.OperationsSegmentDependencies.Add(dependency);

            var processSegmentDependency = new ProcessSegmentDependency(fromSegmentId: request.FromSegmentId,
                                                                         toSegmentId: request.ToSegmentId,
                                                                         description: request.Description ?? "--",
                                                                         dependencyType: request.DependencyType ?? EDependencyType.FinishToStart,
                                                                         dependencyFactor: request.DependencyFactor ?? null,
                                                                         valueUnitOfMeasure: request.ValueUnitOfMeasure ?? "None",
                                                                         isActive: true);

            await _processSegmentDependencyRepository.Add(processSegmentDependency);

            return await _operationsDefinitionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }


    }
}
