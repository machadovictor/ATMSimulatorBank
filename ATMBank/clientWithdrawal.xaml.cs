/*
 * Course: Programmer Analyst LEA.9C
 * Student: Victor Hugo Motta Machado (ID: 653227967)
 * Subject: Integration Project 1 - Object-oriented programming
 * Instructor: Yves Desharnais 
 * Date: May 2023
 */
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
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;

namespace ATMBank
{
    /// <summary>
    /// Lógica interna para clientWithdrawal.xaml
    /// </summary>
    public partial class clientWithdrawal : Window
    {
        bool open = false;

        SqlConnection connection;
        CurrentUser user;
        SqlCommand command;
        Account account;
        List<Account> accounts = new List<Account>();
        internal clientWithdrawal(CurrentUser current)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            user = current;
            //Display the current user in the window's title
            Title += " - " + user.FullName;
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            FillAccountsList();
            open = true;
        }

        public void FillAccountsList()
        {

            //Create the SELECT query
            string selectAccounts = "SELECT clientaccount_id, accounttype_description FROM ClientsAccounts WHERE client_code='" + user.UserId + "'  AND accounttype_description LIKE 'Savings' OR client_code='" + user.UserId + "' AND accounttype_description LIKE 'Checking'  ORDER BY accounttype_code";
            command = new SqlCommand(selectAccounts, connection);
            try
            {

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())

                { //Create a new account
                    account = new Account();
                    account.AccountId = reader["clientaccount_id"].ToString();
                    account.AccountDescription = reader["accounttype_description"].ToString();


                    //Add to list
                    accounts.Add(account);
                }
                wdAccountsList.DataContext = accounts.ToList();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            finally
            {
                connection.Close();
            }
        }

        private void wdAccountsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //If no account is selected
            if (wdAccountsList.SelectedIndex == -1)
            {
                return;
            }

            Account aAccount = accounts[wdAccountsList.SelectedIndex];
            string selectAccount = "SELECT account_balance FROM ClientsAccounts WHERE clientaccount_id = '" + aAccount.AccountId + "'";
            command = new SqlCommand(selectAccount, connection);


            try
            {

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    wdAccountBalance.Text = reader["account_balance"].ToString().Trim();

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }

        }

        public bool ValidateInput()
        {
            bool OK = true;
            if (wdAmount.Text.Trim() == string.Empty ||
             wdAccountsList.SelectedIndex == -1)

            {
                OK = false;
            }
            return OK;
        }

        private void btnWdSave_Click(object sender, RoutedEventArgs e)
        {
            bool OK = ValidateInput();  // Check if all necessary fields are filled out
            if (OK) // If all fields are filled out
            {

                decimal amount;
                decimal balance;
                decimal bankBalance;
                string test = wdAccountBalance.Text;

                Bank b = new Bank(); // Create a new Bank object
                string checkAtmBalance = "SELECT bank_balance FROM Bank WHERE bank_code = '1'"; // SQL query to check ATM balance
                command = new SqlCommand(checkAtmBalance, connection); // Create a new SQL command object


                try
                {
                    connection.Open();// Open database connection
                    SqlDataReader reader = command.ExecuteReader();// Execute SQL query and get results

                    if (reader.Read())// If there are results
                    {
                        b.BankBalance = reader["bank_balance"].ToString();// Get ATM balance from results

                        if (decimal.TryParse(b.BankBalance, out bankBalance) && decimal.TryParse(wdAmount.Text, out amount)) // Try to parse balance and withdrawal amount as decimal
                        {

                            if (amount > bankBalance) // If withdrawal amount is greater than ATM balance
                            {
                                MessageBox.Show("Withdrawal NOT Confirmed. Funds not available in ATM. ", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                return;// Stop processing withdrawal
                            }

                            else // If withdrawal amount is less than or equal to ATM balance
                            {

                                try
                                {

                                    Account a = accounts[wdAccountsList.SelectedIndex]; // Get selected account from account list
                                    if (decimal.TryParse(test, out balance) && decimal.TryParse(wdAmount.Text, out amount)) // Try to parse account balance and withdrawal amount as decimal
                                    {
                                        // SQL query to withdraw money from selected account
                                        string withdrawalMoney = "UPDATE ClientsAccounts SET account_balance = account_balance - '" + wdAmount.Text + "'  WHERE clientaccount_id = '" + a.AccountId + "'";
                                        command = new SqlCommand(withdrawalMoney, connection);// Create a new SQL command object


                                        try
                                        {
                                            connection.Close();// Close current connection to database
                                            connection.Open();// Open new connection to database

                                            if (amount > 1000)// If withdrawal amount is greater than 1000
                                            {
                                                MessageBox.Show("Withdrawal NOT Confirmed. Maximum Amount is C$1000,00. ", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                                return;// Stop processing withdrawal
                                            }

                                            else if (amount > balance) // If withdrawal amount is greater than account balance
                                            {

                                                MessageBox.Show("Withdrawal NOT Confirmed. Funds not available in account.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                                return;// Stop processing withdrawal
                                            }

                                            else // If withdrawal amount is less than or equal to account balance
                                            {
                                                int line = command.ExecuteNonQuery();// Execute SQL query to withdraw money from selected account and get number of affected rows
                                                if (line != 0)// If there are affected rows
                                                {
                                                    //Success Message
                                                    MessageBox.Show("Withdrawal Confirmed", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    this.Close();// Close current window

                                                    //SQL query to insert the withdrawal on the transactions table
                                                    string feedHistory = $"INSERT INTO TransactionsHistory (transaction_date, client_code, accounttype_description, clientaccount_id, transactiontype_description, transaction_amount, transactiontype_code) " +
                                                    $"Values('{DateTime.Now}','{user.UserId}', '{account.AccountDescription}','{account.AccountId}', 'Withdrawal', '{wdAmount.Text}', 'TW')";
                                                    command = new SqlCommand(feedHistory, connection);


                                                    try
                                                    {
                                                        connection.Close();
                                                        connection.Open();
                                                        SqlDataReader reader3 = command.ExecuteReader();

                                                    }

                                                    catch (Exception ex)
                                                    {
                                                        MessageBox.Show(ex.Message);
                                                    }

                                                    finally
                                                    {
                                                        connection.Close();
                                                    }

                                                    //SQL query to update the balance
                                                    string alterAtmBalance = "UPDATE Bank SET bank_balance = bank_balance - '" + wdAmount.Text + "'  WHERE bank_code = '1'";
                                                    command = new SqlCommand(alterAtmBalance, connection);

                                                    try
                                                    {
                                                        connection.Open();
                                                        SqlDataReader reader3 = command.ExecuteReader();

                                                    }

                                                    catch (Exception ex)
                                                    {
                                                        MessageBox.Show(ex.Message);
                                                    }

                                                    finally
                                                    {
                                                        connection.Close();
                                                    }
                                                }
                                            }
                                        }

                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.Message);
                                        }

                                        finally
                                        {
                                            connection.Close();
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }

                                finally
                                {
                                    connection.Close();
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                finally
                {
                    connection.Close();
                }
            }

            else
            {
                //Missing information message
                MessageBox.Show("Missing information in a field.", "Warning!", MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
            }
        }

        //Method called when cancel button is clicked on the withdrawal window
        private void btnWdCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit? ", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.Close();


            }
            else
            {
                MessageBox.Show("Cancelling exit.", "Cancel",
                MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
