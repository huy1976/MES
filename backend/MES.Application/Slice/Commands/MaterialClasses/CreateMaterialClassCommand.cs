namespace MesMicroservice.Application.Commands.MaterialClasses;

[DataContract]
public class CreateMaterialClassCommand : IRequest<bool>
{
    [DataMember]
    public string MaterialClassId { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string Description { get; private set; }
    [DataMember]
    public EAssemblyType? AssemblyType { get; private set; }
    [DataMember]
    public EAssemblyRelationship? AssemblyRelationship { get; private set; }
    [DataMember]
    public List<SavePropertyViewModel>? Properties { get; private set; }

    public CreateMaterialClassCommand(string materialClassId, string name, string description, EAssemblyType? assemblyType, EAssemblyRelationship? assemblyRelationship, List<SavePropertyViewModel>? properties)
    {
        MaterialClassId = materialClassId;
        Name = name;
        Description = description;
        AssemblyType = assemblyType;
        AssemblyRelationship = assemblyRelationship;
        Properties = properties;
    }
}
