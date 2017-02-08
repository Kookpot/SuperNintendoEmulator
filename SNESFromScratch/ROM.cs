using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SNESFromScratch
{
    public class ROM
    {
        #region HIDE

        private readonly string[] _nintendoLicensees = 
        {
            "Unlicensed",
            "Nintendo",
            "Rocket Games/Ajinomoto",
            "Imagineer-Zoom",
            "Gray Matter",
            "Zamuse",
            "Falcom",
            null,
            "Capcom",
            "Hot B Co.",
            "Jaleco",
            "Coconuts Japan",
            "Coconuts Japan/G.X.Media",
            "Micronet",
            "Technos",
            "Mebio Software",
            "Shouei System",
            "Starfish",
            null,
            "Mitsui Fudosan/Dentsu",
            null,
            "Warashi Inc.",
            null,
            "Nowpro",
            null,
            "Game Village",
            "IE Institute",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "Banarex",
            "Starfish",
            "Infocom",
            "Electronic Arts Japan",
            null,
            "Cobra Team",
            "Human/Field",
            "KOEI",
            "Hudson Soft",
            "S.C.P./Game Village",
            "Yanoman",
            null,
            "Tecmo Products",
            "Japan Glary Business",
            "Forum/OpenSystem",
            "Virgin Games (Japan)",
            "SMDE",
            "Yojigen",
            null,
            "Daikokudenki",
            null,
            null,
            null,
            null,
            null,
            "Creatures Inc.",
            "TDK Deep Impresion",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "Destination Software/KSS",
            "Sunsoft/Tokai Engineering",
            "POW (Planning Office Wada)/VR 1 Japan",
            "Micro World",
            null,
            "San-X",
            "Enix",
            "Loriciel/Electro Brain",
            "Kemco Japan",
            "Seta Co.,Ltd.",
            "Culture Brain",
            "Irem Corp.",
            "Palsoft",
            "Visit Co., Ltd.",
            "Intec",
            "System Sacom",
            "Poppo",
            "Ubisoft Japan",
            null,
            "Media Works",
            "NEC InterChannel",
            "Tam",
            "Gajin/Jordan",
            "Smilesoft",
            null,
            null,
            "Mediakite",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "Viacom",
            "Carrozzeria",
            "Dynamic",
            null,
            "Magifact",
            "Hect",
            "Codemasters",
            "Taito/GAGA Communications",
            "Laguna",
            "Telstar Fun & Games/Event/Taito",
            null,
            "Arcade Zone Ltd.",
            "Entertainment International/Empire Software",
            "Loriciel",
            "Gremlin Graphics",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "Seika Corp.",
            "UBI SOFT Entertainment Software",
            "Sunsoft US",
            null,
            "Life Fitness",
            null,
            "System 3",
            "Spectrum Holobyte",
            null,
            "Irem",
            null,
            "Raya Systems",
            "Renovation Products",
            "Malibu Games",
            null,
            "Eidos/U.S. Gold",
            "Playmates Interactive",
            null,
            null,
            "Fox Interactive",
            "Time Warner Interactive",
            null,
            null,
            null,
            null,
            null,
            "Disney Interactive",
            null,
            "Black Pearl",
            null,
            "Advanced Productions",
            null,
            null,
            "GT Interactive",
            "RARE",
            "Crave Entertainment",
            "Absolute Entertainment",
            "Acclaim",
            "Activision",
            "American Sammy",
            "Take 2/GameTek",
            "Hi Tech",
            "LJN Ltd.",
            null,
            "Mattel",
            null,
            "Mindscape/Red Orb Entertainment",
            "Romstar",
            "Taxan",
            "Midway/Tradewest",
            null,
            "American Softworks Corp.",
            "Majesco Sales Inc.",
            "3DO",
            null,
            null,
            "Hasbro",
            "NewKidCo",
            "Telegames",
            "Metro3D",
            null,
            "Vatical Entertainment",
            "LEGO Media",
            null,
            "Xicat Interactive",
            "Cryo Interactive",
            null,
            null,
            "Red Storm Entertainment",
            "Microids",
            null,
            "Conspiracy/Swing",
            "Titus",
            "Virgin Interactive",
            "Maxis",
            null,
            "LucasArts Entertainment",
            null,
            null,
            "Ocean",
            null,
            "Electronic Arts",
            null,
            "Laser Beam",
            null,
            null,
            "Elite Systems",
            "Electro Brain",
            "The Learning Company",
            "BBC",
            null,
            "Software 2000",
            null,
            "BAM! Entertainment",
            "Studio 3",
            null,
            null,
            null,
            "Classified Games",
            null,
            "TDK Mediactive",
            null,
            "DreamCatcher",
            "JoWood Produtions",
            "SEGA",
            "Wannado Edition",
            "LSP (Light & Shadow Prod.)",
            "ITE Media",
            "Infogrames",
            "Interplay",
            "JVC (US)",
            "Parker Brothers",
            null,
            "SCI (Sales Curve Interactive)/Storm",
            null,
            null,
            "THQ Software",
            "Accolade Inc.",
            "Triffix Entertainment",
            null,
            "Microprose Software",
            "Universal Interactive/Sierra/Simon & Schuster",
            null,
            "Kemco",
            "Rage Software",
            "Encore",
            null,
            "Zoo",
            "Kiddinx",
            "Simon & Schuster Interactive",
            "Asmik Ace Entertainment Inc./AIA",
            "Empire Interactive",
            null,
            null,
            "Jester Interactive",
            null,
            "Rockstar Games",
            "Scholastic",
            "Ignition Entertainment",
            "Summitsoft",
            "Stadlbauer",
            null,
            null,
            null,
            "Misawa",
            "Teichiku",
            "Namco Ltd.",
            "LOZC",
            "KOEI",
            null,
            "Tokuma Shoten Intermedia",
            "Tsukuda Original",
            "DATAM-Polystar",
            null,
            null,
            "Bullet-Proof Software",
            "Vic Tokai Inc.",
            null,
            "Character Soft",
            "I'Max",
            "Saurus",
            null,
            null,
            "General Entertainment",
            null,
            null,
            "I'Max",
            "Success",
            null,
            "SEGA Japan",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "Takara",
            "Chun Soft",
            "Video System Co., Ltd./McO'River",
            "BEC",
            null,
            "Varie",
            "Yonezawa/S'pal",
            "Kaneko",
            null,
            "Victor Interactive Software/Pack-in-Video",
            "Nichibutsu/Nihon Bussan",
            "Tecmo",
            "Imagineer",
            null,
            null,
            "Nova",
            "Den'Z",
            "Bottom Up",
            null,
            "TGL (Technical Group Laboratory)",
            null,
            "Hasbro Japan",
            null,
            "Marvelous Entertainment",
            null,
            "Keynet Inc.",
            "Hands-On Entertainment",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "Telenet",
            "Hori",
            null,
            null,
            "Konami",
            "K.Amusement Leasing Co.",
            "Kawada",
            "Takara",
            null,
            "Technos Japan Corp.",
            "JVC (Europe/Japan)/Victor Musical Industries",
            null,
            "Toei Animation",
            "Toho",
            null,
            "Namco",
            "Media Rings Corp.",
            "J-Wing",
            null,
            "Pioneer LDC",
            "KID",
            "Mediafactory",
            null,
            null,
            null,
            "Infogrames Hudson",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "Acclaim Japan",
            "ASCII Co./Nexoft",
            "Bandai",
            null,
            "Enix",
            null,
            "HAL Laboratory/Halken",
            "SNK",
            null,
            "Pony Canyon Hanbai",
            "Culture Brain",
            "Sunsoft",
            "Toshiba EMI",
            "Sony Imagesoft",
            null,
            "Sammy",
            "Magical",
            "Visco",
            null,
            "Compile",
            null,
            "MTO Inc.",
            null,
            "Sunrise Interactive",
            null,
            "Global A Entertainment",
            "Fuuki",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "Taito",
            null,
            "Kemco",
            "Square",
            "Tokuma Shoten",
            "Data East",
            "Tonkin House",
            null,
            "KOEI",
            null,
            "Konami/Ultra/Palcom",
            "NTVIC/VAP",
            "Use Co., Ltd.",
            "Meldac",
            "Pony Canyon (Japan)/FCI (US)",
            "Angel/Sotsu Agency/Sunrise",
            "Yumedia/Aroma Co., Ltd.",
            null,
            null,
            "Boss",
            "Axela/Crea-Tech",
            "Sekaibunka-Sha/Sumire kobo/Marigul Management Inc.",
            "Konami Computer Entertainment Osaka",
            null,
            null,
            "Enterbrain",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "Taito/Disco",
            "Sofel",
            "Quest Corp.",
            "Sigma",
            "Ask Kodansha",
            null,
            "Naxat",
            "Copya System",
            "Capcom Co., Ltd.",
            "Banpresto",
            "TOMY",
            "Acclaim/LJN Japan",
            null,
            "NCS",
            "Human Entertainment",
            "Altron",
            "Jaleco",
            "Gaps Inc.",
            null,
            null,
            null,
            null,
            null,
            "Elf",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "Jaleco",
            null,
            "Yutaka",
            "Varie",
            "T&ESoft",
            "Epoch Co., Ltd.",
            null,
            "Athena",
            "Asmik",
            "Natsume",
            "King Records",
            "Atlus",
            "Epic/Sony Records (Japan)",
            null,
            "IGS (Information Global Service)",
            null,
            "Chatnoir",
            "Right Stuff",
            null,
            "NTT COMWARE",
            null,
            "Spike",
            "Konami Computer Entertainment Tokyo",
            "Alphadream Corp.",
            null,
            "Sting",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "A Wave",
            "Motown Software",
            "Left Field Entertainment",
            "Extreme Entertainment Group",
            "TecMagik",
            null,
            null,
            null,
            null,
            "Cybersoft",
            null,
            "Psygnosis",
            null,
            null,
            "Davidson/Western Tech.",
            "Unlicensed",
            null,
            null,
            null,
            null,
            "The Game Factory Europe",
            "Hip Games",
            "Aspyr",
            null,
            null,
            "Mastiff",
            "iQue",
            "Digital Tainment Pool",
            "XS Games",
            "Daiwon",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "PCCW Japan",
            null,
            null,
            "KiKi Co. Ltd.",
            "Open Sesame Inc.",
            "Sims",
            "Broccoli",
            "Avex",
            "D3 Publisher",
            null,
            "Konami Computer Entertainment Japan",
            null,
            "Square-Enix",
            "KSG",
            "Micott & Basara Inc.",
            null,
            "Orbital Media",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "The Game Factory USA",
            null,
            null,
            "Treasure",
            "Aruze",
            "Ertain",
            "SNK Playmore",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "Yojigen"
        };

        private readonly uint[] crc32Table = new uint[]
        {
            0x00000000, 0x77073096, 0xee0e612c, 0x990951ba, 0x076dc419, 0x706af48f,
            0xe963a535, 0x9e6495a3, 0x0edb8832, 0x79dcb8a4, 0xe0d5e91e, 0x97d2d988,
            0x09b64c2b, 0x7eb17cbd, 0xe7b82d07, 0x90bf1d91, 0x1db71064, 0x6ab020f2,
            0xf3b97148, 0x84be41de, 0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7,
            0x136c9856, 0x646ba8c0, 0xfd62f97a, 0x8a65c9ec, 0x14015c4f, 0x63066cd9,
            0xfa0f3d63, 0x8d080df5, 0x3b6e20c8, 0x4c69105e, 0xd56041e4, 0xa2677172,
            0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b, 0x35b5a8fa, 0x42b2986c,
            0xdbbbc9d6, 0xacbcf940, 0x32d86ce3, 0x45df5c75, 0xdcd60dcf, 0xabd13d59,
            0x26d930ac, 0x51de003a, 0xc8d75180, 0xbfd06116, 0x21b4f4b5, 0x56b3c423,
            0xcfba9599, 0xb8bda50f, 0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924,
            0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d, 0x76dc4190, 0x01db7106,
            0x98d220bc, 0xefd5102a, 0x71b18589, 0x06b6b51f, 0x9fbfe4a5, 0xe8b8d433,
            0x7807c9a2, 0x0f00f934, 0x9609a88e, 0xe10e9818, 0x7f6a0dbb, 0x086d3d2d,
            0x91646c97, 0xe6635c01, 0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e,
            0x6c0695ed, 0x1b01a57b, 0x8208f4c1, 0xf50fc457, 0x65b0d9c6, 0x12b7e950,
            0x8bbeb8ea, 0xfcb9887c, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3, 0xfbd44c65,
            0x4db26158, 0x3ab551ce, 0xa3bc0074, 0xd4bb30e2, 0x4adfa541, 0x3dd895d7,
            0xa4d1c46d, 0xd3d6f4fb, 0x4369e96a, 0x346ed9fc, 0xad678846, 0xda60b8d0,
            0x44042d73, 0x33031de5, 0xaa0a4c5f, 0xdd0d7cc9, 0x5005713c, 0x270241aa,
            0xbe0b1010, 0xc90c2086, 0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f,
            0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4, 0x59b33d17, 0x2eb40d81,
            0xb7bd5c3b, 0xc0ba6cad, 0xedb88320, 0x9abfb3b6, 0x03b6e20c, 0x74b1d29a,
            0xead54739, 0x9dd277af, 0x04db2615, 0x73dc1683, 0xe3630b12, 0x94643b84,
            0x0d6d6a3e, 0x7a6a5aa8, 0xe40ecf0b, 0x9309ff9d, 0x0a00ae27, 0x7d079eb1,
            0xf00f9344, 0x8708a3d2, 0x1e01f268, 0x6906c2fe, 0xf762575d, 0x806567cb,
            0x196c3671, 0x6e6b06e7, 0xfed41b76, 0x89d32be0, 0x10da7a5a, 0x67dd4acc,
            0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5, 0xd6d6a3e8, 0xa1d1937e,
            0x38d8c2c4, 0x4fdff252, 0xd1bb67f1, 0xa6bc5767, 0x3fb506dd, 0x48b2364b,
            0xd80d2bda, 0xaf0a1b4c, 0x36034af6, 0x41047a60, 0xdf60efc3, 0xa867df55,
            0x316e8eef, 0x4669be79, 0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236,
            0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f, 0xc5ba3bbe, 0xb2bd0b28,
            0x2bb45a92, 0x5cb36a04, 0xc2d7ffa7, 0xb5d0cf31, 0x2cd99e8b, 0x5bdeae1d,
            0x9b64c2b0, 0xec63f226, 0x756aa39c, 0x026d930a, 0x9c0906a9, 0xeb0e363f,
            0x72076785, 0x05005713, 0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0x0cb61b38,
            0x92d28e9b, 0xe5d5be0d, 0x7cdcefb7, 0x0bdbdf21, 0x86d3d2d4, 0xf1d4e242,
            0x68ddb3f8, 0x1fda836e, 0x81be16cd, 0xf6b9265b, 0x6fb077e1, 0x18b74777,
            0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c, 0x8f659eff, 0xf862ae69,
            0x616bffd3, 0x166ccf45, 0xa00ae278, 0xd70dd2ee, 0x4e048354, 0x3903b3c2,
            0xa7672661, 0xd06016f7, 0x4969474d, 0x3e6e77db, 0xaed16a4a, 0xd9d65adc,
            0x40df0b66, 0x37d83bf0, 0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9,
            0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6, 0xbad03605, 0xcdd70693,
            0x54de5729, 0x23d967bf, 0xb3667a2e, 0xc4614ab8, 0x5d681b02, 0x2a6f2b94,
            0xb40bbe37, 0xc30c8ea1, 0x5a05df1b, 0x2d02ef8d
        };

        #endregion

        public struct ROMHeader
        {
            public string Name;
            public bool HiROM;
            public byte Type;
            public byte Banks;
            public byte SRAMSize;
        }

        public ROMHeader Header;
        private int _headerCount;
        private Encoding _ascii = new ASCIIEncoding();
        private readonly byte[] _nsrtHeader = new byte[32];

        public byte[][] ROMData; //(0, &H7FFF)

        public void LoadRom(string fileName)
        {
            var data = File.ReadAllBytes(fileName);
            //HeaderRemove(data.Length, ref data);
            //var hi_score = ScoreHiROM(data, false);
            //var lo_score = ScoreLoROM(data, false);

            var banks = data.Length/0x8000;
            //var banksHiROM = data.Length/0x10000;
            ROMData = new byte[banks][];
            for (var i = 0; i < banks; i++)
            {
                ROMData[i] = new byte[0x8000];
                Buffer.BlockCopy(data, i*0x8000, ROMData[i], 0, 0x8000);
            }
            var headerBank = 0;
            if (ROMData[1][0x7FDC] + (ROMData[1][0x7FDD]*0x100) + ROMData[1][0x7FDE] + (ROMData[1][0x7FDF]*0x100) == 0xFFFF)
            {
                headerBank = 1;
            }
            var nameBytes = new byte[20];
            Array.Copy(ROMData[headerBank], 0x7FC0, nameBytes, 0, 20);
            Header.Name = Encoding.UTF8.GetString(nameBytes);
            Header.Name = Header.Name.Trim();
            Header.HiROM = headerBank != 0;
            if (!Header.HiROM)
            {
                //ROMData = new byte[2*banks][];
                //for (var i = 0; i < banksHiROM; i++)
                //{
                //    ROMData[i*2 + 1] = new byte[0x8000];
                //    Buffer.BlockCopy(data, i*0x8000, ROMData[i*2 + 1], 0, 0x8000);
                //}
                //for (var i = 0; i < banksHiROM; i++)
                //{
                //    ROMData[i*2] = new byte[0x8000];
                //    Buffer.BlockCopy(data, banksHiROM*0x8000 + i*0x8000, ROMData[i*2], 0, 0x8000);
                //}
            }

            Header.Type = ROMData[headerBank][0x7FD6];
            Header.Banks = (byte) banks;
            Header.SRAMSize = ROMData[headerBank][0x7FD8];
        }

        private int HeaderRemove(int size, ref byte[] buf)
        {
	        var	calcsize = (size / 0x2000) * 0x2000;

	        if (size - calcsize == 512)
	        {
		        const int nsrtHeadOffSet = 0x1D0; // NSRT Header Location

		        // detect NSRT header
                if (_ascii.GetString(buf, nsrtHeadOffSet+24, 4) != "NSRT")
                {
			        if (buf[nsrtHeadOffSet+28] == 22)
			        {
                        if (((SumBytes(buf, nsrtHeadOffSet, _nsrtHeader.Length) & 0xFF) == buf[nsrtHeadOffSet + 30]) &&
                            (buf[nsrtHeadOffSet + 30] + buf[nsrtHeadOffSet + 31] == 255) && ((buf[nsrtHeadOffSet] & 0x0F) <= 13) &&
                            (((buf[nsrtHeadOffSet] & 0xF0) >> 4) <= 3) && ((buf[nsrtHeadOffSet] & 0xF0) >> 4 != 0))
                        {
                            Buffer.BlockCopy(buf, nsrtHeadOffSet, _nsrtHeader, 0, _nsrtHeader.Length);
                        }
			        }
		        }
	            buf = RemoveFirstBytes(buf, 512);
		        _headerCount++;
		        size -= 512;
	        }

	        return size;
        }

        public byte[] RemoveFirstBytes(byte[] buf, int count)
        {
            var newArray = new byte[buf.Length - count];
            Buffer.BlockCopy(buf, count, newArray,0, buf.Length-count);
            return newArray;
        }

        private static long SumBytes(IList<byte> bytes, int startIndex, int length)
        {
            var sum = 0;
            for (var intI = startIndex; intI < startIndex + length; intI++)
            {
                sum += bytes[intI];
            }
            return sum;
        }

        private static int ScoreHiROM(IList<byte> rom, bool skipheader, int romoff = 0, int calculatedsize = 0)
        {
            var offset = 0xff00 + romoff + (skipheader ? 0x200 : 0);
	        var score = 0;

            if ((rom[offset + 0xd5] & 0x1) != 0)
	        {
	            score += 2;
	        }

            // Mode23 is SA-1
            if (rom[offset + 0xd5] == 0x23)
            {
                score -= 2;
            }

            if (rom[offset + 0xd4] == 0x20)
            {
                score += 2;
            }

            if ((rom[offset + 0xdc] + (rom[offset + 0xdd] << 8)) + (rom[offset + 0xde] + (rom[offset + 0xdf] << 8)) == 0xffff)
	        {
		        score += 2;
                if (0 != (rom[offset + 0xde] + (rom[offset + 0xdf] << 8)))
                {
                    score++;
                }
	        }

            if (rom[offset + 0xda] == 0x33)
            {
                score += 2;
            }

            if ((rom[offset + 0xd5] & 0xf) < 4)
            {
                score += 2;
            }

            if ((rom[offset + 0xfd] & 0x80) == 0)
            {
                score -= 6;
            }

            if ((rom[offset + 0xfc] + (rom[offset + 0xfd] << 8)) > 0xffb0)
            {
                score -= 2; // reduced after looking at a scan by Cowering
            }

            if (calculatedsize > 1024 * 1024 * 3)
            {
                score += 4;
            }

            if ((1 << (rom[offset + 0xd7] - 7)) > 48)
            {
                score -= 1;
            }

            if (!IsAscii(rom, offset + 0xb0, 6))
            {
                score -= 1;
            }

            if (!IsAscii(rom, offset + 0xc0, 23))
            {
                score -= 1;
            }

            return score;
        }

        private static int ScoreLoROM(IList<byte> rom, bool skipheader, int romoff = 0, int calculatedsize = 0)
        {
	        var offset = 0x7f00 + romoff + (skipheader ? 0x200 : 0);
	        var score = 0;

            if ((rom[offset + 0xd5] & 0x1) == 0)
            {
                score += 3;
            }

            // Mode23 is SA-1
            if (rom[offset + 0xd5] == 0x23)
            {
                score += 2;
            }

            if ((rom[offset + 0xdc] + (rom[offset + 0xdd] << 8)) + (rom[offset + 0xde] + (rom[offset + 0xdf] << 8)) == 0xffff)
	        {
		        score += 2;
                if (0 != (rom[offset + 0xde] + (rom[offset + 0xdf] << 8)))
                {
                    score++;
                }
	        }

            if (rom[offset + 0xda] == 0x33)
            {
                score += 2;
            }

            if ((rom[offset + 0xd5] & 0xf) < 4)
            {
                score += 2;
            }

            if ((rom[offset + 0xfd] & 0x80) == 0)
            {
                score -= 6;
            }

            if ((rom[offset + 0xfc] + (rom[offset + 0xfd] << 8)) > 0xffb0)
            {
                score -= 2; // reduced per Cowering suggestion
            }

            if (calculatedsize <= 1024 * 1024 * 16)
            {
                score += 2;
            }

            if ((1 << (rom[offset + 0xd7] - 7)) > 48)
            {
                score -= 1;
            }

            if (!IsAscii(rom, offset + 0xb0, 6))
            {
                score -= 1;
            }

            if (!IsAscii(rom, offset + 0xc0, 23))
            {
                score -= 1;
            }

            return score;
        }

        private static bool IsAscii(byte b)
        {
            return b >= 32 && b <= 126;
        }

        private static bool IsAscii(IList<byte> b, int index, int count)
        {
            var found = false;
            for (var intI = index; intI < index + count;intI++)
            {
                if (IsAscii(b[intI])) continue;
                found = true;
                break;
            }
            return !found;
        }
    }
}