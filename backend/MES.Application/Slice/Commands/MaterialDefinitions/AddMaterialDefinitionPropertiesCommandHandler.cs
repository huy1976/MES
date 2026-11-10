namespace MesMicroservice.Application.Commands.MaterialDefinitions
{
    public class AddMaterialDefinitionPropertiesCommandHandler : IRequestHandler<AddMaterialDefinitionPropertiesCommand, bool>
    {
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

        public AddMaterialDefinitionPropertiesCommandHandler(IMaterialDefinitionRepository materialDefinitionRepository)
        {
            _materialDefinitionRepository = materialDefinitionRepository;
        }

        public async Task<bool> Handle(AddMaterialDefinitionPropertiesCommand request, CancellationToken cancellationToken)
        {
            var materialDefinition = await _materialDefinitionRepository.GetAsync(request.MaterialDefinitionId);

            var properties = request.Properties.Select(x => new MaterialDefinitionProperty(
                x.PropertyId,
                x.Description,
                new Value(x.ValueString, x.ValueType),
                x.ValueUnitOfMeasure,
                x.Name))
                .ToList();

            if (materialDefinition is null)
            {
                throw new ResourceNotFoundException(nameof(MaterialDefinition), request.MaterialDefinitionId);
            }

            materialDefinition.AddProperties(properties);

            return await _materialDefinitionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
