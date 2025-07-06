using System;

namespace Total;

[AttributeUsage(AttributeTargets.Class)]
public class PluginLoadAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute : Attribute
{
    public Type DependencyType { get; }
    
    public DependsOnAttribute(Type dependencyType)
    {
        DependencyType = dependencyType;
    }
}