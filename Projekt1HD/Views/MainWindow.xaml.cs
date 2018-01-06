using HtmlAgilityPack;
using Projekt1HD.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Media;
using System.Data.SQLite;
using System.Windows.Input;
using System.Collections;
using System.Windows.Media.Animation;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Windows.Controls.Primitives;
using Excel = Microsoft.Office.Interop.Excel;
using Projekt1HD.ViewModels;
using System.Diagnostics;
using Microsoft.Win32;

namespace Projekt1HD
{
    public partial class MainWindow
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new DataViewModel(ProductWithReviewsPanel);
        }
        

        
        private bool AutoScroll = true;

        private void SingleProduct_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange == 0)
            {   // Content unchanged : user scroll event
                if (ScrollLog.VerticalOffset == ScrollLog.ScrollableHeight)
                {   // Scroll bar is in bottom
                    // Set autoscroll mode
                    AutoScroll = true;
                }
                else
                {   // Scroll bar isn't in bottom
                    // Unset autoscroll mode
                    AutoScroll = false;
                }
            }

            // Content scroll event : autoscroll eventually
            if (AutoScroll && e.ExtentHeightChange != 0)
            {   // Content changed and autoscroll mode set
                // Autoscroll
                ScrollLog.ScrollToVerticalOffset(ScrollLog.ExtentHeight);
            }
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var rev = DataGridReview.CurrentItem as DbReview;

            SaveFileDialog savefile = new SaveFileDialog();

            savefile.FileName = "rev_" + rev.Rev_CeneoID + ".txt";

            savefile.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            var result = savefile.ShowDialog();

            if (result == true)
            {
                using (StreamWriter sw = new StreamWriter(savefile.FileName))
                {
                    sw.WriteLine("ID: " + rev.Rev_CeneoID);
                    sw.WriteLine("Data: " + rev.Rev_Date);
                    sw.WriteLine("Reviewer: " + rev.Rev_Reviewer);
                    sw.WriteLine("Advantages: " + rev.Rev_Advantages);
                    sw.WriteLine("Defects: " + rev.Rev_Defects);
                    sw.WriteLine("Review: " + rev.Rev_Content);
                    sw.WriteLine("Down votes: " + rev.Rev_DownVotes);
                    sw.WriteLine("Up votes: " + rev.Rev_UpVotes);
                    sw.WriteLine("Rating: " + rev.Rev_Rating);
                    sw.WriteLine("Recom: " + rev.Rev_Recom);
                }
            }


            if (MessageBox.Show("Do you want to open new file?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Process.Start(savefile.FileName);
            }
        }
    }
}
