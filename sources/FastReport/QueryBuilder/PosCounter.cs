using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FastReport.FastQueryBuilder
{
    internal class PosCounter
    {
        private Point next;
        private int initX;
        private int initY;

        public int stepX = 10;
        public int stepY = 10;
        public int maxX = 100;
        public int maxY = 100;

        public Point Next
        {
            get
            {
                next.Y += stepY;
                next.X += stepX;

                if (next.X > maxX)
                    next.X = initX;
                if (next.Y > maxY)
                    next.Y = initY;

                return next;
            }
        }

        public PosCounter(int X, int Y)
        {
            next.X = X - stepX;
            next.Y = Y - stepY;
            initX = X;
            initY = Y;
        }
    }
}
