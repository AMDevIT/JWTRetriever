# JWTRetriever

JWT Retriever is a Windows WPF application written for dotnet 7 that retrieve the JWT token of a specified OAuth client app using the MSAL library.

## Why use it

Configuring your client app inside JWT Retriever allow you to retrieve a valud App-to-App (Confidential) or user interactive token from your B2C tenant. This allow you to test the authentication flows and the JWT token payload using a tool like [Microsoft JWT test site](https://jwt.ms/)

## How to use it

It's simple. Just fill the requested fields, that you can retrieve from the application configuration in your OAUTH app. 

* TenantID: the tenant ID of the application. It's common to the applications universe of the directory.
* ClientID: the ID of the application registered whithin the directory.
* Authority: the authority of the OAUTH process. Usually is the login URI appended with the tenantID in path.
* Domain: it's the B2C directory domain, as configured on Azure.
* API Url: it's the B2C API url as configured in the application dashboard on Azure.
* Scopes: can be needed for interactive login, usually default to ./default in the client application settings.
* Client secret: needed only for App-To-App login. The secret assigned to current client application.

Filling everything allow you to chose from Interactive login and App-To-App login.

You can save the configuration in a JSON file using menu Configuration -> Save configuration, as you can restore it using Configuration -> Load configuration.

## License

This application is provided using MIT license. 



