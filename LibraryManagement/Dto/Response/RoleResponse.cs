using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Dto.Response
{
    public class RoleResponse
    {
        public string? RoleName { get; set; }

        public string? Description { get; set; }
    }
}
