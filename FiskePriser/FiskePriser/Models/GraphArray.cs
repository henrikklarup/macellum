using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiskePriser.Models
{
    public class GraphArray
    {
        public List<GraphInfo> GraphNumbers = new List<GraphInfo>(); 
    }

    public class GraphInfo
    {
        public string Name;
        public decimal Value;
        public decimal Value2;

        public GraphInfo(string name, decimal value)
        {
            Name = name;
            Value = value;
        }
        public GraphInfo(string name, decimal value, decimal value2)
        {
            Name = name;
            Value = value;
            Value2 = value2;
        }
    }
}