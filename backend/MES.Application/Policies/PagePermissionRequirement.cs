global using Microsoft.AspNetCore.Authorization;

namespace MesMicroservice.Application.Policies;


public class PagePermissionRequirement : IAuthorizationRequirement
{
    public bool CanEdit { get; }
    public int PageId { get; }

    public PagePermissionRequirement(bool canEdit, int pageId)
    {
        CanEdit = canEdit;
        PageId = pageId;
    }
}
