namespace ValourSharp.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class AllowBotsAttribute : Attribute
{
    public AllowBotsAttribute() 
    { 
    }
}