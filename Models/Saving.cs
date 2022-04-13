using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GlobalATM.Models
{
    public class Saving : Account
    {
        public double InterestRate {get; set;}
    }
}    
