using Microsoft.AspNetCore.Http;

namespace MesMicroservice.Application.Policies;

public class PagePermissionHandler : AuthorizationHandler<PagePermissionRequirement>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PagePermissionHandler(IHttpContextAccessor httpContextAccessor, IPermissionRepository permissionRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _permissionRepository = permissionRepository;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PagePermissionRequirement requirement)
    {
        var roleClaim = context.User.FindFirst(ClaimTypes.Role);
        if (roleClaim == null)
            return;

        if (!Enum.TryParse<Roles>(roleClaim.Value, ignoreCase: true, out var role))
            return;

        // Kiểm tra permission async
        var hasEdit = await _permissionRepository.HasEditPermissionAsync(role, requirement.PageId);

        if (requirement.CanEdit && hasEdit)
        {
            context.Succeed(requirement);
        }
    }

}