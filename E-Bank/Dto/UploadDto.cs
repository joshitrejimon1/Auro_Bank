namespace E_Bank.Dto
{
    public class UploadDto
    {
        public string DocumentType { get; set; }
        public IFormFile DocumentFile { get; set; }
        public int CustomerId { get; set; }
    }
}
