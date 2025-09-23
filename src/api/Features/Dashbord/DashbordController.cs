using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Features.Dashboard.Services;
using LibraryManagement.Features.Dashboard.DTOs;

namespace LibraryManagement.Features.Dashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> GetDashboardData(string? biblioId)
    {
        try
        {
            var data = await _dashboardService.RefreshDashboardAndNotifyAsync(biblioId);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving dashboard data", error = ex.Message });
        }
    }

}
