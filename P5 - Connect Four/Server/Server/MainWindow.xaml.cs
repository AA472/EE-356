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
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.ComponentModel;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        delegate void SetTextCallback(String text);
        delegate void SetIntCallbCk(int theadnum);
        BackgroundWorker backgroundWorker1 = new BackgroundWorker();
        int[,] board;
        int turn = 1;
        int num_columns;
        int num_rows;
        int currPos = 0;
        int temp;
        Dictionary<int, string> names;
        SolidColorBrush white = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
        SolidColorBrush player1 = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
        SolidColorBrush player2 = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));

        BackgroundWorker[] bkw1 = new BackgroundWorker[5];
        Socket client;
        NetworkStream[] ns = new NetworkStream[5];
        StreamReader[] sr = new StreamReader[5];
        StreamWriter[] sw = new StreamWriter[5];
        List<int> AvailableClientNumbers = new List<int>(5);
        List<int> UsedClientNumbers = new List<int>(5);
        int clientcount = 0;
        bool gamestarted = false;
        Queue<int> test;
        public MainWindow()
        {
            test = new Queue<int>();
            num_columns = 8;
            names = new Dictionary<int, string>();
            num_rows = 6;
            board = new int[num_rows, num_columns]; // 0 for empty 1 for player1 2 for player2
            for (int i = 0; i < num_rows; i++)
                for (int j = 0; j < num_columns; j++)
                    board[i, j] = 0;
            InitializeComponent();
            drawBoard();

        }

#region server
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            String printtext;
            TcpListener newsocket = new TcpListener(IPAddress.Any, 9090);  //Create TCP Listener on server
            newsocket.Start();
            for (int i = 0; i < 5; i++)
            {
                AvailableClientNumbers.Add(i);
            }
            while (AvailableClientNumbers.Count > 0)
            {
                InsertText("waiting for client");                   //wait for connection
                printtext = "Available Clients = " + AvailableClientNumbers.Count;
                InsertText(printtext);                   //wait for connection
                client = newsocket.AcceptSocket();     //Accept Connection
                clientcount = AvailableClientNumbers.First();
                AvailableClientNumbers.Remove(clientcount);
                ns[clientcount] = new NetworkStream(client);                            //Create Network stream
                sr[clientcount] = new StreamReader(ns[clientcount]);
                sw[clientcount] = new StreamWriter(ns[clientcount]);
                string welcome = "Welcome";
                InsertText("client connected");
                sw[clientcount].WriteLine(welcome);     //Stream Reader and Writer take away some of the overhead of keeping track of Message size.  By Default WriteLine and ReadLine use Line Feed to delimit the messages
                sw[clientcount].Flush();
                bkw1[clientcount] = new BackgroundWorker();
                bkw1[clientcount].DoWork += new DoWorkEventHandler(client_DoWork);
                bkw1[clientcount].RunWorkerAsync(clientcount);
                UsedClientNumbers.Add(clientcount);
                

                if(UsedClientNumbers.Count>=2 && !gamestarted)
                {
                    sw[0].WriteLine("Player:1");
                    sw[0].Flush();
                    sw[1].WriteLine("Player:2");
                    sw[1].Flush();
                    gamestarted = true;
                }
            }
        }
        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        private void sendMove(int col)
        {
            //check if its their turn

            //find which row the ellipse should fall into

            int i;
            for (i = num_rows-1; i >= 0; i--)
            {
                if (board[i, col] == 0)
                {
                    board[i, col] = turn;
                    break;
                }

                else continue;
            }
            int passed = 0;
            int winner;
            if (win(i, col, turn))
            {
                if (passed == 0)
                    passed = 1;
                else
                {
                    return;
                    passed = 0;
            }


                if (turn == 2) {
                    temp = 0;
                }
                if (turn == 1)
                {
                    temp = 1;
                    
                }

                int temp2 = UsedClientNumbers[temp];
                UsedClientNumbers.Remove(temp2);
                UsedClientNumbers.Add(temp2);
                foreach (int t in UsedClientNumbers)
                {
                    sw[t].WriteLine("clear:" + turn);
                    sw[t].Flush();
                    if (t > 1)
                    {
                        sw[t].WriteLine("You are on the waitlist to play Connect four!\nYou are number " +
                            (t) + " on the waitlist.\nEstimated wait time is: " + ((t) * 2) + " minutes.");
                        sw[t].Flush();
                        waitlist.Items.Insert(0, names[t]);
                        sw[t].WriteLine("wait:" + names[t]);
                        sw[t].Flush();
                    }
                }
                sw[UsedClientNumbers[0]].WriteLine("Player:1");
                sw[UsedClientNumbers[0]].Flush();
                sw[UsedClientNumbers[1]].WriteLine("Player:2");
                sw[UsedClientNumbers[1]].Flush();
                
                for (int p = 0; p < num_rows; p++)
                    for (int j = 0; j < num_columns; j++)
                        board[p, j] = 0;
                return;
            }
            turn = turn % 2 + 1;
            // send a message to the clients

            foreach (int t in UsedClientNumbers)
            {
                sw[t].WriteLine("move:" + i+","+col);
                sw[t].Flush();
            }

        }
        private void client_DoWork(object sender, DoWorkEventArgs e)
        {
            int clientnum = (int)e.Argument;
            Console.WriteLine(clientnum);
            bkw1[clientnum].WorkerSupportsCancellation = true; ;

            while (true)
            {
                string inputStream;
                try
                {
                    inputStream = sr[clientnum].ReadLine();
                    if (inputStream.Split(':')[0].Equals("move"))
                    {
                        //if ((turn == 1 && clientnum == 0) || (turn == 2 && clientnum == 1))
                        // {
                        int col = Convert.ToInt16(inputStream.Split(':')[1]);
                        Dispatcher.Invoke(new Action(() => { sendMove(col); }));
                        //}

                    }
                    else if (inputStream.Split(':')[0].Equals("name"))
                    {
                        //if ((turn == 1 && clientnum == 0) || (turn == 2 && clientnum == 1))
                        // {
                        string name = inputStream.Split(':')[1];
                        Dispatcher.Invoke(new Action(() =>
                        {
                            if (!names.ContainsKey(clientnum))
                                names.Add(clientnum, name);




                        }));
                        //}

                    }
                    else if (inputStream.Split(':')[0].Equals("cur"))
                    {
                        //// if ((turn == 1 && clientnum == 0) || (turn == 2 && clientnum == 1))
                        {
                            currPos = Convert.ToInt16(inputStream.Split(':')[1]);

                            foreach (int t in UsedClientNumbers)
                            {
                                sw[t].WriteLine("cur:" + currPos);
                                sw[t].Flush();

                            }
                        }
                    }
                    else
                    {
                        if (inputStream == "disconnect")
                        {
                            sr[clientnum].Close();
                            sw[clientnum].Close();
                            ns[clientnum].Close();
                            InsertText("Client " + clientnum + " has disconnected");
                            KillMe(clientnum);
                            break;
                        }

                        InsertText(inputStream);
                        foreach (int t in UsedClientNumbers)
                        {
                            sw[t].WriteLine(inputStream);
                            sw[t].Flush();
                        }

                    }
                }

                catch
                {
                    sr[clientnum].Close();
                    sw[clientnum].Close();
                    ns[clientnum].Close();
                    InsertText("Client " + clientnum + " has disconnected");
                    KillMe(clientnum);
                    break;
                }
            }
        }

       
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            foreach (int t in UsedClientNumbers)
            {
                sw[t].WriteLine(textBox1.Text);
                sw[t].Flush();
            }
            textBox1.Text = "";
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

            // send message to other clients
             //foreach (int t in UsedClientNumbers)
             //{
             //  sw[t].WriteLine(text);
             //  sw[t].Flush();
             //}
        }
        private void KillMe(int threadnum)
        {
            if (this.listBox1.Dispatcher.CheckAccess())
            {
                UsedClientNumbers.Remove(threadnum);
                AvailableClientNumbers.Add(threadnum);
                bkw1[threadnum].CancelAsync();
                bkw1[threadnum].Dispose();
                bkw1[threadnum] = null;
                GC.Collect();

            }
            else
            {
                listBox1.Dispatcher.BeginInvoke(new SetIntCallbCk(KillMe), threadnum);
            }

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerAsync("Message to Worker");
            btn_start.IsEnabled = false;
        }
        
        #endregion
        #region game
        public void drawBoard()
        {


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
                }
            }
        }
        public bool win(int x, int y, int turn)
        {
            if (checkLine(x, y, 0, 1, turn)) return true; // check up and down
            if (checkLine(x, y, 1, 0, turn)) return true; // check right and left
            if (checkLine(x, y, 1, -1, turn)) return true; // check  45 degrees
            if (checkLine(x, y, 1, 1, turn)) return true; // check -45 degrees


            return false;
        }
        public bool checkLine(int x, int y, int xChange, int yChange, int turn)
        {
            int count, tempX = x, tempY = y;
            //check up
            count = 1;
            while (true)
            {
                tempX += xChange;
                tempY += yChange;
                if (inBounds(tempX, tempY) == false) break;
                else if (board[tempX, tempY] == turn) count++;
                else break;
            }
            //check down
            tempY = y; tempX = x;
            while (true)
            {
                tempX -= xChange;
                tempY -= yChange;
                if (inBounds(tempX, tempY) == false) break;
                else if (board[tempX, tempY] == turn) count++;
                else break;
            }

            if (count >= 4)
                return true;
            return false;
        }
        public bool inBounds(int x, int y)
        {
            if (!(x >= 0 && x <= num_rows - 1))
                return false;
            if (!(y >= 0 && y <= num_columns - 1))
                return false;
            return true;
        }

        
    }
    #endregion
    }

