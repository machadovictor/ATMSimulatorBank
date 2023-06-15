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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;

namespace ATMBank
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        bool open = false;
        SqlConnection connection;
        CurrentUser user;
        SqlCommand command;
        Account account;
        List<Account> accounts = new List<Account>();
        internal MainWindow(CurrentUser current)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            user = current;
            //Display the current user in the window's title
            Title += " - " + user.FullName;
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            FillClientInfo();
            FillAccountsList();
            btnMainDeposit.IsEnabled = false;
            btnMainWithdrawal.IsEnabled = false;
            btnMainTransfer.IsEnabled = false;
            btnMainPayBill.IsEnabled = false;
            open = true;
        }

        public void FillClientInfo()
        {
            string selectAccount = "SELECT * FROM Clients WHERE client_code = '" + user.UserId + "'";
            command = new SqlCommand(selectAccount, connection);


            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    mainFullName.Text = reader["client_fullname"].ToString();
                    mainClientCode.Text = reader["client_code"].ToString();
                    mainTelephone.Text = reader["client_phone"].ToString();
                    mainEmail.Text = reader["client_email"].ToString();
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

        public void FillAccountsList()
        {

            //Create the SELECT query
            string selectAccounts = "SELECT clientaccount_id, accounttype_description FROM ClientsAccounts WHERE client_code='" + user.UserId + "' ORDER BY accounttype_code";
            try
            {
                command = new SqlCommand(selectAccounts, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())

                { //Create a new account
                    account = new Account();
                    account.AccountId = reader["clientaccount_id"].ToString();
                    account.AccountDescription = reader["accounttype_description"].ToString().Trim();


                    //Add to list
                    accounts.Add(account);
                }
                AccountsList.DataContext = accounts.ToList();
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

        private void refreshBalance()
        {

            if (AccountsList.SelectedIndex == -1)
            {
                return;
            }


            Account aAccount = accounts[AccountsList.SelectedIndex];
            string refreshBalance = "SELECT account_balance FROM ClientsAccounts WHERE clientaccount_id = '" + aAccount.AccountId + "'";
            command = new SqlCommand(refreshBalance, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    mainViewBalance.Text = "C$" + reader["account_balance"].ToString();
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

        private void AccountsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {



            //If no account is selected
            if (AccountsList.SelectedIndex == -1)
            {
                return;
            }

            refreshBalance();


            Account aAccount = accounts[AccountsList.SelectedIndex];
            string selectAccount = "SELECT * FROM ClientsAccounts WHERE clientaccount_id = '" + aAccount.AccountId + "'";
            command = new SqlCommand(selectAccount, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            try
            {
                if (reader.Read())
                {

                    switch (aAccount.AccountDescription.Trim().ToString())
                    {
                        case "Checking":
                            btnMainDeposit.IsEnabled = true;
                            btnMainWithdrawal.IsEnabled = true;
                            btnMainTransfer.IsEnabled = true;
                            btnMainPayBill.IsEnabled = true;
                            break;

                        case "Savings":
                            btnMainDeposit.IsEnabled = true;
                            btnMainWithdrawal.IsEnabled = true;
                            btnMainTransfer.IsEnabled = false;
                            btnMainPayBill.IsEnabled = false;
                            break;

                        case "Mortgage":

                            btnMainDeposit.IsEnabled = true;
                            btnMainWithdrawal.IsEnabled = false;
                            btnMainTransfer.IsEnabled = false;
                            btnMainPayBill.IsEnabled = false;
                            break;

                        case "Line of Credit":

                            btnMainDeposit.IsEnabled = false;
                            btnMainWithdrawal.IsEnabled = false;
                            btnMainTransfer.IsEnabled = false;
                            btnMainPayBill.IsEnabled = false;
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

        private void btnMainExit_Click(object sender, RoutedEventArgs e)
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

        private void btnMainTransactions_Click(object sender, RoutedEventArgs e)
        {
            //Create new form instance
            clientTransactions showTransactions = new clientTransactions(user);
            //Display the form
            showTransactions.ShowDialog();
        }

        private void btnMainDeposit_Click(object sender, RoutedEventArgs e)
        {
            //Create new form instance
            clientDeposit depositmoney = new clientDeposit(user);
            //Display the form
            depositmoney.ShowDialog();
            mainViewBalance.Clear();
            refreshBalance();

        }

        private void btnMainWithdrawal_Click(object sender, RoutedEventArgs e)
        {
            //Create new form instance
            clientWithdrawal withdrawalmoney = new clientWithdrawal(user);
            //Display the form
            withdrawalmoney.ShowDialog();
            mainViewBalance.Clear();
            refreshBalance();
        }

        private void btnMainTransfer_Click(object sender, RoutedEventArgs e)
        {
            //Create new form instance
            transferOption transfermoney = new transferOption(user);
            //Display the form
            transfermoney.ShowDialog();
            mainViewBalance.Clear();
            refreshBalance();
        }

        private void btnMainPayBill_Click(object sender, RoutedEventArgs e)
        {
            //Create new form instance
            clientPayBill payBill = new clientPayBill(user);
            //Display the form
            payBill.ShowDialog();
            mainViewBalance.Clear();
            refreshBalance();
        }
    }
}
