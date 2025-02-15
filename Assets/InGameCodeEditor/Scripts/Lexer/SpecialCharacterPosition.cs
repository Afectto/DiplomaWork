namespace InGameCodeEditor.Lexer
{
    /// <summary>
    /// Represents a keyword position where a special character may appear.
    /// </summary>
    public enum SpecialCharacterPosition
    {
        /// <summary>
        /// The special character may appear before a keyword.
        /// </summary>
        Start,
        /// <summary>
        /// The special character may appear after a keyword.
        /// </summary>
        End,
    };
}