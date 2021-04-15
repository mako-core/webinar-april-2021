// -----------------------------------------------------------------------
//  <copyright file="GraphExtensionMethods.cs" company="Global Graphics Software Ltd">
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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using Newtonsoft.Json;

namespace Valkyrie.UniversalPrint.Extensions
{
    public static class GraphExtensionMethods
    {
        public static async Task<T> AddAsyncEx<T>(this IBaseRequest request, T serializableObject)
        {
            request.ContentType = "application/json";

            request.GetType().GetProperty(nameof(request.Method))?
                .SetValue(request, "POST");

            if (string.IsNullOrEmpty(request.RequestUrl))
                throw new InvalidOperationException("Request URL cannot be null.");

            using var requestMessage = request.GetHttpRequestMessage();
            await request.Client.AuthenticationProvider.AuthenticateRequestAsync(requestMessage);

            if (serializableObject != null)
            {
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(serializableObject));
                if (!string.IsNullOrEmpty(request.ContentType))
                    requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(request.ContentType);
            }
            var responseMessage = await request.Client.HttpProvider.SendAsync(requestMessage).ConfigureAwait(false);

            var responseHandler = new ResponseHandler(request.Client.HttpProvider.Serializer);

            return await responseHandler.HandleResponse<T>(responseMessage);
        }

        public static async Task<HttpStatusCode> UpdateAsync(this IBaseRequest request, Stream stream, MediaTypeHeaderValue contentType)
        {
            request.ContentType = "application/ipp";

            request.GetType().GetProperty(nameof(request.Method))?
                .SetValue(request, "PATCH");

            if (string.IsNullOrEmpty(request.RequestUrl))
                throw new InvalidOperationException("Request URL cannot be null or empty.");

            using var requestMessage = request.GetHttpRequestMessage();
            await request.Client.AuthenticationProvider.AuthenticateRequestAsync(requestMessage);

            requestMessage.Content = new StreamContent(stream);
            requestMessage.Content.Headers.ContentType = contentType;

            var responseMessage = await request.Client.HttpProvider.SendAsync(requestMessage).ConfigureAwait(false);

            return responseMessage.StatusCode;
        }
    }
}