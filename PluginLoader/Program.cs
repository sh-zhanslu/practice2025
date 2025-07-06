using System;

namespace PluginLoader;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Укажите путь к каталогу плагинов");
            return;
        }

        var pluginsDir = args[0];
        
        if (!Directory.Exists(pluginsDir))
        {
            Console.WriteLine($"Каталог не найден: {pluginsDir}");
            return;
        }

        var loader = new PluginL();
        loader.LoadAndExecutePlugins(pluginsDir);
    }
}