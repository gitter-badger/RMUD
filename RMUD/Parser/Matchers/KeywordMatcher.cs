﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    internal class KeyWord : CommandTokenMatcher
    {
        public String Word;
        public bool Optional = false;

        internal KeyWord(String Word, bool Optional = false)
        {
            this.Word = Word.ToUpper();
            this.Optional = Optional;
        }

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            var R = new List<PossibleMatch>();
			if (State.Next != null && State.Next.Value.ToUpper() == Word)
				R.Add(new PossibleMatch(State.Arguments, State.Next.Next));
            else if (Optional) //Greedy match
                R.Add(new PossibleMatch(State.Arguments, State.Next));
            return R;
        }

		public String Emit() { return Optional ? (Word.ToLower() + "?") : Word.ToLower(); }
    }
}
