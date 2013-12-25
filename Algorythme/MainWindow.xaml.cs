using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Algorythme.MyCollections;
using System.Threading;
using System.ComponentModel;
using NAudio.Wave;

namespace Algorythme
{

    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Worker used to run algorithms asynchronously
        private readonly BackgroundWorker worker = new BackgroundWorker();

        // Graphical objects
        Canvas canvas = null;
        Slider tabSize_Slider = null;
        ComboBox cb_Algos = null;

        // Array & algorithm implementations
        private Algorithms algo;

        // Add bars dispatcher
        private DispatcherOperation dsp;

        // Sound utilities
        private WaveOut waveOut;
        private SineWaveProvider32 sineWaveProvider = new SineWaveProvider32();

        public void StartStopSineWave()
        {
            if (waveOut == null)
            {
                sineWaveProvider.SetWaveFormat(16000, 1); // 16kHz mono
                sineWaveProvider.Frequency = 0;
                sineWaveProvider.Amplitude = 0.1f;
                waveOut = new WaveOut();
                waveOut.Init(sineWaveProvider);
                waveOut.Play();
            }
            else
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            algo = new Algorithms(10,10);

            EventListener tabListener = new EventListener(algo.Tab);
            algo.Tab.Changed += Tab_Changed;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("Finished sorting");
            tabSize_Slider.IsEnabled = true;
            cb_Algos.IsEnabled = true;
            algo.CurrentBar = -1;
            addBars(canvas);
            StartStopSineWave();
        }
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {

            switch (algo.CurrentAlgorithm)
            {
                case "Quick Sort":
                    algo.quicksort(0, algo.TabSize - 1);
                    break;
                case "Bubble Sort":
                    algo.bubbleSort();
                    break;
                case "Cocktail Sort":
                    algo.cocktailSort();
                    break;
                case "Insertion Sort":
                    algo.insertionSort();
                    break;
                case "Selection Sort":
                    algo.selectionSort();
                    break;
                case "Heap Sort":
                    algo.heapSort();
                    break;
                case "Shell Sort":
                    algo.shellSort();
                    break;
                case "Merge Sort":
                    algo.mergeSort(0, algo.TabSize);
                    break;
                case "Bogo Sort":
                    algo.bogoSort();
                    break;
            }
        }
        
        void Tab_Changed(object sender, EventArgs e)
        {
            var args = e as ValueChangedEventArgs;
            if (args != null)
            {
                if (canvas != null)
                {
                    addBars(canvas);
                    sineWaveProvider.Frequency = (int)(((double)args.NewIndex+1)/(double)algo.TabSize * 1000) + 450;
                }
            }
        }
        private void cbAlgos_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<String>();

            for (int i = 0; i < algo.AlgorithmList.Length; i++)
            {
                data.Add(algo.AlgorithmList[i]);
            }

            var cbAlgos = sender as ComboBox;
            cb_Algos = cbAlgos;

            cbAlgos.ItemsSource = data;

            cbAlgos.SelectedIndex = 0;
        }

        private void cbAlgos_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;

            string text = cmb.SelectedItem as string;

            algo.CurrentAlgorithm = text;

        }

        private void canvas_Loaded(object sender, RoutedEventArgs e)
        {
            this.canvas = sender as Canvas;
        }
        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var canvas = sender as Canvas;

            addBars(canvas);
        }

        private void addBars(Canvas canvas)
        {
            if (dsp == null || dsp.Status == DispatcherOperationStatus.Completed)
            {
                dsp = Dispatcher.BeginInvoke((Action)(() =>
                {
                    int numBars = algo.TabSize;

                    double actualWidth = canvas.ActualWidth;

                    double rawBarWidth = (actualWidth / numBars);

                    double separator = 0.5 * rawBarWidth;

                    double actualBarWidth = rawBarWidth - separator;

                    double leftShift = separator / 2;

                    canvas.Children.Clear();
                    for (int i = 0; i < numBars; i++)
                    {
                        var rect = new System.Windows.Shapes.Rectangle();
                        rect.Stroke = new SolidColorBrush(algo.CurrentBar == i ? Colors.Red : Colors.White);
                        rect.Fill = new SolidColorBrush(algo.CurrentBar == i ? Colors.Red : Colors.White);
                        rect.Width = actualBarWidth;
                        rect.Height = canvas.ActualHeight - (numBars - 1 - (int)algo.Tab[i]) * (canvas.ActualHeight / (numBars));

                        Canvas.SetLeft(rect, leftShift + (i * actualBarWidth) + (i * separator));
                        canvas.Children.Add(rect);
                    }
                }));
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            algo.Tab.Changed -= Tab_Changed;
            algo.shuffleTab();
            algo.Tab.Changed += Tab_Changed;
            addBars(canvas);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync();
                tabSize_Slider.IsEnabled = false;
                cb_Algos.IsEnabled = false;
                
                StartStopSineWave();
            }
        }

        private void delaySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            double value = slider.Value;

            if (algo != null) algo.Delay = (int)value;
        }

        private void tabSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            if (!worker.IsBusy)
            {
                if (algo != null)
                {
                    algo.TabSize = (int)slider.Value;
                    algo.changeTabSize();
                    addBars(canvas);
                }
            }
        }

        private void tabSizeSlider_Loaded(object sender, RoutedEventArgs e)
        {
            this.tabSize_Slider = sender as Slider;
        }
    }

}
