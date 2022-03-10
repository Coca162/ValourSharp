using System.Reflection;
using ValourSharp.Attributes;

namespace ValourSharp;

public class CommandInfo
{
    public CommandInfo(string name, MethodInfo method)
    {
        Name = name;
        Method = method;

        var priorityAttribute = Method.GetCustomAttribute(typeof(PriorityAttribute), false) as PriorityAttribute;
        Priority = priorityAttribute is not null ? priorityAttribute.Ranking : 0;

        Parameters = method.GetParameters();
        AllowBots = Method.GetCustomAttribute(typeof(AllowBotsAttribute), false) is not null;
        AllowSelf = Method.GetCustomAttribute(typeof(AllowSelfAttribute), false) is not null;
    }

    public string Name { get; }

    public int Priority { get; }

    public MethodInfo Method { get; }

    public ParameterInfo[] Parameters { get; }

    public bool AllowBots { get; }

    public bool AllowSelf { get; }
}
