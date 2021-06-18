using System.Configuration;
using System;
using System.Linq;
using ITF.DataServices.Authentication.Data;
using ITF.DataServices.Authentication.Models;

namespace ITF.DataServices.Authentication.Services
{
    public class TokenService : ITokenService
    {
        #region Private member variables.
        private const string DefaultTokenExpiry = "900";//15 minutes
        private readonly IDataRepository _dataRepository;
        #endregion

        #region Public constructor.
        /// <summary>
        /// Public constructor.
        /// </summary>
        public TokenService(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }
        #endregion


        #region Public member methods.

        /// <summary>
        ///  Function to generate unique token with expiry against the provided userId.
        ///  Also add a record in database for generated token.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public TokenEntity GenerateToken(int userId)
        {
            string token = Guid.NewGuid().ToString();
            DateTime issuedOn = DateTime.Now;
            DateTime expiredOn = DateTime.Now.AddSeconds(
                Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"] ?? DefaultTokenExpiry));
            var tokendomain = new Token
            {
                UserId = userId,
                AuthToken = token,
                IssuedOn = issuedOn,
                ExpiresOn = expiredOn
            };

            _dataRepository.Add(tokendomain);
            _dataRepository.Commit();
            var tokenModel = new TokenEntity()
            {
                UserId = userId,
                IssuedOn = issuedOn,
                ExpiresOn = expiredOn,
                AuthToken = token
            };

            return tokenModel;
        }

        /// <summary>
        /// Method to validate token against expiry and existence in database.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public bool ValidateToken(string tokenId)
        {
            var now = DateTime.Now;
            var token = _dataRepository.Get<Token>(t => t.AuthToken == tokenId && t.ExpiresOn > now);
            if (token != null)
            {
                token.ExpiresOn = now.AddSeconds(Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"] ?? DefaultTokenExpiry));
                //_dataRepository.Update(token);
                _dataRepository.Commit();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method to kill the provided token id.
        /// </summary>
        /// <param name="tokenId">true for successful delete</param>
        public bool Kill(string tokenId)
        {
            _dataRepository.Delete<Token>(x => x.AuthToken == tokenId);
            _dataRepository.Commit();
            var isNotDeleted = _dataRepository.GetMany<Token>(x => x.AuthToken == tokenId).Any();
            if (isNotDeleted) { return false; }
            return true;
        }

        /// <summary>
        /// Delete tokens for the specific deleted user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>true for successful delete</returns>
        public bool DeleteByUserId(int userId)
        {
            _dataRepository.Delete<Token>(x => x.UserId == userId);
            _dataRepository.Commit();

            var isNotDeleted = _dataRepository.GetMany<Token>(x => x.UserId == userId).Any();
            return !isNotDeleted;
        }

        #endregion
    }
}
