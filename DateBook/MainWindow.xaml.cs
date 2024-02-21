using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DateBook
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Task> tasks = new ObservableCollection<Task>();
        private DateTime selectedDate = DateTime.Today;

        public MainWindow()
        {
            InitializeComponent();
            LoadTasksFromJson("tasks.json");
            tasksList.ItemsSource = tasks;
            datePicker.SelectedDate = DateTime.Today;
            ShowTasksForSelectedDate(DateTime.Today); 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tasks.Add(new Task(taskTextBox.Text, BoxDesription.Text, selectedDate));
            taskTextBox.Text = "";
            BoxDesription.Text = "";
            SaveTasksToJson("tasks.json");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (tasksList.SelectedItem != null)
            {
                Task selectedTask = tasksList.SelectedItem as Task;
                selectedTask.Name = taskTextBox.Text;
                selectedTask.Description = BoxDesription.Text;
                SaveTasksToJson("tasks.json");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (tasksList.SelectedItem != null)
            {
                tasks.Remove(tasksList.SelectedItem as Task);
                SaveTasksToJson("tasks.json");
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedDate = datePicker.SelectedDate ?? DateTime.Today;
            ShowTasksForSelectedDate(selectedDate);
            // Очистка содержимого TextBox при изменении даты
            taskTextBox.Text = "";
            BoxDesription.Text = "";
        }

        private void ShowTasksForSelectedDate(DateTime date)
        {
            var tasksForSelectedDate = tasks.Where(task => task.DueDate.Date == date.Date).ToList();
            tasksList.ItemsSource = tasksForSelectedDate;
        }

        private void SaveTasksToJson(string filePath)
        {
            string json = JsonConvert.SerializeObject(tasks);
            File.WriteAllText(filePath, json);
        }

        private void LoadTasksFromJson(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                tasks = JsonConvert.DeserializeObject<ObservableCollection<Task>>(json);
                ShowTasksForSelectedDate(selectedDate);
            }
        }

        private void tasksList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tasksList.SelectedItem != null)
            {
                Task selectedTask = tasksList.SelectedItem as Task;
                BoxDesription.Text = selectedTask.Description;
                taskTextBox.Text = selectedTask.Name;
            }
        }

    }

    public class Task
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }

        public Task(string name, string description, DateTime dueDate)
        {
            Name = name;
            Description = description;
            DueDate = dueDate;
        }
    }

    public static class TaskSerializer
    {
        public static void SerializeTasks(string filePath, ObservableCollection<Task> tasks)
        {
            string json = JsonConvert.SerializeObject(tasks);
            File.WriteAllText(filePath, json);
        }

        public static ObservableCollection<Task> DeserializeTasks(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<ObservableCollection<Task>>(json);
            }
            return new ObservableCollection<Task>();
        }
    }
}
