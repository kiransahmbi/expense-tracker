using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment4
{
    public class Expense
    {
        public string ExpenseId { get; set; }
        public string Vendor { get; set; }
        public DateTime ExpenseDate { get; set; }
        public ExpenseCategory Category { get; set; }
        public string Name { get; set; }
        public double Cost { get; set; }
        public Expense() { }

        public Expense(string expenseId, string vendor, DateTime expenseDate, ExpenseCategory category, string name, double cost)
        {
            this.ExpenseId = expenseId;
            this.Vendor = vendor;
            this.ExpenseDate = expenseDate;
            this.Category = category;
            this.Name = name;
            this.Cost = cost;
        }

        public Expense(string expenseId, string vendor, ExpenseCategory category, string name, double cost)
        {
            this.ExpenseId = expenseId;
            this.Vendor = vendor;
            this.ExpenseDate = DateTime.Now;
            this.Category = category;
            this.Name = name;
            this.Cost = cost;
        }

        public static string GenerateExpenseId()
        {
            return Guid.NewGuid().ToString();
        }

        public override string ToString()
        {
            return $"{Vendor, -12} | {ExpenseDate.ToString("dd/MMM/y hh:mmtt"), -17} | {Category, -12} | {Name, -30} | {Cost, -8:C2}";
        }
    }
}
