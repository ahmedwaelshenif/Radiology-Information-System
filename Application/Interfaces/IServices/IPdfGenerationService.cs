
namespace Application.Interfaces.IServices
{
    public interface IPdfGenerationService
    {
        byte[] GeneratePdf(List<string> fields);
    }
}
