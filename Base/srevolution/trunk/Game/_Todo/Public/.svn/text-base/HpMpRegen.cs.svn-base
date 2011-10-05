using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Framework;

namespace SrxRevo
{
    public partial class Systems
    {
        public Timer SitDown_HPMP_RegenTimer;
        public Timer HPRegen;
        public Timer MPRegen;
        void StartSitDownTimer()
        {
            try
            {
                SitDown_HPMP_RegenTimer = new Timer(new TimerCallback(SitDownCallback), 0, 0, 3000);
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        void SitDownCallback(object e)
        {
            try
            {
                double RegenMP = Character.Stat.Mp * 0.08; // it's from a trustful site which is about sro,so it's the right value not 10% or wtf^^
                double RegenHP = Character.Stat.Hp * 0.08;

                // HP regen when sit
                if (Character.Stat.SecondHp + (int)RegenHP < Character.Stat.Hp)
                {
                    Character.Stat.SecondHp += (int)RegenHP;
                    UpdateHp();
                }
                else
                {
                    Character.Stat.SecondHp += Character.Stat.Hp - Character.Stat.SecondHp;
                    UpdateHp();
                }

                // MP regen when sit
                if (Character.Stat.SecondMP + (int)RegenMP < Character.Stat.Mp)
                {
                    Character.Stat.SecondMP += (int)RegenMP;
                    UpdateMp();
                }
                else
                {
                    Character.Stat.SecondMP += Character.Stat.Mp - Character.Stat.SecondMP;
                    UpdateMp();
                }

                // stop timer when HP and MP full
                if (Character.Stat.SecondHp == Character.Stat.Mp && Character.Stat.SecondHp == Character.Stat.Hp)
                    StopSitDownTimer();
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        void StopSitDownTimer()
        {
            try
            {
                if (SitDown_HPMP_RegenTimer != null)
                {
                    SitDown_HPMP_RegenTimer.Dispose();
                    SitDown_HPMP_RegenTimer = null;
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        void HPregen(int time)
        {
            try
            {
                HPRegen = new Timer(new TimerCallback(HpregenCallback), 0, 0, time); //10 seconds retail
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        void HpregenCallback(object e)
        {
            try
            {
                if (!Character.State.Die && !Character.Action.sAttack && !Character.Action.sCasting && !Character.Action.nAttack && !Character.Spawning)
                {
                    double RegenHP = Character.Stat.Hp * 0.007; //also from that site.
                    if (Character.Blues.hpregen != 0)
                        RegenHP += RegenHP * (Character.Blues.hpregen / 100);

                    // HP regen
                    if (Character.Stat.SecondHp + (int)RegenHP < Character.Stat.Hp)
                    {
                        Character.Stat.SecondHp += (int)RegenHP;
                        UpdateHp();
                    }
                    else if (Character.Stat.SecondHp != Character.Stat.Hp)
                    {
                        Character.Stat.SecondHp += Character.Stat.Hp - Character.Stat.SecondHp;
                        UpdateHp();
                    }
                    //SavePlayerHPMP();
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        void MPregen(int time)
        {
            try
            {
                MPRegen = new Timer(new TimerCallback(MpregenCallback), 0, 0, time); //10 seconds retail
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        void MpregenCallback(object e)
        {
            try
            {
                if (!Character.State.Die && !Character.Action.sAttack && !Character.Action.sCasting && !Character.Action.nAttack && !Character.Spawning)
                {
                    double RegenMP = Character.Stat.Mp * 0.007; // 2% regen retail

                    if (Character.Blues.mpregen != 0)
                        RegenMP += RegenMP * (Character.Blues.mpregen / 100);

                    if (Character.Stat.SecondMP + (int)RegenMP < Character.Stat.Mp)
                    {
                        Character.Stat.SecondMP += (int)RegenMP;
                        UpdateMp();
                    }
                    else if (Character.Stat.SecondMP != Character.Stat.Mp)
                    {
                        Character.Stat.SecondMP += Character.Stat.Mp - Character.Stat.SecondMP;
                        UpdateMp();
                    }

                    //SavePlayerHPMP();
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }

        }
        public void StopHPRegen()
        {
            try
            {
                if (HPRegen != null)
                {
                    HPRegen.Dispose();
                    HPRegen = null;
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void StopMPRegen()
        {
            try
            {
                if (MPRegen != null)
                {
                    MPRegen.Dispose();
                    MPRegen = null;
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
    }
}
