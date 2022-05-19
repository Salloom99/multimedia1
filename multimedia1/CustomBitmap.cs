using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace multimedia1
{
    internal class CustomBitmap
    {
        Bitmap bitmap;
        int stride;
        int pixelFormatSize;
        SharedPinnedByteArray byteArray;

        public int PixelFormatSize
        {
            get { return pixelFormatSize; }
        }
        public int Stride
        {
            get { return stride; }
        }

        public Bitmap Bitmap
        {
            get { return bitmap; }
            set { bitmap = value; }
        }

        public byte[] Bits
        {
            get { return byteArray.bits; }
        }

        private CustomBitmap owner;

        public CustomBitmap Owner
        {
            get { return owner; }
        }

        public IntPtr BitPtr
        {
            get
            {
                return byteArray.bitPtr;
            }
        }

        public CustomBitmap(Bitmap source, PixelFormat format)
    : this(source.Width, source.Height, format)
        {

            Graphics g = Graphics.FromImage(bitmap);
            g.DrawImageUnscaledAndClipped(source, new Rectangle(0, 0, source.Width, source.Height));
            g.Dispose();
        }

        public CustomBitmap(Bitmap source, PixelFormat format, int newWidth, int newHeight)
    : this(newWidth, newHeight, format)
        {

            Graphics g = Graphics.FromImage(bitmap);
            g.DrawImage(source, 0, 0, newWidth, newHeight);
            g.Dispose();
        }
        public CustomBitmap(Bitmap source) : this(source, source.PixelFormat) { }

        public CustomBitmap(int width, int height, PixelFormat format)
        {
            pixelFormatSize = System.Drawing.Image.GetPixelFormatSize(format) / 8;
            stride = width * pixelFormatSize;
            int padding = (stride % 4);
            stride += padding == 0 ? 0 : 4 - padding;//pad out to multiple of 4
            byteArray = new SharedPinnedByteArray(stride * height);
            bitmap = new Bitmap(width, height, stride, format, byteArray.bitPtr);
        }

        private bool disposed;
        public bool Disposed
        {
            get { return disposed; }
        }

        protected void Dispose(bool disposing)
        {
            if (disposed)
                return;

            bitmap.Dispose();
            byteArray.ReleaseReference();
            disposed = true;

            //Set managed object refs to null if explicitly disposing, so that they can be cleaned up by the GC.
            if (disposing)
            {
                owner = null;
                bitmap = null;
            }
        }

        ~CustomBitmap()
        {
            Dispose(false);
        }
    }



    internal class SharedPinnedByteArray
    {
        internal byte[] bits;
        internal GCHandle handle;
        internal IntPtr bitPtr;

        int refCount;

        public SharedPinnedByteArray(int length)
        {
            bits = new byte[length];
            // if not pinned the GC can move around the array
            handle = GCHandle.Alloc(bits, GCHandleType.Pinned);
            bitPtr = Marshal.UnsafeAddrOfPinnedArrayElement(bits, 0);
            refCount++;
        }

        internal void AddReference()
        {
            refCount++;
        }

        internal void ReleaseReference()
        {
            refCount--;
            if (refCount <= 0)
                Destroy();
        }

        bool destroyed;
        private void Destroy()
        {
            if (!destroyed)
            {
                handle.Free();
                bits = null;
                destroyed = true;
            }
        }

        ~SharedPinnedByteArray()
        {
            Destroy();
        }
    }
}
