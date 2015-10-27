﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CozyPixel.Draw
{
    public class BitmapGenerate
    {
        public static Bitmap Draw(Model.PixelMap pm)
        {
            int w = pm.PixelWidth * pm.data.Width;
            int h = pm.PixelWidth * pm.data.Height;
            if (pm.ShowGrid)
            {
                w += pm.GridWidth * (pm.data.Width + 1);
                h += pm.GridWidth * (pm.data.Height + 1);
            }
            Bitmap b = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(b))
            {

                for (int i = 0; i < pm.data.Width; ++i)
                {
                    for (int j = 0; j < pm.data.Height; ++j)
                    {
                        if(pm.ShowGrid)
                        {
                            g.FillRectangle(
                                new SolidBrush(pm.GetPixel(i, j)),
                                pm.GridWidth + i * (pm.PixelWidth + pm.GridWidth),
                                pm.GridWidth + j * (pm.PixelWidth + pm.GridWidth),
                                pm.PixelWidth,
                                pm.PixelWidth);
                        }
                        else
                        {
                            g.FillRectangle(
                                new SolidBrush(pm.GetPixel(i, j)),
                                i * pm.PixelWidth,
                                j * pm.PixelWidth,
                                pm.PixelWidth,
                                pm.PixelWidth);
                        }
                    }
                }

                if (pm.ShowGrid)
                {
                    var GridPen = new Pen(pm.GridColor, pm.GridWidth);
                    int BlockWidth = pm.PixelWidth + pm.GridWidth;

                    for (int i = 0; i < pm.data.Width; ++i)
                    {
                        g.DrawLine(GridPen, 0, i * BlockWidth, w, i * BlockWidth);
                        g.DrawLine(GridPen, i * BlockWidth, 0, i * BlockWidth, h);
                    }
                }
            }

            return b;
        }
    }
}