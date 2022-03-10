namespace ValourSharp.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class CommandAttribute : Attribute
{
    public string Name { get; set; }

    public CommandAttribute(string name)
    {
        this.Name = name;
    }
}