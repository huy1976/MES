namespace MesMicroservice.Application.Queries.OperationsDefinitions
{
    public class GetOperationsDefinitionByIdQuery : IRequest<OperationDTO>
    {
        public string OperationId { get; set; }

        public GetOperationsDefinitionByIdQuery(string operationId)
        {
            OperationId = operationId;
        }
    }
}
