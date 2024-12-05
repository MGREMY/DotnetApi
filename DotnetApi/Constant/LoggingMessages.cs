namespace DotnetApi.Constant;

public static class LoggingMessages
{
    public const string RetrievedArrayMessage = "Retrieved {count} elements from {model}";
    public const string RetrievedSingleMessage = "Retrieved {id} from {model} : {@instance}";

    public const string CreateSingleMessage = "Creating {model} with values {@request} from {userEmail}";

    public const string UpdateSingleMessage = "Updating {model} with id {id} with values {@request} from {userEmail}";

    public const string DeleteSingleMessage = "Deleting {model} with id {id}";
}