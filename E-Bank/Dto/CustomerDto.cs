using E_Bank.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Bank.Dto
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

       // public DateOnly DOB { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }

        public int CountAccounts { get; set; } = 0;

        [ForeignKey("User")]
        public int UserId { get; set; }

      //  public int Documents { get; set; }//this should change list to int else it turns entire table shows

      //  public int  Queries { get; set; }
    }
}
