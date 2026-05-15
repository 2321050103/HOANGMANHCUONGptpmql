using ClosedXML.Excel;
using DemoMVC.Data;
using DemoMVC.Models;
using DemoMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DemoMVC.Controllers
{
    public class StudentController : Controller
    {
        private readonly SchoolRepository _repository;

        public StudentController(SchoolRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var data = _repository.GetStudents()
                .Select(s => new StudentVM
                {
                    StudentId = s.Id,
                    StudentCode = s.StudentCode,
                    Name = s.Name,
                    Age = s.Age,
                    Email = s.Email,
                    FacultyName = s.Faculty?.FacultyName ?? string.Empty
                })
                .ToList();

            return View(data);
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            var data = _repository.GetStudents()
                .Select(s => new
                {
                    id = s.Id,
                    studentCode = s.StudentCode,
                    name = s.Name,
                    age = s.Age,
                    email = s.Email,
                    facultyId = s.FacultyId,
                    facultyName = s.Faculty?.FacultyName ?? string.Empty
                })
                .ToList();

            return Json(data);
        }

        [HttpGet]
        public IActionResult GetFaculties()
        {
            var data = _repository.GetFaculties()
                .Select(f => new
                {
                    facultyId = f.FacultyId,
                    facultyName = f.FacultyName
                })
                .ToList();

            return Json(data);
        }

        [HttpPost]
        public IActionResult AjaxCreate([FromBody] Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _repository.AddStudent(student);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult AjaxUpdate([FromBody] Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_repository.UpdateStudent(student))
            {
                return NotFound();
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult AjaxDelete(int id)
        {
            if (!_repository.DeleteStudent(id))
            {
                return NotFound();
            }

            return Json(new { success = true });
        }

        public IActionResult Create()
        {
            LoadFacultyDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student student)
        {
            if (!ModelState.IsValid)
            {
                LoadFacultyDropDownList(student.FacultyId);
                return View(student);
            }

            _repository.AddStudent(student);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = _repository.GetStudentById(id.Value);
            if (student == null)
            {
                return NotFound();
            }

            LoadFacultyDropDownList(student.FacultyId);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                LoadFacultyDropDownList(student.FacultyId);
                return View(student);
            }

            if (!_repository.UpdateStudent(student))
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = _repository.GetStudentById(id.Value);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!_repository.DeleteStudent(id))
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        private void LoadFacultyDropDownList(object? selectedFaculty = null)
        {
            ViewBag.FacultyId = new SelectList(
                _repository.GetFaculties(),
                "FacultyId",
                "FacultyName",
                selectedFaculty);
        }

        public IActionResult UploadExcel()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "File không hợp lệ.";
                return View();
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".xlsx")
            {
                ViewBag.Message = "Chỉ hỗ trợ file Excel định dạng .xlsx.";
                return View();
            }

            var filePath = Path.Combine(Path.GetTempPath(), file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            try
            {
                var students = ReadExcel(filePath);

                foreach (var student in students)
                {
                    _repository.AddStudent(student);
                }

                ViewBag.Message = $"Upload thành công {students.Count} sinh viên!";
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            return View();
        }

        private List<Student> ReadExcel(string filePath)
        {
            var list = new List<Student>();
            var validFacultyIds = _repository.GetFaculties()
                .Select(f => f.FacultyId)
                .ToHashSet();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                var usedRange = worksheet.RangeUsed();

                if (usedRange == null)
                {
                    throw new Exception("File Excel không có dữ liệu.");
                }

                var rows = usedRange.RowsUsed().Skip(1);

                foreach (var row in rows)
                {
                    var studentCode = row.Cell(1).GetValue<string>().Trim();
                    if (string.IsNullOrWhiteSpace(studentCode))
                    {
                        continue;
                    }

                    var name = row.Cell(2).GetValue<string>().Trim();
                    var ageText = row.Cell(3).GetValue<string>().Trim();
                    var email = row.Cell(4).GetValue<string>().Trim();
                    var facultyIdText = row.Cell(5).GetValue<string>().Trim();

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        throw new Exception($"Dòng {row.RowNumber()}: Name không được để trống.");
                    }

                    if (!int.TryParse(ageText, out var age))
                    {
                        throw new Exception($"Dòng {row.RowNumber()}: Age phải là số nguyên.");
                    }

                    if (string.IsNullOrWhiteSpace(email))
                    {
                        throw new Exception($"Dòng {row.RowNumber()}: Email không được để trống.");
                    }

                    if (!int.TryParse(facultyIdText, out var facultyId) || !validFacultyIds.Contains(facultyId))
                    {
                        throw new Exception($"Dòng {row.RowNumber()}: FacultyId phải là 1, 2 hoặc 3.");
                    }

                    list.Add(new Student
                    {
                        StudentCode = studentCode,
                        Name = name,
                        Age = age,
                        Email = email,
                        FacultyId = facultyId
                    });
                }
            }

            return list;
        }
    }
}
