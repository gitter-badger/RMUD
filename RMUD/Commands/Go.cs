﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Go : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
				new Sequence(
					new KeyWord("GO", true),
					new Cardinal("DIRECTION")),
				new GoProcessor());
		}
	}

	internal class GoProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var direction = Match.Arguments["DIRECTION"] as Direction?;
			var location = Actor.Location as Room;
			var link = location.Links.FirstOrDefault(l => l.Direction == direction.Value);

			if (link == null)
				MudCore.SendEventMessage(Actor, EventMessageScope.Private, "You can't go that way.\n");
			else
			{
				MudCore.SendEventMessage(Actor, EventMessageScope.Locality, "{0} went {1}", direction);
				var destination = MudCore.Database.LoadObject(link.Destination) as Room;
				if (destination == null) throw new InvalidOperationException("Link does not lead to room.");
				MudObject.Move(Actor, destination);
				MudCore.EnqueuClientCommand(Actor.ConnectedClient, "look");
			}
		}
	}
}
