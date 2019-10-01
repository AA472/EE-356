using Microsoft.Win32;
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
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.Reflection;
using System.IO;

namespace AA_Project2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Chart chtSin = new Chart();
        private Chart chtSin2 = new Chart();
        string fPath;
        MLApp.MLApp matlab = new MLApp.MLApp();
        double fs;
        double ch;
        double totSamp;
        double dur;
        double bits;
        double[,] d;
        double start, end;
        double[,] fResp;
        double[,] freq;
        public MainWindow()
        {
           // Change to the directory where the functions are located           
            string root = new FileInfo(Assembly.GetExecutingAssembly().Location).FullName ;
            string path = root.Substring(0, root.Length - 25)+ "MLFunction";

            Console.WriteLine(path);
            string path2 = @"cd C:\Users\aa472\Desktop\MLFunction";
            matlab.Execute(path);
            InitializeComponent();
            chtSin.ChartAreas.Add("Default");
            chtSin2.ChartAreas.Add("Default");
        }
        //load a wav file
        private void mnuLoad_Click(object sender, RoutedEventArgs e)
        {

            //Add Using Microsoft.Win32 for the OpenFileDialog
            Microsoft.Win32.OpenFileDialog fileChooser = new Microsoft.Win32.OpenFileDialog();
            fileChooser.Filter = "waveform audio files (*.wav)|*.wav|All files|*.*";
            if (fileChooser.ShowDialog() == false)
                return;
            fPath = fileChooser.FileName;

            readWave();
           
        }
        //closes the window
        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void readWave()
        {
            // Matlab function use to get info about wav file
            object result1 = null;
            matlab.Feval("MLWavInfo", 5, out result1, fPath);
            object[] res1 = result1 as object[];
            fs = Math.Round((double)res1[0], 2);
            ch = Math.Round((double)res1[1], 2);
            totSamp = Math.Round((double)res1[2], 2);
            dur = Math.Round((double)res1[3], 2);
            bits = Math.Round((double)res1[4], 2);
            lbl_sf.Content = "Sample Frequency: " + fs;
            lbl_channels.Content = "Num of channels: " + ch;
            lbl_samples.Content = "Num of Samples: " + totSamp;
            lbl_bits.Content = "Bits/Sample: " + bits;
            lbl_dur.Content = "Duration: " + dur;

            //matlab function to read the amplitude
            object result = null;
            matlab.Feval("MLReadWavFile", 2, out result, fPath);
            object[] res = result as object[];
            d = (double[,])res[0];
            fs = (double)res[1];

            //matlab function to read the freq and fresp of the wav file
            object result2 = null; 
            matlab.Feval("MLFreqResp", 2, out result2, fPath);
            object[] res2 = result2 as object[];
            fResp = (double[,])res2[0];
            freq = (double[,])res2[1];
        }

        // plays the wave file
        private void playWave_Click(object sender, RoutedEventArgs e)
        {
            object p = null;
            matlab.Feval("MLPlayWavFile", 0, out p, fPath);
        }
        private void filterLow_Click(object sender, RoutedEventArgs e)
        {filter(0);}
        private void filterHigh_Click(object sender, RoutedEventArgs e) {filter(1);}

        public void filter(int x)
        {
            Window1 test = new Window1();
            test.ShowDialog();
            object dummy;
            double fc = Convert.ToDouble(test.txt_fc.Text);

            if (fc >= fs / 10 && fc <= 4 * fs / 10)
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                Nullable<bool> result = dlg.ShowDialog();
                string wavIn = fPath;
                string wavOut = dlg.FileName + ".wav";
                fPath = wavOut;
                matlab.Feval("MLFilter", 0, out dummy, wavIn, wavOut, x, fc);
                readWave();
                update_graphs();
            }

            else
            {
                System.Windows.MessageBox.Show("fc out of allowed range\nPlease use fs/10 <= fc <= 4fs/10");
            }
        }

        public void update_graphs()
        {

            // Draw the time graph
            chtSin.Series.Clear();
            System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
            host.Child = chtSin;
            this.cnvChart.Children.Add(host);
            double t, y, tIncr;
            start = Convert.ToDouble(txt_start.Text);
            end = Convert.ToDouble(txt_end.Text);
            chtSin.Width = 800;
            chtSin.Height = 700;
            chtSin.Location = new System.Drawing.Point(0, 0);
            Series sinSeries = new Series();
            sinSeries.ChartType = SeriesChartType.Line;
            double numSamples = totSamp * ((end - start) / dur);

            tIncr = dur / totSamp;
            int firstSamp = (int)(totSamp * (start / dur));
            int i = firstSamp;

            for (t = 0; t <= dur; t += tIncr)
            {
                if (i >= totSamp)
                    break;
                y = d[i, 0];
                sinSeries.Points.AddXY(t, y);
                i++;
            }

            chtSin.Series.Add(sinSeries);
            chtSin.ChartAreas[0].AxisX.Maximum = end;
            chtSin.ChartAreas[0].AxisX.Minimum = start;
            chtSin.ChartAreas[0].AxisX.Title = "Time";
            chtSin.ChartAreas[0].AxisX.LabelStyle.Format = "{0.00}";
            chtSin.ChartAreas[0].AxisY.Title = "Amplitude";
            chtSin.ChartAreas[0].AxisY.LabelStyle.Format = "{0.00}";

   

            // Draw the freq graph
            chtSin2.Series.Clear();
            System.Windows.Forms.Integration.WindowsFormsHost host2 = new System.Windows.Forms.Integration.WindowsFormsHost();
            host2.Child = chtSin2;
            this.freqChart.Children.Add(host2);


            chtSin2.Width = 800;
            chtSin2.Height = 700;
            chtSin2.Location = new System.Drawing.Point(0, 0);
            Series sinSeries2 = new Series();
            sinSeries2.ChartType = SeriesChartType.Line;

            int start_freq = Convert.ToInt32(txt_start_freq.Text);
            int end_freq = Convert.ToInt32(txt_end_freq.Text);

            int j = 0;
            for (j=0 ; j < totSamp; j++)
            {
                sinSeries2.Points.AddXY(freq[0,j], fResp[j, 0]);
                
            }

            chtSin2.Series.Add(sinSeries2);
            chtSin2.ChartAreas[0].AxisX.Maximum = end_freq;
            chtSin2.ChartAreas[0].AxisX.Minimum = start_freq;
            chtSin2.ChartAreas[0].AxisX.Title = "freq";
            chtSin2.ChartAreas[0].AxisX.LabelStyle.Format = "{0.00}";
            chtSin2.ChartAreas[0].AxisY.Title = "fResp";
            chtSin2.ChartAreas[0].AxisY.LabelStyle.Format = "{0.00}";

            System.Drawing.Color bk = System.Drawing.Color.FromArgb(Convert.ToInt32(txt_bkR.Text), Convert.ToInt32(txt_bkG.Text), Convert.ToInt32(txt_bkB.Text));
            System.Drawing.Color fr = System.Drawing.Color.FromArgb(Convert.ToInt32(txt_frR.Text), Convert.ToInt32(txt_frG.Text), Convert.ToInt32(txt_frB.Text));
            chtSin.ChartAreas[0].BackColor = bk;
            chtSin2.ChartAreas[0].BackColor = bk;

            chtSin.Series[0].Color = fr;
            chtSin2.Series[0].Color = fr;
        }

        //changes the graphs colors
        private void btn_color_Click(object sender, RoutedEventArgs e)
        {
            update_graphs();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            update_graphs();
        }
    }
}
