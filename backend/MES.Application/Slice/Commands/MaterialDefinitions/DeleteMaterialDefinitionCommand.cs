namespace MesMicroservice.Application.Commands.MaterialDefinitions;

public class DeleteMaterialDefinitionCommand: IRequest<bool>
{
    public string MaterialDefinitionId { get; set; }

    public DeleteMaterialDefinitionCommand(string materialDefinitionId)
    {
        MaterialDefinitionId = materialDefinitionId;
    }
}
