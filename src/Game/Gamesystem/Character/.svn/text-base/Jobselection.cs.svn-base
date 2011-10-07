///////////////////////////////////////////////////////////////////////////
// SRX Revo: Character job selection
// Created by: http://xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;

namespace SrxRevo
{
    public partial class Systems
    {
        void CharacterJobInfo()
        {
            //Wrap our function into catcher
            try
            {
                //Set defaul int's to 0
                int TotalHunters = 0;
                int TotalThiefs = 0;
                //Update hunter and thief count.
                TotalHunters = MsSQL.GetRowsCount("SELECT * FROM users WHERE jobtype='1'");
                TotalThiefs = MsSQL.GetRowsCount("SELECT * FROM users WHERE jobtype='2'");
                //Set total count
                int jobplayercount = TotalThiefs + TotalHunters;
                //Set our null to 1 if this would happen
                if (jobplayercount == 0) jobplayercount = 1;
                //Calculate our % for the jobs
                double thiefpercentage = (double)TotalThiefs / (double)jobplayercount * 100.0;
                double hunterpercentage = (double)TotalHunters / (double)jobplayercount * 100.0;
                //Send visual packet for job %
                PacketWriter writer = new PacketWriter();
                //Add opcode
                writer.Create(Systems.SERVER_CHARACTERSCREEN);
                //Static byte 9
                writer.Byte(9);
                //Static byte 1
                writer.Byte(1);
                //Byte total amount of hunters %
                writer.Byte((byte)hunterpercentage);
                //Byte total amount of thiefs %
                writer.Byte((byte)thiefpercentage);
                //Send bytes to client
                client.Send(writer.GetBytes());
            }
            //Catch bad exception errors
            catch (Exception ex)
            {
                //Write information to the console
                Console.WriteLine("Character job info error {0}", ex);
                //Write info to the debug log
                Systems.Debugger.Write(ex);
            }
        }

        void CharacterJobPick(byte[] buff)
        {
            //Wrap our function inside a catcher
            try
            {
                //Create new packet reader
                PacketReader Reader = new PacketReader(buff);
                //Skip first byte not used
                Reader.Skip(1);
                //Read name lenght
                short CharacterNameLEN = Reader.Int16();
                //Read character name
                string CharacterName = Reader.String(CharacterNameLEN);
                //Read job selection
                byte job = Reader.Byte();
                //Close packet reader
                Reader.Close();
                //Get row count from character to check if the current account and character match
                int NameCheck = MsSQL.GetRowsCount("SELECT * FROM character WHERE account='" + Player.AccountName + "'");
                //Get job information from database as integer
                int jobcheck = MsSQL.GetDataInt("SELECT jobtype FROM users WHERE id='" + Player.AccountName + "'", "jobtype");
                //If the name check is succesfull and account has no job set.
                if (jobcheck == 0 && NameCheck != 0)
                {
                    //Write new job information to the database
                    MsSQL.UpdateData("UPDATE users SET jobtype='" + job + "' WHERE id='" + Player.AccountName + "'");
                }
                //Send visual confirmation in packet
                PacketWriter writer = new PacketWriter();
                //Add opcode
                writer.Create(Systems.SERVER_CHARACTERSCREEN);
                //Write static byte 10
                writer.Byte(0x10);
                //Write succes byte 1
                writer.Byte(1);
                //Send bytes to client
                client.Send(writer.GetBytes());
            }
            //Catch any bad exception error
            catch (Exception ex)
            {
                //Write error information to the console
                Console.WriteLine("Job selection error {0}", ex);
                //Write error to debug log file
                Systems.Debugger.Write(ex);
            }
        }
    }
}