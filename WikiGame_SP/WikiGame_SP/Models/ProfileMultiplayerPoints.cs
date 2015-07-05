using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WikiGame_SP.Models
{
    public class ProfileMultiplayerPoints
    {
        public int points { get; set; }
        public int timeElapsed { get; set; }
        public string opponent { get; set; }
        public string category { get; set; }
        public Nullable<System.DateTime> dateOfGame { get; set; }
        public string winner { get; set; }
    }
}