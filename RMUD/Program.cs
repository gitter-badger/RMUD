﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace RMUD
{
    class Program
    {
		internal class CommandLineOptions
		{
			public String DATABASEPATH { get; set; }

			public CommandLineOptions()
			{
				DATABASEPATH = "database/";
			}
		}

        static void Main(string[] args)
        {
            var commandLineOptions = new CommandLineOptions();
            var error = CommandLine.ParseCommandLine(commandLineOptions);

            if (error != CommandLine.Error.Success)
            {
                Console.WriteLine("Command line error: " + error);
                return;
            }

            TelnetClientSource telnetListener = null;

            if (Mud.Start(commandLineOptions.DATABASEPATH))
            {
                telnetListener = new TelnetClientSource();
                telnetListener.Port = Mud.SettingsObject.TelnetPort;
                telnetListener.Listen();
            }

            if (Mud.SettingsObject.UseConsoleCommands)
            {
                try
                {
                    while (true)
                    {
                        var command = Console.ReadLine();

                        if (command.ToUpper() == "STOP")
                            break;
                        else if (command.ToUpper() == "CLIENTS")
                        {
                            Mud.DatabaseLock.WaitOne();

                            foreach (var client in Mud.ConnectedClients)
                            {
                                Console.Write(client.ConnectionDescription);
                                if (client.Player != null)
                                {
                                    Console.Write(" -- ");
                                    Console.Write(client.Player.Short);
                                }
                                Console.WriteLine();
                            }

                            Mud.DatabaseLock.ReleaseMutex();
                        }
                        else if (command.ToUpper() == "MEMORY")
                        {
                            var mem = System.GC.GetTotalMemory(false);
                            var kb = mem / 1024.0f;
                            Console.WriteLine("Memory usage: " + String.Format("{0:n0}", kb) + " kb");
                            Console.WriteLine("Named objects loaded: " + Mud.NamedObjects.Count);
                        }
                        else if (command.ToUpper() == "HEARTBEAT")
                        {
                            Mud.DatabaseLock.WaitOne();
                            Console.WriteLine("Heartbeat interval: {0} Objects: {1} HID: {2}",
                                Mud.SettingsObject.HeartbeatInterval,
                                Mud.ObjectsRegisteredForHeartbeat.Count,
                                Mud.HeartbeatID);
                            foreach (var Object in Mud.ObjectsRegisteredForHeartbeat)
                                Console.WriteLine(Object.ToString());
                            Mud.DatabaseLock.ReleaseMutex();
                        }
                        else if (command.ToUpper() == "TIME")
                        {
                            Mud.DatabaseLock.WaitOne();
                            Console.WriteLine("Current time in game: {0}", Mud.TimeOfDay);
                            Console.WriteLine("Advance rate: {0} per heartbeat",
                                Mud.SettingsObject.ClockAdvanceRate);
                            Mud.DatabaseLock.ReleaseMutex();
                        }
                        else if (command.ToUpper() == "SAVE")
                        {
                            Mud.DatabaseLock.WaitOne();
                            Console.Write("Saving persistent instances to file...");
                            var totalInstances = Mud.SaveActiveInstances();
                            Console.WriteLine(totalInstances + " instances saved");
                        }
                        else if (command.ToUpper() == "DEBUGON")
                        {
                            Mud.CommandTimeoutEnabled = false;
                            Console.WriteLine("Debugging mode enabled");
                        }
                        else if (command.ToUpper() == "DEBUGOFF")
                        {
                            Mud.CommandTimeoutEnabled = true;
                            Console.WriteLine("Debugging mode disabled");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR:", e.Message, e.Source, e.StackTrace, e.Data);
                }

            }
            else
            {
                while (true) 
                { 
                    //Todo: Shutdown server command breaks this loop.
                }
            }

            telnetListener.Shutdown();
            Mud.Shutdown();
        }
    }
}
