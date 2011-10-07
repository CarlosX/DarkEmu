///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        /////////////////////////////////////////////////////////////////////////
        // Union Apply
        /////////////////////////////////////////////////////////////////////////
        void unionapply()
        {
            try
            {
                //Open our packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Get target id (Targeted).
                int Target = Reader.Int32();
                //Close reader
                Reader.Close();

                //Get target details
                Systems targetplayer = GetPlayer(Target);
                //Make sure the target is still there
                if (targetplayer != null)
                {
                    //If allready in union
                    if (targetplayer.Character.Network.Guild.UnionActive) return;
                    //Set bools for both players
                    targetplayer.Character.State.UnionApply = true;
                    this.Character.State.UnionApply = true;
                    //Set target player to us
                    targetplayer.Character.Network.TargetID = this.Character.Information.UniqueID;
                    //Send request to targeted player
                    targetplayer.client.Send(Packet.PartyRequest(6, this.Character.Information.UniqueID, 0));
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
    }
}