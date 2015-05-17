using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WikiGame.Models
{
    public class GameCategory
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        private List<string> keyWords;
        public List<string> KeyWords
        {
            get
            {
                if (keyWords == null)
                    keyWords = new List<string>();

                return keyWords;
            }
        }
    }
}