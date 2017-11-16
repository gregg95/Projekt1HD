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

namespace Projekt1HD
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private ObservableCollection<HtmlProduct> _productList = new ObservableCollection<HtmlProduct>();
        public ObservableCollection<HtmlProduct> ProductList
        {
            get
            {
                return _productList;
            }
            set
            {
                _productList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Database_Item> _db_items = new ObservableCollection<Database_Item>();
        public ObservableCollection<Database_Item> DB_Items
        {
            get { return _db_items; }
            set
            {
                _db_items = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Database_Review> _db_reviews = new ObservableCollection<Database_Review>();
        public ObservableCollection<Database_Review> DB_Reviews
        {
            get { return _db_reviews; }
            set
            {
                _db_reviews = value;
                OnPropertyChanged();
            }
        }

        private bool _hasStageESucced ;
        public bool HasStageESucced
        {
            get { return _hasStageESucced; }
            set
            {
               
                _hasStageESucced = value;
                

                OnPropertyChanged();
                
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
                OnPropertyChanged();
            }
        }

        private string _pageReviewNumber;
        public string PagerReviewNumber
        {
            get { return _pageReviewNumber; }
            set
            {
                _pageReviewNumber = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }

        SingleProduct sp;
        List<Review> ReviewsList = new List<Review>();

        public string _searchUrl;
        public string _id;

        private List<SingleProduct> _singleProducts;
        public List<SingleProduct> SingleProducts
        {
            get
            {
                return _singleProducts;
            }
            set
            {
                _singleProducts = value;
                OnPropertyChanged();
            }
        }

        private string _productID;

        public HtmlDocument htmlDoc = new HtmlDocument();


        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            this.LoadingSpinner.Visibility = Visibility.Hidden;
        }




        private async void Button_ETL_Click(object sender, RoutedEventArgs e)
        {
            PagerReviewNumber = "";

            this.LoadingSpinner.Visibility = Visibility.Visible;
            ProductList.Clear();

            Log("Starting E process");

            string _key = TextBox_Search.Text;


            if (SearchByID.IsChecked == true)
            {
                _searchUrl = "https://www.ceneo.pl/" + _key + "#tab=reviews";

                //   Search_Specific_Item(_searchUrl);

                
                //E
                    
                Search_Specific_Item(_searchUrl);
                

                //T
                /* Application.Current.Dispatcher.Invoke((Action)delegate
                 {

                     Show_Specific_Item(review);

                 });*/
                //L

                //DataGridItem.Children.Clear();

            }
            else
            {
                _searchUrl = "https://www.ceneo.pl/;szukaj-" + _key;

                Search_List(_searchUrl);
            }




        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        private async void Button_Next_Click(object sender, RoutedEventArgs e)
        {


            ListedResults.Visibility = Visibility.Hidden;
            this.LoadingSpinner.Visibility = Visibility.Visible;

            Int32.TryParse(Regex.Match(CurrentPage, @"\d+").Value, out int c);
            string s = ";0020-30-0-0-" + c + ".htm";
            CurrentPage = "Page: " + (c + 1);

            if (c == PageCount)
            {
                Button_Next.IsEnabled = false;
            }

            if (!Button_Previous.IsEnabled)
            {
                Button_Previous.IsEnabled = true;
            }

            try
            {
                /* WebClient client = new WebClient();
                 var htmlDocu = await client.DownloadStringTaskAsync(_searchUrl + s);

                 HtmlDocument htmlDoc = new HtmlDocument();
                 htmlDoc.LoadHtml(htmlDocu); */

                var htmlDoc = await ConnectWithPageAsync(_searchUrl + s);

                var nodes = htmlDoc.DocumentNode.SelectNodes("//*[contains(@class,'cat-prod-row-name')]");

                ProductList.Clear();

                foreach (HtmlNode node in nodes)
                {
                    HtmlNode aNode = node.SelectSingleNode("./a");

                    if (Regex.IsMatch(aNode.Attributes["href"].Value.Remove(0, 1), @"^[0-9].*"))
                    {
                        ProductList.Add(new HtmlProduct
                        {
                            Name = aNode.InnerText.ToString(),
                            Url = new Uri("https://www.ceneo.pl/" + aNode.Attributes["href"].Value.Remove(0, 1))
                        });
                    }
                    else
                    {

                    }

                }
            }
            catch (NullReferenceException nex)
            {
                //Nothing found
            }


            LoadingSpinner.Visibility = Visibility.Hidden;

            HideAllBut(ListedResults);
        }

        private async void Button_Previous_Click(object sender, RoutedEventArgs e)
        {
            LoadingSpinner.Visibility = Visibility.Visible;
            ListedResults.Visibility = Visibility.Hidden;

            Int32.TryParse(Regex.Match(CurrentPage, @"\d+").Value, out int c);
            string s = ";0020-30-0-0-" + (c - 2) + ".htm";
            CurrentPage = "Page: " + (c - 1);

            if (c - 2 == 0)
            {
                Button_Previous.IsEnabled = false;
            }

            if (!Button_Next.IsEnabled)
            {
                Button_Previous.IsEnabled = true;
            }

            try
            {/*
                WebClient client = new WebClient();
                var htmlDocu = await client.DownloadStringTaskAsync(_searchUrl + s);

                HtmlDocument htmlDoc = new HtmlDocument(_searchUrl + s);
                htmlDoc.LoadHtml(htmlDocu);  */

                var htmlDoc = await ConnectWithPageAsync(_searchUrl + s);
                var nodes = htmlDoc.DocumentNode.SelectNodes("//*[contains(@class,'cat-prod-row-name')]");

                ProductList.Clear();

                foreach (HtmlNode node in nodes)
                {
                    HtmlNode aNode = node.SelectSingleNode("./a");

                    if (Regex.IsMatch(aNode.Attributes["href"].Value.Remove(0, 1), @"^[0-9].*"))
                    {
                        ProductList.Add(new HtmlProduct
                        {
                            Name = aNode.InnerText.ToString(),
                            Url = new Uri("https://www.ceneo.pl/" + aNode.Attributes["href"].Value.Remove(0, 1))
                        });
                    }
                    else
                    {

                    }
                }
            }
            catch (NullReferenceException nex)
            {
                //Nothing found
            }

            LoadingSpinner.Visibility = Visibility.Hidden;

            HideAllBut(ListedResults);
        }

        public void Log(string l)
        {
            TextBlock_Log.Text += "\n" + l;
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
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {

                        Log("Connected!");

                    });

                }
                catch (Exception aaa)
                {
                    MessageBox.Show("Error: Probably you don't have access to Internet.");


                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {

                        Log("Failed..No Internet Connection.");

                    });
                }
            });

            return htmlDoc;
        }



        public async void Search_Specific_Item(string _url_address)
        {
            try
            {

                // var asdf = _url_address.Split(new string[] { "https://www.ceneo.pl/" }, StringSplitOptions.None)[1]
                //                 .Split('#')[0].Trim();



                var sss = _url_address;
                var start = sss.IndexOf("ceneo.pl/") + 9;

                _id = sss.Substring(start, sss.IndexOf("#") - start);


                HideAllBut(DataProductAndReviews);


                /*  var htmlWeb = new HtmlWeb();
                  //  var tcs = new TaskCompletionSource<HttpWebResponse>();
                  htmlWeb.OverrideEncoding = Encoding.UTF8;
                  var htmlDoc = await Task.Run(() => htmlWeb.Load(_url_address)); */
                var htmlDoc = await ConnectWithPageAsync(_url_address);


                var hasFound = htmlDoc.DocumentNode.SelectSingleNode("//meta[contains(@property,'og:type')]").Attributes["content"].Value;

                Log("Checking result..");
                if (hasFound == "product.group")
                {

                    //product not found
                    MessageBox.Show("No results for search id");

                }
                else if (hasFound == "product")
                {

                    //product found



                    //id = textbox
                    //<meta property="og:price:currency" content="PLN" />
                    //< meta property = "og:price:amount" content = "980" />
                    //<h1 class="product-name js_product-h1-link js_product-force-scroll js_searchInGoogleTooltip" data-onselect="true" data-tooltip-autowidth="true" itemprop="name" productlink="/39684857#tab=click_scroll">Logitech G29 Racing Wheel (941-000112)</h1>
                    //input class="js_product-review-form-score required" type="hidden" name="rating" value="4.13">



                    /* strony komentarzy
                     * <div class="pagination">
                    <ul>

                                                            <li class="active"><span>1</span></li>
                        <li> <a href="/39684857/opinie-2" >2</a></li>
                        <li> <a href="/39684857/opinie-3" >3</a></li>
                                            <li class="page-arrow arrow-next"><a href="/39684857/opinie-2" >Następna<i data-icon="&#xe003;"></i></a></li>
                    </ul>
                    </div>
                    <span class="product-reviews-link__votes-count">30 głosów</span>
                    <span itemprop="reviewCount">26</span>
                        */
                    //
                    //div[regex-is-match(text(), 'h.llo')]")) 
                    /*
                    < nav class="breadcrumbs">
        <dl>
            <dt>Jesteś tutaj:</dt>
            <dd>

                        <span itemscope itemtype="http://data-vocabulary.org/Breadcrumb" class="breadcrumb"   data-category-id="0"  ><a href = "/" itemprop="url"><span itemprop = "title" > Ceneo </ span ></ a ></ span >
  
                          < span itemscope itemtype = "http://data-vocabulary.org/Breadcrumb" class="breadcrumb"   data-category-id="43"  ><a href = "/Komputery" itemprop="url"><span itemprop = "title" > Komputery </ span ></ a ></ span >
       
                               < span itemscope itemtype = "http://data-vocabulary.org/Breadcrumb" class="breadcrumb"   data-category-id="1627"  ><a href = "/Drukarki_i_skanery" itemprop="url"><span itemprop = "title" > Drukarki i skanery</span></a></span>
                        <span itemscope itemtype= "http://data-vocabulary.org/Breadcrumb" class="breadcrumb"   data-category-id="2823"  ><a href = "/Urzadzenia_wielofunkcyjne" itemprop="url"><span itemprop = "title" > Urządzenia wielofunkcyjne</span></a></span>
                        <span itemscope itemtype="http://data-vocabulary.org/Breadcrumb" class="breadcrumb"   data-category-id="2830"  ><a href = "/Urzadzenia_wielofunkcyjne_laserowe" itemprop="url"><span itemprop = "title" > Urządzenia wielofunkcyjne laserowe</span></a></span>
                        <strong class="js_searchInGoogleTooltip" data-onselect="true" data-tooltip-autowidth="true">Xerox Wc 5024 Dadf Duplex Base Iot 220V(5024V_U)</strong>
            </dd>
        </dl>
    </nav>            */

                    DataGridItem.Children.Clear();
                    sp = new SingleProduct
                    {
                        Id = TextBox_Search.Text,
                        LowerPrice = htmlDoc.DocumentNode.SelectSingleNode("//meta[contains(@property,'og:price:amount')]")?.Attributes["content"].Value 
                                        + " " + htmlDoc.DocumentNode.SelectSingleNode("//meta[contains(@property,'og:price:currency')]")?.Attributes["content"].Value,
                        Name = htmlDoc.DocumentNode.SelectSingleNode("//h1[contains(@class,'product-name js_product-h1-link')]")?.InnerHtml,
                        Rating = htmlDoc.DocumentNode.SelectSingleNode("//input[contains(@name,'rating')]").Attributes["value"].Value + "/5",
                        VotesCount = htmlDoc.DocumentNode.SelectSingleNode("//span[contains(@class,'product-reviews-link__votes-count')]")?.InnerHtml.Split(' ')[0] ?? "0", 
                        ReviewsCount = htmlDoc.DocumentNode.SelectSingleNode("//span[contains(@itemprop,'reviewCount')]")?.InnerHtml ?? "0",
                        Category = htmlDoc.DocumentNode.SelectSingleNode("//nav[contains(@class,'breadcrumbs')]/dl/dd")?.SelectNodes("span")?.Last().SelectSingleNode(".//span[@itemprop='title']")?.InnerHtml ?? "null",
                        Brand = htmlDoc.DocumentNode.SelectSingleNode("//meta[contains(@property,'og:brand')]")?.Attributes["content"].Value ?? ""
                    };

                    Log("Product found! It is: " + sp.Name);

                    StackPanel stp = new StackPanel();
                    stp.Orientation = Orientation.Horizontal;

                    TextBlock text = new TextBlock();
                    text.FontSize = 20;

                    text.Text = "Product ID: " + sp.Id + "\n" +
                                "Name: " + sp.Name + "\n"
                                + "Lower Price: " + sp.LowerPrice + "\n"
                                + "Brand: " + sp.Brand + "\n"
                                + "Rating: " + sp.Rating + "\n"
                                + "Votes Count: " + sp.VotesCount + "\n"
                                + "Category: " + sp.Category + "\n"
                    + "Reviews Count: " + sp.ReviewsCount;

                    stp.Children.Add(text);
                    DataGridItem.Children.Add(stp);


                    var ReviewCount = htmlDoc.DocumentNode.SelectSingleNode("//span[contains(@itemprop,'reviewCount')]").InnerHtml;  //<span itemprop="reviewCount">124</span>
                                                                                                                                     //     ReviewsPageCount.Remove(ReviewsPageCount.Last());

                    Log("Looking for reviews.. ");
                    ReviewsList = new List<Review>();

                    Int32.TryParse(ReviewCount, out int rc);

                    if (rc % 10 != 0)
                    {
                        rc = rc + (10 - (rc % 10));
                    }

                    Log("Found total " + sp.ReviewsCount + " reviews on " + rc / 10 + " pages");


                    var review_no = 0;

                    for (var i = 1; i <= rc / 10; i++)
                    {
                        Log("Reading page " + i);

                        PagerReviewNumber = i.ToString() + " / " + (rc / 10).ToString();

                        var doc = new HtmlDocument();

                        if (i == 1)
                        {
                            doc = htmlDoc;
                        }
                        else
                        {
                            /* var htmlWeb = new HtmlWeb();
                             var tcs = new TaskCompletionSource<HttpWebResponse>();
                             htmlWeb.OverrideEncoding = Encoding.UTF8;
                             var htmlDoc = await Task.Run(() => htmlWeb.Load(_url_address));  */


                            //    WebClient c = new WebClient();

                            var x = _id;

                            _id += "/opinie-" + i;

                            var c = await ConnectWithPageAsync("https://www.ceneo.pl/" + _id);



                            _id = x;

                            doc = c;


                            //  var d = await c.DownloadStringTaskAsync("https://www.ceneo.pl/" + TextBox_Search.Text + "/opinie-" + i);

                            //      doc.LoadHtml(d);
                        }


                        var Reviews = doc.DocumentNode.SelectNodes("//li[contains(@class,'review-box js_product-review')]");


                        foreach (HtmlNode r in Reviews)
                        {
                            review_no += 1;
                            Log("Reading review " + review_no);

                            /*< span class="js_product-review-usefulness vote">
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


                            ReviewsList.Add(review);

                            Show_Specific_Item(review);
                        };




                    }
                    PagerReviewNumber = "Done";
                    LoadingSpinner.Visibility = Visibility.Hidden;

                    sp.Reviews = ReviewsList;

                    HasStageESucced = true;

                    SingleProducts = new List<SingleProduct>
                    {
                        sp
                    };



                    //   Show_Specific_Item();
                }
            }
            catch (Exception e)
            {
                LoadingSpinner.Visibility = Visibility.Hidden;

                Console.WriteLine(e);
            }

        }













        public async void Search_List(string _url_address)
        {
            dataGrid.SelectedCellsChanged += dataGrid_SelectedCellsChanged;

            // DataProductAndReviews.Visibility = Visibility.Collapsed;
            ListedResults.Visibility = Visibility.Collapsed;

            var htmlDoc = await ConnectWithPageAsync(_url_address);

            try
            {

                string _pageCountValue = htmlDoc.DocumentNode.SelectSingleNode("//input[@id='page-counter']").Attributes["data-pageCount"].Value;

                if (_pageCountValue == "0" || _pageCountValue == null || _pageCountValue == "" || _pageCountValue == "1")
                {
                    PageCount = 1;
                    CurrentPage = "Page: 1";
                    Button_Next.IsEnabled = false;

                }
                else
                {
                    Int32.TryParse(_pageCountValue, out int c);
                    PageCount = c;
                    CurrentPage = "Page: 1";
                    Button_Next.IsEnabled = true;
                }

            }
            catch (NullReferenceException nex)
            {
                Console.WriteLine("1 strona" + nex);
                CurrentPage = "";
                PageCount = 1;
                Button_Next.IsEnabled = false;
            }

            try
            {
                /* WebClient client = new WebClient();
                 var htmlDocu = await client.DownloadStringTaskAsync(_searchUrl + s);

                 HtmlDocument htmlDoc = new HtmlDocument();
                 htmlDoc.LoadHtml(htmlDocu); */

                var nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class,'cat-prod-row js_category-list-item js_man-track-event')]");

                ProductList.Clear();

                foreach (HtmlNode node in nodes)
                {
                    HtmlNode aNode = node.SelectSingleNode("./a");

                    var x = new HtmlDocument();
                    x.LoadHtml(node.InnerHtml);

                    if (Regex.IsMatch(x.DocumentNode.SelectSingleNode("//a[contains(@class, ' js_conv')]").Attributes["href"].Value.Remove(0, 1), @"^[0-9].*"))
                    {
                        ProductList.Add(new HtmlProduct
                        {
                            Name = x.DocumentNode.SelectSingleNode("//a[contains(@class, ' js_conv')]").InnerText,
                            Url = new Uri("https://www.ceneo.pl/" + x.DocumentNode.SelectSingleNode("//a[contains(@class, ' js_conv')]").Attributes["href"].Value.Remove(0, 1)),
                            ReviewsCount = (x.DocumentNode.SelectSingleNode("//a[contains(@class, 'product-reviews-link dotted-link js_reviews-link')]") != null) ?
                                            x.DocumentNode.SelectSingleNode("//a[contains(@class, 'product-reviews-link dotted-link js_reviews-link')]").InnerText : null

                        });
                    }
                    else
                    {

                    }

                }

                if (nodes.Count != 1)
                {
                    HideAllBut(ListedResults);
                }


                Button_Previous.IsEnabled = false;

                HideAllBut(ListedResults);
            }
            catch (NullReferenceException nex)
            {
                //Nothing found
            }



            this.LoadingSpinner.Visibility = Visibility.Hidden;


        }

        public void Hide_Content()
        {

        }

        public void Show_Specific_Item(Review r)
        {
            HideAllBut(DataProductAndReviews);


            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;

            Thickness margin = sp.Margin;
            margin.Left = 10;
            margin.Top = 10;
            margin.Right = 10;
            sp.Margin = margin;


            TextBlock Reviewer = new TextBlock();
            Reviewer.Width = 200;
            Reviewer.FontSize = 15;

            r.Reviewer = Regex.Replace(r.Reviewer, @"\s+", " ");

            Reviewer.Text = "Reviewer: " + r.Reviewer + "\n"
                            + "Data: " + r.Date.ToString() + "\n"
                            + "Rating: " + r.Rating + "\n"
                            + r.Product_Recommended + "\n"
                            + "Votes up: " + r.Votes_Yes.ToString() + "\n"
                            + "Votes down: " + r.Votes_No.ToString();




            sp.Children.Add(Reviewer);


            if (r.Advantages.Count > 0)
            {
                DataGrid LVa = new DataGrid();


                LVa.ItemsSource = r.Advantages.Select(x => new { Advantages = x }).ToList();

                LVa.IsReadOnly = true;

                sp.Children.Add(LVa);
            }


            if (r.Defects.Count > 0)
            {

                DataGrid LVd = new DataGrid();


                LVd.ItemsSource = r.Defects.Select(x => new { Defects = x }).ToList(); ;

                LVd.IsReadOnly = true;

                sp.Children.Add(LVd);
            }

            //    StackPanel sp_reviewsAndAnswers = new StackPanel();

            TextBlock RText = new TextBlock();
            RText.TextWrapping = TextWrapping.WrapWithOverflow;
            RText.Background = Brushes.LightSteelBlue;
            RText.MaxWidth = 500;
            RText.Text = r.Review_Text;


            // sp_reviewsAndAnswers.Children.Add(RText);

            RText.HorizontalAlignment = HorizontalAlignment.Right;

            sp.Children.Add(RText);

            if (r.Comments.Count > 0)
            {
                StackPanel sp_com = new StackPanel();
                sp_com.Orientation = Orientation.Vertical;
                sp_com.HorizontalAlignment = HorizontalAlignment.Right;

                foreach (Comment c in r.Comments)
                {


                    TextBlock CText = new TextBlock();
                    CText.TextWrapping = TextWrapping.WrapWithOverflow;
                    CText.Background = Brushes.LightCyan;
                    CText.MaxWidth = 500;
                    CText.Text = c.CommentString;


                    sp_com.Children.Add(CText);


                    if (c.Replies.Count > 0 || c.Replies != null)
                    {
                        foreach (CommentReply rep in c.Replies)
                        {
                            TextBlock RepText = new TextBlock();
                            RepText.TextWrapping = TextWrapping.WrapWithOverflow;
                            RepText.Background = Brushes.LightGreen;
                            RepText.TextAlignment = TextAlignment.Right;
                            RepText.MaxWidth = 500;
                            RepText.Text = rep.Content;

                            sp_com.Children.Add(RepText);
                        }
                    }


                }

                sp.Children.Add(sp_com);
            }

            DataGridItem.Children.Add(sp);
        }



        public void Show_Listed_Items()
        {

        }

        private void Button_E_Click(object sender, RoutedEventArgs e)
        {
            // _searchUrl = "https://www.ceneo.pl/" + _key + "#tab=reviews";
            Search_Specific_Item("https://www.ceneo.pl/" + TextBox_Search.Text + "#tab=reviews");
            //Downloading data from web page. 




            /*SqlConnection myConnection = new SqlConnection("server=(LocalDB)\\LocalDB;" +
                                       "Trusted_Connection=yes;" +
                                       "database=DB_ProjektHD; " +
                                       "MultipleActiveResultSets=True;");
            try
            {
                myConnection.Open();
                

                Log("Inserting to database new item");

                SqlCommand myCommand = new SqlCommand("INSERT INTO DB_ProjektHD.dbo.Items (Item_type, Item_brand, Item_model, Additional_comments) " +
                                     "Values ('string', 'brand', 'model', 'comment')", myConnection);


                myCommand.ExecuteNonQuery();

            }
            catch (Exception ea)
            {
                Console.WriteLine(ea.ToString());
            }*/

        }

        private void Button_T_Click(object sender, RoutedEventArgs e)
        {
            //Transform data <3

            StageT();

        }

        private void StageT()
        {
            DB_Reviews.Clear();

            ReviewsList.ForEach(x => DB_Reviews
                .Add(new Database_Review()
                {
                    Product_recommend = (x.Product_Recommended == "") ? (bool?)null : (x.Product_Recommended == "Polecam") ? true : false,
                    Advantages = (x.Advantages.Count != 0) ? TransformData(String.Join("/", x.Advantages.ToArray())) : null,
                    Defects = (x.Defects.Count != 0) ? TransformData(String.Join("/", x.Defects.ToArray())) : null,
                    Review = TransformData(x.Review_Text),
                    Rating = x.Rating.Replace(@",", "."),
                    Reviewer = TransformData(x.Reviewer),
                    Review_CeneoID = Convert.ToInt32(x.Review_ID),
                    Review_date = DateTime.Parse(x.Date),
                    Item_ID = Convert.ToInt32(sp.Id),
                    Votes_up = (x.Votes_Yes != "") ? Convert.ToInt32(x.Votes_Yes) : (int?)null,
                    Votes_down = (x.Votes_Yes != "") ? Convert.ToInt32(x.Votes_Yes) : (int?)null


                }));
        }

        public string TransformData(string s)
        {
            // delete all points
            s = s.Replace(@",", "");
            s = s.Replace(@"<br>", "");
            s = s.Replace(@"&#243;", "ó");
            s = Regex.Replace(s, @"\s+", " ");

            return s;
        }


        private void Button_L_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //  ListedResults.Visibility = Visibility.Collapsed;
            LoadingSpinner.Visibility = Visibility.Visible;

            Search_Specific_Item(((HtmlProduct)dataGrid.CurrentCell.Item).Url.ToString());

            dataGrid.SelectedCellsChanged -= dataGrid_SelectedCellsChanged;

            DataGridItem.Children.Clear();


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

        private void Button_Show_Data_Click(object sender, RoutedEventArgs e)
        {
            //Show list of items in database
            SqlConnection myConnection = new SqlConnection("server=(LocalDB)\\LocalDB;" +
                                      "Trusted_Connection=yes;" +
                                      "database=DB_ProjektHD; " +
                                      "MultipleActiveResultSets=True;");

            _db_items.Clear();

            try
            {
                myConnection.Open();

                Log("Connected to database...");
                SqlDataReader myReader = null;

                SqlCommand myCommand = new SqlCommand(
                    "Select * FROM DB_ProjektHD.dbo.Items"
                    , myConnection);

                myReader = myCommand.ExecuteReader();

                Log("Reading stored products.");

                while (myReader.Read())
                {
                    _db_items.Add(new Database_Item()
                    {
                        Item_ID = (int)myReader["Item_ID"],
                        Item_brand = myReader["Item_brand"].ToString(),
                        Item_model = myReader["Item_model"].ToString(),
                        Item_type = myReader["Item_type"].ToString(),
                        Additional_comments = myReader["Additional_comments"].ToString()

                    });

                    Log("Loaded item: ID = " + (int)myReader["Item_ID"]);
                }
            }
            catch (Exception ea)
            {
                Console.WriteLine(ea.ToString());
            }

            HideAllBut(ListOfItemsInDatabase);

        }

        private void Button_Clear_Database_Click(object sender, RoutedEventArgs e)
        {

            var upDir = Directory.GetParent("Database").FullName;
            SQLiteConnection con = new SQLiteConnection("Data Source=" + upDir + "\\ProjektHD.db; Version=3;");
            

            try
            {
                con.Open();


                string sql = "SELECT * FROM  Reviews";

                SQLiteCommand command = new SQLiteCommand(sql, con);

                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    Console.WriteLine("Name: " + reader["Item_ID"] + "\tScore: " + reader["Review_ID"]);


                /*
                 SQLiteDataReader reader = myCommand.ExecuteReader();

                 myCommand.ExecuteNonQuery();

                 if (rowsAffected == 0)
                 {
                     Log("Table Reviews is empty");
                 }
                 else
                 {
                     Log("Deleted " + rowsAffected.ToString() + " rows from table Reviews");
                 }  */
            }
            catch (Exception exa)
            {
                Console.WriteLine(exa.ToString());
            }

        }


        private void HideAllBut(Grid grid)
        {
            //hide all grids
            ReviewsForItemInDatabase.Visibility = Visibility.Collapsed;
            DataProductAndReviews.Visibility = Visibility.Collapsed;
            ListOfItemsInDatabase.Visibility = Visibility.Collapsed;
            ListedResults.Visibility = Visibility.Collapsed;

            //show selected grid
            grid.Visibility = Visibility.Visible;
        }

        private void DataGridItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as DataGrid;
            var col = grid.CurrentColumn.Header;
            var cellValue = grid.CurrentItem as Database_Item;
            var id = cellValue.Item_ID;




            SqlConnection myConnection = new SqlConnection("server=(LocalDB)\\LocalDB;" +
                                      "Trusted_Connection=yes;" +
                                      "database=DB_ProjektHD; " +
                                      "MultipleActiveResultSets=True;");

            _db_reviews.Clear();

            try
            {
                myConnection.Open();

                Log("Connected to database...");
                SqlDataReader myReader = null;

                SqlCommand myCommand = new SqlCommand(
                    "Select * FROM DB_ProjektHD.dbo.Reviews"
                  + " WHERE Item_ID = " + id
                    , myConnection);

                myReader = myCommand.ExecuteReader();

                Log("Reading stored reviews.");

                while (myReader.Read())
                {
                    _db_reviews.Add(new Database_Review()
                    {
                        Item_ID = (int)myReader["Item_ID"],
                        Review_ID = (int)myReader["Review_ID"],
                        Advantages = myReader["Advantages"].ToString(),
                        Defects = myReader["Defects"].ToString(),
                        Review_summary = myReader["Review_summary"].ToString(),
                        Rating = myReader["Rating"].ToString(),
                        Reviewer = myReader["Reviewer"].ToString(),
                        Review = myReader["Review"].ToString(),
                        Review_CeneoID = (myReader["Review_CeneoID"].ToString() != "") ? (int?)myReader["Review_CeneoID"] : null,
                        Product_recommend = (myReader["Product_recommend"].ToString() != "") ? (bool?)myReader["Product_recommend"] : null,
                        Review_date = (myReader["Review_date"].ToString() != "") ? (DateTime?)myReader["Review_date"] : null,
                        Votes_down = (myReader["Votes_down"].ToString() != "") ? (int?)myReader["Votes_down"] : null,
                        Votes_up = (myReader["Votes_up"].ToString() != "") ? (int?)myReader["Votes_up"] : null

                    });

                    Log("Loaded review: ID = " + (int)myReader["Review_ID"]);
                }
            }
            catch (Exception ea)
            {
                Console.WriteLine(ea.ToString());
            }

            HideAllBut(ReviewsForItemInDatabase);
        }
    }
}