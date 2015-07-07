using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WikiGame.Models
{
    public class TestCategoryProvider : ICategoryProvider
    {
        private Dictionary<string, GameCategory> categoriesCatalog;

        public TestCategoryProvider()
        {
            categoriesCatalog = new Dictionary<string, GameCategory>();

            GameCategory cat0 = new GameCategory();
            cat0.Name = "Sport";
            cat0.KeyWords.AddRange(new string[] {"aquatics",
"archery",
"automobile racing",
"badminton",
"base jumping",
"baseball",
"basketball",
"beach volleyball",
"biathlon",
"bobsleigh",
"bocce ball",
"body building",
"boomerang",
"bowling",
"boxing",
"bull fighting",
"camping",
"canoeing",
"caving",
"cheerleading",
"chess",
"classical dance",
"cricket",
"cross country running",
"cross country skiing",
"curling",
"cycling",
"darts",
"decathlon",
"diving",
"dog sledding",
"dog training",
"down hill skiing",
"equestrianism",
"falconry",
"fencing",
"figure skating",
"fishing",
"flag football",
"foosball",
"football",
"fox hunting",
"golf",
"gymnastics",
"hand ball",
"hang gliding",
"high jump",
"hiking",
"hockey",
"horseshoes",
"hot air ballooning",
"hunting",
"ice skating",
"inline skating",
"jai alai",
"judo",
"karate",
"kayaking",
"knee boarding",
"lacrosse",
"land sailing",
"log rolling",
"long jump",
"luge",
"modern dance",
"modern pentathlon",
"motorcycle racing",
"mountain biking",
"mountaineering",
"netball",
"paint ball",
"para gliding",
"parachuting",
"petanque",
"pool playing",
"power walking",
"quad biking",
"racquetball",
"remote control boating",
"river rafting",
"rock climbing",
"rodeo riding",
"roller skating",
"rowing",
"rugby",
"sailing",
"scuba diving",
"shooting",
"shot put",
"shuffleboard",
"skateboarding",
"skeet shooting",
"snooker",
"snow biking",
"snow boarding",
"snow shoeing",
"snow sledding",
"soccer",
"sombo",
"speed skating",
"sport fishing",
"sport guide",
"sprint running",
"squash",
"stunt plane flying",
"sumo wrestling",
"surfing",
"swimming",
"synchronized swimming",
"table tennis",
"taekwondo",
"tchoukball",
"tennis",
"track and field",
"trampolining",
"triathlon",
"tug of war",
"volleyball",
"water polo",
"water skiing",
"weight lifting",
"wheelchair basketball",
"white water rafting",
"wind surfing",
"wrestling",
"wushu",
"yachting",
"yoga"});

            GameCategory cat1 = new GameCategory();
            cat1.Name = "Game Mode Easy";
            cat1.KeyWords.AddRange(new string[] { "a", "an", "of", "the", "and","for","on" });

            GameCategory cat2 = new GameCategory();
            cat2.Name = "Balkans";
            cat2.KeyWords.AddRange(new string[] { "albania", "bosnia and herzegovina", "bulgaria", "croatia", "greece", "kosovo", "macedonia", "montenegro", "romania", "slovenia", "serbia", "turkey" });

            GameCategory cat3 = new GameCategory();
            cat3.Name = "Bulgaria";
            cat3.KeyWords.AddRange(new string[] { "bulgaria" });

            GameCategory cat4 = new GameCategory();
            cat4.Name = "Eight-thousanders";
            cat4.KeyWords.AddRange(new string[] { "mount everest", "k2", "kangchenjunga ", "lhotse", "makalu", "cho oyu", "dhaulagiri", "manaslu", "nanga parbat", "annapurna", "gasherbrum i", "broad peak ", "gasherbrum ii", "shishapangma"});

            GameCategory cat5 = new GameCategory();
            cat5.Name = "Mammals";
            cat5.KeyWords.AddRange(new string[] { "alpaca", "cow", "donkey", "cat", "dog", "fox", "sheep", "horse", "coat", "elephant", "rodent", "ferret", "liama", "pig", "rabbit", "rat", "mice", "hamster", "pig", "gerbil", "chinchila" });

            GameCategory cat6 = new GameCategory();
            cat6.Name = "Top 10 Best Love Songs Of All Time";
            cat6.KeyWords.AddRange(new string[] { "higher love", "iris", "you needed me", "baby baby", "rosanna", "escape", "danny's song", "what's my name?", "lovesong", "love theme from a star is born" });

            GameCategory cat7 = new GameCategory();
            cat7.Name = "Provinces of Bulgaria";
            cat7.KeyWords.AddRange(new string[] { "blagoevgrad", "burgas", "dobrich", "gabrovo", "haskovo", "kardzhali", "kyustendil", "lovech", "montana", "pazardzhik", "pernik", "pleven", "plovdiv", "razgrad", "ruse", "shumen", "silistra", "sliven", "smolyan", "sofia city", "sofia", "stara zagora", "targovishte", "varna", "veliko tarnovo", "vidin", "vratsa", "yambol" });

            this.AddCategory(cat0);
            this.AddCategory(cat1);
            this.AddCategory(cat2);
            this.AddCategory(cat3);
            this.AddCategory(cat4);
            this.AddCategory(cat5);
            this.AddCategory(cat6);
            this.AddCategory(cat7);
        }


        public List<GameCategory> GetAllCategories()
        {
            return categoriesCatalog.Values.ToList<GameCategory>();
        }

        public bool ClearCategories()
        {
            categoriesCatalog.Clear();
            return true;
        }

        public bool AddCategory(GameCategory category)
        {
            categoriesCatalog.Add(category.Name, category);
            return true;
        }


        public GameCategory GetCategoryByName(string name)
        {
            return categoriesCatalog[name];
        }
    }
}