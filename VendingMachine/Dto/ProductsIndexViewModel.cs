﻿using VendingMachine.Models;

namespace VendingMachine.Dto
{
    public class ProductsIndexViewModel
    {
        public List<Product> Products { get; set; }
        public string CurrentSort { get; set; } = "created_date";
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
