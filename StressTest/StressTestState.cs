using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EyeshotLearning.StressTest
{
    public class StressTestState
    {
        public readonly Stopwatch stopwatch = Stopwatch.StartNew();
        public readonly Random random = new();
        public readonly List<Block> blocks = new();
        public readonly List<Entity> entities = new();
    }
}
