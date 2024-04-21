using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Test
{
    public class EmployeeFactoryTest : IDisposable
    {
        private EmployeeFactory _employeeFactory;
        public EmployeeFactoryTest()
        {
            _employeeFactory = new EmployeeFactory();   
        }
        public void Dispose()
        {
            //nothing to cleare here.
        }
        [Fact]
        public void CreateEmployee_ConstructInternalEmployee_SalaryMustBe2500()
        {
            //Arrange
            //nothing to arrange here
            //Act
            var expectedResult = 
                (InternalEmployee) _employeeFactory.CreateEmployee("Badr", "Saeed");

            //Assert
            Assert.Equal(2500, expectedResult.Salary);

        }
        [Fact]
        public void CreateEmployee_ConstructInternalEmployee_SalaryMustBeLessThan3000()
        {
            //Arrange
            //nothing to arrange here
            //Act
            var expectedResult =
                (InternalEmployee)_employeeFactory.CreateEmployee("Badr", "Saeed");

            //Assert
            Assert.True(expectedResult.Salary < 3000);

        }


    }
}
