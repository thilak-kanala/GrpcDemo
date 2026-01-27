using System.Net.Http.Json;
using WSTCConsumer.Models;

namespace WSTCConsumer.Services;

public class TstApiClient
{
    private readonly HttpClient _httpClient;

    public TstApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<TstUserResponseDto>?> GetAllUsersAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<TstUserResponseDto>>("api/v1/tst/users");
    }

    public async Task<TstUserResponseDto?> GetUserByIdAsync(string id)
    {
        return await _httpClient.GetFromJsonAsync<TstUserResponseDto>($"api/v1/tst/users/{id}");
    }

    public async Task<IEnumerable<TstGroupResponseDto>?> GetAllGroupsAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<TstGroupResponseDto>>("api/v1/tst/groups");
    }

    public async Task<TstGroupResponseDto?> GetGroupByIdAsync(string id)
    {
        return await _httpClient.GetFromJsonAsync<TstGroupResponseDto>($"api/v1/tst/groups/{id}");
    }

    public async Task<IEnumerable<TstGroupResponseDto>?> GetUserGroupsAsync(string userId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<TstGroupResponseDto>>($"api/v1/tst/users/{userId}/groups");
    }
}
