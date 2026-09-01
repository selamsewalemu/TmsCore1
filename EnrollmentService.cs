public class EnrollmentService
{
    public EnrollmentRecord ProcessRegistration(Student? student, Course? course)
    {
        if (student is null)
        {
            throw new ArgumentNullException(nameof(student));
        }

        if (course is null)
        {
            throw new ArgumentNullException(nameof(course));
        }

        if (course.Capacity <= 0)
        {
            throw new InvalidOperationException("Course capacity must be greater than zero.");
        }

        if (course.EnrolledCount >= course.Capacity)
        {
            throw new CapacityReachedException(course.Code);
        }

        string standing = student.GPA switch
        {
            >= 3.5m => "Honors",
            >= 2.5m => "GoodStanding",
            >= 2.0m => "Probation",
            _ => "AcademicWarning"
        };

        Console.WriteLine($"{student.Name} is in {standing}.");

        course.EnrolledCount++;

        return new EnrollmentRecord(student.Id, course.Code, DateTime.UtcNow);
    }
}
