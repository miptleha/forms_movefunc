using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            using (var g = this.CreateGraphics())
            {
                //рисуем в буфере, чтоб не было моргания
                Bitmap buffer = new Bitmap(this.Width, this.Height);
                Graphics g1 = Graphics.FromImage(buffer);
                g1.SmoothingMode = SmoothingMode.HighSpeed;

                for (double shift = 0; ; shift += .05)
                {
                    g1.Clear(this.BackColor);
                    DrawF(g1, Math.Sin, 100, 100 * Math.PI, shift); //рисуем функцию со смещением по x (эффект движения вправо)

                    try
                    {
                        g.DrawImage(buffer, new Point(0, 0));
                    }
                    catch
                    {
                        //закрыли форму
                    }
                    await Task.Delay(1); //минипауза, чтоб не блокировало интерфейс
                }
            }
        }

        //рисуем функцию во всю доступную область
        private void DrawF(Graphics g, Func<double, double> f, double XSCALE, double YSCALE, double xShift)
        {
            //масштабирование (сколько пикселей в единичке)
            //double XSCALE = 100;
            //double YSCALE = Math.PI * XSCALE;

            //положение центра
            double XCENTER = this.Width / 2;
            double YCENTER = this.Height / 2;

            //размер "бумаги"
            double XSIZE = this.Width;
            double YSIZE = this.Height;

            Pen pen = new Pen(Brushes.Black, 1);

            //рисуем оси
            g.DrawLine(pen, 0, (int)YCENTER, (int)XSIZE, (int)YCENTER);
            g.DrawLine(pen, (int)XCENTER, 0, (int)XCENTER, (int)YSIZE);
            DrawString(g, "0", (int)XCENTER, (int)YCENTER);

            //насечки на осях
            double DASH = 10; //размер насечки
            for (int i = 1; XCENTER + i * XSCALE < XSIZE; i++)
            {
                g.DrawLine(pen, (int)(XCENTER + i * XSCALE), (int)(-DASH / 2 + YCENTER), (int)(XCENTER + i * XSCALE), (int)(DASH / 2 + YCENTER));
                DrawString(g, i.ToString(), (int)(XCENTER + i * XSCALE), (int)YCENTER);
            }
            for (int i = -1; XCENTER + i * XSCALE > 0; i--)
            {
                g.DrawLine(pen, (int)(XCENTER + i * XSCALE), (int)(-DASH / 2 + YCENTER), (int)(XCENTER + i * XSCALE), (int)(DASH / 2 + YCENTER));
                DrawString(g, i.ToString(), (int)(XCENTER + i * XSCALE), (int)YCENTER);
            }
            for (int i = -1; YCENTER + i * YSCALE > 0; i--)
            {
                g.DrawLine(pen, (int)(-DASH / 2 + XCENTER), (int)(YCENTER + i * YSCALE), (int)(DASH / 2 + XCENTER), (int)(YCENTER + i * YSCALE));
                DrawString(g, (-i).ToString(), (int)XCENTER, (int)(YCENTER + i * YSCALE));
            }
            for (int i = 1; YCENTER + i * YSCALE < YSIZE; i++)
            {
                g.DrawLine(pen, (int)(-DASH / 2 + XCENTER), (int)(YCENTER + i * YSCALE), (int)(DASH / 2 + XCENTER), (int)(YCENTER + i * YSCALE));
                DrawString(g, (-i).ToString(), (int)XCENTER, (int)(YCENTER + i * YSCALE));
            }

            //рисуем функцию линиями: f(x) -> f(x + XSTEP)
            double xStart = -XCENTER / XSCALE - xShift;
            double xEnd = (XSIZE - XCENTER) / XSCALE;
            double XSTEP = .05; //шаг по x
            bool isFirst = true;
            double x1 = 0, y1 = 0; //предыдущая точка
            for (double x = xStart; x < xEnd + XSTEP; x += XSTEP)
            {
                double y = -f(x); //y нужно зеркалить (на бумаге y растет вверх, а на экране вниз)
                if (double.IsNaN(y))
                    continue;
                if (!isFirst)
                    g.DrawLine(pen, (int)((x1 + xShift) * XSCALE + XCENTER), (int)(y1 * YSCALE + YCENTER), (int)((x + xShift) * XSCALE + XCENTER), (int)(y * YSCALE + YCENTER));
                x1 = x;
                y1 = y;
                isFirst = false;
            }
        }

        //написать текст в определенном месте
        private void DrawString(Graphics g, string str, int x, int y)
        {
            SizeF a = TextRenderer.MeasureText(str, this.Font);
            TextRenderer.DrawText(g, str, this.Font, new Point(x, y), Color.Black);
        }
    }
}
