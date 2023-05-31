﻿using devDept.Eyeshot.Control;
using devDept;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeshotLearning.StressTest
{
    public class StressTestManager
    {
        private readonly MainWindow window;
        private readonly Design design;
        private readonly WorkManager<WorkUnit> workManager;

        private readonly StressTestState state = new();

        public StressTestManager(MainWindow window, Design design, WorkManager<WorkUnit> workManager)
        {
            this.window = window;
            this.design = design;
            this.workManager = workManager;

            design.ProgressChanged += Design1_ProgressChanged;
            design.WorkCancelled += Design1_WorkCancelled;
            design.WorkCompleted += Design1_WorkCompleted;
        }

        public void Init()
        {
            Run();
        }

        public void Step()
        {
            Run();
        }

        private void Run()
        {
            workManager.AppendToQueue(new BuildStressTest(state));
            workManager.RunAll(design);
        }

        private void Design1_ProgressChanged(object sender, WorkUnit.ProgressChangedEventArgs e)
        {
            window.progressBarLabel.Content = design.ProgressBar.Text;
            window.progressBar.Value = e.Progress;
        }
        private void Design1_WorkCancelled(object sender, WorkUnitEventArgs e)
        {
            window.progressBarLabel.Content = "Cancelled";
            window.progressBar.Value = 0;
        }
        private void Design1_WorkCompleted(object sender, WorkCompletedEventArgs e)
        {
            window.progressBarLabel.Content = "Completed work";
        }
    }
}
