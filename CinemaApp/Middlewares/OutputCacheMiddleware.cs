using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CinemaApp.Middlewares
{
    public class OutputCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Dictionary<string, CachedResponse> _cache = new();
        private readonly TimeSpan _cacheDuration = TimeSpan.FromSeconds(10); // Cache süresi

        public OutputCacheMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var key = GenerateCacheKey(context.Request);

            // Cache'de veri olup olmadığını kontrol et
            if (_cache.TryGetValue(key, out var cachedResponse))
            {
                // Cache süresi dolmadıysa cache'den yanıtı döndür
                if (DateTime.UtcNow - cachedResponse.Timestamp < _cacheDuration)
                {
                    context.Response.ContentType = cachedResponse.ContentType;
                    context.Response.ContentLength = cachedResponse.Content.Length;
                    await context.Response.Body.WriteAsync(cachedResponse.Content);
                    return;
                }
                else
                {
                    // Cache süresi dolmuşsa cache'i temizle
                    _cache.Remove(key);
                }
            }

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            context.Response.Body = originalBodyStream;
            var responseContent = responseBody.ToArray();
            _cache[key] = new CachedResponse
            {
                ContentType = context.Response.ContentType,
                Content = responseContent,
                Timestamp = DateTime.UtcNow
            };

            await originalBodyStream.WriteAsync(responseContent);
        }

        private string GenerateCacheKey(HttpRequest request)
        {
            return $"{request.Method}:{request.Path}:{request.QueryString}";
        }

        private class CachedResponse
        {
            public string ContentType { get; set; }
            public byte[] Content { get; set; }
            public DateTime Timestamp { get; set; } 
        }
    }
}
