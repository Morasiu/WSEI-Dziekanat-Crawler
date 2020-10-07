using System;
using Tesseract;

namespace WseiDziekanatCrawler {
	public class TesseractOcr : IDisposable {
		private TesseractEngine engine;

		public TesseractOcr(string tessDataPath = "./tessdata", string lang = "eng") {
			engine = new TesseractEngine(tessDataPath, lang);

		}

		public string GetText(string path) {
			var process = engine.Process(Pix.LoadFromFile(path));
			var text = process.GetText().Trim(Environment.NewLine.ToCharArray());
			return text;
		}

		public void Dispose() {
			engine?.Dispose();
		}
	}
}