using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Test.Fixture
{
    public class EmployeeServiceFixture : IDisposable
    {
        
        //making the prop is private and read only make the resource cant added a new instance to it
        public IEmployeeManagementRepository EmployeeManagementRepository { get;  }
        public EmployeeService EmployeeService { get;  }

        public EmployeeServiceFixture()
        {
            //for testing 

            //EmployeeManagementRepository = new EmployeeManagementRepository();
            //EmployeeService = new EmployeeService();
        }
        public void Dispose()
        {
            //to Clear all setup code, if required
        }
    }
}
