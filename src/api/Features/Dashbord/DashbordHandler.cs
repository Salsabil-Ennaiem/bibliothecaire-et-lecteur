using Microsoft.AspNetCore.SignalR;
using LibraryManagement.Features.Dashboard.DTOs;
using LibraryManagement.Features.Dashboard.Repositories;
using Infrastructure.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibraryManagement.Features.Dashboard.Services;

public class DashboardService
{
    private readonly IDashboardRepository _repository;
    private readonly IHubContext<dashboardHub> _hubContext;

    public DashboardService(IDashboardRepository repository, IHubContext<dashboardHub> hubContext)
    {
        _repository = repository;
        _hubContext = hubContext;
    }
    public async Task<DashboardResponse> RefreshDashboardAndNotifyAsync(string? biblioId)
    {
        var dashboardData = await GetDashboardDataAsync();
        await NotifyDataChangeAsync(biblioId);
        return dashboardData;
    }

    private async Task<DashboardResponse> GetDashboardDataAsync()
    {

            var topBooksTask = _repository.GetTopBooksLoansAsync();
            var rotationRatesTask = _repository.GetBookRotationRatesAsync();
            var unusedBooksTask = _repository.GetUnusedBooksAsync();
            var delayDataTask = _repository.GetReatrdAsync();
            var lossDataTask = _repository.GetLossSancAsync();
            var emptopmonthDataTask = _repository.GetEmpMonthTopAsync();
            var policyDataTask = _repository.GetPolicyDataAsync();
            Console.WriteLine("get all repositry  ");

            await Task.WhenAll(topBooksTask, rotationRatesTask, unusedBooksTask, delayDataTask, lossDataTask, emptopmonthDataTask, policyDataTask);
            Console.WriteLine("get all deja ");

            var s= new DashboardResponse
            {
                CatalogueOptimization = new CatalogueOptimizationDto
                {
                    TopBooksLoans = topBooksTask.Result,
                    BookRotationRates = rotationRatesTask.Result,
                    UnusedBooks = unusedBooksTask.Result

                },
                DelayReduction = new DelayReductionDto
                {
                    DelayRate = delayDataTask.Result.delayRate,
                    TopDelayedUsers = delayDataTask.Result.topUsers,
                    ProblematicBooks = delayDataTask.Result.problematicBooks
                },
                LossAnalysis = new LossAnalysisDto
                {
                    SanctionRate = lossDataTask.Result.sanctionRate,
                    MonthlyLosses = lossDataTask.Result.monthlyLosses,
                    DelayVsLoss = lossDataTask.Result.delayVsLoss,
                    TotalLossCost = lossDataTask.Result.totalLossCost
                },
                ResourcePlanning = new ResourcePlanningDto
                {
                    MonthlyLoans = emptopmonthDataTask.Result.monthlyLoans,
                    AverageLoanDuration = emptopmonthDataTask.Result.avgDuration
                },
                PolicyEvaluation = new PolicyEvaluationDto
                {
                    DelayRateBeforePolicy = policyDataTask.Result.beforeRate,
                    DelayRateAfterPolicy = policyDataTask.Result.afterRate,
                    MonthlyComparison = policyDataTask.Result.comparison
                }
            };
if (s == null)
{
     throw new Exception("null") ; 
}
return s;
   
    }

    private async Task NotifyDataChangeAsync(string? biblioId)
    {
        // Send specific change notifications
        await _hubContext.Clients.Group($"biblio_{biblioId}")
            .SendAsync("DataChanged", new { timestamp = DateTime.UtcNow });

    }
}

