using E_Bank.Dto;
using E_Bank.Exceptions;
using E_Bank.Models;
using E_Bank.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace E_Bank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        
        private AccountDto ModelToDto(Account account)
        {
            return new AccountDto()
            {
                AccountNumber = account.AccountNumber,
                AccountBalance=account.AccountBalance,
                AccountType = account.AccountType,
                IntrestRate = account.IntrestRate,
                IsActive = account.IsActive,
                OpenningDate = account.OpenningDate,
                CustomerId = account.CustomerId,
                TransactionsCount = account.Transactions !=null ? account.Transactions.Count() :0
                
            };
        }
       
        [HttpGet("activeAccounts")]//admin view all active account
        public IActionResult GetAll()
        {
            List<AccountDto> result = new List<AccountDto>();
            var DataList = _accountService.GetAll();

            if (DataList.Count == 0)
            {
                throw new UserNotFoundException("Cannot find any Account ");
               // return BadRequest("No customer Added");
            }
            foreach (var Data in DataList)
            {
                result.Add(ModelToDto(Data));
            }
            return Ok(result);
        }


        

        [HttpPut("{accountNumber:int}/activate")] // Activate the account
        public IActionResult ActivateAccount(int accountNumber)
        {
            var result = _accountService.ActivateAccount(accountNumber);

            if (result != null)
            {
                return Ok(new ReturnMessage() { Message = "Account activated successfully" });
            }

            throw new UserNotFoundException("Cannot find the match account");
        }

        [HttpGet("accounRequest")] //admin view all Not active account
        public IActionResult GetRequest()
        {
            //List<AccountDto> result = new List<AccountDto>();
            var DataList = _accountService.GetAllRequest();

            if (DataList.Count == 0)
            {
                throw new UserNotFoundException("Cannot find any Account ");
            }
           
            return Ok(DataList);
        }







       

        [HttpGet("TransactionFilter/{id:int}")] //admin search using id get specific customer 
        public IActionResult GetAccountTransactions(int id)
        {
            var matched = _accountService.AccountFilter(id);

            if (matched != null)
            {
                return Ok(matched);
            }
            throw new UserNotFoundException("Cannot find the match id");

        }


        [HttpGet("{id:int}")] //admin search using id get specific customer 
        public IActionResult Get(int id)
        {
            var matched = _accountService.GetById(id);

            if (matched != null)
            {
                return Ok(matched);
            }
            throw new UserNotFoundException("Cannot find the match id");

        }

        private Account DtoToModel(AccountDto account)
        {
            return new Account()
            {
                
                AccountBalance = account.AccountBalance,
                AccountType = account.AccountType,
                IntrestRate =0,
                IsActive =false,
                OpenningDate = DateTime.Now,
                CustomerId = account.CustomerId,
                

            };
        }



        [HttpPost("customerAccountRequest")]
        public IActionResult Post(AccountDto accountDto)
        {
            //needed to check savings and customerid same exist error return
            var Converted=DtoToModel(accountDto);
            var sucess=  _accountService.Add(Converted);
            if(sucess !=null)
            {
              return  Ok(new ReturnMessage() { Message = "Account Request send succesfully " });
            }
            return BadRequest("Account creating error");
        }

        [HttpPut("")]
        public IActionResult Put(AccountDto accountDto)
        {
            var matched=_accountService.GetById(accountDto.AccountNumber);
            if (matched != null)
            {
                var account = DtoToModel(accountDto);
                _accountService.Update(account);
                return Ok(accountDto);
            }
            throw new UserNotFoundException("Cannot find any Account ");
        }

        [HttpDelete("")]
        public IActionResult Delete(int id)
        {
            var matched=_accountService.GetById(id);
            if (matched != null)
            {
                _accountService.Delete(matched);
                return Ok(matched);
            }
            throw new UserNotFoundException("Cannot find any Account ");
        }

        //new added
        [HttpPost("AccountIntrestUpdate")]
        public IActionResult updateIntrest([FromBody]AccountIntrestUpdateDto accountIntrestUpdateDto)
        {

            var status = _accountService.UpdateInterest(accountIntrestUpdateDto);

            if(status==0)
            {
                throw new UserNotFoundException("Cannot find any Account to update Interest ");
            }
            return Ok(new ReturnMessage() { Message = " succesfully interest updated" });



           
        }



       


        //customer id passed and matched account find
        [HttpGet("customerIdAccountIdget/{id:int}")]
        public IActionResult FindAccountId(int id)
        {
          var matchedAccountId=  _accountService.FindAccountId(id);

            if (matchedAccountId!=null)
            { return Ok(matchedAccountId); }

            throw new UserNotFoundException("Cannot find any AccountId so no account added");
        }
       


    }
}
