namespace MesMicroservice.Infrastructure.Repositories.AuthModelRepositories
{
    public class PermissionRepository : BaseRepository, IPermissionRepository
    {
        public PermissionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> AddOrUpdatePermissionsAsync(List<Permission> permissions)
        {
            foreach (var permission in permissions)
            {
                var existingPermission = await _context.Permissions
                    .FirstOrDefaultAsync(p =>
                        p.Role == permission.Role &&
                        p.PageId == permission.PageId);

                if (existingPermission == null)
                {
                    await _context.Permissions.AddAsync(permission);
                }
                else
                {
                    existingPermission.CanView = permission.CanView;
                    existingPermission.CanEdit = permission.CanEdit;
                    existingPermission.Description = permission.Description;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Permission?> GetByIdAsync(string permissionId)
        {
            return await _context.Permissions
                .FirstOrDefaultAsync(p => p.PermissionId == permissionId);
        }

        public async Task<List<Permission>> GetByRole(Roles role)
        {
            return await _context.Permissions
                .Where(p => p.Role == role)
                .ToListAsync();
        }

        public async Task<bool> HasEditPermissionAsync(Roles role, int pageId)
        {
            return await _context.Permissions
                .AnyAsync(p =>
                    p.Role == role &&
                    p.PageId == pageId &&
                    p.CanEdit);
        }

        public async Task<bool> HasViewPermissionAsync(Roles role, int pageId)
        {
            return await _context.Permissions
                .AnyAsync(p =>
                    p.Role == role &&
                    p.PageId == pageId &&
                    p.CanView);
        }
    }
}