using Microsoft.Identity.Client;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AMDevIT.Security.JWTRetriever.Security.OAuth
{
    public class GenericOAuthClient
    {
        #region Properties

        public string ClientID
        {
            get;
            set;
        }

        public string TenantID
        {
            get;
            set;
        }

        public string? APIUrl
        {
            get;
            set;
        }

        public string? RedirectUri
        {
            get;
            set;
        }

        #endregion

        #region .ctor

        public GenericOAuthClient(string clientID,
                                  string tenantID)
            : this(clientID,
                   tenantID,
                   null,
                   null)
        {

        }

        public GenericOAuthClient(string clientID,
                                  string tenantID,
                                  string? apiUrl,
                                  string? redirectUri)
        {
            this.APIUrl = apiUrl;
            this.ClientID = clientID;
            this.TenantID = tenantID;
            this.RedirectUri = redirectUri;
        }

        #endregion

        #region Methods

        public async Task<string> AuthenticateInteractive(string[]? scopes,
                                                          string? authority = null,
                                                          CancellationToken cancellationToken = default)
        {
            PublicClientApplicationOptions publicClientApplicationOptions = new PublicClientApplicationOptions();
            PublicClientApplicationBuilder publicClientApplicationBuilder;
            IPublicClientApplication publicClientApplication;

            publicClientApplicationBuilder = PublicClientApplicationBuilder.CreateWithApplicationOptions(publicClientApplicationOptions)
                                                                           .WithTenantId(this.TenantID)
                                                                           .WithClientId(this.ClientID)
                                                                           .WithLogging(this.AuthenticationLoggerCallback,
                                                                                        LogLevel.Verbose,
                                                                                        true);

            if (string.IsNullOrWhiteSpace(authority))
                publicClientApplicationBuilder = publicClientApplicationBuilder.WithAuthority(AzureCloudInstance.AzurePublic,
                                                                                              this.TenantID);
            else
                publicClientApplicationBuilder = publicClientApplicationBuilder.WithAuthority(authority);

            if (!string.IsNullOrWhiteSpace(this.RedirectUri))
                publicClientApplicationBuilder = publicClientApplicationBuilder.WithRedirectUri(this.RedirectUri);

            publicClientApplication = publicClientApplicationBuilder.Build();

            try
            {
                AcquireTokenInteractiveParameterBuilder tokenTaskBuilder;
                AuthenticationResult authenticationResult;

                if (scopes == null)
                    scopes = new string[]
                             {
                                $"{this.APIUrl ?? string.Empty}/.default"
                             };

                tokenTaskBuilder = publicClientApplication.AcquireTokenInteractive(scopes);
                authenticationResult = await tokenTaskBuilder.ExecuteAsync(cancellationToken);
                return authenticationResult.AccessToken;
            }
            catch (Exception exc)
            {
                Trace.WriteLine(exc.ToString());
                throw;
            }
        }

        public async Task<string> GetApplicationTokenAsync(string applicationSecret,
                                                           string[]? scopes = null,
                                                           CancellationToken cancellationToken = default)
        {
            ConfidentialClientApplicationOptions clientApplicationOptions = new ConfidentialClientApplicationOptions();
            ConfidentialClientApplicationBuilder confidentialClientApplicationBuilder;
            IConfidentialClientApplication instantiatedClientApplication;

            confidentialClientApplicationBuilder = ConfidentialClientApplicationBuilder
                .CreateWithApplicationOptions(clientApplicationOptions)
                .WithTenantId(this.TenantID)
                .WithClientId(this.ClientID)
                .WithClientSecret(applicationSecret)
                .WithAuthority(AzureCloudInstance.AzurePublic, this.TenantID)
                .WithLogging(this.AuthenticationLoggerCallback,
                             LogLevel.Verbose,
                             true);

            if (!string.IsNullOrWhiteSpace(this.RedirectUri))
                confidentialClientApplicationBuilder = confidentialClientApplicationBuilder.WithRedirectUri(this.RedirectUri);


            instantiatedClientApplication = confidentialClientApplicationBuilder.Build();

            try
            {
                if (scopes == null)
                    scopes = new string[]
                             {
                                $"{this.APIUrl ?? string.Empty}/.default"
                             };

                AcquireTokenForClientParameterBuilder tokenTaskBuilder = instantiatedClientApplication.AcquireTokenForClient(scopes);

                AuthenticationResult authenticationResult = await tokenTaskBuilder.ExecuteAsync(cancellationToken);
                return authenticationResult.AccessToken;
            }
            catch (Exception exc)
            {
                Trace.WriteLine(exc.ToString());
                throw;
            }
        }

        private void AuthenticationLoggerCallback(LogLevel level, string message, bool containsPii)
        {
            Trace.WriteLine($"[{level}] {message}");
        }

        #endregion
    }
}
