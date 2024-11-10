using System;
using System.Drawing;
using System.Windows.Forms;

public class MyCustomButton : Button
{
    private Image buttonImage;

    public Image ButtonImage
    {
        get { return buttonImage; }
        set
        {
            buttonImage = value;
            this.Invalidate(); 
        }
    }
    public MyCustomButton()
    {
        this.BackColor = Color.LightGray; 
        this.FlatStyle = FlatStyle.Flat;  
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        base.OnPaint(pevent);
        Graphics g = pevent.Graphics;

        using (Brush backgroundBrush = new SolidBrush(this.BackColor))
        {
            g.FillRectangle(backgroundBrush, this.ClientRectangle);
        }

        using (Pen borderPen = new Pen(Color.DarkGray, 2))
        {
            g.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);
        }

        if (buttonImage != null)
        {
            int imgX = (this.Width - buttonImage.Width) / 2;
            int imgY = (this.Height - buttonImage.Height) / 2;
            g.DrawImage(buttonImage, imgX, imgY);
        }

        TextRenderer.DrawText(g, this.Text, this.Font, this.ClientRectangle, this.ForeColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }
}
