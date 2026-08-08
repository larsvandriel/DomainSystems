using Microsoft.AspNetCore.Http;

namespace Common.Results.AspNetCore.Mapping
{
    public interface IHttpResultMapper
    {
        IResult Map(Result result);

        IResult Map<T>(Result<T> result);
    }
}
