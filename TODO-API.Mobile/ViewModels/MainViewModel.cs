using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TODO_API.Mobile.Models;
using TODO_API.Mobile.Services;

namespace TODO_API.Mobile.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly TodoApiClient _apiClient;
    private bool _isBusy;
    private string _newTaskText = string.Empty;
    private string _baseUrl;

    public MainViewModel(TodoApiClient apiClient)
    {
        _apiClient = apiClient;
        _baseUrl = _apiClient.BaseUrl;
        Tasks = new ObservableCollection<TaskItem>();

        RefreshCommand = new Command(async () => await RefreshAsync());
        LoadOpenCommand = new Command(async () => await LoadFilteredAsync("open"));
        LoadClosedCommand = new Command(async () => await LoadFilteredAsync("closed"));
        AddTaskCommand = new Command(async () => await AddTaskAsync());
        SaveBaseUrlCommand = new Command(SaveBaseUrl);
    }

    public ObservableCollection<TaskItem> Tasks { get; }

    public string NewTaskText
    {
        get => _newTaskText;
        set => SetProperty(ref _newTaskText, value);
    }

    public string BaseUrl
    {
        get => _baseUrl;
        set => SetProperty(ref _baseUrl, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public Command RefreshCommand { get; }
    public Command LoadOpenCommand { get; }
    public Command LoadClosedCommand { get; }
    public Command AddTaskCommand { get; }
    public Command SaveBaseUrlCommand { get; }

    public async Task RefreshAsync() => await LoadFilteredAsync(null);

    public async Task UpdateTaskAsync(TaskItem task)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        await _apiClient.UpdateTaskAsync(task);
        IsBusy = false;
    }

    public async Task DeleteTaskAsync(TaskItem task)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        var success = await _apiClient.DeleteTaskAsync(task.Id);
        if (success)
        {
            Tasks.Remove(task);
        }

        IsBusy = false;
    }

    private async Task LoadFilteredAsync(string? filter)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        var items = await _apiClient.GetTasksAsync(filter);
        Tasks.Clear();
        foreach (var item in items)
        {
            Tasks.Add(item);
        }

        IsBusy = false;
    }

    private async Task AddTaskAsync()
    {
        if (IsBusy || string.IsNullOrWhiteSpace(NewTaskText))
        {
            return;
        }

        IsBusy = true;
        var created = await _apiClient.AddTaskAsync(NewTaskText.Trim());
        if (created is not null)
        {
            Tasks.Insert(0, created);
            NewTaskText = string.Empty;
        }

        IsBusy = false;
    }

    private void SaveBaseUrl()
    {
        _apiClient.BaseUrl = BaseUrl;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
        {
            return;
        }

        backingStore = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
