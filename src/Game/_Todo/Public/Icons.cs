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
        public void RequestIcons()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            byte IconType = Reader.Byte();
            short IconHexLenght = Reader.Int16();
            string Icon = Reader.String(IconHexLenght);
            Reader.Close();
            //Need to figure out the string.. identification of the icon probably
            client.Send(Packet.IconSend(IconType, Icon));

            // Below is sniffed data
            /*
             * [S -> C][2114]
                01                                                Byte = Type icon (1 = Guild 2 = union).
                1C 01                                             Lenght of hex icon string

                // below is the hex string (Ascii) icon
                74 6D 28 73 81 2A 47 37 F6 13 99 62 8C 3F 4E 29   tm(s.*G7...b.?N)
                0F 04 CB 3D E6 5F FC 0B D6 07 03 DD 0D 72 9A 25   ...=._.......r.%
                8A 88 BF CB 59 5A 8A 08 74 80 32 53 AA 1C 5E 86   ....YZ..t.2S..^.
                8A 88 BF CB 59 5A 8A 08 74 80 32 53 AA 1C 5E 86   ....YZ..t.2S..^.
                8A 88 BF CB 59 5A 8A 08 74 80 32 53 AA 1C 5E 86   ....YZ..t.2S..^.
                8A 88 BF CB 59 5A 8A 08 74 80 32 53 AA 1C 5E 86   ....YZ..t.2S..^.
                8A 88 BF CB 59 5A 8A 08 74 80 32 53 AA 1C 5E 86   ....YZ..t.2S..^.
                8A 88 BF CB 59 5A 8A 08 74 80 32 53 AA 1C 5E 86   ....YZ..t.2S..^.
                8A 88 BF CB 59 5A 8A 08 74 80 32 53 AA 1C 5E 86   ....YZ..t.2S..^.
                8A 88 BF CB 59 5A 8A 08 74 80 32 53 AA 1C 5E 86   ....YZ..t.2S..^.
                8A 88 BF CB 59 5A 8A 08 74 80 32 53 AA 1C 5E 86   ....YZ..t.2S..^.
                8A 88 BF CB 59 5A 8A 08 74 80 32 53 AA 1C 5E 86   ....YZ..t.2S..^.
                8A 88 BF CB 59 5A 8A 08 74 80 32 53 AA 1C 5E 86   ....YZ..t.2S..^.
                8A 88 BF CB 59 5A 8A 08 74 80 32 53 AA 1C 5E 86   ....YZ..t.2S..^.
                8A 88 BF CB 59 5A 8A 08 74 80 32 53 AA 1C 5E 86   ....YZ..t.2S..^.
                8A 88 BF CB 59 5A 8A 08 74 80 32 53 AA 1C 5E 86   ....YZ..t.2S..^.
                8A 88 BF CB 59 5A 8A 08 74 80 32 53 AA 1C 5E 86   ....YZ..t.2S..^.
                FE DC BA 98 76 54 32 10 0F 1E 2D 3C               ....vT2...-<....

                [C -> S][2114]
                02                                                ................
                0C 00                                             ................
                AE A5 BF 3C 23 65 0C 01 03 1E 27 3C               ...<#e....'<....
             */

            /*
            (Nukei)Discovered so far:
            client requests image by contacting 121.128.134.16 (gdmark.joymax.com) using HTTP protocol over port 15080
            like:
               http://gdmark.joymax.com:15080/SRO_CREST/A96_2132_1.crb (this files is saved in sro-client's RD folder as A96_2132.rd converted to BMP format)

            the returned header indicates, that the returned content is 256 Bytes in size (16 x 16 = 256).  the given type is "application". 

            ToDo:
            1.	Why or when the client starts this request for images ?
            2.	Is the returned array of bytes 16x16 palette entries ?
            3.	if 2 is right, what palette is used ?

            Result:
            1. dont know :-P
            2. Yes, we get a paletted image in 16x16 size (without palette, without size, RAW pixel), so every byte is one pixel. pay attention that the Y coordinates are reversed, so the image is bottom up.
            3. discovered that it is the windows system palette !!
            
             * I dont know if the image really is transfered with 2114 packet !!!
             
            Tools used: PhotoShop, Wireshark, WinHex
             */
            /*Xsense: 
             * Result:
             * 1. I believe send on guild name spawn by 1 byte indication active non active.
             * 1. Simple check would be a bool from database on creation of icon set to true,
             * 1. Deletion item / guild disband / union kick / leave would remove the bool information from db.
             * 2. Indexed color 8 bit setting. (Change extension to bmp / checked photoshop indications).
             * 3.
             * 4. RD should be cache folder, which server could check from registry path to see excisting icons.
             */


            /*
             (Rokky93) Discovered:

            I search a bit about CREST System of Joymax and found an interesting blog.So now i will tell you my results:

            1. Silkroad stores Guild Icons on a stand-alone FTP Server. You can find them in the Media.pk2/type.txt
            2. For example A96_2132_1.crb:
                A - type(Guild or Alliance)
                96- Server ID
                2132 - guild id
                1 - crest id

            When we want to convert it to bmp:
            1. Silkroad uses 16x16 pixel big 8bbp bitmaps with static color palette and no compression. The stored crests are 256B big.
            2. BMP Structure:
                header - static
                meta - static
                palette - static
                bitmap (crest file content) - dynamic

            When we want to send icons to Client
            1. We have to change the FTP Server in the Media.pk2
            2. We must send this packet to Client
                byte - type(alliance, guild) 00 or 01
                dword - guildid
                string - guildname
               dword - crestindex

               For example:
               type                 G
               serverId          187
               guildId             2929
               crestid             3
   
               Client makes this : G187_2929_3.crb and download them from server ( this is my  theory)


            GET /SRO_CREST/A239_282_3.crb HTTP/1.1
            Accept-Encoding: gzip, deflate
            User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.3; .NET4.0C; .NET4.0E)
            Host: gdmark.joymax.com:15080
            Connection: Keep-Alive
            */

        }
    }
}