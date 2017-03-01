# Elmah.Io.ContentSecurityPolicy.Proxy
Proxy for capturing Content-Security-Policy results in elmah.io

## Installation
Deploy the repository to a website on Azure. Set up API key and log ID, as explained here: [Configuration with Azure App Services and ASP.NET Core](https://blog.elmah.io/configuration-with-azure-app-services-and-aspnetcore/).

Add the `Content-Security-Policy` header to your web application and specify `<base-url>\reportOnly` as the `report-uri`.