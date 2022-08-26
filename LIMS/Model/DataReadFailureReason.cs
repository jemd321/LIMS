namespace LIMS.Enums
{
    /// <summary>
    /// Describes the reasons that a data read operation failed.
    /// </summary>
    public enum DataReadFailureReason
    {
        /// <summary>
        /// The operation was a success.
        /// </summary>
        None,

        /// <summary>
        /// The input file path was too long.
        /// </summary>
        PathTooLong,

        /// <summary>
        /// The specified file path is invalid (for example, it is on an unmapped drive).
        /// </summary>
        InvalidDirectory,

        /// <summary>
        /// The file path specified a directory, or the caller does not have the required permission.
        /// </summary>
        UnauthorizedAccess,

        /// <summary>
        /// The file specified in the file path was not found.
        /// </summary>
        FileNotFound,

        /// <summary>
        /// The file path is in an invalid format.
        /// </summary>
        NotSupported,

        /// <summary>
        /// The file path is a zero-length string, contains only white space, or contains one or more invalid characters.
        /// </summary>
        GenericArgumentProblem,

        /// <summary>
        /// An I/O error occurred while opening the file.
        /// </summary>
        GenericIOProblem,
    }
}