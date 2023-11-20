namespace E_Bank.Models
{
    public class PageParameters
    {

        const int maxPagesize = 50;

        public int PageNumber { get; set; } = 1;

        private int _pageSize= 5;

        public int PageSize
        {
            get { return _pageSize; }

            set {
                _pageSize = (value > maxPagesize) ? maxPagesize : value;
                }

        }

    }
}
