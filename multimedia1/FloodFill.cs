using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace multimedia1
{

    internal class FloodFill
    {
        protected CustomBitmap bitmap;
        protected byte[] tolerance = new byte[] { 25, 25, 25 };
        protected Color fillColor = Color.Gray;

        protected int bitmapWidth = 0;
        protected int bitmapHeight = 0;
        protected int bitmapStride = 0;
        protected int bitmapPixelFormatSize = 0;
        protected byte[] bitmapBits = null;

        FloodFillQueue ranges = new FloodFillQueue();


        protected bool[] pixelsChecked;
        protected byte[] byteFillColor;
        protected byte[] startColor;

        public FloodFill(FloodFill configSource)
        {
            if (configSource != null)
            {
                this.Bitmap = configSource.Bitmap;
                this.FillColor = configSource.FillColor;
                this.Tolerance = configSource.Tolerance;
            }
        }

        public Color FillColor
        {
            get { return fillColor; }
            set { fillColor = value; }
        }

        public byte[] Tolerance
        {
            get { return tolerance; }
            set { tolerance = value; }
        }

        public CustomBitmap Bitmap
        {
            get { return bitmap; }
            set
            {
                bitmap = value;
            }
        }


        protected void PrepareForFloodFill(Point pt)
        {

            byteFillColor = new byte[] { fillColor.B, fillColor.G, fillColor.R };
            bitmapStride = bitmap.Stride;
            bitmapPixelFormatSize = bitmap.PixelFormatSize;
            bitmapBits = bitmap.Bits;
            bitmapWidth = bitmap.Bitmap.Width;
            bitmapHeight = bitmap.Bitmap.Height;

            pixelsChecked = new bool[bitmapBits.Length / bitmapPixelFormatSize];
        }


        public void StartFill(System.Drawing.Point pt)
        {


            PrepareForFloodFill(pt);

            ranges = new FloodFillQueue(((bitmapWidth + bitmapHeight) / 2) * 5);//new Queue<FloodFillRange>();

            int x = pt.X; int y = pt.Y;
            int idx = CoordsToByteIndex(ref x, ref y);
            startColor = new byte[] { bitmap.Bits[idx], bitmap.Bits[idx + 1], bitmap.Bits[idx + 2] };

            bool[] pixelsChecked = this.pixelsChecked;

            LinearFill(ref x, ref y);

            while (ranges.Count > 0)
            {
                FloodFillRange range = ranges.Dequeue();

                int downPxIdx = (bitmapWidth * (range.Y + 1)) + range.StartX;
                int upPxIdx = (bitmapWidth * (range.Y - 1)) + range.StartX;
                int upY = range.Y - 1;
                int downY = range.Y + 1;
                int tempIdx;
                for (int i = range.StartX; i <= range.EndX; i++)
                {

                    tempIdx = CoordsToByteIndex(ref i, ref upY);
                    if (range.Y > 0 && (!pixelsChecked[upPxIdx]) && CheckPixel(ref tempIdx))
                        LinearFill(ref i, ref upY);

                    tempIdx = CoordsToByteIndex(ref i, ref downY);
                    if (range.Y < (bitmapHeight - 1) && (!pixelsChecked[downPxIdx]) && CheckPixel(ref tempIdx))
                        LinearFill(ref i, ref downY);
                    downPxIdx++;
                    upPxIdx++;
                }

            }


        }
        void LinearFill(ref int x, ref int y)
        {

            byte[] bitmapBits = this.bitmapBits;
            bool[] pixelsChecked = this.pixelsChecked;
            byte[] byteFillColor = this.byteFillColor;
            int bitmapPixelFormatSize = this.bitmapPixelFormatSize;
            int bitmapWidth = this.bitmapWidth;

            int lFillLoc = x; 
            int idx = CoordsToByteIndex(ref x, ref y); 
            int pxIdx = (bitmapWidth * y) + x;
            while (true)
            {
                bitmapBits[idx] = byteFillColor[0];//R
                bitmapBits[idx + 1] = byteFillColor[1];//G
                bitmapBits[idx + 2] = byteFillColor[2];//B

                pixelsChecked[pxIdx] = true;

                lFillLoc--;     
                pxIdx--;        
                idx -= bitmapPixelFormatSize;
               

                if (lFillLoc <= 0 || (pixelsChecked[pxIdx]) || !CheckPixel(ref idx))
                    break;

            }
            lFillLoc++;

            int rFillLoc = x;
            idx = CoordsToByteIndex(ref x, ref y);
            pxIdx = (bitmapWidth * y) + x;
            while (true)
            {
                bitmapBits[idx] = byteFillColor[0];
                bitmapBits[idx + 1] = byteFillColor[1];
                bitmapBits[idx + 2] = byteFillColor[2];
                pixelsChecked[pxIdx] = true;

                rFillLoc++;  
                idx += bitmapPixelFormatSize;
                if (rFillLoc >= bitmapWidth || pixelsChecked[pxIdx] || !CheckPixel(ref idx))
                    break;

            }
            rFillLoc--;

            FloodFillRange r = new FloodFillRange(lFillLoc, rFillLoc, y);
            ranges.Enqueue(ref r);
        }

        protected bool CheckPixel(ref int px)
        {
            return (bitmapBits[px] >= (startColor[0] - tolerance[0])) && bitmapBits[px] <= (startColor[0] + tolerance[0]) &&
                (bitmapBits[px + 1] >= (startColor[1] - tolerance[1])) && bitmapBits[px + 1] <= (startColor[1] + tolerance[1]) &&
                (bitmapBits[px + 2] >= (startColor[2] - tolerance[2])) && bitmapBits[px + 2] <= (startColor[2] + tolerance[2]);
        }

        protected int CoordsToByteIndex(ref int x, ref int y)
        {
            return (bitmapStride * y) + (x * bitmapPixelFormatSize);
        }
        protected int CoordsToPixelIndex(int x, int y)
        {
            return (bitmapWidth * y) + x;
        }

    }
    public struct FloodFillRange
    {
        public int StartX;
        public int EndX;
        public int Y;

        public FloodFillRange(int startX, int endX, int y)
        {
            StartX = startX;
            EndX = endX;
            Y = y;
        }
    }
}
