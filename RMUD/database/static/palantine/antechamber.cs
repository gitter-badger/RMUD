﻿public class antechamber : RMUD.Room
{
	public override void Initialize()
	{
        RoomType = RMUD.RoomType.Interior;
        Short = "Palantine Villa - Antechamber";
        Long = "Two imposing statues stand guard in this small room, on either side of the door to the room beyond. On the left, Jupiter, king of the gods. On the right, Minerva, the goddess of wisdom and beauty.";

        AddScenery(new Jupiter());
        AddScenery("Minerva is turned to regard her father Jupiter, and poses with one hand on her hips and the other on the shaft of a massive hammer.", "minerva");

        var table = new Table();
        RMUD.MudObject.Move(table, this);

        RMUD.MudObject.Move(new RMUD.MudObject("old vase", "An old, cracked vase."), table);


        OpenLink(RMUD.Direction.NORTH, "palantine/disambig");
        OpenLink(RMUD.Direction.EAST, "palantine/solar");
        OpenLink(RMUD.Direction.WEST, "palantine/garden");
    }
}

public class Jupiter : RMUD.Scenery, RMUD.EmitsLight
{
    public Jupiter()
    {
        Nouns.Add("jupiter");
        Long = "Jupiter holds in his left hand a gleaming thunderbolt. It glows bright enough to light the entire chamber. In his right, he holds a chisel.";
        RMUD.Mud.RegisterForHeartbeat(this);
    }

    public RMUD.LightingLevel EmitsLight
    {
        get { return RMUD.LightingLevel.Bright; }
    }

    public override void Heartbeat(ulong HeartbeatID)
    {
        RMUD.Mud.SendLocaleMessage(this, string.Format("Jupiter's thunderbolt sparkles for the {0}th time.\r\n", HeartbeatID));
    }
}

public class Table : RMUD.GenericContainer, RMUD.OpenableRules, RMUD.TakeRules
{
    public Table() : base(RMUD.RelativeLocations.On | RMUD.RelativeLocations.Under, RMUD.RelativeLocations.On)
    {
        Short = "ancient table";
        Long = "As the years have worn long the wood of this table has dried and shrunk, and split, and what was once a finely crafted table is now pitted and gouged. The top is still mostly smooth, from use but not from care.";
        Nouns.Add("ancient", "table");

        Open = false;

        RMUD.MudObject.Move(new RMUD.MudObject("matchbook", "A small book of matches with a thunderbolt on the cover."), this, RMUD.RelativeLocations.Under);
    }

    public override string Indefinite
    {
        get
        {
            return "an ancient table";
        }
    }

    #region OpenableRules

    public bool Open { get; set; }

    RMUD.CheckRule RMUD.OpenableRules.CheckOpen(RMUD.Actor Actor)
    {
        return RMUD.CheckRule.Allow();
    }

    RMUD.CheckRule RMUD.OpenableRules.CheckClose(RMUD.Actor Actor)
    {
        return RMUD.CheckRule.Allow();
    }

    RMUD.RuleHandlerFollowUp RMUD.OpenableRules.HandleOpen(RMUD.Actor Actor)
    {
        return RMUD.RuleHandlerFollowUp.Continue;
    }

    RMUD.RuleHandlerFollowUp RMUD.OpenableRules.HandleClose(RMUD.Actor Actor)
    {
        return RMUD.RuleHandlerFollowUp.Continue;
    }

    #endregion

    RMUD.CheckRule RMUD.TakeRules.Check(RMUD.Actor Actor)
    {
        return RMUD.CheckRule.Disallow("It's far too heavy.");
    }

    RMUD.RuleHandlerFollowUp RMUD.TakeRules.Handle(RMUD.Actor Actor)
    {
        throw new System.NotImplementedException();
    }
}
