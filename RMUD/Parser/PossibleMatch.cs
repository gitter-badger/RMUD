﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class PossibleMatch
    {
        public LinkedListNode<String> Next = null;
        public Dictionary<String, Object> Arguments = null;

        public PossibleMatch(LinkedListNode<String> Next)
        {
            this.Next = Next;
            Arguments = new Dictionary<String, Object>();
        }

		public PossibleMatch(Dictionary<String, Object> Arguments, LinkedListNode<String> Next)
		{
			this.Next = Next;
			this.Arguments = new Dictionary<String, Object>(Arguments);
		}

        public PossibleMatch With(String ArgumentName, Object Value)
        {
            var r = new PossibleMatch(Arguments, Next);
            r.Arguments.Upsert(ArgumentName, Value);
            return r;
        }

        public PossibleMatch AdvanceWith(String ArgumentName, Object Value)
        {
            var r = new PossibleMatch(Arguments, Next.Next);
            r.Arguments.Upsert(ArgumentName, Value);
            return r;
        }

        public PossibleMatch EndWith(String ArgumentName, Object Value)
        {
            var r = new PossibleMatch(Arguments, null);
            r.Arguments.Upsert(ArgumentName, Value);
            return r;
        }
    }
}
