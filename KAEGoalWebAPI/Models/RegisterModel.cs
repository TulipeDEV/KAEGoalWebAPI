﻿namespace KAEGoalWebAPI.Models
{
    public class RegisterModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Displayname { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
