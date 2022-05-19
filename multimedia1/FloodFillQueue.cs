using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace multimedia1
{
    internal class FloodFillQueue
    {
        FloodFillRange[] array;
        int size;
        int head;

        /// <summary>
        /// Returns the number of items currently in the queue.
        /// </summary>
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

        /// <summary>Gets the <see cref="FloodFillRange"/> at the beginning of the queue.</summary>
        public FloodFillRange First
        {
            get { return array[head]; }
        }

        /// <summary>Adds a <see cref="FloodFillRange"/> to the end of the queue.</summary>
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

        /// <summary>Removes and returns the <see cref="FloodFillRange"/> at the beginning of the queue.</summary>
        public FloodFillRange Dequeue()
        {
            FloodFillRange range = new FloodFillRange();
            if (size > 0)
            {
                range = array[head];
                array[head] = new FloodFillRange();
                head++;//advance head position
                size--;//update size to exclude dequeued item
            }
            return range;
        }

   
    }
}
