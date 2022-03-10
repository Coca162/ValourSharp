using System.Reflection;
using ValourSharp.Attributes;

namespace ValourSharp;

public static class CommandsService
{
    public static Dictionary<string, CommandModule> Commands { get; } = new(StringComparer.InvariantCultureIgnoreCase);
}

// Use this when end-to-end testing is being done
//public static class CommandsService
//{
//    static CommandsService()
//    {
//        CommandsProxy = new CommandsProxy();
//    }

//    internal static ICommandsProxy CommandsProxy { private get; set; }

//    public static Dictionary<string, CommandModule> Commands => CommandsProxy.Commands;
//}

//public class CommandsProxy : ICommandsProxy
//{
//    public Dictionary<string, CommandModule> Commands => new(StringComparer.InvariantCultureIgnoreCase);
//}

//public interface ICommandsProxy
//{
//    Dictionary<string, CommandModule> Commands { get; }
//}