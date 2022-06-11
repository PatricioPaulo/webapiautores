using WebApiAutores.DTOs;

namespace WebApiAutores.Utils
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Page<T>(this IQueryable<T> quueryable , PaginationDTO paginationDTO)
        {
            return quueryable
                .Skip((paginationDTO.Page - 1) * paginationDTO.PageSize)
                .Take(paginationDTO.PageSize);
        }
    }
}
