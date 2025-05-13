using Microsoft.EntityFrameworkCore;
using PROG7311_ST10152431_Part2.Data;

namespace PROG7311_ST10152431_Part2.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public required string ProductCategory { get; set; }
        public decimal ProductPrice { get; set; }

        //Foreign Key
        public int FarmerId { get; set; }

        //Navigation Property
        public Farmer Farmer { get; set; } = null!;
    }
}
