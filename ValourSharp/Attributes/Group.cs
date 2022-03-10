namespace ValourSharp.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class GroupAttribute : Attribute
{
    public string[] Names { get; set; }

    public GroupAttribute(params string[] names)
    {
        this.Names = names;
    }
}