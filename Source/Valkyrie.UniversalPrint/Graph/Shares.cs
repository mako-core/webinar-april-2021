// -----------------------------------------------------------------------
//  <copyright file="Shares.cs" company="Global Graphics Software Ltd">
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
using Microsoft.Graph;
using Valkyrie.UniversalPrint.Client;

namespace Valkyrie.UniversalPrint.Graph
{
    public class Shares : GraphClientProviderBase
    {
        public static async Task Add(string name, string printerId)
        {
            Console.WriteLine("Adding new printer share...");
            Console.WriteLine("Using delegated authorization. (Log-in as Universal Print Administrator.)");

            var client = GetDelegatedClient();
            var printerShare = new PrinterShare
            {
                DisplayName = name, 
                IsAcceptingJobs = true,
                AdditionalData = new Dictionary<string, object>()
                {
                    {"printer@odata.bind", $"https://graph.microsoft.com/1.0/print/printers/{printerId}"}
                }
            };

            await client.Print.Shares.Request().AddAsync(printerShare);

            Console.WriteLine("Done.");
        }

        public static async Task Show()
        {
            Console.WriteLine("Getting printer shares...");
            Console.WriteLine("Using delegated authorization. (Log-in as Universal Print Administrator.)");

            var client = GetDelegatedClient();

            var shares = await client.Print.Shares.Request().GetAsync();

            foreach (var share in shares)
            {
                var shareObject = await client.Print.Shares[share.Id].Request().Expand("printer").GetAsync();
                Console.WriteLine($"Printer share '{shareObject.DisplayName}': {shareObject.Id}, printer '{shareObject.Printer?.DisplayName}' ({shareObject.Printer?.Id})");
            }

            Console.WriteLine("Done.");
        }

        public static async Task Delete(string shareId)
        {
            Console.WriteLine("Deleting printer share...");
            Console.WriteLine("Using delegated authorization. (Log-in as Universal Print Administrator.)");

            var client = GetDelegatedClient();

            await client.Print.Shares[shareId].Request().DeleteAsync();

            Console.WriteLine("Done.");
        }
    }
}