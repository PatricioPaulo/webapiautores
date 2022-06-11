using Microsoft.EntityFrameworkCore;

namespace WebApiAutores.Utils
{
    public static class HttpContextExtentions
    {
        public async static Task InsertPaginationParamInHeader<T>(this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if(httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            double count = await queryable.CountAsync();
            httpContext.Response.Headers.Add("totalCount", count.ToString());
        }
    }
}
