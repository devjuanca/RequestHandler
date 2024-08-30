namespace EasyRequestHandler.Request
{
    /// <summary>
    /// Represents an empty response or a no-result scenario, commonly used as a return type for handlers that do not produce a value. 
    /// The <see cref="Empty"/> struct provides a lightweight, immutable alternative to <c>void</c> for indicating the absence of a result.
    /// </summary>
    public readonly struct Empty
    {
        private static readonly Empty _value = new Empty();


        /// <summary>
        /// Gets a singleton instance of the <see cref="Empty"/> struct.
        /// </summary>
        public static ref readonly Empty Value => ref _value;


        /// <summary>
        /// Returns a string that represents the current instance, typically used for debugging purposes.
        /// </summary>
        /// <returns>A string representation of the <see cref="Empty"/> struct.</returns>
        public override string ToString()
        {
            return "{}";
        }
    }
}
