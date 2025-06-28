using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace task05;

public class ClassAnalyzer
{
    private readonly Type _type;

    public ClassAnalyzer(Type type)
    {
        _type = type;
    }

    public IEnumerable<string> GetPublicMethods()
    {
        return _type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                    .Select(m => m.Name)
                    .Distinct();
    }

    public IEnumerable<string> GetMethodParams(string methodName)
    {
        var methods = _type.GetMethods()
                           .Where(m => m.Name == methodName)
                           .ToList();
        
        if (methods.Count == 0) 
            return Enumerable.Empty<string>();
        
        var method = methods.First();
        
        var parameters = method.GetParameters()
                               .Select(p => $"{p.ParameterType.Name} {p.Name}")
                               .ToList();
        parameters.Add($"Returns: {method.ReturnType.Name}");
        
        return parameters;
    }

    public IEnumerable<string> GetAllFields()
    {
        return _type.GetFields(BindingFlags.Instance | BindingFlags.Static | 
                              BindingFlags.Public | BindingFlags.NonPublic)
                    .Select(f => f.Name);
    }

    public IEnumerable<string> GetProperties()
    {
        return _type.GetProperties(BindingFlags.Instance | BindingFlags.Static | 
                                  BindingFlags.Public | BindingFlags.NonPublic)
                    .Select(p => p.Name);
    }

    public bool HasAttribute<T>() where T : Attribute
    {
        return _type.GetCustomAttributes(typeof(T), true).Any();
    }
}
