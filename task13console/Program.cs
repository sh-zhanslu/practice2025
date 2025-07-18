using System;
using System.Collections.Generic;
using task13;

namespace task13console
{
    class Program
    {
        static void Main()
        {
            try
            {
                var student = new Student
                {
                    FirstName = "Иван",
                    LastName = "Иванов",
                    BirthDate = new DateTime(2003, 5, 15),
                    Grades = new List<Subject>
                    {
                        new Subject { Name = "Математика", Grade = 5 },
                        new Subject { Name = "Физика", Grade = 4 }
                    }
                };

                Console.WriteLine("1. Сериализация в JSON:");
                string json = StudentJsonSerializer.Serialize(student);
                Console.WriteLine(json);

                Console.WriteLine("\n2. Сохранение в файл...");
                StudentJsonSerializer.SaveToFile(student, "student.json");
                
                Console.WriteLine("3. Загрузка из файла...");
                Student loadedStudent = StudentJsonSerializer.LoadFromFile("student.json");
                
                Console.WriteLine("\nЗагруженные данные:");
                Console.WriteLine($"Имя: {loadedStudent.FirstName} {loadedStudent.LastName}");
                Console.WriteLine($"Дата рождения: {loadedStudent.BirthDate:yyyy-MM-dd}");
                Console.WriteLine("Оценки:");
                foreach (var subject in loadedStudent.Grades)
                {
                    Console.WriteLine($"  {subject.Name}: {subject.Grade}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}