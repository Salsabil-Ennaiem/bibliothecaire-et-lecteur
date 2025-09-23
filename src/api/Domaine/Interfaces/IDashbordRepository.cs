using LibraryManagement.Features.Dashboard.DTOs;

namespace LibraryManagement.Features.Dashboard.Repositories;

public interface IDashboardRepository
{
    Task<List<BookLoanCountDto>> GetTopBooksLoansAsync( int count = 10);
    Task<List<BookRotationRateDto>> GetBookRotationRatesAsync();
    Task<List<UnusedBookDto>> GetUnusedBooksAsync( int monthsBack = 6);
    Task<(decimal delayRate, List<UserDelayCountDto> topUsers, List<BookDelayCountDto> problematicBooks)> GetReatrdAsync(int top=5);
    Task<(decimal sanctionRate, List<MonthlyLossDto> monthlyLosses, DelayVsLossDto delayVsLoss, decimal totalLossCost)> GetLossSancAsync(int monthbefor=12);
    Task<(List<MonthlyLoanDto> monthlyLoans, decimal avgDuration)> GetEmpMonthTopAsync(int monthbefor=12);
    Task<(decimal beforeRate, decimal afterRate, List<MonthlyPolicyComparisonDto> comparison)> GetPolicyDataAsync(int paramMonth=6 ,int empmonth=12);
}
