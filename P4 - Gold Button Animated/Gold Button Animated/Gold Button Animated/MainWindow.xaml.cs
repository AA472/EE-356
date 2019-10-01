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
using System.Drawing;
using System.Windows.Media.Animation;
using System.Threading;
namespace Gold_Button_Animated
{
    public partial class MainWindow : Window
    {

        int picked = -1;
        int num_squares = 16;
        int num_buttons = 1;
        int[] squares;
        Ellipse[] ell;
        int[] aliveButtons;
        SolidColorBrush gold = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 215, 0));
        SolidColorBrush green = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0));
        SolidColorBrush buttonColor;
        string turn = "You";
        double r1;
        double r2;
        double angleInc;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            cnv1.ClipToBounds = true;
            //dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);

        }
        public void circleDraw()
        {

            // Draw outer Ellipse
            Ellipse outer = new Ellipse();
            outer.Height = 200;
            outer.Width = 200;
            outer.Margin = new Thickness(135, 5, 0, 0);
            outer.Fill = green;
            cnv1.Children.Add(outer);
            r2 = 100;

            //Make it an empty circle
            Ellipse outer2 = new Ellipse();
            outer2.Height = 199;
            outer2.Width = 199;
            outer2.Margin = new Thickness(135.5, 5.5, 0, 0);
            outer2.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            cnv1.Children.Add(outer2);



            // Draw inner Ellipse
            Ellipse circle = new Ellipse();
            circle.Height = 150; circle.Width = 150;
            circle.Margin = new Thickness(160, 30, 0, 0);
            cnv1.Children.Add(circle);
            circle.Fill = green;
            r1 = 75;
            Line rLine;

            //Draw Lines (boundries)
            int temp = num_squares;

            angleInc = (double)360 / num_squares;

            for (int i = 0; i <= num_squares; i++)
            {
                rLine = new Line();
                rLine.Stroke = green;

                rLine.X1 = 310; rLine.X2 = 335;
                rLine.Y1 = 105; rLine.Y2 = 105;
                if (i == 1)
                    rLine.X2 = 380;
                rLine.StrokeThickness = 2;
                cnv1.Children.Add(rLine);
                DoubleAnimation dblAni = new DoubleAnimation();
                dblAni.From = 0;
                dblAni.To = i * angleInc;
                dblAni.Duration = new Duration(TimeSpan.FromSeconds(0));
                RotateTransform rotate = new RotateTransform();
                rotate.CenterX = 235;
                rotate.CenterY = 105;
                rLine.RenderTransform = rotate;
                rotate.BeginAnimation(RotateTransform.AngleProperty, dblAni);
            }


            //Make inner ellipse an empty
            Ellipse circle2 = new Ellipse();
            circle2.Height = 149; circle2.Width = 149;
            circle2.Margin = new Thickness(160.5, 30.5, 0, 0);
            cnv1.Children.Add(circle2);
            circle2.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));






            for (double i = 0; i < angleInc - 4; i++)
            {
                rLine = new Line();
                rLine.X1 = 312; rLine.X2 = 337;
                rLine.Y1 = 107; rLine.Y2 = 107;
                rLine.StrokeThickness = 2;
                cnv1.Children.Add(rLine);
                rLine.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                DoubleAnimation dblAni = new DoubleAnimation();
                dblAni.From = 0;
                dblAni.To = i;
                dblAni.Duration = new Duration(TimeSpan.FromSeconds(0));
                RotateTransform rotate = new RotateTransform();
                rotate.CenterX = 235;
                rotate.CenterY = 105;
                rLine.RenderTransform = rotate;
                rotate.BeginAnimation(RotateTransform.AngleProperty, dblAni);
            }
        }
        public void setup()
        {
            squares = new int[num_squares];
            for (int i = 0; i < num_squares; i++)
                squares[i] = -1;
            aliveButtons = new int[num_buttons];
            for (int i = 0; i < num_buttons; i++)
                aliveButtons[i] = 0;
            aliveButtons[0] = 1; // gold button

            for (int i = 0; i < num_buttons; i++)
            {
                ell[i].Height = 25.0 * (22.0 / num_squares); ell[i].Width = 25.0 * (22.0 / num_squares);
            }

            int location;
            Random rand = new Random();
            location = rand.Next(0, num_squares / 4);
            squares[location] = 0;
            ell[0].Margin = new Thickness(310, 105 - ell[0].Height, 0, 0);
            cnv1.Children.Add(ell[0]);
            animateMove(0, location, 0.5, 0);


            for (int i = 1; i < num_buttons; i++)
            {
                aliveButtons[i] = 1;
                while (squares[location] != -1)
                    location = rand.Next(0, num_squares);
                ell[i].Margin = new Thickness(310, 105, 0, 0);
                cnv1.Children.Add(ell[i]);
                squares[location] = i;
                animateMove(0, location, 0.5, i);
            }

            resetColors();
        }

        public void animateMove(int from, int to, double dur, int ellipse)
        {
            double x, y, angle;
            angle = (from + 0.5) * angleInc * Math.PI / 180;
            x = 87.5 * Math.Cos(angle);
            y = 87.5 * Math.Sin(angle);
            if (angle <= 180) { x = 235 + x; y = 105 - y; }
            if (angle > 180) { x = 235 + x; y = 105 + y; }

            double Startx = x-12.5;
            double Starty = y-12.5;

            cnv1.Children.Remove(ell[ellipse]);
            ell[ellipse] = new Ellipse();
            ell[ellipse].Height = 25.0 * (22.0 / num_squares); ell[ellipse].Width = 25.0 * (22.0 / num_squares);   
            ell[ellipse].Margin = new Thickness(x-12.5, y-12.5, 0, 0);
            ell[ellipse].Fill = buttonColor;
            cnv1.Children.Add(ell[ellipse]);

            int k = 1;
            for (int i = from+1; i <= to; i++)
            {

                angle = (i + 0.5) * angleInc * Math.PI / 180;
                x = 87.5 * Math.Cos(angle);
                y = 87.5 * Math.Sin(angle);
                if (angle <= 180) { x = 235 + x; y = 105 - y; }
                if (angle > 180) { x = 235 + x; y = 105 + y; }
                //ell[ellipse].Margin = new Thickness(x-12.5, y-12.5, 0, 0);
                
                x = x - 12.5;
                y = y - 12.5;
                DoubleAnimation da = new DoubleAnimation();
                da.From = 0;
                da.To = x - Startx;

                if (to ==25)
                da.To = 500;

                da.Duration = new Duration(TimeSpan.FromSeconds(dur));
                TranslateTransform rt = new TranslateTransform();
                ell[ellipse].RenderTransform = rt;
                ell[ellipse].RenderTransformOrigin = new System.Windows.Point(0,0);
                rt.BeginAnimation(TranslateTransform.XProperty, da);

                da.From = 0;
                da.To = y - Starty;
                if (to == 25)
                    da.To = 200;
                rt.BeginAnimation(TranslateTransform.YProperty, da);
                
            }
        }
        // }
        //public void animateMove(int from, int to, double dur, int ellipse)
        //{
        //    TranslateTransform animatedTranslateTransform =
        //        new TranslateTransform();

        //    // Register the transform's name with the page
        //    // so that they it be targeted by a Storyboard.
        //    cnv1.RegisterName("AnimatedTranslateTransform", animatedTranslateTransform);
        //    ell[ellipse].RenderTransform = animatedTranslateTransform;
        //    ell[ellipse].RenderTransformOrigin = new System.Windows.Point(200, 150);

        //    PathGeometry animationPath = new PathGeometry();
        //    PathFigure pFigure = new PathFigure();
        //    pFigure.StartPoint = new System.Windows.Point(0, 0);
        //    PolyBezierSegment pBezierSegment = new PolyBezierSegment();
        //    System.Windows.Point start = new System.Windows.Point(0, 0);
        //    System.Windows.Point end = new System.Windows.Point(445, 185);
        //    LoadPathPoints(pBezierSegment, from, to, dur, ellipse);
        //    pFigure.Segments.Add(pBezierSegment);
        //    animationPath.Figures.Add(pFigure);

        //    animationPath.Freeze();

        //    DoubleAnimationUsingPath translateXAnimation =
        //        new DoubleAnimationUsingPath();
        //    translateXAnimation.PathGeometry = animationPath;
        //    translateXAnimation.Duration = TimeSpan.FromSeconds(5);

        //    translateXAnimation.Source = PathAnimationSource.X;

        //    Storyboard.SetTargetName(translateXAnimation, "AnimatedTranslateTransform");
        //    Storyboard.SetTargetProperty(translateXAnimation,
        //        new PropertyPath(TranslateTransform.XProperty));

        //    DoubleAnimationUsingPath translateYAnimation =
        //        new DoubleAnimationUsingPath();
        //    translateYAnimation.PathGeometry = animationPath;
        //    translateYAnimation.Duration = TimeSpan.FromSeconds(5);


        //    translateYAnimation.Source = PathAnimationSource.Y;


        //    Storyboard.SetTargetName(translateYAnimation, "AnimatedTranslateTransform");
        //    Storyboard.SetTargetProperty(translateYAnimation,
        //        new PropertyPath(TranslateTransform.YProperty));

        //    // Create a Storyboard to contain and apply the animations.
        //    Storyboard pathAnimationStoryboard = new Storyboard();
        //    pathAnimationStoryboard.RepeatBehavior = RepeatBehavior.Forever;
        //    pathAnimationStoryboard.Children.Add(translateXAnimation);
        //    pathAnimationStoryboard.Children.Add(translateYAnimation);
        //    // Start the animations.

        //    pathAnimationStoryboard.Begin(ell[ellipse]);

        //}

        public void LoadPathPoints(PolyBezierSegment pBezierSegment, int from, int to, double dur, int ellipse)
        {
            //double x = 235, y = 105, angle;
            //for (int i = from; i <= to; i++)
            //{
            //    angle = (i + 0.5) * angleInc * Math.PI / 180;
            //    x = 87.5 * Math.Cos(angle);
            //    y = 87.5 * Math.Sin(angle);
            //    if (angle <= 180) { x = 235 + x; y = 105 - y; }
            //    if (angle > 180) { x = 235 + x; y = 105 + y; }
                //double incrx = (end.X - start.X) / 5;
                //double incry = (end.Y - start.Y) / 5;
                //int i;
                //double x, y;
                //x = start.X; y = start.Y;
                //for (i = 0; i < 6; i++)
                //{
                pBezierSegment.Points.Add(new System.Windows.Point(0, 0));
            pBezierSegment.Points.Add(new System.Windows.Point(50, 50));
            pBezierSegment.Points.Add(new System.Windows.Point(100, 100));

            //    x += incrx;
            //    y += incry;
            //}
        }
    
    //private void animateMove(int from, int to, double dur, int ellipse)
    //{
    //    movement = new System.Windows.Point[from - to + 1];
    //    double x=235, y=105, angle;
    //    for (int i = from; i <= to; i++)
    //    {
    //        angle = (i + 0.5) * angleInc * Math.PI / 180;
    //        x = 87.5 * Math.Cos(angle);
    //        y = 87.5 * Math.Sin(angle);
    //        if (angle <= 180) { x = 235 + x; y = 105 - y; }
    //        if (angle > 180) { x = 235 + x; y = 105 + y; }


    //    }

    //}



    private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
    private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            num_squares = Convert.ToInt16(txt_squares.Text);
            num_buttons = Convert.ToInt16(txt_buttons.Text);
            ell = new Ellipse[num_buttons];
            for (int i = 0; i < num_buttons; i++)
                ell[i] = new Ellipse();

            System.Drawing.Color c = System.Drawing.Color.FromName(cb_color.Text);
            buttonColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(c.R,c.G,c.B));

           
            if(num_squares > 46 || num_squares < 16)
                MessageBox.Show("The number of squares must be between 16 and 48", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            else if (num_squares > num_buttons)
            {
                btn_start.IsEnabled = false;
                cb_color.IsEnabled = false;
                txt_buttons.IsEnabled = false;
                txt_squares.IsEnabled = false;
                circleDraw();
                setup();
            }
            else
                MessageBox.Show("The number of buttons must be less than the number of squares.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    private void cnv1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(cnv1);
            getLocation(p);
            turn = "You";
            int temp;

            int index = getLocation(p);
            if (index !=-2)
            {

                temp = index;
                index = squares[index];
                if (index > -1)
                {
                    picked = temp;
                    if (picked == num_squares-1)
                    {
                        take();
                    }
                    else
                    {
                        resetColors();
                        ell[index].Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
                    }
                }
            }
        }
    private void cnv1_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (picked != -1)
            {
                System.Windows.Point p = e.GetPosition(cnv1);
                int index = getLocation(p);

                    if (picked >= index)
                        MessageBox.Show("Illegal move. You can only move from right to left inside squares.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                    else if (squares[index] == -1)
                    {
                        bool test = true;
                        for (int i = picked +1 ; i < index ; i++)
                            if (squares[i] != -1)
                                test = false;
                        if (test)
                        {
                            move(index);
                            computerPlay();
                        }
                        else
                            MessageBox.Show("You cannot jump over other buttons.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                        MessageBox.Show("You cannot move to this spot, there's a button here.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            else
                MessageBox.Show("You have to pick a button first.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    public void move(int drop) {
        int index = squares[picked];
            if (index != -1)
            {
                animateMove(picked,drop,0.2,squares[picked]);
                squares[picked] = -1;
                squares[drop] = index;
                resetColors();
                picked = -1;
            }
        }

        public void take()
        {
            bool done = false;
            if (squares[num_squares-1] == 0)
                done = true;
            resetColors();
            animateMove(num_squares - 1, 25, 10, squares[num_squares - 1]);
            //ell[squares[num_squares-1]].Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 255, 255, 255));
            //aliveButtons[squares[num_squares-1]] = 0;
            squares[num_squares-1] = -1;
            picked = -1;
            if (done)
            {
                end();
            }
        }
        
        public void end()
        {
            img.Opacity = 0;
            FormattedText fmtxt;
            System.Windows.Media.Pen blkPen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, 1);
            System.Windows.Media.Brush bluBrush = System.Windows.Media.Brushes.AliceBlue;
            DrawingVisual vis = new DrawingVisual();
            DrawingContext dc = vis.RenderOpen();
           
            fmtxt = GetFormattedText("The Winner Is: " + turn, 40);
            dc.DrawText(fmtxt, new System.Windows.Point(0, 0));
            dc.Close();

            RenderTargetBitmap bmp = new RenderTargetBitmap(490, 260, 96, 96,
                PixelFormats.Pbgra32);
            bmp.Render(vis);
            WriteableBitmap wb = new WriteableBitmap(bmp);
            img.Source =  wb;
            Storyboard sBrd = new Storyboard();
            DoubleAnimation dblAni = new DoubleAnimation();
            dblAni.From = 0.0;
            dblAni.To = 1.0;
            dblAni.Duration = new Duration(TimeSpan.FromSeconds(3));
            dblAni.AutoReverse = true;
            //dblAni.RepeatBehavior = RepeatBehavior.Forever;

            sBrd.Children.Add(dblAni);
            Storyboard.SetTargetName(dblAni, img.Name);
            Storyboard.SetTargetProperty(dblAni, new PropertyPath(System.Windows.Controls.Image.OpacityProperty));
            sBrd.Begin(this);
        }
        public void computerPlay()
        {
            turn = "Computer";
            int i = 1;
            if (squares[num_squares-1] != -1)
            {
                picked = num_squares-1;
                take();
            }
            else
            {
                for(i=num_squares-2; i >-1 ; i--)
                {
                    if (squares[i] != -1)
                    {
                        picked = i;
                        move(i + 1);
                        break;
                    }
                }
            }
            picked = -1;
        }
        public void resetColors()
        {
            for (int i = 1; i < num_buttons; i++)
            {
                //if (aliveButtons[i] == 1)
                    ell[i].Fill = buttonColor;
            }
            ell[0].Fill = gold;
        }
        public int getLocation(System.Windows.Point p)
        {
            string k = p.X.ToString() + "    " + p.Y.ToString();
            double d = Math.Sqrt((235 - p.X) * (235 - p.X) + (105 - p.Y) * (105 - p.Y));
            double d2 = Math.Sqrt((335 - p.X) * (335 - p.X) + (105 - p.Y) * (105 - p.Y));
            double angle;
            int location = -2;
            double temp;
            if (d < r2 && d > r1)
            {
                temp = (r2 * r2 + d * d - d2 * d2) / (2 * r2 * d);
                temp = Math.Round(temp, 4);
                angle = Math.Acos(temp);
                angle = angle * 180 / Math.PI;
                location = (int)(angle / (angleInc));
                if (p.Y > 105)
                    location = num_squares - location -1;
                //MessageBox.Show(location.ToString());

                if (location >= num_squares)
                    return -3;
                return location;
            }
            else
                return -2;
          
        }
    //public void drawLinePath()
    //{
    //    DrawingVisual vis = new DrawingVisual();
    //    DrawingContext dc = vis.RenderOpen();
    //    System.Windows.Media.Pen blkPen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, 1);
    //    dc.DrawLine(blkPen, new System.Windows.Point(0, 100), new System.Windows.Point(26 * num_squares, 100));
    //    dc.DrawLine(blkPen, new System.Windows.Point(0, 150), new System.Windows.Point(26 * num_squares, 150));

    //    for(int i=0; i<=num_squares; i++)
    //        dc.DrawLine(blkPen, new System.Windows.Point(26*i, 100), new System.Windows.Point(26 *i, 150));
    //    dc.Close();
    //    RenderTargetBitmap bmp = new RenderTargetBitmap(468, 209, 96, 96, PixelFormats.Pbgra32);
    //    bmp.Render(vis);
    //    img.Source = bmp;

    //    //for (x = 10; x < 100; x += 8)
    //    //{
    //    //    dc.DrawEllipse(brushColor[i % 3], blkPen, new Point(x, y), 4, 4);
    //    //    i++;
    //    //}
    //}
    public FormattedText GetFormattedText(string sTmp, int typeSize)
    { FormattedText fmtxt;
        fmtxt = new FormattedText(sTmp, System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight, new Typeface("Times New Roman"),
            typeSize, System.Windows.Media.Brushes.Black); return fmtxt;
    }

       
    }
}
