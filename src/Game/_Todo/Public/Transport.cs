///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Special Transports / Unicorns etc
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        bool HandleSpecialTrans(int ItemID)
        {
            try
            {
                int model = Global.objectdata.GetItem(Data.ItemBase[ItemID].ObjectName);
                if (this.Character.Information.Level < Data.ItemBase[ItemID].Level) return true;

                {
                    model = Global.objectdata.GetItem(Data.ItemBase[ItemID].ObjectName);
                    if (model == 0) return true;
                }
                pet_obj o = new pet_obj();
                o.Model = model;
                o.Ids = new Global.ID(Global.ID.IDS.Object);
                o.UniqueID = o.Ids.GetUniqueID;
                o.x = Character.Position.x;
                o.z = Character.Position.z;
                o.y = Character.Position.y;
                o.xSec = Character.Position.xSec;
                o.ySec = Character.Position.ySec;
                o.Hp = Data.ObjectBase[model].HP;
                o.OwnerID = this.Character.Information.UniqueID;

                this.Character.Transport.Right = true;

                List<int> S = o.SpawnMe();
                o.Information = true;
                client.Send(Packet.Pet_Information(o.UniqueID, o.Model, o.Hp, Character.Information.CharacterID, o));
                Send(S, Packet.Player_UpToHorse(this.Character.Information.UniqueID, true, o.UniqueID));
                Systems.HelperObject.Add(o);
                this.Character.Transport.Horse = o;
                return false;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return false;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Normal Transport
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        bool HandleHorseScroll(int ItemID)
        {
            try
            {
                int model = Global.objectdata.GetItem(Data.ItemBase[ItemID].ObjectName);
                if (model == 0)
                {
                    string extrapath = null;
                    if (this.Character.Information.Level >= 1 && this.Character.Information.Level <= 5)
                        extrapath = "_5";
                    else if (this.Character.Information.Level >= 6 && this.Character.Information.Level <= 10)
                        extrapath = "_10";
                    else if (this.Character.Information.Level >= 11 && this.Character.Information.Level <= 20)
                        extrapath = "_20";
                    else if (this.Character.Information.Level >= 21 && this.Character.Information.Level <= 30)
                        extrapath = "_30";
                    else if (this.Character.Information.Level >= 31 && this.Character.Information.Level <= 45)
                        extrapath = "_45";
                    else if (this.Character.Information.Level >= 46 && this.Character.Information.Level <= 60)
                        extrapath = "_60";
                    else if (this.Character.Information.Level >= 61 && this.Character.Information.Level <= 75)
                        extrapath = "_75";
                    else if (this.Character.Information.Level >= 76 && this.Character.Information.Level <= 90)
                        extrapath = "_90";
                    else if (this.Character.Information.Level >= 91 && this.Character.Information.Level <= 105)
                        extrapath = "_105";
                    else if (this.Character.Information.Level >= 106 && this.Character.Information.Level <= 120)
                        extrapath = "_120";
                    model = Global.objectdata.GetItem(Data.ItemBase[ItemID].ObjectName + extrapath);
                    if (model == 0) return true;
                }
                pet_obj o = new pet_obj();
                o.Model = model;
                o.Ids = new Global.ID(Global.ID.IDS.Object);
                o.UniqueID = o.Ids.GetUniqueID;
                o.x = Character.Position.x;
                o.z = Character.Position.z;
                o.y = Character.Position.y;
                o.xSec = Character.Position.xSec;
                o.ySec = Character.Position.ySec;
                o.Hp = Data.ObjectBase[model].HP;
                o.OwnerID = this.Character.Information.UniqueID;
                o.Speed1 = Data.ObjectBase[model].Speed1;
                o.Speed2 = Data.ObjectBase[model].Speed2;
                this.Character.Transport.Right = true;

                List<int> S = o.SpawnMe();
                o.Information = true;
                client.Send(Packet.Pet_Information(o.UniqueID, o.Model, o.Hp, Character.Information.CharacterID, o));
                Send(Packet.SetSpeed(o.UniqueID, o.Speed1, o.Speed2));//Global Speed Update
                Send(Packet.ChangeStatus(o.UniqueID, 3, 0));// Global Status 
                Send(S, Packet.Player_UpToHorse(this.Character.Information.UniqueID, true, o.UniqueID));

                Systems.HelperObject.Add(o);
                this.Character.Transport.Horse = o;
                return false;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return false;
        }

        bool HandleJobTransport(int ItemID)
        {
            try
            {
                int model = Global.objectdata.GetItem(Data.ItemBase[ItemID].ObjectName);
                if (this.Character.Information.Level < Data.ItemBase[ItemID].Level) return true;

                {
                    model = Global.objectdata.GetItem(Data.ItemBase[ItemID].ObjectName);
                    if (model == 0) return true;
                }
                pet_obj o = new pet_obj();
                o.Model = model;
                o.Named = 4;
                o.Ids = new Global.ID(Global.ID.IDS.Object);
                o.UniqueID = o.Ids.GetUniqueID;
                o.x = Character.Position.x;
                o.z = Character.Position.z;
                o.y = Character.Position.y;
                o.xSec = Character.Position.xSec;
                o.ySec = Character.Position.ySec;
                o.Hp = Data.ObjectBase[model].HP;
                o.OwnerID = this.Character.Information.UniqueID;
                o.OwnerName = Character.Information.Name;
                this.Character.Transport.Right = true;

                o.Information = true;
                //client.Send(Packet.Pet_Information(o.UniqueID, o.Model, o.Hp, Character.Information.CharacterID, o));
                Send(Packet.Player_UpToHorse(this.Character.Information.UniqueID, true, o.UniqueID));
                Systems.HelperObject.Add(o);
                this.Character.Transport.Horse = o;
                return false;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return false;
        }
    }
}