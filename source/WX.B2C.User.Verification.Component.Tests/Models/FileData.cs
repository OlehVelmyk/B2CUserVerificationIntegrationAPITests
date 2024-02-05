using System;
using System.IO;

namespace WX.B2C.User.Verification.Component.Tests.Models
{
    internal class FileData : IDisposable
    {
        private readonly Stream _data;

        public FileData(Stream data, string fileName, string extension, string contentType)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            Extension = extension ?? throw new ArgumentNullException(nameof(extension));
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
        }

        public string FileName { get; }

        public string ContentType { get; }

        public string Extension { get; }

        public Stream GetDataCopy()
        {
            var copy = new MemoryStream();
            _data.CopyTo(copy);

            _data.Seek(0, SeekOrigin.Begin);
            copy.Seek(0, SeekOrigin.Begin);

            return copy;
        }

        public byte[] GetRaw()
        {
            var copy = new MemoryStream();
            _data.CopyTo(copy);

            _data.Seek(0, SeekOrigin.Begin);
            copy.Seek(0, SeekOrigin.Begin);

            return copy.ToArray();
        }

        public void Dispose() => _data.Dispose();
    }
}