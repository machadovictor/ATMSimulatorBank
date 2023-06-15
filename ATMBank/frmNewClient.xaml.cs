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
    /// Lógica interna para frmNewClient.xaml
    /// </summary>
    public partial class frmNewClient : Window
    {
        SqlConnection connection;
        SqlCommand command;

        public frmNewClient()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
        }

        public bool ValidateInput()
        {
            bool OK = true;
            if (newClientCode.Text.Trim() == string.Empty ||
            newFullName.Text.Trim() == string.Empty ||
            newTelephone.Text.Trim() == string.Empty ||
            newEmail.Text.Trim() == string.Empty ||
            newLoginPin.Password.Trim() == string.Empty)
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
                string check = $"SELECT client_code FROM Clients WHERE client_code='{newClientCode.Text}'";
                command = new SqlCommand(check, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        MessageBox.Show("This client code already exists.", "Warning!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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


                string insertClient = $"INSERT INTO Clients (client_code, client_fullname, client_phone, client_email, client_pin, client_situation, client_attempts) " +
                $"Values('{newClientCode.Text}','{newFullName.Text}','{newTelephone.Text}', '{newEmail.Text}','{newLoginPin.Password}','{"U"}', '{"0"}')";

                command = new SqlCommand(insertClient, connection);
                try
                {
                    connection.Open();
                    int line = command.ExecuteNonQuery();
                    if (line != 0)
                    {
                        if
                        (MessageBox.Show("New client added. A checking account was automatically created." +
                            Environment.NewLine +
                            "Would you like to add another?",
                            "Success", MessageBoxButton.YesNo,
                            MessageBoxImage.Question) == MessageBoxResult.No)
                        {
                            this.Close();
                        }

                        else
                        {
                            newClientCode.Text = newFullName.Text = string.Empty;
                            newTelephone.Text = newEmail.Text = string.Empty;
                            newLoginPin.Password = string.Empty;

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
                MessageBox.Show("Missing information in a field.", "Warning!", MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
            }


            string insertnewCheckingAccount = $"INSERT INTO ClientsAccounts (client_code, accounttype_code, clientaccount_id, account_balance, accounttype_description) " +
             $"Values('{newClientCode.Text}','{"AC"}','{newClientCode.Text + "AC"}', '{"0"}','{"Checking"}')";

            command = new SqlCommand(insertnewCheckingAccount, connection);
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

        }

        private void btnNewClientCancel_Click(object sender, RoutedEventArgs e)
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
