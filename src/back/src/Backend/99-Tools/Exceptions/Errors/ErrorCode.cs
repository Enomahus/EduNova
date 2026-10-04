using System;
using System.Collections.Generic;
using System.Text;

namespace Tools.Exceptions.Errors
{
    public enum ErrorCode
    {
        None = 0,

        // Common
        Validation,
        InvalidParameter,
        InvalidStatus,
        ResourceAlreadyExists,
        NotFound,
        MissingData,
        GenericServerError,

        // Auth
        AccessRights,
        AuthenticationFailed,

        // Tech
        ConfigurationMissing,
        IncorrectEntityTypeLinked,
        DataSeeding,
        Storage,
        Export,
    }
}
