using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace GodLesZ.Library {

	public unsafe class FastBitmap : IDisposable {
		public struct PixelData {
			public byte blue;
			public byte green;
			public byte red;
			public byte alpha;

			public override string ToString() {
				return "(" + alpha.ToString() + ", " + red.ToString() + ", " + green.ToString() + ", " + blue.ToString() + ")";
			}
		}

		protected Bitmap mWorkingBitmap = null;
		protected int mWidth = 0;
		protected BitmapData mBitmapData = null;
		protected byte* mPointer = null;
		protected PixelData* mCurrentPixelData = null;

		public byte* Pointer {
			get { return mPointer; }
		}


		public FastBitmap(Bitmap inputBitmap)
			: this(inputBitmap, false) {
		}

		public FastBitmap(Bitmap inputBitmap, bool lockImage) {
			mWorkingBitmap = inputBitmap;

			if (lockImage == true) {
				LockImage();
			}
		}


		public void Dispose() {
			UnlockImage();
		}


		public void LockImage() {
			Rectangle bounds = new Rectangle(Point.Empty, mWorkingBitmap.Size);

			mWidth = (int)(bounds.Width * sizeof(PixelData));
			if (mWidth % 4 != 0)
				mWidth = 4 * (mWidth / 4 + 1);

			// Lock Image
			mBitmapData = mWorkingBitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
			mPointer = (Byte*)mBitmapData.Scan0.ToPointer();
		}

		public void UnlockImage() {
			if (mBitmapData != null) {
				mWorkingBitmap.UnlockBits(mBitmapData);
				mBitmapData = null;
				mPointer = null;
			}
		}


		public Color GetPixel(int x, int y) {
			mCurrentPixelData = (PixelData*)(Pointer + y * mWidth + x * sizeof(PixelData));
			return Color.FromArgb(mCurrentPixelData->alpha, mCurrentPixelData->red, mCurrentPixelData->green, mCurrentPixelData->blue);
		}

		public Color GetPixelNext() {
			mCurrentPixelData++;
			return Color.FromArgb(mCurrentPixelData->alpha, mCurrentPixelData->red, mCurrentPixelData->green, mCurrentPixelData->blue);
		}

		public void SetPixel(int x, int y, Color color) {
			PixelData* data = (PixelData*)(Pointer + (y * mWidth) + (x * sizeof(PixelData)));
			data->alpha = color.A;
			data->red = color.R;
			data->green = color.G;
			data->blue = color.B;
		}

	}

}
