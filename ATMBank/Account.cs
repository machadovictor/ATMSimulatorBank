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

namespace ATMBank
{
    //Account Class and getters & setters methods
    class Account
    {
        public string ClientId { get; set; }
        public string AccountId { get; set; }
        public string AccountDescription { get; set; }
        public string AccountBalance { get; set; }

        public string AccountFullInfo
        {
            get
            {
                return AccountDescription.ToString().Trim() + " , " + AccountId.ToString().Trim();
            }
        }

        public string AccountFullInfo2
        {
            get
            {
                return ClientId.Trim() + "," + AccountId;
            }
        }
    }
}