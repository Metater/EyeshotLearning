using devDept;
using devDept.Eyeshot;
using devDept.Eyeshot.Control;
using devDept.Eyeshot.Control.Labels;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
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
        public MainWindow()
        {
            InitializeComponent();

            design1.ProgressChanged += Design1_ProgressChanged;
            design1.WorkCancelled += Design1_WorkCancelled;
            design1.WorkCompleted += Design1_WorkCompleted;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            design1.ProgressBar.Visible = false;

            BuildCube build = new();
            design1.StartWork(build);

            design1.ActiveViewport.Labels.Add(new TextOnly(0, 0, 1, "Hello, World!", new Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold), System.Drawing.Color.Green));

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

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            design1.CancelWork();
        }

        private class BuildCube : WorkUnit
        {
            public Solid? solid;

            public override void DoWork(BackgroundWorker worker, DoWorkEventArgs e)
            {
                solid = Solid.CreateBox(1, 1, 1);
                
            }

            public override void WorkCompleted(object sender)
            {
                Workspace workspace = (Workspace)sender;
                solid!.SmoothingAngle = Utility.DegToRad(45);

                workspace.Entities.Add(solid, System.Drawing.Color.Red);

                Circle circle = new(0, 0, 1, 1);
                workspace.Entities.Add(circle, System.Drawing.Color.Purple);

                workspace.ZoomFit();
            }
        }
    }
}
