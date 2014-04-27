﻿using System;
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

namespace Games
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.Loaded += MainWindow_Loaded;
            InitializeComponent();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Time = " + DateTime.Now);
            Tournament tournament = new Tournament(new HearthstoneReferee());
            for (int i = 0; i < 100; i++)
            {
                tournament.Players.Add(tournament.MakeNewPlayer());
            }
            tournament.Run(10000);
        }
    }
}
