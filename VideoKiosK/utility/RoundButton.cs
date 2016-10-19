using System;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace VideoKiosk.utility
{
    public class RoundButton : System.Windows.Forms.Button
    {
        protected override void OnResize(EventArgs e)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, ClientSize.Width, ClientSize.Height);
            this.Region = new Region(path);
            
            base.OnResize(e);
        }
    }
}
