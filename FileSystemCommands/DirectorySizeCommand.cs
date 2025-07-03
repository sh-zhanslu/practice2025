using System;
using System.IO;
using CommandLib;

namespace FileSystemCommands
{
    public class DirectorySizeCommand : ICommand
    {
        private readonly string _directoryPath;

        public long Result { get; private set; }

        public DirectorySizeCommand(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Каталог не найден: {directoryPath}");
            
            _directoryPath = directoryPath;
        }

        public void Execute()
        {
            Result = CalculateDirectorySize(_directoryPath);
        }

        private long CalculateDirectorySize(string directory)
        {
            long size = 0;
            
            foreach (var file in Directory.GetFiles(directory))
            {
                size += new FileInfo(file).Length;
            }
            
            foreach (var subDir in Directory.GetDirectories(directory))
            {
                size += CalculateDirectorySize(subDir);
            }
            return size;
        }
    }
}
