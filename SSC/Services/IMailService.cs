using SSC.DTO.MailRequest;

namespace SSC.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequestDTO mailRequest);
    }
}
