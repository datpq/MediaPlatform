using System.ComponentModel.DataAnnotations;

namespace ITF.DataServices.Authentication.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }

        [Key]
        public int UserId { get; set; }
    }
}
