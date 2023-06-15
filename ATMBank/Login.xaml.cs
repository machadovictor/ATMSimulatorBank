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
using System.Data.SqlClient;
using System.Configuration;

namespace ATMBank
{
    /// <summary>
    /// Lógica interna para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        SqlConnection connection;
        SqlCommand command;
        bool open = false;
        public Login()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            loginClientCode.Focus();
            open = true;
            string checkstatus = "SELECT bank_status FROM Bank";
            command = new SqlCommand(checkstatus, connection);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Bank status = new Bank();
                    status.BankStatus = reader["bank_status"].ToString().Trim();


                    if (status.BankStatus.Trim() == "C")
                    {
                        MessageBox.Show("THIS ATM IS CURRENTLY CLOSED.");
                        loginClientCode.Text = String.Empty;
                        loginPin.Password = String.Empty;
                        this.Close();
                        btnMainLogin.IsEnabled = false;

                    }
                    else
                    {
                        ValidateInput();
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


        public bool ValidateInput()
        {
            bool OK = true;
            if (loginClientCode.Text.Trim() == string.Empty ||
            loginPin.Password.Trim() == string.Empty)
            {
                OK = false;
            }
            return OK;
        }

    

        private void loginPin_PasswordChanged_1(object sender, RoutedEventArgs e)
        {
            if (loginClientCode.Text != null)
            {
                btnMainLogin.IsEnabled = true;
            }
        }

        private void loginClientCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (loginPin.Password != null)
            {
                btnMainLogin.IsEnabled = true;
            }
        }

        private void btnMainLogin_Click(object sender, RoutedEventArgs e)
        {
            int attempts;
            CurrentUser user = new CurrentUser();


            string authentication = "SELECT * FROM Clients WHERE client_code = '" + loginClientCode.Text + "' AND client_pin = '" + loginPin.Password + "'";
            command = new SqlCommand(authentication, connection);


            try
            {

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();


                //Check to see if a record was found
                if (reader.Read())
                {
                    //Creates the CurrentUser object
                    user.UserId = reader["client_code"].ToString();
                    user.FullName = reader["client_fullname"].ToString();
                    user.Situation = reader["client_situation"].ToString();
                    user.Attempts = reader["client_attempts"].ToString();


                    if (user.UserId == "0")
                    {
                        //Create an instance of the form
                        adminMainScreen AdminManagement = new adminMainScreen(user);
                        MessageBox.Show("Welcome " + user.FullName);
                        //Show the form
                        AdminManagement.Show();
                        open = false;
                        //Close the Login form
                        this.Close();
                    }


                    else if (user.Situation.Trim() == "B" || user.Attempts == "3")
                    {
                        //Login failed
                        MessageBox.Show("Authentication failed. Your account is blocked.");
                        loginClientCode.Text = String.Empty;
                        loginPin.Password = String.Empty;
                        loginClientCode.Focus();
                    }

                    else
                    {

                        string UpdateAttempts = "UPDATE Clients SET client_attempts = '0' WHERE client_code = '" + user.UserId + "'";
                        command = new SqlCommand(UpdateAttempts, connection);

                        try
                        {
                            connection.Close();
                            connection.Open();
                            SqlDataReader reader2 = command.ExecuteReader();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            connection.Close();
                        }


                        //Create an instance of the form
                        MainWindow clientManagement = new MainWindow(user);
                        MessageBox.Show("Welcome " + user.FullName);
                        //Show the form
                        clientManagement.Show();
                        open = false;
                        //Close the Login form
                        this.Close();
                    }

                }

                else
                {
                    string loginAttempts = "SELECT client_code, client_situation, client_attempts FROM Clients WHERE client_code = '" + loginClientCode.Text + "'";
                    command = new SqlCommand(loginAttempts, connection);


                    try
                    {
                        connection.Close();
                        connection.Open();
                        SqlDataReader reader2 = command.ExecuteReader();

                        if (reader2.Read())
                        {
                            user.UserId = reader2["client_code"].ToString();
                            user.Situation = reader2["client_situation"].ToString();
                            user.Attempts = reader2["client_attempts"].ToString();
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

                    Int32.TryParse(user.Attempts, out attempts);
                    ++attempts;

                    if (attempts >= 3)
                    {
                        string attempt = "UPDATE Clients SET client_situation = 'B' WHERE client_code = '" + user.UserId + "'";
                        command = new SqlCommand(attempt, connection);

                        try
                        {
                            connection.Open();
                            SqlDataReader reader2 = command.ExecuteReader();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            connection.Close();
                        }


                        MessageBox.Show("Sorry, this account is now blocked. Contact your bank manager.");
                        loginClientCode.Text = String.Empty;
                        loginPin.Password = String.Empty;
                        loginClientCode.Focus();
                    }

                    else
                    {
                        string attempt = "UPDATE Clients SET client_attempts = '" + attempts + "' WHERE client_code = '" + user.UserId + "'";
                        command = new SqlCommand(attempt, connection);


                        try
                        {
                            connection.Open();
                            SqlDataReader reader2 = command.ExecuteReader();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            connection.Close();
                        }

                        //Login failed
                        MessageBox.Show("Authentication failed. Please try again. After 3 incorrect attempts, the account will be blocked.");
                        loginClientCode.Text = String.Empty;
                        loginPin.Password = String.Empty;
                        loginClientCode.Focus();
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
            
        private void btnMainClose_Click(object sender, RoutedEventArgs e)
        {
             this.Close();

        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }


   
    }

