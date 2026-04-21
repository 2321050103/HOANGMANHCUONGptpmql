using DemoMVC.Models;

namespace DemoMVC.Data
{
    public class SchoolRepository
    {
        private readonly List<Faculty> _faculties;
        private readonly List<Student> _students;
        private int _nextStudentId;
        private readonly object _lock = new();

        public SchoolRepository()
        {
            _faculties = new List<Faculty>
            {
                new Faculty { FacultyId = 1, FacultyName = "Công nghệ thông tin" },
                new Faculty { FacultyId = 2, FacultyName = "Kinh tế" },
                new Faculty { FacultyId = 3, FacultyName = "Cơ khí" }
            };

            _students = new List<Student>();
            _nextStudentId = 1;
        }

        public List<Faculty> GetFaculties()
        {
            lock (_lock)
            {
                return _faculties
                    .Select(f => new Faculty
                    {
                        FacultyId = f.FacultyId,
                        FacultyName = f.FacultyName
                    })
                    .ToList();
            }
        }

        public List<Student> GetStudents()
        {
            lock (_lock)
            {
                return _students.Select(CloneStudentWithFaculty).ToList();
            }
        }

        public Student? GetStudentById(int id)
        {
            lock (_lock)
            {
                var student = _students.FirstOrDefault(s => s.Id == id);
                return student == null ? null : CloneStudentWithFaculty(student);
            }
        }

        public void AddStudent(Student student)
        {
            lock (_lock)
            {
                var newStudent = new Student
                {
                    Id = _nextStudentId++,
                    StudentCode = student.StudentCode,
                    Name = student.Name,
                    Age = student.Age,
                    Email = student.Email,
                    FacultyId = student.FacultyId
                };

                _students.Add(newStudent);
            }
        }

        public bool UpdateStudent(Student student)
        {
            lock (_lock)
            {
                var existingStudent = _students.FirstOrDefault(s => s.Id == student.Id);
                if (existingStudent == null)
                {
                    return false;
                }

                existingStudent.Name = student.Name;
                existingStudent.StudentCode = student.StudentCode;
                existingStudent.Age = student.Age;
                existingStudent.Email = student.Email;
                existingStudent.FacultyId = student.FacultyId;
                return true;
            }
        }

        public bool DeleteStudent(int id)
        {
            lock (_lock)
            {
                var student = _students.FirstOrDefault(s => s.Id == id);
                if (student == null)
                {
                    return false;
                }

                _students.Remove(student);
                return true;
            }
        }

        private Student CloneStudentWithFaculty(Student student)
        {
            var faculty = _faculties.FirstOrDefault(f => f.FacultyId == student.FacultyId);

            return new Student
            {
                Id = student.Id,
                StudentCode = student.StudentCode,
                Name = student.Name,
                Age = student.Age,
                Email = student.Email,
                FacultyId = student.FacultyId,
                Faculty = faculty == null
                    ? null
                    : new Faculty
                    {
                        FacultyId = faculty.FacultyId,
                        FacultyName = faculty.FacultyName
                    }
            };
        }
    }
}
