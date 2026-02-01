using BudgetPlanner.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;

public class BudgetDbContext : DbContext
{
    public DbSet<BudgetItem> BudgetItems { get; set; }
    public DbSet<IncomeData> IncomeDatas{ get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var dbPath = Path.Combine(
    AppDomain.CurrentDomain.BaseDirectory,
    "budget.db");

        options.UseSqlite($"Data Source={dbPath}");
    }
}
