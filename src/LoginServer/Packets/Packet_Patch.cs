///////////////////////////////////////////////////////////////////////////
// Copyright (C) <2011>  <DarkEmu>
// HOMEPAGE: WWW.XFSGAMES.COM.AR
///////////////////////////////////////////////////////////////////////////
using System;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace LoginServer
{
    public partial class Systems
    {
        /*
        public static byte[] PatchRequest()
        {
            
            PacketWriter Writer = new PacketWriter();
            Writer.Create(SERVER.SERVER_MAIN);
            Writer.Byte(2);
            Writer.Byte(2);
            Writer.Text(Systems.DownloadServer);
            Writer.Word(Systems.DownloadPort);
            Writer.DWord(LoginServer.Global.Versions.clientVersion);

            //List<Content.Patch> Patch = Content.Patches.GetPatches(Convert.ToUInt16(LoginServer.Global.Versions.clientVersion));

            for (int i = 0; i < Patch.Count; i++)
            {
                Writer.Byte(0x01); // new file;
                Writer.DWord(Patch[i].FileID);
                Writer.Text(Patch[i].Name);
                Writer.Text(Patch[i].Path);
                Writer.DWord(Patch[i].Size);
                Writer.Bool(Patch[i].Pk2Compressed);
            }

            Writer.Byte(0x00); // End
            return Writer.GetBytes();
             
        }*/
    }
}
