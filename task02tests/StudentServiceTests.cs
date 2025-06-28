using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using task02;

namespace task02tests
{
    public class StudentServiceTests
    {
        private readonly List<Student> _testStudents;
        private readonly StudentService _service;

        public StudentServiceTests()
        {
            _testStudents = new List<Student>
            {
                new() { Name = "Иван", Faculty = "ФИТ", Grades = new List<int> { 5, 4, 5 } },
                new() { Name = "Анна", Faculty = "ФИТ", Grades = new List<int> { 3, 4, 3 } },
                new() { Name = "Петр", Faculty = "Экономика", Grades = new List<int> { 5, 5, 5 } },
                new() { Name = "Мария", Faculty = "Экономика", Grades = new List<int>() }
            };
            _service = new StudentService(_testStudents);
        }

        [Fact]
        public void GetStudentsByFaculty_ReturnsCorrectStudents()
        {
            var result = _service.GetStudentsByFaculty("ФИТ").ToList();
            result.Should().HaveCount(2);
            result.Should().OnlyContain(s => s.Faculty == "ФИТ");
        }

        [Fact]
        public void GetFacultyWithHighestAverageGrade_ReturnsCorrectFaculty()
        {
            var result = _service.GetFacultyWithHighestAverageGrade();
            result.Should().Be("Экономика");
        }

        [Fact]
        public void GetStudentsWithMinAverageGrade_FiltersCorrectly()
        {
            const double minGrade = 4.0;
            var result = _service.GetStudentsWithMinAverageGrade(minGrade).ToList();
            
            result.Should().HaveCount(2);
            result.Select(s => s.Name).Should().Contain("Иван");
            result.Select(s => s.Name).Should().Contain("Петр");
            result.Select(s => s.Name).Should().NotContain("Анна");
            result.Select(s => s.Name).Should().NotContain("Мария");
        }

        [Fact]
        public void GetStudentsOrderedByName_SortsCorrectly()
        {
            var result = _service.GetStudentsOrderedByName().ToList();
            var names = result.Select(s => s.Name).ToList();
            
            var expectedOrder = new List<string> { "Анна", "Иван", "Мария", "Петр" };
            names.Should().Equal(expectedOrder);
        }

        [Fact]
        public void GroupStudentsByFaculty_CreatesCorrectGroups()
        {
            var lookup = _service.GroupStudentsByFaculty();
            
            var fitGroup = lookup["ФИТ"].ToList();
            fitGroup.Should().HaveCount(2);
            fitGroup.Should().Contain(s => s.Name == "Иван");
            fitGroup.Should().Contain(s => s.Name == "Анна");
            
            var economyGroup = lookup["Экономика"].ToList();
            economyGroup.Should().HaveCount(2);
            economyGroup.Should().Contain(s => s.Name == "Петр");
            economyGroup.Should().Contain(s => s.Name == "Мария");
        }

        [Fact]
        public void GetFacultyWithHighestAverageGrade_HandlesEmptyList()
        {
            var emptyService = new StudentService(new List<Student>());
            var result = emptyService.GetFacultyWithHighestAverageGrade();
            result.Should().BeNull();
        }

        [Fact]
        public void GetStudentsWithMinAverageGrade_HandlesZeroGrades()
        {
            var result = _service.GetStudentsWithMinAverageGrade(0).ToList();
            result.Should().HaveCount(4);
        }
    }
}