using Valour.Api.Items.Messages;

namespace ValourSharp.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public abstract class CheckBaseAttribute : Attribute
{
    public abstract Task<bool> ExecuteCheckAsync(PlanetMessage ctx);
}