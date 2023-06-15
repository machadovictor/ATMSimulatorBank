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
    //Client class and getters & setters methods for each field
    class Client
    {
        public string AccountDescription { get; set; }
        public string AccountId { get; set; }
        public string ClientId { get; set; }
        public string FullName { get; set; }
        public string ClientSituation { get; set; }
        public string TransactionType_code { get; set; }
        public string TransactionType_Description { get; set; }

        public string ClientFullInfo
        {
            get
            {
                return ClientId.ToString();

            }
        }
    }
}