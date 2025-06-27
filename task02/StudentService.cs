using System.Collections.Generic;
using System.Linq;

namespace task02
{
    public class StudentService
    {
        private readonly List<Student> _students;

        public StudentService(List<Student> students) => _students = students;

        public IEnumerable<Student> GetStudentsByFaculty(string faculty)
            => _students.Where(s => s.Faculty == faculty);

        public IEnumerable<Student> GetStudentsWithMinAverageGrade(double minAverageGrade)
            => _students.Where(s => s.Grades.DefaultIfEmpty(0).Average() >= minAverageGrade);

        public IEnumerable<Student> GetStudentsOrderedByName()
            => _students.OrderBy(s => s.Name);

        public ILookup<string, Student> GroupStudentsByFaculty()
            => _students.ToLookup(s => s.Faculty);

        public string? GetFacultyWithHighestAverageGrade()
            => _students
                .GroupBy(s => s.Faculty)
                .Select(g => new 
                {
                    Faculty = g.Key,
                    Avg = g.SelectMany(s => s.Grades).Any() 
                        ? g.SelectMany(s => s.Grades).Average()
                        : 0
                })
                .OrderByDescending(x => x.Avg)
                .FirstOrDefault()?.Faculty;
    }
}