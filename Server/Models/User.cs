using System;
using System.Collections.Generic;

namespace BlazorChat.Server.Models
{
    public partial class User
    {
        public long UserId { get; set; }
        public string EmailAddress { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public byte[]? DateOfBirth { get; set; }
        public string? AboutMe { get; set; }
        public long? Notifications { get; set; }
        public long? DarkTheme { get; set; }
        public byte[]? CreatedDate { get; set; }
    }
}
