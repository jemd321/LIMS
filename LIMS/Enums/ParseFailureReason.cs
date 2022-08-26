namespace LIMS.Enums
{
    /// <summary>
    /// Describes the erros that can occur during parsing of text files from external instrument control/processing software.
    /// </summary>
    public enum ParseFailureReason
    {
        /// <summary>
        /// The parsing succeeded.
        /// </summary>
        None,

        /// <summary>
        /// The imported text file was not in the expected format.
        /// </summary>
        InvalidFileFormat,

        /// <summary>
        /// An exception occured during the parsing/casting of the data.
        /// </summary>
        InvalidCast,

        /// <summary>
        /// Any other error that occured while trying to parse the data.
        /// </summary>
        OtherSystemException,
    }
}
