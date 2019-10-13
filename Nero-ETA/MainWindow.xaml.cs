using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Nero_ETA
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
            InitializeComponent();
           // Closed += MainWindow_Closed;
           // VM.ColorAnimationEvent += VM_ColorAnimationEvent;
        }

        //private void VM_ColorAnimationEvent()
        //{
        //    SolidColorBrush brush = new SolidColorBrush();
        //    Time.Foreground = brush;

        //    ColorAnimation colrAnimation = new ColorAnimation();
        //    colrAnimation.From = Colors.Red;
        //    colrAnimation.To = Colors.Black;
        //    colrAnimation.AutoReverse = true;
        //    colrAnimation.Duration = TimeSpan.FromMilliseconds(500);
        //    brush.BeginAnimation(SolidColorBrush.ColorProperty, colrAnimation);

        //}

        //private void MainWindow_Closed(object sender, EventArgs e)
        //{
        //    BL.Runnung = false;
        //    Thread.Sleep(800);
        //}
    }
}
