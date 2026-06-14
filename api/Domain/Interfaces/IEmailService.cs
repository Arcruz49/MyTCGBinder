namespace MyTCGBinder.Application.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetAsync(string toEmail, string token);
}