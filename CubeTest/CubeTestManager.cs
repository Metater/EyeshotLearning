using devDept;
using devDept.Eyeshot;
using devDept.Eyeshot.Control;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using SelectionChangedEventArgs = devDept.Eyeshot.Control.SelectionChangedEventArgs;

namespace EyeshotLearning.CubeTest
{
    public class CubeTestManager
    {
        private readonly MainWindow window;
        private readonly Design design;
        private readonly WorkManager<WorkUnit> workManager;

        private readonly CubeState state = new();

        public CubeTestManager(MainWindow window, Design design, WorkManager<WorkUnit> workManager)
        {
            this.window = window;
            this.design = design;
            this.workManager = workManager;

            design.ProgressChanged += Design1_ProgressChanged;
            design.WorkCancelled += Design1_WorkCancelled;
            design.WorkCompleted += Design1_WorkCompleted;
            design.SelectionChanged += Design1_SelectionChanged;
        }

        public void Init()
        {
            workManager.AppendToQueue(new BuildCube(state));
            workManager.RunAll(design);
        }

        public void Step()
        {
            workManager.AppendToQueue(new StepCube(state));
            workManager.RunAll(design);
        }

        private void Design1_ProgressChanged(object sender, WorkUnit.ProgressChangedEventArgs e)
        {
            window.progressBarLabel.Content = design.ProgressBar.Text;
            window.progressBar.Value = e.Progress;
        }
        private void Design1_WorkCancelled(object sender, WorkUnitEventArgs e)
        {
            window.progressBarLabel.Content = "Cancelled";
            window.progressBar.Value = 0;
        }
        private void Design1_WorkCompleted(object sender, WorkCompletedEventArgs e)
        {
            window.progressBarLabel.Content = $"Completed work, On step: {state.CurrentStep}";
        }
        private void Design1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StringBuilder sb = new();
            foreach (var addedItem in e.AddedItems)
            {
                sb.Append($"{addedItem.Item.GetType()} ");
            }
            window.infoLabel.Content = sb.ToString();
        }
    }
}
