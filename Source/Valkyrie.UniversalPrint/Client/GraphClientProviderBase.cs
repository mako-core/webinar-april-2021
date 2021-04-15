// -----------------------------------------------------------------------
//  <copyright file="GraphClientProviderBase.cs" company="Global Graphics Software Ltd">
//      Copyright (c) 2020 Global Graphics Software Ltd. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Global Graphics Software Ltd. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace Valkyrie.UniversalPrint.Client
{
    public class GraphClientProviderBase
    {
        protected static GraphServiceClient GetApplicationClient()
        {
            var confidentialClientApplication = GetApplicationAppBuilder();

            var authProvider = new ClientCredentialProvider(confidentialClientApplication);

            return new GraphServiceClient(authProvider);
        }

        protected static IConfidentialClientApplication GetApplicationAppBuilder()
        {
            var confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create("todo")
                .WithTenantId("todo")
                .WithClientSecret("todo")
                .Build();
            return confidentialClientApplication;
        }

        protected static GraphServiceClient GetDelegatedClient()
        {
            var publicClientApplication = GetDelegatedApplicationBuilder();
            publicClientApplication.AcquireTokenInteractive(new List<string>());

            var interactiveAuthProvider = new InteractiveAuthenticationProvider(publicClientApplication);

            return new GraphServiceClient(interactiveAuthProvider);
        }

        protected static IPublicClientApplication GetDelegatedApplicationBuilder()
        {
            var publicClientApplication = PublicClientApplicationBuilder
                .Create("todo")
                .WithTenantId("todo")
                .WithDefaultRedirectUri()
                .Build();

            return publicClientApplication;
        }
    }
}