using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestDotNet.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Point { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
    }
}