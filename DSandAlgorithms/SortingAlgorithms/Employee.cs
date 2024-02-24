using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingAlgorithms
{
    public class Employee : IComparable<Employee>
    {
        public Employee(int id, string name, double salary)
        {
            Id = id;
            Name = name;
            Salary = salary;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public double Salary { get; set; }

        public int CompareTo(Employee? other)
        {
            //if(this.Salary > other?.Salary)
            //    return 1;
            //else if(this.Salary < other?.Salary)
            //    return -1;
            //else return 0;
            return Salary.CompareTo(other?.Salary);
        }

        public override string ToString()
        {
            return $"{Id} - {Name} - {Salary}";
        }
    }
}
