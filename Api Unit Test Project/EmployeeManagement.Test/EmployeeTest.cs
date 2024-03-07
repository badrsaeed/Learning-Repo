using EmployeeManagement.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Test
{
    public class EmployeeTest
    {
        [Fact]
        public void CreateEmplyee_InputFirstNameAndLastName_FullNameConcatenation()
        {
            //Arrange
            var employee = new InternalEmployee("Test", "Test", 0, 2500,false, 1);
            //Act
            employee.FirstName = "Badr";
            employee.LastName = "Saeed";
            //Asset
            Assert.Equal("Badr Saeed", employee.FullName);
        }
        [Fact]
        public void CreateEmplyee_InputFirstNameAndLastName_FullNameStartWith()
        {
            //Arrange
            var employee = new InternalEmployee("Test", "Test", 0, 2500, false, 1);
            //Act
            employee.FirstName = "Badr";
            employee.LastName = "Saeed";
            //Asset
            Assert.StartsWith("Ba", employee.FullName);
        }
        [Fact]
        public void CreateEmplyee_InputFirstNameAndLastName_FullNameEndWith()
        {
            //Arrange
            var employee = new InternalEmployee("Test", "Test", 0, 2500, false, 1);
            //Act
            employee.FirstName = "Badr";
            employee.LastName = "Saeed";
            //Asset
            Assert.EndsWith("ed", employee.FullName);
        }
        [Fact]
        public void CreateEmplyee_InputFirstNameAndLastName_FullNameSounds()
        {
            //Arrange
            var employee = new InternalEmployee("Test", "Test", 0, 2500, false, 1);
            //Act
            employee.FirstName = "Badr";
            employee.LastName = "Saaid";
            //Asset
            Assert.Matches("Badr Sa(3|ee|ai)d", employee.FullName);
        }
    }
}
