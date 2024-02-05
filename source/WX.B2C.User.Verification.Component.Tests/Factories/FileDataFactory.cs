using System;
using System.IO;
using System.Linq;
using Bogus;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Factories
{
    internal class FileDataFactory
    {
        private readonly Faker _faker;

        private static readonly FileMetaData[] Data =
        {
            new()
            {
                Path = Path.Combine(Content.PathToFolder, Content.PngFile),
                ContentType = "image/png",
                Extensions = new[] { "png" }
            },
            new()
            {
                Path = Path.Combine(Content.PathToFolder, Content.JpgFile),
                ContentType = "image/jpeg",
                Extensions = new[] { "jpeg", "jpg" }
            },
            new()
            {
                Path = Path.Combine(Content.PathToFolder, Content.PdfFile),
                ContentType = "application/pdf",
                Extensions = new[] { "pdf" }
            },
            new()
            {
                Path = Path.Combine(Content.PathToFolder, Content.Mp4File),
                ContentType = "video/mp4",
                Extensions = new[] { "mp4" }
            }
        };

        public FileDataFactory(Seed seed)
        {
            _faker = FakerFactory.Create(seed);
        }

        /// <summary>
        /// Create every time distinct file (byte array) 
        /// Achieve it by changing few bytes in middle of array
        /// Such solution works properly when upload file to b2c.verification and to onfido
        /// </summary>
        public FileData Create(string name, string extension, long? length = null)
        {
            var data = Data.FirstOrDefault(x => x.Extensions.Contains(extension))
                       ?? throw new NotSupportedException($"Template for extension { extension } was not found.");

            using var fileTemplate = new FileStream(data.Path, FileMode.Open, FileAccess.Read, FileShare.Read);

            var stream = new MemoryStream();
            fileTemplate.CopyTo(stream);
            var bytes = stream.ToArray();
            stream.Seek(0, SeekOrigin.Begin);

            const int Offset = 100;
            const int Count = 100;
            Enumerable.Range(Offset, Count)
                      .Foreach(i => bytes[i] = (byte) _faker.Random.Number(byte.MinValue, byte.MaxValue));

            stream.Write(bytes, 0, bytes.Count());

            if (length.HasValue)
                stream.SetLength(length.Value);

            stream.Seek(0, SeekOrigin.Begin);

            var result = new FileData(stream, name + "." + extension, extension, data.ContentType);
            return result;
        }

        private class FileMetaData
        {
            public string Path { get; set; }

            public string ContentType { get; set; }

            public string[] Extensions { get; set; }
        }
    }
}
