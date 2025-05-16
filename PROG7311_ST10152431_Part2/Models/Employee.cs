using Microsoft.EntityFrameworkCore;
using PROG7311_ST10152431_Part2.Data;

namespace PROG7311_ST10152431_Part2.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public required string EmployeeName { get; set; }

    }
}
