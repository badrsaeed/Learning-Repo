using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Data.Enum;
using RunGroopWebApp.Models;
using RunGroopWebApp.Repository;

namespace RunGrooper.Tests.Repository
{
    public class ClubReposirotyTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ClubRepository _clubRepository;
        public ClubReposirotyTests()
        {
            _dbContext = GetContext().Result;
            _clubRepository = new ClubRepository(_dbContext);
        }

        [Fact]
        public async Task ClubRepository_AddClub_ReturnTrue()
        {
            //Arrange
            var club = new Club
            {
                Address = new Address
                {
                    City = "1",
                    State = "1",
                    Street = "1",
                    ZipCode = 1
                },
                Title = "1",
                Image = null,
                ClubCategory = ClubCategory.City,
                Description = "1",
                AppUser = null,

            };

            //Act
            var result = _clubRepository.Add(club);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ClubRepository_GetById_ReturnsClub()
        {
            //Arrange
            int id = 1;

            //Act
            var result = _clubRepository.GetByIdAsync(id);

            //Assert
            result.Should().BeOfType(typeof(Task<Club>));
        }


        private async Task<ApplicationDbContext> GetContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(new Guid().ToString())
                .Options;

            var databaseContext = new ApplicationDbContext(options);

            databaseContext.Database.EnsureCreated();

            if (await databaseContext.Clubs.CountAsync() < 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    databaseContext.Clubs.Add(
                        new Club()
                        {
                            Address = new Address
                            {
                                City = i.ToString(),
                                State = i.ToString(),
                                Street = i.ToString(),
                                ZipCode = i
                            },
                            Title = i.ToString(),
                            Image = null,
                            ClubCategory = ClubCategory.City,
                            Description = i.ToString(),
                            AppUser = null,

                        });
                }
                await databaseContext.SaveChangesAsync();
            }

            return databaseContext;
        }
    }
}
