namespace MesMicroservice.Application.Commands.ProductionParameters
{
    public class AddProductionParameterPropertiesCommandHandler : IRequestHandler<AddProductionParameterPropertiesCommand, bool>
    {
        private readonly IProductionParameterRepository _productionParameterRepository;

        public AddProductionParameterPropertiesCommandHandler(IProductionParameterRepository productionParameterRepository)
        {
            _productionParameterRepository = productionParameterRepository;
        }

        public async Task<bool> Handle(AddProductionParameterPropertiesCommand request, CancellationToken cancellationToken)
        {
            var productionParameter = await _productionParameterRepository.GetAsync(request.ProductionParameterId);

            var properties = request.Properties.Select(x => new ProductionParameterProperty(
                x.PropertyId,
                x.Description,
                new Value(x.ValueString, x.ValueType),
                x.ValueUnitOfMeasure,
                x.Name))
                .ToList();

            if (productionParameter is null)
            {
                throw new ResourceNotFoundException(nameof(ProductionParameter), request.ProductionParameterId);
            }

            productionParameter.AddProperties(properties);

            return await _productionParameterRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
