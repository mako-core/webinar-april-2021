// -----------------------------------------------------------------------
//  <copyright file="Tasks.cs" company="Global Graphics Software Ltd">
//      Copyright (c) 2020 Global Graphics Software Ltd. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Global Graphics Software Ltd. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using JawsMako;
using Microsoft.Graph;
using Valkyrie.UniversalPrint.Client;
using File = System.IO.File;

namespace Valkyrie.UniversalPrint.Graph
{
    public class Tasks : GraphClientProviderBase
    {
        public static async Task Poll(string taskDefinitionId, string redirectPrinterId)
        {
            using var mako = IJawsMako.create();

            Console.WriteLine("Polling for tasks...");

            var client = GetApplicationClient();

            while (true)
            {
                var tasks = await client.Print.TaskDefinitions[taskDefinitionId].Tasks.Request().Expand("definition").Filter("status/state eq 'processing'").GetAsync();
                if (!tasks.Any())
                {
                    Console.WriteLine("No tasks found.");
                    Thread.Sleep(TimeSpan.FromSeconds(5));

                    continue;
                }

                foreach (var task in tasks)
                {
                    var (printerId, jobId) = GetJobDetailsFromTask(task);

                    var job = await client.Print.Printers[printerId].Jobs[jobId].Request().Expand("documents").GetAsync();

                    if (job.Status.State != PrintJobProcessingState.Paused)
                    {
                        Console.WriteLine($"Found task without paused job: '{task.Id}'. Aborting...");
                        await AbortTask(taskDefinitionId, client, task);
                        continue;
                    }

                    // Downloading the document...

                    var printDocument = job.Documents.First();

                    Console.WriteLine($"Processing job document: {printDocument.DisplayName}, {printDocument.ContentType}");

                    var jobFile = new FileInfo("temp.oxps");

                    await using (var documentStream = await client.Print.Printers[printerId].Jobs[jobId].Documents[printDocument.Id].Content.Request().GetAsync())
                    {
                        Console.WriteLine("Downloading...");

                        await using var file = File.Create(jobFile.FullName);
                        await documentStream.CopyToAsync(file);
                    }

                    // Modifying the document...

                    Console.WriteLine("Converting...");

                    Formats.Convert.OpenXpsToXps(jobFile, new FileInfo("temp-input.xps"));

                    Console.WriteLine("Redacting...");

                    using var xpsInput = IXPSInput.create(mako);
                    using var assembly = xpsInput.open("temp-input.xps");
                    using var document = assembly.getDocument();
                    using var page = document.getPage();

                    Redaction.Program.RedactPage(mako, page);

                    using var xpsOutput = IXPSOutput.create(mako);
                    xpsOutput.writeAssembly(assembly, "temp-output.xps");

                    Console.WriteLine("Converting...");

                    Formats.Convert.XpsToOpenXps(new FileInfo("temp-output.xps"), jobFile);

                    // Uploading the document

                    Console.WriteLine("Uploading...");

                    long uploadSize;

                    await using (var modifiedDocumentStream = File.OpenRead(jobFile.FullName))
                        uploadSize = modifiedDocumentStream.Length;

                    var printDocumentUploadProperties = new PrintDocumentUploadProperties
                    {
                        ContentType = "application/oxps",
                        DocumentName = $"[Redacted] {printDocument.DisplayName}",
                        Size = uploadSize
                    };

                    var uploadSession = await client.Print.Printers[printerId].Jobs[jobId]
                        .Documents[printDocument.Id].CreateUploadSession(printDocumentUploadProperties).Request()
                        .PostAsync();

                    using var webClient = new WebClient();
                    webClient.Headers.Add("Content-Range", $"bytes 0-{uploadSize - 1}/{uploadSize}");
                    await webClient.UploadFileTaskAsync(uploadSession.UploadUrl, "PUT", jobFile.FullName);

                    Console.WriteLine("Redirecting...");

                    await client.Print.Printers[printerId].Jobs[jobId].Redirect(redirectPrinterId, GetJobConfig()).Request().PostAsync();
                }
            }
        }

        private static (string printerId, string jobId) GetJobDetailsFromTask(PrintTask task)
        {
            var matches = Regex.Match(task.ParentUrl, @".+\/print\/printers\/(.+)\/jobs\/(\d+)$");

            var printerId = matches.Groups[1].Value;
            var jobId = matches.Groups[2].Value;

            return (printerId, jobId);
        }

        private static PrintJobConfiguration GetJobConfig()
        {
            return new PrintJobConfiguration
            {
                ColorMode = PrintColorMode.Color,
                Copies = 1,
                Dpi = 600,
                DuplexMode = PrintDuplexMode.OneSided,
                Finishings = new[] {PrintFinishing.None},
                InputBin = "auto",
                OutputBin = "auto",
                MediaSize = "A4",
                MediaType = "stationery",
                Orientation = PrintOrientation.Portrait,
                PagesPerSheet = 1,
                Quality = PrintQuality.Medium,
                Collate = false,
                Margin = new PrintMargin
                {
                    Bottom=0,Left=0,Top=0,Right=0
                }
            };
        }

        private static async Task CompleteTask(string taskDefinitionId, GraphServiceClient client, PrintTask task)
        {
            Console.WriteLine($"Completing task: '{task.Id}'...");

            await client.Print.TaskDefinitions[taskDefinitionId].Tasks[task.Id].Request()
                .UpdateAsync(new PrintTask { Status = new PrintTaskStatus { State = PrintTaskProcessingState.Aborted } });
        }

        private static async Task AbortTask(string taskDefinitionId, GraphServiceClient client, PrintTask task)
        {
            Console.WriteLine($"Aborting task: '{task.Id}'...");

            await client.Print.TaskDefinitions[taskDefinitionId].Tasks[task.Id].Request()
                .UpdateAsync(new PrintTask { Status = new PrintTaskStatus { State = PrintTaskProcessingState.Aborted } });
        }
    }
}