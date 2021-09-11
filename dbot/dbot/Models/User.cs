﻿using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace dbot.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Username {get;set;}
        public string Discriminator {get;set;}

        public User() { }

        public User(IUser user)
        {
            Username = user.Username;
            Discriminator = user.Discriminator;
        }

        public override bool Equals(object obj)
        {
            return obj is User user &&
                   Username == user.Username &&
                   Discriminator == user.Discriminator;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Username, Discriminator);
        }
    }
}
