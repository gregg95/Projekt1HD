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
using Prism.Mvvm;
using Prism.Commands;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Threading;

namespace Projekt1HD.ViewModels
{
    public class DataViewModel : BindableBase, INotifyPropertyChanged
    {

        private BindableStackPanel _prdPanel;
        public BindableStackPanel PrdPanel
        {
            get { return _prdPanel; }
            set { SetProperty(ref _prdPanel, value); }
        }


        private ObservableCollection<Product> _productCollection = new ObservableCollection<Product>();
        public ObservableCollection<Product> ProductCollection
        {
            get
            {
                return _productCollection;
            }
            set
            {
                _productCollection = value;
                RaisePropertyChanged();
            }
        }

        public enum Views
        {
            Products,
            ProductWithReviews,
            TranformedReviews,
            DbProducts,
            DbReviews,
            NoView,
            WelcomePage
        };

        private string _searchString;
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                _searchString = value;
                RaisePropertyChanged();
            }
        }

        private string _logText;
        public string LogText
        {
            get { return _logText; }
            set
            {
                _logText = value;
                RaisePropertyChanged();
            }
        }

        private string _progressText;
        public string ProgressText
        {
            get { return _progressText; }
            set
            {
                _progressText = value;
                RaisePropertyChanged();
            }
        }

        private Brush _processButtonsPanelBorderBrushColor;
        public Brush ProcessButtonsPanelBorderBrushColor
        {
            get { return _processButtonsPanelBorderBrushColor; }
            set
            {
                _processButtonsPanelBorderBrushColor = value;
                RaisePropertyChanged();

            }
        }

        private Visibility _productsVisibility;
        public Visibility ProductsVisibility
        {
            get { return _productsVisibility; }
            set
            {
                _productsVisibility = value;
                RaisePropertyChanged();
            }
        }

        private Visibility _productWithReviewsVisibility;
        public Visibility ProductWithReviewsVisibility
        {
            get { return _productWithReviewsVisibility; }
            set
            {
                _productWithReviewsVisibility = value;
                RaisePropertyChanged();
            }
        }

        private Visibility _dbReviewsVisibility;
        public Visibility DbReviewsVisibility
        {
            get { return _dbReviewsVisibility; }
            set
            {
                _dbReviewsVisibility = value;
                RaisePropertyChanged();
            }
        }

        private Visibility _dbProductsVisibility;
        public Visibility DbProductsVisibility
        {
            get { return _dbProductsVisibility; }
            set
            {
                _dbProductsVisibility = value;
                RaisePropertyChanged();
            }
        }

        private Visibility _transformedReviewsVisibility;
        public Visibility TransformedDataVisibility
        {
            get { return _transformedReviewsVisibility; }
            set
            {
                _transformedReviewsVisibility = value;
                RaisePropertyChanged();
            }
        }


        private Visibility _loadingSpinnerVisibility;
        public Visibility LoadingSpinnerVisibility
        {
            get { return _loadingSpinnerVisibility; }
            set
            {
                _loadingSpinnerVisibility = value;
                RaisePropertyChanged();
            }
        }

        private Visibility _welcomePageVisibility;
        public Visibility WelcomePageVisibility
        {
            get { return _welcomePageVisibility; }
            set
            {
                _welcomePageVisibility = value;
                RaisePropertyChanged();
            }
        }

        public ICommand SearchCommand { get; set; }
        public ICommand NextPageCommand { get; set; }
        public ICommand PreviousPageCommand { get; set; }
        public ICommand ProductClickCommand { get; set; }
        public ICommand ExtractDataCommand { get; set; }
        public ICommand TransformDataCommand { get; set; }
        public ICommand LoadDataCommand { get; set; }
        public ICommand ShowDatabaseCommand { get; set; }
        public ICommand ClearDatabaseCommand { get; set; }
        public ICommand ETLCommand { get; set; }
        public ICommand DbProductClickCommand { get; set; }
        public ICommand BackToDbProductsCommand { get; set; }
        public ICommand ExportToCSVCommand { get; set; }
        public ICommand GoToWelcomePageCommand { get; set; }

        private ObservableCollection<DbProduct> _dbProducts = new ObservableCollection<DbProduct>();
        public ObservableCollection<DbProduct> DbProducts
        {
            get { return _dbProducts; }
            set
            {
                _dbProducts = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<DbReview> _dbReviews = new ObservableCollection<DbReview>();
        public ObservableCollection<DbReview> DbReviews
        {
            get { return _dbReviews; }
            set
            {
                _dbReviews = value;
                RaisePropertyChanged("DbReviews");

            }
        }

        private bool _isEnabledNextButton;
        public bool IsEnabledNextButton
        {
            get { return _isEnabledNextButton; }
            set
            {
                _isEnabledNextButton = value;
                RaisePropertyChanged();
            }
        }

        private bool _hasStageESucced;
        public bool HasStageESucced
        {
            get { return _hasStageESucced; }
            set
            {

                _hasStageESucced = value;


                RaisePropertyChanged();

            }
        }

        private int _pageCount;
        public int PageCount
        {
            get
            {
                return _pageCount;
            }
            set
            {
                _pageCount = value;
                RaisePropertyChanged();
            }
        }


        private string _currentPage;
        public string CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                _currentPage = value;
                RaisePropertyChanged();
            }
        }


        PrdDetails PrdDetails;

        List<Review> ReviewsList = new List<Review>();

        public string ProductId { get; set; }

        private bool _isSearchByIdChecked;
        public bool IsSearchByIdChecked
        {
            get { return _isSearchByIdChecked; }
            set
            {
                _isSearchByIdChecked = value;
                RaisePropertyChanged();
            }
        }

        private bool _isEAllowed;
        public bool IsEAllowed
        {
            get { return _isEAllowed; }
            set
            {
                _isEAllowed = value;
                RaisePropertyChanged();
            }
        }

        private bool _isTAllowed;
        public bool IsTAllowed
        {
            get { return _isTAllowed; }
            set
            {
                _isTAllowed = value;
                RaisePropertyChanged();
            }
        }

        private bool _isLAllowed;
        public bool IsLAllowed
        {
            get { return _isLAllowed; }
            set
            {
                _isLAllowed = value;
                RaisePropertyChanged();
            }
        }

        private Statistics statistics = new Statistics();


        public HtmlDocument htmlDoc = new HtmlDocument();

        public DataViewModel(StackPanel stackPanel)
        {
            SearchCommand = new RelayCommand(o => SearchClick());
            NextPageCommand = new RelayCommand(o => NextPageClick());
            PreviousPageCommand = new RelayCommand(o => PreviousPageClick());
            ProductClickCommand = new RelayCommand(o => ProductClick(o));

            ETLCommand = new RelayCommand(o => ETLClick());
            ExtractDataCommand = new RelayCommand(o => ExtractClick());
            TransformDataCommand = new RelayCommand(o => TransformClick());
            LoadDataCommand = new RelayCommand(o => LoadClick());

            ShowDatabaseCommand = new RelayCommand(o => ShowDatabaseClick());
            ClearDatabaseCommand = new RelayCommand(o => ClearDatabaseClick());

            DbProductClickCommand = new RelayCommand(o => DbProductClick(o));
            GoToWelcomePageCommand = new RelayCommand(o => GoToWelcomePageClick());

            BackToDbProductsCommand = new RelayCommand(o => HideAllBut(Views.DbProducts));
            ExportToCSVCommand = new RelayCommand(o => ExportToCSV());

            
            ProcessButtonsPanelBorderBrushColor = Brushes.Pink;
            IsSearchByIdChecked = true;
            SearchString = "50851290";
            LoadingSpinnerVisibility = Visibility.Hidden;
            HideAllBut(Views.WelcomePage);
            Log("Log: ");



            
            IsEAllowed = false;
            IsTAllowed = false;
            IsLAllowed = false;

            _prdPanel = new BindableStackPanel();
            PrdPanel.StkPanel = stackPanel;
        }

        public async void ETLClick()
        {
            LoadingSpinnerVisibility = Visibility.Visible;
            IsEAllowed = false;
            IsTAllowed = false;
            IsLAllowed = false;

            Task<bool> t = ExtractData();
            bool xasd = await t;

            await Task.Run(() => TransformData());

            await Task.Run(() => LoadData());


            LoadingSpinnerVisibility = Visibility.Hidden;
            ProcessButtonsPanelBorderBrushColor = Brushes.Pink;
        }

        public async void ExtractClick()
        {

            LoadingSpinnerVisibility = Visibility.Visible;
            IsEAllowed = false;

            Task<bool> t = ExtractData();
            bool xasd = await t;

            IsTAllowed = true;
            LoadingSpinnerVisibility = Visibility.Hidden;

        }

        public async void TransformClick()
        {
            LoadingSpinnerVisibility = Visibility.Visible;
            IsTAllowed = false;

            await Task.Run(() => TransformData());


            IsLAllowed = true;
            LoadingSpinnerVisibility = Visibility.Hidden;
        }


        public async void LoadClick()
        {
            LoadingSpinnerVisibility = Visibility.Visible;
            IsLAllowed = false;

            await Task.Run(() => LoadData());

            ProcessButtonsPanelBorderBrushColor = Brushes.Pink;
            LoadingSpinnerVisibility = Visibility.Hidden;
        }


        public async void DbProductClick(object o)
        {
            HideAllBut(Views.DbReviews);

            var prd = o as DbProduct;
            var id = prd.Prd_CeneoID;

            _dbReviews.Clear();

            Log("Connected to database...");

            await Task.Run(() =>
            {
                GetReviewsForProduct(id);
            });

            Log("Done!");

            ProgressText = "Done";
        }



        public void GetReviewsForProduct(int id)
        {
            try
            {

                string sql = "SELECT * FROM Reviews Where Rev_PrdID = " + id;

                SQLiteCommand command = new SQLiteCommand(sql, Connection());

                SQLiteDataReader reader = command.ExecuteReader();
                
                Log("Connected \nReading stored reviews.");

                int i = 0;

                while (reader.Read())
                {
                    i++;

                    DbReview dbReview = new DbReview()
                    {
                        Rev_PrdID = Convert.ToInt32(reader["Rev_PrdID"].ToString()),
                        Rev_ID = Convert.ToInt32(reader["Rev_ID"]),
                        Rev_Advantages = reader["Rev_Advantages"].ToString(),
                        Rev_Defects = reader["Rev_Defects"].ToString(),
                        Rev_Summary = reader["Rev_Summary"].ToString(),
                        Rev_Rating = reader["Rev_Rating"].ToString(),
                        Rev_Reviewer = reader["Rev_Reviewer"].ToString(),
                        Rev_Content = reader["Rev_Content"].ToString(),
                        Rev_CeneoID = (reader["Rev_CeneoID"].ToString() != "") ? Convert.ToInt32(reader["Rev_CeneoID"]) : 0,
                        Rev_Date = (reader["Rev_Date"].ToString() != "") ? (DateTime?)DateTime.Parse(reader["Rev_Date"].ToString()) : (DateTime?)null,
                        Rev_Recom = (reader["Rev_Recom"].ToString() != "") ? reader["Rev_Recom"].ToString() : "",
                        Rev_DownVotes = (reader["Rev_DownVotes"].ToString() != "") ? Convert.ToInt32(reader["Rev_DownVotes"]) : 0,
                        Rev_UpVotes = (reader["Rev_UpVotes"].ToString() != "") ? Convert.ToInt32(reader["Rev_UpVotes"]) : 0
                    };

                   
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ProgressText = "" + i;
                        DbReviews.Add(dbReview);
                    });
                    Log("Loaded review: ID = " + Convert.ToInt32(reader["Rev_CeneoID"]));

                }
            }
            catch (Exception ea)
            {
                MessageBox.Show(ea.ToString());
            }


        }

        public void ShowDatabaseClick()
        {
            HideAllBut(Views.DbProducts);
            

            _dbProducts.Clear();

            try
            {

                Log("Connected to database...");
                string sql = "SELECT * FROM Products";

                SQLiteCommand command = new SQLiteCommand(sql, Connection());

                SQLiteDataReader reader = command.ExecuteReader();

                Log("Reading stored products.");

                while (reader.Read())
                {
                    _dbProducts.Add(new DbProduct()
                    {
                        Prd_ID = Convert.ToInt32(reader["Prd_ID"]),
                        Prd_CeneoID = Convert.ToInt32(reader["Prd_CeneoID"]),
                        Prd_Brand = reader["Prd_Brand"].ToString(),
                        Prd_Model = reader["Prd_Model"].ToString(),
                        Prd_Type = reader["Prd_Type"].ToString(),
                        Prd_Comments = reader["Prd_Comments"].ToString()

                    });

                    Log("Loaded item: ID = " + reader["Prd_CeneoID"].ToString());
                }
            }
            catch (Exception ea)
            {
                Console.WriteLine(ea.ToString());
            }

        }

        public void ClearDatabaseClick()
        {


            try
            {
                
                string deleteReviewsQuery = "DELETE FROM Reviews";
                string deleteProductsQuery = "DELETE FROM Products";

                SQLiteCommand command = new SQLiteCommand(deleteProductsQuery, Connection());

                int items_deleted = command.ExecuteNonQuery();

                Log("Deleted " + items_deleted + " products.");

                SQLiteCommand command2 = new SQLiteCommand(deleteReviewsQuery, Connection());

                int reviews_deleted = command2.ExecuteNonQuery();

                Log("Deleted " + reviews_deleted + " reviews.");




            }
            catch (Exception exa)
            {
                Console.WriteLine(exa.ToString());
            }
        }

        public async void SearchProduct(string prdId)
        {
            ProgressText = "";
            LoadingSpinnerVisibility = Visibility.Visible;
            ProductCollection.Clear();

            HideAllBut(Views.ProductWithReviews);

            Task<HtmlDocument> longRunningTask = ConnectWithPageAsync("https://www.ceneo.pl/" + prdId + "#tab=reviews");
            htmlDoc = await longRunningTask;


            string _pageType = "";

            //szuka nawet jak id jest przed to usówa string
            try
            {
                _pageType = htmlDoc.DocumentNode.SelectSingleNode("//meta[contains(@property,'og:type')]").Attributes["content"].Value;
            }
            catch (Exception e) 
            {
                LoadingSpinnerVisibility = Visibility.Hidden;
                HideAllBut(Views.WelcomePage);
                MessageBox.Show("No product found.");
                return;
            }

            Log("Checking result..");
            if (_pageType == "product.group")
            {
                //product not found
                MessageBox.Show("No results for search id");
            }
            else if (_pageType == "product")
            {
                PrdPanel.StkPanel.Children.Clear();

                PrdDetails = new PrdDetails
                {
                    Prd_CeneoID = prdId,
                    Prd_LowerPrice = htmlDoc.DocumentNode.SelectSingleNode("//meta[contains(@property,'og:price:amount')]")?.Attributes["content"].Value
                                    + " " + htmlDoc.DocumentNode.SelectSingleNode("//meta[contains(@property,'og:price:currency')]")?.Attributes["content"].Value,
                    Prd_Name = htmlDoc.DocumentNode.SelectSingleNode("//h1[contains(@class,'product-name js_product-h1-link')]")?.InnerHtml,
                    Prd_Rating = htmlDoc.DocumentNode.SelectSingleNode("//input[contains(@name,'rating')]").Attributes["value"].Value + "/5",
                    Prd_VotesCount = htmlDoc.DocumentNode.SelectSingleNode("//span[contains(@class,'product-reviews-link__votes-count')]")?.InnerHtml.Split(' ')[0] ?? "0",
                    Prd_ReviewsCount = htmlDoc.DocumentNode.SelectSingleNode("//span[contains(@itemprop,'reviewCount')]")?.InnerHtml ?? "0",
                    Prd_Type = htmlDoc.DocumentNode.SelectSingleNode("//nav[contains(@class,'breadcrumbs')]/dl/dd")?.SelectNodes("span")?.Last().SelectSingleNode(".//span[@itemprop='title']")?.InnerHtml ?? "null",
                    Prd_Brand = htmlDoc.DocumentNode.SelectSingleNode("//meta[contains(@property,'og:brand')]")?.Attributes["content"].Value ?? ""
                };

                Log("Product found! It is: " + PrdDetails.Prd_Name);

                StackPanel stp = new StackPanel();

                stp.Orientation = Orientation.Horizontal;



                TextBlock text = new TextBlock();

                text.Inlines.Add(new Run
                {
                    FontSize = 30,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.LightGreen,
                    Text = "Product found!\n"
                });

                text.Inlines.Add(new Run
                {
                    FontSize = 20,
                    Text = "Product ID: " + PrdDetails.Prd_CeneoID + "\n"
                           + "Name: " + PrdDetails.Prd_Name + "\n"
                           + "Lower Price: " + PrdDetails.Prd_LowerPrice + "\n"
                           + "Brand: " + PrdDetails.Prd_Brand + "\n"
                           + "Rating: " + PrdDetails.Prd_Rating + "\n"
                           + "Votes Count: " + PrdDetails.Prd_VotesCount + "\n"
                           + "Category: " + PrdDetails.Prd_Type + "\n"
                           + "Reviews Count: " + PrdDetails.Prd_ReviewsCount
                });

                stp.Children.Add(text);
                PrdPanel.StkPanel.Children.Add(stp);

                LoadingSpinnerVisibility = Visibility.Hidden;

                if (Convert.ToInt32(PrdDetails.Prd_ReviewsCount) == 0)
                {
                    MessageBox.Show("There are no reviews for this product.");                    
                    return;
                }

                IsEAllowed = true;
                ProcessButtonsPanelBorderBrushColor = Brushes.LightGreen;

                
            }

        }


        private void ProductClick(object o)
        {
            var prd = o as Product;
            SearchProduct(prd.ProductID);
        }

        private async void NextPageClick()
        {
            HideAllBut(Views.NoView);
            LoadingSpinnerVisibility = Visibility.Visible;

            Int32.TryParse(Regex.Match(CurrentPage, @"\d+").Value, out int c);
            string _pageNo = ";0020-30-0-0-" + c + ".htm";
            CurrentPage = "Page: " + (c + 1);
            
            var htmlDoc = await ConnectWithPageAsync("https://www.ceneo.pl/;szukaj-" + SearchString + _pageNo);


            GetProductsOnPage(htmlDoc);

            if (c + 1 == PageCount)
            {
                IsEnabledNextButton = false;
            }

            LoadingSpinnerVisibility = Visibility.Hidden;
            HideAllBut(Views.Products);
        }

        private async void PreviousPageClick()
        {
            LoadingSpinnerVisibility = Visibility.Visible;
            HideAllBut(Views.NoView);

            Int32.TryParse(Regex.Match(CurrentPage, @"\d+").Value, out int c);
            string _pageNo = ";0020-30-0-0-" + (c - 2) + ".htm";
            CurrentPage = "Page: " + (c - 1);


            var htmlDoc = await ConnectWithPageAsync("https://www.ceneo.pl/;szukaj-" + SearchString + _pageNo);

            GetProductsOnPage(htmlDoc);



            if (c-1 < PageCount)
            {
                IsEnabledNextButton = true;
            }

            LoadingSpinnerVisibility = Visibility.Hidden;
            HideAllBut(Views.Products);
        }

        private void SearchClick()
        {
            ProgressText = "";
            LoadingSpinnerVisibility = Visibility.Visible;
            ProductCollection.Clear();
            LockETLProcess();


            Log("Starting E process");

            if (IsSearchByIdChecked == true)
            {
                SearchProduct(SearchString);
            }
            else
            {
                SearchProducts("https://www.ceneo.pl/;szukaj-" + SearchString);
            }
        }

        public void LockETLProcess()
        {
            IsEAllowed = false;
            IsTAllowed = false;
            IsLAllowed = false;

            ReviewsList.Clear();
            ProcessButtonsPanelBorderBrushColor = Brushes.Pink;
            DbReviews.Clear();
            PrdPanel.StkPanel.Children.Clear();
            
        }

        public void Log(string l)
        {
            _logText += "\n" + l;
            RaisePropertyChanged(nameof(LogText));
        }


        public async Task<HtmlDocument> ConnectWithPageAsync(string _url_address)
        {
            Log("Connecting to web page...");

            var htmlWeb = new HtmlWeb();
            var tcs = new TaskCompletionSource<HttpWebResponse>();
            htmlWeb.OverrideEncoding = Encoding.UTF8;
            var htmlDoc = new HtmlDocument();

            await Task.Run(() =>
            {
                try
                {

                    htmlDoc = htmlWeb.Load(_url_address);
                    Log("Connected!");


                }
                catch (Exception aaa)
                {
                    MessageBox.Show("Error: Probably you don't have access to Internet.");

                    Log("Failed..No Internet Connection.");

                }
            });

            return htmlDoc;
        }



        public async Task<bool> ExtractData()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {


                ReviewsList.Clear();
                Log("Looking for reviews.. ");


                Task<HtmlDocument> longRunningTask = ConnectWithPageAsync("https://www.ceneo.pl/" + PrdDetails.Prd_CeneoID + "#tab=reviews");
                var htmlDoc = await longRunningTask;

                var ReviewCount = htmlDoc.DocumentNode.SelectSingleNode("//span[contains(@itemprop,'reviewCount')]").InnerHtml;  //<span itemprop="reviewCount">124</span>




                Int32.TryParse(PrdDetails.Prd_ReviewsCount, out int rc);

                if (rc % 10 != 0)
                {
                    rc = rc + (10 - (rc % 10));
                }

                var pages = rc / 10;

                Log("Found total " + PrdDetails.Prd_ReviewsCount + " reviews on " + pages + " pages");


                var review_no = 0;

                int xdd = 0;

                for (var i = 1; i <= pages; i++)
                {
                    Log("Reading page " + i);

                    ProgressText = i.ToString() + " / " + pages;

                    var doc = new HtmlDocument();

                    if (i == 1)
                    {
                        doc = htmlDoc;
                    }
                    else
                    {

                        // var x = ProductId + "/opinie-" + i;

                        var c = await ConnectWithPageAsync("https://www.ceneo.pl/" + PrdDetails.Prd_CeneoID + "/opinie-" + i);

                        doc = c;


                        //  var d = await c.DownloadStringTaskAsync("https://www.ceneo.pl/" + TextBox_Search.Text + "/opinie-" + i);

                        //      doc.LoadHtml(d);
                    }


                    var revSegments = doc.DocumentNode.SelectNodes("//li[contains(@class,'review-box js_product-review')]");


                    foreach (HtmlNode r in revSegments)
                    {
                        review_no++;
                        Log("Reading review " + review_no);

                        {/*< span class="js_product-review-usefulness vote">
            <button class="vote-yes js_product-review-vote js_vote-yes" data-icon="" data-url="/SetOpinionVote" data-review-id="3532102" data-vote="1" data-voted="false" data-product-id="39684857" data-total-vote="6"><span id = "votes-yes-3532102" > 6 </ span ></ button >
            < button class="vote-no js_product-review-vote js_vote-no" data-icon="" data-url="/SetOpinionVote" data-review-id="3532102" data-vote="0" data-voted="false" data-product-id="39684857" data-total-vote="5"><span id = "votes-no-3532102" > 5 </ span ></ button >

         </ span >  */
                         //< time datetime = "2017-04-12 02:46:00" > 7 miesięcy temu</ time >

                            /*
                             * div class="product-review-pros-cons">
                    <div class="pros-cell">
                            <span class="pros">Zalety</span>
                            <ul>
                                    <li> ergonomia</li>
                                    <li> wygląd</li>
                            </ul>
                    </div>
                    <div class="cons-cell">
                    </div>
                </div>


                            */
                            //46839349 
                        }

                        HtmlDocument h = new HtmlDocument();
                        h.LoadHtml(r.InnerHtml);
                        var d = h.DocumentNode;

                        var s = h.DocumentNode.SelectSingleNode("//div[contains(@class,'pros-cell')]").InnerHtml;

                        string pattern = @"^\r\n\s+$";

                        Match resultAdv = Regex.Match(s, pattern);


                        if (!resultAdv.Success)
                        {
                            var ss = d.SelectSingleNode("//div[contains(@class,'pros-cell')]/ul").SelectNodes("li");
                        }

                        s = h.DocumentNode.SelectSingleNode("//div[contains(@class,'cons-cell')]").InnerHtml;


                        Match resultCons = Regex.Match(s, pattern);


                        if (!resultCons.Success)
                        {

                            var ss = d.SelectSingleNode("//div[contains(@class,'cons-cell')]/ul").SelectNodes("li").Select(c => c.InnerHtml).ToList();
                        }

                        {
                            /* 
                                            <div id="product-review-comment-3532102" class="js_product-review-form-hook"></div>

                                                <ol class="product-review-comments">
                                                        <li class="product-review-comment js_product-review-hook">
                                                            <div class="product-review-comments-hover">
                                                                <p class="product-review-byline">
                                                                    <strong>Anonim</strong>
                                                                    <span class="review-time">Wystawiono <time datetime="2016-02-06 11:58:04">2 lata temu</time></span>
                                                                </p>

                                                                <p class="product-review-body">Nie trafiłeś na super model g29 tylko miałeś wadliwy model g27. W g27 nic nie trzeszczy, a martwa strefa nawet w dfgt była niezauważalna.</p>
                                                                <a class="capitalize hover-highlight review-link abuse-ico js_report-product-review-comment-abuse" href="#abuse" data-product-id="39684857" data-url="/ReportCommentAbuseOfProductReview" data-review-id="3532102" data-review-comment-id="271351" role="button">Zgłoś nadużycie</a>

                                                                <div class="product-review-comment-toolbar">
                                                                    <a class="product-review-comment-reply-toggle js_product-review-comment-toggle" href="#product-review-comment-reply-271351" data-product-review-id="39684857" data-comment-id="271351" data-review-id="3532102" data-name="Anonim">Odpowiedz</a>
                                                                </div>
                                                            </div>

                                                                <ol class="product-review-replies">
                                                                        <li class="product-review-reply">
                                                                            <p class="product-review-byline">
                                                                                <strong>Trc</strong>
                                                                                <span class="review-time">Wystawiono <time datetime="2017-10-05 00:10:34">3 tygodnie temu</time></span>
                                                                            </p>
                                                                            <p class="product-review-body">&quot;martwa strefa nawet w dfgt była niezauważalna&quot;
                            Takich komentarzy nie da się traktować poważnie.</p>
                                                                            <div class="product-review-comment-toolbar">
                                                                                <a class="capitalize hover-highlight review-link abuse-ico js_report-product-review-comment-abuse" href="#abuse" data-product-id="39684857" data-url="/ReportCommentAbuseOfProductReview" data-review-id="3532102" data-review-comment-id="313115" role="button">Zgłoś nadużycie</a>
                                                                            </div>
                                                                        </li>
                                                                </ol>
                                                            <div id="product-review-comment-reply-271351" class="js_product-review-form-hook"></div>
                                                        </li>
                                                </ol>

                                        </div>  
                                         */
                        }

                        //odpowiedzi do review //zspanie
                        var review_comments = (d.SelectSingleNode("//div[contains(@class,'js_product-review-comments')]/ol[contains(@class,'product-review-comments')]") != null) ?
                            /*then */ d.SelectSingleNode("//div[contains(@class,'js_product-review-comments')]/ol[contains(@class,'product-review-comments')]").SelectNodes("li")
                                                    .Select(c => new Comment()
                                                    {
                                                        Commentator = c.SelectSingleNode("div[contains(@class,'product-review-comments-hover')]/p[contains(@class, 'product-review-byline')]/strong").InnerHtml,
                                                        CommentString = (c.SelectSingleNode("div[contains(@class,'product-review-comments-hover')]/p[contains(@class, 'product-review-body')]") != null) ?
                                                                            c.SelectSingleNode("div[contains(@class,'product-review-comments-hover')]/p[contains(@class, 'product-review-body')]").InnerHtml : null,

                                                        Replies = (c.SelectSingleNode("ol[contains(@class,'product-review-replies')]") != null) ?
                                                                        c.SelectSingleNode("ol[contains(@class,'product-review-replies')]").SelectNodes("li").
                                                                        Select(ra => new CommentReply()
                                                                        {
                                                                            Respondent = ra.SelectSingleNode("p[contains(@class, 'product-review-byline')]/strong").InnerHtml,
                                                                            Content = ra.SelectSingleNode("p[contains(@class, 'product-review-body')]").InnerText

                                                                        }).ToList()
                                                                        : new List<CommentReply>() { }

                                                    })
                                                    /*else */: null;






                        Review review = new Review()
                        {
                            Reviewer = d.SelectSingleNode("//div[contains(@class,'reviewer-name-line')]").InnerHtml,
                            Comments = (review_comments != null) ? review_comments.ToList() : new List<Comment>() { },
                            Rating = d.SelectSingleNode("//span[contains(@class, 'review-score-count')]").InnerHtml,  //<span class="review-score-count">4,5/5</span>
                            Date = d.SelectSingleNode("//span[contains(@class, 'review-time')]").ChildNodes["time"].Attributes["datetime"].Value,
                            Votes_Yes = d.SelectSingleNode("//button[contains(@class, 'vote-yes js_product-review-vote')]").Attributes["data-total-vote"].Value,
                            Votes_No = d.SelectSingleNode("//button[contains(@class, 'vote-no js_product-review-vote')]").Attributes["data-total-vote"].Value,
                            Review_Text = d.SelectSingleNode("//p[contains(@class,'product-review-body')]").InnerHtml, //<p class="product-review-body
                            Product_Recommended = (d.SelectSingleNode("//em[contains(@class,'product-recommended')]") != null) ? d.SelectSingleNode("//em[contains(@class,'product-recommended')]").InnerHtml : ((d.SelectSingleNode("//em[contains(@class,'product-not-recommended')]") != null) ? (d.SelectSingleNode("//em[contains(@class,'product-not-recommended')]").InnerHtml) : null), //, // < em class="product-recommended">Polecam</em>
                            Advantages = (!resultAdv.Success) ? d.SelectSingleNode("//div[contains(@class,'pros-cell')]/ul").SelectNodes("li").Select(c => c.InnerHtml).ToList() : new List<string>() { },
                            Defects = (!resultCons.Success) ? d.SelectSingleNode("//div[contains(@class,'cons-cell')]/ul").SelectNodes("li").Select(c => c.InnerHtml).ToList() : new List<string>() { },
                            Review_ID = d.SelectSingleNode("//button[contains(@class,'vote-yes')]").Attributes["data-review-id"].Value
                            //<button class="vote-yes js_product-review-vote js_vote-yes" data-icon="&#xe00d;" data-url='/SetOpinionVote' data-review-id="4999676" 
                        };



                        xdd++;

                        ReviewsList.Add(review);
                        ShowReview(review, xdd);




                    };
                }

                ProgressText = "Done";


                LoadingSpinnerVisibility = Visibility.Hidden;

                PrdDetails.Prd_Reviews = ReviewsList;

                HasStageESucced = true;





                //   Show_Specific_Item();

            }
            catch (Exception e)
            {
                LoadingSpinnerVisibility = Visibility.Hidden;

                Console.WriteLine(e);
            }



            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;


            statistics.ETime = elapsedMs;
            statistics.ECount = ReviewsList.Count;

            return true;

        }


        public  void TransformData()
        {

            Application.Current.Dispatcher.Invoke(() =>
            {
                DbReviews.Clear();
            });
            HideAllBut(Views.TranformedReviews);

            ProgressText = "";

            var watch = System.Diagnostics.Stopwatch.StartNew();

            DbReview dbReview = new DbReview() { };
            int i = 0;

            ProgressText = i + "/" + ReviewsList.Count;

            try
            {

                foreach (Review x in ReviewsList)
                {
                    i++;



                    dbReview = new DbReview()
                    {
                        Rev_Recom = x.Product_Recommended,
                        Rev_Advantages = (x.Advantages.Count != 0) ? TransformString(String.Join("/", x.Advantages.ToArray())) : "",
                        Rev_Defects = (x.Defects.Count != 0) ? TransformString(String.Join("/", x.Defects.ToArray())) : "",
                        Rev_Content = TransformString(x.Review_Text),
                        Rev_Rating = x.Rating.Replace(@",", "."),
                        Rev_Reviewer = TransformString(x.Reviewer),
                        Rev_CeneoID = Convert.ToInt32(x.Review_ID),
                        Rev_Date = DateTime.Parse(x.Date),
                        Rev_PrdID = Convert.ToInt32(PrdDetails.Prd_CeneoID),
                        Rev_UpVotes = (x.Votes_Yes != "") ? Convert.ToInt32(x.Votes_Yes) : 0,
                        Rev_DownVotes = (x.Votes_No != "") ? Convert.ToInt32(x.Votes_No) : 0

                    };


                    
                    ProgressText = i + "/" + ReviewsList.Count;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        DbReviews.Add(dbReview);
                    });
                    Log("Tranformed rev with id " + dbReview.Rev_CeneoID);
                }


            }
            catch (Exception e)
            {

            }



            ProgressText = "Done";

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;


            statistics.TTime = elapsedMs;
            statistics.TCount = DbReviews.Count;

        }


        


        public async void SearchProducts(string _urlAddress)
        {
            HideAllBut(Views.NoView);
            IsEnabledNextButton = true;

            var htmlDoc = await ConnectWithPageAsync(_urlAddress);


            try
            {

                string _pageCountValue = htmlDoc.DocumentNode.SelectSingleNode("//input[@id='page-counter']").Attributes["data-pageCount"].Value;

                PageCount = 1;
                CurrentPage = "Page: 1";

            }
            catch (Exception nex)
            {
                MessageBox.Show("There are no products for searched input.");
                LoadingSpinnerVisibility = Visibility.Hidden;
                return;
            }

            GetProductsOnPage(htmlDoc);

            LoadingSpinnerVisibility = Visibility.Hidden;

        }

        public void GetProductsOnPage(HtmlDocument htmlDoc)
        {
            try
            {
                var nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class,'cat-prod-row js_category-list-item js_man-track-event')]");



                ProductCollection.Clear();

                foreach (HtmlNode node in nodes)
                {

                    HtmlNode aNode = node.SelectSingleNode("./a");

                    var x = new HtmlDocument();
                    x.LoadHtml(node.OuterHtml);

                    if (Regex.IsMatch(x.DocumentNode.SelectSingleNode("//a[contains(@class, ' js_conv')]").Attributes["href"].Value.Remove(0, 1), @"^[0-9].*"))
                    {

                        var c = Regex.Match(x.DocumentNode.SelectSingleNode("//a[contains(@class, 'product-reviews-link dotted-link js_reviews-link')]")?.ChildNodes[2]?.InnerText ?? "0", @"\d+").Value; 

                        ProductCollection.Add(new Product
                        {
                            Name = x.DocumentNode.SelectSingleNode("//a[contains(@class, ' js_conv')]").InnerText,
                            Url = new Uri("https://www.ceneo.pl/" + x.DocumentNode.SelectSingleNode("//a[contains(@class, ' js_conv')]").Attributes["href"].Value.Remove(0, 1)),
                            ProductID = x.DocumentNode.SelectSingleNode("//div[contains(@class,'cat-prod-row js_category-list-item js_man-track-event')]").Attributes["data-pid"].Value,
                            ReviewsCount = c

                        });
                    }
                }

                HideAllBut(Views.Products);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error " + exc.ToString());
            }

        }

        public SQLiteConnection Connection()
        {
            SQLiteConnection sQLiteConnection =
                new SQLiteConnection("Data Source = ProjektHD.db; " +
                "                     Version = 3; " +
                "                     datetimeformat=CurrentCulture; ");

            try
            {
                sQLiteConnection.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("There is a problem with connection. \n" + e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return sQLiteConnection;
        }

        public void ShowReview(Review r, int xdd)
        {
            //  HideAllBut(Views.Products);

            StackPanel sp = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            Thickness margin = sp.Margin;
            margin.Left = 10;
            margin.Top = 10;
            margin.Right = 10;
            sp.Margin = margin;


            TextBlock Reviewer = new TextBlock
            {
                Width = 200,
                FontSize = 15
            };

            r.Reviewer = Regex.Replace(r.Reviewer, @"\s+", " ");

            Reviewer.Text = "no: " + xdd + " Reviewer: " + r.Reviewer + "\n"
                            + "Data: " + r.Date.ToString() + "\n"
                            + "Rating: " + r.Rating + "\n"
                            + r.Product_Recommended + "\n"
                            + "Votes up: " + r.Votes_Yes.ToString() + "\n"
                            + "Votes down: " + r.Votes_No.ToString();




            sp.Children.Add(Reviewer);


            if (r.Advantages.Count > 0)
            {
                DataGrid LVa = new DataGrid
                {
                    Width = 150,
                    ItemsSource = r.Advantages.Select(x => new { Advantages = x }).ToList(),
                    IsReadOnly = true
                };

                sp.Children.Add(LVa);
            }


            if (r.Defects.Count > 0)
            {

                DataGrid LVd = new DataGrid
                {
                    Width = 150,
                    ItemsSource = r.Defects.Select(x => new { Defects = x }).ToList()
                };
                
                LVd.IsReadOnly = true;

                sp.Children.Add(LVd);
            }

            TextBlock RText = new TextBlock
            {
                TextWrapping = TextWrapping.WrapWithOverflow,
                Background = Brushes.LightSteelBlue,
                MaxWidth = 500,
                Text = r.Review_Text,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            sp.Children.Add(RText);

            if (r.Comments.Count > 0)
            {
                StackPanel sp_com = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Right
                };

                foreach (Comment c in r.Comments)
                {


                    TextBlock CText = new TextBlock
                    {
                        TextWrapping = TextWrapping.WrapWithOverflow,
                        Background = Brushes.LightCyan,
                        MaxWidth = 500,
                        Text = c.CommentString
                    };


                    sp_com.Children.Add(CText);


                    if (c.Replies.Count > 0 || c.Replies != null)
                    {
                        foreach (CommentReply rep in c.Replies)
                        {
                            TextBlock RepText = new TextBlock
                            {
                                TextWrapping = TextWrapping.WrapWithOverflow,
                                Background = Brushes.LightGreen,
                                TextAlignment = TextAlignment.Right,
                                MaxWidth = 500,
                                Text = rep.Content
                            };

                            sp_com.Children.Add(RepText);
                        }
                    }
                }

                sp.Children.Add(sp_com);
            }

            PrdPanel.StkPanel.Children.Add(sp);
        }

        private void LoadData()
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();

            //var x = (from r in DbReviews
            //         select new
            //         {
            //             it = r.Rev_Date
            //         }).ToList().Distinct().Count();


            //var zx = (from r in DbReviews
            //          select new
            //          {
            //              it = r.Rev_Reviewer
            //          }).ToList().Distinct().Count();

            //var azx = (from r in DbReviews
            //           select new
            //           {
            //               r.Rev_PrdID,
            //               r.Rev_Reviewer,
            //               r.Rev_Content,
            //               r.Rev_Recom,
            //               r.Rev_Date,
            //               r.Rev_CeneoID,
            //               r.Rev_Rating,
            //               r.Rev_DownVotes,
            //               r.Rev_UpVotes,
            //               r.Rev_Advantages,
            //               r.Rev_Defects
            //           }).ToList().Distinct();



            //var xa = (from r in DbReviews
            //          select new
            //          {
            //              it = r.Rev_CeneoID
            //          }).ToList().Distinct().Count();


            //var xaa = (from r in DbReviews
            //           select new
            //           {
            //               it = r.Rev_CeneoID
            //           }).ToList();


            

            try
            {

                ProgressText = 1 + "/" + 1;

                var sql_item =
                    "INSERT INTO Products(" +
                    "Prd_CeneoID, " +
                    "Prd_Type, " +
                    "Prd_Brand, " +
                    "Prd_Model, " +
                    "Prd_Comments) " +

                        "SELECT " +
                            "'" + PrdDetails.Prd_CeneoID + "', " +
                            "'" + PrdDetails.Prd_Type + "', " +
                            "'" + PrdDetails.Prd_Brand + "', " +
                            "'" + PrdDetails.Prd_Name + "', " +
                            "'" + PrdDetails.Prd_Name + "'" +


                     "WHERE NOT EXISTS(SELECT 1 FROM Products WHERE Prd_CeneoID = " + PrdDetails.Prd_CeneoID + ") ";


                SQLiteCommand command = new SQLiteCommand(sql_item, Connection());

                int rows = command.ExecuteNonQuery();

                if (rows > 0)
                {
                    Log("Added new product to database: ID = " + PrdDetails.Prd_CeneoID);
                }
                else
                {
                    Log("Product arleady exist in database..\n" +
                        "Checking for new reviews...");
                }


                ProgressText = "Done";

            }
            catch (Exception exa)
            {
                Console.WriteLine(exa.ToString());
            }


            int i = 0;
            int j = 0;

            ProgressText = 1 + "/" + DbReviews.Count;

            var tmpCol = new List<DbReview>(DbReviews);

            foreach (DbReview r in tmpCol)
            {
                j++;

                ProgressText = j + "/" + DbReviews.Count;

                try
                {
                    string sql =

                        "INSERT INTO Reviews( " +
                        "Rev_CeneoID, " +
                        "Rev_PrdID, " +
                        "Rev_Advantages, " +
                        "Rev_Defects, " +
                        "Rev_Summary , " +
                        "Rev_Rating, " +
                        "Rev_Reviewer, " +
                        "Rev_Date, " +
                        "Rev_Recom, " +
                        "Rev_UpVotes, " +
                        "Rev_DownVotes, " +
                        "Rev_Content) " +

                        "SELECT " +
                            r.Rev_CeneoID + ", " +
                            PrdDetails.Prd_CeneoID + ", " +
                            "'" + r.Rev_Advantages + "', " +
                            "'" + r.Rev_Defects + "', " +
                            "'" + r.Rev_Summary + "', " +
                            "'" + r.Rev_Rating + "', " +    
                            "'" + r.Rev_Reviewer + "', " +
                            "'" + r.Rev_Date + "', " +
                            "'" + r.Rev_Recom + "', " +
                            "" + r.Rev_UpVotes + ", " +
                            "" + r.Rev_DownVotes + ", " +
                            " @Val_Review " +

                        " WHERE NOT EXISTS(SELECT 1 FROM Reviews WHERE Rev_CeneoID = " + r.Rev_CeneoID +
                        " ) ";

                    SQLiteCommand command = new SQLiteCommand(sql, Connection());
                    command.Parameters.Add("@Val_Review", DbType.String).Value = r.Rev_Content;

                    int rows = command.ExecuteNonQuery();


                    if (rows != 0)
                    {

                        r.IsReviewInserted = true;
                        Log("Review " + r.Rev_CeneoID + " INSERTED");
                        i++;
                    } else
                    {
                        Log("Review " + r.Rev_CeneoID + " is already in database.");
                    }

                    

                }
                catch (Exception exa)
                {
                    Console.WriteLine(exa.ToString());
                }           
                
            }

            ProgressText = "Done";
            
            if (i == 0)
            {
                Log("All Reviews are arleady in Database.");
                statistics.LCount = 0;
            }
            else
            {                

                Log("Inserted " + (i) + " new reviews");

                statistics.LCount = i;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                var xaxa = (DataGrid)Application.Current.MainWindow.FindName("TransformedData_DataGrid");
                xaxa.Items.Refresh();
            });

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;


            statistics.LTime = elapsedMs;

            MessageBox.Show(
                "Extract time: " + TimeSpan.FromMilliseconds(statistics.ETime) + "\n" +
                "Transform time: " + TimeSpan.FromMilliseconds(statistics.TTime) + "\n" +
                "Load time: " + TimeSpan.FromMilliseconds(statistics.LTime) + "\n" +
                "Extracted items count: " + statistics.ECount + "\n" +
                "Transformed items count: " + statistics.TCount + "\n" +
                "Loaded items count: " + statistics.LCount,
                "Statistisc"
                );
        }



        public string TransformString(string s)
        {
            // delete all points
            s = s.Replace(@",", "");
            s = s.Replace(@"<br>", "");
            s = s.Replace(@"&#243;", "ó");
            s = Regex.Replace(s, @"\s+", " ");

            return s;
        }
        



        private void HideAllBut(Views view)
        {
            //hide all grids
            DbReviewsVisibility = Visibility.Collapsed;
            DbProductsVisibility = Visibility.Collapsed;
            ProductsVisibility = Visibility.Collapsed;
            ProductWithReviewsVisibility = Visibility.Collapsed;
            TransformedDataVisibility = Visibility.Collapsed;
            WelcomePageVisibility = Visibility.Collapsed;

            //show selected grid
            switch (view)
            {
                case Views.Products:
                    ProductsVisibility = Visibility.Visible;
                    break;
                case Views.ProductWithReviews:
                    ProductWithReviewsVisibility = Visibility.Visible;
                    break;
                case Views.TranformedReviews:
                    TransformedDataVisibility = Visibility.Visible;
                    break;
                case Views.DbProducts:
                    DbProductsVisibility = Visibility.Visible;
                    break;
                case Views.DbReviews:
                    DbReviewsVisibility = Visibility.Visible;
                    break;
                case Views.NoView:
                    break;
                case Views.WelcomePage:
                    WelcomePageVisibility = Visibility.Visible;
                    break;
            }
        }


        public void ExportToCSV()
        {
            var _app = new Excel.Application();
            var workbooks = _app.Workbooks;
            Excel.Workbook sampleWorkbook = workbooks.Add();

            Excel.Worksheet sheet = sampleWorkbook.Sheets.Add();


            for (var i = 1; i <= _dbReviews.Count; i++)
            {
                var r = _dbReviews[i - 1];
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

            _app.Visible = true;

        }

        public void GoToWelcomePageClick()
        {
            HideAllBut(Views.WelcomePage);
        }
    }
}
