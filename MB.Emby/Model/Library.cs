using EmbyClient.Dotnet.Model;

namespace MB.Emby.Model;

public class Library : Item
{
    public Library(BaseItemDto item) : base(item) { }

    public override string ToString() => this.Id;
}
