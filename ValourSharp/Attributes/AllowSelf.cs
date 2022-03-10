namespace ValourSharp.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class AllowSelfAttribute : Attribute
{
    public AllowSelfAttribute() 
    {
    }
}