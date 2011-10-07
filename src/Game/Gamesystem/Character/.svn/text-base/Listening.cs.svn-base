///////////////////////////////////////////////////////////////////////////
// SRX Revo: Character listening
// Created by: http://xcoding.net
///////////////////////////////////////////////////////////////////////////

using System;
using Framework;

namespace SrxRevo
{
    public partial class Systems
    {
        //Listening main uses a single packet only so we write our packet here
        public static byte[] CharacterListing(player p)
        {
            //Create new sql query to get all characters except fully deleted
            Systems.MsSQL ms = new Systems.MsSQL("SELECT TOP 4 * FROM character WHERE account='" + p.AccountName + "' AND deleted='0' OR account='" + p.AccountName + "' AND deleted ='1'");
            //Create new packet writer
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_CHARACTERSCREEN);      // Select opcode
            Writer.Byte(2);                                     // Static byte 2
            Writer.Byte(1);                                     // Static byte 1
            Writer.Byte(ms.Count());                            // Byte Character Count
            //Create new sql data reader
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                //While the reader is reading data from the database
                while (reader.Read())
                {
                    Writer.DWord(reader.GetInt32(3));           // DWord Skin Model
                    Writer.Text(reader.GetString(2));           // String Name
                    Writer.Byte(reader.GetByte(4));             // Byte Skin Volume
                    Writer.Byte(reader.GetByte(5));             // Byte Level
                    Writer.LWord(reader.GetInt64(12));          // Long Experience
                    Writer.Word(reader.GetInt16(6));            // Word STR
                    Writer.Word(reader.GetInt16(7));            // Word INT
                    Writer.Word(reader.GetInt16(8));            // Attribute points
                    Writer.DWord(reader.GetInt32(9));           // HP
                    Writer.DWord(reader.GetInt32(10));          // MP
                    //Set defnition for current time
                    DateTime CurrentTime = DateTime.Now;
                    //Subtract the current time with deletion time information
                    TimeSpan TIMESPAN = reader.GetDateTime(43) - DateTime.Now;
                    //If the character has been deleted but still sitting
                    if (reader.GetByte(42) == 1)
                    {
                        //Write state byte 1 for sitting
                        Writer.Byte(1);
                        //Display deletion time reamining
                        Writer.DWord((double)TIMESPAN.TotalMinutes);
                        //If time has expired delete the character
                        if (TIMESPAN.TotalMinutes <= 0)
                        {
                            //Note: There are more ways like full delete but if someone got deleted by someone else's action
                            //It will be easy to recover the character by changing the deletion byte in the database
                            //0 = Not deleted , 1 = Deletion in progress, 2 = Deleted

                            //Set deletion state so it wont be listed again.
                            Systems.MsSQL.UpdateData("UPDATE character SET deleted='2' Where id='" + reader.GetInt32(0) + "'");
                        }
                    }
                    //If not deleted
                    else
                    {
                        //State byte = 0 Standing
                        Writer.Byte(0);
                    }
                    //Static
                    Writer.Word(0);
                    Writer.Byte(0);
                    //Get normal equipped items upto slot 8 not in avatar slots
                    Function.Items.PrivateItemPacket(Writer, reader.GetInt32(0), 8, 0, false);
                    //Get avatar type items
                    Function.Items.PrivateItemPacket(Writer, reader.GetInt32(0), 5, 1, false);
                }
                //Jobtype information
                int jobinfo = Systems.MsSQL.GetDataInt("SELECT * FROM users WHERE id='" + p.AccountName + "'", "jobtype");
                Writer.Byte(Convert.ToByte(jobinfo));
            }
            ms.Close();
            //Return all bytes to the client
            return Writer.GetBytes();
        }
    }
}