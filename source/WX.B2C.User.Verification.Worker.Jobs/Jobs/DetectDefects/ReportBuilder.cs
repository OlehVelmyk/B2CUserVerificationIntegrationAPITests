using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Validators;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects
{
    
    internal interface IReportBuilder
    {
        Task<Stream> BuildPartAsync(IEnumerable<Report> reports, string[] ignoredDefects);
    }
    
    internal class ReportBuilder : IReportBuilder
    {
        private const string CsvSeparator = "\t";
        private const string DetailsSeparator = ",";
        private const string NoDefect = "-";
        private const string NoDetails = "+";
        
        private bool _isHeaderWritten = false;

        public async Task<Stream> BuildPartAsync(IEnumerable<Report> reports, string[] ignoredDefects)
        {
            var ms = new MemoryStream();
            await using var sw = new StreamWriter(ms, Encoding.UTF8, leaveOpen: true);
            if (!_isHeaderWritten)
                await WriteHeadersAsync(sw, ignoredDefects);

            await reports.ForeachConsistently(report => AppendUserDefectsAsync(sw, report, ignoredDefects));
            
            await sw.FlushAsync();
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }

        private async Task WriteHeadersAsync(StreamWriter sw, string[] ignoredDefects)
        {
            var sb = new StringBuilder();
            sb.Append("UserId");
            sb.Append(CsvSeparator);
            
            sb.AppendJoin(CsvSeparator, DefectsToSave(ignoredDefects));
            await sw.WriteLineAsync(sb);
            _isHeaderWritten = true;
        }

        private async Task AppendUserDefectsAsync(StreamWriter sw, Report report, string[] ignoredDefects)
        {
            var sb = new StringBuilder();
            sb.Append(report.UserId);
            sb.Append(CsvSeparator);

            var defects = report.GetDefects();
            var hasDefects = false;
            foreach (var errorCode in DefectsToSave(ignoredDefects))
            {
                if (defects.TryGetValue(errorCode, out var details))
                {
                    if (details.IsNullOrEmpty())
                        sb.Append(NoDetails);
                    else
                        sb.AppendJoin(DetailsSeparator, details);

                    sb.Append(CsvSeparator);
                    hasDefects = true;
                }
                else
                {
                    sb.Append(NoDefect);
                    sb.Append(CsvSeparator);
                }
            }
            
            if (hasDefects)
                await sw.WriteLineAsync(sb);
        }

        private static IEnumerable<string> DefectsToSave(string[] ignoredDefects) =>
            ErrorCodes.AllCodes.Except(ignoredDefects);
    }
}