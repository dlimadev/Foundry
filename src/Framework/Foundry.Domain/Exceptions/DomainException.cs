namespace Foundry.Domain.Exceptions
{
    /// <summary>
    /// A custom exception type for business rule violations that originate from the Domain layer.
    /// It uses an Error Code instead of a hardcoded message to support internationalization (I18N).
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// A unique, machine-readable code for the specific business error. e.g., "orders.cannotBeFilled"
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Optional parameters to be used when formatting the final, user-facing error message.
        /// </summary>
        public object[] Parameters { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException"/> class with an error code and parameters.
        /// </summary>
        /// <param name="errorCode">The machine-readable error code.</param>
        /// <param name="parameters">The parameters to be used in the translated message.</param>
        public DomainException(string errorCode, params object[] parameters)
            : base($"Domain Error: '{errorCode}'. See ErrorCode and Parameters properties for details.")
        {
            ErrorCode = errorCode;
            Parameters = parameters;
        }
    }
}