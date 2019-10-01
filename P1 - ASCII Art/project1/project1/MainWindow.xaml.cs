using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
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
namespace project1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<double, char> letters;
        WriteableBitmap wb_inserted;
        WriteableBitmap wb_converted;
        int letter_width;
        int letter_height;
        int y1 = 100000, y2 = 0;
        int font_size = 6;
        string font_type;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void mnuLoad_Click(object sender, RoutedEventArgs e)
        {
            string fName;
            //Add Using Microsoft.Win32 for the OpenFileDialog
            OpenFileDialog fileChooser = new OpenFileDialog();
            fileChooser.Filter = "bitmap files (*.bmp)|*.bmp|All files|*.*";
            if (fileChooser.ShowDialog() == false)
                return;
            fName = fileChooser.FileName;
            BitmapImage bi = new BitmapImage(new Uri(fName));
            wb_inserted = new WriteableBitmap(bi);
            img_original.Source = wb_inserted;
        }



        private void btn_convert_Click(object sender, RoutedEventArgs e)
        {
            FormattedText fmtxt;
            DrawingVisual vis = new DrawingVisual();
            DrawingContext dc = vis.RenderOpen();
            getDictionary2();

            RenderTargetBitmap bmp = new RenderTargetBitmap(wb_inserted.PixelWidth, wb_inserted.PixelHeight, 96, 96, PixelFormats.Pbgra32);
            char letter;
            for (int x = 0; x <= (wb_inserted.PixelWidth) - letter_width; x += 6 )
                for (int y = 0; y <= (wb_inserted.PixelHeight) - letter_height; y += 6 )
                {
                    letter = GetLetter(wb_inserted, x, y);
                    fmtxt = GetFormattedText(letter.ToString(), font_size + 5);
                    dc.DrawText(fmtxt, new System.Windows.Point(x, y));
                }

            dc.Close();
            bmp.Render(vis);
            wb_converted = new WriteableBitmap(bmp);
            img_original.Source = wb_converted;
        }


        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void SaveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {

        }
        private void mnuSave_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            Nullable<bool> result = dlg.ShowDialog();
            string filename = dlg.FileName + ".png";
            

            var encoder = new PngBitmapEncoder();
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)img_original.Source.Width, (int)img_original.Source.Width, 96, 96, PixelFormats.Pbgra32);
        

            bitmap.Render(img_original);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);

            using (var stream = File.Create(filename))
            {
                encoder.Save(stream);
            }
        }


        private FormattedText GetFormattedText(string sTmp, int typeSize)
        {
            FormattedText fmtxt;
            fmtxt = new FormattedText(sTmp, System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface(font_type), typeSize,
                System.Windows.Media.Brushes.Black);

            return fmtxt;
        }
        
        public System.Windows.Media.Color GetPixel(WriteableBitmap wbm, int x, int y)
        {
            if (y > wbm.PixelHeight - 1 || x > wbm.PixelWidth - 1)
                return System.Windows.Media.Color.FromArgb(0, 0, 0, 0);
            if (y < 0 || x < 0)
                return System.Windows.Media.Color.FromArgb(0, 0, 0, 0);
            PixelFormat pfmt = wbm.Format;
            if (!(pfmt.Equals(PixelFormats.Bgr32) || pfmt.Equals(PixelFormats.Bgra32) || pfmt.Equals(PixelFormats.Indexed1) || pfmt.Equals(PixelFormats.Pbgra32) || pfmt.Equals(PixelFormats.Default)))
            {
                return System.Windows.Media.Color.FromArgb(0, 0, 0, 0);
            }
            IntPtr buff = wbm.BackBuffer;
            int Stride = wbm.BackBufferStride;
            System.Windows.Media.Color c;
            unsafe
            {
                byte* pbuff = (byte*)buff.ToPointer();
                int loc = y * Stride + x * 4;
                c = System.Windows.Media.Color.FromArgb(pbuff[loc + 3], pbuff[loc + 2],
                                       pbuff[loc + 1], pbuff[loc]);
            }
            return c;
        }
        
        public char GetLetter(WriteableBitmap wbm, int x, int y)
        {

            int sumR = 0, sumG = 0, sumB = 0;
            System.Windows.Media.Color c;
            char ch = '"';
            double average = 0;
            for (int col = x; col < x + letter_width; col++)
            {
                for (int row = y; row < y + letter_height; row++)
                {
                    if (!(y > wbm.PixelHeight - 1 || x > wbm.PixelWidth - 1))
                    {
                        c = GetPixel(wbm, col, row);
                        //average += c.R * 30 / 100;
                        //average += c.G * 59 / 100;
                        //average += c.B * 11 / 100;
                        sumR += c.R;
                        sumG += c.G;
                        sumB += c.B;
                    }
                }
            }
            double temp = (sumB+sumG+sumB)/ (double) (letter_height * letter_width*3);
            //double temp = average / (double)(letter_height * letter_width);
            double weight = 100000;
            foreach (double w in letters.Keys)
            {
                if (Math.Abs(w - temp) < Math.Abs(weight - temp))
                    weight = w;
            }
            ch = letters[weight];

            return ch;
        }
        private void getDictionary2()
        {
            letters = new Dictionary<double, char>();
            string symbols = "_ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&* .,:(){}[]:\";~`";
            font_type = cb_type.Text;
            font_size = Convert.ToInt32(cb_size.Text);
            System.Drawing.Pen black = new System.Drawing.Pen(System.Drawing.Brushes.Black, 1);
            FormattedText fmtxt;
            System.Windows.Media.Color c;
            int xmax = 0, xmin = 40, ymax = 0, ymin = 40;
            int[] blackpoints = new int[100];

            for (int i = 0; i < symbols.Length; i++)
            {
                blackpoints[i] = 0;
                DrawingVisual vis = new DrawingVisual();
                DrawingContext dc = vis.RenderOpen();
                RenderTargetBitmap bmp = new RenderTargetBitmap(40, 40, 96, 96, PixelFormats.Pbgra32);
                fmtxt = GetFormattedText(symbols[i].ToString(), font_size);
                dc.DrawText(fmtxt, new System.Windows.Point(0, 0));
                dc.Close();
                bmp.Render(vis);
                WriteableBitmap temp = new WriteableBitmap(bmp);



                for (int x = 0; x < 40; x++)
                {
                    for (int y = 0; y < 40; y++)
                    {
                        c = GetPixel(temp, x, y);
                        if (c.A > 220)
                        {
                            blackpoints[i]++;
                            if (x > xmax)
                                xmax = x;

                            if (x < xmin)
                                xmin = x;
                            if (y > ymax)
                                ymax = x;
                            if (y < ymin)
                                ymin = x;
                        }
                    }
                }


            }

            for (int i = 0; i < symbols.Length; i++)
            {
                letter_width = xmax - xmin;
                letter_height = ymax - ymin;
                double weight = blackpoints[i] / (double)(letter_height * letter_width);
                weight *= 255;
                if (!letters.ContainsKey(weight))
                    letters.Add(weight, symbols[i]);
            }
            for (int i = 220; i < 255; i++)
                letters[i] = ' ';
          
        }
        }
    }

/*       private void getDictionary()
               {
                   letters = new Dictionary<double, char>();

                   string symbols = "_ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&* .";

                   font_type = cb_type.Text;
                   string type = cb_type.Text;
                   string size = cb_size.Text;
                   font_size = Convert.ToInt32(size);
                   string fname = type + size + ".bmp";
                   int sumR = 0, sumG = 0, sumB = 0;


                   string root = new FileInfo(Assembly.GetExecutingAssembly().Location).FullName;
                   string path = root.Substring(0, root.Length - 22) + "Resources\\" + fname;
                   Assembly _assembly = Assembly.GetExecutingAssembly();
                   BitmapImage bi = new BitmapImage(new Uri(path));
                   WriteableBitmap wbm = new WriteableBitmap(bi);
                   System.Windows.Media.Color c;



                   if (type == "Consolas")
                   {
                       if (size == "5")
                       {
                           y1 = 4;
                           y2 = 12;
                           letter_width = 6;
                           font_size = 5;
                       }
                       else if (size == "6")
                       {
                           y1 = 5;
                           y2 = 14;
                           letter_width = 7;
                           font_size = 6;
                       }
                       else
                       {
                           y1 = 8;
                           y2 = 29;
                           letter_width = 13;
                           font_size = 12;
                       }
                   }
                   else if (type == "Courier New")
                   {
                       if (size == "5")
                       {
                           y1 = 9;
                           y2 = 17;
                           letter_width = 7;
                           font_size = 5;
                       }
                       else if (size == "6")
                       {
                           y1 = 4;
                           y2 = 10;
                           letter_width = 8;
                           font_size = 6;
                       }
                       else
                       {
                           y1 = 7;
                           y2 = 29;
                           letter_width = 15;
                           font_size = 12;

                       }
                   }
                   else if (type == "Lucida Console")
                   {
                       if (size == "5")
                       {
                           y1 = 3;
                           y2 = 11;
                           letter_width = 6;
                           font_size = 5;
                       }
                       if (size == "6")
                       {
                           y1 = 11;
                           y2 = 20;
                           letter_width = 7;
                           font_size = 6;

                       }
                       else
                       {
                           y1 = 7;
                           y2 = 26;
                           letter_width = 14;
                           font_size = 12;
                       }
                   }
                   int r, b, g;
                   letter_height = y2 - y1;
                   int test;
                   for (int i = 0; i < symbols.Length; i++)
                   {
                       sumR = 0; sumG = 0; sumB = 0;
                       for (int x = 1 + ((i * letter_width)); x < ((i + 1) * letter_width + 1); x++)
                       {
                           for (int y = y1; y < y1 + letter_height; y++)
                           {
                               if (!(y > wbm.PixelHeight - 1 || x > wbm.PixelWidth - 1))
                               {
                                   c = GetPixel(wbm, x, y);
                                   if (c.R < 180)
                                       r = 0;
                                   else r = c.R;
                                   sumR += r;

                                   if (c.G < 180)
                                       g = 0;
                                   else g = c.G;
                                   sumG += g;

                                   if (c.B < 180)
                                       b = 0;
                                   else b = c.B;
                                   sumB += b;


                               }
                           }
                       }

                       //   double weight = ((sumR + sumG + sumB) / (double)(256 * letter_width * (letter_height+1) * 3));
                       double weight = (sumR + sumG + sumB) / (double)(letter_width * letter_height * 3);
                       //double weight = test / (double)(letter_height * letter_height);
                       if (!letters.ContainsKey(weight))
                           letters.Add(weight, symbols[i]);
                   }
                   letters[255] = ' ';
                   foreach (double w in letters.Keys)
                   {
                       //Console.WriteLine(w + "   " + letters[w]);
                   }
               }
               */
