using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Algorythme.MyCollections;

namespace Algorythme
{
    namespace MyCollections // Taken From Microsoft http://msdn.microsoft.com/en-us/library/aa645739(v=vs.71).aspx
    {
        using System.Collections;
        public class EventListener
        {
            private ListWithChangedEvent List;

            public EventListener(ListWithChangedEvent list)
            {
                List = list;
                // Add "ListChanged" to the Changed event on "List".
                List.Changed += new ChangedEventHandler(ListChanged);
            }

            // This will be called whenever the list changes.
            private void ListChanged(object sender, EventArgs e)
            {
                //Console.WriteLine("This is called when the event fires.");
            }

            public void Detach()
            {
                // Detach the event and delete the list
                List.Changed -= new ChangedEventHandler(ListChanged);
                List = null;
            }
        }

        public class ValueChangedEventArgs : EventArgs
        {
            public int NewIndex { get; set; }
            public object NewValue { get; set; }
        }

        // A delegate type for hooking up change notifications.
        public delegate void ChangedEventHandler(object sender, EventArgs e);

        // A class that works just like ArrayList, but sends event
        // notifications whenever the list changes.
        public class ListWithChangedEvent : ArrayList
        {
            // An event that clients can use to be notified whenever the
            // elements of the list change.
            public event ChangedEventHandler Changed;

            // Invoke the Changed event; called whenever list changes
            protected virtual void OnChanged(EventArgs e)
            {
                if (Changed != null)
                    Changed(this, e);
            }

            // Override some of the methods that can change the list;
            // invoke event after each
            public override int Add(object value)
            {
                int i = base.Add(value);
                OnChanged(EventArgs.Empty);
                return i;
            }

            public override void Clear()
            {
                base.Clear();
                OnChanged(EventArgs.Empty);
            }

            public override object this[int index]
            {
                set
                {
                    base[index] = value;
                    ValueChangedEventArgs args = new ValueChangedEventArgs();
                    args.NewIndex = index;
                    args.NewValue = base[index];
                    OnChanged(args);
                }
            }
        }
    }
    static class ArrayExtensions // Taken from grenade@http://stackoverflow.com/a/1262619
    {

        public static void Shuffle(this IList list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
    class Algorithms
    {

        public ListWithChangedEvent Tab { get; set; }
        public string[] AlgorithmList { get; set; }
        public string CurrentAlgorithm { get; set; }
        public int CurrentBar { get; set; }

        public int TabSize { get; set; } // Size of the sorted array
        public int Delay { get; set; }

        public Algorithms(int tabsize, int delay)
        {
            Delay = delay;
            CurrentBar = -1;
            this.TabSize = tabsize;

            AlgorithmList = new string[] {
                "Bubble Sort",
                "Cocktail Sort",
                "Selection Sort",
                "Insertion Sort",
                "Shell Sort",
                "Heap Sort",
                "Quick Sort",
                "Merge Sort",
                "Bogo Sort"
            };
            this.Tab = new ListWithChangedEvent();

            initTab();
        }

        private void initTab()
        {
            for (int i = 0; i < TabSize; i++)
            {
                Tab.Add(i);
            }
        }

        public void changeTabSize()
        {
            Tab.Clear();
            for (int i = 0; i < TabSize; i++)
            {
                Tab.Add(i);
            }
        }

        // TOOLS
        public void shuffleTab()
        {
            Tab.Shuffle();
        }
        private void SwapAndSleep(int a, int b)
        {
            CurrentBar = a;
            int tmp = (int)Tab[a];
            Tab[a] = Tab[b];
            Tab[b] = tmp;
            System.Threading.Thread.Sleep(Delay);
        }

        // SORTING ALGORITHMS
        public void bubbleSort()
        {
            int i;
            int j;

            for (i = (TabSize - 1); i >= 0; i--)
            {
                for (j = 1; j <= i; j++)
                {
                    if ((int)Tab[j - 1] > (int)Tab[j])
                    {
                        SwapAndSleep(j, j - 1);
                    }
                }
            }
        }
        public void quicksort(int left, int right)
        {
            int i = left, j = right;
            int pivot = (int)Tab[(left + right) / 2];

            while (i <= j)
            {
                while ((int)Tab[i] < pivot)
                {
                    i++;
                }

                while ((int)Tab[j] > pivot)
                {
                    j--;
                }

                if (i <= j)
                {
                    // Swap
                    SwapAndSleep(i, j);
                    i++;
                    j--;
                }
            }

            // Recursive calls
            if (left < j)
            {
                quicksort(left, j);
            }

            if (i < right)
            {
                quicksort(i, right);
            }
        }
        public void selectionSort()
        {
            int k;

            for (int i = 0; i < TabSize; i++)
            {
                k = i;
                for (int j = i + 1; j < TabSize; j++)
                {
                    if ((int)Tab[j] < (int)Tab[k])
                    {
                        k = j;
                    }
                }
                SwapAndSleep(k, i);
            }
        }
        public void insertionSort()
        {
            for (int i = 0; i < TabSize; i++)
            {
                int value = (int)Tab[i], j = i - 1;
                while (j >= 0 && (int)Tab[j] > value)
                {
                    Tab[j + 1] = Tab[j];
                    CurrentBar = j + 1;
                    System.Threading.Thread.Sleep(Delay);
                    j--;
                }
                Tab[j + 1] = value;
            }
        }
        public void heapSort()
        {
            //Build-Max-Heap
            int heapSize = TabSize;
            for (int p = (heapSize - 1) / 2; p >= 0; p--)
                maxHeapify(heapSize, p);

            for (int i = TabSize - 1; i > 0; i--)
            {
                //Swap
                SwapAndSleep(i, 0);

                heapSize--;
                maxHeapify(heapSize, 0);
            }
        }
        private void maxHeapify(int heapSize, int index)
        {
            int left = (index + 1) * 2 - 1;
            int right = (index + 1) * 2;
            int largest = 0;

            if (left < heapSize && (int)Tab[left] > (int)Tab[index])
                largest = left;
            else
                largest = index;

            if (right < heapSize && (int)Tab[right] > (int)Tab[largest])
                largest = right;

            if (largest != index)
            {
                SwapAndSleep(index, largest);

                maxHeapify(heapSize, largest);
            }
        }
        public void shellSort()
        {
            int i, j, increment, temp;
            increment = 3;
            while (increment > 0)
            {
                for (i = 0; i < TabSize; i++)
                {
                    j = i;
                    temp = (int)Tab[i];
                    while ((j >= increment) && ((int)Tab[j - increment] > temp))
                    {
                        Tab[j] = Tab[j - increment];
                        System.Threading.Thread.Sleep(Delay);
                        CurrentBar = j;
                        j = j - increment;
                    }
                    Tab[j] = temp;
                    System.Threading.Thread.Sleep(Delay);
                    CurrentBar = j;
                }
                if (increment / 2 != 0)
                    increment = increment / 2;
                else if (increment == 1)
                    increment = 0;
                else
                    increment = 1;
            }
        }
        public void mergeSort(int low, int high)
        {
            int N = high - low;
            if (N <= 1)
                return;

            int mid = low + N / 2;

            mergeSort(low, mid);
            mergeSort(mid, high);

            int[] aux = new int[N];
            int i = low, j = mid;
            for (int k = 0; k < N; k++)
            {
                if (i == mid) aux[k] = (int)Tab[j++];
                else if (j == high) aux[k] = (int)Tab[i++];
                else if ((int)Tab[i] > (int)Tab[j]) aux[k] = (int)Tab[j++];
                else aux[k] = (int)Tab[i++];
            }

            for (int k = 0; k < N; k++)
            {
                Tab[low + k] = aux[k];
                System.Threading.Thread.Sleep(Delay);
                CurrentBar = low + k;
            }
        }
        public void cocktailSort()
        {
            bool swapped;
            do
            {
                swapped = false;
                for (int i = 0; i <= TabSize - 2; i++)
                {
                    if ((int)Tab[i] > (int)Tab[i + 1])
                    {
                        //test whether the two elements are in the wrong order
                        SwapAndSleep(i + 1, i);
                        swapped = true;
                    }
                }
                if (!swapped)
                {
                    //we can exit the outer loop here if no swaps occurred.
                    break;
                }
                swapped = false;
                for (int i = TabSize - 2; i >= 0; i--)
                {
                    if ((int)Tab[i] > (int)Tab[i + 1])
                    {
                        SwapAndSleep(i, i + 1);
                        swapped = true;
                    }
                }
                //if no elements have been swapped, then the list is sorted
            } while (swapped);
        }
        public void bogoSort()
        {
            Random rnd = new Random();
            while (!isSorted()) SwapAndSleep(rnd.Next(0, TabSize), rnd.Next(0, TabSize));
        }
        private bool isSorted() // Inspired by http://stackoverflow.com/questions/18225010/c-sharp-functional-way-to-check-if-array-of-numbers-is-sequential
        {
            return Enumerable.Range(1, TabSize - 1).All(i => (int)Tab[i] - 1 == (int)Tab[i - 1]);
        }
    }
}
