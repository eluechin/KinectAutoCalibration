﻿using System;
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
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Calibration;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //var s = new KinectCalibration();
            //s.ShowArea();
            //MessageBox.Show("aaa");
            //var beamer2 = new Beamer(beamerWindow);
            //beamer2.DisplayCalibrationImage(true);
            //beamerWindow.Show();
            
           
            //MessageBox.Show("hallo");
            

            //Thread newThread = new Thread(new ThreadStart(() =>
            //    {
            //        new KinectCalibration().StartCalibration();
            //        System.Windows.Threading.Dispatcher.Run();
            //    }));
            //newThread.SetApartmentState(ApartmentState.STA);
            //newThread.IsBackground = true;
            //newThread.Start();
            //MessageBox.Show("test");
            var a = new KinectCalibration();
            a.StartCalibration();

        }
    }
}
