using System;
using System.Collections.Generic;

namespace BankAccounts.Models{
    public class User{
        public int UserId {get;set;}
        public string first_name {get;set;}
        public string last_name {get;set;}
        public string email {get;set;}
        public string password {get;set;}
        public DateTime created_at {get;set;}
        public DateTime updated_at {get;set;}
        public List<Ledger> transactions {get;set;}
        public User(){
            transactions = new List<Ledger>();
            this.created_at = DateTime.Now;
            this.updated_at = DateTime.Now;
        }
    }
}