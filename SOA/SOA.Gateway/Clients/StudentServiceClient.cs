using SOA.Dto.Student;

namespace SOA.Gateway.Clients;

public class StudentsServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<StudentsServiceClient> _logger;

    public StudentsServiceClient(
        HttpClient httpClient,
        ILogger<StudentsServiceClient> logger
    )
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all students from the Students microservice.
    /// </summary>
    public async Task<IEnumerable<StudentDto>?> GetAllStudentsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/students", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch students: {StatusCode}", response.StatusCode);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<IEnumerable<StudentDto>>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching students from StudentsService");

            return null;
        }
    }

    /// <summary>
    /// Retrieves a single student by ID.
    /// </summary>
    public async Task<StudentDto?> GetStudentByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<StudentDto>($"/api/students/{id}", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching student with ID {Id}", id);

            return null;
        }
    }

    /// <summary>
    /// Creates a new student.
    /// </summary>
    public async Task<bool> CreateStudentAsync(StudentDto student, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/students", student, cancellationToken);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating student");

            return false;
        }
    }
}