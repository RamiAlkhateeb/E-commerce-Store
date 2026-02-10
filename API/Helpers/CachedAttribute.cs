using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace API.Helpers
{
    /*filters allow code to be run before or after specific stages and request processing pipeline.
     * to run code immediately before an action method is called and after.
     * A method has been executed.
     * before the action method is called, we're going to want to see if
     * we've got the thing that they're asking for inside our cash.
     * And if we don't have it inside our cash, then we're going to execute the request 
     * and then the results of that we're going to put into our cash.
     * So the next person who comes in asking for the same thing, 
     * we're just going to get the result from the cash.
     */
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds;

        public CachedAttribute(int timeToLiveSeconds)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }

        /* next (parameter) is the part we use when the action is executed.
         * So are our controllers going to have gone to the database 
         * and it's received the response back from the
         * database at this point.
         */
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

            var cacheKey= GenerateCacheKeyFromRequest(context.HttpContext.Request);

            var cachedResponse = await cacheService.GetCachedResponseAsync<string>(cacheKey);

            if (!string.IsNullOrEmpty(cachedResponse))
            {
                var contentResult = new ContentResult
                {
                    Content= cachedResponse,
                    ContentType= "application/json",
                    StatusCode= 200,
                };
                context.Result= contentResult;
                return;
            }
            var executedContext = await next(); //move to controller
            if(executedContext.Result is OkObjectResult okObjectResult) 
            {
                await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value,
                    TimeSpan.FromSeconds(_timeToLiveSeconds));
            }
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{request.Path}");

            foreach ( var (key,value) in request.Query.OrderBy(x => x.Key) ) 
                keyBuilder.Append($"|{key}-{value}");

            return keyBuilder.ToString();
            
        }
    }
}
