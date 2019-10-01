using System;
using System.Collections;
using System.Text;
using System.Diagnostics;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Display;
using System.Drawing;
using GHIElectronics.TinyCLR.Devices.Gpio;



namespace Example1
{
    class Program
    {
        #region globals
        static Graphics screen;
        static GpioPin led = GpioController.GetDefault().OpenPin(
        GHIElectronics.TinyCLR.Pins.G400D.GpioPin.PC18);
        static double x = 75, y = 125;
        static int level=0;
        static int planeWidth = 13, planeHeight = 13;
        static int boxWidth = 50, boxHeight = 150;
        static GpioPin padUp = GpioController.GetDefault().OpenPin(
        GHIElectronics.TinyCLR.Pins.G400D.GpioPin.PA24);
        static GpioPin padDown = GpioController.GetDefault().OpenPin(
        GHIElectronics.TinyCLR.Pins.G400D.GpioPin.PA4);
        static GpioPin padRight = GpioController.GetDefault().OpenPin(
        GHIElectronics.TinyCLR.Pins.G400D.GpioPin.PD9);
        static GpioPin padLeft = GpioController.GetDefault().OpenPin(
        GHIElectronics.TinyCLR.Pins.G400D.GpioPin.PD7);
        static GpioPin padSelect = GpioController.GetDefault().OpenPin(
        GHIElectronics.TinyCLR.Pins.G400D.GpioPin.PD8);
        static Pen GreenPen = new Pen(Color.Green);
        static Pen BluePen = new Pen(Color.Blue);
        static SolidBrush blueBrush = new SolidBrush(Color.Blue);
        static SolidBrush greenBrush = new SolidBrush(Color.Green);
        static SolidBrush yellowBrush = new SolidBrush(Color.Yellow);
        static SolidBrush whiteBrush = new SolidBrush(Color.White);
        static SolidBrush redBrush = new SolidBrush(Color.Red);
        static Font font = Resource1.GetFont(Resource1.FontResources.Arial2);
        static Font font2 = Resource1.GetFont(Resource1.FontResources.Arial3);
        static DateTime diedSince = DateTime.Now;
        static bool again = false;
        static ArrayList players = new ArrayList();
        static Player key;
        static int score;
        #endregion

        static void Main()
        {
           
            TimeSpan difference = DateTime.Now - DateTime.Now;
            ArrayList points = new ArrayList();
            ArrayList avg = new ArrayList();
            Random rnd = new Random();
            Random r = new Random();
            initialScreen();
            DateTime start;
            string name;
            int offsetShifter;
            int num_remove;
            int num_rand;
            int counter;
            int moveCounter;
            bool dead;
            int offsetChange;
            double speedChange;
            int time;
            int levelIncrease;

            while (true){
                
                difference = DateTime.Now - DateTime.Now;
                offsetShifter = 0;
                num_remove = 1;
                num_rand = 4;
                counter = 1;
                moveCounter = 0;
                dead = false;
                offsetChange = 2;
                speedChange = 0;
                x = 75; y = 125;

                if (again)
                {
                    Debug.WriteLine("HERE");
                    showHighscores();
                }
                while (true) {
                   
                    if (padUp.Read() == 0)
                        break;
                    else if (padDown.Read() == 0)
                        showHighscores();
                    else if (padLeft.Read() == 0)
                        showInstructions();
                    else if (padRight.Read() == 0)
                        showAbout();
                    else
                        Thread.Sleep(100);
                }
                name = askForName();
                if (name == "")
                    name = "User";
                startGame();


                start = DateTime.Now;


                for (int i = 0; i < num_rand; i++)
                {
                    avg.Add(rnd.Next(272));
                }

                for (int i = 0; i < 12; i++)
                {
                    points.Add(100);
                }
                while (true)
                {
                    screen.Clear(Color.Black);
                    if (dead)
                        if ((DateTime.Now - diedSince).Seconds == 1)
                            break;



                    for (int i = 0; i < 12; i++)
                    {
                        screen.FillRectangle(whiteBrush, i * 50 - offsetShifter, (int)points[i] - 25, boxWidth, boxHeight);
                    }


                    if (offsetShifter > 50)
                    {
                        for (int i = 0; i < num_remove; i++)
                        {
                            points.RemoveAt(0);
                            points.Add(average(avg, num_rand));
                            avg.RemoveAt(0);
                            avg.Add(rnd.Next(272));
                        }
                        offsetShifter = 0;
                    }

                    if (dead)
                        screen.DrawImage(Resource2.GetBitmap(Resource2.BitmapResources.bird4), (int)x, (int)y);
                    else if (counter == 1)
                    {
                        screen.DrawImage(Resource2.GetBitmap(Resource2.BitmapResources.bird1), (int)x, (int)y);
                        counter = 2;
                    }
                    else if (counter == 2)
                    {
                        screen.DrawImage(Resource2.GetBitmap(Resource2.BitmapResources.bird2), (int)x, (int)y);
                        counter = 3;
                    }
                    else if (counter == 3)
                    {
                        screen.DrawImage(Resource2.GetBitmap(Resource2.BitmapResources.bird3), (int)x, (int)y);
                        counter = 1;
                    }

                    drawWall();

                    difference = DateTime.Now - start;
                    time = (int)difference.TotalSeconds + (int)difference.TotalMinutes;
                    levelIncrease = time / 10;
                    level = levelIncrease;
                    offsetChange = 2 + level;
                    speedChange = 1 + 0.25 * level;
                    screen.DrawString(("Time Elapsed: " + time + 's'), font, blueBrush, 350, 15);
                    screen.DrawString(("Player:  " + name), font, blueBrush, 350, 0);
                    screen.DrawString(("Level:  " + level), font, blueBrush, 350, 30);
                    offsetShifter += offsetChange;
                    moveCounter++;

                    if (dead)
                        y += 3;
                    else
                    {
                        if (padUp.Read() == 0)
                        {
                            y -= speedChange;
                        }

                        if (padDown.Read() == 0)
                        {
                            y += speedChange;
                        }
                        if (padRight.Read() == 0)
                        {
                            x += speedChange / 2;
                        }
                        if (padLeft.Read() == 0)
                        {
                            x -= speedChange / 2;
                        }
                        if (crash(points))
                        {
                            dead = true;
                            diedSince = DateTime.Now;
                        }
                    }
                    screen.Flush();
                }
                score = difference.Seconds + difference.Minutes * 60;
                players.Add(new Player(name, score));
                die();
                screen.Clear(Color.Black);
                screen.Flush();
                again = true;
                points.Clear();
                avg.Clear();
            }
        }
        static void die()
        {
            if (score < 10)
            {
                try
                {
                    int offsetShifter = 0;
                    screen.Clear(Color.Black);
                    int time = (DateTime.Now - diedSince).Seconds;

                    while (time < 3)
                    {
                        if (time < 2)
                        {
                            screen.DrawImage(Resource2.GetBitmap(Resource2.BitmapResources.randallF1), 480 - offsetShifter, 0);
                            screen.Flush();
                            offsetShifter += 30;
                        }
                        time = (DateTime.Now - diedSince).Seconds;
                        Thread.Sleep(10);
                    }
                }
                catch
                {
                    return;
                }
            }
            return;

        }
        static void showHighscores()
        {
            sort();

            screen.Clear(Color.Black);
            screen.Flush();
            changeScreen();
            screen.DrawLine(BluePen, 0, 0, 480, 0);
            screen.DrawString("High Scores", font2, new SolidBrush(Color.Red), 180, 0);
            screen.DrawLine(BluePen, 0, 20, 480, 20);

            screen.DrawString("Name", font2, new SolidBrush(Color.Red), 90, 20);
            screen.DrawString("Score", font2, new SolidBrush(Color.Red),340, 20);
            screen.DrawLine(BluePen, 0, 40, 480, 40);

            screen.DrawLine(BluePen, 240, 20, 240, 220);

            for (int i=0; i<players.Count; i++)
            {
                if (i == 10)
                    break;
                screen.DrawLine(BluePen, 0, (i+3) *20, 480, (i + 3) * 20);
                screen.DrawString(((Player)(players[i])).name, font2, blueBrush, 0, (i + 2) * 20);
                screen.DrawString(((Player)(players[i])).score.ToString(), font2, blueBrush, 240, (i + 2) * 20);

            }
            screen.Flush();
        }
        static void sort()
        {
           
            int j;
            for(int i =1; i <players.Count; i++)
            {
                key = (Player) players[i];
                j = i - 1;
                while(j>=0 && ((Player)players[j]).score < key.score)
                {
                    players[j + 1] = players[j];
                    j = j - 1;
                }
                players[j + 1] = key;
            }
        }
        static void showInstructions()
        {
            screen.Clear(Color.Black);
            changeScreen();
            screen.DrawString("INSTRUCTIONS: ", font2, new SolidBrush(Color.Red), 0, 0);
            screen.DrawString("1.Click UP to Start playing.", font2, new SolidBrush(Color.Blue), 20, 40);
            screen.DrawString("2.You can move in all four directions in the game.", font2, new SolidBrush(Color.Blue), 20, 60);
            screen.DrawString("3.Dodge walls or you will die.", font2, new SolidBrush(Color.Blue), 20, 80);
            screen.DrawString("4.Dodge bats or shoot them by clicking RIGHT.", font2, new SolidBrush(Color.Blue), 20, 100);
            screen.DrawString("5.Don't go back too far or the wall will catch you.", font2, new SolidBrush(Color.Blue), 20, 120);
            screen.DrawString("6.Have fun!", font2, new SolidBrush(Color.Blue), 20, 140);

            screen.Flush();

        }
        static void showAbout()
        {
            screen.Clear(Color.Black);
            changeScreen();
            screen.DrawString("ABOUT: ", font2, new SolidBrush(Color.Red), 0, 0);
            screen.DrawString("Author: ", font2, new SolidBrush(Color.Red), 20, 40);
            screen.DrawString("School: ", font2, new SolidBrush(Color.Red), 20, 60);
            screen.DrawString("Class: ", font2, new SolidBrush(Color.Red), 20, 80);
            screen.DrawString("Assignment: ", font2, new SolidBrush(Color.Red), 20, 100);
            screen.DrawString("Description: ", font2, new SolidBrush(Color.Red), 20, 120);

            screen.DrawString("Abdullah Aljandali ", font2, new SolidBrush(Color.Yellow), 130, 40);
            screen.DrawString("University of Evansville ", font2, new SolidBrush(Color.Yellow), 130, 60);
            screen.DrawString("Small Computer Software EE356 ", font2, new SolidBrush(Color.Yellow), 130, 80);
            screen.DrawString("Project6: Cave Runner ", font2, new SolidBrush(Color.Yellow), 130, 100);
            screen.DrawString("This is a cave runner game." , font2, new SolidBrush(Color.Yellow), 130, 120);
            screen.DrawString("Players should try to avoid obstacles.", font2, new SolidBrush(Color.Yellow), 20, 140);

            screen.Flush();

        }
        static int average(ArrayList a, int total)
        {
            int sum = 0;
            foreach (int t in a)
                sum += t;
            return sum / total;

        }
        static bool crash(ArrayList points)
        {

            int temp = level;
            if (temp > 7)
                temp = 7;

            if (x <= temp * 30 +10 || x >= 480)
                return true;
            else if (checkCorner((int) x, (int) y,points))
                return true;
            else if (checkCorner((int)x + planeWidth, (int)y, points))
                return true;
            else if (checkCorner((int)x +planeWidth, (int)y + planeHeight, points))
                return true;
            else if (checkCorner((int)x, (int)y + planeHeight, points))
                return true;
            else
                return false;

        }
        static bool checkCorner(int cornerX, int cornerY, ArrayList points)
        {
           
            cornerY = cornerY + 25;
            cornerX = cornerX - 25;
            
            int i = (cornerX / boxWidth) + 1;
            if (cornerY <= (int)points[i] || cornerY >= (int)points[i] + boxHeight)
                return true;

            return false;
        }
        static void initialScreen()
        {
            var displayController = DisplayController.GetDefault();
            // Enter the proper display configurations
            displayController.SetConfiguration(new ParallelDisplayControllerSettings
            {
                Width = 480,
                Height = 272,
                DataFormat = DisplayDataFormat.Rgb565,
                PixelClockRate = 20000000,
                PixelPolarity = false,
                DataEnablePolarity = true,
                DataEnableIsFixed = false,
                HorizontalFrontPorch = 2,
                HorizontalBackPorch = 2,
                HorizontalSyncPulseWidth = 41,
                HorizontalSyncPolarity = false,
                VerticalFrontPorch = 2,
                VerticalBackPorch = 2,
                VerticalSyncPulseWidth = 10,
                VerticalSyncPolarity = false,
            });

            displayController.Enable();
            screen = Graphics.FromHdc(displayController.Hdc);
            screen.Clear(Color.Black);
            screen.DrawImage(Resource2.GetBitmap(Resource2.BitmapResources.randall3hrs), 300, 0);
            screen.DrawString("Abdullahs crappy game", font2, new SolidBrush(Color.Red), 20, 40);
            screen.DrawString("To: ", font2, new SolidBrush(Color.Blue), 20, 20);
            screen.DrawString("Welcome", font2, new SolidBrush(Color.Yellow), 20, 0);
            screen.DrawString("Click UP to play", font2, new SolidBrush(Color.Green), 20, 100);
            screen.DrawString("Click DOWN to view highscores ", font2, new SolidBrush(Color.Green), 20, 120);
            screen.DrawString("Click RIGHT for about", font2, new SolidBrush(Color.Green), 20, 140);
            screen.DrawString("Click LEFT for instructions", font2, new SolidBrush(Color.Green), 20, 160);
            screen.Flush();

        }
        static void changeScreen()
        {
            screen.DrawString("Click UP to play", font, new SolidBrush(Color.Green), 10, 250);
            screen.DrawString("Click DOWN for highscores ", font, new SolidBrush(Color.Green), 90, 250);
            screen.DrawString("Click RIGHT for about", font, new SolidBrush(Color.Green), 230, 250);
            screen.DrawString("Click LEFT for instructions", font, new SolidBrush(Color.Green), 350, 250);

        }
        static string askForName()
        {
            int currentLetter = 0;
            string name = "";
            printName(currentLetter,name);

            while (true)
            {
                if (padUp.Read() == 0)
                {
                    if (currentLetter > 9 )
                    {
                        currentLetter -= 10;
                        printName(currentLetter, name);
                    }
                    Thread.Sleep(500);

                }
                else if (padDown.Read() == 0)
                {
                    if (currentLetter < 18)
                    {
                        currentLetter += 10;
                        printName(currentLetter, name);
                    }
                    Thread.Sleep(500);

                }
                else if (padLeft.Read() == 0)
                {
                    if ((currentLetter < 10 && currentLetter > 0)
                        || (currentLetter < 20 && currentLetter > 10)
                        || (currentLetter < 28 && currentLetter > 20))
                    {
                        currentLetter -= 1;
                        printName(currentLetter, name);
                    }
                    Thread.Sleep(500);

                }

                else if (padRight.Read() == 0)
                {
                    if ((currentLetter < 9 && currentLetter > -1)
                       || (currentLetter < 19 && currentLetter > 9)
                       || (currentLetter < 27 && currentLetter > 19))
                    {
                        currentLetter += 1;
                        printName(currentLetter, name);
                    }
                    Thread.Sleep(500);

                }
                else if (padSelect.Read() == 0)
                {
                    if (currentLetter == 26)
                        return name;
                    if (currentLetter == 27 && name.Length > 0)
                    {
                        string temp = "";
                        for (int i = 0; i < name.Length - 1; i++)
                            temp += name[i];
                        name = temp;
                        printName(currentLetter, name);

                    }
                    else
                    {
                        char ch = (char)(65 + currentLetter);
                        name += ch;
                        printName(currentLetter, name);
                    }
                    Thread.Sleep(500);

                }
                else
                    Thread.Sleep(500);
            }

        }
        static void printName(int currentLetter, string name)
        {
            screen.Clear(Color.Black);
            char ch = (char)65;
            screen.DrawString("Enter nickname: " + name, font2, new SolidBrush(Color.Red), 0, 0);


            for (int i = 0; i < 28; i++)
            {
                SolidBrush color = new SolidBrush(Color.Blue);
                if (i == currentLetter)
                    color = new SolidBrush(Color.Yellow);

                if (i < 10)
                    screen.DrawString(ch.ToString(), font2, color, 40 * (i + 1), 60);
                else if (i < 20)
                    screen.DrawString(ch.ToString(), font2, color, 40 * (i + 1 - 10), 120);
                else if (i < 26)
                    screen.DrawString(ch.ToString(), font2, color, 40 * (i + 1 - 20), 180);
                else if (i == 26)
                    screen.DrawString("Done", font2, color, 40 * (i + 1 - 20) , 180);
                else if (i==27)
                    screen.DrawString("Delete", font2, color, 40 * (i + 1 - 20) + 20, 180);

                ch++;
            }
            screen.Flush();
        }
        static void startGame()
        {
            screen.Clear(Color.Black);
            screen.DrawString("3", font2, new SolidBrush(Color.Green), 230, 130);
            screen.Flush();
            Thread.Sleep(1000);

            screen.Clear(Color.Black);
            screen.DrawString("2", font2, new SolidBrush(Color.Green), 230, 130);
            screen.Flush();
            Thread.Sleep(1000);

            screen.Clear(Color.Black);
            screen.DrawString("1", font2, new SolidBrush(Color.Green), 230, 130);
            screen.Flush();
            Thread.Sleep(1000);

            screen.Clear(Color.Black);
            screen.DrawString("Go!", font2, new SolidBrush(Color.Green), 230, 130);
            screen.Flush();
            Thread.Sleep(200);

            screen.Clear(Color.Black);
            screen.Flush();

        }
        static void drawWall()
        {
            int temp = level;
            if (temp >7 )
                temp = 7;
            if (level > 0)
            {
                screen.FillRectangle(new SolidBrush(Color.Blue), 0, 0, 30 * temp, 400);
                for(int i =0; i<14; i++)
                    screen.FillEllipse(new SolidBrush(Color.Blue), 20 * temp +  10 * (temp -1), i *20, 20, 20);
                
            }

        }
    }
}

