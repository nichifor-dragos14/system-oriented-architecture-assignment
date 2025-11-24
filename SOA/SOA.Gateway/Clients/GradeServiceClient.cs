using SOA.Dto.Grade;

namespace SOA.Gateway.Clients;

public class GradesServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GradesServiceClient> _logger;

    public GradesServiceClient(
        HttpClient httpClient,
        ILogger<GradesServiceClient> logger
    )
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Get all grades for a given student.
    /// Calls: GET /api/students/{studentId}/grades
    /// </summary>
    public async Task<IEnumerable<GradeDto>?> GetGradesForStudentAsync(Guid studentId, CancellationToken cancellationToken)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<GradeDto>>($"/api/grades/student/{studentId}", cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error fetching grades for student {StudentId}", studentId);

            return null;
        }
    }

    /// <summary>
    /// Add a grade for a given student.
    /// Calls: POST /api/students/grades
    /// </summary>
    public async Task<bool> AddGradeAsync(CreateGradeDto grade, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/grades", grade, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to add grade for student {StudentId}: {Status}", grade.StudentId, response.StatusCode);

                return false;
            }

            return response.IsSuccessStatusCode;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error adding grade for student {StudentId}", grade.StudentId);

            return false;
        }
    }

    /// <summary>
    /// Update a grade.
    /// Calls: PUT /api/grades/{gradeId}
    /// </summary>
    public async Task<bool> UpdateGradeAsync(Guid gradeId, UpdateGradeDto grade, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/grades/{gradeId}", grade, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to update grade {GradeId}: {Status}", gradeId, response.StatusCode);
            }

            return response.IsSuccessStatusCode;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error updating grade {GradeId}", gradeId);

            return false;
        }
    }

    /// <summary>
    /// Delete a grade.
    /// Calls: DELETE /api/grades/{gradeId}
    /// </summary>
    public async Task<bool> DeleteGradeAsync(Guid gradeId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/grades/{gradeId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to delete grade {GradeId}: {Status}", gradeId, response.StatusCode);
            }

            return response.IsSuccessStatusCode;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error deleting grade {GradeId}", gradeId);

            return false;
        }
    }
}