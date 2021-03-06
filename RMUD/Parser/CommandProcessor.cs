﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public interface CommandProcessor
    {
		void Perform(PossibleMatch Match, Actor Actor);
    }

	public class CommandProcessorWrapper : CommandProcessor
	{
		private Action<PossibleMatch, Actor> Processor;

		public CommandProcessorWrapper(Action<PossibleMatch, Actor> Processor)
		{
			this.Processor = Processor;
		}

		public void Perform(PossibleMatch Match, Actor Actor)
		{
			Processor(Match, Actor);
		}
	}

}
