using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;


namespace Assignment4
{
    internal static class ExpenseManager
    {
        private static List<Expense> EXPENSES;
        private static LinkedList<string> ACTIONTRACKER;

        static ExpenseManager()
        {
            EXPENSES = new List<Expense>();
            ACTIONTRACKER = new LinkedList<string>();
        }

        public static void Add(Expense expense)
        {
            EXPENSES.Add(expense);
            ACTIONTRACKER.AddLast($"{"Added:",-10} {expense.ToString()}");
        }

        public static void Remove(string expenseId)
        {
            try
            {
                ACTIONTRACKER.AddLast($"{"Removed:",-10} {EXPENSES.Find(item => item.ExpenseId == expenseId).ToString()}");
                EXPENSES.RemoveAll(item => item.ExpenseId == expenseId);
                ExpenseManager.Create();
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }
        }
        public static void Update(string expenseToUpdateId, Expense update)
        {
            try
            {
                //Remove object to be updated. Add updated object.
                EXPENSES.RemoveAll(expense => expense.ExpenseId == expenseToUpdateId);
                EXPENSES.Add(update);
                ExpenseManager.Create();
                ACTIONTRACKER.AddLast($"{"Edited:",-10} {update.ToString()}");
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }
        }
        public static void Create()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Expense>));
            TextWriter writer = new StreamWriter("expenseStorage.xml");
            serializer.Serialize(writer, EXPENSES);
            writer.Close();
        }

        public static List<Expense> Read()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Expense>));
            TextReader reader = new StreamReader("expenseStorage.xml");
            EXPENSES = (List<Expense>)serializer.Deserialize(reader);
            reader.Close();
            return EXPENSES;
        }

        public static List<Expense> GetExpenses()
        {
            return EXPENSES;
        }

        public static string GetActionTracker()
        {
            string actionTracker = "";
            LinkedListNode<string> node = ACTIONTRACKER.First;

            while (node != null)
            {
                actionTracker += node.Value + "\n";
                node = node.Next;
            }
            return actionTracker.Substring(0, actionTracker.Length-2);
        } 

    }
}
