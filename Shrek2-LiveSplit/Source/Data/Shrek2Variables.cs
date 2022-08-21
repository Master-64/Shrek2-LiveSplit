using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shrek2_LiveSplit.Data
{
    public class Shrek2Variables
    {
        public const string ComponentName = "Shrek 2 Auto Splitter";
        public const string UpdateUrl = "https://raw.githubusercontent.com/Master-64/Shrek2-LiveSplit/master/";

        public const int SleepTime = 1;

        public static List<string> ExcludedSplitMaps = new List<string>()
        {
            "book_frontend",
            "book_story_1",
            // "book_story_2", // Master_64: Excluding this so that Carriage Hijack splits at the right time
            // "book_story_3", // Master_64: Excluding this so that Hamlet End splits at the right time
            "book_story_4",
            "book_storybook",
            "credits",
            "entry",
            "beanstalk_bonus",
            "beanstalk_bonus_dawn",
            "beanstalk_bonus_knight",
            "1_shreks_swamp",
            "4_fgm_pib",
            "3_the_hunt_part1",
            "7_prison_donkey"

            // Master_64: These below are now excluded so that each map splits instead of each mission like it originally did
            // "3_the_hunt_part2",
            // "3_the_hunt_part3",
            // "3_the_hunt_part4",
            // "5_fgm_donkey",
            // "6_hamlet_end",
            // "6_hamlet_mine"
        };
    }
}
