using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace WX.B2C.User.Verification.BlobStorage.Storages
{
    public interface ICsvBlobStorage
    {
        Task<T[]> GetAsync<T>(string containerName, string fileName);
    }

    /// <summary>
    /// Returns entities from csv-file from blob storage
    /// </summary>
    /// <remarks>
    /// BE CAREFUL WHEN CHANGE SOMETHING OR EVEN UPDATE PACKAGE
    /// String like "null" or "NULL" will be parsed as null
    /// All whitespace around a field will be trimmed
    /// Array should be in raw format like: [a,b,c]
    /// Array can be parsed as null
    /// To parse array of enums specify converter: cref="ArrayConverter<T>" or cref="NullableArrayConverter<T>"
    /// </remarks>
    internal class CsvBlobStorage : ICsvBlobStorage
    {
        private readonly IBlobStorage _blobStorage;

        public CsvBlobStorage(IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage ?? throw new ArgumentNullException(nameof(blobStorage));
        }

        public async Task<T[]> GetAsync<T>(string containerName, string fileName)
        {
            using var stream = await _blobStorage.DownloadAsync(containerName, fileName);
            using var streamReader = new StreamReader(stream);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
                TrimOptions = TrimOptions.Trim,
                WhiteSpaceChars = new[] {' '},
                IgnoreBlankLines = true,
            };
            using var csvReader = new CsvReader(streamReader, config);
            csvReader.Context.TypeConverterCache = new NullableTypeConverterCache();
            try
            {
                return csvReader.GetRecords<T>().ToArray();
            }
            catch (Exception e)
            {
                throw new CsvParsingException(e.Message);
            }
        }
    }

    /// <summary>
    /// Default exception from CSV provider cannot be logger, therefore exception must be wrapped
    /// </summary>
    [Serializable]
    internal class CsvParsingException : Exception
    {
        public CsvParsingException(string message):base(message)
        {
        }

        protected CsvParsingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }

    // TODO: Move to infrastructure if csv parsing will be used somewhere else
    internal class NullableTypeConverterCache : TypeConverterCache
    {
        public NullableTypeConverterCache()
        {
            CreateDefaultConverters();
        }

        public void AddNullableConverter(Type type, ITypeConverter typeConverter) =>
            AddConverter(type, new NullableConverter(typeConverter));

        private void CreateDefaultConverters()
        {
            AddNullableConverter(typeof(BigInteger), new BigIntegerConverter());
            AddNullableConverter(typeof(bool), new BooleanConverter());
            AddNullableConverter(typeof(byte), new ByteConverter());
            AddNullableConverter(typeof(byte[]), new ByteArrayConverter());
            AddNullableConverter(typeof(char), new CharConverter());
            AddNullableConverter(typeof(DateTime), new DateTimeConverter());
            AddNullableConverter(typeof(DateTimeOffset), new DateTimeOffsetConverter());
            AddNullableConverter(typeof(decimal), new DecimalConverter());
            AddNullableConverter(typeof(double), new DoubleConverter());
            AddNullableConverter(typeof(float), new SingleConverter());
            AddNullableConverter(typeof(Guid), new GuidConverter());
            AddNullableConverter(typeof(short), new Int16Converter());
            AddNullableConverter(typeof(int), new Int32Converter());
            AddNullableConverter(typeof(long), new Int64Converter());
            AddNullableConverter(typeof(sbyte), new SByteConverter());
            AddNullableConverter(typeof(string), new StringConverter());
            AddNullableConverter(typeof(TimeSpan), new TimeSpanConverter());
            AddNullableConverter(typeof(ushort), new UInt16Converter());
            AddNullableConverter(typeof(uint), new UInt32Converter());
            AddNullableConverter(typeof(ulong), new UInt64Converter());
            AddNullableConverter(typeof(Enum), new EnumConverter());

            AddNullableConverter(typeof(BigInteger[]), new ArrayConverter<BigInteger>());
            AddNullableConverter(typeof(bool[]), new ArrayConverter<bool>());
            AddNullableConverter(typeof(char[]), new ArrayConverter<char>());
            AddNullableConverter(typeof(DateTime[]), new ArrayConverter<DateTime>());
            AddNullableConverter(typeof(DateTimeOffset[]), new ArrayConverter<DateTimeOffset>());
            AddNullableConverter(typeof(decimal[]), new ArrayConverter<decimal>());
            AddNullableConverter(typeof(double[]), new ArrayConverter<double>());
            AddNullableConverter(typeof(float[]), new ArrayConverter<float>());
            AddNullableConverter(typeof(Guid[]), new ArrayConverter<Guid>());
            AddNullableConverter(typeof(short[]), new ArrayConverter<short>());
            AddNullableConverter(typeof(int[]), new ArrayConverter<int>());
            AddNullableConverter(typeof(long[]), new ArrayConverter<long>());
            AddNullableConverter(typeof(sbyte[]), new ArrayConverter<sbyte>());
            AddNullableConverter(typeof(string[]), new ArrayConverter<string>());
            AddNullableConverter(typeof(TimeSpan[]), new ArrayConverter<TimeSpan>());
            AddNullableConverter(typeof(ushort[]), new ArrayConverter<ushort>());
            AddNullableConverter(typeof(uint[]), new ArrayConverter<uint>());
            AddNullableConverter(typeof(ulong[]), new ArrayConverter<ulong>());
        }
    }

    internal class NullableConverter : ITypeConverter
    {
        private readonly ITypeConverter _inner;

        public NullableConverter(ITypeConverter inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData) =>
            text.Equals("null", StringComparison.OrdinalIgnoreCase) ? null : _inner.ConvertFromString(text, row, memberMapData);

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData) =>
            value is null ? "null" : _inner.ConvertToString(value, row, memberMapData);
    }

    internal class EnumConverter : ITypeConverter
    {
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            var enumType = memberMapData.Member is Type type ? type : memberMapData.Type;
            if (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>))
                enumType = Nullable.GetUnderlyingType(enumType);

            return Enum.Parse(enumType, text, true);
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData) =>
            value.ToString();
    }

    public class ArrayConverter<T> : ITypeConverter
    {
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (text == "[]")
                return Array.Empty<T>();

            var elementType = typeof(T);
            var converter = row.Context.TypeConverterCache.GetConverter(elementType);
            var dummyMapData = new MemberMapData(elementType);

            return text.Trim('[', ']')
                       .Split(',')
                       .Select(item => (T)converter.ConvertFromString(item.Trim(), null, dummyMapData))
                       .ToArray();
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            var enumerable = (IEnumerable<T>)value;
            var str = $"[{string.Join(",", enumerable)}]";
            row.WriteField(str);

            return null;
        }
    }

    public class NullableArrayConverter<T> : ITypeConverter
    {
        private readonly ITypeConverter _converter;

        public NullableArrayConverter()
        {
            _converter = new NullableConverter(new ArrayConverter<T>());
        }

        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData) =>
            _converter.ConvertFromString(text, row, memberMapData);

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData) =>
            _converter.ConvertToString(value, row, memberMapData);
    }
}
