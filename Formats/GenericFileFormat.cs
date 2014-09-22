using System;
using System.IO;
using System.Text;

namespace GodLesZ.Library.Formats {

	/// <summary>
	/// Provides generic logic for reading binary file formats.
	/// </summary>
	public abstract class GenericFileFormat : IFileFormat, IDisposable {

		/// <summary>
		/// Stream reader object
		/// </summary>
		public BinaryReader Reader {
			get;
			protected set;
		}

		/// <summary>
		/// Stream writer object
		/// </summary>
		public BinaryWriter Writer {
			get;
			protected set;
		}

		/// <summary>
		/// Returns the underlying reader stream
		/// </summary>
		public Stream ReaderBaseStream {
			get { return Reader.BaseStream; }
		}

		/// <summary>
		/// Returns the underlying writer stream
		/// </summary>
		public Stream WriterBaseStream {
			get { return Writer.BaseStream; }
		}

		/// <summary>
		/// Gets or sets the filepath of the current file.
		/// </summary>
		public string Filepath {
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets a file version, used to identify various file versions.
		/// </summary>
		public GenericFileFormatVersion Version {
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets the encoding for reading and writing
		/// </summary>
		public Encoding Encoding {
			get;
			protected set;
		}

		/// <summary>
		/// Event to be triggered before the used stream resources will be flushed.
		/// Should be used to flush custom resources like images objects or release file streams.
		/// Reader and writer streams will be flushed after the event has ben fired.
		/// </summary>
		public event OnFlushHandler OnFlush;


		/// <summary>
		/// If implemented, creates a new instance without any filepath and uses the <see cref="Encoding.Default"/> encoding for
		/// further reading and writing
		/// </summary>
		public GenericFileFormat()
			: this("") {
		}

		/// <summary>
		/// If implemented, creates a new instance without any filepath and the given encoding for further reading and writing
		/// <param name="enc">The used encoding for reading and writing</param>
		/// </summary>
		public GenericFileFormat(Encoding enc)
			: this("", enc) {
		}

		/// <summary>
		/// If implemented, creates a new instance with the given filepath and used the <see cref="Encoding.Default"/> 
		/// encoding for further reading and writing.
		/// The <see cref="Read()"/> method will be called automatically.
		/// <param name="filepath">The filepath to read from</param>
		/// </summary>
		public GenericFileFormat(string filepath)
			: this(filepath, null) {
		}

		/// <summary>
		/// If implemented, creates a new instance with the given filepath and encoding for reading and writing.
		/// The <see cref="Read()"/> method will be called automatically.
		/// <param name="filepath">The filepath to read from</param>
		/// <param name="enc">The used encoding for reading and writing</param>
		/// </summary>
		public GenericFileFormat(string filepath, Encoding enc) {
			Encoding = enc ?? Encoding.Default;

			if (string.IsNullOrEmpty(filepath)) {
				return;
			}

			if (File.Exists(filepath) == false) {
				throw new ArgumentException("File not found: " + filepath);
			}

			Read(filepath);
		}

		/// <summary>
		/// If implemented, creates a new instance with the given filepath and used the <see cref="Encoding.Default"/> 
		/// encoding for further reading and writing.
		/// The <see cref="Read()"/> method will be called automatically.
		/// <param name="stream">The stream to read from</param>
		/// </summary>
		public GenericFileFormat(Stream stream)
			: this(stream, null) {
		}

		/// <summary>
		/// If implemented, creates a new instance with the given filepath and encoding for reading and writing.
		/// The <see cref="Read()"/> method will be called automatically.
		/// <param name="stream">The stream to read from</param>
		/// <param name="enc">The used encoding for reading and writing</param>
		/// </summary>
		public GenericFileFormat(Stream stream, Encoding enc) {
			Encoding = enc ?? Encoding.Default;

			if (stream != null) {
				Read(stream);
			}
		}

		~GenericFileFormat() {
			Dispose();
		}


		/// <summary>
		/// Trys to dispose the given object using the <see cref="IDisposable.Dispose"/> method or
		/// <see cref="IGenericFileFormatData.Dispose"/> method.
		/// The object references will be deleted.
		/// </summary>
		/// <param name="obj"></param>
		public static void aFree(object obj) {
			if (obj == null) {
				return;
			}

			if (obj is IDisposable) {
				((IDisposable)obj).Dispose();
			} else if (obj is IGenericFileFormatData) {
				((IGenericFileFormatData)obj).Dispose();
			}

			obj = null;
		}


		#region Read
		/// <summary>
		/// Trys to read the current file.
		/// </summary>
		public virtual bool Read() {
			if (Reader == null) {
				return false;
			}
			if (Encoding == null) {
				Encoding = Encoding.Default;
			}

			return ReadInternal();
		}

		/// <summary>
		/// Trys to read the given stream.
		/// <param name="stream">The stream to read from</param>
		/// </summary>
		public virtual bool Read(Stream stream) {
			if (stream == null || stream.CanRead == false) {
				throw new Exception("Failed to read form stream!");
			}

			Filepath = null;
			Reader = new BinaryReader(stream, Encoding);

			return Read();
		}

		/// <summary>
		/// Trys to read the given filepath.
		/// <param name="filepath">The file to read from</param>
		/// </summary>
		public virtual bool Read(string filepath) {
			if (File.Exists(filepath) == false) {
				throw new ArgumentException("File \"" + filepath + "\" not found!");
			}
			if (Encoding == null) {
				Encoding = Encoding.Default;
			}

			Filepath = filepath;
			Reader = new BinaryReader(File.OpenRead(filepath), Encoding);

			return Read();
		}

		/// <summary>
		/// Internal method to read the file format.
		/// </summary>
		/// <returns></returns>
		protected virtual bool ReadInternal() {
			return false;
		}
		#endregion

		#region Write
		public virtual bool Write(string destinationPath) {
			return Write(destinationPath, true);
		}

		public virtual bool Write(string destinationPath, bool overwrite) {
			if (File.Exists(destinationPath) == true) {
				if (overwrite == false) {
					return false;
				}
				File.Delete(destinationPath);
			}

			Writer = new BinaryWriter(File.OpenWrite(destinationPath), Encoding);
			return true;
		}
		#endregion

		#region Flush
		public virtual void Flush() {
			Flush(false);
		}

		public virtual void Flush(bool OnDestruct) {
			if (OnFlush != null) {
				OnFlush(OnDestruct);
			}

			if (Reader != null) {
				try {
					Reader.Close();
					Reader = null;
				} catch { }
			}
			if (Writer != null) {
				try {
					Writer.Close();
					Writer = null;
				} catch { }
			}
		}
		#endregion

		public void Dispose() {
			Flush(true);
		}

	}

	public delegate void OnFlushHandler(bool OnDestruct);

}
