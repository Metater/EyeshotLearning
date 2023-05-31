using devDept;
using devDept.Eyeshot;
using devDept.Eyeshot.Control;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using EyeshotLearning.CubeTest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private readonly Random random = new();

        public BuildStressTest(StressTestState state)
        {
            this.state = state;
        }

        public override void DoWork(BackgroundWorker worker, DoWorkEventArgs e)
        {
            lock (state.entitiesLock)
            {
                state.entities.Clear();

                if (state.Cts != null)
                {
                    state.Cts.Cancel();
                    state.Task!.GetAwaiter().GetResult();
                }

                state.Cts = new();
                state.stopwatch.Restart();
                state.Task = Task.Run(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(15);

                        Update(state.stopwatch.Elapsed.TotalSeconds);
                    }
                }, state.Cts.Token);
                state.lastUpdateTime = -1;

                for (int i = 0; i < 1000; i++)
                {
                    int argb = random.Next(int.MinValue, int.MaxValue);
                    // Ensure no transparency
                    unchecked
                    {
                        argb |= (int)0xFF000000;
                    }
                    Color color = Color.FromArgb(argb);
                    Mesh cube = Mesh.CreateBox(1, 1, 1);
                    cube.ColorMethod = colorMethodType.byEntity;
                    cube.Color = color;
                    Vector3D position = new(GetRandomDouble(-50, 50), GetRandomDouble(-50, 50), GetRandomDouble(-50, 50));
                    cube.Translate(position);
                    cube.EntityData = new StressTestEntityData();
                    state.entities.Add(cube);

                    if (Cancelled(worker, e))
                        return;

                    UpdateProgress(i, 1000, $"Stress test in progress...", worker);
                }
            }
        }

        public override void WorkCompleted(object sender)
        {
            var workspace = (Workspace)sender;

            workspace.Entities.Clear();
            lock (state.entitiesLock)
            {
                workspace.Entities.AddRange(state.entities);
            }
            workspace.ZoomFit();
            workspace.Invalidate();
        }

        private double GetRandomDouble(double min, double max)
        {
            return min + ((max - min) * random.NextDouble());
        }

        private void Update(double time)
        {
            // Only update once every 0.1 seconds
            if (state.lastUpdateTime > 0 && time - state.lastUpdateTime < 0.1)
            {
                return;
            }

            double deltaTime = state.lastUpdateTime < 0 ? 0.1 : time - state.lastUpdateTime;

            state.lastUpdateTime = time;

            lock (state.entitiesLock)
            {
                foreach (var entity in state.entities)
                {
                    entity.Translate(0, 0, deltaTime * 10);
                    entity.Regen(new RegenParams(0.1)
                    {
                        
                    });
                }
            }
        }
    }
}
