using devDept.Eyeshot.Entities;

namespace EyeshotLearning.CubeTest
{
    public class CubeState
    {
        public Step CurrentStep { get; private set; } = Step.NegX;
        public Solid? Cube { get; set; }
        public Solid? Cone { get; set; }
        public Circle? Circle { get; set; }
        public Text? Text { get; set; }

        public void NextStep(out Step step)
        {
            // Increment step
            CurrentStep = (Step)(((int)CurrentStep) + 1);

            // Rollover
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
