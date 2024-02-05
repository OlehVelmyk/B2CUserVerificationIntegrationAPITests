using System.Linq;
using Microsoft.Data.SqlClient;

namespace WX.B2C.User.Verification.Worker.Jobs.Extensions
{
    internal static class ExceptionExtensions
    {
        public static bool IsUniqueKeyViolation(this SqlException ex)
        {
            return ex.Errors.Cast<SqlError>().Any(error => error.Class == 14 && error.Number is 2601 or 2627);
        }
    }
}