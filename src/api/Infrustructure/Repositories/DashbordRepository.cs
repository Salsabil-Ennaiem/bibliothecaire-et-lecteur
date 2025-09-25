using Microsoft.EntityFrameworkCore;
using LibraryManagement.Features.Dashboard.DTOs;
using Mapster;
using Data;

namespace LibraryManagement.Features.Dashboard.Repositories;

public class DashboardRepository : IDashboardRepository
{
        private readonly IDbContextFactory<BiblioDbContext> _contextFactory;


    public DashboardRepository(IDbContextFactory<BiblioDbContext> contextFactory)
    {
                _contextFactory = contextFactory;

    }
    
    public async Task<List<BookLoanCountDto>> GetTopBooksLoansAsync(int count = 10)
    {
        try
        {
            await using var context = _contextFactory.CreateDbContext();
        var query = await (from e in context.Emprunts
                           join i in context.Inventaires on e.Id_inv equals i.id_inv
                           join l in context.Livres on i.id_liv equals l.id_livre
                           group new { l.id_livre, l.titre } by new { l.id_livre, l.titre } into g
                           orderby g.Count() descending
                           select new { BookId = g.Key.id_livre, BookTitle = g.Key.titre, LoanCount = g.Count() })
                          .Take(count)
                          .ToListAsync();

        return query.Adapt<List<BookLoanCountDto>>();
        }
        catch (Exception ex)
        {
            throw new Exception("get top 1 book ", ex);
        }
    }
    public async Task<List<BookRotationRateDto>> GetBookRotationRatesAsync()
    {
        try
        {            await using var context = _contextFactory.CreateDbContext();
            var query = await (from i in context.Inventaires
                               join l in context.Livres on i.id_liv equals l.id_livre
                               let totalCopies = context.Inventaires.Count()
                               let loanCount = (from e in context.Emprunts
                                                join inv in context.Inventaires on e.Id_inv equals inv.id_inv
                                                select e).Count()
                               where totalCopies > 0
                               select new
                               {
                                   BookTitle = l.titre,
                                   RotationRate = totalCopies > 0 ? (decimal)loanCount / totalCopies : 0,
                                   Available = totalCopies - loanCount,
                                   Loaned = loanCount
                               })
                              .OrderByDescending(x => x.RotationRate)
                              .ToListAsync();

            return query.Adapt<List<BookRotationRateDto>>();
        }
        catch (Exception ex)
        {
            throw new Exception("get rotation 2 book ", ex);
        }
    }
    public async Task<List<UnusedBookDto>> GetUnusedBooksAsync(int monthsBack = 6)
    {
        try
        {
                        await using var context = _contextFactory.CreateDbContext();

            var cutoffDate = DateTime.UtcNow.AddMonths(-monthsBack);
            var query = await (from inv in context.Inventaires
                               join livre in context.Livres on inv.id_liv equals livre.id_livre
                               where !(from e in context.Emprunts
                                       join l in context.Livres on e.Id_inv equals l.id_livre
                                       where inv.id_liv == l.id_livre && e.date_emp >= cutoffDate
                                       select e).Any()
                               select new
                               {
                                   BookTitle = livre.titre,
                                   BookId = livre.id_livre,
                                   LastLoan = (from e in context.Emprunts
                                               join invent in context.Inventaires on e.Id_inv equals invent.id_inv
                                               where invent.id_liv == livre.id_livre
                                               orderby e.date_emp descending
                                               select e.date_emp).FirstOrDefault()
                               })
                              .ToListAsync();

            return query.Adapt<List<UnusedBookDto>>();
        
           }
        catch (Exception ex)
        {
            throw new Exception("get unused 3 book ", ex);
        }
    }
    public async Task<(decimal delayRate, List<UserDelayCountDto> topUsers, List<BookDelayCountDto> problematicBooks)> GetReatrdAsync(int top = 5)
    {
        try{
                        await using var context = _contextFactory.CreateDbContext();

        var totalLoans = await context.Emprunts.CountAsync();
        var delayedLoans = await context.Emprunts
            .CountAsync(e => e.date_effectif.HasValue &&
                        e.date_effectif > e.date_retour_prevu);

        var delayRate = totalLoans > 0 ? (decimal)delayedLoans / totalLoans * 100 : 0;

        // Top delayed users
        var delayedUsersQuery = await (from e in context.Emprunts
                                       join m in context.Membres on e.id_membre equals m.id_membre
                                       where e.date_effectif.HasValue &&
                                             e.date_effectif > e.date_retour_prevu
                                       group new { m.id_membre, m.nom, m.prenom } by new { m.id_membre, m.nom, m.prenom } into g
                                       orderby g.Count() descending
                                       select new
                                       {
                                           UserName = $"{g.Key.nom} {g.Key.prenom}",
                                           DelayCount = g.Count(),
                                           UserId = g.Key.id_membre
                                       })
                                      .Take(top)
                                      .ToListAsync();

        var topUsers = delayedUsersQuery.Adapt<List<UserDelayCountDto>>();

        // Problematic books
        var problematicBooksQuery = await (from e in context.Emprunts
                                           join i in context.Inventaires on e.Id_inv equals i.id_inv
                                           join l in context.Livres on i.id_liv equals l.id_livre
                                           where e.date_effectif.HasValue &&
                                                 e.date_effectif > e.date_retour_prevu
                                           group new { l.id_livre, l.titre } by new { l.id_livre, l.titre } into g
                                           orderby g.Count() descending
                                           select new
                                           {
                                               BookTitle = g.Key.titre,
                                               DelayCount = g.Count(),
                                               BookId = g.Key.id_livre
                                           })
                                          .Take(top)
                                          .ToListAsync();

        var problematicBooks = problematicBooksQuery.Adapt<List<BookDelayCountDto>>();

        return (delayRate, topUsers, problematicBooks);
        
       }
        catch (Exception ex)
        {
            throw new Exception("get retard 4 book ", ex);
        }
    }
    public async Task<(decimal sanctionRate, List<MonthlyLossDto> monthlyLosses, DelayVsLossDto delayVsLoss, decimal totalLossCost)> GetLossSancAsync(int monthbefor = 12)
    {
        try {
                                    await using var context = _contextFactory.CreateDbContext();
        var totalLoans = await context.Emprunts.CountAsync();
        var sanctionCount = await context.Sanctions.CountAsync();
        var sanctionRate = totalLoans > 0 ? (decimal)sanctionCount / totalLoans * 100 : 0;

        var monthlyLossesQuery = await (from s in context.Sanctions
                                        where s.date_sanction >= DateTime.UtcNow.AddMonths(-monthbefor)
                                        group s by new { s.date_sanction.Month, s.date_sanction.Year } into g
                                        orderby g.Key.Year, g.Key.Month
                                        select new
                                        {
                                            Month = g.Key.Month,
                                            Year = g.Key.Year,
                                            LossCost = g.Where(s => s.raison.Equals("perte")).Sum(s => s.montant),
                                            FineAmount = g.Where(s => s.raison.Equals("degat")).Sum(s => s.montant)
                                        })
                                       .ToListAsync();

        var monthlyLosses = monthlyLossesQuery.Adapt<List<MonthlyLossDto>>();

        var delayCount = await context.Emprunts
            .CountAsync(e => e.date_effectif.HasValue &&
                        e.date_effectif > e.date_retour_prevu);

        var lossCount = await context.Sanctions
            .CountAsync(s => s.raison.Equals("perte"));

        var totalLossCost = await context.Sanctions
            .SumAsync(s => (decimal?)s.montant) ?? 0;

        var delayVsLoss = new DelayVsLossDto { DelayCount = delayCount, LossCount = lossCount };

        return (sanctionRate, monthlyLosses, delayVsLoss, totalLossCost);
    
       }
        catch (Exception ex)
        {
            throw new Exception("get loss 5 book ", ex);
        }
    }
    public async Task<(List<MonthlyLoanDto> monthlyLoans, decimal avgDuration)> GetEmpMonthTopAsync(int monthbefor = 12)
    {
        try {
                                    await using var context = _contextFactory.CreateDbContext();
        var monthlyLoansQuery = await (from e in context.Emprunts
                                       where e.date_emp >= DateTime.UtcNow.AddMonths(-monthbefor)
                                       group e by new { e.date_emp.Month, e.date_emp.Year } into g
                                       orderby g.Key.Year, g.Key.Month
                                       select new
                                       {
                                           Month = g.Key.Month,
                                           Year = g.Key.Year,
                                           LoanCount = g.Count()
                                       })
                                      .ToListAsync();

        var monthlyLoans = monthlyLoansQuery.Adapt<List<MonthlyLoanDto>>();

        var avgDurationNullable = await (from e in context.Emprunts
                                         where e.date_effectif.HasValue
                                         select (double?)(e.date_effectif!.Value - e.date_emp).TotalDays)
                                        .AverageAsync();

        var avgDuration = (decimal)(avgDurationNullable ?? 0);

        return (monthlyLoans, avgDuration);
        
       }
        catch (Exception ex)
        {
            throw new Exception("get top emp month 6 book ", ex);
        }
    }
    public async Task<(decimal beforeRate, decimal afterRate, List<MonthlyPolicyComparisonDto> comparison)> GetPolicyDataAsync(int paramMonth = 6, int empmonth = 12)
    {
        
                                    await using var context = _contextFactory.CreateDbContext();
            var policyChangeDate = await context.Parametres
                .Select(p => p.date_modification)
                .OrderByDescending(o=>o.Date)
                .FirstOrDefaultAsync();

            if (policyChangeDate == default)
                policyChangeDate = DateTime.UtcNow.AddMonths(-paramMonth);

            var totalLoansBefore = await context.Emprunts
                .CountAsync(e => e.date_emp < policyChangeDate);

            var delayedLoansBefore = await context.Emprunts
                .CountAsync(e => e.date_emp < policyChangeDate &&
                            e.date_effectif.HasValue &&
                            e.date_effectif > e.date_retour_prevu);

            var totalLoansAfter = await context.Emprunts
                .CountAsync(e => e.date_emp >= policyChangeDate);

            var delayedLoansAfter = await context.Emprunts
                .CountAsync(e => e.date_emp >= policyChangeDate &&
                            e.date_effectif.HasValue &&
                            e.date_effectif > e.date_retour_prevu);

            var beforeRate = totalLoansBefore > 0 ? (decimal)delayedLoansBefore / totalLoansBefore * 100 : 0;
            var afterRate = totalLoansAfter > 0 ? (decimal)delayedLoansAfter / totalLoansAfter * 100 : 0;

           var monthlyComparisonQuery = await (from e in context.Emprunts
                                   where e.date_emp >= DateTime.UtcNow.AddMonths(-empmonth)
                                   group e by new { e.date_emp.Month, e.date_emp.Year } into g
                                   select new
                                   {
                                       Month = g.Key.Month,
                                       Year = g.Key.Year,
                                       TotalInMonth = g.Count(),
                                       DelayedInMonthBefore = g.Count(x => x.date_emp < policyChangeDate &&
                                                                          x.date_effectif.HasValue &&
                                                                          x.date_effectif > x.date_retour_prevu),
                                       DelayedInMonthAfter = g.Count(x => x.date_emp >= policyChangeDate &&
                                                                         x.date_effectif.HasValue &&
                                                                         x.date_effectif > x.date_retour_prevu),
                                   })
                                   .OrderBy(x => x.Year)
                                   .ThenBy(x => x.Month)
                                   .ToListAsync();


            var comparison = monthlyComparisonQuery.Adapt<List<MonthlyPolicyComparisonDto>>();


        return (beforeRate, afterRate, comparison);
      
    }
}
