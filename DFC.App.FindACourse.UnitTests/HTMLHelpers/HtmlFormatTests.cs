using DFC.App.FindACourse.Helpers;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;


namespace DFC.App.FindACourse.UnitTests
{
    public class HtmlFormatTests
    {
        [Theory]
        [InlineData ("aa\r\nbb", "aa<br/>bb")]
        [InlineData("aa\r\n \r\nbb", "aa<br/> <br/>bb")]
        public void HtmlFormatsCorrectly(string input, string expectedOutput)
        {
            //Act
            var fakeHelper = A.Fake<IHtmlHelper>();

            var output = HTMLHelpers.HtmlFormat(fakeHelper, input);

            //Asserts
            output.ToString().Should().Be(expectedOutput);
        }

    }
}
