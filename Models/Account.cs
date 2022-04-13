using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GlobalATM.Models
{
    public abstract class Account
    {
        [Key]
        public int AccountId {get;set;}

        public User User {get; set;}
        public int UserId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public string AccountNumber {get; set;}
        public string RoutingNumber {get; set;}

        public List<Transaction> Transactions {get;set;} = new List<Transaction>();


        // Will need to .Include() transactions to use Balance!
        public double Balance
        {
            get
            {
                return Transactions.Sum(t => t.Amount);

            }
        }


        // public double GetSum(List<Transaction> allTransactions)
        // {
        //     double Total = 0;
        //     foreach (Transaction transaction in allTransactions)
    
        //     {
        //         Total += transaction.Amount;
        //     }
        //     return Total;
        // }
    }
}    
