﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class BasicDoor : Portal, OpenableRules
    {
        public BasicDoor()
        {
            this.Nouns.Add("DOOR", "CLOSED");
            Open = false;
        }

        #region IOpenable

        public bool Open { get; set; }

        CheckRule OpenableRules.CheckOpen(Actor Actor)
        {
            if (Open) return CheckRule.Disallow("It's already open.");
            else return CheckRule.Allow();
        }

        CheckRule OpenableRules.CheckClose(Actor Actor)
        {
            if (Open) return CheckRule.Allow();
            else return CheckRule.Disallow("It's already closed.");
        }

        protected RuleHandlerFollowUp ImplementHandleOpen(Actor Actor)
        { 
            Open = true;
            Nouns.RemoveAll(n => n == "CLOSED");
            Nouns.Add("OPEN");

            var location = Actor.Location as Room;
            var otherSide = this.OppositeSide(location);
            if (otherSide != null)
            {
                Mud.SendLocaleMessage(otherSide as Room, String.Format("{0} opens {1}.", Actor.Short, this.Definite));
                Mud.MarkLocaleForUpdate(otherSide);
            }

            return RuleHandlerFollowUp.Continue;
        }

        RuleHandlerFollowUp OpenableRules.HandleOpen(Actor Actor)
        {
            return ImplementHandleOpen(Actor);
        }

        protected RuleHandlerFollowUp ImplementHandleClose(Actor Actor)
        {
            Open = false;
            Nouns.RemoveAll(n => n == "OPEN");
            Nouns.Add("CLOSED");

            var location = Actor.Location as Room;
            var otherSide = this.OppositeSide(location);
            if (otherSide != null)
            {
                Mud.SendLocaleMessage(otherSide, String.Format("{0} closes {1}.", Actor.Short, this.Definite));
                Mud.MarkLocaleForUpdate(otherSide);
            }

            return RuleHandlerFollowUp.Continue;
        }


        RuleHandlerFollowUp OpenableRules.HandleClose(Actor Actor)
        {
            return ImplementHandleClose(Actor);
        }

        #endregion
    }
}
