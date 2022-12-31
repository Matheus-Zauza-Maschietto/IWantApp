namespace IWantApp.Endpoints;

public static class ProgramDetailsExtensions
{
    public static Dictionary<string, string[]> ConvertToProblemDetails(this IReadOnlyCollection<Notification> notifications)
    {
        return notifications.GroupBy(p => p.Key)
                .ToDictionary(p => p.Key, p => p.Select(x => x.Message).ToArray());
    }
    public static Dictionary<string, string[]> ConvertToProblemDetails(this IEnumerable<IdentityError> error)
    {
        var dictionary = new Dictionary<string, string[]>();
        dictionary.Add("Error", error.Select(e => e.Description).ToArray());
        return dictionary;
    }
}
