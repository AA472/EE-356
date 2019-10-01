using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Net.Sockets;   //include sockets class
using System.Net;  //needed for type IPAddress
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace AA_Project2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NetworkStream ns;
        StreamReader sr;
        StreamWriter sw;
        int you = 0;
        delegate void SetTextCallback(String text);
        BackgroundWorker backgroundWorker1 = new BackgroundWorker();
        int[,] board;
        bool yourTurn = false;
        int curscolor = 1;
        int num_columns;
        int num_rows;
        int currPos = 0;
        SolidColorBrush white = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
        SolidColorBrush player1 = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
        SolidColorBrush player2 = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));

        public MainWindow()
        {
            InitializeComponent();

            num_columns = 8;
            num_rows = 6;
            board = new int[num_rows, num_columns]; // 0 for empty 1 for player1 2 for player2
            for (int i = 0; i < num_rows; i++)
                for (int j = 0; j < num_columns; j++)
                    board[i, j] = 0;
            drawBoard();

            e1.Fill = player1;
            e2.Fill = player2;
            curr.Fill = player1;
            left.IsEnabled = false;
            right.IsEnabled = false;
            drop.IsEnabled = false;
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            //backgroundWorker1.RunWorkerAsync("Message to Worker");

        }
        private void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            sw.WriteLine(lbl_username.Content + ">> " + textBox1.Text);
            sw.Flush();
            if (textBox1.Text == "disconnect")
            {
                sw.Close();
                sr.Close();
                ns.Close();
                System.Environment.Exit(System.Environment.ExitCode); //close all 

            }
            textBox1.Text = "";

        }

        private void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TcpClient newcon = new TcpClient();
                newcon.Connect("127.0.0.1", 9090);  //IPAddress of Server
                ns = newcon.GetStream();
                sr = new StreamReader(ns);  //Stream Reader and Writer take away some of the overhead of keeping track of Message size.  By Default WriteLine and ReadLine use Line Feed to delimit the messages
                sw = new StreamWriter(ns);
                Window1 test = new Window1();
                test.ShowDialog();
                string username = test.txt_fc.Text;
                lbl_username.Content = username;
                sw.WriteLine("name:" + username);
                sw.Flush();
                backgroundWorker1.RunWorkerAsync("Message to Worker");
            }
            catch { MessageBox.Show("No available seats. Please try again later."); }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                try
                {
                    string inputStream = sr.ReadLine();  //Note Read only reads into a byte array.  Also Note that Read is a "Blocking Function"


                    if (inputStream.Split(':')[0].Equals("move"))
                    {
                        int row = Convert.ToInt16(inputStream.Split(':')[1].Split(',')[0]);
                        int col = Convert.ToInt16(inputStream.Split(':')[1].Split(',')[1]);
                        move(row, col);
                    }
                    else if (inputStream.Split(':')[0].Equals("cur"))
                    {
                        currPos = Convert.ToInt16(inputStream.Split(':')[1]);
                        Dispatcher.Invoke(new Action(() => { curr.Margin = new Thickness(currPos * 50 + 25, 50, 0, 0); }));
                    }
                    else if (inputStream.Split(':')[0].Equals("wait"))
                    {
                        string waiter = inputStream.Split(':')[1];
                        Dispatcher.Invoke(new Action(() => { lst_waiting.Items.Insert(0, waiter); }));
                    }
                    else if (inputStream.Split(':')[0].Equals("Player"))
                    {
                        InsertText(inputStream);
                        int temp = Convert.ToInt16(inputStream.Split(':')[1]);
                        Dispatcher.Invoke(new Action(() => {
                            you = temp;
                            if (temp == 1)
                            {
                                you = 1;
                                lbl_player1.Content = "You"; lbl_player2.Content = "Opponent";
                                yourTurn = true;
                                left.IsEnabled = true;
                                right.IsEnabled = true;
                                drop.IsEnabled = true;
                            }
                            if (temp == 2)
                            {
                                you = 2;
                                lbl_player2.Content = "You"; lbl_player1.Content = "Opponent";
                                yourTurn = false;
                                left.IsEnabled = false;
                                right.IsEnabled = false;
                                drop.IsEnabled = false;
                            }

                        }));

                    }
                    else if (inputStream.Split(':')[0].Equals("clear"))
                    {
                        int winner = Convert.ToInt16(inputStream.Split(':')[1]);
                        Dispatcher.Invoke(new Action(() => {
                            cnv1.Children.Clear();

                            for (int i = 0; i < num_rows; i++)
                            {
                                for (int j = 0; j < num_columns; j++)
                                {
                                    Ellipse e2 = new Ellipse();
                                    e2.Height = 40;
                                    e2.Width = 40;
                                    e2.Margin = new Thickness(50 * j + 10, 50 * i + 10, 0, 0);
                                    board[i, j] = 0;
                                     e2.Fill = white;
                                    cnv1.Children.Add(e2);
                                }
                            }
                            curr.Margin = new Thickness(25, 50, 0, 0);
                            curr.Fill = player1;
                            if (winner == you)
                                MessageBox.Show("You won! You get to play again.");
                            else if (you == 0)
                            {
                                MessageBox.Show("It's your turn to play!");
                            }
                            else
                            {
                                MessageBox.Show("You lost. Please wait for your turn if you want to play again. ");
                                you = 0;
                            }
                        }));
                    }
                    else
                    {
                        InsertText(inputStream);
                        if (inputStream == "disconnect")
                        {
                            sw.WriteLine("disconnect");
                            sw.Flush();
                            sr.Close();
                            sw.Close();
                            ns.Close();
                            System.Environment.Exit(System.Environment.ExitCode); //close all =
                            break;
                        }
                    }
                }
                catch
                {
                    ns.Close();
                    System.Environment.Exit(System.Environment.ExitCode); //close all 
                }

            }

        }


        private void InsertText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.listBox1.Dispatcher.CheckAccess())
            {
                this.listBox1.Items.Insert(0, text);

            }
            else
            {
                listBox1.Dispatcher.BeginInvoke(new SetTextCallback(InsertText), text);
            }
        }
        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sw.WriteLine("disconnect");
                sw.Flush();
                sr.Close();
                sw.Close();
                ns.Close();
                System.Environment.Exit(System.Environment.ExitCode); //close all 
            }
            catch
            {
                ns.Close();
                System.Environment.Exit(System.Environment.ExitCode); //close all 
            }

        }
        private void btn_Disconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sw.WriteLine("disconnect");
                sw.Flush();
                sr.Close();
                sw.Close();
                ns.Close();
                System.Environment.Exit(System.Environment.ExitCode); //close all 
            }
            catch
            {
                ns.Close();
                System.Environment.Exit(System.Environment.ExitCode); //close all 
            }
        }

        #region
        //game
        public void drawBoard()
        {
            cnv1.Children.Clear();
            for (int i = 0; i < num_rows; i++)
            {
                for (int j = 0; j < num_columns; j++)
                {
                    Ellipse e = new Ellipse();
                    e.Height = 40;
                    e.Width = 40;
                    e.Margin = new Thickness(50 * j + 10, 50 * i + 10, 0, 0);
                    if (board[i, j] == 0)
                        e.Fill = white;
                    else if (board[i, j] == 1)
                        e.Fill = player1;
                    else
                        e.Fill = player2;
                    cnv1.Children.Add(e);
                }
            }
        }
        public void move(int row, int col)
        {
            
            Dispatcher.Invoke(new Action(() => {
                drawBoard();
                TranslateTransform rt = new TranslateTransform();
                Ellipse ell = new Ellipse();
                ell.Height = 40;
                ell.Width = 40;
                ell.Margin = new Thickness(currPos * 50 + 10, 30, 0, 0);
                cnv1.Children.Add(ell);
                if (curscolor == 1)
                    ell.Fill = player1;
                else
                    ell.Fill = player2;

                DoubleAnimation da = new DoubleAnimation();
                da.From = 0;
                da.To = 50 * row - 20;
                ell.RenderTransform = rt;
                da.Duration = new Duration(TimeSpan.FromSeconds(2));
                rt.BeginAnimation(TranslateTransform.YProperty, da);


            //    Ellipse e = new Ellipse();
            //     e.Height = 40;
            //e.Width = 40;
            //e.Margin = new Thickness(50 * col + 10, 50 * row + 10, 0, 0);

            //if (curscolor == 1)
            //    e.Fill = player1;
            //else
            //    e.Fill = player2;
            //    cnv1.Children.Add(e);

            if (yourTurn && you != 0)
                {
                 yourTurn = false;
                 left.IsEnabled = false;
                 right.IsEnabled = false;
                 drop.IsEnabled = false;
                }
                else if (!yourTurn && you != 0)
                {
                    yourTurn = true;
                    left.IsEnabled = true;
                    right.IsEnabled = true;
                    drop.IsEnabled = true;
                }
                curscolor = curscolor % 2 + 1;
                if (curscolor == 1)
                {
                    curr.Fill = player1;
                    board[row, col] = 2;
                }
                else
                {
                    curr.Fill = player2;
                    board[row, col] = 1;
                }



            }

        ));

        }

      


            #endregion


            private void Window_KeyDown_1(object sender, KeyEventArgs e)
        {
            //   if (e.Key == Key.Right && currPos < num_columns - 1)
            //    {
            //        currPos++;
            //        sw.WriteLine("cur:" + currPos);
            //        sw.Flush();
            //    }
            //    else if (e.Key == Key.Right && currPos == num_columns - 1)
            //    {
            //        currPos = 0;
            //        sw.WriteLine("cur:" + currPos);
            //        sw.Flush();
            //    }
            //    if (e.Key == Key.Left && currPos > 0)
            //    {
            //        currPos--;
            //        sw.WriteLine("cur:" + currPos);
            //        sw.Flush();
            //    }
            //    else if (e.Key == Key.Left && currPos == 0)
            //    {
            //        currPos = num_columns - 2;
            //        sw.WriteLine("cur:" + currPos);
            //        sw.Flush();
            //    }

            //    if (e.Key == Key.Down)
            //    {
            //        sw.WriteLine("move:" + currPos);
            //        sw.Flush();
            //    }
            //
        }

        private void right_Click(object sender, RoutedEventArgs e)
        {

            if (currPos < num_columns - 1)
            {
                currPos++;
                sw.WriteLine("cur:" + currPos);
                sw.Flush();
            }
            else if (currPos == num_columns - 1)
            {
                currPos = 0;
                sw.WriteLine("cur:" + currPos);
                sw.Flush();
            }
        }

        private void left_click(object sender, RoutedEventArgs e)
        {
            if (currPos > 0)
            {
                currPos--;
                sw.WriteLine("cur:" + currPos);
                sw.Flush();
            }
            else if (currPos == 0)
            {
                currPos = num_columns - 1;
                sw.WriteLine("cur:" + currPos);
                sw.Flush();
            }
        }

        private void drop_Click(object sender, RoutedEventArgs e)
        {

            sw.WriteLine("move:" + currPos);
            sw.Flush();

        }




    }
}
