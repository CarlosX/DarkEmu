///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
// File info: Public packet data
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace SrxRevo
{
    public partial class Systems
    {
        void HandleAttackPet(byte slot, int ItemID)
        {
            try
            {
                //Check if player level is high enough to spawn.
                if (!CheckItemLevel(Character.Information.Level, ItemID))
                {
                    client.Send(Packet.MoveItemError(0x6C, 0x18));
                }
                //If ok we continue to spawn the attack pet.
                else
                {
                    //Our sql query
                    MsSQL ms = new MsSQL("SELECT * FROM pets WHERE pet_itemid='" + ItemID + "' AND playerid='" + Character.Information.CharacterID + "'");
                    //Create new pet object.
                    pet_obj o = new pet_obj();
                    //Open our data reader
                    using (SqlDataReader reader = ms.Read())
                    {
                        //Start reading data from the query above.
                        while (reader.Read())
                        {
                            Character.Attackpet.Uniqueid = reader.GetInt32(11);
                            Character.Attackpet.Spawned = true;
                            o.UniqueID                  = Character.Attackpet.Uniqueid;
                            o.Model                     = Global.objectdata.GetItem(Data.ItemBase[ItemID].ObjectName);                            o.Level                     = reader.GetByte(13);
                            o.exp                       = reader.GetInt64(14);
                            o.x                         = Character.Position.x + rnd.Next(1, 3);
                            o.z                         = Character.Position.z;
                            o.y                         = Character.Position.y + rnd.Next(1, 3);
                            o.xSec                      = Character.Position.xSec;
                            o.ySec                      = Character.Position.ySec;
                            o.OwnerID                   = Character.Information.CharacterID;
                            o.OwnerName                 = Character.Information.Name;
                            o.Walking                   = Character.Position.Walking;
                            o.Petname                   = reader.GetString(3);
                            o.Named                     = 3;
                            o.Run                       = Character.Speed.RunSpeed;
                            o.Walk                      = Character.Speed.WalkSpeed;
                            o.Zerk                      = Character.Speed.BerserkSpeed;
                        }
                        ms.Close();
                    }
                    //We set our pet active bool, so user cannot spawn multiple.
                    Character.Attackpet.Active = true;
                    o.Information = true;
                    //Set all details above to definitions
                    Character.Attackpet.Details = o;
                    //Global spawn the pet
                    Systems.HelperObject.Add(o);
                    //Spawn ourselfs
                    o.SpawnMe();
                    //Send then packet required (Pet information block).
                    client.Send(Packet.AttackPetStats(o, slot));
                    client.Send(Packet.AttackPetHGP(o));
                    //Update pet status to active (For relog purposes).
                    MsSQL.UpdateData("UPDATE pets SET pet_active='1' WHERE pet_unique='" + Character.Grabpet.Grabpetid + "' AND playerid='" + Character.Information.CharacterID + "'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Attack pet spawn error : " + ex);
            }
        }
    }
}
