// -----------------------------------------------------------------------
//  <copyright file="Program.cs" company="Global Graphics Software Ltd">
//      Copyright (c) 2020 Global Graphics Software Ltd. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Global Graphics Software Ltd. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using Valkyrie.UniversalPrint.Graph;
using Command = System.CommandLine.Command;

namespace Valkyrie.UniversalPrint
{
    internal static class Program
    {
        static async Task<int> Main(string[] args)
        {
            var addPrinterCommand = new Command("add")
            {
                new Argument<string>("name", "Name of the printer."),
                new Argument<string>("model", "Model of the printer."),
                new Argument<string>("manufacturer", "Manufacturer of the printer.")
            };
            addPrinterCommand.Handler = CommandHandler.Create<string, string, string>(Printers.Add);

            var showPrintersCommand = new Command("show")
            {
                Handler = CommandHandler.Create(Printers.Show)
            };

            var deletePrinterCommand = new Command("delete")
            {
                new Argument<string>("printerId", "Printer id.")
            };
            deletePrinterCommand.Handler = CommandHandler.Create<string>(Printers.Delete);

            var updatePrintersCommand = new Command("update")
            {
                new Argument<string>("printerId", "Printer id.")
            };
            updatePrintersCommand.Handler = CommandHandler.Create<string>(Printers.Update);

            var readDefaultsPrinterCommand = new Command("read-defaults")
            {
                new Argument<string>("printerId", "Printer id."),
                new Argument<string>("output", "Path to output file.")
            };
            readDefaultsPrinterCommand.Handler = CommandHandler.Create<string, FileInfo>(Printers.ReadDefaults);

            var readCapabilitiesPrinterCommand = new Command("read-capabilities")
            {
                new Argument<string>("printerId", "Printer id."),
                new Argument<string>("output", "Path to output file.")
            };
            readCapabilitiesPrinterCommand.Handler = CommandHandler.Create<string, FileInfo>(Printers.ReadCapabilities);

            var writeDefaultsPrinterCommand = new Command("write-defaults")
            {
                new Argument<string>("printerId", "Printer id."),
                new Argument<string>("input", "Path to input file.")
            };
            writeDefaultsPrinterCommand.Handler = CommandHandler.Create<string, FileInfo>(Printers.WriteDefaults);

            var writeCapabilitiesPrinterCommand = new Command("write-capabilities")
            {
                new Argument<string>("printerId", "Printer id."),
                new Argument<string>("input", "Path to input file.")
            };
            writeCapabilitiesPrinterCommand.Handler = CommandHandler.Create<string, FileInfo>(Printers.WriteCapabilities);

            var addShareCommand = new Command("add")
            {
                new Argument<string>("name", "Name of the share."),
                new Argument<string>("printerId", "Id of the printer to share."),
            };
            addShareCommand.Handler = CommandHandler.Create<string, string>(Shares.Add);

            var showSharesCommand = new Command("show")
            {
                Handler = CommandHandler.Create(Shares.Show)
            };

            var deleteShareCommand = new Command("delete")
            {
                new Argument<string>("shareId", "Share id.")
            };
            deleteShareCommand.Handler = CommandHandler.Create<string>(Shares.Delete);

            var showDelegatedAccessTokenCommand = new Command("show")
            {
                Handler = CommandHandler.Create(AccessTokens.ShowDelegated)
            };

            var showApplicationAccessTokenCommand = new Command("show")
            {
                Handler = CommandHandler.Create(AccessTokens.ShowApplication)
            };

            var showTaskDefinitionsCommand = new Command("show")
            {
                Handler = CommandHandler.Create(TaskDefinitions.Show)
            }; 
            
            var deleteTaskDefinitionsCommand = new Command("delete")
            {
                new Argument<string>("taskDefinitionId", "Task definition id.")
            };
            deleteTaskDefinitionsCommand.Handler = CommandHandler.Create<string>(TaskDefinitions.Delete);

            var addTaskDefinitionsCommand = new Command("add")
            {
                new Argument<string>("name", "Task definition id."),
                new Argument<string>("createdByName", "Name of the application creating the task definition.")
            };
            addTaskDefinitionsCommand.Handler = CommandHandler.Create<string, string>(TaskDefinitions.Add);

            var deleteTaskTriggerCommand = new Command("delete")
            {
                new Argument<string>("printerId", "Printer id."),
                new Argument<string>("taskTriggerDefinitionId", "Task trigger definition id.")
            };
            deleteTaskTriggerCommand.Handler = CommandHandler.Create<string,string>(TaskTriggers.Delete);

            var addTaskTriggerCommand = new Command("add")
            {
                new Argument<string>("printerId", "Printer id."),
                new Argument<string>("taskDefinitionId", "Task definition id.")
            };
            addTaskTriggerCommand.Handler = CommandHandler.Create<string, string>(TaskTriggers.Add);

            var showTaskTriggersCommand = new Command("show")
            {
                new Argument<string>("printerId", "Printer id.")
            };
            showTaskTriggersCommand.Handler = CommandHandler.Create<string>(TaskTriggers.Show);

            var tasksPollCommand = new Command("poll")
            {
                new Argument<string>("taskDefinitionId", "Task definition id."),
                new Argument<string>("redirectPrinterId", "The ID of the printer to redirect to."),
            };
            tasksPollCommand.Handler = CommandHandler.Create<string, string>(Tasks.Poll);

            var rootCommand = new RootCommand
            {
                new Command("printers")
                {
                    addPrinterCommand,
                    showPrintersCommand,
                    deletePrinterCommand,
                    updatePrintersCommand,
                    readCapabilitiesPrinterCommand,
                    readDefaultsPrinterCommand,
                    writeDefaultsPrinterCommand,
                    writeCapabilitiesPrinterCommand,
                },
                new Command("shares")
                {
                    addShareCommand,
                    showSharesCommand,
                    deleteShareCommand
                },
                new Command("task-definitions")
                {
                    addTaskDefinitionsCommand,
                    showTaskDefinitionsCommand,
                    deleteTaskDefinitionsCommand
                },
                new Command("task-trigger-definitions")
                {
                    addTaskTriggerCommand,
                    showTaskTriggersCommand,
                    deleteTaskTriggerCommand
                },
                new Command("tasks")
                {
                    tasksPollCommand
                },
                new Command("delegated-access-token")
                {
                    showDelegatedAccessTokenCommand
                },
                new Command("application-access-token")
                {
                    showApplicationAccessTokenCommand
                }
            };

            return
                await rootCommand.InvokeAsync(args);
        }
    }
}
