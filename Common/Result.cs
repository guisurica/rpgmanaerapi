namespace rpgmanagerapi.Common;

public sealed class Result<T> where T : class
{
    public string Message { get; private set; }
    public int Code { get; private set; }
    public T Data { get; private set; }
    public bool IsSuccessResult { get; private set; }

    public Result() { }

    public Result(string message, int code, T data, bool isSuccessResult)
    {
        Message = message;
        Code = code;
        Data = data;
        IsSuccessResult = isSuccessResult;
    }

    public Result(Result<T> result)
    {
        Message = result.Message;
        Code = result.Code;
        Data = result.Data;
        IsSuccessResult = result.IsSuccessResult;
    }

    public static Result<T> Success(string message, int code, T data)
    {
        return new Result<T>(message, code, data, true);
    }

    public static Result<T> Success(string message, int code)
    {
        return new Result<T>(message, code, null, true);
    }

    public static Result<T> Success(Result<T> result)
    {
        return new Result<T>(result);
    }

    public static Result<T> Failure(string message, int code)
    {
        return new Result<T>(message, code, null, false);
    }

    public static Result<T> Failure(Result<T> result)
    {
        return new Result<T>(result);
    }

    
}