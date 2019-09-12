---
page_type: sample
languages:
- css
- csharp
- javascript
- html
- asp
products:
- azure
description: "This code sample is one of three referenced in the Azure AD sessions of the Microsoft Cloud Roadshow. Recordings of these sessions will be available shortly here."
urlFragment: active-directory-dotnet-demo-tdlr
---


# Demo - To-do list reimagined web app
This code sample is one of three referenced in the Azure AD sessions of the [Microsoft Cloud Roadshow](https://www.microsoftcloudroadshow.com/).  Recordings of these sessions will be available shortly [here](https://mva.microsoft.com/en-US/training-courses/add-identity-into-your-cloudbased-apps-13989).  We recommend you watch one of these recordings to understand the purpose and goals of this code sample.

To-Do List Reimagined (tdlr;) is a new cloud service that allows users to store and manage a list of tasks.  It integrates with Azure AD in order to provide enterprise features to its customers that have an existing Azure AD tenant.  These features include:

- Discovery of accounts with an existing Azure AD tenant
- Signing up for the app with a work account
- One-click user authorization using consent & OAuth 2.0
- Signing into the app with a work account
- Sharing tasks between users in the same company using a "people picker".
- Outsourced user management to company admins

The full service consists of three different sample projects:

- [The tdlr; web application](https://github.com/azureadsamples/azureroadshow-web), written as .NET 4.5 MVC app.
- [The tdlr; iOS application](https://github.com/azureadsamples/azureroadshow-xamarin), written as a cross-platform Xamarin app.
- [The tdlr; admin web portal](https://github.com/azureadsamples/azureroadshow-web-autouserprovisioning), written as a .NET 4.5 MVC app.

## Running the tdlr; web app

### Register an app with Azure AD

You'll first need to register an app in the Azure Management Portal so that your version of tdlr; can sign users in and get information from Azure AD.  

You'll need an Azure Activce Directory tenant in which to register your application. For more information on how to get an Azure AD tenant, please see [How to get an Azure AD tenant](https://azure.microsoft.com/en-us/documentation/articles/active-directory-howto-tenant/). You may also wish to create an additional tenant, since the tdlr; app is 'multi-tenant' - it allows users from any organization to sign up & sign in.  You'll want to create a few users in your tenant(s) for testing purposes - a guest user with a personal MSA account will not work for this sample.

1. Sign in to the [Azure portal](https://portal.azure.com).
2. On the top bar, click on your account and under the **Directory** list, choose the Active Directory tenant where you wish to register your application.
3. Click on **More Services** in the left hand nav, and choose **Azure Active Directory**.
4. Click on **App registrations** and choose **Add**.
5. Enter a friendly name for the application, for example 'TDLR;' and select 'Web Application and/or Web API' as the Application Type. For the sign-on URL, enter the base URL for the sample, which is by default `https://localhost:44322/`. NOTE:  It is important, due to the way Azure AD matches URLs, to ensure there is a trailing slash on the end of this URL.  If you don't include the trailing slash, you will receive an error when the application attempts to redeem an authorization code. Click  **Create** to create the application.
6. While still in the Azure portal, choose your application, click on **Settings** and choose **Properties**.
7. Find the Application ID value and copy it to the clipboard.   
8. Find "multi-tenanted" switch and flip it to yes.
9. Edit the App ID URI - enter `https://<your_tenant_name>/tdlr`, replacing `<your_tenant_name>` with the name of your Azure AD tenant, like `mytenant.onmicrosoft.com`.
10. From the Settings menu, choose **Keys** and add a key - select a key duration of either 1 year or 2 years. When you save this page, the key value will be displayed, copy and save the value in a safe location - you will need this key later to configure the project in Visual Studio - this key value will not be displayed again, nor retrievable by any other means, so please record it as soon as it is visible from the Azure Portal.
11. Configure permissions for your application - in the Settings menu, choose the 'Required permissions' section, click on **Add**, then **Select an API**, and select 'Microsoft Graph' (this is the Graph API). Then, click on  **Select Permissions** and select 'Read all users' basic profiles'.  


### Download the code

Now you can [download this repo as a zip](https://github.com/AzureADSamples/azureroadshow-web/archive/master.zip) or clone it to your local machine:

`git clone https://github.com/azureadsamples/azureroadshow-web`

In your local repo, open the `TDLR.sln` file.  We recommned you use [Visual Studio 2015](https://www.visualstudio.com/), which will restore all necessary packages for you when you run the app for the first time. 

### Edit the app's config

To run the app, you'll need to enter the information from your app registration.  In Visual Studio, open the `web.config` file in the root of the project and locate the `<appSettings>` section.  Replace the following values with your own:

```
    <add key="ida:ClientId" value="[Enter your clientID from the Azure Management Portal, e.g. b1132c6b-fbf8-43b3-a9d8-329be1c87fcb]" />
    <add key="ida:AppKey" value="[Enter your key from the Azure Management Portal, e.g. TpNUr1CrYMP5bkvXKwmRKQvINuTp2nyp4kIzoabgZC0=]" />
    <add key="ida:Tenant" value="[Enter the name of the tenant where you registered your app, e.g. mytenant.onmicrosoft.com]" />
    <add key="ida:TaskApiResourceId" value="[Enter your App ID URI from the Azure Management Portal, e.g. https://mytenant.onmicrosoft.com/tdlr]" />
```

### Run the app!

You can now run the tdlr; app and explore its functionality.  Try signing up and signing in with your Azure AD users, creating tasks, and sharing them with other users.  To understand the code behind the app, we recommend you watch on of the recorded Microsoft Cloud Roadshow sessions which will be available soon [here](https://mva.microsoft.com/en-US/training-courses/add-identity-into-your-cloudbased-apps-13989).  If you're already familiar with Azure AD, you may find the code comments instructive as well.


