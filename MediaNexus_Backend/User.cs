using System;

namespace MediaNexus_Backend
{
    /// <summary>
    /// Represents the different roles a user can have in the system.
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// User with administrative privileges.
        /// </summary>
        Admin,
        /// <summary>
        /// User with moderation privileges.
        /// </summary>
        Moderator,
        /// <summary>
        /// Regular user with standard access.
        /// </summary>
        User,
        /// <summary>
        /// Guest user with limited access.
        /// </summary>
        Guest
    }

    /// <summary>
    /// Represents a user in the system with various properties related to the user's profile and status.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the hashed password of the user.
        /// </summary>
        public string HashPassword { get; set; }

        /// <summary>
        /// Gets or sets the URL of the user's profile image.
        /// </summary>
        public string UserImageURL { get; set; }

        /// <summary>
        /// Gets or sets the role of the user in the system.
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// Gets or sets the user's nickname.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets the date the user registered.
        /// </summary>
        public DateTime? RegisterDate { get; set; }

        /// <summary>
        /// Gets or sets the date the user last logged in.
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is banned.
        /// </summary>
        public bool IsBanned { get; set; }

        /// <summary>
        /// Gets or sets the date when the user's ban ends.
        /// </summary>
        public DateTime? DateEndBan { get; set; }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user's birthday date.
        /// </summary>
        public DateTime? BirthdayDate { get; set; }

        /// <summary>
        /// Gets or sets a description of the user.
        /// </summary>
        public string UserDescription { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class with default values.
        /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class by copying another user.
        /// </summary>
        /// <param name="other">The user to copy from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class with specified properties.
        /// </summary>
        /// <param name="id">The unique identifier for the user.</param>
        /// <param name="hashPassword">The hashed password of the user.</param>
        /// <param name="userImageURL">The URL of the user's profile image.</param>
        /// <param name="role">The role of the user in the system.</param>
        /// <param name="nickname">The user's nickname.</param>
        /// <param name="registerDate">The date the user registered.</param>
        /// <param name="lastLoginDate">The date the user last logged in.</param>
        /// <param name="isBanned">Indicates whether the user is banned.</param>
        /// <param name="dateEndBan">The date when the user's ban ends.</param>
        /// <param name="email">The user's email address.</param>
        /// <param name="birthdayDate">The user's birthday date.</param>
        /// <param name="userDescription">A description of the user.</param>
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
