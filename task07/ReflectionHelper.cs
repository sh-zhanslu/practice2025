using System;
using System.Reflection;

namespace task07
{
    public static class ReflectionHelper
    {
        public static void PrintTypeInfo(Type type)
        {
            // Вывод информации о классе
            var classDisplayName = GetDisplayName(type);
            var classVersion = GetClassVersion(type);

            Console.WriteLine($"Тип: {type.Name}");
            Console.WriteLine($"Отображаемое имя: {classDisplayName}");
            Console.WriteLine($"Версия класса: {classVersion}");
            Console.WriteLine();

            // Вывод информации о методах
            Console.WriteLine("Методы:");
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (method.IsSpecialName) continue;
                
                var methodName = GetDisplayName(method);
                Console.WriteLine($"- {method.Name} => {methodName}");
            }
            Console.WriteLine();

            // Вывод информации о свойствах
            Console.WriteLine("Свойства:");
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                var propName = GetDisplayName(property);
                Console.WriteLine($"- {property.Name} => {propName}");
            }
        }

        static string GetDisplayName(MemberInfo member)
        {
            var attribute = member.GetCustomAttribute<DisplayNameAttribute>();
            return attribute != null ? attribute.DisplayName : "Нет атрибута";
        }

        static string GetClassVersion(Type type)
        {
            var attribute = type.GetCustomAttribute<VersionAttribute>();
            return attribute != null ? $"{attribute.Major}.{attribute.Minor}" : "Нет версии";
        }
    }
}