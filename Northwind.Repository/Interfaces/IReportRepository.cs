namespace Northwind.Repository.Interfaces
{
    public interface IReportRepository
    {
        Task<List<Tuple<string, int, decimal>>> GetSalesByMonthAsync(int year);
        Task<List<Tuple<string, int, decimal>>> GetSalesByProductAsync();
        Task<List<Tuple<string, int, decimal>>> GetSalesByCategoryAsync();
        Task<List<Tuple<string, int, decimal>>> GetSalesByCustomerAsync();
        Task<List<Tuple<string, int, decimal>>> GetSalesByEmployeeAsync();
    }
}