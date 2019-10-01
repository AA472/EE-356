using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Media.Animation;

namespace HourGlass
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        bool rotated;
        int running;
        Queue<int[,]> genQ;
        int[,] gen;
        int w, h,num_sand;
        double diameter;
        static BackgroundWorker bkw1 = new BackgroundWorker();
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer dispatcherTimer2 = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Media.Pen blkPen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, 1);
        System.Windows.Media.Brush[] brushColor = new System.Windows.Media.Brush[3];
        int qLength;
        Thread t;
        SolidColorBrush grainColor;
        public MainWindow()
        {
            
            rotated = false;
            bkw1.DoWork += new DoWorkEventHandler(bkw1_DoWork);

            bkw1.RunWorkerCompleted += new
                          RunWorkerCompletedEventHandler(bkw1_RunWorkerCompleted);


            InitializeComponent();
            this.KeyDown += new KeyEventHandler(OnButtonKeyDown);
            btn_begin.IsEnabled = false;
            btn_forever.IsEnabled = false;
            btn_stop.IsEnabled = false;
            btn_rotate.IsEnabled = false;
            running = 1;
            
            getInfo();
            reset(true);
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0,0,0,0,0);

            dispatcherTimer2.Tick += dispatcherTimer2_Tick;
            dispatcherTimer2.Interval = new TimeSpan(0, 0, 0, 0, 0);

        }
        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key.ToString() == "Space")
            {
                btn_begin.IsEnabled = true;
                btn_forever.IsEnabled = true;
                btn_stop.IsEnabled = true;
                btn_rotate.IsEnabled = true;
                bkw1.RunWorkerAsync("Message to Worker");
            }

        }
        public void getInfo()
        {
            
            string userColor = cb_color.Text;
            if (userColor == "") userColor = "Turquoise";
            System.Drawing.Color aColor = System.Drawing.Color.FromName(userColor);
            SolidColorBrush brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(aColor.R, aColor.G, aColor.B));
            
            brushColor[0] = System.Windows.Media.Brushes.Black;
            brushColor[1] = brush;
            brushColor[2] = System.Windows.Media.Brushes.Tomato;


            num_sand = Convert.ToInt32(txt_num.Text);
            int a = 1; int b = 30; int c = 1 - 4 * num_sand;
            w =  (int) Math.Ceiling((Math.Sqrt(b * b - 4 * a * c) -b) / (2 * a));
            if (w % 2 == 0)
                w++;
            h = w - 1 + 16;

           
        }
        public void reset(bool draw)
        {
            
            qLength = 0;
            int test = 0;
            gen = new int[h+2,w+2];
            genQ = new Queue<int[,]>();

            for (int i = 0; i < h+2; i++)
            {
                for (int j = 0; j < w+2; j++)
                {
                    gen[i, j] = 5;
                }
            }

            //set full spots to 1
            for (int row = 1; row < 9; row++)
            {
                for (int col = 1; col <=w; col++)
                {
                    gen[row, col] = 1;
                    test++;
                }
            }
            for (int row = 9; row < 9+((w-1)/2); row++ )
            {
                for (int col = 1 + (row - 8); col < (w+1) - (row - 8); col++)
                {
                    if (test >= num_sand)
                        gen[row, col] = 0;

                    else
                    {
                        gen[row, col] = 1;
                        test++;
                    }
                }
            }
            //set empty spots to 0

            for (int row = w+9; row < h+1; row++)
            {
                for (int col = 1; col <=w; col++)
                    gen[row, col] = 0;

            }
            for (int row = (w-1)/2 + 9 ; row < w + 9; row++)
            {
                for (int col = (w + 1) / 2 - (row - ((w - 1) / 2 + 9)) ; col < (w+1)/2 + (row - ((w - 1) / 2 + 8)); col++)
                    gen[row, col] = 0;

            }

            //set whhat is not set (the walls and edges)  to 2
            for (int i=0; i< h+2; i++)
            {
                for (int j = 0; j < w+2; j++)
                {
                    if (gen[i, j] == 5 )
                        gen[i, j] = 2;
                }
            }
            if (draw)
            {
                drawCurrent(gen);
            }
        }
        public int moveAll()
        {
            int test = 0;
            for (int i = h; i > 0; i--)
                for (int j = w; j > 0; j--)
                {
                    test += move(i, j);
                }
            return test;
        }
        public int move(int row, int col)
        {
            int i = row;
            int j = col;

                Random rand = new Random();
               
                if (gen[i, j] != 1)
                    return 0;
                else if (gen[i + 1, j] == 0)
                {
                    gen[i + 1, j] = 1;
                    gen[i, j] = 0;
                return 1;
                }
                else if (gen[i + 1, j - 1] == 0 && gen[i + 1, j + 1] == 0)
                {
                    if (rand.Next(0, 2) == 0)
                    {
                        gen[i + 1, j - 1] = 1;
                        gen[i, j] = 0;
                    
                    }
                    else
                    {
                        gen[i + 1, j + 1] = 1;
                        gen[i, j] = 0;
                    
                    }
                return 1;

            }
                else if (gen[i + 1, j - 1] == 0)
                {
                    gen[i + 1, j - 1] = 1;
                    gen[i, j] = 0;
                    return 1;
            }
                else if (gen[i + 1, j + 1] == 0)
                {
                    gen[i + 1, j + 1] = 1;
                    gen[i, j] = 0;
                return 1;
            }

            return 0;
        }
        public void drawCurrent(int[,] gen)
        {
            diameter = Convert.ToDouble(txt_diameter.Text);
            double pixelW = 4, pixelH = 4;

           

            DrawingVisual vis = new DrawingVisual();
            DrawingContext dc = vis.RenderOpen();


            for (int i = 0; i < h+2; i++)
            {
                pixelW = 4;
                for (int j = 0; j < w+2; j++) {
                    if (gen[i, j] == 2)
                        dc.DrawEllipse(brushColor[0], blkPen, new System.Windows.Point(pixelW, pixelH), diameter / 2, diameter / 2);
                    else if (gen[i, j] == 1)
                        dc.DrawEllipse(brushColor[1], blkPen, new System.Windows.Point(pixelW, pixelH), diameter / 2, diameter / 2);
                     if ( i == (h+2)/2 && j== (w - 1) / 2 +1 && gen[i,j] ==1)
                    {
                        dc.DrawEllipse(brushColor[2], blkPen, new System.Windows.Point(pixelW, pixelH), diameter / 2, diameter / 2);
                    }
                     if (gen[i,j] == 1 && j == 1 + (w - 1) / 2 && i ==  (h + 2) / 2 - 1)
                    {
                        dc.DrawEllipse(brushColor[2], blkPen, new System.Windows.Point(pixelW, pixelH), diameter / 2, diameter / 2);
                    }
                    pixelW += diameter;
            }
                pixelH += diameter;
         }
            dc.Close();
            RenderTargetBitmap bmp = new RenderTargetBitmap((int) (400 * ((double)num_sand /400)) , (int) (400 * ((double)num_sand / 400)), 96, 96, PixelFormats.Pbgra32);

            bmp.Render(vis);
            img.Source = bmp;
        }     
        private void btn_begin_Click(object sender, RoutedEventArgs e)
        {
            runOnce();
            dispatcherTimer.Start();
        }
        public void runOnce()
        {
            Thread.Sleep(100);
            if (running == 0)
                running = 1;
            else
            {
                t = new Thread(compute);
                t.Start();
            }
        }
     

        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            dispatcherTimer2.Stop();
            running = 0;
        }
        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            getInfo();
            running = 1;
            reset(true);
        }
        private void btn_rotate_Click(object sender, RoutedEventArgs e)
        {
            reset(true);
        }
        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Slider_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            double value = slider.Value;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)value);
            dispatcherTimer2.Interval = new TimeSpan(0, 0, 0, 0, (int)value);

        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {

            if (qLength == 0)
            {
                dispatcherTimer.Stop();
            }
            else
            {
                int[,] k = genQ.Dequeue();
                qLength--;
                drawCurrent(k);
            }


        }
        private void dispatcherTimer2_Tick(object sender, EventArgs e)
        {
            if (qLength == 0)
            {
                dispatcherTimer2.Stop();
                reset(false);
                rotate();
                computeForever();
            }
            else
            {
                int[,] k = genQ.Dequeue();
                qLength--;
                drawCurrent(k);
            }
           

        }
        public void compute()
        {

            while (true)
            {

                if (moveAll() == 0)
                {
                    break;
                }
                int[,] gen2 = new int[h + 2, w + 2];
                for (int i = 0; i < h + 2; i++)
                {
                    for (int j = 0; j < w + 2; j++)
                    {
                        gen2[i, j] = gen[i, j];
                    }
                }
                genQ.Enqueue(gen2);
                qLength++;
            }
        }



        public void computeForever()
        {
            runOnce();
            dispatcherTimer2.Start();
        }

        private void btn_forever_Click(object sender, RoutedEventArgs e)
        {
                runOnce();
                dispatcherTimer2.Start();
        }
      


        

        public void rotate()
        {
            DoubleAnimation dblAni = new DoubleAnimation();
            if (rotated) {
                dblAni.From = 180;
                dblAni.To = 0;
                rotated = false;
                Console.WriteLine("First");
            }
            else {
                dblAni.From = 0;
                dblAni.To = 180;
                rotated = true;
                Console.WriteLine("Second");

            }

            dblAni.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            img.RenderTransformOrigin = new System.Windows.Point(.5, .5);
            RotateTransform rt = new RotateTransform();
            img.RenderTransform = rt;
            rt.BeginAnimation(RotateTransform.AngleProperty, dblAni);
        }

        void bkw1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btn_begin.IsEnabled = false;
            btn_forever.IsEnabled = false;
            btn_stop.IsEnabled = false;
            btn_rotate.IsEnabled = false;

            //pgb1.Value = 0;
        }
        void bkw1_DoWork(object sender, DoWorkEventArgs e)
        {
            int i = 1;
            for (i = 0; i < 100; i++)                  //runs Sleep(100) 100 times
            {
                if (bkw1.CancellationPending)     //See if we should cancel
                {
                    e.Cancel = true;
                    return;
                }
                Thread.Sleep(100);
                //bkw1.ReportProgress(i);         //Report progress as percent done
                //pgb1.Value = i;
            }
            e.Result = "Done";                 //Send Done note on completion
        }


    }

}
