using System;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using Image = SixLabors.ImageSharp.Image;
using Rectangle = SixLabors.ImageSharp.Rectangle;

namespace WseiDziekanatCrawler {
	public class Crawler : IDisposable {
		private ChromeDriver _driver;

		public Crawler() {
			var chromeConfig = new ChromeConfig();
			new DriverManager().SetUpDriver(chromeConfig);
			var service = ChromeDriverService.CreateDefaultService();
			service.SuppressInitialDiagnosticInformation = true;
			service.HideCommandPromptWindow = true;

			var chromeOptions = new ChromeOptions();
#if !DEBUG
			chromeOptions.AddArgument("headless");
#endif
			_driver = new ChromeDriver(service, chromeOptions) {
				Url = "https://dziekanat.wsei.edu.pl/"
			};

		}

		public void Login() {
			var navMenu = _driver.FindElementByClassName("menu");
			var links = navMenu.FindElements(By.TagName("a"));
			var studentLogin = links.Single(a => a.Text == "Studenci");
			_driver.Navigate().GoToUrl(studentLogin.GetProperty("href"));
			var loginField = _driver.FindElement(By.Id("login"));
			var passwordField = _driver.FindElement(By.Id("haslo"));

			loginField.SendKeys("hubertmorawski");
			passwordField.SendKeys("Morasiu2@");

			var captchaImg = _driver.FindElements(By.Id("captchaImg")).FirstOrDefault();
			if (captchaImg != null) {
				var path = Path.GetTempFileName();
				var screenshot = _driver.GetScreenshot();
				screenshot.SaveAsFile(path, ScreenshotImageFormat.Png);
				var point = captchaImg.Location;
				var width = captchaImg.Size.Width;
				var height = captchaImg.Size.Height;
				var cropArea = new Rectangle(point.X, point.Y, width, height);

				using (var image = Image.Load(File.ReadAllBytes(path))) {
					image.Mutate(a => a.Crop(cropArea));
					image.SaveAsPng(path);
				}

				string text;
				using (var ocr = new TesseractOcr()) {
					text = ocr.GetText(path);
				}

				if (text.Length != 5) throw new InvalidOperationException($"Cannot ocr");
				var captchaText = _driver.FindElementById("captcha");
				captchaText.SendKeys(text);
			}

			passwordField.Submit();
		}


		public void Dispose() {
			_driver?.Dispose();
		}
	}
}