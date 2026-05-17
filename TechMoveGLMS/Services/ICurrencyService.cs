namespace TechMoveGLMS.Services
{
    public interface ICurrencyService
    {
        Task<decimal> GetUsdToZarRate();
        decimal ConvertUsdToZar(decimal usdAmount, decimal rate);
    }
}