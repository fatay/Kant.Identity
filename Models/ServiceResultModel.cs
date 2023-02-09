namespace Identite.Models;

public class ServiceResultModel<T>
{
    public ServiceResultModel() => Errors = new List<ErrorModel>();

    public ServiceResultModel(T data) => Data = data;

    public List<ErrorModel> GetErrors() => Errors;

    public bool AnyErrors() => Errors.Count > 0;

    public void AddError(string errorCode, string errorMessage)
    {
        Errors.Add(new ErrorModel
        {
            ErrorCode = errorCode,
            ErrorMessage = errorMessage
        });
    }

    public void AddError(ErrorModel error) => Errors.Add(error);

    public void AddError(List<ErrorModel> errors) => Errors.AddRange(errors);

    public void AddError(IEnumerable<ErrorModel> errors) => Errors.AddRange(errors.ToList());

    public bool IsSucceed => Errors.Count == 0;

    public List<ErrorModel> Errors { get; set; }

    public T Data { get; set; }
}

public class ErrorModel
{
    public string ErrorCode { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;
}

