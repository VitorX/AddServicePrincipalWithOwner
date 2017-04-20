using Microsoft.Azure.ActiveDirectory.GraphClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddServicePrincipalWithOwner
{
    class ServicePrincipals
    {

        public async void CreateServicePrincipalWithOwner()
        {
            string accessToken = "";
            string graphResourceId = "https://graph.windows.net";
            string tenantId = "xxx.onmicrosoft.com";
            string ownUserName = "xxx@adfei.onmicrosoft.com";
            string appName = "testAppOwner";

            Uri servicePointUri = new Uri(graphResourceId);
            Uri serviceRoot = new Uri(servicePointUri, tenantId);

            ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot, async () => await Task.FromResult(accessToken));

            var user = (Microsoft.Azure.ActiveDirectory.GraphClient.User)await activeDirectoryClient.Users.Where(u => u.UserPrincipalName == ownUserName).ExecuteSingleAsync();

            string applicationObjectId = Guid.NewGuid().ToString();
            string servicePrincipalObjectId = Guid.NewGuid().ToString();
            Application application = new Application()
            {
                AvailableToOtherTenants = false,
                DisplayName = appName,
                ErrorUrl = null,
                GroupMembershipClaims = null,
                Homepage = "http://" + appName,
                IdentifierUris = new List<string>() { "https://" + appName },
                KeyCredentials = new List<KeyCredential>(),
                KnownClientApplications = new List<Guid>(),
                LogoutUrl = null,
                Oauth2AllowImplicitFlow = false,
                Oauth2AllowUrlPathMatching = false,
                Oauth2Permissions = new List<OAuth2Permission>(),
                Oauth2RequirePostResponse = false,
                PasswordCredentials = new List<PasswordCredential>(),
                PublicClient = false,
                ReplyUrls = new List<string>(),
                RequiredResourceAccess = new List<RequiredResourceAccess>(),
                SamlMetadataUrl = null,
                ExtensionProperties = new List<ExtensionProperty>(),
                Manager = null,
                ObjectType = "Application",
                DeletionTimestamp = null,
                CreatedOnBehalfOf = null,
                CreatedObjects = new List<DirectoryObject>(),
                DirectReports = new List<DirectoryObject>(),
                Members = new List<DirectoryObject>(),
                MemberOf = new List<DirectoryObject>(),
                Owners = new List<DirectoryObject>() { user },
                OwnedObjects = new List<DirectoryObject>(),
                Policies = new List<DirectoryObject>(),
                ObjectId = applicationObjectId,
            };

            activeDirectoryClient.Applications.AddApplicationAsync(application).Wait();


            ServicePrincipal newServicePrincpal = new ServicePrincipal();
            if (application != null)
            {
                newServicePrincpal.DisplayName = application.DisplayName;
                newServicePrincpal.AccountEnabled = true;
                newServicePrincpal.AppId = application.AppId;
                newServicePrincpal.Owners.Add(user);
                newServicePrincpal.ObjectId = servicePrincipalObjectId;
                try
                {
                    activeDirectoryClient.ServicePrincipals.AddServicePrincipalAsync(newServicePrincpal).Wait();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }


            var listSP= activeDirectoryClient.ServicePrincipals.Where(servicePrincipal => servicePrincipal.DisplayName.Equals(appName) ).ExecuteAsync().Result.CurrentPage.ToList();
            if (listSP.Count > 0)
            {
                var sp = (ServicePrincipal)listSP[0];
                sp.Owners.Add(user);
                sp.UpdateAsync();
            }

        }
    }
}
