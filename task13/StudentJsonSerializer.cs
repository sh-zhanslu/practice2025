using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13
{
    public static class StudentJsonSerializer
    {
        private static JsonSerializerOptions GetOptions()
        {
            return new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new DateTimeConverter() },
                WriteIndented = true
            };
        }

        public static string Serialize(Student student)
        {
            ValidateStudent(student);
            return JsonSerializer.Serialize(student, GetOptions());
        }

        public static Student Deserialize(string json)
        {
            var options = GetOptions();
            var student = JsonSerializer.Deserialize<Student>(json, options);
            ValidateStudent(student);
            return student;
        }

        public static void SaveToFile(Student student, string filePath)
        {
            File.WriteAllText(filePath, Serialize(student));
        }

        public static Student LoadFromFile(string filePath)
        {
            return Deserialize(File.ReadAllText(filePath));
        }

        private static void ValidateStudent(Student student)
        {
            if (student == null)
                throw new ArgumentException("Invalid student data: null object");

            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(student);
            
            if (!Validator.TryValidateObject(student, context, validationResults, true))
            {
                var errors = string.Join("\n", validationResults.Select(r => r.ErrorMessage));
                throw new ValidationException($"Student validation failed:\n{errors}");
            }
            
            if (student.Grades != null)
            {
                foreach (var subject in student.Grades)
                {
                    validationResults.Clear();
                    if (!Validator.TryValidateObject(subject, 
                        new ValidationContext(subject), 
                        validationResults, 
                        true))
                    {
                        var errors = string.Join("\n", validationResults.Select(r => r.ErrorMessage));
                        throw new ValidationException($"Subject validation failed:\n{errors}");
                    }
                }
            }
        }
    }

    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private const string Format = "yyyy-MM-dd";
        
        public override DateTime Read(
            ref Utf8JsonReader reader, 
            Type typeToConvert, 
            JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString(), Format, null);
        }

        public override void Write(
            Utf8JsonWriter writer, 
            DateTime value, 
            JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }
    }
}