using Microsoft.AspNetCore.SignalR;
using LibraryManagement.Features.Dashboard.DTOs;
using LibraryManagement.Features.Dashboard.Repositories;
using Infrastructure.SignalR;

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

        await Task.WhenAll(topBooksTask, rotationRatesTask, unusedBooksTask, delayDataTask, lossDataTask, emptopmonthDataTask, policyDataTask);

        return new DashboardResponse
        {
            CatalogueOptimization = new CatalogueOptimizationDto
            {
                TopBooksLoans = await topBooksTask,
                BookRotationRates = await rotationRatesTask,
                UnusedBooks = await unusedBooksTask
            },
            DelayReduction = new DelayReductionDto
            {
                DelayRate = (await delayDataTask).delayRate,
                TopDelayedUsers = (await delayDataTask).topUsers,
                ProblematicBooks = (await delayDataTask).problematicBooks
            },
            LossAnalysis = new LossAnalysisDto
            {
                SanctionRate = (await lossDataTask).sanctionRate,
                MonthlyLosses = (await lossDataTask).monthlyLosses,
                DelayVsLoss = (await lossDataTask).delayVsLoss,
                TotalLossCost = (await lossDataTask).totalLossCost
            },
            ResourcePlanning = new ResourcePlanningDto
            {
                MonthlyLoans = (await emptopmonthDataTask).monthlyLoans,
                AverageLoanDuration = (await emptopmonthDataTask).avgDuration
            },
            PolicyEvaluation = new PolicyEvaluationDto
            {
                DelayRateBeforePolicy = (await policyDataTask).beforeRate,
                DelayRateAfterPolicy = (await policyDataTask).afterRate,
                MonthlyComparison = (await policyDataTask).comparison
            }
        };

    }

    private async Task NotifyDataChangeAsync(string? biblioId)
    {
        // Send specific change notifications
        await _hubContext.Clients.Group($"biblio_{biblioId}")
            .SendAsync("DataChanged", new { timestamp = DateTime.UtcNow });

    }
}

