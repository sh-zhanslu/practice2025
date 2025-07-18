using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace task13
{
    public class Subject
    {
        [Required(ErrorMessage = "Subject name is required")]
        public string Name { get; set; }

        [Range(1, 5, ErrorMessage = "Grade must be between 1 and 5")]
        public int Grade { get; set; }
    }

    public class Student
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        public List<Subject> Grades { get; set; } = new List<Subject>();
    }
}