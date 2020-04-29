using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Framework
{
    public class CorrelationIdProvider : ICorrelationIdProvider
    {
        private const string CorrelationId = "DssCorrelationId";

        private readonly IHttpContextAccessor httpContextAccessor;

        string ICorrelationIdProvider.CorrelationId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public CorrelationIdProvider(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

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
