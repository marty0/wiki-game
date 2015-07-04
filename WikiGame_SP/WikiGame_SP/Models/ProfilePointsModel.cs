using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WikiGame_SP.Models
{
    public class ProfilePointsModel
    {
        public int points { get; set; }
        public int timeElapsed { get; set; }
        public string category { get; set; }
        public Nullable<System.DateTime> dateOfGame { get; set; }
    }
}