using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLib;
using FileSystemCommands;
using Xunit;

namespace task08tests
{
    public class FileSystemCommandsTests : IDisposable
    {
        private readonly string _testDir;
        private readonly string _subDir;

        public FileSystemCommandsTests()
        {
            _testDir = Path.Combine(Path.GetTempPath(), $"TestDir_{Guid.NewGuid().ToString()[..8]}");
            _subDir = Path.Combine(_testDir, "SubDir");
            
            Directory.CreateDirectory(_testDir);
            Directory.CreateDirectory(_subDir);
            
            File.WriteAllText(Path.Combine(_testDir, "test1.txt"), "Hello");
            File.WriteAllText(Path.Combine(_testDir, "test2.log"), "Log file");
            File.WriteAllText(Path.Combine(_subDir, "test3.txt"), "Another text");
            
            using (var fs = File.Create(Path.Combine(_testDir, "largefile.bin")))
            {
                fs.SetLength(1024);
            }
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDir))
            {
                try
                {
                    Directory.Delete(_testDir, true);
                }
                catch
                {
                    
                }
            }
        }

        [Fact]
        public void DirectorySizeCommand_ShouldCalculateCorrectSize()
        {
            long expectedSize = 
                new FileInfo(Path.Combine(_testDir, "test1.txt")).Length +
                new FileInfo(Path.Combine(_testDir, "test2.log")).Length +
                new FileInfo(Path.Combine(_subDir, "test3.txt")).Length +
                1024; 
            
            var command = new DirectorySizeCommand(_testDir);

            command.Execute();

            Assert.Equal(expectedSize, command.Result);
        }

        [Fact]
        public void DirectorySizeCommand_ShouldThrowForInvalidDirectory()
        {
            var invalidPath = Path.Combine(_testDir, "non_existent_folder");

            Assert.Throws<DirectoryNotFoundException>(() => new DirectorySizeCommand(invalidPath));
        }

        [Fact]
        public void FindFilesCommand_ShouldFindMatchingFiles()
        {
            var expectedFiles = new List<string>
            {
                Path.Combine(_testDir, "test1.txt"),
                Path.Combine(_subDir, "test3.txt")
            };
            
            var command = new FindFilesCommand(_testDir, "*.txt");

            command.Execute();
            var result = command.Result;

            Assert.Equal(2, result.Count);
            Assert.Contains(expectedFiles[0], result);
            Assert.Contains(expectedFiles[1], result);
        }

        [Fact]
        public void FindFilesCommand_ShouldFindAllFilesWithWildcard()
        {
            var command = new FindFilesCommand(_testDir, "*.*");

            command.Execute();

            Assert.Equal(4, command.Result.Count);
        }

        [Fact]
        public void FindFilesCommand_ShouldReturnEmptyForNoMatches()
        {
            var command = new FindFilesCommand(_testDir, "*.md");

            command.Execute();

            Assert.Empty(command.Result);
        }

        [Fact]
        public void FindFilesCommand_ShouldThrowForInvalidDirectory()
        {
            var invalidPath = Path.Combine(_testDir, "non_existent_folder");

            Assert.Throws<DirectoryNotFoundException>(() => new FindFilesCommand(invalidPath, "*.txt"));
        }

        [Fact]
        public void Commands_ShouldImplementICommandInterface()
        {
            var sizeCommand = new DirectorySizeCommand(_testDir);
            var findCommand = new FindFilesCommand(_testDir, "*.txt");

            Assert.IsAssignableFrom<ICommand>(sizeCommand);
            Assert.IsAssignableFrom<ICommand>(findCommand);
        }
    }
}