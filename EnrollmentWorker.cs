
public class EnrollmentWorker(IServiceScopeFactory scopeFactory)
{
    public async Task ProcessBatch()
    {
        using var scope = scopeFactory.CreateScope();

        var enrollmentService = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();

        var enrollments = await enrollmentService.GetAllAsync();

    }
}