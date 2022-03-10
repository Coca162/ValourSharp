namespace ValourSharp.Attributes;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public class RemainderAttribute : Attribute
{
    public RemainderAttribute() { }
}