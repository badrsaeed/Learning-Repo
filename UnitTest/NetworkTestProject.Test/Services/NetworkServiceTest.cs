using FakeItEasy;
using FluentAssertions;
using System.Net.NetworkInformation;
using TestApp;
using TestApp.Services;

namespace NetworkTestProject.Test.Services
{
    public class NetworkServiceTest
    {
        private readonly NetworkService _networkService;
        private readonly IDNSService _dns;
        public NetworkServiceTest()
        {
            _dns = A.Fake<IDNSService>();
            _networkService = new NetworkService(_dns);
        }

        [Fact]
        public void GetNetworkName_NoAction_ReturnStringRepresentNetworkName()
        {
            //Arrage

            //Act
            var result = _networkService.GetNetworkName();
            //Assert
            result.Should().Be("Badr");
        }

        [Fact]
        public void Ping_ByIpAndPortNumber_ReturnOneIfSuccess()
        {
            //Arrage
            string ip = "1203";
            int port = 25;
            //Act
            var result = _networkService.Ping(ip, port);
            //Assert
            result.Should().Be(1);
            result.Should().BeGreaterThanOrEqualTo(1);
            result.Should().NotBe(-1);
        }
        [Fact]
        public void Ping_ByInvalidIpAndPortNumber_ReturnNegativeOne()
        {
            //Arrage
            string ip = "123";
            int port = 25;
            //Act
            var result = _networkService.Ping(ip, port);
            //Assert
            result.Should().Be(-1);
            //result.Should().BeGreaterThanOrEqualTo();
            result.Should().NotBe(1);
            //result.Should().
        }

        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(2, 3, 5)]
        [InlineData(3, 4, 7)]
        public void CalculateTimeOur_ByTwoNumbers_ReturnSumAsTimeOut(int first, int seconed, int sum)
        {
            //Arrage

            //Act
            var result = _networkService.CalculateTimeOut(first, seconed);
            //Assert
            result.Should().Be(sum);
            //result.Should().BeGreaterThanOrEqualTo();
            result.Should().BeGreaterThan(first);
            result.Should().BeGreaterThan(seconed);

        }

        [Fact]
        public void GetLastTest_ReturnDateTimeNow()
        {
            //Arrange

            //Act
            var result = _networkService.GetLastTestDateTime();
            //Assert
            result.Should().BeAfter(DateTime.Now.AddDays(-1));
            result.Should().BeBefore(DateTime.Now.AddDays(1));
        }

        [Fact]
        public void PingOptions_GetOptions_ReturnOptions()
        {
            //Arrange
            PingOptions options = new PingOptions
            {
                DontFragment = true,
                Ttl = 1
            };
            //Act
            var result = _networkService.GetPingOptions();
            //Assert

            result.Should().BeEquivalentTo(options);
            result.Should().BeOfType<PingOptions>();
            //result.Should().be
        }

        [Fact]
        public void GetPingTryies_Tryies_ReturnNumberFrom0To99()
        {
            //Arrange
            var expected = Enumerable.Range(0, 100).ToList();
            //Act
            var result = _networkService.GetAllPingTryies().ToList();
            //Assert
            result.Should().BeOfType<List<int>?>();
            result.Should().Contain(x => x == expected[0]);
            result.Should().NotBeEmpty()
                .And.HaveCount(expected.Count)
                .And.ContainInOrder(Enumerable.Range(0, 100).ToList())
                .And.HaveCountLessThan(150)
                .And.HaveSameCount(expected);




            //result.Should().Contain(expected[5]);
        }

        [Fact]
        public void GetPingOptionList_ReturnListOfOptions()
        {
            //Arrange
            var options = new PingOptions { DontFragment = true, Ttl = 1 };
            //Act
            var result = _networkService.GetPingOptionsList().ToList();
            //Assert
            result.Should().ContainEquivalentOf(options);
            result.Should().BeOfType<List<PingOptions>?>();
            result.Should().Contain(r => r.DontFragment == true && r.Ttl == 1);
        }

        [Fact]
        public void SendTestPing_ReturnSuccess()
        {
            //Arrange
            A.CallTo(() => _dns.SendPing()).Returns(true);
            //Act
            var result = _networkService.SendTestPing();

            //Assert
            result.Should().NotBeNullOrEmpty()
                .And.Be("Successfull Send")
                .And.Contain("Successfull ", Exactly.Once());

        }
    }
}
