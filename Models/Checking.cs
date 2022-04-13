using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GlobalATM.Models
{
    public class Checking : Account
    {
        public string CardNumber {get; set;}

        public bool IsCardStolen { get; set; }

        public Checking()
        {
            IsCardStolen = false;
        }

        public string RandomDigits(int length)
            {
                var random = new Random();
                string s = string.Empty;
                for (int i = 0; i < length; i++)
                    s = String.Concat(s, random.Next(10).ToString());
                return s;
            }
    }
}    
