using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLib;

namespace CommandRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Использование: CommandRunner");
                return;
            }

            string dllPath = args[0];
            
            if (!File.Exists(dllPath))
            {
                Console.WriteLine($"Файл не найден: {dllPath}");
                return;
            }

            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);
                var commandTypes = assembly.GetTypes()
                    .Where(t => t.GetInterfaces().Contains(typeof(ICommand)))
                    .Where(t => !t.IsAbstract)
                    .ToList();

                if (commandTypes.Count == 0)
                {
                    Console.WriteLine($"Не найдено реализации ICommand implementations в {Path.GetFileName(dllPath)}");
                    return;
                }

                Console.WriteLine("Доступные команды:");
                for (int i = 0; i < commandTypes.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {commandTypes[i].Name}");
                }

                Console.Write("Выберите команду: ");
                if (int.TryParse(Console.ReadLine(), out int commandIndex) && 
                    commandIndex >= 1 && commandIndex <= commandTypes.Count)
                {
                    Type commandType = commandTypes[commandIndex - 1];
                    ExecuteCommand(commandType, args.Skip(1).ToArray());
                }
                else
                {
                    Console.WriteLine("Неверный выбор команды");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void ExecuteCommand(Type commandType, string[] args)
        {
            try
            {
                object commandInstance = CreateCommandInstance(commandType, args);
                
                if (commandInstance is ICommand command)
                {
                    Console.WriteLine($"Проведение {commandType.Name}...");
                    command.Execute();
                    Console.WriteLine("Команда выполнена успешно!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Выполнение неудачно: {ex.Message}");
            }
        }

        static object CreateCommandInstance(Type type, string[] args)
        {
            ConstructorInfo[] constructors = type.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .ToArray();

            foreach (var constructor in constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                if (parameters.Length <= args.Length)
                {
                    try
                    {
                        object[] convertedArgs = new object[parameters.Length];
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            convertedArgs[i] = Convert.ChangeType(args[i], parameters[i].ParameterType);
                        }
                        return constructor.Invoke(convertedArgs);
                    }
                    catch
                    {
                        // Пропуск конструктора при ошибке преобразования
                    }
                }
            }
            throw new InvalidOperationException($"Не найден подходящий конструктор для {type.Name}");
        }
    }
}
