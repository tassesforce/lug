using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using lug.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace lug.Handler.Token
{
    /*
     * Handler de gestion des JWT
     */
    public class TokenHandler
    {
        private readonly ILogger Logger;
        
        #region constructeur
        public TokenHandler(ILogger logger) {
            this.Logger = logger;
        }
        #endregion

        #region construction de tokens
        ///<summary>Generation de l'access token a renvoyer au client</summary>
        ///<param name="criterias">Criteres de creation du token</param>
        ///<returns>La chaine de caractere correspondant a l'access token</returns>
        public string GenerateAccessToken(TokenCriterias criterias)
        {
            // accessToken valable 2min
            // TODO exporter la durabilite dans un .conf
            return CreateToken(criterias);
        }

        ///<summary>Generation du refresh token a renvoyer au client</summary>
        ///<param name="criterias">Criteres de creation du token</param>
        ///<returns>La chaine de caractere correspondant au refresh token</returns>
        public string GenerateRefreshToken(TokenCriterias criterias)
        {
            // refreshToken valable 8h
            // TODO exporter la durabilite dans un .conf
            return CreateToken(criterias);
        }
        
        ///<summary>Creation d'un token</summary>
        ///<param name="criterias">Criteres de creation du token</param>
        ///<returns>La chaine de caractere correspondant au token</returns>
        private string CreateToken(TokenCriterias criterias)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            DateTime now = DateTime.UtcNow;
            ClaimsIdentity claims = new ClaimsIdentity(new[]
                {
                    new Claim( ClaimTypes.Name, criterias.Login, ClaimValueTypes.String),
                    new Claim( ClaimTypes.NameIdentifier, criterias.UserId, ClaimValueTypes.String),
                    new Claim( ClaimTypes.Spn, criterias.AccountType, ClaimValueTypes.String),
                    new Claim( ClaimTypes.Sid, criterias.ClientId, ClaimValueTypes.String)                    
                });
            if (criterias.Roles != null && criterias.Roles.Count > 0)
            {
                criterias.Roles.ForEach (r => claims.AddClaim(new Claim( ClaimTypes.Role, r, ClaimValueTypes.String)));
            }

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Issuer = criterias.Issuer,
                Expires = now.AddMinutes(criterias.TokenDurability),
                IssuedAt = now,
                Audience = criterias.Audience,
                SigningCredentials = new SigningCredentials(new Microsoft
                .IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(criterias.Key)), SecurityAlgorithms.HmacSha256Signature),

            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        #endregion

        #region lecture de token
        
        ///<summary>Recuperation du JWT a partir d'une string</summary>
        ///<param name="token">Token a lire</param>
        ///<returns>Le JwtToken issu de la chaine de caracteres</returns>
        public JwtSecurityToken ReadToken (string token)
        {
            if (string.IsNullOrEmpty(token)) {
                Logger.LogError("Le token demandé est invalide (chaine vide)");
                throw new InvalidTokenException();
            }
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(token);
        }
        #endregion

        #region refresh tokens
        
        ///<summary>Generation d'un nouvel access token a la presentation d'un refresh token valide</summary>
        ///<param name="token">Refresh token a utiliser</param>
        ///<param name="userId">Identifiant de l'utilisateur authentifie</param>
        ///<param name="clientId">Identifiant de l'application consommatrice</param>
        ///<param name="roles">Liste des roles de l'utilisateur authentifie</param>
        ///<returns>La chaine de caractere correspondant au nouvel access token</returns>
        public string RefreshToken(string token, TokenCriterias criterias)
        {
            JwtSecurityToken refreshToken = ReadToken(token);
            CheckRefreshToken(refreshToken, criterias.UserId, criterias.ClientId);
            
            return GenerateAccessToken(criterias);
        }

        ///<summary>Verifie la validite du refresh token</summary>
        ///<param name="refreshToken">Refresh token a utiliser</param>
        ///<param name="userId">Identifiant de l'utilisateur authentifie</param>
        ///<param name="clientId">Identifiant de l'application consommatrice</param>
        private void CheckRefreshToken(JwtSecurityToken refreshToken, string userId, string clientId)
        {
            if (DateTime.Now.Ticks > (Convert.ToInt64(refreshToken.Payload.Iss) + Convert.ToInt64(refreshToken.Payload.Exp))) {
                Logger.LogError("Le refresh token est périmé");
                throw new ObsoleteTokenException();
            }
            var userIdClaim = refreshToken.Payload.Claims.Single(c => c.Type == "unique_name");
            var clientIdClaim = refreshToken.Payload.Claims.Single(c => c.Type == "unique_name");
            if (!refreshToken.Subject.Equals(userId) && !refreshToken.Subject.Equals(clientId))
            {
                Logger.LogError("La cible du refresh token ne correspond pas");
                throw new InvalidSubjectTokenException();
            }
        }
        #endregion
    }
}