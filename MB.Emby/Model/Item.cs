using EmbyClient.Dotnet.Model;

namespace MB.Emby.Model;

public class Item
{
    public string Id { get; set; }
    public string? Name { get; set; } = null;
    public string? Type { get; set; } = null;
    public bool? IsFolder { get; set; } = null;

    public Item() => this.Id = "uninitialized";
    public Item(string id) => this.Id = id;
    public Item(string id, string name, string type)
    {
        this.Id = id;
        this.Name = name;
        this.Type = type;
    }
    public Item(BaseItemDto item)
    {
        this.Id = item.Id;
        this.Name = item.Name;
        this.Type = item.Type;
        this.IsFolder = item.IsFolder;
    }

    public override string ToString() => this.Id.ToString();
}
