using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Web;
using HtmlAgilityPack;

namespace FiskePriser.Models
{
    public class Helper
    {

        public List<Arter> PerformSwap(List<Arter> allArters, string replaceName, int id)
        {
            var firstOrDefault = allArters.FirstOrDefault(s => s.Navn.Normalize().Trim().ToLower() == replaceName.Normalize().Trim().ToLower());
            if (firstOrDefault != null)
            {
                var x = firstOrDefault.Id;
                allArters.Swap(allArters.FindIndex(s => s.Id == x), id);
            }
            return allArters;
        }
    }


    static class IListExtensions
    {
        public static void Swap<T>(
            this IList<T> list,
            int firstIndex,
            int secondIndex
        )
        {
            Contract.Requires(list != null);
            Contract.Requires(firstIndex >= 0 && firstIndex < list.Count);
            Contract.Requires(secondIndex >= 0 && secondIndex < list.Count);
            if (firstIndex == secondIndex)
            {
                return;
            }
            T temp = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = temp;
        }
    }
}