namespace LibraryManagement.Dto.Request
{
    public class RolePermissionUpdateRequest
    {
        public string OldRoleName { get; set; }
        public string OldPermissionName { get; set; }
        public string? NewRoleName { get; set; }
        public string? NewPermissionName { get; set; }
    }
}
