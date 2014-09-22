using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GodLesZ.Library.PixelSearch {

	public class PixelSearch : IDisposable {
		[DllImport("gdi32.dll")]
		private static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);
		[DllImport("user32.dll")]
		private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);
		[DllImport("gdi32.dll")]
		private static extern IntPtr DeleteDC(IntPtr hDc);
		[DllImport("gdi32.dll")]
		private static extern IntPtr DeleteObject(IntPtr hDc);
		[DllImport("gdi32.dll")]
		private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
		[DllImport("gdi32.dll")]
		private static extern IntPtr CreateCompatibleDC(IntPtr hdc);
		[DllImport("gdi32.dll")]
		private static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
		[DllImport("user32.dll")]
		private static extern IntPtr GetDesktopWindow();
		[DllImport("user32.dll")]
		private static extern IntPtr GetWindowDC(IntPtr ptr);


		public static Point PointNega = new Point(-1, -1);
		public static PixelFormat DefaultCaptureFormat = PixelFormat.Format24bppRgb;

		private BackgroundWorker mBackgroundWorker;

		public bool IsBusy {
			get { return mBackgroundWorker != null && mBackgroundWorker.IsBusy; }
		}

		public bool Locked {
			get;
			set;
		}

		public Size CaptureSize {
			get;
			set;
		}

		public int Delay {
			get;
			set;
		}

		public EPixelPosition PixelPos {
			get;
			set;
		}

		public Point CapturePosition {
			get;
			set;
		}

		public event UpdatePixelinfo_Handler OnUpdate;


		public PixelSearch(Size size, int delay) {
			CaptureSize = size;
			Delay = delay;

			PixelPos = EPixelPosition.Center;
			Locked = false;
			CapturePosition = PointNega;
		}

		public PixelSearch(Size size)
			: this(size, 10) {
		}

		~PixelSearch() {
			Dispose();
		}


		public void Start() {
			if (mBackgroundWorker == null) {
				mBackgroundWorker = new BackgroundWorker();
				mBackgroundWorker.WorkerReportsProgress = true;
				mBackgroundWorker.WorkerSupportsCancellation = true;
				mBackgroundWorker.ProgressChanged += mBackgroundWorker_ProgressChanged;
				mBackgroundWorker.RunWorkerCompleted += mBackgroundWorker_RunWorkerCompleted;
				mBackgroundWorker.DoWork += mBackgroundWorker_DoWork;
			}

			if (mBackgroundWorker.IsBusy == false)
				mBackgroundWorker.RunWorkerAsync(mBackgroundWorker);
		}

		public void Stop() {
			if (mBackgroundWorker == null || IsBusy == false)
				return;
			mBackgroundWorker.CancelAsync();
		}


		public void Dispose() {
			if (mBackgroundWorker == null)
				return;
			try {
				mBackgroundWorker.CancelAsync();
				mBackgroundWorker.Dispose();
				mBackgroundWorker = null;
			} catch { }
		}


		private void mBackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
			var worker = e.Argument as BackgroundWorker;
			var info = new PixelInfo();

			while (true) {
				if (Locked == false) {
					Point mp = (CapturePosition == PointNega ? Control.MousePosition : CapturePosition);
					info.Image = CaptureScreenRegion(mp.X - CaptureSize.Width / 2, mp.Y - CaptureSize.Height / 2, CaptureSize.Width, CaptureSize.Height);
					info.Position = mp;
					switch (PixelPos) {
						case EPixelPosition.TopLeft:
							info.Color = info.Image.GetPixel(0, 0);
							break;
						case EPixelPosition.Top:
							info.Color = info.Image.GetPixel(CaptureSize.Width / 2, 0);
							break;
						case EPixelPosition.TopRight:
							info.Color = info.Image.GetPixel(CaptureSize.Width, 0);
							break;

						case EPixelPosition.CenterLeft:
							info.Color = info.Image.GetPixel(0, CaptureSize.Height / 2);
							break;
						case EPixelPosition.Center:
							info.Color = info.Image.GetPixel(CaptureSize.Width / 2, CaptureSize.Height / 2);
							break;
						case EPixelPosition.CenterRight:
							info.Color = info.Image.GetPixel(CaptureSize.Width, CaptureSize.Height / 2);
							break;

						case EPixelPosition.BottomLeft:
							info.Color = info.Image.GetPixel(0, CaptureSize.Height);
							break;
						case EPixelPosition.Bottom:
							info.Color = info.Image.GetPixel(CaptureSize.Width / 2, CaptureSize.Height);
							break;
						case EPixelPosition.BottomRight:
							info.Color = info.Image.GetPixel(CaptureSize.Width, CaptureSize.Height);
							break;
					}

					worker.ReportProgress(0, info);
				}

				System.Threading.Thread.Sleep(Delay);
			}
		}

		private void mBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
			if (OnUpdate != null)
				OnUpdate(e.UserState as PixelInfo);
		}

		private void mBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {

		}


		public static Point Search(Rectangle rect, Color pixelColor, int shadeVariation, ref Image img) {
			return Search(rect, pixelColor, Color.FromArgb(shadeVariation, shadeVariation, shadeVariation), ref img);
		}

		public static Point Search(Rectangle rect, Color pixelColor, Color shadeVariation, ref Image regionInBitmap) {
			var pixelCoords = PointNega;
			regionInBitmap = CaptureScreenRegion(rect);
			var regionInBitmapData = (regionInBitmap as Bitmap).LockBits(new Rectangle(0, 0, regionInBitmap.Width, regionInBitmap.Height), ImageLockMode.ReadWrite, DefaultCaptureFormat);

			var fromColor = new[] { 
				pixelColor.B - shadeVariation.B, 
				pixelColor.G - shadeVariation.G, 
				pixelColor.R - shadeVariation.R 
			};

			var toColor = new[] { 
				pixelColor.B + shadeVariation.B, 
				pixelColor.G + shadeVariation.G, 
				pixelColor.R + shadeVariation.R
			};

			unsafe {
				for (var y = 0; y < regionInBitmapData.Height; y++) {
					if (pixelCoords != PointNega) {
						break;
					}

					var row = (byte*)regionInBitmapData.Scan0 + (y * regionInBitmapData.Stride);

					for (var x = 0; x < regionInBitmapData.Width; x++) {
						var rowBlue = row[(x * 3) + 0];
						var rowGreen = row[(x * 3) + 1];
						var rowRed = row[(x * 3) + 2];
						if (rowBlue < fromColor[0] || rowBlue > toColor[0]) {
							continue;
						}
						if (rowGreen < fromColor[1] || rowGreen > toColor[1]) {
							continue;
						}
						if (rowRed < fromColor[2] || rowRed > toColor[2]) {
							continue;
						}

						pixelCoords = new Point(x + rect.X, y + rect.Y);
						break;
					}
				}
			}

			(regionInBitmap as Bitmap).UnlockBits(regionInBitmapData);

			return pixelCoords;
		}

		public static Bitmap CaptureScreenRegion(int x, int y, int width, int height) {
			return CaptureScreenRegion(new Rectangle(x, y, width, height));
		}

		public static Bitmap CaptureScreenRegion(Rectangle rect) {
			IntPtr hDesk = GetDesktopWindow();
			IntPtr hSrce = GetWindowDC(hDesk);
			IntPtr hDest = CreateCompatibleDC(hSrce);
			IntPtr hBmp = CreateCompatibleBitmap(hSrce, rect.Width, rect.Height);
			IntPtr hOldBmp = SelectObject(hDest, hBmp);
			BitBlt(hDest, 0, 0, rect.Width, rect.Height, hSrce, rect.X, rect.Y, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);

			var bmp = Image.FromHbitmap(hBmp);

			SelectObject(hDest, hOldBmp);
			DeleteObject(hBmp);
			DeleteDC(hDest);
			ReleaseDC(hDesk, hSrce);

			return bmp;
		}

	}

	public delegate void UpdatePixelinfo_Handler(PixelInfo info);

}
