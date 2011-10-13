///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public class Formule
    {
        public static float cavegamex(float x, float x1)//########################################################################################
        {
            return x - x1;
        }
        public static float cavegamey(float y, float y1)
        {
            return y - y1;
        }
        public static float cavegamez(float z, float z1)
        {
            return z - z1;
        }
        public static float cavegamex(float x)
        {
            byte sector = 0;
            return ((sector - 135) * 192 + (x / 10));// ############################## CAVE COORD FORMULAS ######################################
        }
        public static float cavegamey(float y)
        {
            byte sector = 0;
            return ((sector - 92) * 192 + (y / 10));
        }
        public static float cavepacketx(float x)
        {
            byte sector = 0;
            return ((x - ((sector) - 135) * 192) * 10);
        }
        public static float cavepackety(float y)
        {
            byte sector = 0;
            return ((y - ((sector) - 92) * 192) * 10);
        }    

        public static float packetx(float x, byte sector)
        {
            return ((x - ((sector) - 135) * 192) * 10);
        }
        public static float packety(float y, byte sector)
        {
            return ((y - ((sector) - 92) * 192) * 10);
        }
        public static float gamex(float x, byte sector)
        {
            return ((sector - 135) * 192 + (x / 10));
        }
        public static float gamey(float y, byte sector)
        {
            return ((sector - 92) * 192 + (y / 10));
        }
        public static double gamedistance(float x1, float y1, float x2, float y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));//Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));// + ((z1 - z2) * (z1 - z2))
        }
        public static double gamedamage(double maxDMG, double aPower, double absrob, double def, double pBalance, double uAttack)
        {
            return ((maxDMG) * (1 + (0.01 * aPower)) / (1 + (absrob * 0.001)) - def) * (0.01 * pBalance) * (1 + (0.01 * uAttack)) * 1;
        }
        public static int gamePhp(byte level, short str)
        {
            return Convert.ToInt32(Math.Pow(1.02, ((level) - 1)) * (str) * 10);
        }
        public static int gamePmp(byte level, short Int)
        {
            return Convert.ToInt32(Math.Pow(1.02, ((level) - 1)) * (Int) * 10);
        }
       
        public static double gamedistance(DarkEmu_GameServer.character._pos p1, DarkEmu_GameServer.character._pos p2)
        {  // Nukei: for test with range checking on objects, maybe faster than only calculating distance
            if ((p1.xSec >= p2.xSec - 1) && (p1.xSec <= p2.xSec + 1) && (p1.ySec >= p2.ySec - 1) && (p1.ySec <= p2.ySec + 1))
            {
                return gamedistance(p1.x, p1.y, p2.x, p2.y);
            }
            else
            {
                return 99999999999999;
            }
        }
        public static double gamedistance(Global.vektor p1, Global.vektor p2)
        {  // Nukei: for test with range checking on objects, maybe faster than only calculating distance
            if ((p1.xSec >= p2.xSec - 1) && (p1.xSec <= p2.xSec + 1) && (p1.ySec >= p2.ySec - 1) && (p1.ySec <= p2.ySec + 1))
            {
                return gamedistance(p1.x, p1.y, p2.x, p2.y);
            }
            else
            {
                return 99999999999999;
            }
        }
        public static double gamedistance(obj p1, DarkEmu_GameServer.character._pos p2)
        {  // Nukei: for test with range checking on objects, maybe faster than only calculating distance
            if ((p1.xSec >= p2.xSec - 1) && (p1.xSec <= p2.xSec + 1) && (p1.ySec >= p2.ySec - 1) && (p1.ySec <= p2.ySec + 1))
            {
                return gamedistance((float)p1.x, (float)p1.y, p2.x, p2.y);
            }
            else
            {
                return 99999999999999;
            }
        }
        public static double gamedistance(spez_obj p1, DarkEmu_GameServer.character._pos p2)
        {  // Nukei: for test with range checking on objects, maybe faster than only calculating distance
            if ((p1.xSec >= p2.xSec - 1) && (p1.xSec <= p2.xSec + 1) && (p1.ySec >= p2.ySec - 1) && (p1.ySec <= p2.ySec + 1))
            {
                return gamedistance((float)p1.x, (float)p1.y, p2.x, p2.y);
            }
            else
            {
                return 99999999999999;
            }
        }
        public static double gamedistance(world_item p1, DarkEmu_GameServer.character._pos p2)
        {  // Nukei: for test with range checking on objects, maybe faster than only calculating distance
            if ((p1.xSec >= p2.xSec - 1) && (p1.xSec <= p2.xSec + 1) && (p1.ySec >= p2.ySec - 1) && (p1.ySec <= p2.ySec + 1))
            {
                return gamedistance((float)p1.x, (float)p1.y, p2.x, p2.y);
            }
            else
            {
                return 99999999999999;
            }
        } 

        public static short makeRegion(byte xSector, byte ySector)
        {
            return (short)((ySector << 8) | xSector);
        }
        public static byte getXsector(short region)
        {
            return (byte)(region & 0xFF);
        }
        public static byte getYsector(short region)
        {
            return (byte)(((region) >> 8) & 0xFF);
        }
        public static float GetHeightAt(byte xSec, byte ySec, float x, float y)
        {
            short region = Formule.makeRegion(xSec, ySec);
            return Data.MapObject[region].GetHeightAt(packetx(x, xSec) / 20.0f, packety(y, ySec) / 20.0f);
        }
        // collision detection 2D - /With Linear Algebra/
        public static bool isCollided_onMovement(Global.vektor fromPos, Global.vektor toPos, ref Global.vektor CollisionPoint, Systems c)
        {
            try
            {
                fromPos.x = packetx(fromPos.x, fromPos.xSec) / 20.0f;
                fromPos.y = packety(fromPos.y, fromPos.ySec) / 20.0f;
                toPos.x = packetx(toPos.x, toPos.xSec) / 20.0f;
                toPos.y = packety(toPos.y, toPos.ySec) / 20.0f;

                // iterated line
                Global.vektor Line_A = new Global.vektor();
                Global.vektor Line_B = new Global.vektor();

                List<Global.vektor> CollisionPoints = new List<Global.vektor>();

                // get current region to filter out the objects
                short region = makeRegion(fromPos.xSec, fromPos.ySec);

                // get all entitys in this region and try to cut them with our movement line
                foreach (Global.SectorObject.n7nEntity obj in Data.MapObject[region].entitys)
                {
                    foreach (Global.SectorObject.n7nEntity.sLine line in obj.OutLines)
                    {
                        // if not passable
                        if (line.flag != 0)
                        {
                            Line_A.x = obj.Points[line.PointA].x + obj.Position.x;
                            Line_A.y = obj.Points[line.PointA].y + obj.Position.y;
                            Line_B.x = obj.Points[line.PointB].x + obj.Position.x;
                            Line_B.y = obj.Points[line.PointB].y + obj.Position.y;

                            if (lineSegmentIntersection(fromPos, toPos, Line_A, Line_B, ref CollisionPoints))
                            {
                                
                                Console.WriteLine("x:{0} y:{1}", gamex(CollisionPoints[0].x, fromPos.xSec), gamey(CollisionPoints[0].y, fromPos.ySec));
                            }
                        }
                    }
                }
                if (CollisionPoints.Count == 0) return false;

                // set the nearest collision point for return point
                double minDistance = gamedistance(fromPos.x, fromPos.y, CollisionPoints[0].x, CollisionPoints[0].y);
                foreach (Global.vektor cp in CollisionPoints)
                {
                    //double currentDistance = gamedistance(fromPos.x, fromPos.y, cp.x, cp.y);
                    double currentDistance = gamedistance(fromPos, cp);

                    if (currentDistance <= minDistance)
                    {
                        CollisionPoint.x = cp.x;
                        CollisionPoint.y = cp.y;
                    }
                }

                // translate the collision point on the movement line to get real coordinates (not the exact point of collision)
                double sin_alpha = Math.Abs(CollisionPoints[0].y - fromPos.y) / minDistance;
                double cos_alpha = Math.Abs(CollisionPoints[0].x - fromPos.x) / minDistance;

                CollisionPoints[0].y = (float)(sin_alpha * (minDistance - 3));
                CollisionPoints[0].x = (float)(cos_alpha * (minDistance - 3));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Collision detection failed: {0}", ex.Message);
            }
            return true;
        }
        public static bool lineSegmentIntersection(Global.vektor Line1_A, Global.vektor Line1_B, Global.vektor Line2_A, Global.vektor Line2_B, ref List<Global.vektor> CollisionPoints)
        {
            //  Fail if either line segment is zero-length.
            if (Line1_A.x == Line1_B.x && Line1_A.y == Line1_B.y || Line2_A.x == Line2_B.x && Line2_A.y == Line2_B.y) return false;

            //  Translate the system so that point A is on the origin.
            Line1_B.x -= Line1_A.x; Line1_B.y -= Line1_A.y;
            Line2_A.x -= Line1_A.x; Line2_A.y -= Line1_A.y;
            Line2_B.x -= Line1_A.x; Line2_B.y -= Line1_A.y;

            //  Discover the length of segment A-B.
            float distAB = (float)Math.Sqrt(Line1_B.x * Line1_B.x + Line1_B.y * Line1_B.y);

            //  Rotate the system so that point B is on the positive X axis.
            float theCos = Line1_B.x / distAB;
            float theSin = Line1_B.y / distAB;
            float newX = Line2_A.x * theCos + Line2_A.y * theSin;
            Line2_A.y = Line2_A.y * theCos - Line2_A.x * theSin; Line2_A.x = newX;
            newX = Line2_B.x * theCos + Line2_B.y * theSin;
            Line2_B.y = Line2_B.y * theCos - Line2_B.x * theSin; Line2_B.x = newX;

            //  Fail if segment C-D doesn't cross line A-B.
            if (Line2_A.y < 0.0f && Line2_B.y < 0.0f || Line2_A.y >= 0.0f && Line2_B.y >= 0.0f) return false;

            //  Discover the position of the intersection point along line A-B.
            float ABpos = Line2_B.x + (Line2_A.x - Line2_B.x) * Line2_B.y / (Line2_B.y - Line2_A.y);

            //  Fail if segment C-D crosses line A-B outside of segment A-B.
            if (ABpos < 0.0f || ABpos > distAB) return false;

            //  Apply the discovered position to line A-B in the original coordinate system.
            Global.vektor cp = new Global.vektor();
            cp.x = Line1_A.x + ABpos * theCos;
            cp.y = Line1_A.y + ABpos * theSin;
            CollisionPoints.Add(cp);

            return true;
        }
        public static bool GetSurroundRange(Global.vektor obj, Global.vektor origo, int range)
        {
            if (obj.x >= (origo.x - range) && obj.x <= ((origo.x - range) + range * 2) && obj.y >= (origo.y - range) && obj.y <= ((origo.y - range) + range * 2))
            {
                return true;
            }
            return false;
        }
    }
}
