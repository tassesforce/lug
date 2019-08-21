using System.Collections.Generic;

namespace lug.Handler.Token
{
    /// <summary>Handler de gestion des JWT</summary>
    public class TokenCriterias
    {
        ///<summary>Identifiant anonymise de l'utilisateur</summary>
        public string UserId {get;set;}
        ///<summary>Login de l'utilisateur</summary>
        public string Login {get;set;}
        ///<summary>Type de compte (agence, entreprise, candidat)</summary>
        public string AccountType {get;set;}
        ///<summary>Roles de l'utilisateur</summary>
        public List<string> Roles {get;set;}
        ///<summary>Durabilite du token</summary>
        public int TokenDurability {get;set;}
        ///<summary>Client Id de l'application consommatrice</summary>
        public string ClientId {get;set;}
        ///<summary>Issuer du token</summary>
        public string Issuer {get;set;}
        ///<summary>Audience du token</summary>
        public string Audience {get;set;}
        ///<summary>Cle de chiffrement</summary>
        public string Key {get;set;}
    }
}