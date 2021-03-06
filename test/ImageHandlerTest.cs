﻿namespace Nine.Imaging.Http.Test
{
    using Xunit;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Nine.Imaging.Test;

    public class ImageHandlerTest
    {
        [Theory]
        [InlineData("testimages/car.bmp")]
        [InlineData("testimages/car.bmp?")]

        [InlineData("testimages/car.bmp?width=100")]
        [InlineData("testimages/car.bmp?height=100")]
        [InlineData("testimages/car.bmp?width=100&height=100")]

        [InlineData("testimages/portrait.png?flipx")]
        [InlineData("testimages/portrait.png?tint=CCC&hsb=true")]

        [InlineData("testimages/car.bmp?jpg")]
        [InlineData("testimages/car.bmp?width=100&height=100&jpg=20")]

        [InlineData("testimages/car.bmp?width=100&height=100&blur=2")]
        public async Task image_handler_rest_api_test(string url)
        {
            var handler = new ImageHandler("api", new MemoryImageCache(), new LocalImageLoader()) { InnerHandler = new HttpClientHandler() };
            var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost/api/local/") };

            var response = await client.GetAsync(url);
            var content = await response.EnsureSuccessStatusCode().Content.ReadAsStreamAsync();

            var image = Image.Load(content);
            image.VerifyAndSave($"TestResult/Http/{ Uri.EscapeDataString(url) }.png");
        }
    }
}