using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlobalATM.Models
{
    public class LogUser
    {
    [NotMapped]

    [Required]
    public string LoginAccountNum {get; set;}


    [DataType(DataType.Password)]
    [Required]
    [MinLength(4, ErrorMessage="Must be 4 characters or longer!")]
    public string LoginPin {get; set;}
    }
}