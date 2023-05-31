using devDept.Eyeshot.Control;
using devDept.Geometry;
using devDept;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeshotLearning.CubeTest
{
    public class StepCube : WorkUnit
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
}
