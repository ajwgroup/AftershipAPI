using AftershipAPI;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;

namespace AftershipAPITests
{
    [TestClass]
    public class ConnectionApiTests
    {
        [TestMethod]
        public void NewConnectionAPI_OnlyApiKeyArgument_NullApiKeyString_ThrowsArgumentError()
        {
            Action act = () => new ConnectionAPI(null);

            act.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void NewConnectionAPI_OnlyApiKeyArgument_WhiteSpaceApiKeyString_ThrowsArgumentError()
        {
            Action act = () => new ConnectionAPI(" ");

            act.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void NewConnectionAPI_OnlyApiKeyArgument_ApiKeyString_CreatesConnectionApi()
        {
            var result = new ConnectionAPI("key");

            result.Should().NotBeNull();
        }

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow(null, " ")]
        [DataRow(" ", null)]
        [DataRow(" ", " ")]
        public void NewConnectionAPI_KeyAndUrlArguments_EmptyString_ThrowsArgumentError(string? key, string? url)
        {
            Action act = () => new ConnectionAPI(key, url);

            act.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void NewConnectionAPI_ApiAndUrlArgument_ApiKeyString_CreatesConnectionApi()
        {
            var result = new ConnectionAPI("key", "url");

            result.Should().NotBeNull();
        }

        [TestMethod]
        public void CreateTracking()
        {
            var requestClient = new Mock<IRequestClient>();

            var tracking = new Tracking("tracking");

            var response = new Response()
            {
                Meta = new Meta() { Code = 200 },
                Data = new Data()
                {
                    Tracking = tracking
                }
            };

            string responseString = JsonConvert.SerializeObject(response);
            requestClient.Setup(x => x.RunRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(responseString);

            var connectionApi = new ConnectionAPI("", "", requestClient.Object);            

            var result = connectionApi.CreateTracking(tracking);

            requestClient.Verify(x => x.RunRequest("POST", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
