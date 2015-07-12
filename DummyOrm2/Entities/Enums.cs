namespace DummyOrm2.Entities
{
    public enum UserStatus
    {
        Active,
        Passive,
        AwaitingActivation,
        Banned
    }

    public enum LoginType
    {
        Email,
        Facebook,
        Cookie
    }

    public enum LoginResult
    {
        Successful,
        InvalidUsername,
        InvalidEmail,
        InvalidPassword,
        InvalidFacebookToken,
        InvalidCookieToken,
        ExpiredCookieToken,
        SystemError
    }

    public enum UserType
    {
        User,
        Admin,
        Moderator,
        Anonymous
    }
    
    public enum AccessLevel
    {
        Private,
        Public
    }

    public enum ConfirmationCodeStatus
    {
        AwaitingConfirmation,
        Confirmed,
        Failed,
        Expired
    }

    public enum ConfirmationReason
    {
        NewUser,
        PasswordRecovery,
        AccountReactivation
    }

    public enum NotificationType
    {
        Like,
        Retag,
        NewFollower,
        Comment,
        PrivateMessage
    }

    public enum NotificationStatus
    {
        Unread,
        Read
    }

    public enum PrivateMessageStatus
    {
        Unread,
        Read
    }

    public enum TokenType
    {
        Auth,
        Request
    }
}