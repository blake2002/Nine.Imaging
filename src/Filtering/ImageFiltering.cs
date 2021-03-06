﻿namespace Nine.Imaging.Filtering
{
    using System;
    using System.IO;

    public static class ImageFiltering
    {
        private static readonly IImageSampler defaultSampler = new SuperSamplingSampler();

        // Transform
        public static Image Transform(this Image source, RotationType rotate, FlippingType flip)
            => PerformAction(source, image => ImageBaseOperations.Transform(image, rotate, flip));

        public static Image FlipX(this Image source)
            => Transform(source, RotationType.None, FlippingType.Horizontal);

        public static Image FlipY(this Image source)
            => Transform(source, RotationType.None, FlippingType.Vertical);

        public static Image Rotate90(this Image source)
            => Transform(source, RotationType.Rotate90, FlippingType.None);

        public static Image Rotate180(this Image source)
            => Transform(source, RotationType.Rotate180, FlippingType.None);

        public static Image Rotate270(this Image source)
            => Transform(source, RotationType.Rotate270, FlippingType.None);

        // Resize
        public static Image Crop(this Image source, Rectangle bounds)
            => PerformAction(source, image => ImageBaseOperations.Crop(image, bounds));

        public static Image Width(this Image source, int width, int height = -1, StretchMode mode = StretchMode.Fill, IImageSampler sampler = null)
            => Resize(source, width, height >= 0 ? height : (int)Math.Round(width / source.AspectRatio), mode, sampler);

        public static Image Height(this Image source, int height, int width = -1, StretchMode mode = StretchMode.Fill, IImageSampler sampler = null)
            => Resize(source, width >= 0 ? width : (int)Math.Round(height * source.AspectRatio), height, mode, sampler);

        public static Image Resize(this Image source, int size, StretchMode mode = StretchMode.Fill, IImageSampler sampler = null)
        {
            int width = 0;
            int height = 0;

            var ratio = source.AspectRatio;

            if (source.Width > source.Height && ratio > 0)
            {
                width = size;
                height = (int)Math.Round(width / ratio);
            }
            else
            {
                height = size;
                width = (int)Math.Round(height * ratio);
            }

            return Resize(source, width, height, mode, sampler);
        }

        public static Image Resize(this Image source, int width, int height, StretchMode mode = StretchMode.Fill, IImageSampler sampler = null)
        {
            if (mode != StretchMode.Fill) throw new NotImplementedException();

            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            if (width > Image.MaxWidth || width > Image.MaxHeight)
            {
                throw new ArgumentOutOfRangeException(
                    $"Target size '{ width }x{ width }' is bigger then the max allowed size '{ Image.MaxWidth }x{ Image.MaxHeight }'");
            }

            sampler = sampler ?? defaultSampler;
            return PerformAction(source, image => sampler.Sample(image, width, height));
        }

        // Per pixel filtering effects
        public static Image Blur(this Image source, double amount) => source.Filter(new GaussianBlur { Variance = amount });
        public static Image Tint(this Image source, Color tint, bool hsb = false) => source.Filter(new Tint { TintColor = tint, UseHsbSpace = hsb });
        public static Image Gray(this Image source) => source.Filter(new Grayscale());
        public static Image Invert(this Image source) => source.Filter(new Inverter());
        public static Image Brightness(this Image source, int amount) => source.Filter(new Brightness(amount));
        public static Image Contrast(this Image source, int amount) => source.Filter(new Contrast(amount));
        public static Image Circle(this Image source, double radius = -1) => source.Filter(new CropCircle { Radius = radius });

        // Edge detection
        public static Image Prewitt(this Image source) => source.Filter(new Prewitt());
        public static Image Sobel(this Image source) => source.Filter(new Sobel());

        // Formatting
        public static Stream Jpg(this Image source, int quality = 80) => FillStream(ms => source.SaveAsJpeg(ms, quality));
        public static Stream Png(this Image source) => FillStream(ms => source.SaveAsPng(ms));

        private static Stream FillStream(Action<Stream> fill)
        {
            var ms = new MemoryStream();
            fill(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public static Image Filter(this Image source, IImageFilter filter) => Filter(source, source.Bounds, filter);
        public static Image Filter(this Image source, Rectangle rectangle, IImageFilter filter)
        {
            return PerformAction(source, image => filter.Apply(image, rectangle));
        }

        public static Image Filter(this Image source, params IImageFilter[] filters) => Filter(source, source.Bounds, filters);
        public static Image Filter(this Image source, Rectangle rectangle, params IImageFilter[] filters)
        {
            foreach (IImageFilter filter in filters)
            {
                source = PerformAction(source, image => filter.Apply(image, rectangle));
            }
            return source;
        }

        private static Image PerformAction(Image source, Func<Image, Image> action)
        {
            var animatedImage = source as AnimatedImage;
            if (animatedImage == null)
            {
                return action(source);
            }

            var frames = new Image[animatedImage.Frames.Count];

            for (var i = 0; i < frames.Length; i++)
            {
                frames[i] = action(animatedImage.Frames[i]);
            }

            return new AnimatedImage(animatedImage.FrameDuration, frames);
        }
    }
}
