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
    /// Lógica interna para adminMainScreen.xaml
    /// </summary>
    public partial class adminMainScreen : Window
    {
        bool open = false;
        SqlConnection connection;
        CurrentUser user;
        SqlCommand command;
        Account account;
        List<Account> accounts = new List<Account>();
        internal adminMainScreen(CurrentUser current)
        {
            {
                // Set the window startup location and initialize it
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
                InitializeComponent();
                user = current;
                //Display the current user in the window's title
                Title += " - " + user.FullName;
                // Initialize the database connection and fill the accounts list
                connection = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
                FillAccountsList();
                // Set the open flag to true
                open = true;
            }
        }

        // Fills the accounts list with data from the database
        public void FillAccountsList()
        {

            //Create the SELECT query
            string selectAccounts = "SELECT client_code, clientaccount_id, accounttype_description FROM ClientsAccounts ORDER BY client_code";

            try
            {
                // Execute the query and read the data
                command = new SqlCommand(selectAccounts, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())

                {
                    //Create a new account and set its properties
                    account = new Account();
                    account.AccountId = reader["clientaccount_id"].ToString();
                    account.AccountDescription = reader["accounttype_description"].ToString();

                    //Add Add the account to the list
                    accounts.Add(account);
                }
                // Set the list view's data context to the accounts list
                AdminMainAccountsList.DataContext = accounts.ToList();
            }

            catch (Exception ex)
            {
                // Display an error message if something goes wrong
                MessageBox.Show(ex.Message);
            }

            finally
            {
                // Close the database connection
                connection.Close();
            }
        }

        // Refreshes the balance display for the currently selected account
        private void refreshBalance()
        {
            if (AdminMainAccountsList.SelectedIndex == -1)
            {
                return;
            }

            // Get the selected account and create a query to get its balance
            Account aAccount = accounts[AdminMainAccountsList.SelectedIndex];
            string refreshBalance = "SELECT account_balance FROM ClientsAccounts WHERE clientaccount_id = '" + aAccount.AccountId + "'";

            command = new SqlCommand(refreshBalance, connection);

            try
            {
                // Execute the query and display the balance
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    AdminMainViewBalance.Text = "C$" + reader["account_balance"].ToString();
                }

            }
            catch (Exception ex)
            {
                // Display an error message if something goes wrong
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Close the database connection
                connection.Close();
            }

        }

        // Handles the selection changed event for the accounts list view
        private void AdminMainAccountsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //If no account is selected, do nothing
            if (AdminMainAccountsList.SelectedIndex == -1)
            {
                return;
            }

            // Refresh the balance display and get the selected account
            refreshBalance();

            Account aAccount = accounts[AdminMainAccountsList.SelectedIndex];
            // Create a query to get the account's details and execute it
            string selectAccount = "SELECT * FROM ClientsAccounts FULL OUTER JOIN Clients ON ClientsAccounts.client_code = Clients.client_code WHERE clientaccount_id = '" + aAccount.AccountId + "'";
            command = new SqlCommand(selectAccount, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            try
            {
                if (reader.Read())
                {
                    // Update the form fields with the client's information
                    AdminMainFullName.Text = reader["client_fullname"].ToString();
                    AdminMainClientCode.Text = reader["client_code"].ToString();
                    AdminMainTelephone.Text = reader["client_phone"].ToString();
                    AdminMainEmail.Text = reader["client_email"].ToString();

                    // Update the client's status radio button
                    switch (reader["client_situation"].ToString())
                    {
                        case "B":
                            rbBlocked.IsChecked = true;
                            break;

                        case "U":
                            rbUnblocked.IsChecked = true;
                            break;
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

        private void btnAdminAddClient_Click(object sender, RoutedEventArgs e)
        {
            {
                //Create new form instance
                frmNewClient addClient = new frmNewClient();
                //Display the form
                addClient.ShowDialog();
                //Refresh accounts list
                accounts.Clear();
                FillAccountsList();

            }
        }

        private void btnAdminAddAccount_Click(object sender, RoutedEventArgs e)
        {
            //Create new form instance
            frmNewAccount addAccount = new frmNewAccount();
            //Display the form
            addAccount.ShowDialog();
            //Refresh account list
            accounts.Clear();
            FillAccountsList();
        }

        private void btnMainExit_Click_1(object sender, RoutedEventArgs e)
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

        public void blockCLient()
        {
            // SQL query to block the selected client
            string blockClient = "UPDATE Clients SET client_situation = 'B' WHERE client_code = '" + AdminMainClientCode.Text.Trim() + "'";

            command = new SqlCommand(blockClient, connection);
            connection.Open();
            try
            {
                // Confirm the action with the user before executing the query
                if (MessageBox.Show("Are you sure you want to block client access? ", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    int line = command.ExecuteNonQuery();
                    MessageBox.Show("Client is now blocked.", "Confirmation",
                       MessageBoxButton.OK, MessageBoxImage.Information);
                }

                else
                {
                    // If the user cancels the action, update the radio button selection
                    rbUnblocked.IsChecked = true;
                    MessageBox.Show("Action canceled.", "Cancel",
                        MessageBoxButton.OK, MessageBoxImage.Information);
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

        public void unblockCLient()
        {
            // SQL query to unblock the selected client
            string unblockClient = "UPDATE Clients SET client_situation = 'U' WHERE client_code = '" + AdminMainClientCode.Text.Trim() + "'";

            command = new SqlCommand(unblockClient, connection);
            connection.Open();
            try
            {

                if (MessageBox.Show("Are you sure you want to unblock client access? ", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    int line = command.ExecuteNonQuery();
                    MessageBox.Show("Client is now unblocked.", "Confirmation",
                       MessageBoxButton.OK, MessageBoxImage.Information);
                }

                else
                {
                    rbBlocked.IsChecked = true;
                    MessageBox.Show("Action canceled.", "Cancel",
                        MessageBoxButton.OK, MessageBoxImage.Information);
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

        // This method is called when the "Unblocked" radio button is clicked
        private void rbUnblocked_Click(object sender, RoutedEventArgs e)
        {
            unblockCLient();
        }

        // This method is called when the "Blocked" radio button is clicked
        private void rbBlocked_Click(object sender, RoutedEventArgs e)
        {
            blockCLient();
        }

        // This method is called when the "Increase Limit" button is clicked
        private void btnAdminMainIncreaseLimit_Click(object sender, RoutedEventArgs e)
        {
            // SQL query to increase the line of credit for all clients with account type "AL"
            string increaseLineOfCredit = "UPDATE ClientsAccounts SET account_balance = account_balance + (account_balance*0.05) WHERE accounttype_code = 'AL'";

            command = new SqlCommand(increaseLineOfCredit, connection);
            connection.Open();

            try
            {
                // Ask for confirmation before executing the query
                if (MessageBox.Show("Increase 5% to all line of credit accounts? ", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    int line = command.ExecuteNonQuery();
                    MessageBox.Show("Increased successfully.", "Confirmation",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Transaction canceled.", "Cancel",
                       MessageBoxButton.OK, MessageBoxImage.Information);
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

            // Refresh the balance after the operation is complete
            refreshBalance();

        }

        // This method is called when the "Add Interest" button is clicked
        private void btnAdminMainAddInterest_Click(object sender, RoutedEventArgs e)
        {
            // SQL query to add 1% interest to all savings accounts
            string addInterest = "UPDATE ClientsAccounts SET account_balance = account_balance + (account_balance*0.01) WHERE accounttype_code = 'AS'";

            command = new SqlCommand(addInterest, connection);
            connection.Open();

            try
            {
                // Ask for confirmation before executing the query
                if (MessageBox.Show("Add 1% interest to all savings accounts? ", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    int line = command.ExecuteNonQuery();
                    MessageBox.Show("Interests added successfully.", "Confirmation",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Transaction canceled.", "Cancel",
                       MessageBoxButton.OK, MessageBoxImage.Information);
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
            // Refresh the balance after the operation is complete
            refreshBalance();

        }

        // This method is called when the "Transactions" button is clicked
        private void btnAdminMainTransactions_Click(object sender, RoutedEventArgs e)
        {
            //Create new form instance
            frmTransactions transactionsHistory = new frmTransactions();
            //Display the form
            transactionsHistory.ShowDialog();
        }

        // This method is called when the "Add Money" button is clicked
        private void btnAdminMainAddMOney_Click(object sender, RoutedEventArgs e)
        {

            // Create a new instance of the "DepositPaperMoney" form and display it
            DepositPaperMoney fillAtm = new DepositPaperMoney(user);
            //Display the form
            fillAtm.ShowDialog();
        }

        // This method is called when the "Close ATM" button is clicked
        private void btnMainCloseATM_Click(object sender, RoutedEventArgs e)
        {
            // SQL query to close the ATM
            string closeATM = "UPDATE Bank SET bank_status = 'C'";

            command = new SqlCommand(closeATM, connection);
            connection.Open();

            try
            {
                // Ask for confirmation before executing the query
                if (MessageBox.Show("Are you sure you want to CLOSE THE ATM? ", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    int line = command.ExecuteNonQuery();
                    MessageBox.Show("ATM IS NOW CLOSED.", "Confirmation",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Action canceled.", "Cancel",
                       MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void btnAdminMainMortgageW_Click(object sender, RoutedEventArgs e)
        {
            //Create new form instance
            adminWithdraw MortgageWithdrawl = new adminWithdraw();
            //Display the form
            MortgageWithdrawl.ShowDialog();
            //Refresh main window balance
            refreshBalance();
        }
    }
}

