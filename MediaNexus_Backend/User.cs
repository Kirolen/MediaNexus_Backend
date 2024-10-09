using Mysqlx.Expr;
using System;

namespace MediaNexus_Backend
{
    public enum UserRole
    {
        Admin,      
        Moderator,  
        User,
        Guest
    }

    public class User
    {
        public int Id { get; set; }
        public string HashPassword { get; set; }
        public string UserImageURL { get; set; }
        public UserRole Role { get; set; } 
        public string Nickname { get; set; }
        public DateTime?   RegisterDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool IsBanned { get; set; }
        public DateTime? DateEndBan { get; set; } 
        public string Email { get; set; }
        public DateTime? BirthdayDate { get; set; } 
        public string UserDescription { get; set; }

        public User()
        {
            Id = 0;
            HashPassword = null;
            UserImageURL = null;
            Role = UserRole.Guest;
            Nickname = null;
            RegisterDate = null;
            LastLoginDate = null;
            IsBanned = false;
            DateEndBan = null;
            Email = null;
            BirthdayDate = null;
            UserDescription = null;
        }

        public User(User other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            Id = other.Id;
            HashPassword = other.HashPassword;
            UserImageURL = other.UserImageURL;
            Role = other.Role;
            Nickname = other.Nickname;
            RegisterDate = other.RegisterDate;
            LastLoginDate = other.LastLoginDate;
            IsBanned = other.IsBanned;
            DateEndBan = other.DateEndBan;
            Email = other.Email;
            BirthdayDate = other.BirthdayDate;
            UserDescription = other.UserDescription;
        }

        public User(int id, string hashPassword, string userImageURL, UserRole role, string nickname, DateTime registerDate,
            DateTime? lastLoginDate, bool isBanned, DateTime? dateEndBan, string email, DateTime? birthdayDate, string userDescription)
        {
            Id = id;
            HashPassword = hashPassword;
            UserImageURL = userImageURL;
            Role = role;
            Nickname = nickname;
            RegisterDate = registerDate;
            LastLoginDate = lastLoginDate;
            IsBanned = isBanned;
            DateEndBan = dateEndBan;
            Email = email;
            BirthdayDate = birthdayDate;
            UserDescription = userDescription;
        }
    }
}

