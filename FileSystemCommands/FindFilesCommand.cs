using System;
using System.Collections.Generic;
using System.IO;
using CommandLib;

namespace FileSystemCommands
{
    public class FindFilesCommand : ICommand
    {
        private readonly string _directoryPath;
        private readonly string _searchPattern;

        public List<string> Result { get; private set; }

        public FindFilesCommand(string directoryPath, string searchPattern)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Каталог не найден: {directoryPath}");
            
            _directoryPath = directoryPath;
            _searchPattern = searchPattern;
        }

        public void Execute()
        {
            Result = new List<string>(Directory.GetFiles(
                _directoryPath, 
                _searchPattern, 
                SearchOption.AllDirectories
            ));
        }
    }
}