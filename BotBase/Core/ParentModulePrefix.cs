using Microsoft.Extensions.DependencyInjection;

namespace BotBase;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ParentModulePrefixAttribute(Type parentModule) : Attribute
{
    public Type ParentModule { get; } = parentModule;
}