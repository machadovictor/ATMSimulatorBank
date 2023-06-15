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
    /// Lógica interna para transferOption.xaml
    /// </summary>
    public partial class transferOption : Window
    {

        bool open = false;
        SqlConnection connection;
        CurrentUser user;
        SqlCommand command;
        internal transferOption(CurrentUser current)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            user = current;
            //Display the current user in the window's title
            Title += " - " + user.FullName;
            InitializeComponent();
        }

        private void btnBetweenAccounts_Click(object sender, RoutedEventArgs e)
        {
            //Create new form instance
            clientTransfer transfermoney = new clientTransfer(user);
            //Display the form
            this.Close();
            transfermoney.ShowDialog();
        }

        private void btnAnotherHolder_Click(object sender, RoutedEventArgs e)
        {
            //Create new form instance
            clientTransferToOther transfermoney = new clientTransferToOther(user);
            //Display the form
            this.Close();
            transfermoney.ShowDialog();
        }
    }
}
