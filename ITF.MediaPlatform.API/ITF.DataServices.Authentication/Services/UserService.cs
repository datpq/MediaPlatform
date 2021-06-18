using System;
using System.Diagnostics;
using ITF.DataServices.Authentication.Data;
using ITF.DataServices.Authentication.Models;
using NLog;

namespace ITF.DataServices.Authentication.Services
{
    public class UserService : IUserService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDataRepository _dataRepository;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public UserService(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        /// <summary>
        /// Public method to authenticate user by user name and password.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int Authenticate(string userName, string password)
        {
            try
            {
                var stopWatch = Stopwatch.StartNew();
                var user = _dataRepository.Get<User>(u => u.Enabled && u.UserName == userName && u.Password == password);
                var result = 0;
                if (user != null && user.UserId > 0)
                {
                    result = user.UserId;
                }
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"Authenticate({userName},{user?.UserId}), Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }
    }
}
