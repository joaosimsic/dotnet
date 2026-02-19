using PhoneBook.Api.DTOs;

namespace PhoneBook.Api.Services;

public class DeletionLogService : IDeletionLogService
{
    private readonly string _logPath;
    private readonly ILogger<DeletionLogService> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public DeletionLogService(IConfiguration configuration, ILogger<DeletionLogService> logger)
    {
        _logger = logger;
        _logPath = Environment.GetEnvironmentVariable("DELETION_LOG_PATH") 
            ?? configuration["LogSettings:DeletionLogPath"] 
            ?? "/app/logs/deletion_log.txt";

        try
        {
            var directory = Path.GetDirectoryName(_logPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create deletion log directory at {Path}", _logPath);
            throw;
        }
    }

    public async Task LogDeletionAsync(ContactDto contact)
    {
        await _semaphore.WaitAsync();
        try
        {
            var phones = string.Join(", ", contact.Phones.Select(p => p.PhoneNumber));
            var logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Contact deleted: ID={contact.Id}, Name={contact.Name}, Age={contact.Age}, Phones=[{phones}]{Environment.NewLine}";
            await File.AppendAllTextAsync(_logPath, logEntry);
            _logger.LogInformation("Logged deletion of contact {ContactId}", contact.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log deletion for contact {ContactId}", contact.Id);
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
