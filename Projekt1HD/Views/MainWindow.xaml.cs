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
using Microsoft.WindowsAPICodePack.Dialogs;

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

        private void Log_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange == 0)
            {  
                if (Log.VerticalOffset == Log.ScrollableHeight)
                { 
                    AutoScroll = true;
                }
                else
                {   
                    AutoScroll = false;
                }
            }
            
            if (AutoScroll && e.ExtentHeightChange != 0)
            {  
                Log.ScrollToVerticalOffset(Log.ExtentHeight);
            }
        }

                
        private void ExportToCSV_Button_Click(object sender, RoutedEventArgs e)
        {
            ExportToCSV_Button.IsEnabled = false;

            SaveFileDialog dialog = new SaveFileDialog()
            {
                FileName = "Reviews " + DateTime.Now.ToShortDateString() + ".csv",
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
            };

            var result = dialog.ShowDialog();

            if (result.Value)
            {
                var app = new Excel.Application();
                var workbooks = app.Workbooks;
                Excel.Workbook sampleWorkbook = workbooks.Add();
                Excel.Worksheet sheet = sampleWorkbook.Sheets.Add();


                for (var i = 1; i <= DbReviews_DataGrid.Items.Count; i++)
                {
                    var r = (DbReview)DbReviews_DataGrid.Items[i - 1];
                    sheet.Cells[i, 1] = r.Rev_CeneoID + "," +
                                        r.Rev_Reviewer + "," +
                                        r.Rev_Advantages + "," +
                                        r.Rev_Defects + "," +
                                        r.Rev_Recom + "," +
                                        r.Rev_Date + "," +
                                        r.Rev_Rating + "," +
                                        r.Rev_Content + "," +
                                        r.Rev_UpVotes + "," +
                                        r.Rev_DownVotes + ",";
                }

                
                sheet.SaveAs(dialog.FileName);                
                workbooks.Close();
                ExportToCSV_Button.IsEnabled = true;

                MessageBox.Show("File saved.");
            } else
            {
                ExportToCSV_Button.IsEnabled = true;
                return;
            }



        }

        private void ExportToTxtButton_Click(object sender, RoutedEventArgs e)
        {
            ExportToTxtButton.IsEnabled = false;

            var revs = DbReviews_DataGrid.SelectedItems;

            if (revs.Count == 0)
            {
                revs = DbReviews_DataGrid.Items;
            }

            if (revs.Count > 1)
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog()
                {
                    IsFolderPicker = true
                };

                CommonFileDialogResult result = dialog.ShowDialog();

                if (result.ToString() == "Ok")
                {
                    foreach (DbReview rev in revs)
                    {
                        string fileName = dialog.FileName + "\\rev_" + rev.Rev_CeneoID + ".txt";

                        using (StreamWriter sw = new StreamWriter(fileName))
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

                    MessageBox.Show("Files are saved in: " + dialog.FileName);
                    ExportToTxtButton.IsEnabled = true;
                } else
                {

                    ExportToTxtButton.IsEnabled = true;
                    return;
                }
                
            } 
            else if (revs.Count == 1)
            {
                DbReview rev = revs[0] as DbReview;
                 
                SaveFileDialog dialog = new SaveFileDialog()
                {
                    FileName = "rev_" + rev.Rev_CeneoID + ".txt",
                    Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
                };
                
                var result = dialog.ShowDialog();

                if (result.Value)
                {
                    using (StreamWriter sw = new StreamWriter(dialog.FileName))
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

                    ExportToTxtButton.IsEnabled = true;

                    if (MessageBox.Show("File saved!\nDo you want to open new file?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Process.Start(dialog.FileName);
                    }
                } else
                {
                    ExportToTxtButton.IsEnabled = true;
                    return;
                }
                
            }
        }
    }
}
