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
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public partial class GlobalUnique
    {

        public static List<obj> TigerGirl = new List<obj>();
        public static List<obj> Urichi = new List<obj>();
        public static List<obj> Isytaru = new List<obj>();
        public static List<obj> LordYarkan = new List<obj>();
        public static List<obj> DemonShaitan = new List<obj>();
        public static List<obj> Cerberus = new List<obj>();
        public static List<obj> CapIvy = new List<obj>();
        public static List<obj> Medusa = new List<obj>();
        public static List<obj> Roc = new List<obj>();
        public static List<obj> Neith = new List<obj>();
        public static List<obj> Isis = new List<obj>();
        public static List<obj> Sphinx = new List<obj>();
        public static bool Tiger, Uri, Isy, Lord, Demon, Cerb, Ivy, Medusa_s, Roc_s, Neith_s, Isis_s, Sphinx_s;

        static Random rnd = new Random();

        public static Timer tiger1;
        public static Timer uri1;
        public static Timer isy1;
        public static Timer lord1;
        public static Timer demon1;
        public static Timer cerb1;
        public static Timer ivy1;
        public static Timer medusa1;
        public static Timer roc1;
        public static Timer neith1;
        public static Timer isis1;
        public static Timer sphinx1;

        public static void StartNeith(int timer, int per)
        {
            neith1 = new Timer(new TimerCallback(NEITH_CB), 0, timer, per);
        }
        public static void StartIsis(int timer, int per)
        {
            isis1 = new Timer(new TimerCallback(ISIS_CB), 0, timer, per);
        }
        public static void StartSphinx(int timer, int per)
        {
            sphinx1 = new Timer(new TimerCallback(SPHINX_CB), 0, timer, per);
        }
        public static void StartRoc(int timer, int per)
        {
            roc1 = new Timer(new TimerCallback(ROC_CB), 0, timer, per);
        }
        public static void StartMedusa(int timer, int per)
        {
            medusa1 = new Timer(new TimerCallback(MEDUSA_CB), 0, timer, per);
        }
        public static void StartTGUnique(int time, int per)
        {
            tiger1 = new Timer(new TimerCallback(TG), 0, time, per);
        }
        public static void StartUriUnique(int time, int per)
        {
            uri1 = new Timer(new TimerCallback(URI), 0, time, per);
        }
        public static void StartIsyUnique(int time, int per)
        {
            isy1 = new Timer(new TimerCallback(ISY), 0, time, per);
        }
        public static void StartLordUnique(int time, int per)
        {
            lord1 = new Timer(new TimerCallback(LORD), 0, time, per);
        }
        public static void StartDemonUnique(int time, int per)
        {
            demon1 = new Timer(new TimerCallback(DEMON), 0, time, per);
        }
        public static void StartCerbUnique(int time, int per)
        {
            cerb1 = new Timer(new TimerCallback(CERB), 0, time, per);
        }
        public static void StartIvyUnique(int time, int per)
        {
            ivy1 = new Timer(new TimerCallback(IVY), 0, time, per);
        }

        public static void SPHINX_CB(object e)
        {
            try
            {
                if (!Sphinx_s)
                {
                    obj o = Sphinx[rnd.Next(0, Sphinx.Count)];
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    Sphinx_s = true;
                    Console.WriteLine("Sphinx spawn warp location : {0}{1}, {2}, {3}", o.xSec, o.ySec, o.x, o.y);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Sphinx spawn error {0}", ex);
            }
        }
        public static void ISIS_CB(object e)
        {
            try
            {
                if (!Isis_s)
                {
                    obj o = Isis[rnd.Next(0, Isis.Count)];
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    Isis_s = true;
                    Console.WriteLine("Isis spawn warp location : {0}{1}, {2}, {3}", o.xSec, o.ySec, o.x, o.y);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Isis spawn error {0}", ex);
            }
        }
        public static void NEITH_CB(object e)
        {
            try
            {
                if (!Neith_s)
                {
                    obj o = Neith[rnd.Next(0, Neith.Count)];
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    Neith_s = true;
                    Console.WriteLine("Neith spawn warp location : {0}{1}, {2}, {3}", o.xSec, o.ySec, o.x, o.y);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Neith spawn error {0}", ex);
            }
        }
        public static void ROC_CB(object e)
        {
            try
            {
                if (!Roc_s)
                {
                    obj o = Roc[rnd.Next(0, Roc.Count)];
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    Roc_s = true;
                    Console.WriteLine("Roc spawn warp location : {0}{1}, {2}, {3}", o.xSec, o.ySec, o.x, o.y);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Roc spawn error {0}", ex);
            }
        }
        public static void MEDUSA_CB(object e)
        {
            try
            {
                if (!Medusa_s)
                {
                    obj o = Medusa[rnd.Next(0, Medusa.Count - 1)];
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    Medusa_s = true;
                    Console.WriteLine("Medusa spawn warp location : {0}{1}, {2}, {3}", o.xSec, o.ySec, o.x, o.y);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Medusa spawn error {0}", ex);
            }
        }

        public static void TG(object e)
        {
            try
            {
                if (!Tiger)
                {
                    obj o = TigerGirl[rnd.Next(0, TigerGirl.Count - 1)];
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    Tiger = true;
                    Console.WriteLine("Tiger girl spawn warp location : {0}{1}, {2}, {3}", o.xSec, o.ySec, o.x, o.y);
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public static void URI(object e)
        {
            try
            {
                if (!Uri)
                {
                    obj o = Urichi[rnd.Next(0, Urichi.Count - 1)];
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    Uri = true;
                    Console.WriteLine("Urichi spawn warp location : {0}{1}, {2}, {3}", o.xSec, o.ySec, o.x, o.y);
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public static void ISY(object e)
        {
            try
            {
                if (!Isy)
                {
                    obj o = Isytaru[rnd.Next(0, Isytaru.Count - 1)];
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    Isy = true;
                    Console.WriteLine("Isytaru spawn warp location : {0}{1}, {2}, {3}", o.xSec, o.ySec, o.x, o.y);
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public static void LORD(object e)
        {
            try
            {
                if (!Lord)
                {
                    obj o = LordYarkan[rnd.Next(0, LordYarkan.Count - 1)];
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    Lord = true;
                    Console.WriteLine("Lord yarkan spawn warp location : {0}{1}, {2}, {3}", o.xSec, o.ySec, o.x, o.y);
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public static void DEMON(object e)
        {
            try
            {
                if (!Demon)
                {
                    obj o = DemonShaitan[rnd.Next(0, DemonShaitan.Count - 1)];
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    Demon = true;
                    Console.WriteLine("Demon Shaitan spawn warp location : {0}{1}, {2}, {3}", o.xSec, o.ySec, o.x, o.y);
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public static void CERB(object e)
        {
            try
            {
                if (!Cerb)
                {
                    obj o = Cerberus[rnd.Next(0, Cerberus.Count - 1)];
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    Cerb = true;
                    Console.WriteLine("Cerberus spawn warp location : {0}{1}, {2}, {3}", o.xSec, o.ySec, o.x, o.y);
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public static void IVY(object e)
        {
            try
            {
                if (!Ivy)
                {
                    obj o = CapIvy[rnd.Next(0, CapIvy.Count)];
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Packet.Unique_Data(5, (int)o.ID, null));
                    Ivy = true;
                    Console.WriteLine("Captain ivy spawn warp location : {0}{1}, {2}, {3}", o.xSec, o.ySec, o.x, o.y);
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }

        public static void AddObject(obj o)
        {
            switch (o.ID)
            {
                case 1954:
                    TigerGirl.Add(o);
                    break;
                case 1982:
                    Urichi.Add(o);
                    break;
                case 2002:
                    Isytaru.Add(o);
                    break;
                case 3810:
                    LordYarkan.Add(o);
                    break;
                case 3875:
                    DemonShaitan.Add(o);
                    break;
                case 3877:
                    Roc.Add(o);
                    break;
                case 5871:
                    Cerberus.Add(o);
                    break;
                case 14778:
                    CapIvy.Add(o);
                    break;
                case 14839:
                    Medusa.Add(o);
                    break;
                case 32768:
                    Neith.Add(o);
                    break;
                case 32752:
                    Sphinx.Add(o);
                    break;
                case 32770:
                    Isis.Add(o);
                    break;
            }
        }

        public static void ClearObject(obj o)
        {
            character n = new character();
            try
            {
                switch (o.ID)
                {
                    case 1954:
                        Systems.SendAll(Packet.Unique_Data(6, (int)1954, n.Information.Name));
                        Tiger = false;
                        DarkEmu_GameServer.GlobalUnique.StartTGUnique(rnd.Next(10, 20) * 60000, 600);   //Random spawn tiger girl
                        break;
                    case 1982:
                        Systems.SendAll(Packet.Unique_Data(6, (int)1982, n.Information.Name));
                        Uri = false;
                        DarkEmu_GameServer.GlobalUnique.StartUriUnique(rnd.Next(10, 20) * 60000, 600);   //Random spawn urichi
                        break;
                    case 2002:
                        Systems.SendAll(Packet.Unique_Data(6, (int)2002, n.Information.Name));
                        Isy = false;
                        DarkEmu_GameServer.GlobalUnique.StartIsyUnique(rnd.Next(10, 20) * 60000, 600);   //Random spawn isy
                        break;
                    case 3810:
                        Systems.SendAll(Packet.Unique_Data(6, (int)3810, n.Information.Name));
                        Lord = false;
                        DarkEmu_GameServer.GlobalUnique.StartLordUnique(rnd.Next(10, 20) * 60000, 600);   //Random spawn lord yarkan
                        break;
                    case 3875:
                        Systems.SendAll(Packet.Unique_Data(6, (int)3875, n.Information.Name));
                        Demon = false;
                        DarkEmu_GameServer.GlobalUnique.StartDemonUnique(rnd.Next(10, 20) * 60000, 600);   //Random spawn demon shaitan
                        break;
                    case 3877:
                        Systems.SendAll(Packet.Unique_Data(6, (int)3877, n.Information.Name));
                        Roc_s = false;
                        DarkEmu_GameServer.GlobalUnique.StartRoc(rnd.Next(10, 20) * 60000, 600);   //Random spawn roc
                        break;
                    case 5871:
                        Systems.SendAll(Packet.Unique_Data(6, (int)5871, n.Information.Name));
                        Cerb = false;
                        DarkEmu_GameServer.GlobalUnique.StartCerbUnique(rnd.Next(10, 20) * 60000, 600);   //Random spawn cerberus
                        break;
                    case 14778:
                        Systems.SendAll(Packet.Unique_Data(6, (int)14538, n.Information.Name));
                        Ivy = false;
                        DarkEmu_GameServer.GlobalUnique.StartIvyUnique(rnd.Next(10, 20) * 60000, 600);   //Random spawn captain ivy
                        break;
                    case 14839:
                        Systems.SendAll(Packet.Unique_Data(6, (int)22654, n.Information.Name));
                        Cerb = false;
                        DarkEmu_GameServer.GlobalUnique.StartMedusa(rnd.Next(10, 20) * 90000, 600);   //Random spawn medusa
                        break;
                    case 32768:
                        Systems.SendAll(Packet.Unique_Data(6, (int)32768, n.Information.Name));
                        Neith_s = false;
                        DarkEmu_GameServer.GlobalUnique.StartNeith(rnd.Next(10, 20) * 90000, 600);   //Random spawn neith
                        break;
                    case 32752:
                        Systems.SendAll(Packet.Unique_Data(6, (int)32752, n.Information.Name));
                        Sphinx_s = false;
                        DarkEmu_GameServer.GlobalUnique.StartSphinx(rnd.Next(10, 20) * 90000, 600);   //Random spawn sphinx
                        break;
                    case 32770:
                        Systems.SendAll(Packet.Unique_Data(6, (int)32770, n.Information.Name));
                        Isis_s = false;
                        DarkEmu_GameServer.GlobalUnique.StartIsis(rnd.Next(10, 20) * 90000, 600);   //Random spawn isis
                        break;
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
    }
}