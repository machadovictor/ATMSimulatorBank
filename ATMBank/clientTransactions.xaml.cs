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
using System.Data;

namespace ATMBank
{
    /// <summary>
    /// Lógica interna para clientTransactions.xaml
    /// </summary>
    public partial class clientTransactions : Window
    {
        SqlConnection connection;
        SqlCommand command;
        CurrentUser user;

        internal clientTransactions(CurrentUser current)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            user = current;
            //Display the current user in the window's title
            Title += " - " + user.FullName;
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //SQL query to select the transactions specifically from the logged-in client 
            string showTransactions = "SELECT transaction_date, accounttype_description, clientaccount_id, transactiontype_description, clientaccount_id_transferto, transaction_amount  FROM TransactionsHistory WHERE client_code = '" + user.UserId + "' ORDER BY transaction_date";

            command = new SqlCommand(showTransactions, connection);
            connection.Open();

            try
            {

                DataTable dt = new DataTable();
                SqlDataAdapter a = new SqlDataAdapter(command);
                a.Fill(dt);
                grdTransactions.ItemsSource = dt.DefaultView;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                //close database connection
                connection.Close();
            }
        }
    }
}
