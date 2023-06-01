using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeshotLearning.StressTest
{
    public class TranslatingEntity : BlockReference
    {
        private const double speed = 0.5;
        private const double amplitude = 10;

        private readonly Stopwatch stopwatch;
        private readonly double offsetSeconds;

        private double position = 0;
        private Transformation? customTransform = null;

        public TranslatingEntity(Stopwatch stopwatch, Random random, string blockName) : base(0, 0, 0, blockName, 1, 1, 1, 0)
        {
            this.stopwatch = stopwatch;
            offsetSeconds = random.NextDouble() * 3600;
        }

        protected override void Animate(int frameNumber)
        {
            double time = stopwatch.Elapsed.TotalSeconds + offsetSeconds;
            time *= speed;
            position = amplitude * Math.Sin(time);
        }

        public override void MoveTo(DrawParams data)
        {
            base.MoveTo(data);

            customTransform = new Translation(0d, 0d, position);
            data.RenderContext.MultMatrixModelView(customTransform);
        }

        public override bool IsInFrustum(FrustumParams data, Point3D center, double radius)
        {
            return true;
        }
    }
}
