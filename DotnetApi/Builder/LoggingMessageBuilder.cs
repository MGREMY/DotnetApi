using System.Text;

namespace DotnetApi.Builder;

public class LoggingMessage
{
    public OperationType? Type { get; set; }
    public bool WithModelName { get; set; } = false;
    public string? ModelName { get; set; }
    public bool WithCount { get; set; } = false;
    public int? Count { get; set; }
    public bool WithUserEmail { get; set; } = false;
    public string? UserEmail { get; set; }
    public bool WithId { get; set; } = false;
    public object? Id { get; set; }
    public bool WithRequest { get; set; } = false;
    public object? Request { get; set; }
    public bool WithValue { get; set; } = false;
    public object? Value { get; set; }

    public override string ToString()
    {
        if (Type is null) throw new NullReferenceException("Type cannot be null");

        StringBuilder sb = new();

        sb.Append(Type);

        if (WithModelName) sb.Append(" on {modelName}");
        if (WithCount) sb.Append(" {count} elements");
        if (WithId) sb.Append(" with id : {@id}");
        if (WithUserEmail) sb.Append(" from email {email}");
        if (WithRequest) sb.Append("; with request {@request}");
        if (WithValue) sb.Append("; with value {@value}");

        return sb.ToString();
    }

    public enum OperationType
    {
        Get,
        Create,
        Update,
        Delete,
    }
}

public class LoggingMessageBuilder
{
    private readonly LoggingMessage _loggingMessage = new();

    #region WithType

    public LoggingMessageBuilder WithType(LoggingMessage.OperationType type)
    {
        _loggingMessage.Type = type;
        return this;
    }

    #endregion

    #region WithModelName

    public LoggingMessageBuilder WithModelName()
    {
        _loggingMessage.WithModelName = true;
        return this;
    }

    public LoggingMessageBuilder WithModelName(string modelName)
    {
        _loggingMessage.ModelName = modelName;
        return WithModelName();
    }

    public LoggingMessageBuilder WithModelName<T>()
    {
        return WithModelName(typeof(T).FullName ?? typeof(T).Name);
    }

    #endregion

    #region WithCount

    public LoggingMessageBuilder WithCount()
    {
        _loggingMessage.WithCount = true;
        return this;
    }

    public LoggingMessageBuilder WithCount(int count)
    {
        _loggingMessage.Count = count;
        return WithCount();
    }

    #endregion

    #region WithUserEmail

    public LoggingMessageBuilder WithUserEmail()
    {
        _loggingMessage.WithUserEmail = true;
        return this;
    }

    public LoggingMessageBuilder WithUserEmail(string userEmail)
    {
        _loggingMessage.ModelName = userEmail;
        return WithUserEmail();
    }

    #endregion

    #region WithId

    public LoggingMessageBuilder WithId()
    {
        _loggingMessage.WithId = true;
        return this;
    }

    public LoggingMessageBuilder WithId(object id)
    {
        _loggingMessage.Id = id;
        return WithId();
    }

    #endregion

    #region WithRequest

    public LoggingMessageBuilder WithRequest()
    {
        _loggingMessage.WithRequest = true;
        return this;
    }

    public LoggingMessageBuilder WithRequest(object request)
    {
        _loggingMessage.Request = request;
        return WithRequest();
    }

    #endregion

    #region WithValue

    public LoggingMessageBuilder WithValue()
    {
        _loggingMessage.WithValue = true;
        return this;
    }

    public LoggingMessageBuilder WithValue(object? value)
    {
        _loggingMessage.Value = value;
        return WithValue();
    }

    #endregion

    public LoggingMessage Build()
    {
        return _loggingMessage;
    }

    public string BuildAndString()
    {
        return _loggingMessage.ToString();
    }

    public void BuildAndLogValue(Action<string, object[]> action)
    {
        var parameters = new List<object>(6);

        if (_loggingMessage.WithModelName) parameters.Add(_loggingMessage.ModelName!);
        if (_loggingMessage.WithCount) parameters.Add(_loggingMessage.Count!);
        if (_loggingMessage.WithId) parameters.Add(_loggingMessage.Id!);
        if (_loggingMessage.WithUserEmail) parameters.Add(_loggingMessage.UserEmail!);
        if (_loggingMessage.WithRequest) parameters.Add(_loggingMessage.Request!);
        if (_loggingMessage.WithValue) parameters.Add(_loggingMessage.Value!);

        action(_loggingMessage.ToString(), parameters.ToArray());
    }

    public void BuildAndLog<T0>(Action<string, T0> action, T0 p0)
    {
        action(_loggingMessage.ToString(), p0);
    }

    public void BuildAndLog<T0, T1>(Action<string, T0, T1> action, T0 p0, T1 p1)
    {
        action(_loggingMessage.ToString(), p0, p1);
    }

    public void BuildAndLog<T0, T1, T2>(Action<string, T0, T1, T2> action, T0 p0, T1 p1, T2 p2)
    {
        action(_loggingMessage.ToString(), p0, p1, p2);
    }

    public void BuildAndLog(Action<string, object[]> action, params object[] p)
    {
        action(_loggingMessage.ToString(), p);
    }
}