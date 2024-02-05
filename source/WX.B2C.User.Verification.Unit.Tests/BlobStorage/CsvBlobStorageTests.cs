using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;
using FluentAssertions;
using FsCheck;
using NSubstitute;
using NUnit.Framework;
using WX.B2C.User.Verification.BlobStorage.Storages;
using IgnoreAttribute = CsvHelper.Configuration.Attributes.IgnoreAttribute;

namespace WX.B2C.User.Verification.Unit.Tests.BlobStorage
{
    internal class CsvBlobStorageTests
    {
        private ICsvBlobStorage _sut;
        private IBlobStorage _blobStorage;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _blobStorage = Substitute.For<IBlobStorage>();
            _sut = new CsvBlobStorage(_blobStorage);
        }

        [Test]
        public async Task ShoulReturnEntiites()
        {
            // Arrange
            var path = Directory.GetCurrentDirectory() + "/BlobStorage/Sources/some.csv";
            using var stream = (Stream) new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            _blobStorage.DownloadAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(stream));

            // Act
            var result = await _sut.GetAsync<CsvSpecimen>(string.Empty, string.Empty);

            // Assert
            var specimen = result.Should().HaveCount(1).And.Subject.First();
            specimen.String.Should().Be("ABC");
            specimen.Int.Should().Be(100);
            specimen.Decimal.Should().Be(5.5m);
            specimen.Bool.Should().Be(true);
            specimen.Guid.Should().Be(new Guid("7acdfb22-ebea-44ba-8a92-d53fa2153fe9"));
            specimen.DateTime.Should().Be(DateTime.Parse("04/28/2022 00:00:00", CultureInfo.InvariantCulture));
        }

        [Test]
        public async Task ShoulReturnEntiites_WhenNullableTypes()
        {
            // Arrange
            var path = Directory.GetCurrentDirectory() + "/BlobStorage/Sources/some.csv";
            using var stream = (Stream)new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            _blobStorage.DownloadAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(stream));

            // Act
            var result = await _sut.GetAsync<NullableCsvSpecimen>(string.Empty, string.Empty);

            // Assert
            var specimen = result.Should().HaveCount(1).And.Subject.First();
            specimen.String.Should().Be("ABC");
            specimen.Int.Should().Be(100);
            specimen.Decimal.Should().Be(5.5m);
            specimen.Bool.Should().Be(true);
            specimen.Guid.Should().Be(new Guid("7acdfb22-ebea-44ba-8a92-d53fa2153fe9"));
            specimen.DateTime.Should().Be(DateTime.Parse("04/28/2022 00:00:00", CultureInfo.InvariantCulture));
        }

        [Test]
        public async Task ShoulReturnEntiityWithNulls()
        {
            // Arrange
            var path = Directory.GetCurrentDirectory() + "/BlobStorage/Sources/null_values.csv";
            using var stream = (Stream)new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            _blobStorage.DownloadAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(stream));

            // Act
            var result = await _sut.GetAsync<NullableCsvSpecimen>(string.Empty, string.Empty);

            // Assert
            var specimen = result.Should().HaveCount(1).And.Subject.First();
            specimen.String.Should().BeNull();
            specimen.Int.Should().BeNull();
            specimen.Decimal.Should().BeNull();
            specimen.Bool.Should().BeNull();
            specimen.Guid.Should().BeNull();
            specimen.DateTime.Should().BeNull();
        }

        [Test]
        public async Task ShoulReturnArrays()
        {
            // Arrange
            var path = Directory.GetCurrentDirectory() + "/BlobStorage/Sources/array.csv";
            using var stream = (Stream)new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            _blobStorage.DownloadAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(stream));

            // Act
            var result = await _sut.GetAsync<ArrayCsvSpecimen>(string.Empty, string.Empty);

            // Assert
            var specimen = result.Should().HaveCount(1).And.Subject.First();
            specimen.Array1.Should().BeEquivalentTo(new[] { 1,2,3 });
            specimen.Array2.Should().BeEquivalentTo(new[] { new Guid("7acdfb22-ebea-44ba-8a92-d53fa2153fe9") });
            specimen.Array3.Should().BeEmpty();
            specimen.Array4.Should().BeNull();
        }

        [Test]
        public async Task ShoulReturnEmptyArray_WhenSourceIsEmpty()
        {
            // Arrange
            var path = Directory.GetCurrentDirectory() + "/BlobStorage/Sources/empty.csv";
            using var stream = (Stream)new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            _blobStorage.DownloadAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(stream));

            // Act
            var result = await _sut.GetAsync<CsvSpecimen>(string.Empty, string.Empty);

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public async Task ShoulReturnEntiityWithDefaultValues_WhenValuesOptional()
        {
            // Arrange
            var path = Directory.GetCurrentDirectory() + "/BlobStorage/Sources/some.csv";
            using var stream = (Stream)new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            _blobStorage.DownloadAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(stream));

            // Act
            var result = await _sut.GetAsync<OptionalCsvSpecimen>(string.Empty, string.Empty);

            // Assert
            var specimen = result.Should().HaveCount(1).And.Subject.First();
            specimen.Optional.Should().Be(default);
            specimen.Ignore.Should().Be(default);
            specimen.EnumValue.Should().Be(CsvTestEnum.B);
        }

        [Test]
        public async Task ShoulReturnEntiityWithEnums()
        {
            // Arrange
            var path = Directory.GetCurrentDirectory() + "/BlobStorage/Sources/enum.csv";
            using var stream = (Stream)new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            _blobStorage.DownloadAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(stream));

            // Act
            var result = await _sut.GetAsync<EnumCsvSpecimen>(string.Empty, string.Empty);

            // Assert
            var specimen = result.Should().HaveCount(1).And.Subject.First();
            specimen.One.Should().Be(CsvTestEnum.A);
            specimen.Array.Should().BeEquivalentTo(new[] { CsvTestEnum.A, CsvTestEnum.B, CsvTestEnum.C });
            specimen.NullArray.Should().BeNull();
            specimen.Null.Should().BeNull();
            specimen.Nullable.Should().Be(CsvTestEnum.B);
            specimen.Unreal.Should().Be(0);
        }
    }

    internal class CsvSpecimen
    {
        public string String { get; set; }

        public int Int { get; set; }

        public decimal Decimal { get; set; }

        public bool Bool { get; set; }

        public Guid Guid { get; set; }

        public DateTime DateTime { get; set; }
    }

    internal class NullableCsvSpecimen
    {
        public string String { get; set; }

        public int? Int { get; set; }

        public decimal? Decimal { get; set; }

        public bool? Bool { get; set; }

        public Guid? Guid { get; set; }

        public DateTime? DateTime { get; set; }
    }

    internal class OptionalCsvSpecimen : CsvSpecimen
    {
        [Optional]
        public int Optional { get; set; }

        [Ignore]
        public string Ignore { get; set; }

        [Optional]
        public CsvTestEnum EnumValue { get; set; } = CsvTestEnum.B;
    }

    internal class ArrayCsvSpecimen
    {
        public int[] Array1 { get; set; }

        public Guid[] Array2 { get; set; }

        public string[] Array3 { get; set; }

        public bool[] Array4 { get; set; }
    }

    internal class EnumCsvSpecimen
    {
        public CsvTestEnum One { get; set; }

        [TypeConverter(typeof(ArrayConverter<CsvTestEnum>))]
        public CsvTestEnum[] Array { get; set; }

        [TypeConverter(typeof(NullableArrayConverter<CsvTestEnum>))]
        public CsvTestEnum[] NullArray { get; set; }

        public CsvTestEnum? Null { get; set; }

        public CsvTestEnum? Nullable { get; set; }

        public CsvTestEnum Unreal { get; set; }
    }

    internal enum CsvTestEnum
    {
        A = 1,
        B,
        C
    }
}
