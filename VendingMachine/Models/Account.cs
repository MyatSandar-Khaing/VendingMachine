﻿using System.ComponentModel.DataAnnotations;

namespace VendingMachine.Models
{
    public class Account
    {
        public int ID { get; set; } 
        public string Name { get; set; }=string.Empty;
        public string Email {  get; set; }=string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public ICollection<PurchaseTransaction> PurchaseTransactions { get; set; }

    }
}
