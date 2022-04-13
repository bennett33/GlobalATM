using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

//this goes unused with current api and foreign currency converter using only JS. This can be CHANGED
namespace GlobalATM.Models
{
    public class ConversionRaw
    {
        public double Amount {get; set;}
        public string Base_Currency_Code {get; set;}
        public string Base_Currency_Name {get;set;}

        public string Currency_Name {get;set;}

        public double Rate {get; set;}
        public double Rate_For_Amount {get;set;}

    }
    public class ConversionObject
    {
        public ConversionRaw[] conversion {get;set;}
    }
    //this is our deserialized json object, but need work to display
    // var conversionObject = JsonConvert.DeserializeObject<ConversionObject>(json);

} 


