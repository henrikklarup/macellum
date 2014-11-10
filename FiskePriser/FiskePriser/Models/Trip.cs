using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiskePriser.Models
{
    public class Trip
    {
        private DateTime _turTime;

        public Trip()
        { }

        public Trip(List<Fisk> fangst, DateTime turTime, List<Arter> alleArters)
        {
            this.Fangst = fangst;
            this.TurTime = turTime;
            this.AlleArters = alleArters;
        }

        public List<Fisk> Fangst { get; set; }
        public string CurFangst { get; set; }
        public string CurFishId { get; set; }
        public string CurFishSort { get; set; }
        public string CurFishAmount { get; set; }


        public DateTime TurTime
        {
            get { return _turTime; }
            set { _turTime = value; }
        }

        public List<Arter> AlleArters { get; set; }
    }
}