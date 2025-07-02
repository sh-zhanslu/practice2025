﻿using System;

namespace task07
{
    [AttributeUsage(
        AttributeTargets.Class | 
        AttributeTargets.Method | 
        AttributeTargets.Property,
        AllowMultiple = false
    )]
    public class DisplayNameAttribute : Attribute
    {
        public string DisplayName { get; }

        public DisplayNameAttribute(string displayName)
        {
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }
    }
}