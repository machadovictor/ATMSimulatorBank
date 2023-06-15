/*
 * Course: Programmer Analyst LEA.9C
 * Student: Victor Hugo Motta Machado (ID: 653227967)
 * Subject: Integration Project 1 - Object-oriented programming
 * Instructor: Yves Desharnais 
 * Date: May 2023
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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

namespace ATMBank
{
    /// <summary>
    /// Lógica interna para adminWithdraw.xaml
    /// </summary>
    public partial class adminWithdraw : Window
    {
        bool open = false;

        SqlConnection connection;
        SqlCommand command;

        Account account;
        List<Account> accounts = new List<Account>();

        public adminWithdraw()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            //Display the current user in the window's title
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            FillAccountsList();
            open = true;
        }

        public void FillAccountsList()
        {
            Account account = new Account();
            //Create the SELECT query
            string selectAccounts = "SELECT clientaccount_id, client_code FROM ClientsAccounts WHERE accounttype_description LIKE 'Mortgage' ORDER BY accounttype_code";
            command = new SqlCommand(selectAccounts, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())

                { //Create a new account
                    account = new Account();
                    account.ClientId = reader["client_code"].ToString();
                    account.AccountId = reader["clientaccount_id"].ToString();


                    //Add to list
                    accounts.Add(account);
                }
                adminWdAccountsList.DataContext = accounts.ToList();
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

        private void adminWdAccountsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //If no account is selected
            if (adminWdAccountsList.SelectedIndex == -1)
            {
                return;
            }

            Account aAccount = accounts[adminWdAccountsList.SelectedIndex];
            string selectAccount = "SELECT account_balance FROM ClientsAccounts WHERE clientaccount_id = '" + aAccount.AccountId + "'";
            command = new SqlCommand(selectAccount, connection);


            try
            {

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    AdminWdAccountBalance.Text = "C$" + reader["account_balance"].ToString();

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
            if (AdminWdAmount.Text.Trim() == string.Empty ||
             adminWdAccountsList.SelectedIndex == -1)

            {
                OK = false;
            }
            return OK;
        }

        private void btnAdminWdSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate user input
            bool OK = ValidateInput();
            if (OK)
            {
                // Get the selected account
                Account a = accounts[adminWdAccountsList.SelectedIndex];
                // Create SQL statement for updating the account balance
                string MortgageWithdrawal = "UPDATE ClientsAccounts SET account_balance = account_balance - " + AdminWdAmount.Text + "  WHERE clientaccount_id = '" + a.AccountId + "'";
                command = new SqlCommand(MortgageWithdrawal, connection);

                try
                {
                    // Execute the SQL command to update the account balance
                    connection.Open();
                    int line = command.ExecuteNonQuery();
                    if (line != 0)
                    {
                        // Show confirmation message and close the window
                        MessageBox.Show("Mortgage Withdrawal Confirmed", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();

                    }
                }

                catch (Exception ex)
                {
                    // Show error message if an exception is caught
                    MessageBox.Show(ex.Message);
                }

                finally
                {
                    // Close the database connection
                    connection.Close();
                }
            }

            else
            {
                // Show a warning message if user input is invalid
                MessageBox.Show("Missing information in a field.", "Warning!", MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
            }
            // Get the selected account
            Account account = accounts[adminWdAccountsList.SelectedIndex];
            // Create SQL statement for inserting a new transaction record in the TransactionsHistory table
            string feedHistory = $"INSERT INTO TransactionsHistory (transaction_date, client_code, accounttype_description, clientaccount_id, transactiontype_description,transactiontype_code, clientaccount_id_transferto, transaction_amount) " +
             $"Values('{DateTime.Now}','{account.ClientId}', '{account.AccountDescription}','{account.AccountId}', '{"Withdrawal"}','{"TW"}', '{"By Admin"}', '{AdminWdAmount.Text}')";
            command = new SqlCommand(feedHistory, connection);


            try
            {
                // Execute the SQL command to insert the new transaction record
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

            }

            catch (Exception ex)
            {
                // Show error message if an exception is caught
                MessageBox.Show(ex.Message);
            }

            finally
            {
                // Close the database connection
                connection.Close();
            }

        }

        // This method is called when the "Cancel" button is clicked in the Admin Withdrawal window
        private void btnAdminWdCancel_Click(object sender, RoutedEventArgs e)
        {
            // Displays a confirmation message box to ask the user if they are sure they want to exit
            if (MessageBox.Show("Are you sure you want to exit? ", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // If the user clicks "Yes", closes the Admin Withdrawal window
                this.Close();
            }
            else
            {
                // If the user clicks "No", displays a message box to inform them that the exit has been cancelled
                MessageBox.Show("Cancelling exit.", "Cancel",
                MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
