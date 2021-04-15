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

using System;
using System.Collections.Generic;
using JawsMako;

namespace Valkyrie.Redaction
{
    public static class Program
    {
        private static void Main()
        {
            using var mako = IJawsMako.create();
            IJawsMako.enableAllFeatures(mako);

            using var input = IPDFInput.create(mako);

            using var assembly = input.open(@"TestFiles\statement_sample_mako.pdf");

            using var document = assembly.getDocument(0);
            using var page = document.getPage();

            RedactPage(mako, page);

            using var pdfOutput = IPDFOutput.create(mako);
            pdfOutput.writeAssembly(assembly, "out.pdf");
        }

        public static void RedactPage(IJawsMako mako, IPage page)
        {
            var pageLayout = IPageLayout.create(mako.getFactory(), page.getContent());

            pageLayout.analyze(ePageAnalysis.ePAAll);

            var text = pageLayout.getPageTextAsSysString();

            var termsToRedact = FindRedactionTerms(text);

            foreach (var termToRedact in termsToRedact)
            {
                Console.WriteLine($"Finding occurrences of '{termToRedact}'...");

                using var textSearch = ITextSearch.create(mako.getFactory(), pageLayout);

                var hits = textSearch.search(termToRedact, true, true);

                foreach (var hit in hits.first.toArray())
                {
                    var hitRect = new FRect();

                    foreach (var point in hit.toArray())
                        hitRect.expandToPoint(point);

                    using var redaction = IRedactionAnnotation.create(mako, hitRect);
                    using var red = IDOMColor.createFromArray(mako.getFactory(), IDOMColorSpaceDeviceRGB.create(mako.getFactory()), 1, new[] { 1.0f, 0.0f, 0.0f });

                    redaction.setInteriorColor(red);

                    page.addAnnotation(redaction);
                }
            }

            using var transform = IRedactorTransform.create(mako);
            transform.transformPage(page);
        }

        private static IEnumerable<string> FindRedactionTerms(string text)
        {
            // Here, we'd send the text to our AI model, but instead,
            // we'll simply return some parts of the text we want to redact.

            return new[]
            {
                "Katie Smith 972 Lakeshore Ct San Jose, California (CA), 95126",
                "0000032956"
            };
        }
    }
}
