using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Test.Fixture
{
    public class EmployeeServiceWithDIFixture
    {
        private readonly ServiceProvider _serviceProvider;

        public IEmployeeManagementRepository EmployeeManagementRepository
        {
            get
            { 
                return _serviceProvider.GetRequiredService<IEmployeeManagementRepository>();
            }
        }

        public IEmployeeService EmployeeService
        {
            get 
            {
                return _serviceProvider.GetRequiredService<IEmployeeService>();
            }
        }

        public EmployeeServiceWithDIFixture()
        {
            var service = new ServiceCollection();
            service.AddScoped<IEmployeeService, EmployeeService>();
            service.AddScoped<IEmployeeManagementRepository, EmployeeManagementRepository>();
            service.AddScoped<EmployeeFactory>();

            _serviceProvider = service.BuildServiceProvider(); 
        }
    }
}
