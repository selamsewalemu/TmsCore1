using System.Diagnostics;

// ==============================
// Module 1 Lab — Session 1: The Data Model
// ==============================
// Exercise 1: Null Safety
string? region = null;
string? upperRegion = region?.ToUpper();
Console.WriteLine($"Region (conditional): {upperRegion}");

string displayRegion = region ?? "Unassigned";
Console.WriteLine($"Region (coalesced): {displayRegion}");

region ??= "Addis Ababa";
Console.WriteLine($"Region (assigned): {region}");

string studentName = "Abeba";
string studentId = "STU-001";
int enrollmentCount = 3;
decimal grantAmount = 1999.99m;
DateTime enrolledAt = DateTime.UtcNow;
string? campusRegion = null;

Console.WriteLine($"Student: {studentName} ({studentId})");
Console.WriteLine($"Courses: {enrollmentCount}");
Console.WriteLine($"Grant: {grantAmount:F2}");
Console.WriteLine($"Enrolled: {enrolledAt:yyyy-MM-dd}");
Console.WriteLine($"Campus: {campusRegion ?? "Not assigned"}");

// Exercise 2: Exact financial math
// Legacy bug: double introduced fractional drift in money calculations.
decimal grantPerStudent = 1999.99m;
decimal totalAllocation = grantPerStudent * 100_000m;
Console.WriteLine($"Total allocated (decimal): {totalAllocation}");
Console.WriteLine($"Total allocated (formatted): {totalAllocation:F2}");

// Exercise 3: Immutable record / data integrity
var enrollment = new EnrollmentRecord("STU-001", "CS-401", DateTime.UtcNow);
Console.WriteLine(enrollment);

var corrected = enrollment with { CourseCode = "CS-402" };
Console.WriteLine(corrected);

var duplicate = new EnrollmentRecord("STU-001", "CS-401", enrollment.EnrolledAt);
Console.WriteLine($"Same data? {enrollment == duplicate}");

// Exercise 3 — Part 2: Validated mutable course model
var course = new Course { Code = "CS-401", Title = "Advanced C#", Capacity = 30 };
Console.WriteLine($"Course: {course.Title} (Capacity: {course.Capacity})");

try
{
    course.Capacity = -5;
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine($"Caught: {ex.Message}");
}

try
{
    course.Title = "";
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Caught: {ex.Message}");
}

// Exercise 3 — Part 3: Student validation
var s = new Student { Id = "S1", Name = "Abeba", Age = 20, GPA = 3.8m };
Console.WriteLine($"Student: {s.Name}, GPA: {s.GPA}");

// Invalid examples: uncomment to see validation exceptions.
// var invalidName = new Student { Id = "S2", Name = "", Age = 20, GPA = 3.0m };
// var invalidAge = new Student { Id = "S3", Name = "Test", Age = 12, GPA = 3.0m };
// var invalidGpa = new Student { Id = "S4", Name = "Test", Age = 20, GPA = 5.0m };

// Exercise 3B: Interface contract wiring
void PrintGradeReport(IEnumerable<IGradable> assessments)
{
    Console.WriteLine("--- Grade Report ---");
    foreach (var item in assessments)
    {
        Console.WriteLine($"{item.Title}: {item.CalculateGrade():F2}%");
    }
}

IGradable[] cohortAssessments =
[
    new Quiz { Title = "C# Basics", CorrectAnswers = 18, TotalQuestions = 20 },
    new LabAssignment { Title = "Registration API", FunctionalityScore = 90m, CodeQualityScore = 85m }
];

PrintGradeReport(cohortAssessments);

// ==============================
// Module 1 Lab — Session 2: Query and Classification
// ==============================
// Exercise 4: Guard clauses and pattern matching
var service = new EnrollmentService();

var validStudent = new Student { Id = "S1", Name = "Abeba", Age = 20, GPA = 3.8m };
var validCourse = new Course { Code = "CS-401", Title = "Advanced C#", Capacity = 30 };
var result = service.ProcessRegistration(validStudent, validCourse);
Console.WriteLine($"Enrolled: {result.StudentId} in {result.CourseCode}");

try
{
    service.ProcessRegistration(null, validCourse);
}
catch (ArgumentNullException ex)
{
    Console.WriteLine($"Guard caught: {ex.ParamName}");
}

var fullCourse = new Course { Code = "CS-402", Title = "Full Course", Capacity = 1 };
fullCourse.EnrolledCount = 1;

try
{
    service.ProcessRegistration(validStudent, fullCourse);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Business rule: {ex.Message}");
}

// Exercise 5: LINQ queries and classification
List<Student> students =
[
    new Student { Id = "S1", Name = "Abeba", Age = 22, GPA = 3.8m },
    new Student { Id = "S2", Name = "Kidane", Age = 21, GPA = 2.4m },
    new Student { Id = "S3", Name = "Dawit", Age = 20, GPA = 3.1m },
    new Student { Id = "S4", Name = "Sara", Age = 23, GPA = 3.9m },
    new Student { Id = "S5", Name = "Frehiwot", Age = 19, GPA = 2.0m },
    new Student { Id = "S6", Name = "Yonas", Age = 24, GPA = 3.5m },
    new Student { Id = "S7", Name = "Meron", Age = 22, GPA = 1.8m },
    new Student { Id = "S8", Name = "Tesfaye", Age = 21, GPA = 2.9m }
];

var leaderboard = students
    .Where(s => s.GPA >= 3.5m)
    .OrderByDescending(s => s.GPA)
    .Select(s => s.Name)
    .ToList();

Console.WriteLine($"Found {leaderboard.Count} Honors Students:");
foreach (var name in leaderboard)
{
    Console.WriteLine($"- {name}");
}

decimal averageGpa = students.Count > 0 ? students.Average(s => s.GPA) : 0m;
Console.WriteLine($"\nClass Average GPA: {averageGpa:F2}");

var standingGroups = students
    .GroupBy(s => s.GPA switch
    {
        >= 3.5m => "Honors",
        >= 2.5m => "GoodStanding",
        >= 2.0m => "Probation",
        _ => "Academic Warning"
    });

Console.WriteLine("\n--- Academic Standing Report ---");
foreach (var group in standingGroups)
{
    Console.WriteLine($"\n{group.Key} ({group.Count()}):");
    foreach (var studentEntry in group)
    {
        Console.WriteLine($" {studentEntry.Name} GPA: {studentEntry.GPA}");
    }
}

string[] backendCourses = ["C#", "ASP.NET Core"];
string[] frontendCourses = ["TypeScript", "Angular"];
string[] allCourses = [.. backendCourses, .. frontendCourses, "Capstone"];
Console.WriteLine($"\nFull curriculum: {string.Join(", ", allCourses)}");

// ==============================
// Module 1 Lab — Session 3: Async and Resilience
// ==============================

// Exercise 6: async/await and parallelism
var sw = Stopwatch.StartNew();
for (int i = 0; i < 5; i++)
{
    Thread.Sleep(300);
}
Console.WriteLine($"Blocking sequential: {sw.ElapsedMilliseconds}ms");

sw.Restart();
for (int i = 0; i < 5; i++)
{
    await Task.Delay(300);
}
Console.WriteLine($"Async sequential: {sw.ElapsedMilliseconds}ms");

sw.Restart();
var tasks = Enumerable.Range(0, 5).Select(_ => Task.Delay(300));
await Task.WhenAll(tasks);
Console.WriteLine($"Async parallel: {sw.ElapsedMilliseconds}ms");

async Task<Student> FetchStudentAsync(string id)
{
    Console.WriteLine($" Fetching {id}...");
    await Task.Delay(300);

    return new Student
    {
        Id = id,
        Name = $"Student-{id}",
        Age = 20,
        GPA = id switch
        {
            "S1" => 3.8m,
            "S2" => 2.4m,
            "S3" => 3.5m,
            "S4" => 1.9m,
            "S5" => 3.2m,
            _ => 2.5m
        }
    };
}

async Task<Course> FetchCourseAsync(string code)
{
    Console.WriteLine($" Fetching course {code}...");
    await Task.Delay(200);

    return new Course
    {
        Code = code,
        Title = $"Course-{code}",
        Capacity = code switch
        {
            "CRS-101" => 2,
            "CRS-201" => 30,
            "CRS-301" => 15,
            _ => 25
        }
    };
}

sw.Restart();
string[] studentIds = ["S1", "S2", "S3", "S4", "S5"];
string[] courseCodes = ["CRS-101", "CRS-201", "CRS-301"];

var studentTasks = studentIds.Select(id => FetchStudentAsync(id));
var courseTasks = courseCodes.Select(code => FetchCourseAsync(code));

Student[] loadedStudents = await Task.WhenAll(studentTasks);
Course[] loadedCourses = await Task.WhenAll(courseTasks);
Console.WriteLine($"\nLoaded {loadedStudents.Length} students and {loadedCourses.Length} courses in {sw.ElapsedMilliseconds}ms");
foreach (var student in loadedStudents)
{
    Console.WriteLine($" {student.Name} GPA: {student.GPA}");
}

var enrollCourse = new Course { Code = "CRS-101", Title = "C# Mastery", Capacity = 2 };
var enrollService = new EnrollmentService();
var enrollments = new List<EnrollmentRecord>();
var failures = new List<string>();

sw.Restart();
foreach (var student in loadedStudents)
{
    try
    {
        var record = enrollService.ProcessRegistration(student, enrollCourse);
        enrollCourse.EnrolledCount++;
        enrollments.Add(record);
        Console.WriteLine($" Enrolled: {student.Name}");
    }
    catch (InvalidOperationException ex)
    {
        failures.Add($"{student.Name}: {ex.Message}");
        Console.WriteLine($" Rejected: {student.Name} {ex.Message}");
    }
}

async Task SendConfirmationAsync(Student student)
{
    try
    {
        await Task.Delay(100);
        Console.WriteLine($" Email sent to {student.Name}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Email failed for {student.Name}: {ex.Message}");
    }
}

try
{
    var overflowCourse = new Course { Code = "CRS-999", Title = "Overflow Test", Capacity = 2 };
    overflowCourse.EnrolledCount = 2;
    enrollService.ProcessRegistration(new Student { Id = "S99", Name = "Test", Age = 20, GPA = 3.0m }, overflowCourse);
}
catch (CapacityReachedException ex)
{
    Console.WriteLine();
    Console.WriteLine("Domain exception caught:");
    Console.WriteLine($" Course: {ex.CourseCode}");
    Console.WriteLine($" Message: {ex.Message}");
}

sw.Stop();
decimal classAverage = loadedStudents.Length > 0 ? loadedStudents.Average(s => s.GPA) : 0m;
Console.WriteLine("\n========== ENROLLMENT SUMMARY ==========");
Console.WriteLine($"Total students loaded: {loadedStudents.Length}");
Console.WriteLine($"Successful enrollments: {enrollments.Count}");
Console.WriteLine($"Failed enrollments: {failures.Count}");
Console.WriteLine($"Class average GPA: {classAverage:F2}");
Console.WriteLine($"Total elapsed time: {sw.ElapsedMilliseconds}ms");

if (failures.Count > 0)
{
    Console.WriteLine("\n--- Failure Details ---");
    foreach (var failure in failures)
    {
        Console.WriteLine($" {failure}");
    }
}

Console.WriteLine("========================================");

var notifier = new EnrollmentNotifier();
notifier.Listener += s => Console.WriteLine($"SMS SENT: Welcome to the TMS, {s.Name}!");
notifier.FinalizeEnrollment(new Student { Id = "S1", Name = "Abeba", Age = 20, GPA = 3.8m });
