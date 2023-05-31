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
        public readonly object entitiesLock = new();
        public readonly List<Entity> entities = new();
        public readonly Stopwatch stopwatch = new();
        public CancellationTokenSource? Cts { get; set; } = null;
        public Task? Task { get; set; } = null;
        public double lastUpdateTime = -1;
    }
}
