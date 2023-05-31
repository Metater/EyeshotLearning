using devDept.Eyeshot.Control;
using devDept.Eyeshot;
using devDept.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeshotLearning.Helpers
{
    public static class DesignHelper
    {
        public static void SetDisplayMode(Design design, displayType displayType)
        {
            design.ActiveViewport.DisplayMode = displayType;
            SetBackgroundStyleAndColor(design);
            design.UpdateBoundingBox(); // Updates simplified representation (when available)
            design.Invalidate();
        }

        public static void SetBackgroundStyleAndColor(Design design)
        {
            design.ActiveViewport.CoordinateSystemIcon.Lighting = false;
            design.ActiveViewport.ViewCubeIcon.Lighting = false;

            switch (design.ActiveViewport.DisplayMode)
            {

                case displayType.HiddenLines:
                    ((BackgroundSettings)design.ActiveViewport.Background).TopColor = RenderContextUtility.ConvertColor(System.Drawing.Color.FromArgb(0xD2, 0xD0, 0xB9));
                    design.ActiveViewport.CoordinateSystemIcon.Lighting = true;
                    design.ActiveViewport.ViewCubeIcon.Lighting = true;

                    break;

                default:
                    ((BackgroundSettings)design.ActiveViewport.Background).TopColor = RenderContextUtility.ConvertColor(System.Drawing.Color.FromArgb(0xED, 0xED, 0xED));
                    break;
            }

            design.CompileUserInterfaceElements();
        }
    }
}
