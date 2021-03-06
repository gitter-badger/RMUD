﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RMUD
{
	public class ParserCommandHandler : ClientCommandHandler
	{
		internal CommandParser Parser;

		public ParserCommandHandler()
		{
			Parser = new CommandParser();

			foreach (var cmd in CommandFactory.AllCommands)
			    CommandFactory.GetCommand(cmd.Key).Create(Parser);
		}

        public void HandleCommand(Client Client, String Command)
        {
            if (String.IsNullOrEmpty(Command)) return;

            bool displayMatches = false;
            bool displayTime = false;

            if (Command[0] == '@')
            {
                if (Command.ToUpper().StartsWith("@MATCH "))
                {
                    Command = Command.Substring("@MATCH ".Length);
                    displayMatches = true;
                }
                else if (Command.ToUpper().StartsWith("@TIME "))
                {
                    Command = Command.Substring("@TIME ".Length);
                    displayTime = true;
                }
                else if (Command.ToUpper().StartsWith("@DEBUG "))
                {
                    Command = Command.Substring("@DEBUG ".Length);
                    if (Client.Rank < 500)
                    {
                        Mud.SendMessage(Client, "You do not have sufficient rank to use the debug command.");
                        return;
                    }

                    Mud.CommandTimeoutEnabled = false;
                }
                else
                {
                    Mud.SendMessage(Client, "I don't recognize that debugging command.");
                    return;
                }
            }

            var startTime = DateTime.Now;

            var matchedCommand = Parser.ParseCommand(Command, Client.Player);
            
            if (displayMatches)
            {
                var matchEndTime = DateTime.Now;

                if (matchedCommand == null)
                {
                    Mud.SendMessage(Client, String.Format("Matched nothing in {0:n0} milliseconds.",
                        (matchEndTime - startTime).TotalMilliseconds));
                }
                else
                {
                    Mud.SendMessage(Client, String.Format("Matched {0} in {1:n0} milliseconds. {2} unique matches.",
                        matchedCommand.Command.Processor.GetType().Name,
                        (matchEndTime - startTime).TotalMilliseconds,
                        matchedCommand.Matches.Count));
                    foreach (var match in matchedCommand.Matches)
                    {
                        var builder = new StringBuilder();

                        foreach (var arg in match.Arguments)
                        {
                            builder.Append("[");
                            builder.Append(arg.Key);
                            builder.Append(" : ");
                            builder.Append(arg.Value.ToString());
                            builder.Append("] ");
                        }

                        Mud.SendMessage(Client, builder.ToString());
                    }
                }

                return;
            }

            if (matchedCommand != null)
            {
                if (matchedCommand.Matches.Count > 1)
                    Client.CommandHandler = new DisambigCommandHandler(Client, matchedCommand, this);
                else
                    Mud.ProcessPlayerCommand(matchedCommand.Command.Processor, matchedCommand.Matches[0], Client.Player);
            }
            else
                Mud.SendMessage(Client, "huh?");

            if (displayTime)
            {
                var endTime = DateTime.Now;

                Mud.SendMessage(Client, String.Format("Command completed in {0} milliseconds.", (endTime - startTime).TotalMilliseconds));
            }
        }
    }
}
