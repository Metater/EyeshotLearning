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
using Vector3D = devDept.Geometry.Vector3D;

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
            SetDisplayMode(design1, displayType.Rendered);

            design1.ProgressBar.Visible = false;

            // design1.ActiveViewport.Labels.Add(new TextOnly(0, 0, 0, "Hello, World!", new Font("Tahoma", 16f, System.Drawing.FontStyle.Bold), System.Drawing.Color.Green, ContentAlignment.MiddleCenter));

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
            labelProgressBar.Content = $"Completed work, On step: {state.CurrentStep}";
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
                state.Cube = Solid.CreateBox(1, 1, 1);
                state.Cube.Translate(-0.5, -0.5, -0.5);

                state.Cone = Solid.CreateCone(0.5, 0.1, 1, 16);
                state.Cone.Translate(0, 0, -0.5);
                state.Cone.Rotate(Utility.DegToRad(-90), Vector3D.AxisY);
                state.Cone.Translate(-0.5, 0, 0);

                state.Circle = new Circle(Plane.YZ, new Point3D(-1.5, 0, 0), 0.5);

                state.Text = new Text(Plane.YZ, new Point3D(-1.5, 0, 0), "Hello, World!", 0.1, Text.alignmentType.MiddleCenter)
                {
                    Billboard = true
                };
            }

            public override void WorkCompleted(object sender)
            {
                Workspace workspace = (Workspace)sender;
                state.Cube!.SmoothingAngle = Utility.DegToRad(45);

                workspace.Entities.Add(state.Cube, System.Drawing.Color.Red);
                workspace.Entities.Add(state.Cone, System.Drawing.Color.Lime);
                workspace.Entities.Add(state.Circle, System.Drawing.Color.Blue);
                workspace.Entities.Add(state.Text, System.Drawing.Color.OrangeRed);

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
                state.NextStep(out var step);

                switch (step)
                {
                    case CubeState.Step.NegX:
                        state.Cone!.Rotate(Utility.DegToRad(-90), Vector3D.AxisY, Point3D.Origin);
                        state.Circle!.Rotate(Utility.DegToRad(-90), Vector3D.AxisY, Point3D.Origin);
                        state.Text!.Color = System.Drawing.Color.OrangeRed;
                        break;
                    case CubeState.Step.NegY:
                        state.Cone!.Rotate(Utility.DegToRad(90), Vector3D.AxisZ, Point3D.Origin);
                        state.Circle!.Rotate(Utility.DegToRad(90), Vector3D.AxisZ, Point3D.Origin);
                        state.Text!.Color = System.Drawing.Color.DarkRed;
                        break;
                    case CubeState.Step.NegZ:
                        state.Cone!.Rotate(Utility.DegToRad(90), Vector3D.AxisX, Point3D.Origin);
                        state.Circle!.Rotate(Utility.DegToRad(90), Vector3D.AxisX, Point3D.Origin);
                        state.Text!.Color = System.Drawing.Color.MediumVioletRed;
                        break;
                    case CubeState.Step.PosX:
                        state.Cone!.Rotate(Utility.DegToRad(-90), Vector3D.AxisY, Point3D.Origin);
                        state.Circle!.Rotate(Utility.DegToRad(-90), Vector3D.AxisY, Point3D.Origin);
                        state.Text!.Color = System.Drawing.Color.Green;
                        break;
                    case CubeState.Step.PosY:
                        state.Cone!.Rotate(Utility.DegToRad(90), Vector3D.AxisZ, Point3D.Origin);
                        state.Circle!.Rotate(Utility.DegToRad(90), Vector3D.AxisZ, Point3D.Origin);
                        state.Text!.Color = System.Drawing.Color.DarkGreen;
                        break;
                    case CubeState.Step.PosZ:
                        state.Cone!.Rotate(Utility.DegToRad(90), Vector3D.AxisX, Point3D.Origin);
                        state.Circle!.Rotate(Utility.DegToRad(90), Vector3D.AxisX, Point3D.Origin);
                        state.Text!.Color = System.Drawing.Color.Lime;
                        break;
                }

                state.Text!.InsertionPoint = state.Circle!.Center;
                state.Text.TextString = step.ToString();
            }

            public override void WorkCompleted(object sender)
            {
                Workspace workspace = (Workspace)sender;

                workspace.Entities.Regen();

                // workspace.ZoomFit();
            }
        }

        public class CubeState
        {
            public Step CurrentStep { get; private set; } = Step.NegX;
            public Solid? Cube { get; set; }
            public Solid? Cone { get; set; }
            public Circle? Circle { get; set; }
            public Text? Text { get; set; }

            public void NextStep(out Step step)
            {
                CurrentStep = (Step)(((int)CurrentStep) + 1);

                if (CurrentStep == Step.Done)
                {
                    CurrentStep = Step.NegX;
                }

                step = CurrentStep;
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
