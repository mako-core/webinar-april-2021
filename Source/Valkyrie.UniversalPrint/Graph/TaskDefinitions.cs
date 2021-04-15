// -----------------------------------------------------------------------
//  <copyright file="TaskDefinitions.cs" company="Global Graphics Software Ltd">
//      Copyright (c) 2020 Global Graphics Software Ltd. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Global Graphics Software Ltd. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.Graph;
using Valkyrie.UniversalPrint.Client;

namespace Valkyrie.UniversalPrint.Graph
{
    public class TaskDefinitions : GraphClientProviderBase
    {
        public static async Task Show()
        {
            Console.WriteLine("Getting printer task definitions...");
            Console.WriteLine("Using application authorization.");

            var daemonClient = GetApplicationClient();

            var definitions = await daemonClient.Print.TaskDefinitions.Request().Expand("tasks").GetAsync();
            foreach (var currentDefinition in definitions)
            {
                Console.WriteLine($"Found task definition: {currentDefinition.DisplayName} ({currentDefinition.Id})");
            }
        }

        public static async Task Delete(string taskDefinitionId)
        {
            Console.WriteLine("Deleting printer task definition...");
            Console.WriteLine("Using application authorization.");

            var daemonClient = GetApplicationClient();

            await daemonClient.Print.TaskDefinitions[taskDefinitionId].Request().DeleteAsync();
            
            Console.WriteLine("Done.");
        }

        public static async Task Add(string name, string createdByName)
        {
            Console.WriteLine("Creating printer task definition...");
            Console.WriteLine("Using application authorization.");

            var daemonClient = GetApplicationClient();

            var printTaskDefinition = new PrintTaskDefinition
            {
                ODataType = null,
                DisplayName = name,
                CreatedBy = new AppIdentity
                {
                    DisplayName = createdByName,
                    ODataType = null
                }
            };

            var definition = await daemonClient.Print.TaskDefinitions.Request().AddAsync(printTaskDefinition);

            Console.WriteLine($"Task definition ID: {definition.Id}");
        }
    }
}