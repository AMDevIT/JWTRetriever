using AMDevIT.Security.JWTRetriever.Common;
using AMDevIT.Security.JWTRetriever.Models.Configuration;
using AMDevIT.Security.JWTRetriever.Security.OAuth;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace AMDevIT.Security.JWTRetriever.ViewModels
{
    public class MainWindowViewModel
        : ViewModelBase
    {
        #region Fields

        private string? tenantID;
        private string? clientID;
        private string? authority;
        private string? clientSecret;
        private string? apiUrl;
        private string? groupedScopes;
        private string? domain;
        private string? redirectURI = null;

        private string? jwtToken;

        #endregion

        #region Properties

        public string? ClientID
        {
            get
            {
                return this.clientID;
            }
            set
            {
                if (this.SetProperty(ref this.clientID, value))
                {
                    this.AuthenticateApplicationCommand.CanExecute(null);
                    this.AuthenticateInteractiveCommand.CanExecute(null);
                }
            }
        }

        public string? TenantID
        {
            get
            {
                return this.tenantID;
            }
            set
            {
                if (this.SetProperty(ref this.tenantID, value))
                {
                    this.AuthenticateApplicationCommand.CanExecute(null);
                    this.AuthenticateInteractiveCommand.CanExecute(null);
                }
            }
        }

        public string? Authority
        {
            get
            {
                return this.authority;
            }
            set
            {
                if (this.SetProperty(ref this.authority, value))
                {
                    this.AuthenticateApplicationCommand.CanExecute(null);
                    this.AuthenticateInteractiveCommand.CanExecute(null);
                }
            }
        }

        public string? ClientSecret
        {
            get
            {
                return this.clientSecret;
            }
            set
            {
                if (this.SetProperty(ref this.clientSecret, value))
                {
                    this.AuthenticateApplicationCommand.CanExecute(null);
                    this.AuthenticateInteractiveCommand.CanExecute(null);
                }
            }
        }

        public string? APIUrl
        {
            get
            {
                return this.apiUrl;
            }
            set
            {
                if (this.SetProperty(ref this.apiUrl, value))
                {
                    this.AuthenticateApplicationCommand.CanExecute(null);
                    this.AuthenticateInteractiveCommand.CanExecute(null);
                }
            }
        }

        public string? GroupedScopes
        {
            get
            {
                return this.groupedScopes;
            }
            set
            {
                if (this.SetProperty(ref this.groupedScopes, value))
                {
                    this.AuthenticateApplicationCommand.CanExecute(null);
                    this.AuthenticateInteractiveCommand.CanExecute(null);
                }
            }
        }

        public string? Domain
        {
            get
            {
                return this.domain;
            }
            set
            {
                if (this.SetProperty(ref this.domain, value))
                {
                    this.AuthenticateApplicationCommand.CanExecute(null);
                    this.AuthenticateInteractiveCommand.CanExecute(null);
                }
            }
        }

        public string? JWTToken
        {
            get
            {
                return this.jwtToken;
            }
            set
            {
                this.SetProperty(ref this.jwtToken, value);
            }
        }

        public string? RedirectUri
        {
            get
            {
                return this.redirectURI;   
            }
            set
            {
                this.SetProperty(ref this.redirectURI, value);
            }
        }

        #endregion

        #region Commands

        private readonly Lazy<DelegateCommand> authenticateApplicationCommand;
        private readonly Lazy<DelegateCommand> authenticateInteractiveCommand;

        private readonly Lazy<DelegateCommand> loadSettingsCommand;
        private readonly Lazy<DelegateCommand> saveSettingsCommand;
        private readonly Lazy<DelegateCommand> exitApplicationCommand;

        public DelegateCommand AuthenticateApplicationCommand
        {
            get
            {
                return this.authenticateApplicationCommand.Value;
            }
        }

        public DelegateCommand AuthenticateInteractiveCommand
        {
            get
            {
                return this.authenticateInteractiveCommand.Value;
            }
        }

        public DelegateCommand LoadSettingsCommand
        {
            get
            {
                return this.loadSettingsCommand.Value;
            }
        }

        public DelegateCommand SaveSettingsCommand
        {
            get
            {
                return this.saveSettingsCommand.Value;
            }
        }

        public DelegateCommand ExitApplicationCommand
        {
            get
            {
                return this.exitApplicationCommand.Value;
            }
        }

        #endregion

        #region .ctor

        public MainWindowViewModel()
        {
            this.authenticateApplicationCommand = new Lazy<DelegateCommand>(() =>
            {
                return new DelegateCommand(this.LoginWithApplication,
                                           this.CanLoginWithApplication);
            });

            this.authenticateInteractiveCommand = new Lazy<DelegateCommand>(() =>
            {
                return new DelegateCommand(this.LoginWithInteractive,
                                           this.CanLoginWithInteractive);
            });

            this.loadSettingsCommand = new Lazy<DelegateCommand>(() =>
            {
                return new DelegateCommand(this.LoadConfiguration);
            });

            this.saveSettingsCommand = new Lazy<DelegateCommand>(() =>
            {
                return new DelegateCommand(this.SaveConfiguration);
            });

            this.exitApplicationCommand = new Lazy<DelegateCommand>(() =>
            {
                return new DelegateCommand(this.ExitApplication);
            });
        }

        #endregion

        #region Methods

        private bool CanLoginWithApplication(object? parameter)
        {
            if (!string.IsNullOrWhiteSpace(this.TenantID) &&
                !string.IsNullOrWhiteSpace(this.ClientID) &&
                !string.IsNullOrWhiteSpace(this.Domain) &&
                !string.IsNullOrWhiteSpace(this.ClientSecret))
                return true;
            else
                return false;
        }

        private void LoginWithApplication(object? parameter)
        {
            string[]? scopes = this.ParseScopes(this.GroupedScopes);
            GenericOAuthClient oAuthClient;
            Task<string> jwtAuthTask;

            oAuthClient = new GenericOAuthClient(this.ClientID!,
                                                 this.TenantID!,
                                                 this.APIUrl,
                                                 this.RedirectUri);

            if (scopes.Length == 0)
            {
                scopes = new string[]
                {
                    $"{this.APIUrl ?? string.Empty}/.default",
                };
            }

            jwtAuthTask = oAuthClient.GetApplicationTokenAsync(this.ClientSecret!,
                                                               scopes);
            jwtAuthTask.ContinueWith((taskResult) =>
            {
                string? jwtToken;

                if (taskResult.IsFaulted == false)
                    jwtToken = taskResult.Result;
                else
                    jwtToken = taskResult.Exception?.ToString();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.JWTToken = jwtToken;
                });
            });
        }

        private bool CanLoginWithInteractive(object? parameter)
        {
            if (!string.IsNullOrWhiteSpace(this.TenantID) &&
                !string.IsNullOrWhiteSpace(this.ClientID) &&
                !string.IsNullOrWhiteSpace(this.Authority))
                return true;
            else
                return false;
        }

        private void LoginWithInteractive(object? parameter)
        {
            string[]? scopes = this.ParseScopes(this.GroupedScopes);
            GenericOAuthClient oAuthClient;
            Task<string> jwtAuthTask;

            oAuthClient = new GenericOAuthClient(this.ClientID!,
                                                 this.TenantID!,
                                                 this.APIUrl,
                                                 this.RedirectUri);
            jwtAuthTask = oAuthClient.AuthenticateInteractive(scopes,
                                                              this.Authority);
            jwtAuthTask.ContinueWith((taskResult) =>
            {
                string? jwtToken;

                if (taskResult.IsFaulted == false)
                    jwtToken = taskResult.Result;
                else
                    jwtToken = taskResult.Exception?.ToString();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.JWTToken = jwtToken;
                });
            });
        }

        private void LoadConfiguration(object? parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool? dialogResult;

            openFileDialog.CheckFileExists = true;
            openFileDialog.DefaultExt = "*.json";
            openFileDialog.Filter = "JSON Configuration file|*.json";
            openFileDialog.Title = "Apri configurazione";
            openFileDialog.Multiselect = false;

            dialogResult = openFileDialog.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value == true)
            {
                Stream jsonFileStream = openFileDialog.OpenFile();
                string jsonText;

                using (StreamReader streamReader = new StreamReader(jsonFileStream))
                {
                    jsonText = streamReader.ReadToEnd();
                }

                if (!string.IsNullOrWhiteSpace(jsonText))
                {
                    ConfigurationData? configurationData = JsonSerializer.Deserialize<ConfigurationData>(jsonText);

                    if (configurationData != null)
                    {
                        this.ClientID = configurationData.ClientID;
                        this.TenantID = configurationData.TenantID;
                        this.Authority = configurationData.Authority;
                        this.ClientSecret = configurationData.ClientSecret;
                        this.APIUrl = configurationData.APIUrl;
                        this.Domain = configurationData.Domain;
                        this.RedirectUri = configurationData.RedirectURI;

                        if (configurationData.Scopes != null)
                            this.GroupedScopes = this.GroupScopes(configurationData.Scopes);
                        else
                            this.GroupedScopes = string.Empty;
                    }
                }
            }
        }

        private void SaveConfiguration(object? parameter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            bool? dialogResult;

            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.Title = "Salva configurazione client";
            saveFileDialog.DefaultExt = "*.json";
            saveFileDialog.Filter = "JSON Configuration file|*.json";

            dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult.HasValue == true && dialogResult.Value == true)
            {
                ConfigurationData configurationData;
                Stream fileStream;
                string[] scopes;
                string configurationJson;

                scopes = this.ParseScopes(this.GroupedScopes);
                configurationData = new ConfigurationData(this.ClientID,
                                                          this.TenantID,
                                                          this.Authority,
                                                          this.ClientSecret,
                                                          this.APIUrl,
                                                          this.Domain,
                                                          this.RedirectUri,
                                                          scopes);

                configurationJson = JsonSerializer.Serialize<ConfigurationData>(configurationData);
                fileStream = saveFileDialog.OpenFile();

                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine(configurationJson);
                }
            }
        }

        private void ExitApplication(object? parameter)
        {
            Application.Current.Shutdown();
        }

        private string GroupScopes(string[] scopes)
        {
            if (scopes.Length > 0)
                return string.Join(';', scopes);
            else
                return string.Empty;
        }

        private string[] ParseScopes(string? groupedScopes)
        {
            string[] scopes;

            if (!string.IsNullOrWhiteSpace(groupedScopes))
            {
                if (groupedScopes.Contains(';'))
                    scopes = groupedScopes.Split(';');
                else
                    scopes = new string[]
                    {
                        groupedScopes
                    };
            }
            else
                scopes = Array.Empty<string>();

            return scopes;
        }

        #endregion
    }
}
