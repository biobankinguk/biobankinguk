﻿namespace Biobanks.IdentityModel.Types
{
    /// <summary>
    /// Encoded output formats for representing a byte array as a string
    /// </summary>
    public enum OutputFormat
    {
        /// <summary>
        /// URL-safe Base64
        /// </summary>
        Base64Url,
        /// <summary>
        /// Base64
        /// </summary>
        Base64,
        /// <summary>
        /// Hex
        /// </summary>
        Hex
    }
}
