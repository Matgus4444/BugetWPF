using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetPlanner.Models
{
    public class IncomeData //For saving YearlyIncome in DB
    {
        public int Id{ get; set; }
        public decimal YearlyIncome { get; set; }
    }
}
