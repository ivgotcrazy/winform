using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace meetingdemo_csharp
{
    public partial class TransparentLabel : UserControl
    {
        private Brush bgBrush = null;
        private float opacity = 1f;

        public TransparentLabel()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.Opaque, true);

            this.BackColor = Color.Transparent;
            BackgroundBrush = Brushes.Transparent;
        }

        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                ResetBgBrush();
            }
        }

        public int Radius
        {
            get;
            set;
        }

        public float Opacity
        {
            get
            {
                return opacity;
            }
            set
            {
                if (value > 1 || value < 0)
                {
                    throw new Exception("Out of range,the Value should be in [0,1]");
                }
                else
                {
                    opacity = value;
                    ResetBgBrush();
                }
            }
        }

        protected virtual Brush BackgroundBrush
        {
            get
            {
                return bgBrush;
            }
            set
            {
                bgBrush = value;
            }
        }

        protected Color GetOpacityColor(Color baseColor, float op)
        {
            return Color.FromArgb(Convert.ToInt32(op * baseColor.A), baseColor);
        }

        protected void ResetBgBrush()
        {
            BackgroundBrush = new SolidBrush(GetOpacityColor(BackColor, Opacity));
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle = 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        private void FillRadiusRectangle(Graphics g)
        {
            int radius = Radius;
            int width = (int)this.Bounds.Width;
            int height = (int)this.Bounds.Height;

            GraphicsPath path = new GraphicsPath();

            path.AddLine(new Point(radius, 0), new Point(width - radius, 0));

            if (radius > 0)
                path.AddArc(new Rectangle(width - 2 * radius, 0, 2 * radius, 2 * radius), 270, 90);

            path.AddLine(new Point(width, radius), new Point(width, height - radius));

            if (radius > 0)
                path.AddArc(new Rectangle(width - 2 * radius, height - 2 * radius, 2 * radius, 2 * radius), 0, 90);

            path.AddLine(new Point(radius, height), new Point(width - radius, height));

            if (radius > 0)
                path.AddArc(new Rectangle(0, height - 2 * radius, 2 * radius, 2 * radius), 90, 90);

            path.AddLine(new Point(0, radius), new Point(0, height - radius));

            if (radius > 0)
                path.AddArc(new Rectangle(0, 0, 2 * radius, 2 * radius), 180, 90);

            path.CloseAllFigures();

            g.FillPath(BackgroundBrush, path);
        }

        protected void DrawImage(Graphics g)
        {
            Image img = this.BackgroundImage;

            g.DrawImage(img, new Rectangle(new Point(0, 0), new Size(img.Width, img.Height)));
        }

        private void DrawText(Graphics g)
        {
            Brush brush = new SolidBrush(ForeColor);

            Size textSize = TextRenderer.MeasureText(this.Text, this.Font);

            int left = (this.Bounds.Width - textSize.Width) / 2;
            int top = (this.Bounds.Height - textSize.Height) / 2;

            if (left >= 0 && top >= 0)
            {
                g.DrawString(Text, this.Font, brush, left, top);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;  //使绘图质量最高，即消除锯齿
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

            FillRadiusRectangle(e.Graphics);

            DrawImage(e.Graphics);

            DrawText(e.Graphics);
        }
    }
}
