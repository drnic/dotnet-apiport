﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Ocelot.Middleware;
using System;
using System.Threading.Tasks;

namespace PortabilityService.Gateway.Middleware
{
    /// <summary>
    /// This custom Ocelot middleware propagates content headers to downstream requests to workaround
    /// https://github.com/ThreeMammals/Ocelot/issues/263
    /// </summary>
    public class ContentHeaderForwardingMiddleware
    {
        // Ocelot currently only forwards Content-Type and request headers (as opposed to content headers)
        // https://github.com/dotnet/corefx/blob/master/src/System.Net.Http/src/System/Net/Http/Headers/KnownHeaders.cs
        // These are common content headers that we may need to forward manually.
        static readonly string[] ContentHeaders = new string[] { "Content-Encoding", "Content-Language" };

        public static async Task ForwardContentHeaders(DownstreamContext context, Func<Task> next)
        {
            foreach (var headerKey in ContentHeaders)
            {
                if (context.HttpContext.Request.Headers.ContainsKey(headerKey))
                {
                    var values = context.HttpContext.Request.Headers[headerKey];
                    context.DownstreamRequest.Content.Headers.TryAddWithoutValidation(headerKey, values.ToArray());
                }
            }

            await next();
        }
    }
}