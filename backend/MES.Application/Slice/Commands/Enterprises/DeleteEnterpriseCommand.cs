namespace MesMicroservice.Application.Commands.Enterprises;

public class DeleteEnterpriseCommand : IRequest<bool>
{
    public string EnterpriseId { get; set; }

    public DeleteEnterpriseCommand(string enterpriseId)
    {
        EnterpriseId = enterpriseId;
    }
}
