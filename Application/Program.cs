using System;
using System.Reflection;

namespace Application
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Укажите путь к DLL файлу");
                Console.WriteLine("Пример: dotnet run -- ../MyLibrary/bin/Debug/net8.0/MyLibrary.dll");
                return;
            }

            try
            {
                PrintAssemblyMetadata(args[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void PrintAssemblyMetadata(string assemblyPath)
        {
            var assembly = Assembly.LoadFrom(assemblyPath);
            Console.WriteLine($"Сборка: {assembly.FullName}\n");

            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass) continue;
                
                Console.WriteLine($"Класс: {type.FullName}");
                Console.WriteLine(new string('-', 40));
                
                Console.WriteLine("Конструкторы:");
                foreach (var ctor in type.GetConstructors())
                {
                    Console.Write($"  {type.Name}(");
                    PrintParameters(ctor.GetParameters());
                    Console.WriteLine(")");
                }
                
                Console.WriteLine("\nМетоды:");
                foreach (var method in type.GetMethods())
                {
                    if (method.IsSpecialName) continue; 
                    Console.Write($"  {method.ReturnType.Name} {method.Name}(");
                    PrintParameters(method.GetParameters());
                    Console.WriteLine(")");
                }
                
                Console.WriteLine("\nПоля:");
                foreach (var field in type.GetFields())
                {
                    Console.WriteLine($"  {field.FieldType.Name} {field.Name}");
                }
                
                Console.WriteLine("\nСвойства:");
                foreach (var prop in type.GetProperties())
                {
                    Console.WriteLine($"  {prop.PropertyType.Name} {prop.Name}");
                }
                
                Console.WriteLine(new string('=', 60) + "\n");
            }
        }

        static void PrintParameters(ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                Console.Write($"{parameters[i].ParameterType.Name} {parameters[i].Name}");
                if (i < parameters.Length - 1) Console.Write(", ");
            }
        }
    }
}