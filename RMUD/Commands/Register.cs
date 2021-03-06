﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Register : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new KeyWord("REGISTER", false),
                    new FailIfNoMatches(
                        new SingleWord("USERNAME"),
                        "You must supply a username.")),
                    new RegistrationProcessor(),
                    "Create a new account.");
        }
	}

    internal class RegistrationProcessor : AuthenticationCommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (Actor != null)
            {
                Mud.SendMessage(Actor.ConnectedClient, "You are already logged in.");
                return;
            }

            var client = Match.Arguments["CLIENT"] as Client;
            var userName = Match.Arguments["USERNAME"].ToString();

            client.CommandHandler = new PasswordCommandHandler(client, this, userName);
        }

        public void Authenticate(Client Client, String UserName, String Password)
        {
            var existingAccount = Mud.FindAccount(UserName);
            if (existingAccount != null)
            {
                Mud.SendMessage(Client, "Account already exists.");
                return;
            }

            var newAccount = Mud.CreateAccount(UserName, Password);
            if (newAccount == null)
            {
                Mud.SendMessage(Client, "Could not create account.");
                return;
            }

            LoginCommandHandler.LogPlayerIn(Client, newAccount);
        }
    }
}
