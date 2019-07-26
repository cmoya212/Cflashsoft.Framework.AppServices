using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Cflashsoft.Framework.Types
{
    /// <summary>
    /// Represents a typical authentication reponse model.
    /// </summary>
    public class AppAuthResponseModel
    {
        /// <summary>
        /// Access token.
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Token type.
        /// </summary>
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// Expiration date.
        /// </summary>
        [JsonProperty("expiration")]
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// Refresh token.
        /// </summary>

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Scope.
        /// </summary>
        [JsonProperty("scope")]
        public string Scope { get; set; }
    }
}