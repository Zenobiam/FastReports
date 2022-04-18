using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;

namespace FastReport.Export.BIFF8
{
    /// <summary>
    /// Provides API to binary stream
    /// </summary>
    public /*abstract*/ class StreamHelper : MemoryStream
    {
        internal void SkipBytes(int count)
        {
            base.Position = base.Position + count;
        }

        internal ushort ReadUshort()
        {
            byte hi, low;
            low = (byte)ReadByte();
            hi = (byte)ReadByte();
            return (ushort)((int)low + (int)(hi << 8));
        }

        internal uint ReadUint()
        {
            uint b0 = (uint)ReadByte();
            uint b1 = (uint)ReadByte();
            uint b2 = (uint)ReadByte();
            uint b3 = (uint)ReadByte();

            uint result = (b3 << 24) + (b2 << 16) + (b1 << 8) + b0;

            return result;
        }

        internal int ReadInt()
        {
            int b0 = ReadByte();
            int b1 = ReadByte();
            int b2 = ReadByte();
            int b3 = ReadByte();

            int result = (b3 << 24) + (b2 << 16) + (b1 << 8) + b0;

            return result;
        }

        internal double ReadDouble()
        {
            double[] result = new double[1];
            byte[] bytes = new byte[8];
            for (int i = 0; i < 8; i++) bytes[i] = (byte)ReadByte();

            IntPtr ptr = Marshal.AllocHGlobal(8);
            Marshal.Copy(bytes, 0, ptr, 8);
            Marshal.Copy(ptr, result, 0, 1);
            Marshal.FreeHGlobal(ptr);

            return result[0];
        }

        internal string ReadUnicodeString(bool short_len)
        {
            string str = "";
            int num_rich_runs = 0;

            UInt16 Char_Count = short_len ? (UInt16)ReadByte() : ReadUshort();
            byte options = (byte)ReadByte();
            if ((options & 0x8) != 0)
                num_rich_runs = ReadUshort();
            if ((options & 0x1) != 0)
            {
                for (int i = 0; i < Char_Count; i++)
                {
                    char ch = (char)ReadUshort();
                    str += ch;
                }
            }
            else
            {
                for (int i = 0; i < Char_Count; i++)
                {
                    char ch = (char)ReadByte();
                    str += ch;
                }
            }
            return str;
        }

        internal bool CanCompressString(string s)
        {
            int str_len = s.Length;
            for (int i = 0; i < str_len; i++)
                if (s[i] > 255) return false;
            return true;
        }

        internal void WriteUnicodeString(string FontName, bool short_len)
        {
            WriteUnicodeString(FontName, short_len, false);
        }

        internal void WriteUnicodeString(string FontName, bool short_len, bool compress)
        {
            int str_len = FontName.Length;
            if (short_len)
            {
                WriteByte((byte)str_len);
            }
            else
            {
                WriteUshort((ushort)str_len);
            }

            WriteByte(compress ? (byte)0 : (byte)0x001); // Uncompressed

            if (compress)
            {
                for (int i = 0; i < str_len; i++)
                {
                    char ch = FontName[i];
                    if (ch == 0xd)
                        WriteByte((byte)0xa);
                    else
                        WriteByte((byte)ch);
                }
            }
            else
            {
                for (int i = 0; i < str_len; i++)
                {
                    char ch = FontName[i];
                    if (ch == 0xd)
                        WriteUshort((ushort)0xa);
                    else
                        WriteUshort((ushort)ch);
                }
            }
        }

        internal int SizeUnicodeString(string FontName, bool short_len)
        {
            return SizeUnicodeString(FontName, short_len, false);
        }

        internal int SizeUnicodeString(string FontName, bool short_len, bool compress)
        {
            int str_len = FontName.Length;

            if (!compress)
                str_len *= 2;

            return str_len + (short_len ? 2 : 3);
        }

        internal byte[] ReadBytes(int count)
        {
            byte[] result = new byte[count];
            for (int i = 0; i < count; i++)
                result[i] = unchecked((byte)ReadByte());
            return result;
        }

        internal ushort[] ReadUshorts(int count)
        {
            ushort[] result = new ushort[count];
            for (int i = 0; i < count; i++) result[i] = ReadUshort();
            return result;
        }

        internal int[] ReadInts(int count)
        {
            int[] result = new int[count];
            for (int i = 0; i < count; i++) result[i] = ReadInt();
            return result;
        }

        internal void WriteUshort(ushort value)
        {
            unchecked
            {
                byte lo = (byte)(value);
                WriteByte(lo);
                byte hi = (byte)(value >> 8);
                WriteByte(hi);
            }
        }

        internal void WriteUint(uint value)
        {
            unchecked
            {
                byte b0 = (byte)value;
                byte b1 = (byte)(value >> 8);
                byte b2 = (byte)(value >> 16);
                byte b3 = (byte)(value >> 24);

                WriteByte(b0);
                WriteByte(b1);
                WriteByte(b2);
                WriteByte(b3);
            }
        }

        internal void WriteInt(int value)
        {
            unchecked
            {
                byte b0 = (byte)value;
                byte b1 = (byte)(value >> 8);
                byte b2 = (byte)(value >> 16);
                byte b3 = (byte)(value >> 24);

                WriteByte(b0);
                WriteByte(b1);
                WriteByte(b2);
                WriteByte(b3);
            }
        }

        internal void WriteDouble(double value)
        {
            double[] source = new double[1];
            source[0] = value;
            byte[] bytes = new byte[8];

            IntPtr ptr = Marshal.AllocHGlobal(8);
            Marshal.Copy(source, 0, ptr, 1);
            Marshal.Copy(ptr, bytes, 0, bytes.Length);
            Marshal.FreeHGlobal(ptr);

            for (int i = 0; i < 8; i++) WriteByte(bytes[i]);
        }

        internal void WriteBytes(byte[] values)
        {
            for (int i = 0; i < values.Length; i++) WriteByte(values[i]);
        }

        internal void WriteBytes(byte[] values, int start_index, int count)
        {
            for (int i = 0; i < count; i++) WriteByte(values[start_index + i]);
        }

        internal void WriteInts(int[] values)
        {
            WriteInts(values, 0);
        }

        internal void WriteInts(int[] values, int start_index)
        {
            for (int i = start_index; i < values.Length; i++) WriteInt(values[i]);
        }

        internal void WriteUints(uint[] values)
        {
            WriteUints(values, 0);
        }

        internal void WriteUints(uint[] values, int start_index)
        {
            for (int i = start_index; i < values.Length; i++) WriteUint(values[i]);
        }
    }

    internal class DirectoryEntry
    {
        internal enum DirEntryType
        {
            // Type of the entry:
            Empty = 0x00, // 00H = Empty

            UserStorage = 0x01, // 01H = User storage
            UseStream = 0x02, // 02H = User stream
            LockBytes = 0x03, // 03H = LockBytes (unknown)
            Property = 0x04, // 04H = Property (unknown)
            RootStorage = 0x05  // 05H = Root storage
        }

#if false
        // 0 64
        // Character array of the name of the entry, always 16-bit Unicode characters, with trailing
        //zero character (results in a maximum name length of 31 characters)
        byte[] name = new byte[64];

        //64 2
        //Size of the used area of the character buffer of the name (not character count), including
        //the trailing zero character (e.g. 12 for a name with 5 characters: (5+1)∙2 = 12)
        ushort name_size;
#else
        public string entry_name;
#endif

        //  66 1
        // Type of the entry: 00H = Empty 03H = LockBytes (unknown)
        // 01H = User storage 04H = Property (unknown)
        // 02H = User stream 05H = Root storage
        internal DirEntryType type;

        // 67 1 Node colour of the entry: 00H = Red 01H = Black
        internal byte colour;

        // 68 4 DirID of the left child node inside the red-black tree of all direct members of the parent
        // storage (if this entry is a user storage or stream), –1 if there is no left child
        internal int leftChildDirID;

        //72 4 DirID of the right child node inside the red-black tree of all direct members of the parent
        //storage (if this entry is a user storage or stream), –1 if there is no right child
        internal int rightChildDirID;

        // 76 4 DirID of the root node entry of the red-black tree of all storage members (if this entry is a
        //storage,), –1 otherwise
        internal int rootDirID;

        // 80 16 Unique identifier, if this is a storage (not of interest in the following, may be all 0)
        private byte[] uid = new byte[16];

        // 96 4 User flags (not of interest in the following, may be all 0)
        private uint userFlags;

        // 100 8 Time stamp of creation of this entry.
        private byte[] creationTime = new byte[8];

        // 108 8 Time stamp of last modification of this entry
        private byte[] modificationTime = new byte[8];

        // 116 4 SecID of first sector or short-sector, if this entry refers to a stream
        public UInt32 bOF;

        //120 4 Total stream size in bytes, if this entry refers to a stream
        public UInt32 size;

        // 124 4 Not used
        public void Read(StreamHelper File)
        {
            entry_name = "";
            ushort name_size;

            for (int i = 0; i < 32; i++)
            {
                char ch = (char)File.ReadUshort();
                if (ch != '\0') entry_name += ch;
            }
            name_size = File.ReadUshort();

            type = (DirEntryType)File.ReadByte();
            colour = (byte)File.ReadByte();
            leftChildDirID = File.ReadInt();
            rightChildDirID = File.ReadInt();
            rootDirID = File.ReadInt();
            uid = File.ReadBytes(uid.Length);
            userFlags = File.ReadUint();
            creationTime = File.ReadBytes(creationTime.Length);
            modificationTime = File.ReadBytes(modificationTime.Length);
            bOF = File.ReadUint();
            size = File.ReadUint();
            File.SkipBytes(sizeof(UInt32));
        }

        public DirectoryEntry()
        {
            colour = 0x00; // Black
            leftChildDirID = -1;
            rightChildDirID = -1;
            rootDirID = -1;
            uid = new byte[16];
            // 96 4 User flags (not of interest in the following, may be all 0)
            userFlags = 0;
            // 100 8 Time stamp of creation of this entry. Most implementations do not write a valid
            // time stamp, but fill up this space with zero bytes.
            creationTime = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            // 108 8 Time stamp of last modification of this entry. Most implementations do not write
            // a valid time stamp, but fill up this space with zero bytes.
            modificationTime = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

            // Fixed empry values
            this.bOF = 0;
            this.size = 0;
            this.type = DirEntryType.Empty;
            this.entry_name = "";
        }

        public DirectoryEntry(string name, DirEntryType type) : this()
        {
            this.type = type;
            this.entry_name = name;
        }

        internal void Write(StreamHelper File)
        {
            int name_len = entry_name.Length;
            if (name_len > 32) name_len = 32;
            for (int i = 0; i < name_len; i++)
            {
                char ch = entry_name[i];
                File.WriteUshort(ch);
            }
            for (int i = name_len; i < 32; i++)
            {
                File.WriteUshort(0);
            }
            File.WriteUshort((ushort)(2 + name_len * 2));

            File.WriteByte((byte)type);
            File.WriteByte(colour);
            File.WriteInt(leftChildDirID);
            File.WriteInt(rightChildDirID);
            File.WriteInt(rootDirID);
            File.WriteBytes(uid);
            File.WriteUint(userFlags);
            File.WriteBytes(creationTime);
            File.WriteBytes(modificationTime);
            File.WriteUint(bOF);
            File.WriteUint(size);
            File.SkipBytes(sizeof(UInt32));
        }
    }

    internal class CompoundDocumentHeader : StreamHelper
    {
        private byte[] id0 = new byte[8];			// Must be: D0 CF 11 E0 A1 B1 1A E1
        private byte[] uid = new byte[16];
        private ushort revision;			        // Might be: 3E
        private ushort version;			        // Might be: 03
        private ushort byteOrder;			        // Little-Endian: FE FF; Big-Endian: FF FE
        private ushort secSize;			        // Sector size is 2**SecSize bytes
        private ushort shortSecSize;                // Short sector size is 2**ShortSecSize bytes

        //        fixed byte NotUsed1[10];
        private uint satCount;			        // Count of sectors used for the SAT

        public int dir;                     // First sector of the directory stream

                                            //        fixed byte NotUsed2[4];
        public UInt32 minStreamSize;		        // Streams that have sizes less than this value are stored in the short stream

        private int sSAT;				        // First sector of the SSAT
        private uint sSATCount;			        // Count of sectors used for the SSAT
        private int mSAT;				        // First sector of the MSAT
        private uint mSATCount;			        // Count of sectors used for the MSAT
        private int[] mSATSectors = new int[109];	// First 109 SecID values in the MSAT

        private ArrayList sat = new ArrayList();
        private ArrayList shortSAT = new ArrayList();
        private ArrayList additional_mSAT = new ArrayList();

        private uint totalAllocatedSectors;
        private uint totalAllocatedShortSectors;

        internal BIFF8_Container shortStreamContainer;

        public int SectorSize { get { return 1 << secSize; } }
        public int ShortSectorSize { get { return 1 << shortSecSize; } }

        internal int SectorOffset(int index)
        {
            int a = 512 + index * SectorSize;
            return a;
        }

        internal int ShortSectorOffset(int index)
        {
            int a = index * ShortSectorSize;
            return a;
        }

        internal int NextSector(int index)
        {
            return (int)sat[index];
        }

        internal int NextShortSector(int index)
        {
            return (int)shortSAT[index];
        }

        internal void Read()
        {
            id0 = ReadBytes(id0.Length);
            uid = ReadBytes(uid.Length);
            revision = ReadUshort();
            version = ReadUshort();
            byteOrder = ReadUshort();
            secSize = ReadUshort();
            shortSecSize = ReadUshort();
            SkipBytes(10);
            satCount = ReadUint();
            dir = ReadInt();
            SkipBytes(4);
            minStreamSize = ReadUint();
            sSAT = ReadInt();
            sSATCount = ReadUint();
            mSAT = ReadInt();
            mSATCount = ReadUint();
            mSATSectors = ReadInts(mSATSectors.Length);

            // Read SAT into memory
            for (int i = 0; i < satCount; i++)
            {
                if (i >= 109) throw new Exception("Huge SAT not implemented");
                int SAT_Sector = mSATSectors[i];
                int Position = SectorOffset(SAT_Sector);
                this.Position = Position;
                sat.AddRange(new ArrayList(ReadInts(this.SectorSize / sizeof(UInt32))));
            }

            // Read SSAT into memory
            int SSAT_Sector = sSAT;
            for (int i = 0; i < sSATCount; i++)
            {
                int Position = SectorOffset(SSAT_Sector);
                this.Position = Position;
                shortSAT.AddRange(new ArrayList(ReadInts(this.SectorSize / sizeof(UInt32))));
                SSAT_Sector = NextSector(SSAT_Sector);
            }
        }

        internal void Write()
        {
            this.Position = 0;
            WriteBytes(id0);
            WriteBytes(uid);
            WriteUshort(revision);
            WriteUshort(version);
            WriteUshort(byteOrder);
            WriteUshort(secSize);
            WriteUshort(shortSecSize);
            SkipBytes(10);
            WriteUint(satCount);
            WriteInt(dir);
            SkipBytes(4);
            WriteUint(minStreamSize);
            WriteInt(sSAT);
            WriteUint(sSATCount);
            WriteInt(mSAT);
            WriteUint(mSATCount);
            WriteInts(mSATSectors);

            // Write SSAT to memory
            int SSATEntriesPerSector = this.SectorSize / sizeof(UInt32);
            int SSAT_Sector = sSAT;
            for (int i = 0; i < sSATCount;)
            {
                this.Position = SectorOffset(SSAT_Sector);
                for (int j = 0; j < SSATEntriesPerSector; j++)
                {
                    int value = (int)shortSAT[i * SSATEntriesPerSector + j];
                    this.WriteInt(value);
                }
                i++;
                if (i == sSATCount) continue;
                SSAT_Sector = this.AllocateSector(SSAT_Sector);
            }

            // Write SAT to memory
            for (int i = 0; i < satCount; i++)
            {
                int SAT_Sector = 0;
                if (i >= 109) SAT_Sector = (int)this.additional_mSAT[i - 109];//throw new Exception("Huge SAT not implemented");
                else SAT_Sector = this.mSATSectors[i];
                this.Position = SectorOffset(SAT_Sector);
                for (int j = 0; j < this.SectorSize / sizeof(UInt32); j++)
                {
                    int value = -1;
                    int idx = i * this.SectorSize / sizeof(UInt32) + j;
                    if (idx < sat.Count)
                        value = (int)sat[idx];
                    this.WriteInt(value);
                }
            }

            // Write MSAT to memory
            int MSATEntriesPerSector = this.SectorSize / sizeof(Int32);
            int MSAT_Sector = mSAT;
            for (int i = 0; i < mSATCount;)
            {
                this.Position = SectorOffset(MSAT_Sector);
                for (int j = 0; j < MSATEntriesPerSector; j++)
                {
                    int n = i * SSATEntriesPerSector + j;
                    int value = -1;
                    if(n < satCount)
                        value = n >= 109 ? (int)additional_mSAT[n - 109] : mSATSectors[n];
                    this.WriteInt(value);
                }
                i++;
                if (i == mSATCount) continue;
                MSAT_Sector = this.AllocateSector(MSAT_Sector);
            }
        }

        internal void Reset()
        {
            id0 = new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 };
            revision = 0x003e;
            version = 0x0003;
            byteOrder = 0xfffe;
            secSize = 0x0009;
            shortSecSize = 0x0006;
            satCount = 0;
            minStreamSize = 0x00001000;
            mSAT = -2;
            mSATCount = 0;
            // Reset SAT table
            this.sat.Clear();
            // Reset SSAT table
            sSATCount = 0;
            this.shortSAT.Clear();
            // Reset MSAT table
            for (int i = 0; i < mSATSectors.Length; i++)
            {
                mSATSectors[i] = -1;
            }
            this.additional_mSAT.Clear();
            this.totalAllocatedSectors = 0;
            this.totalAllocatedShortSectors = 0;

            this.shortStreamContainer.SetLength(0);
            dir = AllocateSector();
            sSAT = AllocateSector();
            mSAT = AllocateSector();
            mSATCount = 1;
        }

        internal CompoundDocumentHeader(FileStream f)
            : base()
        {
            f.CopyTo(this, (int)f.Length);
            this.Position = 0;
        }

        internal CompoundDocumentHeader()
            : base()
        {
            shortStreamContainer = new BIFF8_Container();
            Reset();
            this.Position = 0;
        }

        internal void FinalWriteToStream(Stream f)
        {
            this.Position = 0;
            this.CopyTo(f, (int)this.Length);
        }

        internal void SetCurentSector(int Dir)
        {
            base.Position = SectorOffset(Dir);
        }

        internal void ReadShortStreamContainer(DirectoryEntry entry)
        {
            int StorePosition = (int)this.Position;
            int SectorOfContainer;
            shortStreamContainer = new BIFF8_Container();
            int idx = 0;
            for (
                SectorOfContainer = (int)entry.bOF;
                SectorOfContainer >= 0;
                SectorOfContainer = this.NextSector(SectorOfContainer)
                )
            {
                this.SetCurentSector(SectorOfContainer);
                for (int i = 0; i < this.SectorSize && idx < entry.size; i++)
                {
                    shortStreamContainer.WriteByte((byte)ReadByte());
                }
            }
            this.Position = StorePosition;
        }

        internal int WriteShortStreamContainer()
        {
            int StorePosition = (int)this.Position;
            int FirstSectorOfContainer = this.AllocateSector();
            int ContainerSize = (int)shortStreamContainer.Length;
            shortStreamContainer.Position = 0;
            for (
                int SectorOfContainer = (int)FirstSectorOfContainer;
                ContainerSize > 0;
                SectorOfContainer = this.AllocateSector(SectorOfContainer)
                )
            {
                this.SetCurentSector(SectorOfContainer);
                for (int i = 0; i < this.SectorSize; i++)
                {
                    byte value = 0xff;
                    if (shortStreamContainer.Position < shortStreamContainer.Length)
                    {
                        value = (byte)shortStreamContainer.ReadByte();
                    }
                    this.WriteByte(value);
                }
                ContainerSize -= this.SectorSize;
                if (ContainerSize <= 0) break;
            }
            this.Position = StorePosition;
            return FirstSectorOfContainer;
        }

        internal int AllocateSector()
        {
            int new_sector;
            new_sector = sat.IndexOf(-1);
            if (new_sector == -1)
            {
                AllocateSATSector();
                new_sector = sat.IndexOf(-1);
                if (new_sector == -1)
                {
                    throw new Exception("Unable allocate SAT sector");
                }
                sat[new_sector] = -2;
                int self = sat.IndexOf(-1);
                sat[self] = -3;
                if (satCount >= 109)
                {
                    //throw new Exception("Huge SAT not implemented");
                    this.additional_mSAT.Add(self);
                }
                else mSATSectors[satCount] = self;
                satCount++;
            }
            totalAllocatedSectors++;
            sat[new_sector] = -2;
            return new_sector;
        }

        internal int AllocateSector(int PrevSector)
        {
            int Sector = AllocateSector();
            sat[PrevSector] = Sector;
            return Sector;
        }

        internal int AllocateShortSector()
        {
            int new_sector;

            new_sector = shortSAT.IndexOf(-1);
            if (new_sector == -1)
            {
                AllocateSSATSector();
                new_sector = shortSAT.IndexOf(-1);
                if (new_sector == -1)
                {
                    throw new Exception("Unable allocate SSAT sector");
                }
            }
            totalAllocatedShortSectors++;
            shortSAT[new_sector] = -2;
            return new_sector;
        }

        internal int AllocateShortSector(int PrevSector)
        {
            int Sector = AllocateShortSector();
            shortSAT[PrevSector] = Sector;
            return Sector;
        }

        private void AllocateSATSector()
        {
            if (satCount > 109)
            {
                //throw new Exception("Large MSAT does not implemented yet");
            }
            int SATEntriesPerSector = this.ShortSectorSize / sizeof(UInt32);
            ArrayList sat_new_sectors = new ArrayList();
#if false // BIG DEAL
            sat_new_sectors.Add(-3); // Sector is used by SAT itself
            for (int i = 1; i < SATEntriesPerSector; i++)
#else
            for (int i = 0; i < SATEntriesPerSector; i++)
#endif
            {
                sat_new_sectors.Add(-1);
            }
            sat.AddRange(sat_new_sectors);
            int local = sat.Count / SATEntriesPerSector;
            //            MSATSectors[SATCount] = (int)
            totalAllocatedSectors++;
        }

        private void AllocateSSATSector()
        {
            int SSATEntriesPerSector = this.SectorSize / sizeof(UInt32);
            ArrayList ssat_new_sectors = new ArrayList();
            //            ssat_new_sectors.Add(-3); // Sector is used by SAT itself
            for (int i = 0; i < SSATEntriesPerSector; i++)
            {
                ssat_new_sectors.Add(-1);
            }
            this.shortSAT.AddRange(ssat_new_sectors);
            int local = shortSAT.Count / SSATEntriesPerSector;
            sSATCount++;
        }

#if false
    internal int AllocateStream(int WriteQueueLength, bool non_packing_stream)
    {
      int BOF;
      int Sector;
      //            int WriteQueueLength = Container.Length;
      if (WriteQueueLength == 0) return -1;
      if (WriteQueueLength >= this.MinStreamSize || non_packing_stream)
      {
        BOF = Sector = this.AllocateSector();

        //            int SourceCopyIndex = 0;

        do
        {
          int CopyCount = (WriteQueueLength < this.SectorSize) ? WriteQueueLength : this.SectorSize;

          //                this.SetCurentSector(Sector);
          //                this.WriteBytes(Container, SourceCopyIndex, CopyCount);

          WriteQueueLength -= CopyCount;

          if (WriteQueueLength == 0) break;

          Sector = this.AllocateSector(Sector);
        } while (true);
      }
      else
      {
        BOF = Sector = AllocateShortSector();

        //            int SourceCopyIndex = 0;

        do
        {
          int CopyCount = (WriteQueueLength < this.ShortSectorSize) ? WriteQueueLength : this.ShortSectorSize;

          //                this.SetCurentSector(Sector);
          //                this.WriteBytes(Container, SourceCopyIndex, CopyCount);

          WriteQueueLength -= CopyCount;

          if (WriteQueueLength == 0) break;

          Sector = this.AllocateShortSector(Sector);
        } while (true);
      }

      return BOF;
    }
#endif
    }

    internal class DirectoryStream : ArrayList
    {
        private CompoundDocumentHeader documentHeader;

        internal new DirectoryEntry this[int index]
        {
            get
            {
                return base[index] as DirectoryEntry;
            }
        }

        internal void Reset()
        {
            for (int i = 0; i < this.Count; i++)
            {
                base[i] = 0;
            }
            base.Clear();
        }

        internal void Read(int Dir, StreamHelper file)
        {
            do
            {
                documentHeader.SetCurentSector(Dir);
                for (int i = 0; i < documentHeader.SectorSize / 128; i++)
                {
                    DirectoryEntry entry = new DirectoryEntry();
                    entry.Read(file);
                    Add(entry);
                }
                Dir = documentHeader.NextSector(Dir);
            } while (Dir != -2);
        }

        internal void Write(int Dir, StreamHelper file)
        {
            int local_directory_index = 0;
            DirectoryEntry entry;
            documentHeader.SetCurentSector(Dir);
            for (int i = 0; i < documentHeader.SectorSize / 128; i++)
            {
                if (local_directory_index < this.Count)
                {
                    entry = this[local_directory_index] as DirectoryEntry;
                }
                else
                {
                    entry = new DirectoryEntry();
                }
                entry.Write(file);
                local_directory_index++;
            }
        }

        public DirectoryStream(CompoundDocumentHeader header)
        {
            documentHeader = header;
        }

        internal DirectoryEntry Add(
            string FileName,
            BIFF8_Stream stream,
            int left, int right)
        {
            DirectoryEntry entry = stream.streamDirEntry;
            entry.entry_name = FileName;
            entry.type = DirectoryEntry.DirEntryType.UseStream;
            entry.size = (uint)stream.Length;
            entry.leftChildDirID = left;
            entry.rightChildDirID = right;
            entry.colour = 0x01;
            stream.Position = 0;

            //            byte[] payload = stream.ReadBytes((int) entry.Size);
            //            entry.BOF = (uint)DocumentHeader.AllocateStream( payload.Length, false );

            stream.streamDirEntry = entry;
            this.Add(entry);
            return entry;
        }

        internal DirectoryEntry Add(string FileName)
        {
            BIFF8_Container Container = documentHeader.shortStreamContainer;
            DirectoryEntry entry = new DirectoryEntry();
            entry.entry_name = FileName;
            entry.type = DirectoryEntry.DirEntryType.RootStorage;
            entry.size = (uint)Container.Length;
            entry.bOF = (uint)documentHeader.WriteShortStreamContainer();
            entry.rootDirID = 3;
            entry.colour = 0x01;
            this.Add(entry);
            return entry;
        }
    }

    /// <summary>
    ///
    /// </summary>
    internal class BIFF8_Container : StreamHelper
    {
    }

    internal class BIFF8_Stream : StreamHelper
    {
        internal DirectoryEntry streamDirEntry;
        internal CompoundDocumentHeader documentHeader;

        private int ShortSectorSize { get { return documentHeader.ShortSectorSize; } }

        internal void Write()
        {
            this.Position = 0;

            int BytesCount = (int)this.Length;

            if (BytesCount < documentHeader.minStreamSize)
            {
                this.streamDirEntry.bOF = (uint)this.documentHeader.AllocateShortSector();
                int Sector = (int)this.streamDirEntry.bOF;
                while (BytesCount > 0)
                {
                    int chunk_size = (BytesCount >= ShortSectorSize) ? ShortSectorSize : BytesCount;
                    byte[] sector = this.ReadBytes(documentHeader.ShortSectorSize);
                    documentHeader.shortStreamContainer.Position = documentHeader.ShortSectorOffset(Sector);
                    documentHeader.shortStreamContainer.Write(sector, 0, chunk_size);
                    BytesCount -= chunk_size;
                    if (BytesCount <= 0) break;
                    Sector = documentHeader.AllocateShortSector(Sector);
                }
            }
            else
            {
                this.streamDirEntry.bOF = (uint)this.documentHeader.AllocateSector();
                int Sector = (int)this.streamDirEntry.bOF;
                while (BytesCount > 0)
                {
                    documentHeader.SetCurentSector(Sector);
                    byte[] sector = this.ReadBytes(documentHeader.SectorSize);
                    documentHeader.Write(sector, 0, documentHeader.SectorSize);
                    BytesCount -= documentHeader.SectorSize;
                    if (BytesCount <= 0) break;
                    Sector = documentHeader.AllocateSector(Sector);
                }
            }
        }

        public BIFF8_Stream(CompoundDocumentHeader document_header, DirectoryEntry entry)
        {
            this.streamDirEntry = entry;
            this.documentHeader = document_header;
            int Sector = (int)entry.bOF;
            int BytesCount = (int)entry.size;
            if (BytesCount < documentHeader.minStreamSize)
            {
                while (BytesCount > 0)
                {
                    int chunk_size = (BytesCount >= ShortSectorSize) ? ShortSectorSize : BytesCount;
                    documentHeader.shortStreamContainer.Position = documentHeader.ShortSectorOffset(Sector);
                    byte[] sector_data = documentHeader.shortStreamContainer.ReadBytes(chunk_size);
                    this.Write(sector_data, 0, chunk_size);
                    Sector = documentHeader.NextShortSector(Sector);
                    BytesCount -= chunk_size;
                }
            }
            else
            {
                while (BytesCount > 0)
                {
                    documentHeader.SetCurentSector(Sector);
                    if (BytesCount >= documentHeader.SectorSize)
                    {
                        byte[] sector = documentHeader.ReadBytes(documentHeader.SectorSize);
                        base.Write(sector, 0, documentHeader.SectorSize);
                        BytesCount -= documentHeader.SectorSize;
                    }
                    else
                    {
                        byte[] sector = documentHeader.ReadBytes(BytesCount);
                        base.Write(sector, 0, BytesCount);
                        BytesCount -= documentHeader.SectorSize;
                    }
                    Sector = documentHeader.NextSector(Sector);
                }
            }
            base.Position = 0;
        }
    }
}