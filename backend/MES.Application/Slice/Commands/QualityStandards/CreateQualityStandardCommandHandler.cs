global using MesMicroservice.Domain.AggregateModels.QualityStandardAggregate;

namespace MesMicroservice.Application.Commands.QualityStandards
{
    public class CreateQualityStandardCommandHandler : IRequestHandler<CreateQualityStandardCommand, bool>
    {
        private readonly IQualityStandardRepository _qualityStandardRepository;
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

        public CreateQualityStandardCommandHandler(IQualityStandardRepository qualityStandardRepository, IMaterialDefinitionRepository materialDefinitionRepository)
        {
            _qualityStandardRepository = qualityStandardRepository;
            _materialDefinitionRepository = materialDefinitionRepository;
        }

        public async Task<bool> Handle(CreateQualityStandardCommand request, CancellationToken cancellationToken)
        {
            var properties = request.Properties.Select(x => new QualityStandardProperty(
                x.PropertyId,
                x.Description,
                new Value(x.ValueString, x.ValueType),
                x.ValueUnitOfMeasure,
                x.Name))
                .ToList();

            var materialDefinitions = new List<MaterialDefinition>();
            foreach (var materialDefinitionId in request.MaterialDefinitionIds)
            {
                var materialDefinition = await _materialDefinitionRepository.GetAsync(materialDefinitionId) 
                    ?? throw new ResourceNotFoundException(nameof(MaterialDefinition), materialDefinitionId);
                materialDefinitions.Add(materialDefinition);
            }


            var qualityStandard = new QualityStandard(qualityStandardId: request.QualityStandardId,
                                                      description: request.Description ?? "--",
                                                      validFrom: request.ValidFrom,
                                                      validTo: request.ValidTo ?? DateTime.MinValue,
                                                      properties: properties,
                                                      materialDefinitions: materialDefinitions,
                                                      status: EStatus.Available);

            await _qualityStandardRepository.Add(qualityStandard);

            return await _qualityStandardRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }


    }
}
