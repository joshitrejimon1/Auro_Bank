using E_Bank.Dto;
using E_Bank.Models;
using E_Bank.Repository;

using static E_Bank.Repository.IRepository;

namespace E_Bank.Services
{
    public class TransactionService : ITransactionService
    {
        private IRepository<TransactionClass> _Transactionrepository;

        private IRepository<Account> _accountRepository;

        public TransactionService(IRepository<TransactionClass> repository,IRepository<Account> repository1)
        {
            this._Transactionrepository = repository;
            _accountRepository = repository1;
        }



        //note detached
        public int Deposite(TransactionDto transactionDto)
        {
            var matchedAccount = _accountRepository.Get()
                .Where(acn => acn.AccountNumber == transactionDto.AccountId && acn.IsActive)
                .FirstOrDefault();

            //detached dout??
            if (matchedAccount == null)
            {
                return 0;
            }

            double amount = transactionDto.TransactionAmount;
            double accountBalnce = matchedAccount.AccountBalance;

            double newBalance=accountBalnce + amount;
            matchedAccount.AccountBalance = newBalance; 

             var sucess= _accountRepository.Update(matchedAccount);


            if (sucess != null)
            {
                TransactionClass newTransaction = new TransactionClass
                {
                    AccountId = transactionDto.AccountId,
                    TransactionDate = DateTime.Now,
                    Description = transactionDto.Description,
                    TransactionAmount = transactionDto.TransactionAmount,
                    IsActive =  true,
                    TransactionType = transactionDto.TransactionType,
                    State =  "Sucess",

                };
                _Transactionrepository.Add(newTransaction);
                return 1;
            }
           return 0;

        }

        //withdrawal
        public int Withdraw(TransactionDto transactionDto)
        {
          var matchedAccount=  _accountRepository.Get()
                .Where(acn=>acn.AccountNumber == transactionDto.AccountId && acn.IsActive)
                .FirstOrDefault();

            if(matchedAccount == null)
                { return 0; }

            double amount = transactionDto.TransactionAmount;

            double currentBalance=matchedAccount.AccountBalance;

            double newBalance=currentBalance - amount;

            matchedAccount.AccountBalance = newBalance;

           var success= _accountRepository.Update(matchedAccount);

            if (success != null)
            {
                TransactionClass newTransaction = new TransactionClass
                {
                    TransactionType = transactionDto.TransactionType,
                    IsActive = true,
                    AccountId = transactionDto.AccountId,
                    TransactionDate = DateTime.Now,
                    Description = transactionDto.Description,
                    TransactionAmount = transactionDto.TransactionAmount,
                    State = "Success"

                };

                _Transactionrepository.Add(newTransaction);
                return 1;

            }

            return 0;
        }

        //Transaction
        public int TransferAmount(TransferDto transactionDto)
        {
           var reciverAccount=_accountRepository.Get()
                .Where(acn=>acn.AccountNumber==transactionDto.TargetAccountNumber && acn.IsActive)
                .FirstOrDefault();

            var sourceAccount=_accountRepository.Get()
                .Where(acn=>acn.AccountNumber==transactionDto.AccountNumber && acn.IsActive)
                .FirstOrDefault();


            if(reciverAccount ==null && sourceAccount==null)
            {
                return 0;
            }

            double amount=transactionDto.Amount;
            reciverAccount.AccountBalance += amount;
            sourceAccount.AccountBalance -= amount;

            _accountRepository.Update(sourceAccount);
            _accountRepository.Update(reciverAccount);

            TransactionClass newTransaction = new TransactionClass
            {
                TransactionDate = DateTime.Now,
                TransactionAmount = transactionDto.Amount,
                Description = transactionDto.Description,
                IsActive = true,
                TransactionType = "Transfer Amount",
                AccountId = transactionDto.AccountNumber,
                State = "Success",

            };

            _Transactionrepository.Add(newTransaction);
            return 1;
        }

        public List<TransactionClass> GetAll()
        {
            return _Transactionrepository.GetAll().Where(tran=>tran.IsActive).ToList();

        }


        public TransactionClass GetById(int id)
        {
            var tableName = _Transactionrepository.Get();
            var DataFound = tableName.Where(cus => cus.IsActive == true && cus.TransactionId == id)
                  .OrderBy(cus => cus.TransactionId)
                  .FirstOrDefault();
            if (DataFound != null)
            {
                _Transactionrepository.Detached(DataFound);
            }
            return DataFound;
        }

      
        public void Delete(TransactionClass transaction)
        {
            _Transactionrepository.delete(transaction);

        }


        //single Date filter
        public List<TransactionClass> GetBysingleDate(DateTime dateTime)
        {
            DateTime startDate = dateTime.Date;
            DateTime endDate = startDate.AddDays(1).AddTicks(-1);

            return _Transactionrepository.Get().Where(tra => tra.TransactionDate >= startDate && tra.TransactionDate <= endDate).ToList();
        }


        //two date filter
        public List<TransactionClass> GetByDate(DateDto dateDto)
        {
            DateTime startDate = dateDto.Date;
            DateTime endDate = dateDto.EndDate;
            

            return _Transactionrepository.Get()
                .Where(tra => tra.TransactionDate >= startDate && tra.TransactionDate <= endDate 
                 && tra.AccountId==dateDto.AccountNumber).ToList();
        }




        //public int Add(TransactionClass transaction)
        //{
        //    return _repository.Add(transaction);
        //}

        //public TransactionClass Update(TransactionClass transaction)
        //{
        //    return _repository.Update(transaction);
        //}

    }
}
