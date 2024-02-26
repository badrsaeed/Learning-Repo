using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Test
{
    public class EmployeeFactoryTest
    {
        [Fact]
        public void CreateEmployee_ConstructInternalEmployee_SalaryMustBe2500()
        {
            //Arrange
            var employeeFactory = new EmployeeFactory();

            //Act
            var expectedResult = 
                (InternalEmployee) employeeFactory.CreateEmployee("Badr", "Saeed");

            //Assert
            Assert.Equal(2500, expectedResult.Salary);

        }
    }
}
