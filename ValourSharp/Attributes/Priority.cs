namespace ValourSharp.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class PriorityAttribute : Attribute
{
    public int Ranking { get; set; }

    public PriorityAttribute(int ranking)
    {
        this.Ranking = ranking;
    }
}