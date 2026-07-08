
public interface IEnrollmentService
{
    Task<EnrollmentRecord> EnrollAsync(string studentId, string courseId);
    Task<EnrollmentRecord?> GetByIdAsync(string id);
    Task<IReadOnlyList<EnrollmentRecord>> GetAllAsync();
    Task<bool> DeleteAsync(string id);
};

public class EnrollmentService : IEnrollmentService
{
    private readonly Dictionary<string, EnrollmentRecord> _store = new();
    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(ILogger<EnrollmentService> logger)
    {
        _logger = logger;
    }

    public Task<EnrollmentRecord> EnrollAsync(string studentId, string courseId)
    {
        //Check for duplicate enrollment
        var existing = _store.Values
            .FirstOrDefault(e => e.studentId == studentId && e.courseId == courseId);
        if(existing is not null)
        {
            _logger.LogWarning(
                "Duplicate enrollment attempt {studentId} already in {courseId} (record{EnrollmentId})",
                studentId, courseId, existing.id);
                return Task.FromResult(existing);
        }

        var id = Guid.NewGuid().ToString("N")[..8];
        var record = new EnrollmentRecord(id, studentId, courseId, DateTime.UtcNow);
        _store[id] = record;
        return Task.FromResult(record);
    }


    public Task<EnrollmentRecord?> GetByIdAsync(string id)
    {
        var record = _store.GetValueOrDefault(id);
        if(record is null)
        {
            _logger.LogWarning("Enrollment {EnrollmentId} not found", id);
        }
        
        return Task.FromResult(record);
    }

    public Task<IReadOnlyList<EnrollmentRecord>> GetAllAsync()
    {
        IReadOnlyList<EnrollmentRecord> all = _store.Values.ToList();
        return Task.FromResult(all);
    }

    public Task<bool> DeleteAsync(string id)
    {
        var removed = _store.Remove(id);
        if (removed)
        {
            _logger.LogInformation("Deleted enrollment {EnrollmentId}", id);
        }
        else
        {
            _logger.LogWarning("Delete failed enrollment {EnrollmentId} not found", id);
        }
        return Task.FromResult(removed);
    }
}

public record EnrollmentRecord(string id, string studentId, string courseId, DateTime EnrolledAt);