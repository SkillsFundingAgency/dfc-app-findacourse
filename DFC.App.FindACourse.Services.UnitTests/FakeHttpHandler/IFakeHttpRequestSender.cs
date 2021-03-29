using System.Net.Http;

namespace DFC.App.FindACourse.Services.UnitTests.FakeHttpHandler
{
    public interface IFakeHttpRequestSender
    {
        HttpResponseMessage Send(HttpRequestMessage request);
    }
}
