// -----------------------------------------------------------------------
//  <copyright file="AccessTokens.cs" company="Global Graphics Software Ltd">
//      Copyright (c) 2020 Global Graphics Software Ltd. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Global Graphics Software Ltd. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Valkyrie.UniversalPrint.Client;

namespace Valkyrie.UniversalPrint.Graph
{
    public class AccessTokens : GraphClientProviderBase
    {
        public static async Task ShowDelegated()
        {
            var client = GetDelegatedApplicationBuilder();
            var result = await client.AcquireTokenInteractive(new List<string>()).ExecuteAsync();

            Console.WriteLine("Access token:");
            Console.WriteLine(result.AccessToken);
        }

        public static async Task ShowApplication()
        {
            var client = GetApplicationAppBuilder();
            var result = await client.AcquireTokenForClient(new List<string>()).ExecuteAsync();

            Console.WriteLine("Access token:");
            Console.WriteLine(result.AccessToken);
        }
    }
}