using System;

namespace multimedia1
{
    internal class FloodFillQueue
    {
        FloodFillRange[] array;
        int size;
        int head;

        public int Count
        {
            get { return size; }
        }

        public FloodFillQueue() : this(10000)
        {

        }

        public FloodFillQueue(int initialSize)
        {
            array = new FloodFillRange[initialSize];
            head = 0;
            size = 0;
        }

        public FloodFillRange First
        {
            get { return array[head]; }
        }

        public void Enqueue(ref FloodFillRange r)
        {
            if (size + head == array.Length)
            {
                FloodFillRange[] newArray = new FloodFillRange[2 * array.Length];
                Array.Copy(array, head, newArray, 0, size);
                array = newArray;
                head = 0;
            }
            array[head + (size++)] = r;
        }

        public FloodFillRange Dequeue()
        {
            FloodFillRange range = new FloodFillRange();
            if (size > 0)
            {
                range = array[head];
                array[head] = new FloodFillRange();
                head++;
                size--;
            }
            return range;
        }

   
    }
}
