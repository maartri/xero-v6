using System;

namespace Xero.Models
{
    public class EmployeeXeroDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleNames { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? OrdinaryEarningsRateID { get; set; }
        public Guid? PayrollCalendarID { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string TaxFileNumber { get; set; }
        public int Id { get; set; }
    }
}
