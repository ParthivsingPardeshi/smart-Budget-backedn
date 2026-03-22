using FinanceManagerAPI.Data;
using FinanceManagerAPI.DTOs;
using FinanceManagerAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FinanceManagerAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public ReportsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Monthly expense totals for charting.
    /// </summary>
    [HttpGet("monthly-expenses")]
    public async Task<IActionResult> GetMonthlyExpenses([FromQuery] int months = 6)
    {
        var userId = User.GetUserId();
        if (months is < 1 or > 36)
        {
            return BadRequest(new { message = "months must be between 1 and 36." });
        }

        var startDate = DateTime.UtcNow.AddMonths(-months + 1);
        var firstDay = new DateTime(startDate.Year, startDate.Month, 1);

        var results = new List<MonthlyExpenseReportDto>();

        var connection = (NpgsqlConnection)_dbContext.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        await using (var command = connection.CreateCommand())
        {
            command.CommandText =
                """
                SELECT
                    EXTRACT(YEAR FROM date_trunc('month', "Date"))::int AS "Year",
                    EXTRACT(MONTH FROM date_trunc('month', "Date"))::int AS "Month",
                    COALESCE(SUM("Amount"), 0)::numeric AS "TotalAmount"
                FROM "Expenses"
                WHERE "UserId" = @userId AND "Date" >= @firstDay
                GROUP BY 1, 2
                ORDER BY 1, 2;
                """;

            command.Parameters.AddWithValue("userId", userId);
            command.Parameters.AddWithValue("firstDay", firstDay);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new MonthlyExpenseReportDto
                {
                    Year = reader.GetInt32(0),
                    Month = reader.GetInt32(1),
                    TotalAmount = reader.GetDecimal(2)
                });
            }
        }

        return Ok(results);
    }

    /// <summary>
    /// Expense distribution by category.
    /// </summary>
    [HttpGet("category-expenses")]
    public async Task<IActionResult> GetCategoryExpenses([FromQuery] int? month, [FromQuery] int? year)
    {
        var userId = User.GetUserId();
        var query = _dbContext.Expenses.Where(e => e.UserId == userId);

        if (month.HasValue)
        {
            query = query.Where(e => e.Date.Month == month.Value);
        }

        if (year.HasValue)
        {
            query = query.Where(e => e.Date.Year == year.Value);
        }

        var data = await query
            .GroupBy(e => e.Category)
            .Select(g => new CategoryExpenseReportDto
            {
                Category = g.Key.ToString(),
                TotalAmount = g.Sum(x => x.Amount)
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToListAsync();

        return Ok(data);
    }

    /// <summary>
    /// Savings trend: income minus expenses by month.
    /// </summary>
    [HttpGet("savings-trend")]
    public async Task<IActionResult> GetSavingsTrend([FromQuery] int months = 6)
    {
        var userId = User.GetUserId();
        if (months is < 1 or > 36)
        {
            return BadRequest(new { message = "months must be between 1 and 36." });
        }

        var startDate = DateTime.UtcNow.AddMonths(-months + 1);
        var firstDay = new DateTime(startDate.Year, startDate.Month, 1);

        var results = new List<SavingsTrendReportDto>();

        var connection = (NpgsqlConnection)_dbContext.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        await using (var command = connection.CreateCommand())
        {
            command.CommandText =
                """
                WITH income AS (
                    SELECT date_trunc('month', "Date") AS period, SUM("Amount") AS total_income
                    FROM "Incomes"
                    WHERE "UserId" = @userId AND "Date" >= @firstDay
                    GROUP BY 1
                ),
                expense AS (
                    SELECT date_trunc('month', "Date") AS period, SUM("Amount") AS total_expense
                    FROM "Expenses"
                    WHERE "UserId" = @userId AND "Date" >= @firstDay
                    GROUP BY 1
                )
                SELECT
                    EXTRACT(YEAR FROM COALESCE(income.period, expense.period))::int AS "Year",
                    EXTRACT(MONTH FROM COALESCE(income.period, expense.period))::int AS "Month",
                    COALESCE(income.total_income, 0)::numeric AS "TotalIncome",
                    COALESCE(expense.total_expense, 0)::numeric AS "TotalExpense"
                FROM income
                FULL OUTER JOIN expense ON income.period = expense.period
                ORDER BY 1, 2;
                """;

            command.Parameters.AddWithValue("userId", userId);
            command.Parameters.AddWithValue("firstDay", firstDay);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var totalIncome = reader.GetDecimal(2);
                var totalExpense = reader.GetDecimal(3);

                results.Add(new SavingsTrendReportDto
                {
                    Year = reader.GetInt32(0),
                    Month = reader.GetInt32(1),
                    TotalIncome = totalIncome,
                    TotalExpense = totalExpense,
                    Savings = totalIncome - totalExpense
                });
            }
        }

        return Ok(results);
    }
}
