///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
// File info: Public packet data
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using System.Collections.Generic;
using GameServer;

namespace DarkEmu_GameServer
{
    #region Old Event System
    public partial class Systems
    {
        public void Gameguide()//Will need to read this byte by byte to get the id for the server to record for the chardata
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                bool Guideok = false;
                int[] b1 = new int[8];
                for (int b = 0; b < 8; ++b)//Reads Guide Data
                {
                    b1[b] = Reader.Byte();//Puts into a int Array
                }

                for (int gc = 0; gc < 8; ++gc)//This Checks The Last Send Guide Paket To Make Sure The Same Packet Is Not Read Twice
                {
                    if (b1[gc] == Character.Guideinfo.Gchk[gc])
                    {
                        Guideok = false;//If Guide Packet Has Been Sent Will Return False
                    }
                    else
                    {
                        Guideok = true;//If Guide Packet Is New Will Retun True And Break
                        break;
                    }
                }

                if (Guideok)
                {
                    for (int gc = 0; gc < 8; ++gc)// Guide Packet Check
                    {
                        Character.Guideinfo.Gchk[gc] = b1[gc];//Adds Packet To Int Array
                    }

                    for (int gi = 0; gi < 8; ++gi)//Guide Packet Update For Save And Teleport,Return,Etc
                    {
                        Character.Guideinfo.G1[gi] = Character.Guideinfo.G1[gi] + b1[gi];//Adds The Packet And Updates The Data
                    }
                    PacketWriter Writer = new PacketWriter();//Writes the Packet Responce For Guide Window
                    Writer.Create(Systems.SERVER_SEND_GUIDE);
                    Writer.Byte(1);
                    for (int b = 0; b < 8; ++b)
                    {
                        Writer.Byte(b1[b]);
                    }
                    client.Send(Writer.GetBytes());
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
     }
    #endregion
    #region New Event System
    public class EventMain
    {
        private Program main;
        public List<ScriptCommand> commands = new List<ScriptCommand>();

        public EventMain()
        {
        }

        public EventMain(string pathtofile)
        {
            string[] file = System.IO.File.ReadAllLines(pathtofile);

            foreach (string s in file)
            {
                ScriptCommand temp = new ScriptCommand(s);
                this.commands.Add(temp);
            }
        }

        private void doCommand(string command, List<string> parameters)
        {
            try
            {
                Console.WriteLine("[EVENT] Command: {0}, Parameters: {1}", command, listToString(parameters));

                switch (NameToEnum(command))
                {
                    case ScriptCommandType.Sleep:
                        Thread.Sleep(Int32.Parse(parameters[0]));
                        break;
                    case ScriptCommandType.SpawnUnique:
                        this.SpawnUnique(parameters);
                        break;
                    case ScriptCommandType.Notice:
                        Systems.SendAll(Packet.ChatPacket(7, 0, listToString(parameters, false), ""));
                        break;
                    case ScriptCommandType.SpawnMob:
                        this.spawnMob(parameters);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("exception at handling command: Type = {0} ::: Parameters = {1}", command, listToString(parameters));
            }
        }

        private void SpawnUnique(List<string> parameters)
        {
            obj o = new obj();
            switch ((Uniques)Int32.Parse(parameters[0]))
            {
                case Uniques.TigerGirl: // INSERT SPAWN CODE HERE
                    o = getMob(1954, parameters);
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    break;
                case Uniques.Urichi:
                    o = getMob(1982, parameters);
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    break;
                case Uniques.Isyutaru:
                    o = getMob(2002, parameters);
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    break;
                case Uniques.LordYarkan:
                    o = getMob(3810, parameters);
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    break;
                case Uniques.DemonShaitan:
                    o = getMob(3875, parameters);
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    break;
                case Uniques.Cerberus:
                    o = getMob(5871, parameters);
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    break;
                case Uniques.CapIvy:
                    o = getMob(14778, parameters);
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    break;
                case Uniques.Medusa:
                    o = getMob(14839, parameters);
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    break;
                case Uniques.Roc:
                    o = getMob(3877, parameters);
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    break;
                case Uniques.Neith:
                    o = getMob(32768, parameters);
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    break;
                case Uniques.Isis:
                    o = getMob(32770, parameters);
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    break;
                case Uniques.Sphinx:
                    o = getMob(32752, parameters);
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    break;
            }
        }

        private obj getMob(int id, List<string> param)
        {
            short AREA = short.Parse(param[1]);
            float x = Convert.ToInt32(param[2]);
            float z = Convert.ToInt32(param[3]);
            float y = Convert.ToInt32(param[4]);
            obj o = new obj();
            o.AutoMovement = true;
            o.LocalType = Data.ObjectBase[id].Type;
            o.OrgMovement = o.AutoMovement;
            o.StartRunTimer(Global.RandomID.GetRandom(3000, 8000));
            o.ID = id;
            o.Ids = new Global.ID(Global.ID.IDS.Object);
            o.UniqueID = o.Ids.GetUniqueID;

            o.xSec = Convert.ToByte((AREA).ToString("X4").Substring(2, 2), 16);
            o.ySec = Convert.ToByte((AREA).ToString("X4").Substring(0, 2), 16);
            o.x = (o.xSec - 135) * 192 + (x) / 10;
            o.z = z;
            o.y = (o.ySec - 92) * 192 + (y) / 10;

            o.oX = o.x;
            o.oY = o.y;
            Systems.aRound(ref o.oX, ref o.oY, 9);
            o.State = 1;
            o.Move = 1;
            o.AutoSpawn = true;
            o.State = 2;
            o.HP = Data.ObjectBase[id].HP;
            o.Kat = 1;
            o.Agro = new List<_agro>();
            o.SpeedWalk = Data.ObjectBase[o.ID].SpeedWalk;
            o.SpeedRun = Data.ObjectBase[o.ID].SpeedRun;
            o.SpeedZerk = Data.ObjectBase[o.ID].SpeedZerk;
            o.oldAgresif = o.Agresif;
            if (o.Type == 1) o.Agresif = 1;
            //if (o.Type == 0) o.Agresif = 0;
            o.spawnOran = 20;
            if (id == 1979 || id == 2101 || id == 2124 || id == 2111 || id == 2112) o.AutoMovement = false;
            o.OrgMovement = o.AutoMovement;

            if (o.AutoMovement) o.StartRunTimer(Global.RandomID.GetRandom(3000, 8000));
            o.Type = Systems.RandomType(Data.ObjectBase[id].Level, ref o.Kat, false, ref o.Agresif);
            o.HP *= o.Kat;
            if (o.Type == 1)
                o.Agresif = 1;
            return o;
        }

        public void Start()
        {
            Thread th = new Thread(new ThreadStart(this.RunLoop));
            th.Start();
        }

        public void RunLoop()
        {
            try
            {
                foreach (ScriptCommand command in this.commands)
                {
                    this.doCommand(command.CommandType, command.Parameters);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception at running command. {0}", e.ToString());
            }
        }

        private string listToString(List<string> list, bool seperate = true)
        {
            string strings = "";
            foreach (string s in list)
            {
                strings += s;
                strings += (seperate) ? "," : "";
            }

            if (seperate)
                strings = strings.Substring(0, strings.Length - 1);
            return strings;
        }

        private void spawnMob(List<string> parameters)
        {
            obj o = this.getMob(Int32.Parse(parameters[0]), parameters);
            Systems.Objects.Add(o);
            o.SpawnMe();
        }

        public ScriptCommandType NameToEnum(string name)
        {
            switch (name.ToLower())
            {
                case "spawn":
                case "mob":
                case "spawnmob":
                case "2":
                    return ScriptCommandType.SpawnMob;
                case "notice":
                case "sendnotice":
                case "4":
                    return ScriptCommandType.Notice;
                case "unique":
                case "spawnunique":
                case "3":
                    return ScriptCommandType.SpawnUnique;
                case "wait":
                case "sleep":
                case "1":
                    return ScriptCommandType.Sleep;
                case "0":
                    return ScriptCommandType.Nothing;
                default:
                    return ScriptCommandType.Nothing;
            }
        }
    }

    public enum ScriptCommandType
    {
        Nothing = 0,
        Sleep = 1,
        SpawnMob = 2,
        SpawnUnique = 3,
        Notice = 4
    }

    public enum Uniques
    {
        TigerGirl = 1,
        Urichi = 2,
        Isyutaru = 3,
        LordYarkan = 4,
        DemonShaitan = 5,
        Cerberus = 6,
        CapIvy = 7,
        Medusa = 8,
        Roc = 9,
        Neith = 10,
        Isis = 11,
        Sphinx = 12
    }
    public class ScriptCommand
    {
        private string _commandType = "";
        private List<string> _parameters = new List<string>();

        public string CommandType
        {
            get { return this._commandType; }
            private set { this._commandType = value; }
        }

        public List<string> Parameters
        {
            get { return this._parameters; }
            private set { this._parameters = value; }
        }

        public ScriptCommand()
        {
        }

        public ScriptCommand(string commandLine)
        {
            ScriptCommand temp = Parse(commandLine);
            this.Parameters = temp.Parameters;
            this.CommandType = temp.CommandType;
        }

        public ScriptCommand(string type, List<string> parm)
        {
            this.CommandType = type;
            this.Parameters = parm;
        }

        public ScriptCommand Parse(string line)
        {
            string[] linesplitt = null;
            string type = "";
            List<string> param = new List<string>();
            try
            {
                linesplitt = line.Split(',');
                type = linesplitt[0];
                for (int i = 1; i < linesplitt.Length; i++)
                {
                    param.Add(linesplitt[i]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Couldn't parse command. {0}", e.ToString());
            }
            return new ScriptCommand(type, param);
        }

    }
    #endregion
}