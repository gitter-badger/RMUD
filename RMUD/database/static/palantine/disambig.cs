﻿
public class disambig : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Palantine Villa - Hall of Ambiguity";
        Long = "This room might be round. It is roundish, at the very least. It is very hard to tell, what with how all of the walls are covered, floor to ceiling, in mirrors. Your only point of reference in the place is the doors on opposite walls. There are thousands of them, reflected here and there and everywhere.";

        AddScenery("Which do you mean?", "MIRROR", "MIRRORS");

        OpenLink(RMUD.Direction.WEST, "palantine\\library", RMUD.Mud.GetObject("palantine\\disambig_blue_door"));
        OpenLink(RMUD.Direction.EAST, "palantine\\dark_room", RMUD.Mud.GetObject("palantine\\disambig_red_door"));
        OpenLink(RMUD.Direction.SOUTH, "palantine\\antechamber");

        RMUD.MudObject.Move(RMUD.Mud.GetObject("palantine\\disambig_key"), this);
        RMUD.MudObject.Move(RMUD.Mud.GetObject("palantine\\library_key"), this);
        RMUD.MudObject.Move(new torch(), this);
        RMUD.MudObject.Move(RMUD.Mud.GetObject("palantine/skull"), this);
	}
}

public class torch : RMUD.MudObject, RMUD.EmitsLight
{
    public torch()
    {
        Short = "torch";
        Nouns.Add("torch");
    }

    RMUD.LightingLevel RMUD.EmitsLight.EmitsLight
    {
        get { return RMUD.LightingLevel.Bright; }
    }
}
