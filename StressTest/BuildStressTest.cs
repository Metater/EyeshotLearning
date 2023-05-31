using devDept;
using devDept.Eyeshot;
using devDept.Eyeshot.Control;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using EyeshotLearning.CubeTest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EyeshotLearning.StressTest
{
    public class BuildStressTest : WorkUnit
    {
        private readonly StressTestState state;

        public BuildStressTest(StressTestState state)
        {
            this.state = state;
        }

        public override void DoWork(BackgroundWorker worker, DoWorkEventArgs e)
        {
            state.blocks.Clear();
            state.entities.Clear();

            for (int i = 0; i < 1000; i++)
            {
                int argb = state.random.Next(int.MinValue, int.MaxValue);
                // Ensure no transparency
                unchecked
                {
                    argb |= (int)0xFF000000;
                }
                Color color = Color.FromArgb(argb);

                int meshType = state.random.Next(0, 5);
                Mesh mesh = null!;
                switch (meshType)
                {
                    case 0:
                        mesh = Mesh.CreateBox(1, 1, 1);
                        break;
                    case 1:
                        mesh = Mesh.CreateTorus(1, 0.5, 16, 8);
                        break;
                    case 2:
                        mesh = Mesh.CreateSphere(1, 16, 16);
                        break;
                    case 3:
                        mesh = Mesh.CreateCylinder(1, 1, 16);
                        break;
                    case 4:
                        mesh = Mesh.CreateCone(1, 0, 1, 16);
                        break;
                }

                // Mesh cube = Mesh.CreateBox(1, 1, 1);

                mesh.ColorMethod = colorMethodType.byEntity;
                mesh.Color = color;
                Vector3D position = new(GetRandomDouble(-50, 50), GetRandomDouble(-50, 50), GetRandomDouble(-50, 50));
                mesh.Translate(position);
                mesh.EntityData = new StressTestEntityData();

                string blockName = i.ToString();

                Block b = new(blockName);
                b.Entities.Add(mesh);
                state.blocks.Add(b);

                state.entities.Add(new TranslatingEntity(state.stopwatch, state.random, blockName));

                if (Cancelled(worker, e))
                    return;

                UpdateProgress(i, 1000, $"Stress test in progress...", worker);
            }
        }

        public override void WorkCompleted(object sender)
        {
            var workspace = (Workspace)sender;

            workspace.Blocks.Clear();
            workspace.Entities.Clear();

            workspace.Blocks.AddRange(state.blocks);
            workspace.Entities.AddRange(state.entities);

            workspace.ZoomFit();
            workspace.Invalidate();
        }

        private double GetRandomDouble(double min, double max)
        {
            return min + ((max - min) * state.random.NextDouble());
        }
    }
}
