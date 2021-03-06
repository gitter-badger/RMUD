﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

    public static class HelperExtensions
    {
        public static void Upsert<A, B>(this Dictionary<A, B> Dict, A _a, B _b)
        {
            if (Dict.ContainsKey(_a)) Dict[_a] = _b;
            else Dict.Add(_a, _b);
        }

		public static void Add<T>(this List<T> List, params T[] objects)
		{
			List.AddRange(objects);
		}

        public static B ValueOrDefault<A,B>(this Dictionary<A, B> Dict, A Key)
        {
            if (Dict.ContainsKey(Key)) return Dict[Key];
            else return default(B);
        }
    }
