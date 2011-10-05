using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SrxRevo
{
    public partial class Systems
    {
        class QuestInformation
        {
            public void QuestProcess(int questid, character Character)
            {
                switch (Data.QuestData[questid].Questname)
                {
                    case "SN_CON_QTUTORIAL_CH_01":
                        //Tutorial Quest (Static info for now) Still thinking of best options
                        //Any suggestion is welcome.

                        //The idea i have:
                        //(Set quest options and actions per quest ID string
                        //Ex: Talk to npc (Check completion quest task 1 / 2 etc).
                        //Then when final task is done add reward..
                        //All taken information will come from database (To see active and all other info).

                        Character.Quest.QuestActive = true;
                        Character.Quest.QuestDrop = 0;
                        Character.Quest.TalkToNpc = 0;
                        break;
                }
            }
        }
        
        class QuestState
        {
        }
    }
}