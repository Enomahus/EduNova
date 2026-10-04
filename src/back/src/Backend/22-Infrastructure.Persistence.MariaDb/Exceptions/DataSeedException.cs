using Tools.Exceptions;
using Tools.Exceptions.Errors;

namespace Infrastructure.Persistence.MariaDb.Exceptions;

public class DataSeedException : AppException
{
    public DataSeedException(string message)
        : base(ErrorCode.DataSeeding, ErrorKind.Technical, message) { }
}
