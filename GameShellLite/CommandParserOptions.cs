using System;
using System.Collections.Generic;
using System.Text;

namespace GameShellLite
{
    /// <summary>
    /// Specifies the numeric precision to use when parsing number arguments.
    /// </summary>
    public enum NumberPrecision
    {
        /// <summary>
        /// Parse numbers as single-precision floating-point values (float).
        /// </summary>
        Float,
        /// <summary>
        /// Parse numbers as double-precision floating-point values (double).
        /// </summary>
        Double,
        /// <summary>
        /// Parse numbers as decimal values with high precision (decimal).
        /// </summary>
        Decimal
    }

    /// <summary>
    /// Provides configuration options for the command parser.
    /// </summary>
    public class CommandParserOptions
    {
        /// <summary>
        /// Gets or sets the numeric precision to use when parsing number arguments.
        /// </summary>
        public NumberPrecision NumberPrecision { get; set; }

        /// <summary>
        /// Gets or sets whether null values are allowed in command arguments. Default is true.
        /// </summary>
        public bool AllowNull { get; set; } = true;
    }
}
