﻿// ===============================================================================
// ImageBase.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

namespace Nine.Imaging
{
    using System;

    /// <summary>
    /// Base classes for all Images.
    /// </summary>
    public partial class ImageBase
    {
        #region Properties
        
        /// <summary>
		/// If not 0, this field specifies the number of hundredths (1/100) of a second to 
		/// wait before continuing with the processing of the Data Stream. 
		/// The clock starts ticking immediately after the graphic is rendered. 
		/// This field may be used in conjunction with the User Input Flag field. 
		/// </summary>
        public int? DelayTime { get; set; }

        private byte[] _pixels;
        /// <summary>
        /// Returns all pixels of the image as simple byte array.
        /// </summary>
        /// <value>All image pixels as byte array.</value>
        /// <remarks>The returned array has a length of Width * Length * 4 bytes
        /// and stores the red, the green, the blue and the alpha value for
        /// each pixel in this order.</remarks>
        public byte[] Pixels
        {
            get { return _pixels; }
        }

        private int _pixelHeight;
        /// <summary>
        /// Gets the height of this <see cref="Image"/> in pixels.
        /// </summary>
        /// <value>The height of this image.</value>
        /// <remarks>The height will be initialized by the constructor
        /// or when the data will be pixel data will set.</remarks>
        public int PixelHeight
        {
            get { return _pixelHeight; }
        }

        private int _pixelWidth;
        /// <summary>
        /// Gets the width of this <see cref="Image"/> in pixels.
        /// </summary>
        /// <value>The width of this image.</value>
        /// <remarks>The width will be initialized by the constructor
        /// or when the data will be pixel data will set.</remarks>
        public int PixelWidth
        {
            get { return _pixelWidth; }
        }

        /// <summary>
        /// Gets the ratio between the width and the height of this <see cref="ImageBase"/> instance.
        /// </summary>
        /// <value>The ratio between the width and the height.</value>
        public double PixelRatio
        {
            get { return (double)PixelWidth / PixelHeight; }
        }

        /// <summary>
        /// Gets or sets the color of a pixel at the specified position.
        /// </summary>
        /// <param name="x">The x-coordinate of the pixel. Must be greater
        /// than zero and smaller than the width of the pixel.</param>
        /// <param name="y">The y-coordinate of the pixel. Must be greater
        /// than zero and smaller than the width of the pixel.</param>
        /// <value>The color of the pixel.</value>
        /// <exception cref="ArgumentException">
        ///     <para><paramref name="x"/> is smaller than zero or greater than
        ///     the width of the image.</para>
        ///     <para>- or -</para>
        ///     <para><paramref name="y"/> is smaller than zero or greater than
        ///     the height of the image.</para>
        /// </exception>
        public Color this[int x, int y]
        {
            get
            {
                int start = (y * PixelWidth + x) * 4;

                return new Color(_pixels[start + 3], _pixels[start + 0], _pixels[start + 1], _pixels[start + 2]);
            }
            set
            {
                int start = (y * PixelWidth + x) * 4;

                _pixels[start + 0] = value.R;
                _pixels[start + 1] = value.G;
                _pixels[start + 2] = value.B;
                _pixels[start + 3] = value.A;
            }
        }

        /// <summary>
        /// Calculates a new rectangle which represents 
        /// the dimensions of this image.
        /// </summary>
        /// <value>The <see cref="Rectangle"/> object, which
        /// represents the image dimension.</value>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(0, 0, PixelWidth, PixelHeight);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBase"/> class
        /// with the height and the width of the image.
        /// </summary>
        /// <param name="width">The width of the image in pixels.</param>
        /// <param name="height">The height of the image in pixels.</param>
        /// <exception cref="ArgumentException">
        ///     <para><paramref name="width"/> is equals or less than zero.</para>
        ///     <para>- or -</para>
        ///     <para><paramref name="height"/> is equals or less than zero.</para>
        /// </exception>
        public ImageBase(int width, int height)
        {
            if (width < 0) throw new ArgumentException("Width must be greater or equals than zero.");
            if (height < 0) throw new ArgumentException("Height must be greater or equals than zero.");

            _pixelWidth = width;
            _pixelHeight = height;

            _pixels = new byte[PixelWidth * PixelHeight * 4];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBase"/> class
        /// by making a copy from another image.
        /// </summary>
        /// <param name="other">The other, where the clone should be made from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentException"><paramref name="other"/> is not loaded.</exception>
        public ImageBase(ImageBase other)
        {
            if (other == null) throw new ArgumentNullException("Other image cannot be null.");

            byte[] pixels = other.Pixels;

            _pixelWidth  = other.PixelWidth;
            _pixelHeight = other.PixelHeight;
            _pixels = new byte[pixels.Length];

            Array.Copy(pixels, _pixels, pixels.Length);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBase"/> class.
        /// </summary>
        public ImageBase()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the pixel array of the image.
        /// </summary>
        /// <param name="width">The new width of the image.
        /// Must be greater than zero.</param>
        /// <param name="height">The new height of the image.
        /// Must be greater than zero.</param>
        /// <param name="pixels">The array with colors. Must be a multiple
        /// of four, width and height.</param>
        /// <exception cref="ArgumentException">
        /// 	<para><paramref name="width"/> is smaller than zero.</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="height"/> is smaller than zero.</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="pixels"/> is not a multiple of four, 
        /// 	width and height.</para>
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="pixels"/> is null.</exception>
        public void SetPixels(int width, int height, byte[] pixels)
        {
            if (width < 0) throw new ArgumentException("Width must be greater or equals than zero.");
            if (height < 0) throw new ArgumentException("Height must be greater or equals than zero.");
            if (pixels.Length != width * height * 4) throw new ArgumentException("Pixel array must have the length of width * height * 4.");

            _pixelWidth  = width;
            _pixelHeight = height;
            _pixels = pixels;
        }

        #endregion
    }
}
