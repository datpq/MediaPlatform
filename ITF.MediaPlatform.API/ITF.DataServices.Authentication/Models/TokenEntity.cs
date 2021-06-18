using System;

namespace ITF.DataServices.Authentication.Models
{
    public class TokenEntity
    {
        public int UserId { get; set; }
        public DateTime IssuedOn { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string AuthToken { get; set; }
    }
}
