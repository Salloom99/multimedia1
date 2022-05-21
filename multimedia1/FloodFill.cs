using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace multimedia1
{
    public delegate void UpdateScreenDelegate(ref int x, ref int y);

    internal class FloodFill
    {
        protected CustomBitmap bitmap;
        protected byte[] tolerance = new byte[] { 25, 25, 25 };
        protected Color fillColor = Color.Magenta;

        //cached bitmap properties
        protected int bitmapWidth = 0;
        protected int bitmapHeight = 0;
        protected int bitmapStride = 0;
        protected int bitmapPixelFormatSize = 0;
        protected byte[] bitmapBits = null;

        FloodFillQueue ranges = new FloodFillQueue();


        //internal int timeBenchmark = 0;
        internal UpdateScreenDelegate UpdateScreen;

        //internal, initialized per fill
        //protected BitArray pixelsChecked;
        protected bool[] pixelsChecked;
        protected byte[] byteFillColor;
        protected byte[] startColor;
        //protected int stride;

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
            //cache data in member variables to decrease overhead of property calls
            //this is especially important with Width and Height, as they call
            //GdipGetImageWidth() and GdipGetImageHeight() respectively in gdiplus.dll - 
            //which means major overhead.
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


            //***Prepare for fill.
            PrepareForFloodFill(pt);

            ranges = new FloodFillQueue(((bitmapWidth + bitmapHeight) / 2) * 5);//new Queue<FloodFillRange>();

            //***Get starting color.
            int x = pt.X; int y = pt.Y;
            int idx = CoordsToByteIndex(ref x, ref y);
            try
            {
                startColor = new byte[] { bitmap.Bits[idx], bitmap.Bits[idx + 1], bitmap.Bits[idx + 2] };
            }
            catch (IndexOutOfRangeException ex) { 
            }
            bool[] pixelsChecked = this.pixelsChecked;

            //***Do first call to floodfill.
            LinearFill(ref x, ref y);

            //***Call floodfill routine while floodfill ranges still exist on the queue
            while (ranges.Count > 0)
            {
                //**Get Next Range Off the Queue
                FloodFillRange range = ranges.Dequeue();

                //**Check Above and Below Each Pixel in the Floodfill Range
                int downPxIdx = (bitmapWidth * (range.Y + 1)) + range.StartX;//CoordsToPixelIndex(lFillLoc,y+1);
                int upPxIdx = (bitmapWidth * (range.Y - 1)) + range.StartX;//CoordsToPixelIndex(lFillLoc, y - 1);
                int upY = range.Y - 1;//so we can pass the y coord by ref
                int downY = range.Y + 1;
                int tempIdx;
                for (int i = range.StartX; i <= range.EndX; i++)
                {
                    //*Start Fill Upwards
                    //if we're not above the top of the bitmap and the pixel above this one is within the color tolerance
                    tempIdx = CoordsToByteIndex(ref i, ref upY);
                    if (range.Y > 0 && (!pixelsChecked[upPxIdx]) && CheckPixel(ref tempIdx))
                        LinearFill(ref i, ref upY);

                    //*Start Fill Downwards
                    //if we're not below the bottom of the bitmap and the pixel below this one is within the color tolerance
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

            //cache some bitmap and fill info in local variables for a little extra speed
            byte[] bitmapBits = this.bitmapBits;
            bool[] pixelsChecked = this.pixelsChecked;
            byte[] byteFillColor = this.byteFillColor;
            int bitmapPixelFormatSize = this.bitmapPixelFormatSize;
            int bitmapWidth = this.bitmapWidth;

            //***Find Left Edge of Color Area
            int lFillLoc = x; //the location to check/fill on the left
            int idx = CoordsToByteIndex(ref x, ref y); //the byte index of the current location
            int pxIdx = (bitmapWidth * y) + x;//CoordsToPixelIndex(x,y);
            while (true)
            {
                //**fill with the color
                bitmapBits[idx] = byteFillColor[0];
                bitmapBits[idx + 1] = byteFillColor[1];
                bitmapBits[idx + 2] = byteFillColor[2];
                //**indicate that this pixel has already been checked and filled
                pixelsChecked[pxIdx] = true;
                //**screen update for 'slow' fill

                //**de-increment
                lFillLoc--;     //de-increment counter
                pxIdx--;        //de-increment pixel index
                idx -= bitmapPixelFormatSize;//de-increment byte index
                //**exit loop if we're at edge of bitmap or color area
                if (lFillLoc <= 0 || (pixelsChecked[pxIdx]) || !CheckPixel(ref idx))
                    break;

            }
            lFillLoc++;

            //***Find Right Edge of Color Area
            int rFillLoc = x; //the location to check/fill on the left
            idx = CoordsToByteIndex(ref x, ref y);
            pxIdx = (bitmapWidth * y) + x;
            while (true)
            {
                //**fill with the color
                bitmapBits[idx] = byteFillColor[0];
                bitmapBits[idx + 1] = byteFillColor[1];
                bitmapBits[idx + 2] = byteFillColor[2];
                //**indicate that this pixel has already been checked and filled
                pixelsChecked[pxIdx] = true;
                //**screen update for 'slow' fill

                //**increment
                rFillLoc++;     //increment counter
                pxIdx++;        //increment pixel index
                idx += bitmapPixelFormatSize;//increment byte index
                //**exit loop if we're at edge of bitmap or color area
                if (rFillLoc >= bitmapWidth || pixelsChecked[pxIdx] || !CheckPixel(ref idx))
                    break;

            }
            rFillLoc--;

            //add range to queue
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
