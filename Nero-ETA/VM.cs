using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Nero_ETA
{

    public class VM : DependencyObject
    {
        private static readonly int startPosition = 14;
        private static int endPosition = 821;
        private static int pcbHeight = 28;
        private int error;
        private bool connect;
        private bool connected
        {
            get => connect;
            set
            {
                connect = value;
                ButText = connect ? "Delete first" : "Connect!";
            }
        }

        private int FullTime;

        public DelegateCommand FirstCommand { get; }
        public DelegateCommand LastCommand { get; }
        public DelegateCommand ResetCommand { get; }

        private readonly Uri pcbUri = new Uri(System.IO.Directory.GetCurrentDirectory() + @"\PCB.jpg");
        BitmapImage pcb;
        public ObservableCollection<UIElement> PCB { get; } = new ObservableCollection<UIElement>();
        private ColorAnimation colorAnimation = new ColorAnimation(Colors.Red, Colors.Black, TimeSpan.FromMilliseconds(1000));

        public VM()
        {

            pcb = new BitmapImage(pcbUri);
            LastCommand = new DelegateCommand(LastCommands);
            FirstCommand = new DelegateCommand(FirstCommands);
            ResetCommand = new DelegateCommand(ResetCommands);

            BL.PcbDataChanged += PcbDataResived;
            SpeedPCB = BL.SpeedPCB;
            FullTime = BL._fullTime;
            connected = BL.Running();

            TimeColor = new SolidColorBrush(Colors.Black);
            colorAnimation.AutoReverse = false;
            colorAnimation.FillBehavior = FillBehavior.Stop;

            LineAdd(1);
            LineAdd(31);
        }


        #region Command
        private void FirstCommands(object obj)
        {
            if (connected)
            {
                BL.SendMinus("M");
                if (CountPCB != 0)
                {
                    PCB.RemoveAt(PCB.Count - 1);
                    CountPCB--;
                }

            }

            else
            {
                connected = BL.Running();
            }
        }

        private void LastCommands(object obj)
        {
            BL.SendMinus("m");
        }

        private void ResetCommands(object obj)
        {
            CountPCBFull = 0;
        }

        #endregion


        #region Property

        public SolidColorBrush TimeColor
        {
            get { return (SolidColorBrush)GetValue(TimeColorProperty); }
            set { SetValue(TimeColorProperty, value); }
        }
        public static readonly DependencyProperty TimeColorProperty =
            DependencyProperty.Register("TimeColor", typeof(SolidColorBrush), typeof(VM), new PropertyMetadata());


        public int TimePCB
        {
            get { return (int)GetValue(TimePCBProperty); }
            set { SetValue(TimePCBProperty, value); }
        }
        public static readonly DependencyProperty TimePCBProperty =
            DependencyProperty.Register("TimePCB", typeof(int), typeof(VM), new PropertyMetadata(0));

        public int CountPCB
        {
            get { return (int)GetValue(CountPCBProperty); }
            set { SetValue(CountPCBProperty, value); }
        }
        public static readonly DependencyProperty CountPCBProperty =
            DependencyProperty.Register("CountPCB", typeof(int), typeof(VM), new PropertyMetadata(0));

        public int CountPCBFull
        {
            get { return (int)GetValue(CountPCBFullProperty); }
            set { SetValue(CountPCBFullProperty, value); }
        }
        public static readonly DependencyProperty CountPCBFullProperty =
            DependencyProperty.Register("CountPCBFull", typeof(int), typeof(VM), new PropertyMetadata(0));

        public string SpeedPCB
        {
            get { return (string)GetValue( SpeedPCBProperty); }
            set {SetValue(SpeedPCBProperty, value);}
        }
        public static readonly DependencyProperty SpeedPCBProperty =
            DependencyProperty.Register("SpeedPCB", typeof(string), typeof(VM), new PropertyMetadata( "", SpeedPCBPCB_changed));

        private static void SpeedPCBPCB_changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VM current)
            {
                BL.SpeedPCB = current.SpeedPCB;
            }
        }

        public string ButText
        {
            get { return (string)GetValue(ButTextProperty); }
            set { SetValue(ButTextProperty, value); }
        }
        public static readonly DependencyProperty ButTextProperty =
            DependencyProperty.Register("ButText", typeof(string), typeof(VM), new PropertyMetadata(""));
        #endregion


        private void LineAdd(int top)
        {

            Line line = new Line();
            SolidColorBrush lineBrush = new SolidColorBrush(Colors.Black);
            line.Stroke = lineBrush;
            line.X1 = -910;
            line.X2 = 920;
            line.StrokeThickness = 2;
            line.StrokeDashArray = new DoubleCollection(2) { 2, 1 };
            PCB.Add(line);
            Canvas.SetLeft(PCB.Last(), 0);
            Canvas.SetTop(PCB.Last(), top);

            DoubleAnimation lineAnimation = new DoubleAnimation();
            lineAnimation.From = startPosition;
            lineAnimation.To = endPosition;
            lineAnimation.RepeatBehavior = RepeatBehavior.Forever;
            lineAnimation.Duration = TimeSpan.FromSeconds(FullTime - 1);
            PCB.Last().BeginAnimation(Canvas.LeftProperty, lineAnimation);
        }

        private void PcbDataResived(object sender, EventArgsSerial e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {   
                if(CountPCB < (PCB.Count - 2))
                {
                    error++;
                    if (error > 5)
                    {
                        PCB.RemoveAt(2);
                    }
                }
                else
                {
                    error = 0;
                }
            
            
                while (CountPCB < e.data[0])
                {
                    CountPCB++;
                    ImageAdd();
                }
                while (CountPCB > e.data[0])
                {
                    CountPCB--;
                    ImageLastDelet();
                }
                
                TimePCB = e.data[1];
                if((TimePCB *100) / FullTime < 7)
                { TimeColor.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);}               

            }));

        }

        private void ImageAdd()
        {
            Image t = new Image();
            t.Source = pcb;
            t.RenderTransform = new RotateTransform(90.0f);
            PCB.Add(t);
            Canvas.SetLeft(PCB.Last(), -10);
            Canvas.SetTop(PCB.Last(), 2);

            DoubleAnimation anim = new DoubleAnimation();
            anim.From = startPosition;
            anim.To = endPosition;
            anim.Duration = TimeSpan.FromSeconds(FullTime - 1);
            PCB.Last().BeginAnimation(Canvas.LeftProperty, anim);
        }

        private void ImageLastDelet()
        {
            DoubleAnimation animationDelet = new DoubleAnimation();
            animationDelet.From = Canvas.GetLeft(PCB[2]);
            animationDelet.To = 850;
            animationDelet.Duration = TimeSpan.FromMilliseconds(700);
            animationDelet.Completed += PCB_Last_Delet;
            PCB[2].BeginAnimation(Canvas.LeftProperty,animationDelet);
            
        }

        private void PCB_Last_Delet(object sender, EventArgs eventArgs)
        {
            PCB.RemoveAt(2);
            CountPCBFull++;
        }

    }
}
