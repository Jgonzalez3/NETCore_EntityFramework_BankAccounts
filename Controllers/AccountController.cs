using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BankAccounts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BankAccounts.Controllers{
    public class AccountController : Controller{

        private BankAccountsContext _context;
        public AccountController(BankAccountsContext context){
            _context = context;
        }
        [HttpGet]
        [Route("/account/{AccountId}")]
        public IActionResult DisplayAccount(int AccountId){
            int? UserId = HttpContext.Session.GetInt32("userid");
            if(UserId == null){
                return RedirectToAction("Index", "Home");
            }
            User Name = _context.Users.SingleOrDefault(user => user.UserId == UserId);
            ViewBag.Username = Name.first_name;
            List<Ledger> Transactions = _context.Ledger.Include(Ledger=>Ledger.User).Where(Ledger=>Ledger.UserId == UserId).ToList();
            Transactions.Reverse();
            ViewBag.Ledger = Transactions;
            int balance = 0;            
            foreach(var trans in Transactions){
                balance += trans.transaction;
            }
            HttpContext.Session.SetInt32("balance", balance);
            ViewBag.Balance = balance;
            return View("Account");
        }

        [HttpPost]
        [Route("/account/transaction")]
        public IActionResult Transaction(int transaction){
            int? UserId = HttpContext.Session.GetInt32("userid");
            int? Balance = HttpContext.Session.GetInt32("balance");
            if((transaction + (int)Balance) < 0){
                TempData["Insufficientfunds"] = "Cannot withdraw more than current account balance";
                return RedirectToAction("DisplayAccount", new{AccountId=(int)UserId});
            }
            Ledger NewTrans = new Ledger{
                transaction = transaction,
                UserId = (int)UserId,
            };
            _context.Ledger.Add(NewTrans);
            _context.SaveChanges();
            return RedirectToAction("DisplayAccount", new {AccountId=(int)UserId});
        }
    }
}