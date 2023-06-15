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
    /// Lógica interna para frmNewAccount.xaml
    /// </summary>
    public partial class frmNewAccount : Window
    {
        SqlConnection connection;
        SqlCommand command;
        Account account;
        Client client;
        List<Client> clients = new List<Client>();
        List<Account> accounts = new List<Account>();
        string AccountType;

        public frmNewAccount()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            FillClientsList();
            newAccountType.Items.Add("Checking");
            newAccountType.Items.Add("Mortgage");
            newAccountType.Items.Add("Line of Credit");
            newAccountType.Items.Add("Savings");
        }

        public void FillClientsList()
        {
            //Create the SELECT query
            string selectClient = "SELECT * FROM Clients ORDER BY client_code";

            try
            {
                command = new SqlCommand(selectClient, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    //Create a new client
                    client = new Client();
                    client.ClientId = reader["client_code"].ToString().Trim();

                    //Add to list
                    clients.Add(client);
                }
                newAccountClientCode.DataContext = clients.ToList();

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

        private void newAccountType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (newAccountType.SelectedIndex)
            {
                case 0:
                    AccountType = "AC";
                    break;
                case 1:
                    AccountType = "AL";
                    break;
                case 2:
                    AccountType = "AM";
                    break;
                case 3:
                    AccountType = "AS";
                    break;
            }
        }

        public bool ValidateInput()
        {
            bool OK = true;
            if (newAccountId.Text.Trim() == string.Empty ||
            newAccountBalance.Text.Trim() == string.Empty ||
             newAccountType.SelectedIndex == -1 ||
             newAccountClientCode.SelectedIndex == -1)

            {
                OK = false;
            }
            return OK;
        }

        private void btnNewClientSave_Click(object sender, RoutedEventArgs e)
        {
            bool OK = ValidateInput();
            if (OK)
            {

                //Query to check if code exists
                string check = $"SELECT clientaccount_id FROM ClientsAccounts WHERE clientaccount_id='{newAccountId.Text.ToString().Trim()}'";
                command = new SqlCommand(check, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        MessageBox.Show("This account ID already exists.", "Warning!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
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

                string checkLC = $"SELECT accounttype_code FROM ClientsAccounts WHERE clientaccount_id='{newAccountId.Text.ToString().Trim()}' AND accounttype_description = 'Line of Credit'";
                command = new SqlCommand(checkLC, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        Account account = new Account();
                        account.AccountDescription = reader["accounttype_description"].ToString().Trim();

                        if (account.AccountDescription == "Line of Credit")
                        {
                            MessageBox.Show("This client already has a Line of Credit.", "Warning!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }

                        else
                        {
                            string insertAccount = $"INSERT INTO ClientsAccounts (client_code, accounttype_code, clientaccount_id, account_balance, accounttype_description) " +
                             $"Values('{newAccountClientCode.Text}','{AccountType}','{newAccountId.Text}', '{newAccountBalance.Text}','{newAccountType.Text}')";

                            command = new SqlCommand(insertAccount, connection);
                            try
                            {

                                connection.Open();
                                int line = command.ExecuteNonQuery();
                                if (line != 0)
                                {
                                    if
                                    (MessageBox.Show("New account added." +
                                        Environment.NewLine +
                                        "Would you like to add another?",
                                        "Success", MessageBoxButton.YesNo,
                                        MessageBoxImage.Question) == MessageBoxResult.No)
                                    {
                                        this.Close();
                                    }

                                    else
                                    {
                                        newAccountClientCode.Text = newAccountType.Text = string.Empty;
                                        newAccountId.Text = newAccountBalance.Text = string.Empty;

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

                    else
                    {
                        MessageBox.Show("Missing information in a field.", "Warning!", MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);
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

        private void btnNewClientCancel_Click_1(object sender, RoutedEventArgs e)
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

        private void newAccountClientCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //If no client is selected
            if (newAccountClientCode.SelectedIndex == -1)
            {
                return;
            }


            Client clientName = clients[newAccountClientCode.SelectedIndex];
            string showClientName = "SELECT client_fullname FROM Clients WHERE client_code = '" + clientName.ClientId + "'";
            command = new SqlCommand(showClientName, connection);


            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    clientShowFullName.Text = reader["client_fullname"].ToString().Trim();

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

            accounts.Clear();


            string selectExistingAccounts = "SELECT clientaccount_id, accounttype_description FROM ClientsAccounts WHERE client_code= '" + clientName.ClientId + "' ORDER BY clientaccount_id";
            command = new SqlCommand(selectExistingAccounts, connection);



            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                    //Create a new account
                    account = new Account();
                    account.AccountId = reader["clientaccount_id"].ToString();
                    account.AccountDescription = reader["accounttype_description"].ToString();

                    //Add to list
                    accounts.Add(account);
                }
                newAccountExistingAccounts.DataContext = accounts.ToList();


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
