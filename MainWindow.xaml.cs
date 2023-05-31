using devDept;
using devDept.Eyeshot;
using EyeshotLearning.CubeTest;
using EyeshotLearning.Helpers;
using EyeshotLearning.StressTest;
using System;
using System.Windows;

namespace EyeshotLearning
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly WorkManager<WorkUnit> workManager = new();
        // private readonly CubeTestManager cubeTestManager;
        private readonly StressTestManager stressTestManager;

        public MainWindow()
        {
            InitializeComponent();

            // cubeTestManager = new(this, design1, workManager);
            stressTestManager = new(this, design1, workManager);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            // DesignHelper.SetDisplayMode(design1, displayType.Rendered);
            // DesignHelper.SetDisplayMode(design1, displayType.Wireframe);
            design1.ActionMode = actionType.SelectVisibleByPick;
            design1.ProgressBar.Visible = false;
            design1.ShowFps = true;

            // design1.ActiveViewport.Labels.Add(new TextOnly(0, 0, 0, "Hello, World!", new Font("Tahoma", 16f, System.Drawing.FontStyle.Bold), System.Drawing.Color.Green, ContentAlignment.MiddleCenter));

            // cubeTestManager.Init();
            stressTestManager.Init();

            base.OnContentRendered(e);
        }

        private void Button_Step_Click(object sender, RoutedEventArgs e)
        {
            // cubeTestManager.Step();
            stressTestManager.Step();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            design1.CancelWork();
        }
    }
}
