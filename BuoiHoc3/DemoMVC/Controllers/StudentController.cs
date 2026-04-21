using DemoMVC.Data;
using DemoMVC.Models;
using DemoMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClosedXML.Excel;

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

        // ================== PHẦN MỚI: UPLOAD EXCEL ==================

        // Hiển thị form upload
        public IActionResult UploadExcel()
        {
            return View();
        }

        // Xử lý upload + đọc file Excel
        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "File không hợp lệ";
                return View();
            }

            // Lưu file tạm
            var filePath = Path.Combine(Path.GetTempPath(), file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Đọc dữ liệu từ Excel
            var students = ReadExcel(filePath);

            // Lưu vào DB
            foreach (var student in students)
            {
                _repository.AddStudent(student);
            }

            ViewBag.Message = "Upload thành công!";
            return View();
        }

        // Hàm đọc Excel
        private List<Student> ReadExcel(string filePath)
        {
            var list = new List<Student>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                foreach (var row in rows)
                {
                    // Bỏ dòng trống
                    if (string.IsNullOrEmpty(row.Cell(1).GetValue<string>()))
                        continue;

                    var student = new Student
                    {
                        // ⚠️ PHẢI KHỚP MODEL CỦA BẠN
                        Name = row.Cell(1).GetValue<string>(),

                        // Gán tạm FacultyId (tránh lỗi)
                        FacultyId = 1
                    };

                    list.Add(student);
                }
            }

            return list;
        }
    }
}
