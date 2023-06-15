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
    /// Lógica interna para clientPayBill.xaml
    /// </summary>
    public partial class clientPayBill : Window
    {
        bool open = false;

        SqlConnection connection;
        CurrentUser user;
        SqlCommand command;
        Account account;
        List<Account> accounts = new List<Account>();
        internal clientPayBill(CurrentUser current)
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
            string selectAccounts = "SELECT clientaccount_id, accounttype_description FROM ClientsAccounts WHERE client_code='" + user.UserId + "'  AND accounttype_description = 'Checking' ORDER BY accounttype_code";
            try
            {
                command = new SqlCommand(selectAccounts, connection);
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
                paybillAccountsList.DataContext = accounts.ToList();
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

        private void paybillAccountsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //If no account is selected
            if (paybillAccountsList.SelectedIndex == -1)
            {
                return;
            }

            Account aAccount = accounts[paybillAccountsList.SelectedIndex];
            string selectAccount = "SELECT account_balance FROM CLientsAccounts WHERE clientaccount_id = '" + aAccount.AccountId + "'";
            command = new SqlCommand(selectAccount, connection);


            try
            {

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    paybillAccountBalance.Text = "C$" + reader["account_balance"].ToString();

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

        //Validate input
        public bool ValidateInput()
        {
            bool OK = true;
            if (paybillTo.Text.Trim() == string.Empty ||
                paybillAmount.Text.Trim() == string.Empty ||
             paybillAccountsList.SelectedIndex == -1)

            {
                OK = false;
            }
            return OK;
        }

        //Method called when save button is clicked on the bill payment window
        private void btnBillSave_Click(object sender, RoutedEventArgs e)
        {
            bool OK = ValidateInput();
            if (OK)
            {
                //SQL query that update the balance when a bill is paid. Balance - (Bill value + 1.25)
                Account a = accounts[paybillAccountsList.SelectedIndex];
                string payBill = "UPDATE ClientsAccounts SET account_balance = (account_balance - " + paybillAmount.Text + ") - 1.25 WHERE clientaccount_id = '" + a.AccountId + "'";
                command = new SqlCommand(payBill, connection);

                try
                {
                    connection.Open();
                    int line = command.ExecuteNonQuery();
                    if (line != 0)
                    {

                        MessageBox.Show("Bill paid successfully.", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
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



            Client c = new Client();
            //SQL query to insert the bill payment transaction on transactions table
            string feedHistory = $"INSERT INTO TransactionsHistory (transaction_date, client_code, accounttype_description, clientaccount_id, transactiontype_description,transactiontype_code, clientaccount_id_transferto, transaction_amount) " +
               $"Values('{DateTime.Now}', '{user.UserId}', '{account.AccountDescription}','{account.AccountId}', 'Bill','TB', '{paybillTo.Text}','{paybillAmount.Text}')";
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

            //SQL query to insert the bill payment fee on transactions table
            string chargefee = $"INSERT INTO TransactionsHistory (transaction_date, client_code, accounttype_description, clientaccount_id, transactiontype_description, transaction_amount, transactiontype_code) " +
              $"Values('{DateTime.Now}', '{user.UserId}', '{account.AccountDescription}','{account.AccountId}', 'Fee', '1.25', 'TB')";
            command = new SqlCommand(chargefee, connection);


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
        
        //Method called when cancel button is clicked on the bill payment window
        private void btnBillCancel_Click(object sender, RoutedEventArgs e)
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
