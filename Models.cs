using System;

// ==============================
// TMS Domain Models
// ==============================
// These types model the core data the Training Management System uses.
// Each model is built to protect valid state and prevent the common runtime bugs
// described in the lab: null values, invalid numeric ranges, and accidental mutation.

// This record represents an enrollment event after it has been processed.
// Records are immutable by default, which means a logging pipeline cannot silently
// mutate the enrollment data after a student has been enrolled.
public record EnrollmentRecord(string StudentId, string CourseCode, DateTime EnrolledAt);

// The Course entity is mutable because course settings can evolve during a semester,
// but it still validates input at the moment values are assigned.
public class Course
{
    // Code is required and must be set during object initialization.
    public required string Code { get; init; }

    // Title is validated so blank or whitespace values are rejected immediately.
    public required string Title
    {
        get;
        set => field = !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException("Title cannot be empty or whitespace.", nameof(value));
    }

    // Capacity prevents negative or zero values, which protects the schedule logic.
    public int Capacity
    {
        get;
        set => field = value > 0
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), "System constraint: Capacity must be greater than zero.");
    }

    // EnrolledCount tracks how many students are currently in that course.
    public int EnrolledCount { get; set; }
}

// The Student entity enforces valid values for identity, age, and GPA.
public class Student
{
    // Id is required to ensure the student record is always identifiable.
    public required string Id { get; init; }

    // Name cannot be empty or whitespace.
    public required string Name
    {
        get;
        set => field = !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException("Name cannot be empty or whitespace.", nameof(value));
    }

    // Age is restricted to a realistic student age range.
    public int Age
    {
        get;
        set => field = value is >= 16 and <= 100
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), "Age must be between 16 and 100.");
    }

    // GPA is kept within the academic scale used by the system.
    public decimal GPA
    {
        get;
        set => field = value is >= 0.0m and <= 4.0m
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), "GPA must be between 0.0 and 4.0.");
    }
}

// Interface contract for all grade-bearing assessments used in a reporting pipeline.
public interface IGradable
{
    string Title { get; }
    decimal CalculateGrade();
}

// Quiz grades are based on correct answers over total questions.
public class Quiz : IGradable
{
    public required string Title { get; init; }
    public required int CorrectAnswers { get; init; }
    public required int TotalQuestions { get; init; }

    public decimal CalculateGrade()
    {
        if (TotalQuestions == 0)
        {
            return 0m;
        }

        return (decimal)CorrectAnswers / TotalQuestions * 100m;
    }
}

// Lab assignments mix functionality and code-quality scores using the course rubric.
public class LabAssignment : IGradable
{
    public required string Title { get; init; }
    public required decimal FunctionalityScore { get; init; }
    public required decimal CodeQualityScore { get; init; }

    public decimal CalculateGrade()
    {
        return (FunctionalityScore * 0.7m) + (CodeQualityScore * 0.3m);
    }
}

public class TmsDatabaseException : Exception
{
    public string Operation { get; }

    public TmsDatabaseException(string operation, string message)
        : base(message)
    {
        Operation = operation;
    }

    public TmsDatabaseException(string operation, string message, Exception innerException)
        : base(message, innerException)
    {
        Operation = operation;
    }
}

public class CapacityReachedException : InvalidOperationException
{
    public string CourseCode { get; }

    public CapacityReachedException(string courseCode)
        : base($"Course {courseCode} has reached maximum capacity.")
    {
        CourseCode = courseCode;
    }

    public CapacityReachedException(string courseCode, Exception innerException)
        : base($"Course {courseCode} has reached maximum capacity.", innerException)
    {
        CourseCode = courseCode;
    }
}

public delegate void StudentNotificationHandler(Student student);

public class EnrollmentNotifier
{
    public event StudentNotificationHandler? Listener;

    public void FinalizeEnrollment(Student student)
    {
        Console.WriteLine("Persisting to database...");
        Listener?.Invoke(student);
    }
}