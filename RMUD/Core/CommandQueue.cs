﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public static partial class Mud
    {
        private class PendingCommand
        {
            internal Client Client;
            internal String RawCommand;
        }

        private static Mutex PendingCommandLock = new Mutex();
        private static LinkedList<PendingCommand> PendingCommands = new LinkedList<PendingCommand>();
        private static Thread CommandExecutionThread;
        private static Thread IndividualCommandThread;
        private static AutoResetEvent CommandReadyHandle = new AutoResetEvent(false);
        private static AutoResetEvent CommandFinishedHandle = new AutoResetEvent(false);
        private static PendingCommand NextCommand;

        //The client command handler can set this flag when it wants the command timeout to be ignored.
        public static bool CommandTimeoutEnabled = true;


        internal static ParserCommandHandler ParserCommandHandler;
        public static CommandParser Parser { get { return ParserCommandHandler.Parser; } }
        internal static LoginCommandHandler LoginCommandHandler;

        internal static void EnqueuClientCommand(Client Client, String RawCommand)
        {
            PendingCommandLock.WaitOne();
            PendingCommands.AddLast(new PendingCommand { Client = Client, RawCommand = RawCommand });
            PendingCommandLock.ReleaseMutex();
        }

        private static void InitializeCommandProcessor()
        {
            ParserCommandHandler = new ParserCommandHandler();
            LoginCommandHandler = new LoginCommandHandler();

            CommandExecutionThread = new Thread(ProcessCommands);
            CommandExecutionThread.Start();
        }

        private static void ProcessIndividualCommand()
        {
            while (!ShuttingDown)
            {
                CommandReadyHandle.WaitOne();

                try
                {
                    NextCommand.Client.TimeOfLastCommand = DateTime.Now;
                    NextCommand.Client.CommandHandler.HandleCommand(NextCommand.Client, NextCommand.RawCommand);
                }
                catch (System.Threading.ThreadAbortException e)
                {
                    LogError("Command worker thread was aborted. Timeout hit?");
                    ClearPendingMessages();
                }
                catch (Exception e)
                {
                    LogCommandError(e);
                    ClearPendingMessages();
                }

                NextCommand = null;

                CommandFinishedHandle.Set();
            }
        }
                
        private static void ProcessCommands()
        {
            IndividualCommandThread = new Thread(ProcessIndividualCommand);
            IndividualCommandThread.Start();

            while (!ShuttingDown)
            {
                System.Threading.Thread.Sleep(10);
                DatabaseLock.WaitOne();
                Heartbeat();
                DatabaseLock.ReleaseMutex();

                while (PendingCommands.Count > 0)
                {
                    PendingCommand PendingCommand = null;

                    PendingCommandLock.WaitOne();

                    try
                    {
                        PendingCommand = PendingCommands.FirstOrDefault(pc => (DateTime.Now - pc.Client.TimeOfLastCommand).TotalMilliseconds > SettingsObject.AllowedCommandRate);
                        if (PendingCommand != null)
                            PendingCommands.Remove(PendingCommand);
                    }
                    catch (Exception e)
                    {
                        LogCommandError(e);
                        PendingCommand = null;
                    }

                    PendingCommandLock.ReleaseMutex();

                    if (PendingCommand != null)
                    {
                        DatabaseLock.WaitOne();

                        NextCommand = PendingCommand;
                        CommandTimeoutEnabled = true;
                        CommandReadyHandle.Set(); //Signal worker thread to proceed.
                        if (CommandFinishedHandle.WaitOne(SettingsObject.CommandTimeOut))
                        {
                            UpdateMarkedObjects();
                            SendPendingMessages();
                        }
                        else
                        {
                            if (!CommandTimeoutEnabled)
                            {
                                //Timeout is disabled, go ahead and wait for infinity.
                                CommandFinishedHandle.WaitOne();
                                UpdateMarkedObjects();
                                SendPendingMessages();
                            }
                            else
                            {
                                //Kill the command processor thread.
                                IndividualCommandThread.Abort();
                                PendingCommand.Client.Send("Command timeout.\r\n");
                                Mud.LogError(String.Format("Command timeout. {0} - {1}", PendingCommand.Client.IPString, PendingCommand.RawCommand));
                                IndividualCommandThread = new Thread(ProcessIndividualCommand);
                                IndividualCommandThread.Start();
                            }
                        }
                                                
                        DatabaseLock.ReleaseMutex();
                    }
                }
            }
        }
    }
}
