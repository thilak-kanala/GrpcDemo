using WSTCConsumer.Services;

var httpClient = new HttpClient
{
    BaseAddress = new Uri("http://localhost:5000")
};

var apiClient = new TstApiClient(httpClient);

try
{
    var users = await apiClient.GetAllUsersAsync();
    Console.WriteLine($"Retrieved {users?.Count() ?? 0} users");

    var groups = await apiClient.GetAllGroupsAsync();
    Console.WriteLine($"Retrieved {groups?.Count() ?? 0} groups");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
