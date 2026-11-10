namespace MesMicroservice.Domain.AggregateModels.UserAggregate
{
    public class Permission : IAggregateRoot
    {
        public string PermissionId { get; private set; } = Guid.NewGuid().ToString();
        public string Description { get; set; } = string.Empty;
        public int PageId { get; set; } 
        public Roles Role { get; set; }
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }

    }
}
