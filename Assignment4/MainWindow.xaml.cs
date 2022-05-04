using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace Assignment4
{
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            InitializeUI();
        }
        public void InitializeUI()
        {
            //Button Responses
            btnReset.Click += OnResetExpenseForm;
            btnExpense.Click += OnCreateExpense;
            btnRemove.Click += OnRemoveExpense;
            btnEdit.Click += OnEditExpense;
            btnResetDataView.Click += OnFilterReset;
            btnSubmitFilter.Click += OnFilterSubmit;
            ExpenseDataView.AutoGeneratingColumn += AutoGeneratingColumn;

            //Sample Data
            SampleFormData();
            SampleDataViewData();
            SetDataView(ExpenseManager.Read());
            UpdateStatusBar();
        }
        private void OnResetExpenseForm(object sender, EventArgs e)
        {
            txtExpenseId.Text = Expense.GenerateExpenseId();
            comboCategory.SelectedItem = null;
            txtVendor.Text = null;
            txtName.Text = null;
            expenseDate.SelectedDate = null;
            txtCost.Text = null;
        }

        private void OnCreateExpense(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtVendor.Text) ||
                comboCategory.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtCost.Text) ||
                expenseDate.SelectedDate == null)
            {
                MessageBox.Show("Please fill all fields to add a new expense.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Expense newExpense = new Expense();
                newExpense.ExpenseId = txtExpenseId.Text;
                newExpense.Vendor = txtVendor.Text;
                newExpense.Name = txtName.Text;
                newExpense.Cost = Convert.ToDouble(txtCost.Text);

                if (expenseDate.SelectedDate != null)
                {
                    newExpense.ExpenseDate = (DateTime)expenseDate.SelectedDate;
                }

                newExpense.Category = (ExpenseCategory)Enum.Parse(typeof(ExpenseCategory), comboCategory.SelectedValue.ToString());

                try
                {
                    if ((string)btnExpense.Content == "Edit Expense")
                    {
                        ExpenseManager.Update(newExpense.ExpenseId, newExpense);
                        btnExpense.Content = "Create Expense";
                        MessageBox.Show("Expense Edited!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        ExpenseManager.Add(newExpense);
                        MessageBox.Show("Expense added!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    this.OnResetExpenseForm(sender, e);
                    UpdateStatusBar();
                    SetDataView(ExpenseManager.GetExpenses());

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void OnRemoveExpense(object sender, EventArgs e)
        {
            if (ExpenseDataView.SelectedItem != null) { 
                foreach (var item in ExpenseDataView.SelectedItems)
                {
                    ExpenseManager.Remove(((Expense)item).ExpenseId);
                }
                UpdateStatusBar();
                SetDataView(ExpenseManager.GetExpenses());
            }
            else
            {
                MessageBox.Show("Select item to edit.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public void OnEditExpense(object sender, EventArgs e)
        {
            if (ExpenseDataView.SelectedItem != null) 
            {
                btnExpense.Content = "Edit Expense";
                Expense item = (Expense)ExpenseDataView.SelectedItem;
                txtExpenseId.Text = item.ExpenseId;
                comboCategory.SelectedValue = item.Category;
                txtVendor.Text = item.Vendor;
                txtName.Text = item.Name;
                expenseDate.SelectedDate = item.ExpenseDate;
                txtCost.Text = Convert.ToString(item.Cost);
            }
            else
            {
                MessageBox.Show("Select item to edit.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SampleFormData()
        {
            txtExpenseId.Text = Expense.GenerateExpenseId();
            comboCategory.SelectedValue = "Restaurant";
            txtVendor.Text = "Lavelle";
            txtName.Text = "Girl's Night Out";
            expenseDate.SelectedDate = new DateTime(2022, 3, 31, 13, 10, 20);
            txtCost.Text = "65.70";
        }

        public void SampleDataViewData()
        {
            ExpenseManager.Add(new Expense(Expense.GenerateExpenseId(), "Amazon", new DateTime(2022, 4, 17, 13, 10, 20), ExpenseCategory.Personal, "Monthly Skin Care Products", 60));
            ExpenseManager.Add(new Expense(Expense.GenerateExpenseId(), "Netflix", new DateTime(2022, 4, 16, 13, 10, 20), ExpenseCategory.Subscription, "Monthly Netflix Subscription", 19.99));
            ExpenseManager.Add(new Expense(Expense.GenerateExpenseId(), "Jack Astors", new DateTime(2022, 4, 15, 13, 10, 20), ExpenseCategory.Restaurant, "Date Night", 50.25));
            ExpenseManager.Add(new Expense(Expense.GenerateExpenseId(), "Hydro One", new DateTime(2022, 4, 15, 13, 10, 20), ExpenseCategory.Utilities, "Water Bill", 125.30));
            ExpenseManager.Add(new Expense(Expense.GenerateExpenseId(), "Amazon", new DateTime(2022, 4, 16, 13, 10, 20), ExpenseCategory.Subscription, "Cat Food", 40));
            ExpenseManager.Add(new Expense(Expense.GenerateExpenseId(), "Shein", new DateTime(2022, 4, 17, 13, 10, 20), ExpenseCategory.Personal, "Clothing Shopping", 120));
            ExpenseManager.Add(new Expense(Expense.GenerateExpenseId(), "Sephora", new DateTime(2022, 4, 17, 13, 10, 20), ExpenseCategory.Personal, "Make-up", 120));
            ExpenseManager.Create();
        }

        private void OnGenerateExpenseColumns(object sender, EventArgs e)
        {
            Dictionary<string, string> columnNames = new Dictionary<string, string>() {
                {"ExpenseDate", "Expense Date"},
                {"Min_Cost", "Min Cost"},
                {"Max_Cost", "Max Cost"},
                {"Total_Cost", "Total Cost"},
                {"Average_Cost", "Average Cost"},
            };
            foreach (var column in ((DataGrid)sender).Columns)
            {
                column.MinWidth = 87;

                if (column.Header.ToString() == "ExpenseId")
                {
                    column.Visibility = Visibility.Hidden;
                }
                foreach (var item in columnNames)
                {
                    if (column.Header.ToString() == item.Key)
                    {
                        column.Header = item.Value;
                    }
                }
            }
        }
        private void AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == Type.GetType("System.DateTime"))
            {
                ((DataGridTextColumn)e.Column).Binding = new Binding(e.PropertyName) { StringFormat = "dd/MMM/y hh:mmtt" };
            }
            if (e.PropertyName == "Cost")
            {
                ((DataGridTextColumn)e.Column).Binding = new Binding(e.PropertyName) { StringFormat = "C2" };
            }
        }

        private void UpdateStatusBar()
        {
            txtStatus.Text = null;
            txtStatus.Text = ExpenseManager.GetActionTracker();
            txtStatus.ScrollToEnd();
        }

        //Change DataView Buttons based on Focused Tab
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((string)TabControl.SelectedValue == "Create Expense")
            {
                btnRemove.Visibility = Visibility.Visible;
                btnEdit.Visibility = Visibility.Visible;
                btnResetDataView.Visibility = Visibility.Collapsed;
                btnSubmitFilter.Visibility = Visibility.Collapsed;

            } else
            {
                OnFilterReset(sender, e);
                btnResetDataView.Visibility = Visibility.Visible;
                btnSubmitFilter.Visibility = Visibility.Visible;
                btnRemove.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
            }
        }

        //FILTER FUNCTIONS

        public void OnFilterReset(object sender, EventArgs e)
        {
            checkHousing.IsChecked = true;
            checkUtilities.IsChecked = true;
            checkGroceries.IsChecked = true;
            checkTransporation.IsChecked = true;
            checkSubscription.IsChecked = true;
            checkHealthCare.IsChecked = true;
            checkRestaurant.IsChecked = true;
            checkEntertainment.IsChecked = true;
            checkPersonal.IsChecked = true;
            checkMiscellaneous.IsChecked = true;

            startDate.SelectedDate = DateTime.Now.AddDays(-30);
            endDate.SelectedDate = DateTime.Now;

            radioByNone.IsChecked = true;

            OnFilterSubmit(sender, e);
        }

        public void OnFilterSubmit(object sender, EventArgs e)
        {
            var query = ExpenseManager.GetExpenses();

            Dictionary<CheckBox, ExpenseCategory> listOfCheckBoxes = new Dictionary<CheckBox, ExpenseCategory>() {
                {checkHousing, ExpenseCategory.Housing },
                { checkUtilities, ExpenseCategory.Utilities },
                { checkGroceries, ExpenseCategory.Groceries },
                { checkTransporation, ExpenseCategory.Transporation },
                { checkSubscription, ExpenseCategory.Subscription },
                { checkHealthCare, ExpenseCategory.HealthCare },
                { checkRestaurant, ExpenseCategory.Restaurant },
                { checkEntertainment, ExpenseCategory.Entertainment },
                { checkPersonal, ExpenseCategory.Personal },
                { checkMiscellaneous, ExpenseCategory.Miscellaneous }
            };

            var listOfChecked = listOfCheckBoxes.Keys.ToList().Where(item => item.IsChecked == true);
            List<ExpenseCategory> listOfKeys = new List<ExpenseCategory>();

            foreach (var item in listOfChecked) { listOfKeys.Add(listOfCheckBoxes[item]); }

            //Query Select Categories
            query = (List<Expense>)query.Where(item => listOfKeys.Contains(item.Category)).ToList();
            
            //Query Between Start & End Dates
            if (startDate.SelectedDate != null & endDate.SelectedDate != null)
            {
                if (endDate.SelectedDate < startDate.SelectedDate)
                {
                    MessageBox.Show("End date must be after start date.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                query = (List<Expense>)query.Where(item => item.ExpenseDate > (DateTime)startDate.SelectedDate & item.ExpenseDate < (DateTime)endDate.SelectedDate).ToList();
            }

            //GROUP BY QUERIES
            //Group By Category
            if ((bool)radioByCategory.IsChecked)
            {
                //Group By Sum
                if ((bool)radioBySum.IsChecked)
                {
                    var groupByCategorySumQuery = query.GroupBy(item => item.Category).Select(gitem => new { Category = gitem.Key, Total_Cost = Math.Round(gitem.Sum(x => x.Cost), 2) }).ToList();
                    SetDataView(groupByCategorySumQuery);
                } 
                //Group By Average
                else if ((bool)radioByAverage.IsChecked)
                {
                    var groupByCategoryAverageQuery = query.GroupBy(item => item.Category).Select(gitem => new { Category = gitem.Key, Average_Cost = Math.Round(gitem.Average(x => x.Cost), 2) }).ToList();
                    SetDataView(groupByCategoryAverageQuery);
                } 
                //Group By Min/Max
                else if ((bool)radioByMinMax.IsChecked)
                {
                    var groupByCategoryMinMaxQuery = query.GroupBy(item => item.Category).Select(gitem => new { Category = gitem.Key, Min_Cost = gitem.Min(x => x.Cost), Max_Cost = gitem.Max(x => x.Cost) }).ToList();
                    SetDataView(groupByCategoryMinMaxQuery);
                }
            }
            //Group By Vendor
            else if ((bool)radioByVendor.IsChecked)
            {
                //Group By Sum
                if ((bool)radioBySum.IsChecked)
                {
                    var groupByVendorSumQuery = query.GroupBy(item => item.Vendor).Select(gitem => new { Vendor = gitem.Key, Total_Cost = Math.Round(gitem.Sum(x => x.Cost), 2) }).ToList();
                    SetDataView(groupByVendorSumQuery);
                }
                //Group By Average
                else if ((bool)radioByAverage.IsChecked)
                {
                    var groupByVendorAverageQuery = query.GroupBy(item => item.Vendor).Select(gitem => new { Vendor = gitem.Key, Average_Cost = Math.Round(gitem.Average(x => x.Cost), 2) }).ToList();
                    SetDataView(groupByVendorAverageQuery);
                }
                //Group By Min/Max
                else if ((bool)radioByMinMax.IsChecked)
                {
                    var groupByVendorMinMaxQuery = query.GroupBy(item => item.Vendor).Select(gitem => new { Vendor = gitem.Key, Min_Cost = gitem.Min(x => x.Cost), Max_Cost = gitem.Max(x => x.Cost) }).ToList();
                    SetDataView(groupByVendorMinMaxQuery);
                }
            }
            else
            {
                SetDataView(query);
            }

        }

        public void SetDataView<T>(List<T> dataTable)
        {
            ExpenseDataView.ItemsSource = null;
            ExpenseDataView.ItemsSource = dataTable;
        }
    }
}
