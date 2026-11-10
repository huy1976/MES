global using MesMicroservice.Domain.AggregateModels.ParameterAggregate;
using MesMicroservice.Application.Commands.QualityStandards;

namespace MesMicroservice.Application.Commands.ProductionParameters
{
    public class CreateProductionParameterCommandHandler : IRequestHandler<CreateProductionParameterCommand, bool>
    {
        private readonly IProductionParameterRepository _productionParameterRepository ;

        public CreateProductionParameterCommandHandler(IProductionParameterRepository productionParameterRepository)
        {
            _productionParameterRepository = productionParameterRepository;
        }

        public async Task<bool> Handle(CreateProductionParameterCommand request, CancellationToken cancellationToken)
        {
            var properties = request.Properties.Select(x => new ProductionParameterProperty(
                x.PropertyId,
                x.Description,
                new Value(x.ValueString, x.ValueType),
                x.ValueUnitOfMeasure,
                x.Name))
                .ToList();
            
            var productionParameter = new ProductionParameter(productionParameterId: request.ProductionParameterId,
                                                      description: request.Description ?? "--",
                                                      validFrom: request.ValidFrom,
                                                      validTo: request.ValidTo ?? DateTime.MinValue,
                                                      properties: properties,
                                                      status: EStatus.Available);

            await _productionParameterRepository.Add(productionParameter);

            return await _productionParameterRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }


    }
}
