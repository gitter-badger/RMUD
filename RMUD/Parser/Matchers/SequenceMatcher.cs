﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class Sequence : CommandTokenMatcher
    {
        internal List<CommandTokenMatcher> Matchers = new List<CommandTokenMatcher>();

		public Sequence(params CommandTokenMatcher[] matchers)
		{
			Matchers.AddRange(matchers);
		}

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            var Matches = new List<PossibleMatch>();
            Matches.Add(State);
            foreach (var Matcher in Matchers)
            {
                var NextMatches = new List<PossibleMatch>();
                foreach (var Match in Matches)
                    NextMatches.AddRange(Matcher.Match(Match, Context));
                Matches = NextMatches;
				if (Matches.Count == 0) return Matches; //Shortcircuit for when no matches are found.
            }
            return Matches;
        }

		public String Emit() { return String.Join(" ", Matchers.Select(m => m.Emit())); }

    }

}
