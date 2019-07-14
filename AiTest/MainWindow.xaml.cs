using System;
using System.ComponentModel;
using System.Windows;

namespace AiTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const int DotCount = 100;
        private DateTime startTime = DateTime.Now;

        private Brain brain;

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public string FrameRateText => $"{brain?.FrameRate ?? 0:F2} fps";
        public string TimeText => $"Elapsed {(DateTime.Now - startTime).TotalSeconds:F2}";
        public string DeadText => $"Dead {brain?.DeadDotCount ?? 0}/{DotCount}";
        public bool IsRestartButtonEnabled { get; set; } = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Start();
        }

        private void Start()
        {
            IsRestartButtonEnabled = false;
            RaisePropertyChanged(nameof(IsRestartButtonEnabled));

            brain = new Brain(DotCount, Canvas);
            brain.FrameHappened += Brain_FrameHappened;
            brain.Finished += Brain_Finished;
            brain.Start();
            startTime = DateTime.Now;
        }

        private void Brain_Finished(object sender, EventArgs e)
        {
            brain.FrameHappened -= Brain_FrameHappened;
            brain.Finished -= Brain_Finished;
            IsRestartButtonEnabled = true;
            RaisePropertyChanged(nameof(IsRestartButtonEnabled));
        }

        private void Brain_FrameHappened(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(FrameRateText));
            RaisePropertyChanged(nameof(TimeText));
            RaisePropertyChanged(nameof(DeadText));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Canvas.Children.Clear();
            Start();
        }
    }
}
