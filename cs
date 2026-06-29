using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;

namespace AvaloniaApplication1
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<TaskItem> AllTasks { get; set; } = new();
        public ObservableCollection<TaskItem> FilteredTasks { get; set; } = new();
        public List<string> Categories { get; set; } = new() { "Все", "Работа", "Личное", "Учёба" };
        public List<string> CreateCategories { get; set; } = new() { "Работа", "Личное", "Учёба" };

        public MainWindow()
        {
            InitializeComponent();
            CategoryFilterCombo.ItemsSource = Categories;
            CategoryFilterCombo.SelectedIndex = 0;
            NewTaskCategoryCombo.ItemsSource = CreateCategories;
            NewTaskCategoryCombo.SelectedIndex = 1;
            TaskControlList.ItemsSource = FilteredTasks;
            AddTask("Купить молоко", "Личное");
            AddTask("Сдать лабораторную", "Учёба");
        }
        private void OnAddTaskClick(object sender, RoutedEventArgs e)
        {
            string title = NewTaskText.Text?.Trim() ?? "";
            string category = NewTaskCategoryCombo.SelectedItem as string ?? "Личное";

            if (!string.IsNullOrWhiteSpace(title))
            {
                AddTask(title, category);
                NewTaskText.Text = string.Empty;
            }
        }
        private void AddTask(string title, string category)
        {
            var task = new TaskItem { Title = title, Category = category };
            task.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(TaskItem.IsCompleted))
                {
                    UpdateFiltersAndProgress();
                }
            };

            AllTasks.Add(task);
            UpdateFiltersAndProgress();
        }
        private void OnDeleteTaskClick(object sender, RoutedEventArgs e)
        {
            if (TaskControlList.SelectedItem is TaskItem selectedTask)
            {
                AllTasks.Remove(selectedTask);
                UpdateFiltersAndProgress();
            }
        }
        private void UpdateFiltersAndProgress()
        {
            if (TaskProgressBar == null || RadioActive == null || RadioCompleted == null || CategoryFilterCombo == null)
                return;
            if (AllTasks.Count == 0)
            {
                TaskProgressBar.Value = 0;
            }
            else
            {
                double completedCount = AllTasks.Count(t => t.IsCompleted);
                TaskProgressBar.Value = (completedCount / AllTasks.Count) * 100;
            }
            string selectedCategory = CategoryFilterCombo.SelectedItem as string ?? "Все";
            bool? statusFilter = null;
            if (RadioActive.IsChecked == true) statusFilter = false;
            if (RadioCompleted.IsChecked == true) statusFilter = true;

            var filtered = AllTasks.Where(t =>
                (selectedCategory == "Все" || t.Category == selectedCategory) &&
                (statusFilter == null || t.IsCompleted == statusFilter)
            ).ToList();
            FilteredTasks.Clear();
            foreach (var task in filtered)
            {
                FilteredTasks.Add(task);
            }
        }
        private void OnFilterChanged(object sender, SelectionChangedEventArgs e) => UpdateFiltersAndProgress();
        private void OnRadioFilterChanged(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radio && radio.IsChecked == true)
            {
                UpdateFiltersAndProgress();
            }
        }
    }
    public class TaskItem : INotifyPropertyChanged
    {
        private string _title = string.Empty;
        private bool _isCompleted;
        private string _category = "Личное";

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set { _isCompleted = value; OnPropertyChanged(); }
        }
        public string Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(); }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
