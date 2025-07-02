using System;

namespace task07
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class VersionAttribute : Attribute
    {
        public int Major { get; }
        public int Minor { get; }
        
        public string Version => $"{Major}.{Minor}";

        public VersionAttribute(int major, int minor)
        {
            Major = major;
            Minor = minor;
        }
    }
}