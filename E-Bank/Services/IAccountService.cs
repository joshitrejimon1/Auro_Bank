using E_Bank.Dto;
using E_Bank.Models;
using E_Bank.Repository;

namespace E_Bank.Services
{
    public interface IAccountService
    {
        public Account ActivateAccount(int accountNumber);
        public List<Account> GetAll();

        public List<Account> GetAllRequest();

        public int ActivateRequest(int id);

        public List<Account> AccountFilter(int id);//account flter for transaction

        public Account GetById(int id);

        public int Add(Account account);

        public Account Update(Account account);

        public void Delete(Account account);

        public int UpdateInterest(AccountIntrestUpdateDto accountIntrestUpdateDto);//update account intrest

        public Account FindAccountId(int id);//customerId fetch account id

        //public PageList<TransactionClass> getallaccounts(PageParameters pageparameters);//pagination
        //public List<TransactionClass> GetAllAccountsName();
    }
}
