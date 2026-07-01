using System;
using System.Collections.Generic;

namespace project_edp
{
    public class CartData
    {
        public static List<string> Items = new List<string>();
        public static decimal TotalAmount { get; set; }
        public static string CurrentTransactionID { get; set; }
    }
}