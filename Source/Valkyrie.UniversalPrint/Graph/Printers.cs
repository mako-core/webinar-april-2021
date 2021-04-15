// -----------------------------------------------------------------------
//  <copyright file="Printers.cs" company="Global Graphics Software Ltd">
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
using System.IO;
using System.Threading.Tasks;
using Microsoft.Graph;
using Newtonsoft.Json;
using Valkyrie.UniversalPrint.Client;

namespace Valkyrie.UniversalPrint.Graph
{
    public class Printers : GraphClientProviderBase
    {
        public static async Task ReadDefaults(string printerId, FileInfo output)
        {
            Console.WriteLine("Reading defaults...");
            Console.WriteLine("Using application authorization.");

            var delegatedClient = GetApplicationClient();

            var printer = await delegatedClient.Print.Printers[printerId].Request().Select("defaults").GetAsync();

            var json = JsonConvert.SerializeObject(printer.Defaults);

            await System.IO.File.WriteAllTextAsync(output.FullName, json);
        }

        public static async Task ReadCapabilities(string printerId, FileInfo output)
        {
            Console.WriteLine("Reading capabilities...");
            Console.WriteLine("Using application authorization.");

            var delegatedClient = GetApplicationClient();

            var printer = await delegatedClient.Print.Printers[printerId].Request().Select("capabilities").GetAsync();

            var json = JsonConvert.SerializeObject(printer.Capabilities);

            await System.IO.File.WriteAllTextAsync(output.FullName, json);
        }

        public static async Task WriteDefaults(string printerId, FileInfo input)
        {
            Console.WriteLine("Writing defaults...");
            Console.WriteLine("Using application authorization.");

            var delegatedClient = GetApplicationClient();

            var json = await System.IO.File.ReadAllTextAsync(input.FullName);
            var defaults = JsonConvert.DeserializeObject<PrinterDefaults>(json);

            var printer = new Printer
            {
                Defaults = defaults
            };

            await delegatedClient.Print.Printers[printerId].Request().UpdateAsync(printer);
        }

        public static async Task WriteCapabilities(string printerId, FileInfo input)
        {
            Console.WriteLine("Writing capabilities...");
            Console.WriteLine("Using application authorization.");

            var delegatedClient = GetApplicationClient();

            var json = await System.IO.File.ReadAllTextAsync(input.FullName);
            var capabilities = JsonConvert.DeserializeObject<PrinterCapabilities>(json);

            var printer = new Printer
            {
                Capabilities = capabilities
            };

            await delegatedClient.Print.Printers[printerId].Request().UpdateAsync(printer);
        }

        public static async Task Update(string printerId)
        {
            Console.WriteLine("Updating printer...");
            Console.WriteLine("Using application authorization.");

            var delegatedClient = GetApplicationClient();

            var printer = new Printer
            {
                DisplayName = "Mako Virtual Printer",
                IsAcceptingJobs = true,
                Status = new PrinterStatus
                {
                    Description = string.Empty,
                    State = PrinterProcessingState.Idle,
                    Details = new List<PrinterProcessingStateDetail> { PrinterProcessingStateDetail.None }
                },
                Defaults = new PrinterDefaults
                {
                    CopiesPerJob = 1,
                    ContentType = "application/pdf",
                    Finishings = new[] { PrintFinishing.None },
                    MediaType = "stationary",
                    MediaSize = "A4",
                    PagesPerSheet = 1,
                    Orientation = PrintOrientation.Portrait,
                    InputBin = "auto",
                    OutputBin = "auto",
                    ColorMode = PrintColorMode.Auto,
                    Quality = PrintQuality.High,
                    DuplexMode = PrintDuplexMode.OneSided,
                    Dpi = 600
                },
                Capabilities = new PrinterCapabilities
                {
                    CopiesPerJob = new IntegerRange { Start = 1, End = 1 },
                    ContentTypes = new List<string>
                    {
                        "application/pdf"
                    },
                    Finishings = new[] { PrintFinishing.None },
                    MediaTypes = new[] { "stationary" },
                    MediaSizes = new[] { "A4" },
                    PagesPerSheet = new []{1},
                    Orientations = new [] {PrintOrientation.Landscape, PrintOrientation.Portrait},
                    InputBins = new []{"auto"},
                    OutputBins = new []{"auto"},
                    ColorModes = new [] {PrintColorMode.Auto},
                    Qualities = new [] {PrintQuality.High},
                    DuplexModes = new [] {PrintDuplexMode.OneSided},
                    Dpis = new []{600}
                }
            };

            await delegatedClient.Print.Printers[printerId].Request().UpdateAsync(printer);

            Console.WriteLine("Updated printer.");
        }

        public static async Task Delete(string printerId)
        {
            Console.WriteLine("Deleting printer...");
            Console.WriteLine("Using delegated authorization. (Log-in as Universal Print Administrator.)");

            var delegatedClient = GetDelegatedClient();

            await delegatedClient.Print.Printers[printerId].Request().DeleteAsync();

            Console.WriteLine("Deleted printer.");
        }

        public static async Task Show()
        {
            Console.WriteLine("Showing printers...");
            Console.WriteLine("Using delegated authorization. (Log-in as Universal Print Administrator.)");

            var delegatedClient = GetDelegatedClient();

            var printers = await delegatedClient.Print.Printers.Request().GetAsync();

            foreach (var printer in printers)
            {
                Console.WriteLine($"Printer: {printer.DisplayName} - ({printer.Id})" +
                                  $", {(printer.IsShared.HasValue && printer.IsShared.Value ? "Shared" : "Not Shared")}" +
                                  $", {(printer.IsAcceptingJobs.HasValue && printer.IsAcceptingJobs.Value ? "Accepting Jobs" : "Not Accepting Jobs")}" +
                                  $", {printer.Status?.Description}");
            }
        }

        public static async Task Add(string name, string model, string manufacturer)
        {
            Console.WriteLine("Creating printer...");
            Console.WriteLine("Using delegated authorization. (Log-in as Universal Print Administrator.)");

            var delegatedClient = GetDelegatedClient();

            Console.WriteLine("Creating signing request");

            var (csrData, transportKey) = Encryption.CreateCsr();

            var certRequest = new PrintCertificateSigningRequestObject
            {
                Content = csrData,
                TransportKey = transportKey
            };

            Console.WriteLine("Adding printer...");

            var request = delegatedClient.Print.Printers.Create(name, manufacturer, model, certRequest, null, false).Request();
            await request.PostAsync();

            Console.WriteLine("Done. (May take a minute to show in Azure Portal.)");
        }
    }
}