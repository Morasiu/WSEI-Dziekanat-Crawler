using System.Windows;
using WseiDziekanatCrawler;

namespace CrawlerApp {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		private Crawler _crawler;

		public MainWindow() {
			InitializeComponent();
			Loaded += OnLoaded;
			Closed += (sender, args) => { _crawler.Dispose(); };
		}

		private async void OnLoaded(object sender, RoutedEventArgs e) {
			_crawler = new Crawler();
			_crawler.Login();
		}
	}
}