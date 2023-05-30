using devDept;
using devDept.Eyeshot;
using devDept.Eyeshot.Control;
using devDept.Eyeshot.Control.Labels;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using devDept.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Point3D = devDept.Geometry.Point3D;

namespace EyeshotLearning
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CubeState state = new();
        private readonly WorkManager<WorkUnit> workManager = new();

        public MainWindow()
        {
            InitializeComponent();

            design1.ProgressChanged += Design1_ProgressChanged;
            design1.WorkCancelled += Design1_WorkCancelled;
            design1.WorkCompleted += Design1_WorkCompleted;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            SetDisplayMode(design1, displayType.Wireframe);

            design1.ProgressBar.Visible = false;

            design1.ActiveViewport.Labels.Add(new TextOnly(0.5, 0.5, 1.5, "Hello, World!", new Font("Tahoma", 16f, System.Drawing.FontStyle.Bold), System.Drawing.Color.Green, ContentAlignment.MiddleCenter));

            workManager.AppendToQueue(new BuildCube(state));
            workManager.RunAll(design1);

            base.OnContentRendered(e);
        }

        private void Design1_ProgressChanged(object sender, WorkUnit.ProgressChangedEventArgs e)
        {
            labelProgressBar.Content = design1.ProgressBar.Text;
            progressBar.Value = e.Progress;
        }
        private void Design1_WorkCancelled(object sender, WorkUnitEventArgs e)
        {
            labelProgressBar.Content = "Cancelled";
            progressBar.Value = 0;
        }
        private void Design1_WorkCompleted(object sender, WorkCompletedEventArgs e)
        {
            labelProgressBar.Content = "Completed";
        }

        private void Button_Step_Click(object sender, RoutedEventArgs e)
        {
            workManager.AppendToQueue(new StepCube(state));
            workManager.RunAll(design1);
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            design1.CancelWork();
        }

        #region Design Config
        private static void SetDisplayMode(Design design, displayType displayType)
        {
            design.ActiveViewport.DisplayMode = displayType;
            SetBackgroundStyleAndColor(design);
            design.UpdateBoundingBox(); // Updates simplified representation (when available)
            design.Invalidate();
        }
        public static void SetBackgroundStyleAndColor(Design design)
        {
            design.ActiveViewport.CoordinateSystemIcon.Lighting = false;
            design.ActiveViewport.ViewCubeIcon.Lighting = false;

            switch (design.ActiveViewport.DisplayMode)
            {

                case displayType.HiddenLines:
                    ((BackgroundSettings)design.ActiveViewport.Background).TopColor = RenderContextUtility.ConvertColor(System.Drawing.Color.FromArgb(0xD2, 0xD0, 0xB9));
                    design.ActiveViewport.CoordinateSystemIcon.Lighting = true;
                    design.ActiveViewport.ViewCubeIcon.Lighting = true;

                    break;

                default:
                    ((BackgroundSettings)design.ActiveViewport.Background).TopColor = RenderContextUtility.ConvertColor(System.Drawing.Color.FromArgb(0xED, 0xED, 0xED));
                    break;
            }

            design.CompileUserInterfaceElements();
        }
        #endregion

        private class BuildCube : WorkUnit
        {
            private readonly CubeState state;

            public BuildCube(CubeState state)
            {
                this.state = state;
            }

            public override void DoWork(BackgroundWorker worker, DoWorkEventArgs e)
            {
                state.Solid = Solid.CreateBox(1, 1, 1);
                state.Text = new Text(0.5, 0.5, 1.01, "Hello, World!", 0.1, Text.alignmentType.MiddleCenter);
                state.Circle = new Circle(0.5, 0.5, 1, 1);
            }

            public override void WorkCompleted(object sender)
            {
                Workspace workspace = (Workspace)sender;
                state.Solid!.SmoothingAngle = Utility.DegToRad(45);

                workspace.Entities.Add(state.Solid, System.Drawing.Color.Red);
                workspace.Entities.Add(state.Text, System.Drawing.Color.Cyan);
                workspace.Entities.Add(state.Circle, System.Drawing.Color.Purple);

                workspace.ZoomFit();
            }
        }

        private class StepCube : WorkUnit
        {
            private readonly CubeState state;

            public StepCube(CubeState state)
            {
                this.state = state;
            }

            public override void DoWork(BackgroundWorker worker, DoWorkEventArgs e)
            {
                state.Text!.Translate(0, 0, 0.25);
                state.Circle!.Scale(2);
            }

            public override void WorkCompleted(object sender)
            {
                Workspace workspace = (Workspace)sender;

                workspace.Refresh();

                workspace.ZoomFit();
            }
        }

        public class CubeState
        {
            private Step step = Step.NegX;
            public Solid? Solid { get; set; }
            public Text? Text { get; set; }
            public Circle? Circle { get; set; }

            public bool TryStep(out Step nextStep)
            {
                if (step == Step.Done)
                {
                    nextStep = Step.Done;
                    return false;
                }

                nextStep = (Step)(((int)step) + 1);
                step = nextStep;
                return true;
            }

            public enum Step
            {
                NegX,
                NegY,
                NegZ,
                PosX,
                PosY,
                PosZ,
                Done
            }
        }
    }
}
