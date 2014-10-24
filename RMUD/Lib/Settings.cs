﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Settings : MudObject
	{
		public String Banner;
		public String MessageOfTheDay;

        public int TelnetPort = 8669;
        public int WebsocketPort = 8670;

        public String NewPlayerStartRoom;
        public bool UpfrontCompilation = true;
        public bool UseConsoleCommands = false;

        public int AllowedCommandRate = 100; //How many milliseconds to allow between commands - default is to not limit very much.

        //How many milliseconds should a command be allowed to run before it is aborted?
        //Aborting a player's command is always reported as a critical error, however this
        //  helps guard against infinite loops in the database source that could lock up
        //  the server.
        public int CommandTimeOut = 10000;

        public int AFKTime = 1000 * 60 * 5; //Go AFK after five minutes of inactivity.

        public String ProscriptionList = "proscriptions.txt";
        public int MaximumChatChannelLogSize = 1000;
        public int HeartbeatInterval = 1000 * 10; //Heart beat every ten seconds.
        public TimeSpan ClockAdvanceRate = TimeSpan.FromMinutes(1);

        public Dictionary<int, String> RankNames;

        public Settings()
        {
            RankNames = new Dictionary<int, string>();
            RankNames.Add(Int32.MaxValue, "deum confugiat");
            RankNames.Add(500, "deus");
            RankNames.Add(400, "imperator");
            RankNames.Add(300, "patrician");
            RankNames.Add(200, "senator");
            RankNames.Add(100, "equester");
            RankNames.Add(0, "proletarian");
            RankNames.Add(Int32.MinValue, "sentina");
        }

        public String GetNameForRank(int Rank)
        {
            foreach (var entry in RankNames)
            {
                if (entry.Key <= Rank) return entry.Value;
            }

            return "errorem magnificum";
        }
	}
}
