using E_Bank.Data;
using E_Bank.Dto;
using E_Bank.Models;
using E_Bank.Repository;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using static E_Bank.Repository.IRepository;

namespace E_Bank.Services
{
    
    public class AccountService : IAccountService
    {
            private IRepository<Account> _repository;
            private IRepository<TransactionClass> _transactionClassRepository;
        private readonly MyContext _context;
            public AccountService(IRepository<Account> repository,IRepository<TransactionClass> transactionRep, MyContext context)
            {
                _repository = repository;
                _transactionClassRepository = transactionRep;
            _context = context;
            }

        
        public Account ActivateAccount(int accountNumber)
        {
            var account = _context.accountsTable.FirstOrDefault(a => a.AccountNumber == accountNumber);

            if (account != null)
            {
                account.IsActive = true;
                _context.SaveChanges();
            }

            return account;
        }

        public List<Account> GetAll()
        {
            return _repository.GetAll().Where(cus => cus.IsActive)
                     .Include(acnt => acnt.Transactions.Where(acnt => acnt.IsActive == true)).ToList();

        }

        public List<Account> GetAllRequest()
        {
            return _repository.GetAll()
                              .Where(acn => acn.IsActive == false)
                              .ToList();



        }


        public int ActivateRequest(int accountActivateDto)
        {
          var matched  =  _repository.Get()
                          .Where(acn=>acn.AccountNumber==accountActivateDto && acn.IsActive==false)
                         .FirstOrDefault();
            if(matched==null)
            {
                throw new InvalidOperationException("Account not found");
            }

            matched.IsActive=true;

          if(  _repository.Update(matched) == null )
            {
                throw new InvalidOperationException("Account not found");
            }

            return 1;


        }



        public List<Account> AccountFilter(int id)
        {
           var matched= _repository.Get()
                        .Where(acn => acn.AccountNumber == id && acn.IsActive == true)
                        .Include(acn => acn.Customer)  // Include the Customer navigation property
                        .Include(acn => acn.Transactions.Where(tran => tran.IsActive == true))
                        .ToList();

            return matched;
        }




        public int Add(Account account)
        {
            var save = _repository.GetAll().Where(acc => acc.AccountType == "Savings").FirstOrDefault();
            var current = _repository.GetAll().Where(acc => acc.AccountType == "Current").FirstOrDefault();
            var Fdd = _repository.GetAll().Where(acc => acc.AccountType == "FD").FirstOrDefault();


            if (save.AccountType == account.AccountType)
            {
                account.IntrestRate = save.IntrestRate;
            }
            else if (current.AccountType == account.AccountType)
            {
                account.IntrestRate = current.IntrestRate;
            }
            else if (Fdd.AccountType == account.AccountType)
            {
                account.IntrestRate = Fdd.IntrestRate;
            }
            return _repository.Add(account);

        }
    

        public Account GetById(int id)
        {
             var account = _repository.Get()
                      .Where(acn => acn.AccountNumber == id && acn.IsActive)
                      .OrderBy(acn => acn.AccountNumber)
                      .FirstOrDefault();

                if (account != null)
                {
                    _repository.Detached(account);
                }
                return account;

        }

        public Account Update(Account account)
        {
          return _repository.Update(account);
        }

        public void Delete(Account account)
        {
            _repository.delete(account);
            var transactionQuery=_transactionClassRepository.Get();
            foreach (var transactionClass in transactionQuery.Where(tran=>tran.AccountId==account.AccountNumber))
            {
                _transactionClassRepository.delete(transactionClass);
            }

        }

        public int UpdateInterest(AccountIntrestUpdateDto accountIntrestUpdateDto)
        {
          var matched= _repository.GetAll().Where(acn=>acn.AccountType==accountIntrestUpdateDto.AccountType).ToList();

            if (matched.Count == 0)
            {
                return 0;
            }

            foreach (var account in matched)
            {
                account.IntrestRate=accountIntrestUpdateDto.InterestRate;
               
                _repository.Update(account);

            }
            return 1;



        }

        public Account FindAccountId(int id)
        {
          var matchedAccount=   _repository.Get().Where(acn=>acn.CustomerId==id && acn.AccountType=="Savings").FirstOrDefault();
            if (matchedAccount==null)
            {
                return null;
            }
            return matchedAccount;
        }
    }
}
