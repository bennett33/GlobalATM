using System;
using System.ComponentModel.DataAnnotations;

namespace GlobalATM.Models
{
    public class Transaction//Belongs to the account 1:n relationship 
    {
        [Key]
        public int TransactionId {get;set;}
        public int UserId {get;set;}

        public Account Account {get; set;}

        [Required(ErrorMessage ="is required")]
        public double Amount {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;

    }
}