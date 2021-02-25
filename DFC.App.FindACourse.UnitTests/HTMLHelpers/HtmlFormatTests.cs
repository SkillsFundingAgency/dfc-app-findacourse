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
            //Setup
            var fakeHelper = A.Fake<IHtmlHelper>();

            //Act
            var output = HtmlHelpers.HtmlFormat(fakeHelper, input);

            //Asserts
            output.ToString().Should().Be(expectedOutput);
        }

        [Fact]
        public void FormatThrowsExceptionForNullParamValues()
        {
            //Setup
            var fakeHelper = A.Fake<IHtmlHelper>();

            Action act = () => HtmlHelpers.HtmlFormat(fakeHelper, null);

            //Assert
            act.Should().Throw<ArgumentNullException>();
        }

    }
}
