// -----------------------------------------------------------------------
//  <copyright file="TaskTriggers.cs" company="Global Graphics Software Ltd">
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
    public class TaskTriggers : GraphClientProviderBase
    {
        public static async Task Add(string printerId, string taskDefinitionId)
        {
            Console.WriteLine("Creating printer task trigger...");
            Console.WriteLine("Using delegated authorization. (Log-in as Universal Print Administrator.)");

            var delegatedClient = GetDelegatedClient();

            var printTaskTrigger = new PrintTaskTrigger
            {
                ODataType = null,
                Event = PrintEvent.JobStarted,
                AdditionalData = new Dictionary<string, object>
                {
                    {"definition@odata.bind", "https://graph.microsoft.com/1.0/print/taskDefinitions/" + taskDefinitionId}
                }
            };

            var trigger = await delegatedClient.Print.Printers[printerId].TaskTriggers.Request().AddAsync(printTaskTrigger);

            Console.WriteLine($"New task trigger ID: {trigger.Id}");
        }

        public static async Task Show(string printerId)
        {
            Console.WriteLine("Showing printer task triggers...");
            Console.WriteLine("Using delegated authorization. (Log-in as Universal Print Administrator.)");

            var delegatedClient = GetDelegatedClient();

            var triggers = await delegatedClient.Print.Printers[printerId].TaskTriggers.Request().GetAsync();

            foreach (var trigger in triggers)
            {
                Console.WriteLine($"Trigger: {trigger.Id},  {trigger.Event}");
            }
        }

        public static async Task Delete(string printerId, string taskTriggerDefinitionId)
        {
            Console.WriteLine("Deleting printer task trigger...");
            Console.WriteLine("Using delegated authorization. (Log-in as Universal Print Administrator.)");

            var delegatedClient = GetDelegatedClient();

            await delegatedClient.Print.Printers[printerId].TaskTriggers[taskTriggerDefinitionId].Request().DeleteAsync();

            Console.WriteLine("Done.");
        }
    }
}