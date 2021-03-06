using System.Reflection;
using System.Runtime.CompilerServices;
using Valour.Api.Items.Messages;
using static ValourSharp.CommandsService;
using ValourSharp.Attributes;

namespace ValourSharp;

public static class Registration
{
    public static void RegisterCommands(Assembly assembly)
    {
        var modules = assembly.ExportedTypes.Where(type =>
        {
            var info = type.GetTypeInfo();
            return info.IsModuleCandidateType() && !info.IsNested;
        });

        foreach (Type module in modules)
            RegisterCommands(module);
    }

    public static void RegisterCommands<T>() where T : BaseCommandModule => RegisterCommands(typeof(T));

    public static void RegisterCommands(Type commandModule)
    {
        IEnumerable<Func<PlanetMessage, Task<bool>>>
            checks = commandModule.GetCustomAttributes<CheckBaseAttribute>(true).Select(x =>
            {
                Func<PlanetMessage, Task<bool>> thing = x.ExecuteCheckAsync;
                return thing;
            });

        var attribute = commandModule.GetCustomAttribute<GroupAttribute>(false);
        if (attribute is not null)
        {
            var module = GroupToModule(commandModule, attribute);
            foreach (string name in attribute.Names)
                Commands.Add(name, module.CombineChecks(checks));
            return;
        }

        foreach ((string name, CommandModule module) in TypeToModules(commandModule))
            Commands.Add(name, module.CombineChecks(checks));
    }

    internal static Dictionary<string, CommandModule> TypeToModules(Type commandModule)
    {
        Dictionary<string, CommandModule> modules = new();
        var baseModule = BaseToModules(commandModule);
        baseModule.ForEach(module =>
        {
            foreach (string name in module.Names)
            {
                modules.Add(name, module);
            }
        });
        return modules;
    }

    internal static List<CommandModule> BaseToModules(Type commandModule)
    {
        var methods = commandModule.GetMethods();
        List<CommandInfo> commands = new(methods.Length);

        List<CommandModule> modules = new();

        //This would be LINQ if we didn't use the attribute from guard if
        foreach (var method in commandModule.GetMethods())
        {
            var attribute = method.GetCustomAttribute<CommandAttribute>(false);
            if (attribute is null) continue;

            CommandInfo command = new(attribute.Name, method);
            commands.Add(command);
        }

        var grouped = commands.GroupBy(x => x.Name);

        foreach(var group in grouped)
        {
            var alias = group.Select(x => x.Method.GetCustomAttribute<AliasesAttribute>(false)?.Names).MaxBy(x => x is null ? 0 : x.Length);
            var checks = group.Select(x => x.Method.GetCustomAttributes<CheckBaseAttribute>(true).ToArray()).MaxBy(x => x.Length)?.Select(x =>
            {
                Func<PlanetMessage, Task<bool>> thing = x.ExecuteCheckAsync;
                return thing;
            }).ToArray();
            checks = checks is null ? Array.Empty<Func<PlanetMessage, Task<bool>>>() : checks;

            if (alias is null) alias = new string[1] { group.Key };
            else
            {
                int array1OriginalLength = alias.Length;
                Array.Resize(ref alias, array1OriginalLength + 1);
                alias[array1OriginalLength] = group.Key;
            }

            modules.Add(new(alias, group.ToArray(), checks));
        }

        return modules;
    }

    internal static CommandModule GroupToModule(Type commandModule, GroupAttribute group)
    {
        var methods = commandModule.GetMethods();

        List<CommandInfo> commands = commandModule.GetMethods()
                                                  .Where(method => method.GetCustomAttribute<GroupCommandAttribute>(false) is not null)
                                                  .Select(method => new CommandInfo("", method))
                                                  .ToList();

        IEnumerable<Type> nested = commandModule.GetNestedTypes()
                                                .Where(type => type.GetTypeInfo().IsModuleCandidateType());

        Dictionary<string, CommandModule> modules = new();

        foreach (Type type in nested)
        {
            if (type.GetCustomAttribute(typeof(GroupAttribute)) is not GroupAttribute attribute) continue;

            var module = GroupToModule(type, attribute);
            foreach(string name in module.Names is null? Array.Empty<string>() : module.Names)
            {
                modules.Add(name, module);
            }
        }

        foreach ((string name, CommandModule typesCommands) in TypeToModules(commandModule))
            modules.Add(name, typesCommands);

        var checks = commands.Select(x => x.Method.GetCustomAttributes<CheckBaseAttribute>(true).ToArray()).MaxBy(x => x.Length)?.Select(x =>
        {
            Func<PlanetMessage, Task<bool>> thing = x.ExecuteCheckAsync;
            return thing;
        }).ToArray();
        checks = checks is null ? Array.Empty<Func<PlanetMessage, Task<bool>>>() : checks;

        return commands.Count != commands.DistinctBy(x => x.Priority).Count()
            ? throw new Exception("Same priority or no priority for overloads of command!")
            : new CommandModule(group.Names, commands.OrderByDescending(x => x.Priority).ToArray(), checks, modules);
    }

    private static bool IsModuleCandidateType(this TypeInfo type)
    {
        // check if compiler-generated
        if (type.GetCustomAttribute<CompilerGeneratedAttribute>(false) is not null)
            return false;

        // check if derives from the required base class
        TypeInfo module = typeof(BaseCommandModule).GetTypeInfo();
        if (!module.IsAssignableFrom(type))
            return false;

        // check if anonymous
        if (type.IsGenericType && type.Name.Contains("AnonymousType") && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$")) && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic)
            return false;

        // check if abstract, static, or not a class
        if (!type.IsClass || type.IsAbstract)
            return false;

        // check if delegate type
        var @delegate = typeof(Delegate).GetTypeInfo();
        if (@delegate.IsAssignableFrom(type))
            return false;

        // qualifies if any method or type qualifies
        return type.DeclaredMethods.Any(xmi => xmi.IsCommandCandidate()) || type.DeclaredNestedTypes.Any(xti => xti.IsModuleCandidateType());
    }

    private static bool IsCommandCandidate(this MethodInfo method)
    {
        // check if exists
        if (method == null)
            return false;

        // check if static, non-public, abstract, a constructor, or a special name
        if (method.IsStatic || method.IsAbstract || method.IsConstructor || method.IsSpecialName)
            return false;

        // check if appropriate return and arguments
        var parameters = method.GetParameters();
        if (!parameters.Any() || parameters.First().ParameterType != typeof(PlanetMessage) || method.ReturnType != typeof(Task))
            return false;

        // qualifies
        return true;
    }
}
