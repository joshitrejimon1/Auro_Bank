using E_Bank.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Bank.Dto
{
    public class DocDto
    {
        public byte[] DocumentData { get; set; }

        public string DocumentType { get; set; }

        public DateTime UploadDate { get; set; }

        public string Status { get; set; }


        public int CustomerId { get; set; }



    }
}
