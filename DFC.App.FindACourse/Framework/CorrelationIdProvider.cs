using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.App.FindACourse.Framework
{
    [ExcludeFromCodeCoverage]
    public class CorrelationIdProvider : ICorrelationIdProvider
    {
        private const string CorrelationId = "CorrelationId";

        private readonly IHttpContextAccessor httpContextAccessor;

        public CorrelationIdProvider(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        string ICorrelationIdProvider.CorrelationId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string GetId()
        {
            var result = string.Empty;
            if (httpContextAccessor.HttpContext != null)
            {
                result = httpContextAccessor.HttpContext.Request.Headers[CorrelationId].FirstOrDefault();
            }

            return result;
        }
    }
}
