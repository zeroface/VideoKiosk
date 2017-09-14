using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace VideoKiosK.utility
{
    public class RoundButton : Button
    {
        private bool ShowBorder { get; set; }
        public RoundButton()
        : base()
        {
            // Prevent the button from drawing its own border
            FlatAppearance.BorderSize = 0;

            // Set up a blue border and back colors for the button
            //FlatAppearance.BorderColor = Color.Transparent;
            FlatAppearance.CheckedBackColor = Color.Transparent;
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
            FlatStyle = FlatStyle.Flat;

            // Set the size for the button to be the same as a ToolStripButton
            Size = new System.Drawing.Size(23, 22);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            // Show the border when you hover over the button
            ShowBorder = true;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            // Hide the border when you leave the button
            ShowBorder = false;
        }
    

        protected override void OnPaint(PaintEventArgs e)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, Size.Width, Size.Height);
            this.Region = new Region(path);
            base.OnPaint(e);
        }
    }
}
