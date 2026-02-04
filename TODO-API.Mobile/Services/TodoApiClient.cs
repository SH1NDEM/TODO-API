using System.Net.Http.Json;
using Microsoft.Maui.Storage;
using TODO_API.Mobile.Models;

namespace TODO_API.Mobile.Services;

public class TodoApiClient
{
    private const string PreferenceKey = "TodoApiBaseUrl";
    private readonly HttpClient _httpClient;
    private string _baseUrl;

    public TodoApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _baseUrl = Preferences.Get(PreferenceKey, "http://10.0.2.2:5000");
        UpdateBaseAddress();
    }

    public string BaseUrl
    {
        get => _baseUrl;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            _baseUrl = value.Trim();
            Preferences.Set(PreferenceKey, _baseUrl);
            UpdateBaseAddress();
        }
    }

    public async Task<List<TaskItem>> GetTasksAsync(string? filter = null, CancellationToken cancellationToken = default)
    {
        var path = filter switch
        {
            "open" => "/taskitems/isnotcomplite",
            "closed" => "/taskitems/iscomplite",
            _ => "/taskitems"
        };

        var response = await _httpClient.GetFromJsonAsync<List<TaskItem>>(path, cancellationToken);
        return response ?? new List<TaskItem>();
    }

    public async Task<TaskItem?> AddTaskAsync(string text, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/taskitems", new TaskItem
        {
            Text = text,
            IsClose = false
        }, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TaskItem>(cancellationToken: cancellationToken);
    }

    public async Task<bool> UpdateTaskAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"/taskitems/{task.Id}", task, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteTaskAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"/taskitems/{id}", cancellationToken);
        return response.IsSuccessStatusCode;
    }

    private void UpdateBaseAddress()
    {
        if (!Uri.TryCreate(_baseUrl, UriKind.Absolute, out var uri))
        {
            return;
        }

        _httpClient.BaseAddress = uri;
    }
}
