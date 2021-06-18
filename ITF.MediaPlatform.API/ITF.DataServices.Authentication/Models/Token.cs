using System;
using System.ComponentModel.DataAnnotations;

namespace ITF.DataServices.Authentication.Models
{
    public class Token
    {
        public int UserId { get; set; }
        public string AuthToken { get; set; }
        public DateTime IssuedOn { get; set; }
        public DateTime ExpiresOn { get; set; }

        [Key]
        public int TokenId { get; set; }
    }
}
