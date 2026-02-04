using TODO_API.Mobile.Models;
using TODO_API.Mobile.ViewModels;

namespace TODO_API.Mobile;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel.Tasks.Count == 0)
        {
            _ = _viewModel.RefreshAsync();
        }
    }

    private async void OnTaskCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.BindingContext is TaskItem task)
        {
            task.IsClose = e.Value;
            await _viewModel.UpdateTaskAsync(task);
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is TaskItem task)
        {
            await _viewModel.DeleteTaskAsync(task);
        }
    }
}
