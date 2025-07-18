using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Xunit;
using task13;

namespace task13tests
{
    public class StudentJsonSerializerTests
    {
        [Fact]
        public void Serialize_IgnoresNullValues()
        {
            var student = new Student
            {
                FirstName = "Anna",
                LastName = "Smith",
                BirthDate = new DateTime(2000, 1, 1)
            };
            
            string json = StudentJsonSerializer.Serialize(student);
            Assert.DoesNotContain("Grades", json);
        }

        [Fact]
        public void Serialize_UsesCustomDateFormat()
        {
            var student = new Student
            { 
                FirstName = "Test", 
                LastName = "User", 
                BirthDate = new DateTime(2020, 1, 1)
            };
            string json = StudentJsonSerializer.Serialize(student);
            Assert.Contains("2020-01-01", json);
        }

        [Fact]
        public void Deserialize_ThrowsOnInvalidData()
        {
            string invalidJson = @"{""LastName"":""Smith"",""BirthDate"":""1990-01-01""}";
            Assert.Throws<ValidationException>(() => 
                StudentJsonSerializer.Deserialize(invalidJson));
        }

        [Fact]
        public void FileOperations_WorkCorrectly()
        {
            var student = new Student
            { 
                FirstName = "File", 
                LastName = "Test", 
                BirthDate = new DateTime(2000, 1, 1),
                Grades = { new Subject { Name = "Math", Grade = 5 } }
            };
            
            string path = "test_student.json";
            StudentJsonSerializer.SaveToFile(student, path);
            Student loaded = StudentJsonSerializer.LoadFromFile(path);
            File.Delete(path);
            
            Assert.Equal(student.FirstName, loaded.FirstName);
            Assert.Equal(student.LastName, loaded.LastName);
            Assert.Equal(student.BirthDate, loaded.BirthDate);
            Assert.Single(loaded.Grades);
        }

        [Fact]
        public void Validation_PassesForValidObject()
        {
            var student = new Student
            {
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateTime(1995, 5, 15),
                Grades = { new Subject { Name = "Physics", Grade = 4 } }
            };
            
            var exception = Record.Exception(() => StudentJsonSerializer.Serialize(student));
            Assert.Null(exception);
        }
    }
}