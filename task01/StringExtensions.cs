using System.Linq;
namespace task01;

public static class StringExtensions
{
    public static bool IsPalindrome(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;
        string normalized = new string(
            input.ToLower()
                .Where(c => !char.IsPunctuation(c) && !char.IsWhiteSpace(c))
                .ToArray()
        );
        // Проверка на палиндром
        return normalized.SequenceEqual(normalized.Reverse());
    }
}