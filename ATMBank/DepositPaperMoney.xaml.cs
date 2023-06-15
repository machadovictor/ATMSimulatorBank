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
    /// Lógica interna para DepositPaperMoney.xaml
    /// </summary>
    public partial class DepositPaperMoney : Window
    {
        SqlConnection connection;
        CurrentUser user;
        SqlCommand command;
        internal DepositPaperMoney(CurrentUser current)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            user = current;
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            ShowAtmBalance();
        }

        private void ShowAtmBalance()
        {
            //Create the SELECT query
            string showBalance = "SELECT bank_balance FROM Bank WHERE bank_code = '1'";

            try
            {
                command = new SqlCommand(showBalance, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())

                {
                    availableBalance.Text = reader["bank_balance"].ToString();

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

        private void btnFillAtm_Click(object sender, RoutedEventArgs e)
        {
            //Create the SELECT query
            string updateBalance = "UPDATE Bank SET bank_balance = '20000' WHERE bank_code = '1'";
            command = new SqlCommand(updateBalance, connection);
            connection.Open();

            try
            {

                if (MessageBox.Show("Fill up ATM Balance to C$20.000? ", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    int line = command.ExecuteNonQuery();
                    MessageBox.Show("ATM Balance Filled.", "Confirmation",
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

            ShowAtmBalance();

        }
    }
}
