//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ATMBank
{
    using System;
    using System.Collections.Generic;

    class Bank
    {
        public string BankCode { get; set; }
        public string BankBalance { get; set; }
        public string BankStatus { get; set; }


        public string BankInfo
        {
            get
            {
                return BankStatus.ToString();

            }
        }
    }
}
