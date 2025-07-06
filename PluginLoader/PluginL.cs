using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Total; 

public class PluginL
{
    public void LoadAndExecutePlugins(string pluginsDirectory)
    {
        var pluginFiles = Directory.GetFiles(pluginsDirectory, "*.dll");
        var assemblies = new List<Assembly>();

        foreach (var file in pluginFiles)
        {
            try
            {
                var assembly = Assembly.LoadFrom(file);
                assemblies.Add(assembly);
                Console.WriteLine($"Загружена сборка: {Path.GetFileName(file)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки {file}: {ex.Message}");
            }
        }

        var pluginTypes = new List<Type>();
        foreach (var assembly in assemblies)
        {
            try
            {
                Type[]? types = assembly.GetTypes();
                if (types == null) continue;
                
                foreach (var type in types)
                {
                    if (type.GetCustomAttribute<PluginLoadAttribute>() != null)
                    {
                        pluginTypes.Add(type);
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine($"Ошибка загрузки типов из {assembly.FullName}: {ex.Message}");
                if (ex.LoaderExceptions != null)
                {
                    foreach (var loaderEx in ex.LoaderExceptions)
                    {
                        Console.WriteLine($" - {loaderEx?.Message}");
                    }
                }
            }
        }

        var validPlugins = new List<Type>();
        foreach (var type in pluginTypes)
        {
            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
            {
                Console.WriteLine($"Пропуск {type.FullName}: отсутствует конструктор без параметров");
                continue;
            }

            var executeMethod = type.GetMethod("Execute");
            if (executeMethod == null || executeMethod.ReturnType != typeof(void))
            {
                Console.WriteLine($"Пропуск {type.FullName}: отсутствует метод Execute()");
                continue;
            }

            validPlugins.Add(type);
            Console.WriteLine($"Найден плагин: {type.FullName}");
        }

        var dependencyGraph = BuildDependencyGraph(validPlugins);
        var executionOrder = TopologicalSort(dependencyGraph);
        var pluginInstances = new Dictionary<Type, object>();

        foreach (var pluginType in executionOrder)
        {
            try
            {
                var instance = Activator.CreateInstance(pluginType);
                if (instance == null)
                {
                    Console.WriteLine($"Ошибка: не удалось создать экземпляр {pluginType.FullName}");
                    continue;
                }
                
                pluginInstances.Add(pluginType, instance);
                
                Console.WriteLine($"Выполнение плагина: {pluginType.FullName}");
                var executeMethod = pluginType.GetMethod("Execute");
                
                executeMethod?.Invoke(instance, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка выполнения {pluginType.FullName}: {ex.Message}");
            }
        }
    }

    private Dictionary<Type, List<Type>> BuildDependencyGraph(List<Type> pluginTypes)
    {
        var graph = new Dictionary<Type, List<Type>>();
        
        foreach (var type in pluginTypes)
        {
            graph[type] = new List<Type>();
        }

        foreach (var type in pluginTypes)
        {
            var dependencies = type.GetCustomAttributes<DependsOnAttribute>();
            foreach (var dep in dependencies)
            {
                if (pluginTypes.Contains(dep.DependencyType))
                {
                    graph[type].Add(dep.DependencyType);
                }
                else
                {
                    Console.WriteLine($"Предупреждение: {type.FullName} зависит от отсутствующего плагина {dep.DependencyType.FullName}");
                }
            }
        }

        return graph;
    }

    private List<Type> TopologicalSort(Dictionary<Type, List<Type>> graph)
    {
        var sorted = new List<Type>();
        var visited = new Dictionary<Type, bool>();
        var tempMark = new Dictionary<Type, bool>();

        foreach (var node in graph.Keys)
        {
            visited[node] = false;
            tempMark[node] = false;
        }

        foreach (var node in graph.Keys)
        {
            if (!visited[node])
            {
                Visit(node, graph, visited, tempMark, sorted);
            }
        }

        return sorted;
    }

    private void Visit(
        Type node, 
        Dictionary<Type, List<Type>> graph, 
        Dictionary<Type, bool> visited, 
        Dictionary<Type, bool> tempMark, 
        List<Type> sorted)
    {
        if (tempMark[node])
        {
            throw new InvalidOperationException($"Обнаружена циклическая зависимость: {node.FullName}");
        }

        if (!visited[node])
        {
            tempMark[node] = true;
            
            foreach (var dependency in graph[node])
            {
                Visit(dependency, graph, visited, tempMark, sorted);
            }

            visited[node] = true;
            tempMark[node] = false;
            sorted.Add(node);
        }
    }
}