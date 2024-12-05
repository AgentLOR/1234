using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[Serializable]
public class Student
{
    public string FullName { get; set; }
    public int Age { get; set; }
    public int YearOfBirth { get; set; }
    public string Group { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public List<Grade> Grades { get; set; } = new List<Grade>();
}

[Serializable]
public class Teacher
{
    public string FullName { get; set; }
    public int YearOfBirth { get; set; }
    public List<string> Subjects { get; set; } = new List<string>();
    public string Group { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
}

[Serializable]
public class Grade
{
    public string Subject { get; set; }
    public int Score { get; set; }
    public DateTime Date { get; set; }
}

public static class DataStorage
{
    private const string StudentFile = "students.bin";
    private const string TeacherFile = "teachers.bin";

    public static void SaveStudents(List<Student> students)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(StudentFile, FileMode.Create)))
        {
            foreach (var student in students)
            {
                writer.Write(student.FullName);
                writer.Write(student.Age);
                writer.Write(student.YearOfBirth);
                writer.Write(student.Group);
                writer.Write(student.Login);
                writer.Write(student.Password);
                writer.Write(student.Grades.Count);
                foreach (var grade in student.Grades)
                {
                    writer.Write(grade.Subject);
                    writer.Write(grade.Score);
                    writer.Write(grade.Date.ToBinary());
                }
            }
        }
    }

    public static List<Student> LoadStudents()
    {
        var students = new List<Student>();
        if (File.Exists(StudentFile))
        {
            using (BinaryReader reader = new BinaryReader(File.Open(StudentFile, FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    var student = new Student
                    {
                        FullName = reader.ReadString(),
                        Age = reader.ReadInt32(),
                        YearOfBirth = reader.ReadInt32(),
                        Group = reader.ReadString(),
                        Login = reader.ReadString(),
                        Password = reader.ReadString()
                    };
                    int gradesCount = reader.ReadInt32();
                    for (int i = 0; i < gradesCount; i++)
                    {
                        student.Grades.Add(new Grade
                        {
                            Subject = reader.ReadString(),
                            Score = reader.ReadInt32(),
                            Date = DateTime.FromBinary(reader.ReadInt64())
                        });
                    }
                    students.Add(student);
                }
            }
        }
        return students;
    }

    public static void SaveTeachers(List<Teacher> teachers)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(TeacherFile, FileMode.Create)))
        {
            foreach (var teacher in teachers)
            {
                writer.Write(teacher.FullName);

                writer.Write(teacher.YearOfBirth);
                writer.Write(teacher.Group);
                writer.Write(teacher.Login);
                writer.Write(teacher.Password);
                writer.Write(teacher.Subjects.Count);
                foreach (var subject in teacher.Subjects)
                {
                    writer.Write(subject);
                }
            }
        }
    }

    public static List<Teacher> LoadTeachers()
    {
        var teachers = new List<Teacher>();
        if (File.Exists(TeacherFile))
        {
            using (BinaryReader reader = new BinaryReader(File.Open(TeacherFile, FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {

                    var teacher = new Teacher
                    {
                        FullName = reader.ReadString(),
                        YearOfBirth = reader.ReadInt32(),
                        Group = reader.ReadString(),
                        Login = reader.ReadString(),
                        Password = reader.ReadString()
                    };
                    int subjectsCount = reader.ReadInt32();
                    for (int i = 0; i < subjectsCount; i++)
                    {
                        teacher.Subjects.Add(reader.ReadString());
                    }
                    teachers.Add(teacher);
                }
            }
        }
        return teachers;
    }
}

class Program
{
    private static List<Student> students = DataStorage.LoadStudents();
    private static List<Teacher> teachers = DataStorage.LoadTeachers();

    static void Main(string[] args)
    {
        // Сначала сохраняем студентов и преподавателей, если файлы пустые
        if (!students.Any())
        {
            InitializeTestData();
        }

        while (true)
        {
            Console.WriteLine("Введите логин:");
            string login = Console.ReadLine();

            Console.WriteLine("Введите пароль:");
            string password = Console.ReadLine();

            if (ValidateUser(login, password, out UserRole role))
            {
                switch (role)
                {
                    case UserRole.Student:
                        StudentMenu(login);
                        break;
                    case UserRole.Teacher:
                        TeacherMenu(login);
                        break;
                    case UserRole.Admin:
                        AdminMenu();
                        break;
                }
            }
            else
            {
                Console.WriteLine("Неверный логин или пароль. Попробуйте снова.");
            }
        }
    }

    private static void InitializeTestData()
    {
        // Создание тестовых данных для студентов и преподавателей
        var student = new Student
        {
            FullName = "Мосин Иван Алексеевич",
            Age = 18,
            YearOfBirth = 2006,
            Group = "СА50-3-22",
            Login = "mofix", // Изменён логин на "mofix"
            Password = "123"
        };
        student.Grades.Add(new Grade { Subject = "Математика", Score = 5, Date = DateTime.Now });
        student.Grades.Add(new Grade { Subject = "Программирование", Score = 4, Date = DateTime.Now });

        var studentsList = new List<Student> { student };
        DataStorage.SaveStudents(studentsList);

        var teacher = new Teacher
        {
            FullName = "Юшина Дарья Денисовна",
            YearOfBirth = 1939,
            Group = "СА50-3-22",
            Login = "teach", // Изменён логин на "teach"
            Password = "123",

            Subjects = new List<string> { "Программирование", "Математика" }
        };

        var teachersList = new List<Teacher> { teacher };
        DataStorage.SaveTeachers(teachersList);
    }

    private static void StudentMenu(string login)
    {
        var student = students.FirstOrDefault(s => s.Login == login);
        if (student == null) return;

        Console.WriteLine("Меню студента:");
        Console.WriteLine("Оценки:");
        foreach (var grade in student.Grades)
        {
            Console.WriteLine($"Предмет: {grade.Subject}, Оценка: {grade.Score}, Дата: {grade.Date.ToShortDateString()}");
        }
        Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в главное меню.");
        Console.ReadKey();
    }

    private static void TeacherMenu(string login)
    {
        var teacher = teachers.FirstOrDefault(t => t.Login == login);
        if (teacher == null) return;

        Console.WriteLine($"Меню преподавателя {teacher.FullName}:");
        Console.WriteLine("Дисциплины: " + string.Join(", ", teacher.Subjects));

        while (true)
        {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Просмотреть журнал оценок");
            Console.WriteLine("2. Выставить оценку");
            Console.WriteLine("3. Изменить оценку");
            Console.WriteLine("4. Удалить оценку");
            Console.WriteLine("5. Выйти");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewGrades(teacher);
                    break;
                case "2":
                    AddGrade(teacher);
                    break;
                case "3":
                    EditGrade(teacher);
                    break;
                case "4":
                    RemoveGrade(teacher);
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Некорректный выбор. Попробуйте снова.");
                    break;
            }
        }
    }

    private static void ViewGrades(Teacher teacher)
    {
        Console.WriteLine("Журнал оценок:");
        foreach (var student in students.Where(s => s.Group == teacher.Group))
        {
            Console.WriteLine($"Студент: {student.FullName}");
            foreach (var grade in student.Grades.Where(g => teacher.Subjects.Contains(g.Subject)))
            {
                Console.WriteLine($"  Предмет: {grade.Subject}, Оценка: {grade.Score}, Дата выставления: {grade.Date.ToShortDateString()}");
            }
        }
        Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в меню преподавателя.");
        Console.ReadKey();
    }

    private static void AddGrade(Teacher teacher)
    {
        Console.WriteLine("Введите ФИО студента, которому хотите выставить оценку:");
        string studentName = Console.ReadLine();
        var student = students.FirstOrDefault(s => s.FullName.Equals(studentName, StringComparison.OrdinalIgnoreCase) && s.Group == teacher.Group);

        if (student == null)
        {
            Console.WriteLine("Студент не найден или не в вашей группе.");
            return;
        }

        Console.WriteLine("Введите предмет:");
        string subject = Console.ReadLine();
        if (!teacher.Subjects.Contains(subject))
        {
            Console.WriteLine("Вы не можете выставить оценку за этот предмет.");
            return;
        }

        Console.WriteLine("Введите оценку (1-5):");
        int score;
        while (!int.TryParse(Console.ReadLine(), out score) || score < 1 || score > 5)
        {

            Console.WriteLine("Некорректная оценка. Попробуйте снова.");
        }

        var grade = new Grade
        {
            Subject = subject,
            Score = score,
            Date = DateTime.Now
        };

        student.Grades.Add(grade);
        DataStorage.SaveStudents(students); // Сохранение изменений
        Console.WriteLine("Оценка успешно выставлена.");
    }

    private static void EditGrade(Teacher teacher)
    {
        Console.WriteLine("Введите ФИО студента, у которого хотите изменить оценку:");
        string studentName = Console.ReadLine();
        var student = students.FirstOrDefault(s => s.FullName.Equals(studentName, StringComparison.OrdinalIgnoreCase) && s.Group == teacher.Group);

        if (student == null)
        {
            Console.WriteLine("Студент не найден или не в вашей группе.");
            return;
        }

        Console.WriteLine("Введите предмет:");
        string subject = Console.ReadLine();

        var grade = student.Grades.FirstOrDefault(g => g.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase));
        if (grade != null)
        {
            Console.WriteLine($"Текущая оценка: {grade.Score}. Введите новую оценку (1-5):");
            int newScore;
            while (!int.TryParse(Console.ReadLine(), out newScore) || newScore < 1 || newScore > 5)
            {
                Console.WriteLine("Некорректная оценка. Попробуйте снова.");
            }
            grade.Score = newScore;
            grade.Date = DateTime.Now; // Обновление даты
            DataStorage.SaveStudents(students); // Сохранение изменений
            Console.WriteLine("Оценка успешно изменена.");
        }
        else
        {
            Console.WriteLine("Оценка за указанный предмет не найдена.");
        }
    }

    private static void RemoveGrade(Teacher teacher)
    {
        Console.WriteLine("Введите ФИО студента, у которого хотите удалить оценку:");
        string studentName = Console.ReadLine();
        var student = students.FirstOrDefault(s => s.FullName.Equals(studentName, StringComparison.OrdinalIgnoreCase) && s.Group == teacher.Group);

        if (student == null)
        {
            Console.WriteLine("Студент не найден или не в вашей группе.");
            return;
        }

        Console.WriteLine("Введите предмет:");
        string subject = Console.ReadLine();

        var grade = student.Grades.FirstOrDefault(g => g.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase));
        if (grade != null)
        {
            student.Grades.Remove(grade);
            DataStorage.SaveStudents(students); // Сохранение изменений
            Console.WriteLine("Оценка успешно удалена.");
        }
        else
        {
            Console.WriteLine("Оценка за указанный предмет не найдена.");
        }
    }

    private static void AdminMenu()
    {
        Console.WriteLine("Меню администратора:");

        while (true)
        {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Просмотр всех студентов");
            Console.WriteLine("2. Просмотр всех преподавателей");
            Console.WriteLine("3. Выход");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewAllStudents();
                    break;
                case "2":
                    ViewAllTeachers();
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Некорректный выбор. Попробуйте снова.");
                    break;
            }
        }
    }

    private static void ViewAllStudents()
    {
        Console.WriteLine("Список студентов:");
        foreach (var student in students)
        {
            Console.WriteLine($"ФИО: {student.FullName}, Возраст: {student.Age}, Группа: {student.Group}");
        }
        Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в меню администратора.");
        Console.ReadKey();
    }

    private static void ViewAllTeachers()
    {
        Console.WriteLine("Список преподавателей:");
        foreach (var teacher in teachers)
        {
            Console.WriteLine($"ФИО: {teacher.FullName}, Дисциплины: {string.Join(", ", teacher.Subjects)}, Группа: {teacher.Group}");
        }
        Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в меню администратора.");
        Console.ReadKey();
    }

    private static bool ValidateUser(string login, string password, out UserRole role)
    {
        var student = students.FirstOrDefault(s => s.Login == login && s.Password == password);
        if (student != null)
        {
            role = UserRole.Student;
            return true;
        }

        var teacher = teachers.FirstOrDefault(t => t.Login == login && t.Password == password);
        if (teacher != null)
        {
            role = UserRole.Teacher;
            return true;
        }

        if (login == "admin" && password == "123")
        {
            role = UserRole.Admin;
            return true;
        }

        role = UserRole.None;
        return false;
    }

    enum UserRole
    {
        None,
        Student,
        Teacher,
        Admin
    }
}
