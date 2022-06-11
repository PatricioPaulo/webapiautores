namespace WebApiAutores.DTOs
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;
        private int pageSize = 10;
        private readonly int totalCount = 50;

        public int PageSize 
        {
            get { return pageSize; }
            set { pageSize = (value > totalCount ? totalCount : value); }
        }
    }
}
