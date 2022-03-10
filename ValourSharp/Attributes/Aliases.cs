namespace ValourSharp.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class AliasesAttribute : Attribute
{
    public string[] Names { get; set; }

    public AliasesAttribute(params string[] aliases)
    {
        this.Names = aliases;
    }
}