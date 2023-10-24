namespace AMDevIT.Security.JWTRetriever.Models.Configuration
{
    public class ConfigurationData
    {
        #region Properties

        public string? ClientID
        {
            get;
            set;
        }

        public string? TenantID
        {
            get;
            set;
        }

        public string? Authority
        {
            get;
            set;
        }

        public string? ClientSecret
        {
            get;
            set;
        }

        public string? APIUrl
        {
            get;
            set;
        }

        public string[]? Scopes
        {
            get;
            set;
        }

        public string? Domain
        {
            get;
            set;
        }

        #endregion

        #region .ctor

        public ConfigurationData()
        {

        }

        public ConfigurationData(string? clientID,
                                 string? tenantID,
                                 string? authority,
                                 string? clientSecret,
                                 string? apiUrl,
                                 string? domain,
                                 string[]? scopes)
        {
            this.ClientID = clientID;
            this.TenantID = tenantID;
            this.Authority = authority;
            this.ClientSecret = clientSecret;
            this.APIUrl = apiUrl;
            this.Scopes = scopes;
            this.Domain = domain;
        }

        #endregion
    }
}
