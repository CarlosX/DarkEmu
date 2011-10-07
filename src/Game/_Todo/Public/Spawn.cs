///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        /////////////////////////////////////////////////////////////////////////////////
        // Spawn system
        /////////////////////////////////////////////////////////////////////////////////       
        public void ObjectSpawnCheck()
        {
            try
            {
                if (this.Character.deSpawning) return;
                int spawnrange = 100;
                lock (this)
                {
                    //Make sure character info is not null or not spawning yet allready.
                    if (this.Character != null && !this.Character.Spawning)
                    {
                        //Set spawn state to true so cannot be doubled
                        this.Character.Spawning = true;

                        //Repeat for each client ingame
                        #region Clients
                        for (int i = 0; i < Systems.clients.Count; i++)
                        {
                            //Get defined information for the client
                            Systems playerspawn = Systems.clients[i];
                            //Make sure that the spawning case is not ourselfs, or not spawned yet and not null
                            if (playerspawn != null && playerspawn != this && !Character.Spawned(playerspawn.Character.Information.UniqueID) && playerspawn.Character.Information.Name != this.Character.Information.Name)
                            {
                                //If our position is lower or higher then 50 + spawnrange of that of the player
                                if (playerspawn.Character.Position.x >= (Character.Position.x - 50) && playerspawn.Character.Position.x <= ((Character.Position.x - 50) + spawnrange) && playerspawn.Character.Position.y >= (Character.Position.y - 50) && playerspawn.Character.Position.y <= ((Character.Position.y - 50) + spawnrange))
                                {
                                    //Make sure the unique id is not null
                                    if (playerspawn.Character.Information.UniqueID != 0)
                                    {
                                        Character.Spawn.Add(playerspawn.Character.Information.UniqueID);
                                        client.Send(Packet.ObjectSpawn(playerspawn.Character));
                                    }
                                    //Spawn ourselfs to the other players currently in spawn range.
                                    ObjectPlayerSpawn(playerspawn);
                                }
                            }
                        }
                        #endregion
                        //Repeat for each helper object
                        #region Helper objects
                        for (int i = 0; i < Systems.HelperObject.Count; i++)
                        {
                            //If the helper object is not null , or not spawned for us yet and the unique id is not null
                            if (Systems.HelperObject[i] != null && !Systems.HelperObject[i].Spawned(this.Character.Information.UniqueID))
                            {
                                //If our position is lower or higher then 50 + spawnrange of that of the object
                                if (this.Character.Position.x >= (Systems.HelperObject[i].x - 50) && this.Character.Position.x <= ((Systems.HelperObject[i].x - 50) + spawnrange) && this.Character.Position.y >= (Systems.HelperObject[i].y - 50) && this.Character.Position.y <= ((Systems.HelperObject[i].y - 50) + spawnrange))
                                {
                                    if (Systems.HelperObject[i].UniqueID != 0)
                                    {
                                        //Add our spawn
                                        Systems.HelperObject[i].Spawn.Add(this.Character.Information.UniqueID);
                                        //Send visual packet
                                        this.client.Send(Packet.ObjectSpawn(Systems.HelperObject[i]));
                                    }
                                }
                            }
                        }
                        #endregion
                        /*
                        #region Special objects
                        for (int i = 0; i < Systems.SpecialObjects.Count; i++)
                        {
                            //If the special object is not null , or not spawned for us yet and the unique id is not null
                            if (Systems.SpecialObjects[i] != null && !Systems.SpecialObjects[i].Spawned(this.Character.Information.UniqueID))
                            {
                                //If our position is lower or higher then 50 + spawnrange of that of the object
                                if (this.Character.Position.x >= (Systems.SpecialObjects[i].x - 50) && this.Character.Position.x <= ((Systems.SpecialObjects[i].x - 50) + spawnrange) && this.Character.Position.y >= (Systems.SpecialObjects[i].y - 50) && this.Character.Position.y <= ((Systems.SpecialObjects[i].y - 50) + spawnrange))
                                {
                                    if (Systems.SpecialObjects[i].UniqueID != 0)
                                    {
                                        //Add our spawn
                                        Systems.SpecialObjects[i].Spawn.Add(this.Character.Information.UniqueID);
                                        //Send visual packet
                                        client.Send(Packet.ObjectSpawn(Systems.SpecialObjects[i]));
                                        //Console.WriteLine("Spawning {0}", Data.ObjectBase[Systems.Objects[i].ID].Name);
                                    }
                                }
                            }
                        }
                        #endregion
                         */
                        //Repeat for each object
                        #region Objects
                        for (int i = 0; i < Systems.Objects.Count; i++)
                        {
                            //If the helper object is not null , or not spawned for us yet and the unique id is not null
                            if (Systems.Objects[i] != null && !Systems.Objects[i].Spawned(this.Character.Information.UniqueID))
                            {
                                //If our position is lower or higher then 50 + spawnrange of that of the object
                                if (this.Character.Position.x >= (Systems.Objects[i].x - 50) && this.Character.Position.x <= ((Systems.Objects[i].x - 50) + spawnrange) && this.Character.Position.y >= (Systems.Objects[i].y - 50) && this.Character.Position.y <= ((Systems.Objects[i].y - 50) + spawnrange))
                                {
                                    if (Systems.Objects[i].UniqueID != 0 && !Systems.Objects[i].Die)
                                    {
                                        //Add our spawn
                                        Systems.Objects[i].Spawn.Add(this.Character.Information.UniqueID);
                                        //Send visual packet
                                        client.Send(Packet.ObjectSpawn(Systems.Objects[i]));
                                        //Console.WriteLine("Spawning {0}", Data.ObjectBase[Systems.Objects[i].ID].Name);
                                    }
                                }
                            }
                        }
                        #endregion
                        //Repeat for each world item
                        #region Helper objects
                        for (int i = 0; i < Systems.WorldItem.Count; i++)
                        {
                            //If the helper object is not null , or not spawned for us yet and the unique id is not null
                            if (Systems.WorldItem[i] != null && !Systems.WorldItem[i].Spawned(this.Character.Information.UniqueID))
                            {
                                //If our position is lower or higher then 50 + spawnrange of that of the object
                                if (this.Character.Position.x >= (Systems.WorldItem[i].x - 50) && this.Character.Position.x <= ((Systems.WorldItem[i].x - 50) + spawnrange) && this.Character.Position.y >= (Systems.WorldItem[i].y - 50) && this.Character.Position.y <= ((Systems.WorldItem[i].y - 50) + spawnrange))
                                {
                                    if (Systems.WorldItem[i].UniqueID != 0)
                                    {
                                        //Add our spawn
                                        Systems.WorldItem[i].Spawn.Add(this.Character.Information.UniqueID);
                                        //Send visual packet
                                        this.client.Send(Packet.ObjectSpawn(Systems.WorldItem[i]));
                                    }
                                }
                            }
                        }
                        #endregion
                        //If we are riding a horse and its not spawned to the player yet
                        #region Transports
                        if (Character.Transport.Right)
                        {
                            //If not spawned
                            if (!Character.Transport.Spawned)
                            {
                                //Set bool true
                                Character.Transport.Spawned = true;
                                //Spawn horse object
                                Character.Transport.Horse.SpawnMe();
                                //Send visual update player riding horse
                                Character.Transport.Horse.Send(Packet.Player_UpToHorse(this.Character.Information.UniqueID, true, Character.Transport.Horse.UniqueID));
                            }
                        }
                        #endregion
                        //Reset bool to false so we can re-loop the function
                        this.Character.Spawning = false;
                        ObjectDeSpawnCheck();
                    }
                    //If something wrong happened and we are null, we set our bool false as well.
                    this.Character.Spawning = false;
                }
            }
            catch (Exception ex)
            {
                //If any exception happens we disable the loop bool for re-use
                this.Character.Spawning = false;
                Console.WriteLine("Spawn check error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Spawn system (Spawn our char to others).
        /////////////////////////////////////////////////////////////////////////////////   
        void ObjectPlayerSpawn(Systems s)
        {
            try
            {
                if (!s.Character.Spawned(this.Character.Information.UniqueID) && this.Character.Information.UniqueID != 0 && !s.Character.Spawning)
                {
                    //We loop the spawn check for the player that needs it.
                    s.ObjectSpawnCheck();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Object player spawn error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // DE-Spawn system
        /////////////////////////////////////////////////////////////////////////////////  
        public void ObjectDeSpawnCheck()
        {
            //We wrap our function inside a catcher
            try
            {
                if (this.Character.Spawning) return;
                //Set default spawn range
                int spawnrange = 110;
                //Make sure that the character is not null, and not despawning allready!
                if (this.Character != null && !this.Character.deSpawning && this != null)
                {
                    //Set our loop (active) bool to true so we cant do it double same way.
                    this.Character.deSpawning = true;
                    //Region helper objects
                    #region Helper objects
                    //Repeat for each helper object
                    for (int i = 0; i < Systems.HelperObject.Count; i++)
                    {
                        //If our object is not null, and the object is spawned to our character
                        if (Systems.HelperObject[i] != null && Systems.HelperObject[i].Spawned(this.Character.Information.UniqueID))
                        {
                            //Make sure that its going out of range instead of in range
                            if (this.Character.Position.x >= (Systems.HelperObject[i].x - 50) && this.Character.Position.x <= ((Systems.HelperObject[i].x - 50) + spawnrange) && this.Character.Position.y >= (Systems.HelperObject[i].y - 50) && this.Character.Position.y <= ((Systems.HelperObject[i].y - 50) + spawnrange))
                            {
                            }
                            //When out of range start despawn
                            else
                            {
                                //Make sure we are on the same sectors (To prevent overlaps).
                                //Make sure the despawning object is not null
                                if (Systems.HelperObject[i].UniqueID != 0)
                                {
                                    //Then we remove the spawn
                                    Systems.HelperObject[i].Spawn.Remove(this.Character.Information.UniqueID);
                                    //And send despawn packet
                                    client.Send(Packet.ObjectDeSpawn(Systems.HelperObject[i].UniqueID));
                                }
                            }
                        }
                    }
                    #endregion
                    //Region for objects
                    #region Objects
                    for (int i = 0; i < Systems.Objects.Count; i++)
                    {
                        //Make sure the object in this case is not null
                        if (Systems.Objects[i] != null)
                        {
                            //If the object is spawned to us but not in death state
                            if (Systems.Objects[i].Spawned(Character.Information.UniqueID) && !Systems.Objects[i].Die)
                            {
                                //The range we chosen
                                if (Systems.Objects[i].x >= (Character.Position.x - 50) && Systems.Objects[i].x <= ((Character.Position.x - 50) + spawnrange) && Systems.Objects[i].y >= (Character.Position.y - 50) && Systems.Objects[i].y <= ((Character.Position.y - 50) + spawnrange))
                                {
                                }
                                //If out of range start despawn
                                else
                                {
                                    //Make sure the id and unique id are not null
                                    if (Systems.Objects[i].UniqueID != 0 && !Systems.Objects[i].Die)
                                    {
                                        //Start removing our spawn
                                        Systems.Objects[i].Spawn.Remove(Character.Information.UniqueID);
                                        //Despawn packet sending
                                        client.Send(Packet.ObjectDeSpawn(Systems.Objects[i].UniqueID));
                                        //Console.WriteLine("DE- Spawning {0}", Data.ObjectBase[Systems.Objects[i].ID].Name);
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    /*
                    #region Special objects
                    for (int i = 0; i < Systems.SpecialObjects.Count; i++)
                    {
                        //Make sure the object in this case is not null
                        if (Systems.SpecialObjects[i] != null)
                        {
                            //If the object is spawned to us but not in death state
                            if (Systems.SpecialObjects[i].Spawned(Character.Information.UniqueID))
                            {
                                //The range we chosen
                                if (Systems.SpecialObjects[i].x >= (Character.Position.x - 50) && Systems.SpecialObjects[i].x <= ((Character.Position.x - 50) + spawnrange) && Systems.SpecialObjects[i].y >= (Character.Position.y - 50) && Systems.SpecialObjects[i].y <= ((Character.Position.y - 50) + spawnrange))
                                {
                                }
                                //If out of range start despawn
                                else
                                {
                                    //Sector check as we do above.
                                    //if (Character.Position.xSec == Systems.Objects[i].xSec && Character.Position.ySec == Systems.Objects[i].ySec)
                                    {
                                        //Make sure the id and unique id are not null
                                        if (Systems.SpecialObjects[i].UniqueID != 0)
                                        {
                                            //Start removing our spawn
                                            Systems.SpecialObjects[i].Spawn.Remove(Character.Information.UniqueID);
                                            //Despawn packet sending
                                            client.Send(Packet.ObjectDeSpawn(Systems.SpecialObjects[i].UniqueID));
                                            //Console.WriteLine("DE- Spawning {0}", Data.ObjectBase[Systems.Objects[i].ID].Name);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    #endregion
                     */
                    //Region for clients
                    #region Clients
                    for (int i = 0; i < Systems.clients.Count; i++)
                    {
                        if (Systems.clients[i] != null && Systems.clients[i] != this && Character.Spawned(Systems.clients[i].Character.Information.UniqueID))
                        {
                            if (Systems.clients[i].Character.Position.x >= (Character.Position.x - 50) && Systems.clients[i].Character.Position.x <= ((Character.Position.x - 50) + spawnrange) && Systems.clients[i].Character.Position.y >= (Character.Position.y - 50) && Systems.clients[i].Character.Position.y <= ((Character.Position.y - 50) + spawnrange))
                            {
                            }
                            else
                            {
                                if (Character.Spawned(Systems.clients[i].Character.Information.UniqueID))
                                {
                                    //Extra check before we send the packet update
                                    if (Systems.clients[i].Character.Information.UniqueID != 0)
                                    {
                                        Character.Spawn.Remove(Systems.clients[i].Character.Information.UniqueID);
                                        client.Send(Packet.ObjectDeSpawn(Systems.clients[i].Character.Information.UniqueID));
                                    }
                                }
                                ObjectDePlayerSpawn(Systems.clients[i]);
                            }
                        }
                    }
                    #endregion
                    //Region for items
                    #region Items
                    for (int i = 0; i < Systems.WorldItem.Count; i++)
                    {
                        if (Systems.WorldItem[i] != null && Systems.WorldItem[i].Spawned(Character.Information.UniqueID) && Systems.WorldItem[i].UniqueID != 0)
                        {
                            if (Systems.WorldItem[i].x >= (Character.Position.x - 50) && Systems.WorldItem[i].x <= ((Character.Position.x - 50) + spawnrange) && Systems.WorldItem[i].y >= (Character.Position.y - 50) && Systems.WorldItem[i].y <= ((Character.Position.y - 50) + spawnrange))
                            {
                            }
                            else
                            {
                                if (Systems.WorldItem[i].Spawned(Character.Information.UniqueID) && Systems.WorldItem[i].Model != 0)
                                {
                                    if (Systems.WorldItem[i].UniqueID != 0)
                                    {
                                        Systems.WorldItem[i].Spawn.Remove(Character.Information.UniqueID);
                                        client.Send(Packet.ObjectDeSpawn(Systems.WorldItem[i].UniqueID));
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    //Set bool to false so we can use the loop again
                    this.Character.deSpawning = false;
                }
                //Set bool to false so we can use the loop again (incase something happened and we are null).
                this.Character.deSpawning = false;
            }

            catch (Exception ex)
            {
                //If any exception is made we disable the loop
                this.Character.deSpawning = false;
                Console.WriteLine("Despawn error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // DE-Spawn system (Spawn our char to others).
        /////////////////////////////////////////////////////////////////////////////////    
        void ObjectDePlayerSpawn(Systems s)
        {

            try
            {
                if (s.Character.Spawned(this.Character.Information.UniqueID) && !s.Character.deSpawning)
                {
                    if (s.Character.Information.UniqueID != 0)
                    {
                        s.ObjectDeSpawnCheck();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Systems despawn error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // DE-Spawn
        /////////////////////////////////////////////////////////////////////////////////    
        void DeSpawnMe()
        {
            //Wrap our function inside a catcher
            try
            {
                //Checks before continuing
                if (this.Character.Network.Exchange.Window) Exchange_Close();
                if (this.Character.Action.nAttack) StopAttackTimer();
                if (this.Character.Action.sAttack) StopAttackTimer();
                if (this.Character.Action.sCasting) StopAttackTimer();
                if (this.Character.Stall.Stallactive) StallClose();
                //Clients
                #region Client spawns
                for (int b = 0; b < Systems.clients.Count; b++)
                {
                    if (Systems.clients[b] != null && Systems.clients[b].Character.Spawned(this.Character.Information.UniqueID) && Systems.clients[b] != this)
                    {
                        if (Systems.clients[b].Character.Information.UniqueID != 0)
                        {
                            Systems.clients[b].Character.Spawn.Remove(this.Character.Information.UniqueID);
                            Systems.clients[b].client.Send(Packet.ObjectDeSpawn(this.Character.Information.UniqueID));
                        }
                    }
                }
                this.Character.Spawn.Clear();
                #endregion
                //Helper objects
                #region Helper objects
                for (int i = 0; i < Systems.HelperObject.Count; i++)
                {
                    if (Systems.HelperObject[i] != null && Systems.HelperObject[i].Spawned(this.Character.Information.UniqueID))
                    {
                        if (Character.Information.UniqueID != 0 && Systems.HelperObject[i].UniqueID != 0)
                        {
                            Systems.HelperObject[i].Spawn.Remove(this.Character.Information.UniqueID);
                        }
                    }
                }
                #endregion
                //Objects
                #region Objects
                for (int i = 0; i < Systems.Objects.Count; i++)
                {
                    if (Systems.Objects[i] != null && Systems.Objects[i].Spawned(this.Character.Information.UniqueID))
                    {
                        if (Character.Information.UniqueID != 0 && Systems.Objects[i].UniqueID != 0)
                        {
                            Systems.Objects[i].Spawn.Remove(Character.Information.UniqueID);
                        }
                    }
                }
                #endregion
                //Drops
                #region Drops
                for (int i = 0; i < Systems.WorldItem.Count; i++)
                {
                    if (Systems.WorldItem[i] != null && Systems.WorldItem[i].Spawned(Character.Information.UniqueID) && Systems.WorldItem[i].UniqueID != 0)
                    {
                        if (Systems.WorldItem[i].Spawned(Character.Information.UniqueID))
                        {
                            if (Character.Information.UniqueID != 0 && Systems.WorldItem[i].UniqueID != 0)
                            {
                                Systems.WorldItem[i].Spawn.Remove(Character.Information.UniqueID);
                            }
                        }
                    }
                }
                #endregion
                //Char spawns
                #region Char spawns
                for (int i = 0; i < Systems.clients.Count; i++)
                {
                    if (Systems.clients[i] != this && Systems.clients[i] != null)
                    {
                        if (Character.Spawned(Systems.clients[i].Character.Information.UniqueID))
                        {
                            if (Character.Information.UniqueID != 0 && Systems.clients[i].Character.Information.UniqueID != 0)
                            {
                                Character.Spawn.Remove(Systems.clients[i].Character.Information.UniqueID);
                            }
                        }
                    }
                }
                #endregion
                //Check if our character is not null
                if (Character.Information.UniqueID != 0)
                {
                    //Check if we have transport active
                    if (Character.Transport.Right)
                    {
                        //Set bools for despawning
                        Character.Transport.Spawned = false;
                        Character.Transport.Horse.Information = false;
                        Character.Transport.Horse.DeSpawnMe(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Despawn me error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
    }
}
