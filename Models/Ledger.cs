using System;
using System.Collections.Generic;

namespace BankAccounts.Models{
    public class Ledger{
        public int LedgerId {get;set;}
        public int transaction {get;set;}
        public DateTime created_at {get;set;}
        public DateTime updated_at {get;set;}
        public int UserId {get;set;}
        public User User {get;set;}
        public Ledger(){
            this.created_at = DateTime.Now;
            this.updated_at = DateTime.Now;

        }
    }
}