namespace MesMicroservice.Application.Commands.MaterialClasses;

public class UpdateMaterialClassCommand: IRequest<bool>
{
    public string MaterialClassId { get; set; }
    public string Name { get; set; }

    public UpdateMaterialClassCommand(string materialClassId, string name)
    {
        MaterialClassId = materialClassId;
        Name = name;
    }
}
