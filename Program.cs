var builder = WebApplication.CreateBuilder(args);

// Services: add authentication / authorization services
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// First(outer wrapper) middleware
app.UseMiddleware<RequestLoggingMiddleware>();

// TODO 1: Register routing in the pipeline where it belongs for your app.
app.UseRouting();

// TODO 2: Register authentication and authorization in the pipeline
app.UseStatusCodePages();
app.UseAuthentication();
app.UseAuthorization();

// TODO 3: Map GET /api/assessments/results with the same response body as the starter, but require authorization for that route.
app.MapGet("/api/assessments/results", () => Results.Ok(new
{
    courseCode = "CS-101",
    studentId = "S-001",
    letterGrade = "A"
})).RequireAuthorization();

app.Run();