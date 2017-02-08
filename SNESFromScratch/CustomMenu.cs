using System.Drawing;
using System.Windows.Forms;

namespace SNESFromScratch
{
    public class CustomMenu : ToolStripRenderer
    {
        public Color BgColor1 = Color.FromArgb(255, 90, 90, 90);
        public Color BgColor2 = Color.FromArgb(255, 52, 52, 52);
        public Color ItemColor = Color.FromArgb(255, 250, 250, 250);
        public Color ItemHover = Color.FromArgb(255, 0, 71, 171);
        public Color ItemSelect = Color.FromArgb(255, 0, 57, 139);

        public void DrawRoundedRectangle(Graphics objGraphics, int intxAxis, int intyAxi,
                                         int intWidth, int intHeight, int diameter, Color color)
        {
            var pen = new Pen(color);
            var baseRect = new RectangleF(intxAxis, intyAxi, intWidth, intHeight);
            var arcRect = new RectangleF(baseRect.Location, new SizeF(diameter, diameter));

            objGraphics.DrawArc(pen, arcRect, 180, 90);
            objGraphics.DrawLine(pen, intxAxis + (diameter/2), intyAxi, intxAxis + intWidth - (diameter/2), intyAxi);

            arcRect.X = baseRect.Right - diameter;
            objGraphics.DrawArc(pen, arcRect, 270, 90);
            objGraphics.DrawLine(pen, intxAxis + intWidth, intyAxi + (diameter/2), intxAxis + intWidth,
                                 intyAxi + intHeight - (diameter/2));

            arcRect.Y = baseRect.Bottom - diameter;
            objGraphics.DrawArc(pen, arcRect, 0, 90);
            objGraphics.DrawLine(pen, intxAxis + (diameter/2), intyAxi + intHeight, intxAxis + intWidth - (diameter/2),
                                 intyAxi + intHeight);

            arcRect.X = baseRect.Left;
            objGraphics.DrawArc(pen, arcRect, 90, 90);
            objGraphics.DrawLine(pen, intxAxis, intyAxi + (diameter/2), intxAxis, intyAxi + intHeight - (diameter/2));
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            base.OnRenderToolStripBackground(e);

            var b = new System.Drawing.Drawing2D.LinearGradientBrush(e.AffectedBounds, BgColor1, BgColor2,
                                                                     System.Drawing.Drawing2D.LinearGradientMode.
                                                                         Horizontal);
            e.Graphics.FillRectangle(b, e.AffectedBounds);
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            base.OnRenderImageMargin(e);

            var rect = new Rectangle(e.AffectedBounds.Width, 2, 1, e.AffectedBounds.Height);
            var rect2 = new Rectangle(e.AffectedBounds.Width + 1, 2, 1, e.AffectedBounds.Height);
            var rect3 = new Rectangle(0, 0, e.ToolStrip.Width, e.ToolStrip.Height);
            var rect4 = new Rectangle(0, 1, e.ToolStrip.Width - 1, e.ToolStrip.Height - 2);

            e.Graphics.FillRectangle(new SolidBrush(BgColor1), rect3);
            e.Graphics.FillRectangle(new SolidBrush(BgColor1), e.AffectedBounds);
            e.Graphics.FillRectangle(new SolidBrush(BgColor1), rect);
            e.Graphics.FillRectangle(new SolidBrush(ItemColor), rect2);
            e.Graphics.DrawRectangle(new Pen(ItemColor), rect4);
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            base.OnRenderItemCheck(e);

            var rect = new Rectangle(4, 2, 18, 18);
            e.Graphics.FillRectangle(new SolidBrush(ItemColor), rect);
            DrawRoundedRectangle(e.Graphics, rect.Left - 1, rect.Top - 1, rect.Width, rect.Height + 1, 4, ItemColor);
            e.Graphics.DrawImage(e.Image, new Point(5, 3));
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            base.OnRenderSeparator(e);

            var rect = new Rectangle(32, 3, e.Item.Width - 32, 1);
            var rect2 = new Rectangle(32, 4, e.Item.Width - 32, 1);
            e.Graphics.FillRectangle(new SolidBrush(BgColor2), rect);
            e.Graphics.FillRectangle(new SolidBrush(ItemColor), rect2);
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = ItemColor;
            base.OnRenderArrow(e);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderMenuItemBackground(e);
            var item = e.Item as ToolStripMenuItem;
            if (item == null) return;
            if (item.Enabled)
            {
                if (item.Selected)
                {
                    var rect = new Rectangle(3, 2, e.Item.Width - 6, e.Item.Height - 4);
                    e.Graphics.FillRectangle(new SolidBrush(ItemHover), rect);
                    DrawRoundedRectangle(e.Graphics, rect.Left - 1, rect.Top - 1, rect.Width, rect.Height + 1, 4,
                                         ItemHover);
                }

                if (item.DropDown.Visible && !item.IsOnDropDown)
                {
                    var rect = new Rectangle(3, 2, e.Item.Width - 6, e.Item.Height - 4);
                    e.Graphics.FillRectangle(new SolidBrush(ItemSelect), rect);
                    DrawRoundedRectangle(e.Graphics, rect.Left - 1, rect.Top - 1, rect.Width, rect.Height + 1, 4,
                                         ItemSelect);
                }

                if (!item.IsOnDropDown)
                {
                    item.Text = (item.Text).ToUpper();
                }

                item.ForeColor = ItemColor;
            }
            else
            {
                item.ForeColor = BgColor1;
            }
        }
    }
}