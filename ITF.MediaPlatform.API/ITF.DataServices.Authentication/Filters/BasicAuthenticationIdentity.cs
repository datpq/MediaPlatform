using System.Security.Principal;

namespace ITF.DataServices.Authentication.Filters
{
    public class BasicAuthenticationIdentity : GenericIdentity
    {
        /// <summary>
        /// Get/Set for password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Get/Set for UserName
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Get/Set for Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Basic Authentication Identity Constructor
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public BasicAuthenticationIdentity(string userName, string password) : base(userName, "Basic")
        {
            Password = password;
            UserName = userName;
        }
    }
}