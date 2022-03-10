namespace ValourSharp.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class GroupCommandAttribute : Attribute
{
    public GroupCommandAttribute() {}
}