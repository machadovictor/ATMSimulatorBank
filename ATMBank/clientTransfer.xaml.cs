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
    /// Lógica interna para clientTransfer.xaml
    /// </summary>
    public partial class clientTransfer : Window
    {
        bool open = false;
        SqlConnection connection;
        CurrentUser user;
        SqlCommand command;

        Account account;
        List<Account> accounts = new List<Account>();

        Account account2;
        List<Account> accounts2 = new List<Account>();

        //Transfer between accounts of the same account holder
        internal clientTransfer(CurrentUser current)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            user = current;
            //Display the current user in the window's title
            Title += " - " + user.FullName;
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            FillTransferFromList();
            open = true;
        }

        public void FillTransferFromList()
        {
            //Create the SELECT query
            string selectAccountsFrom = "SELECT clientaccount_id, accounttype_description FROM ClientsAccounts WHERE client_code='" + user.UserId + "' AND accounttype_description = 'Checking' ORDER BY accounttype_code";
            command = new SqlCommand(selectAccountsFrom, connection);

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
                TransferFromAccountsList.DataContext = accounts.ToList();
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

        private void TransferFromAccountsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //If no account is selected
            if (TransferFromAccountsList.SelectedIndex == -1)
            {
                return;
            }

            Account aAccount = accounts[TransferFromAccountsList.SelectedIndex];
            string showFromBalance = "SELECT account_balance FROM CLientsAccounts WHERE clientaccount_id = '" + aAccount.AccountId + "'";
            command = new SqlCommand(showFromBalance, connection);


            try
            {

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    transferFromAccountBalance.Text = "C$" + reader["account_balance"].ToString();

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

            accounts2.Clear();


            string selectAccountsTo = "SELECT clientaccount_id, accounttype_description FROM ClientsAccounts WHERE client_code='" + user.UserId + "' AND clientaccount_id <> '" + aAccount.AccountId + "' ORDER BY accounttype_code";
            command = new SqlCommand(selectAccountsTo, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())

                { //Create a new account
                    account2 = new Account();
                    account2.AccountId = reader["clientaccount_id"].ToString();
                    account2.AccountDescription = reader["accounttype_description"].ToString();


                    //Add to list
                    accounts2.Add(account2);
                }
                TransferToAccountsList.DataContext = accounts2.ToList();
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

        private void TransferToAccountsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //If no account is selected
            if (TransferToAccountsList.SelectedIndex == -1)
            {
                return;
            }

            Account aAccount2 = accounts2[TransferToAccountsList.SelectedIndex];
            string showToBalance = "SELECT account_balance FROM CLientsAccounts WHERE clientaccount_id = '" + aAccount2.AccountId + "'";
            command = new SqlCommand(showToBalance, connection);


            try
            {

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    transferToAccountBalance.Text = "C$" + reader["account_balance"].ToString();

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
            if (TransferFromAccountsList.SelectedIndex == -1 ||
                transferAmount.Text.Trim() == string.Empty ||
             TransferToAccountsList.SelectedIndex == -1)

            {
                OK = false;
            }
            return OK;
        }

        private void btnTransferSave_Click(object sender, RoutedEventArgs e)
        {
            bool OK = ValidateInput();
            if (OK)
            {
                Account a = accounts[TransferFromAccountsList.SelectedIndex];
                string transferFromMoney = "UPDATE ClientsAccounts SET account_balance = account_balance - " + transferAmount.Text + "  WHERE clientaccount_id = '" + a.AccountId + "'";
                command = new SqlCommand(transferFromMoney, connection);

                try
                {
                    connection.Open();
                    int line = command.ExecuteNonQuery();

                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                finally
                {
                    connection.Close();
                }

                Account a2 = accounts2[TransferToAccountsList.SelectedIndex];
                string transferToMoney = "UPDATE ClientsAccounts SET account_balance = account_balance + " + transferAmount.Text + "  WHERE clientaccount_id = '" + a2.AccountId + "'";
                command = new SqlCommand(transferToMoney, connection);

                try
                {
                    connection.Open();
                    int line = command.ExecuteNonQuery();
                    if (line != 0)
                    {

                        MessageBox.Show("Transfer Confirmed", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();

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
                MessageBox.Show("Missing information in a field.", "Warning!", MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
            }



            string feedHistory = $"INSERT INTO TransactionsHistory (transaction_date, client_code, accounttype_description, clientaccount_id, transactiontype_description, clientaccount_id_transferto, transaction_amount, transactiontype_code) " +
               $"Values('{DateTime.Now}','{user.UserId}', '{account.AccountDescription}','{account.AccountId}', '{"Transfer"}', '{account2.AccountId}', '{transferAmount.Text}', 'TT')";
            command = new SqlCommand(feedHistory, connection);


            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

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

        private void btnTransferCancel_Click(object sender, RoutedEventArgs e)
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
