using Swashbuckle.AspNetCore.SwaggerGen;

namespace Kant.Identity.Models;

public class ServiceResultModel<T>
{
    public ServiceResultModel()
    {
        Succeeded = true;
    }

    public ServiceResultModel(T data, string successMessage = null)
    {
        Succeeded = true;
        Message = successMessage;
        Data = data;
    }

    public ServiceResultModel(List<KeyValuePair<string, string>> errors)
    {
        Succeeded = false;
        Errors = errors;
    }

    public bool Succeeded { get; set; }
    public string Message { get; set; }
    public List<KeyValuePair<string, string>> Errors { get; set; }
    public T Data { get; set; }
}
