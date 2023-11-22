using E_Bank.Dto;
using E_Bank.Exceptions;
using E_Bank.Models;
using E_Bank.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace E_Bank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {

        private readonly IDocumentService _docService;


        public DocumentController(IDocumentService docService)
        {
            _docService = docService;
        }
        private DocDto ModelToDto(Documents customer)
        {
            return new DocDto()
            {
                CustomerId = customer.CustomerId,
                // DocumentId = customer.DocumentId,
                DocumentData = customer.DocumentData,
                DocumentType = customer.DocumentType,
                Status = customer.Status,
                UploadDate = customer.UploadDate



            };
        }


        [HttpGet]
        public IActionResult GetAllDocuments()
        {

            var documents = _docService.GetAll();

            if (documents.Count == 0)
            {
                return NotFound("No documents found");
            }


            return Ok(documents);
        }
        //[HttpGet("")]
        //public IActionResult GetAll()
        //{
        //    List<DocDto> result = new List<DocDto>();
        //    var DataList = _docService.GetAll();

        //    if (DataList.Count == 0)
        //    {
        //        return BadRequest("No customer Added");
        //    }
        //    foreach (var Data in DataList)
        //    {
        //        result.Add(ModelToDto(Data));
        //    }
        //    return Ok(result);
        //}

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var CustomerData = _docService.GetById(id);

            if (CustomerData != null)
            {
                return Ok(CustomerData);
            }
            throw new UserNotFoundException("Cannot find the match id");
        }


        //private Documents ConvertoModel(DocDto customerDto)
        //{
        //    return new Documents()
        //    {
        //       UploadDate = customerDto.UploadDate,
        //       CustomerId = customerDto.CustomerId,
        //       DocumentData = customerDto.DocumentData,
        //       DocumentType = customerDto.DocumentType,
        //       Status = customerDto.Status,


        //    };
        //}

        private Documents ConvertToModel(DocDto docDto, byte[] documentData)
        {
            return new Documents()
            {
                UploadDate = DateTime.Now,
                CustomerId = docDto.CustomerId,
                DocumentData = documentData,
                DocumentType = docDto.DocumentType,
                Status = "Pending"
            };
        }
        //[HttpPost("")]
        //public IActionResult Post([FromForm] DocDto docDto)
        //{
        //    // Check if a file is provided
        //    if (docDto.DocumentFile != null && docDto.DocumentFile.Length > 0)
        //    {
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            docDto.DocumentFile.CopyTo(memoryStream);
        //            byte[] fileBytes = memoryStream.ToArray();

        //            // Now, you can save the fileBytes to your database or perform other actions
        //            var customer = ConvertToModel(docDto, fileBytes);
        //            var status = _docService.Add(customer);

        //            if (status != null)
        //            {
        //                return Ok(new ReturnMessage() { Message = "Document sent successfully" });
        //            }
        //            return BadRequest("Cannot add document");
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest("No file provided");
        //    }
        //}

        //public IActionResult Post(DocDto customerDto)
        //{
        //    var customer = ConvertoModel(customerDto);
        //    var status = _docService.Add(customer);

        //    if (status != null)
        //    {
        //        return Ok(new ReturnMessage() { Message = "Documnet send succesfully " });
        //    }
        //    return BadRequest("cannot added");
        //}

        //[HttpPut]

        //public IActionResult Put(DocDto customerDto)
        //{
        //    var Customer = _docService.GetById(customerDto.DocumentId);

        //    if (Customer != null)
        //    {
        //       // var modified = ConvertoModel(customerDto);
        //        _docService.Update(modified);
        //        return Ok(modified);
        //    }
        //    return BadRequest("Cannot modify data not found");
        //}

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var matched = _docService.GetById(id);
            if (matched != null)
            {
                _docService.Delete(matched);
                return Ok(matched);
            }
            return BadRequest("cannot find id to delete");
        }



        [HttpPost("upload")]
        public IActionResult UploadDocument([FromForm] UploadDto documentDto)
        {
            if (documentDto == null || documentDto.DocumentFile == null || documentDto.DocumentFile.Length <= 0)
            {
                return BadRequest("Invalid data or file");
            }

            using (var ms = new MemoryStream())
            {
                documentDto.DocumentFile.CopyTo(ms);
                var customerDocument = new Documents
                {
                    DocumentType = documentDto.DocumentType,
                    DocumentData = ms.ToArray(),
                    CustomerId = documentDto.CustomerId,
                    UploadDate = DateTime.Now,
                    Status = "Success"
                };


                var statuss = _docService.Add(customerDocument);
                if (statuss == 1)
                {
                    return Ok(new ReturnMessage() { Message = " succesfully Documnet Uploaded" });
                }
                return BadRequest("error in documnet uploading");

            }
        }
    }
}
