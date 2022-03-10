using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valour.Api.Items.Messages;
using ValourSharp.Attributes;

namespace ValourSharp;

public class CommandModule
{
    public CommandModule(string[] names, CommandInfo[] commands, Func<PlanetMessage, Task<bool>>[] checks)
    {
        Names = names;
        ModuleCommands = commands;
        _checks = checks;
    }

    public CommandModule(string[] names, CommandInfo[] groupCommands, Func<PlanetMessage, Task<bool>>[] checks, Dictionary<string, CommandModule> subGroups) : this(names, groupCommands, checks)
        => SubGroups = subGroups;

    public string[] Names { get; }

    private Func<PlanetMessage, Task<bool>>[] _checks;

    public Func<PlanetMessage, Task<bool>>[] Checks { get => _checks; }

    public CommandModule CombineChecks(IEnumerable<Func<PlanetMessage, Task<bool>>> newChecks)
    {
        _checks = _checks.Concat(newChecks).ToArray();

        if (SubGroups is not null)
            foreach (var module in SubGroups.DistinctBy(x => x.Value))
                module.Value.CombineChecks(newChecks);

        return this;
    }

    public CommandInfo[] ModuleCommands { get; }

    public Dictionary<string, CommandModule>? SubGroups { get; }
}
