using EmbyClient.Dotnet.Model;

namespace MB.Emby.Model;

public class User
{
    public string Id { get; set; }
    public string? Name { get; set; } = null;
    public bool? HasPassword { get; set; } = null;
    public DateTime? LastLoginDate { get; set; } = null;
    public DateTime? LastActivityDate { get; set; } = null;

    public User() => this.Id = "uninitialized";
    public User(string id) => this.Id = id;
    public User(UserDto user)
    {
        this.Id = user.Id;
        this.Name = user.Name;
        this.HasPassword = user.HasPassword;

        if (user.LastLoginDate != null) this.LastLoginDate = ((DateTimeOffset)user.LastLoginDate).LocalDateTime;
        if (user.LastActivityDate != null) this.LastActivityDate = ((DateTimeOffset)user.LastActivityDate).LocalDateTime;
    }

    public override string ToString() => this.Id;
}
