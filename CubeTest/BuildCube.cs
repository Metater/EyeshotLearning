using devDept.Eyeshot.Control;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using devDept;
using System.ComponentModel;

namespace EyeshotLearning.CubeTest
{
    public class BuildCube : WorkUnit
    {
        private readonly CubeState state;

        public BuildCube(CubeState state)
        {
            this.state = state;
        }

        public override void DoWork(BackgroundWorker worker, DoWorkEventArgs e)
        {
            UpdateProgress(0, 4, "Waiting", worker);
            // Thread.Sleep(2000);

            state.Cube = Solid.CreateBox(1, 1, 1);
            state.Cube.Translate(-0.5, -0.5, -0.5);
            // Thread.Sleep(500);

            UpdateProgress(1, 4, "Generated cube", worker);

            state.Cone = Solid.CreateCone(0.25, 0, 1, 16);
            state.Cone.Translate(0, 0, -0.25);
            state.Cone.Rotate(Utility.DegToRad(-90), Vector3D.AxisY);
            state.Cone.Translate(-0.5, 0, 0);
            // Thread.Sleep(500);

            UpdateProgress(2, 4, "Generated cone", worker);

            state.Circle = new Circle(Plane.YZ, new Point3D(-1.5, 0, 0), 0.5);
            // Thread.Sleep(500);

            UpdateProgress(3, 4, "Generated circle", worker);

            state.Text = new Text(Plane.YZ, new Point3D(-1.5, 0, 0), "Hello, World!", 0.1, Text.alignmentType.MiddleCenter)
            {
                Billboard = true
            };
            // Thread.Sleep(500);

            UpdateProgress(4, 4, "Generated text", worker);
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
}
