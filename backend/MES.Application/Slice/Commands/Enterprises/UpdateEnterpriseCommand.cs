namespace MesMicroservice.Application.Commands.Enterprises;

public class UpdateEnterpriseCommand : IRequest<bool>
{
    public string EnterpriseId { get; set; }
    public string HierarchyModelId { get; set; }
    public string Name { get; set; }

    public UpdateEnterpriseCommand(string enterpriseId, string hierarchyModelId, string name)
    {
        EnterpriseId = enterpriseId;
        HierarchyModelId = hierarchyModelId;
        Name = name;
    }
}
