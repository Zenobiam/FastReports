using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

#pragma warning disable CS3001, CS3002, CS3003, CS1591

namespace FastReport.Fonts
{
  /// <summary>
  /// Name table keep human friendly description about font properties, including font names, author and copyright notes
  /// </summary>
  public class NameTableClass : TrueTypeTable, ICollection
  {
    #region "Structure dfinition"
    public enum NameID
    {
      CopyrightNotice = 0,
      FamilyName = 1,
      SubFamilyName = 2,
      UniqueID = 3,
      FullName = 4,
      Version = 5,
      PostscriptName = 6,
      Trademark = 7,
      Manufacturer = 8,
      Designer = 9,
      Description = 10,
      URL_Vendor = 11,
      URL_Designer = 12,
      LicenseDescription = 13,
      LicenseInfoURL = 14,
      PreferredFamily = 16,
      PreferredSubFamily = 17,
      CompatibleFull = 18,
      SampleText = 19,
      PostscriptCID = 20,
      WWS_Family_Name = 21,
      WWS_SubFamily_Name = 22
    }


    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct NamingTableHeader
    {
      [FieldOffset(0)]
      public ushort TableVersion;
      [FieldOffset(2)]
      public ushort Count;
      [FieldOffset(4)]
      public ushort stringOffset;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct NamingRecord
    {
      [FieldOffset(0)]
      public ushort PlatformID;
      [FieldOffset(2)]
      public ushort EncodingID;
      [FieldOffset(4)]
      public ushort LanguageID;
      [FieldOffset(6)]
      public ushort NameID;
      [FieldOffset(8)]
      public ushort Length;
      [FieldOffset(10)]
      public ushort Offset;
    }
    #endregion

    NamingTableHeader name_header;
    IntPtr namerecord_ptr;
    IntPtr string_storage_ptr;
    Dictionary<NameID, List<string>> names = new Dictionary<NameID, List<string>>();

        public Dictionary<NameID, List<string>> NamesDict
        {
            get
            {
                return names;
            }
        }

    public int Count
    {
      get
      {
        return name_header.Count;
      }
    }

    public object SyncRoot
    {
      get
      {
        return null; // Names.SyncRoot;
      }
    }

    public bool IsSynchronized
    {
      get
      {
        return true; // Names.IsSynchronized;
      }
    }

    private void ChangeEndian()
    {
      name_header.TableVersion = SwapUInt16(name_header.TableVersion);
      name_header.Count = SwapUInt16(name_header.Count);
      name_header.stringOffset = SwapUInt16(name_header.stringOffset);
    }

    internal override void Load(IntPtr font)
    {
      IntPtr nameheader_ptr = Increment(font, (int)this.Offset);
      name_header = (NamingTableHeader)Marshal.PtrToStructure(nameheader_ptr, typeof(NamingTableHeader));

      ChangeEndian();

      namerecord_ptr = Increment(nameheader_ptr, Marshal.SizeOf(name_header));
      string_storage_ptr = Increment(nameheader_ptr, (int)name_header.stringOffset);

      // Parse table to local storage
      IntPtr record_ptr = namerecord_ptr;

      for (int i = 0; i < name_header.Count; i++)
      {
        NamingRecord name_rec = (NamingRecord)Marshal.PtrToStructure(record_ptr, typeof(NamingRecord));
        record_ptr = Increment(record_ptr, Marshal.SizeOf(name_rec));

        name_rec.PlatformID = SwapUInt16(name_rec.PlatformID);
        name_rec.EncodingID = SwapUInt16(name_rec.EncodingID);
        name_rec.LanguageID = SwapUInt16(name_rec.LanguageID);
        name_rec.NameID = SwapUInt16(name_rec.NameID);
        name_rec.Length = SwapUInt16(name_rec.Length);
        name_rec.Offset = SwapUInt16(name_rec.Offset);

        byte[] Temp = new byte[name_rec.Length];
        IntPtr string_ptr = Increment(string_storage_ptr, name_rec.Offset);
        Marshal.Copy(string_ptr, Temp, 0, (int)Temp.Length);

        string value;
        if (name_rec.PlatformID == 1)
        {
          value = Encoding.GetEncoding("utf-8").GetString(Temp);
        }
        else
        {
          value = Encoding.GetEncoding(1201).GetString(Temp);
        }
        NameID id = (NameID)name_rec.NameID;
        List<string> values;
        if (!names.ContainsKey(id))
        {
          values = new List<string>();
          names.Add(id, values);
        }
        else
        {
          values = (List<string>)names[id];
        }
        if (!values.Contains(value))
        {
          values.Add(value);
        }
#if _DEBUG
                else
                {
                    Debug.WriteLine("Skip duplicated name: " + value);
                }
#endif
      }
    }

    public void CopyTo(Array array, int index)
    {
      throw new NotImplementedException();
    }

    public IEnumerator GetEnumerator()
    {
      return names.Keys.GetEnumerator();
    }

    public ICollection this[NameID Index]
    {
      get
      {
#if true
        ICollection rec = null;
        if (names.ContainsKey(Index))
        {
          rec = names[Index] as ICollection;
        }
        return rec;
#else
					IntPtr record_ptr = namerecord_ptr;

					for (int i = 0; i < name_header.Count; i++)
					{
					NamingRecord name_rec = (NamingRecord)Marshal.PtrToStructure(record_ptr, typeof(NamingRecord));
					record_ptr = Increment(record_ptr, Marshal.SizeOf(name_rec));

					name_rec.PlatformID = SwapUInt16(name_rec.PlatformID);
					name_rec.EncodingID = SwapUInt16(name_rec.EncodingID);
					name_rec.LanguageID = SwapUInt16(name_rec.LanguageID);
					name_rec.NameID = SwapUInt16(name_rec.NameID);
					name_rec.Length = SwapUInt16(name_rec.Length);
					name_rec.Offset = SwapUInt16(name_rec.Offset);

					if (((name_rec.PlatformID == 3 && name_rec.EncodingID == 1) || name_rec.PlatformID == 0) &&
					(NameID)name_rec.NameID == Index)
					{
					byte[] Temp = new byte[name_rec.Length];
					IntPtr string_ptr = Increment(string_storage_ptr, name_rec.Offset);
					Marshal.Copy(string_ptr, Temp, 0, (int)Temp.Length);

					return Encoding.GetEncoding(1201).GetString(Temp);
					}
					}
#if !DEBUG_TABLE
					record_ptr = namerecord_ptr;
					for (int i = 0; i < name_header.Count; i++)
					{
					NamingRecord name_rec = (NamingRecord)Marshal.PtrToStructure(record_ptr, typeof(NamingRecord));
					record_ptr = Increment(record_ptr, Marshal.SizeOf(name_rec));

					name_rec.PlatformID = SwapUInt16(name_rec.PlatformID);
					name_rec.EncodingID = SwapUInt16(name_rec.EncodingID);
					name_rec.LanguageID = SwapUInt16(name_rec.LanguageID);
					name_rec.NameID = SwapUInt16(name_rec.NameID);
					name_rec.Length = SwapUInt16(name_rec.Length);
					name_rec.Offset = SwapUInt16(name_rec.Offset);

					if ((NameID)name_rec.NameID == Index)
					{
					continue;
					}
					}
#endif
					return null;
#endif
      }
    }

    public NameTableClass(TrueTypeTable src) : base(src) { }
  }

}
#pragma warning restore