using Microsoft.EntityFrameworkCore;
using PROG7311_ST10152431_Part2.Data;
using System.Collections.Generic;

namespace PROG7311_ST10152431_Part2.Models
{
    public class Farmer
    {
        public int FarmerId { get; set; }
        public required string FarmerName { get; set; }
        public required string FarmerSurname { get; set; }
        public required string FarmerPassword { get; set; }


        //Navigation Property
        public ICollection<Product> Products { get; set; } = new List<Product>();

    }
}
