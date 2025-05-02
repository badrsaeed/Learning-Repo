using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Controllers;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;

namespace RunGrooper.Tests.Controller
{
    public class ClubControllerTests
    {

        private readonly IClubRepository _clubRepository;
        private readonly IPhotoService _photoService;
        private readonly ClubController _clubController;
        public ClubControllerTests()
        {
            //Dependancies
            _clubRepository = A.Fake<IClubRepository>();
            _photoService = A.Fake<IPhotoService>();

            _clubController = new ClubController(_clubRepository, _photoService);
        }

        [Fact]
        public void ClubController_Index_ReturnSuccess()
        {
            var page = 1;
            var pageSize = 6;
            var clubs = A.Fake<IEnumerable<Club>>();
            A.CallTo(() => _clubRepository.GetSliceAsync((page - 1) * pageSize, pageSize)).Returns(clubs);


            //Act
            var result = _clubController.Index(page, pageSize);


            //Assert

            result.Should().BeOfType<Task<IActionResult>?>();
        }

        [Fact]
        public void ClubController_Details_ReturnSuccess()
        {
            //Arrange
            var club = A.Fake<Club>();
            int id = 1;
            A.CallTo(() => _clubRepository.GetByIdAsync(id)).Returns(club);
            //Act
            var result = _clubController.DetailClub(id, null);

            //Assert

            result.Should()
                .BeOfType<Task<IActionResult>>();
        }
    }
}
