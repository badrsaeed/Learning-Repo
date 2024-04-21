using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Test.Fixture
{
    public class EmployeeServiceFixtureWithDI : IClassFixture<EmployeeServiceWithDIFixture>
    {
        private readonly EmployeeServiceWithDIFixture _employeeServiceFixtureWithDI;

        public EmployeeServiceFixtureWithDI(
            EmployeeServiceWithDIFixture employeeServiceFixtureWithDI)
        {
            _employeeServiceFixtureWithDI = employeeServiceFixtureWithDI;
        }
        [Fact]
        public void CreateInteralEmployee_InternalEmployeeCreated_MustHaveAttendedCourses()
        {
            //Arrange
            var _oboligtoryCourses = _employeeServiceFixtureWithDI.EmployeeManagementRepository.GetCourse(Guid.Parse(""));

            //Act
            var internalEmp = _employeeServiceFixtureWithDI
                .EmployeeService.CreateInternalEmployee("Badr", "Saeed");

            //Assert
            Assert.Contains(_oboligtoryCourses, internalEmp.AttendedCourses);
        }
    }
}
