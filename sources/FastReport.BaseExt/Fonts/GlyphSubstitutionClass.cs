using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FastReport.Fonts.LinqExts;

#pragma warning disable CS3001, CS3002, CS3003, CS1591  // Missing XML comment for publicly visible type or member

namespace FastReport.Fonts
{
    /// <summary>
    /// GlyphSubstitution table
    /// </summary>
    public class GlyphSubstitutionClass : TrueTypeTable
  {
    #region "Structure definition"
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct GSUB_Header
    {
      [FieldOffset(0)]
      public uint Version; //  	Version of the GSUB table-initially set to 0x00010000
      [FieldOffset(4)]
      public ushort ScriptList; // 	Offset to ScriptList table-from beginning of GSUB table
      [FieldOffset(6)]
      public ushort FeatureList; // 	Offset to FeatureList table-from beginning of GSUB table
      [FieldOffset(8)]
      public ushort LookupList; // 	Offset to LookupList table-from beginning of GSUB table
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ScriptListTable
    {
      [FieldOffset(0)]
      public ushort CountScripts; //  	Count of ScriptListRecord
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ScriptListRecord
    {
      [FieldOffset(0)]
      public uint ScriptTag; //  	4-byte ScriptTag identifier
      [FieldOffset(4)]
      public ushort ScriptOffset; // 	Offset to Script table-from beginning of ScriptList        
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ScriptTable
    {
      [FieldOffset(0)]
      public ushort DefaultLangSys; //  	Offset to DefaultLangSys table-from beginning of Script table-may be NULL
      [FieldOffset(2)]
      public ushort LangSysCount;   // 	Number of LangSysRecords for this script-excluding the DefaultLangSys
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct LangSysRecord
    {
      [FieldOffset(0)]
      public uint LangSysTag; //  	4-byte LangSysTag identifier
      [FieldOffset(4)]
      public ushort LangSys;    // 	Offset to LangSys table-from beginning of Script table        
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct LangSysTable
    {
      [FieldOffset(0)]
      public ushort LookupOrder; //  	= NULL (reserved for an offset to a reordering table)
      [FieldOffset(2)]
      public ushort ReqFeatureIndex; // 	Index of a feature required for this language system- if no required features = 0xFFFF
      [FieldOffset(4)]
      public ushort FeatureCount; // 	Number of FeatureIndex values for this language system-excludes the required feature    
    }

    // Related to feature table

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct FeaturesListTable
    {
      [FieldOffset(0)]
      public ushort CountFeatures; //  	Count of FeaturesListRecord
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct FeatureRecord
    {
      [FieldOffset(0)]
      public uint FeatureTag; //  	4-byte feature identification tag
      [FieldOffset(4)]
      public ushort Feature; // 	Offset to Feature table-from beginning of FeatureList        
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct FeatureTable
    {
      [FieldOffset(0)]
      public ushort FeatureParams;
      [FieldOffset(2)]
      public ushort LookupCount;
    }

    #endregion

    private GSUB_Header header;
    private IntPtr gsub_ptr;
    private Hashtable script_list = new Hashtable();
    private LookupEntry[] lookup_list;

    public IEnumerable<string> Scripts
    {
      get
      {
        foreach (string script_str in script_list.Keys)
          yield return script_str;
      }
    }

    private ushort[] LoadFeature(uint feature_idx, out string FeatureTag)
    {

      IntPtr feature_list_table_ptr = Increment(gsub_ptr, (int)header.FeatureList);
      ushort feature_count = SwapUInt16((ushort)Marshal.PtrToStructure(feature_list_table_ptr, typeof(ushort)));
      if (feature_idx >= feature_count) throw new Exception("Feature index out of bound");

      IntPtr feature_record_ptr = Increment(feature_list_table_ptr, (int)(sizeof(ushort) + feature_idx * 6));
      FeatureRecord feature_record = (FeatureRecord)Marshal.PtrToStructure(feature_record_ptr, typeof(FeatureRecord));
      feature_record.Feature = SwapUInt16(feature_record.Feature);

      FeatureTag = "" +
          (char)(0xff & feature_record.FeatureTag) +
          (char)(0xff & (feature_record.FeatureTag >> 8)) +
          (char)(0xff & (feature_record.FeatureTag >> 16)) +
          (char)(0xff & (feature_record.FeatureTag >> 24));

      IntPtr feature_table_ptr = Increment(feature_list_table_ptr, feature_record.Feature);
      FeatureTable feature_table = (FeatureTable)Marshal.PtrToStructure(feature_table_ptr, typeof(FeatureTable));
      feature_table.LookupCount = SwapUInt16(feature_table.LookupCount);

      ushort[] OffsetLookupList = new ushort[feature_table.LookupCount];

      IntPtr lookup_list_ptr = Increment(feature_table_ptr, Marshal.SizeOf(feature_table));

      for (int i = 0; i < feature_table.LookupCount; i++)
      {
        ushort lookuip_index = SwapUInt16((ushort)Marshal.PtrToStructure(lookup_list_ptr, typeof(ushort)));
        OffsetLookupList[i] = lookuip_index;
        if (lookuip_index == 30)
        {
          //to do remove
        }
        lookup_list_ptr = Increment(lookup_list_ptr, sizeof(ushort));
      }
      return OffsetLookupList;
    }

    internal IEnumerable<string> GetFeatures(string script, string language)
    {
      if (script_list.ContainsKey(script))
        if ((script_list[script] as Hashtable).ContainsKey(language))
        {
          foreach (string feature_str in ((script_list[script] as Hashtable)[language] as Hashtable).Keys)
            yield return feature_str;
        }
    }



    internal IEnumerable<string> Languages(string script)
    {
      if (script_list.ContainsKey(script))
      {
        foreach (string lang_str in (script_list[script] as Hashtable).Keys)
          yield return lang_str;
      }
    }

    private Hashtable LoadLanguageSystemTable(IntPtr lang_sys_rec_ptr)
    {
      Hashtable Features = new Hashtable();

      LangSysTable lang_sys_table = (LangSysTable)Marshal.PtrToStructure(lang_sys_rec_ptr, typeof(LangSysTable));
      lang_sys_table.LookupOrder = SwapUInt16(lang_sys_table.LookupOrder);
      lang_sys_table.ReqFeatureIndex = SwapUInt16(lang_sys_table.ReqFeatureIndex);
      lang_sys_table.FeatureCount = SwapUInt16(lang_sys_table.FeatureCount);

      IntPtr feature_index_ptr = Increment(lang_sys_rec_ptr, Marshal.SizeOf(lang_sys_table));
      ushort[] feature_indexes = new ushort[lang_sys_table.FeatureCount];
      for (int k = 0; k < lang_sys_table.FeatureCount; k++)
      {
        feature_indexes[k] = SwapUInt16((ushort)Marshal.PtrToStructure(feature_index_ptr, typeof(ushort)));

        string FeatureTag;
        ushort[] LookupOffsets = LoadFeature(feature_indexes[k], out FeatureTag);

#if DEBUG_TTF
        Console.WriteLine("\t\t[" + k + "]: " + FeatureTag + " of " + LookupOffsets.Length);
#endif
        if (!Features.ContainsKey(FeatureTag))
          Features.Add(FeatureTag, LookupOffsets);
#if DEBUG_TTF
        else
          Console.WriteLine("Duplicated record " + FeatureTag);
#endif
        feature_index_ptr = Increment(feature_index_ptr, sizeof(ushort));
      }
      return Features;
    }

    private void LoadScriptList()
    {
      IntPtr script_list_table_ptr = Increment(gsub_ptr, (int)header.ScriptList);
      ScriptListTable script_list_table = (ScriptListTable)Marshal.PtrToStructure(script_list_table_ptr, typeof(ScriptListTable));

      script_list_table.CountScripts = SwapUInt16(script_list_table.CountScripts);

      IntPtr script_record_ptr = Increment(script_list_table_ptr, Marshal.SizeOf(script_list_table));

      for (int i = 0; i < script_list_table.CountScripts; i++)
      {
        ScriptListRecord script_record = (ScriptListRecord)Marshal.PtrToStructure(script_record_ptr, typeof(ScriptListRecord));
        script_record.ScriptOffset = SwapUInt16(script_record.ScriptOffset);

        string ScriptTag = "" +
            (char)(0xff & script_record.ScriptTag) +
            (char)(0xff & (script_record.ScriptTag >> 8)) +
            (char)(0xff & (script_record.ScriptTag >> 16)) +
            (char)(0xff & (script_record.ScriptTag >> 24));

#if DEBUG_TTF
        Console.WriteLine("[" + ScriptTag + "]");
#endif
        Hashtable lang_sys_hash = new Hashtable();
        script_list.Add(ScriptTag, lang_sys_hash);

        IntPtr script_table_ptr = Increment(script_list_table_ptr, script_record.ScriptOffset);
        ScriptTable script_table = (ScriptTable)Marshal.PtrToStructure(script_table_ptr, typeof(ScriptTable));

        script_table.DefaultLangSys = SwapUInt16(script_table.DefaultLangSys);
        script_table.LangSysCount = SwapUInt16(script_table.LangSysCount);

        IntPtr lang_sys_rec_ptr;

        if (script_table.DefaultLangSys != 0)
        {
          lang_sys_rec_ptr = Increment(script_table_ptr, script_table.DefaultLangSys);
#if DEBUG_TTF
          Console.WriteLine("\t\"!DEF\"");
#endif
          lang_sys_hash.Add("", LoadLanguageSystemTable(lang_sys_rec_ptr));
        }

        lang_sys_rec_ptr = Increment(script_table_ptr, Marshal.SizeOf(script_table));
        for (int j = 0; j < script_table.LangSysCount; j++)
        {
          LangSysRecord lang_sys_rec = (LangSysRecord)Marshal.PtrToStructure(lang_sys_rec_ptr, typeof(LangSysRecord));
          lang_sys_rec.LangSys = SwapUInt16(lang_sys_rec.LangSys);

          string LangSysTag = "" +
              (char)(0xff & lang_sys_rec.LangSysTag) +
              (char)(0xff & (lang_sys_rec.LangSysTag >> 8)) +
              (char)(0xff & (lang_sys_rec.LangSysTag >> 16)) +
              (char)(0xff & (lang_sys_rec.LangSysTag >> 24));

#if DEBUG_TTF
          Console.WriteLine("\t\"" + LangSysTag + "\"");
#endif
          IntPtr lang_sys_rec_ptr_offset = Increment(script_table_ptr, lang_sys_rec.LangSys);

          lang_sys_hash.Add(LangSysTag, LoadLanguageSystemTable(lang_sys_rec_ptr_offset));

          lang_sys_rec_ptr = Increment(lang_sys_rec_ptr, Marshal.SizeOf(lang_sys_rec));
        }

        script_record_ptr = Increment(script_record_ptr, Marshal.SizeOf(script_record));
      }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct LookupTableRecordHeader
    {
      [FieldOffset(0)]
      public ushort LookupType;       // 	Different enumerations for GSUB and GPOS
      [FieldOffset(2)]
      public ushort LookupFlag;       // 	Lookup qualifiers
      [FieldOffset(4)]
      public ushort SubTableCount;    // 	Number of SubTables for this lookup            [FieldOffset(2)]
    }

    internal struct LookupEntry
    {
      public LookupTableRecordHeader record_header;
      public ushort[] subtable_offsets;
      public IntPtr[] subtable_ptrs;
      public Substitution[] subs;
      public override string ToString()
      {
        return "" + ((LookupTypes)record_header.LookupType).ToString() + " : " + record_header.LookupFlag + " [" + record_header.SubTableCount.ToString() + "]";
      }
    }



    public enum LookupTypes
    {
      Single = 1,     // Replace one glyph with one glyph
      Multiple = 2,   // Replace one glyph with more than one glyph
      Alternate = 3,  // Replace one glyph with one of many glyphs
      Ligature = 4,   // Replace multiple glyphs with one glyph
      Context = 5,    // Replace one or more glyphs in context
      ChainingContext = 6,                //	Replace one or more glyphs in chained context
      ExtensionSubstitution = 7,           //	Extension mechanism for other substitutions (i.e. this excludes the Extension type substitution itself)
      ReverseChainingContextSingle = 8    //	Applied in reverse order, replace single glyph in chaining context
                                          //9+ 	Reserved 	For future use (set to zero) 
    }

    private void LoadLookupList()
    {
      IntPtr lookup_list_table_ptr = Increment(gsub_ptr, (int)header.LookupList);
      ushort LookupListCount = SwapUInt16((ushort)Marshal.PtrToStructure(lookup_list_table_ptr, typeof(ushort)));

      lookup_list = new LookupEntry[LookupListCount];

      IntPtr lookup_list_ptr = Increment(lookup_list_table_ptr, sizeof(ushort));

      for (int i = 0; i < LookupListCount; i++)
      {
        ushort lookup_index = SwapUInt16((ushort)Marshal.PtrToStructure(lookup_list_ptr, typeof(ushort)));

        lookup_list_ptr = Increment(lookup_list_ptr, sizeof(ushort));

        IntPtr lookup_list_entry_ptr = Increment(lookup_list_table_ptr, lookup_index);
        IntPtr lookup_table_ptr = lookup_list_entry_ptr;
        lookup_list[i].record_header = (LookupTableRecordHeader)Marshal.PtrToStructure(lookup_list_entry_ptr, typeof(LookupTableRecordHeader));
        lookup_list_entry_ptr = Increment(lookup_list_entry_ptr, Marshal.SizeOf(lookup_list[i].record_header));

        lookup_list[i].record_header.LookupType = SwapUInt16(lookup_list[i].record_header.LookupType);
        lookup_list[i].record_header.LookupFlag = SwapUInt16(lookup_list[i].record_header.LookupFlag);
        lookup_list[i].record_header.SubTableCount = SwapUInt16(lookup_list[i].record_header.SubTableCount);

        lookup_list[i].subtable_offsets = new ushort[lookup_list[i].record_header.SubTableCount];
        lookup_list[i].subtable_ptrs = new IntPtr[lookup_list[i].record_header.SubTableCount];

        lookup_list[i].subs = new Substitution[lookup_list[i].record_header.SubTableCount];

        for (int j = 0; j < lookup_list[i].record_header.SubTableCount; j++)
        {
          lookup_list[i].subtable_offsets[j] = SwapUInt16((ushort)Marshal.PtrToStructure(lookup_list_entry_ptr, typeof(ushort)));
          lookup_list[i].subtable_ptrs[j] = Increment(lookup_table_ptr, lookup_list[i].subtable_offsets[j]);
          lookup_list_entry_ptr = Increment(lookup_list_entry_ptr, sizeof(ushort));

          lookup_list[i].subs[j] = LoadSubstitution(lookup_list[i].record_header.LookupType, lookup_list[i].subtable_ptrs[j]);
        }

        //to do: lookup list


      }
    }

    private Substitution LoadSubstitution(ushort lookupType, IntPtr intPtr)
    {
#if DEBUG_TTF
      Console.WriteLine("\tLookupType[" + ((LookupTypes)lookupType).ToString() + "]");
#endif
      switch ((LookupTypes)lookupType)
      {
        case LookupTypes.Single:
          return LoadSingleSubstitution(intPtr);
        case LookupTypes.Multiple:
          return LoadMultipleSubstitution(intPtr);
        case LookupTypes.Alternate:

          break;
        case LookupTypes.Ligature:
          return LoadLigaturesSubtable(intPtr);
          //break;
        case LookupTypes.Context:
          return LoadContextSubstitution(intPtr);
          //break;
        case LookupTypes.ChainingContext:
          return LoadChainingContext(intPtr);
          //break;
        case LookupTypes.ExtensionSubstitution:
          ExtensionSubstFormat extensionSubstFormat = (ExtensionSubstFormat)
              Marshal.PtrToStructure(intPtr,
              typeof(ExtensionSubstFormat));
          extensionSubstFormat.ExtensionLookupType = SwapUInt16(extensionSubstFormat.ExtensionLookupType);
          extensionSubstFormat.SubstFormat = SwapUInt16(extensionSubstFormat.SubstFormat);
          extensionSubstFormat.ExtensionOffset = SwapUInt32(extensionSubstFormat.ExtensionOffset);
          Extension extension = new Extension();
          extension.Format = extensionSubstFormat.SubstFormat;
          extension.LookupType = extensionSubstFormat.ExtensionLookupType;
          extension.Substitution = LoadSubstitution(
              extension.LookupType,
              Increment(intPtr, extensionSubstFormat.ExtensionOffset));
          return extension;
        case LookupTypes.ReverseChainingContextSingle:
          break;
      }
      return new VoidSubstitution();
    }

    private Substitution LoadContextSubstitution(IntPtr lookup_entry)
    {

      ushort format = (ushort)Marshal.PtrToStructure(lookup_entry, typeof(ushort));

      format = SwapUInt16(format);

      switch (format)
      {
        case 1:
          ContextSubstFormat1 type1 = (ContextSubstFormat1)Marshal.PtrToStructure(lookup_entry, typeof(ContextSubstFormat1));
          type1.SubstFormat = SwapUInt16(type1.SubstFormat);
          type1.Coverage = SwapUInt16(type1.Coverage);
          type1.SubRuleSetCount = SwapUInt16(type1.SubRuleSetCount);

          IntPtr sets_offset = Increment(lookup_entry, Marshal.SizeOf(type1));
          Contextual1.SubRule[][] subRuleSets = new Contextual1.SubRule[type1.SubRuleSetCount][];
          for (int i = 0; i < type1.SubRuleSetCount; i++)
          {
            ushort offset = (ushort)Marshal.PtrToStructure(sets_offset, typeof(ushort));
            offset = SwapUInt16(offset);
            IntPtr subruleset_offset_table = Increment(lookup_entry, offset);
            ushort subRuleCount = (ushort)Marshal.PtrToStructure(subruleset_offset_table, typeof(ushort));
            subRuleCount = SwapUInt16(subRuleCount);

            IntPtr subrule_offsets = Increment(subruleset_offset_table, Marshal.SizeOf(subRuleCount));
            Contextual1.SubRule[] subRuleSet = new Contextual1.SubRule[subRuleCount];
            for (int j = 0; j < subRuleCount; j++)
            {
              ushort subRuleOffset = (ushort)Marshal.PtrToStructure(subrule_offsets, typeof(ushort));
              IntPtr subrule_table = Increment(subruleset_offset_table, subRuleOffset);
              SubRuleTable subRule = (SubRuleTable)Marshal.PtrToStructure(subrule_table, typeof(SubRuleTable));

              IntPtr subrule_table_arrays = Increment(subrule_table, Marshal.SizeOf(subRule));

              subRule.GlyphCount = SwapUInt16(subRule.GlyphCount);
              subRule.SubstitutionCount = SwapUInt16(subRule.SubstitutionCount);

              ushort[] glyphs = new ushort[subRule.GlyphCount - 1];

              for (int k = 0; k < subRule.GlyphCount - 1; k++)
              {
                ushort glyph = (ushort)Marshal.PtrToStructure(subrule_table_arrays, typeof(ushort));
                glyph = SwapUInt16(glyph);
                subrule_table_arrays = Increment(subrule_table_arrays, Marshal.SizeOf(glyph));
                glyphs[k] = glyph;
              }

              SubstLookupRecord[] records = new SubstLookupRecord[subRule.SubstitutionCount];
              for (int k = 0; k < subRule.SubstitutionCount; k++)
              {
                SubstLookupRecord record = (SubstLookupRecord)Marshal.PtrToStructure(subrule_table_arrays, typeof(SubstLookupRecord));
                record.GlyphSequenceIndex = SwapUInt16(record.GlyphSequenceIndex);
                record.LookupListIndex = SwapUInt16(record.LookupListIndex);
                subrule_table_arrays = Increment(subrule_table_arrays, Marshal.SizeOf(record));
                records[k] = record;
              }
              subRuleSet[j] = new Contextual1.SubRule(glyphs, records);
              subrule_offsets = Increment(subrule_offsets, Marshal.SizeOf(subRuleOffset));
            }
            subRuleSets[i] = subRuleSet;
            sets_offset = Increment(lookup_entry, Marshal.SizeOf(offset));
          }
          return new Contextual1(this, subRuleSets, LoadCoverage(Increment(lookup_entry, type1.Coverage)));
        case 2:
          ContextSubstFormat2 type2 = (ContextSubstFormat2)Marshal.PtrToStructure(lookup_entry, typeof(ContextSubstFormat2));
          type2.SubstFormat = SwapUInt16(type2.SubstFormat);
          type2.Coverage = SwapUInt16(type2.Coverage);
          type2.ClassDefOffset = SwapUInt16(type2.ClassDefOffset);
          type2.SubClassSetCount = SwapUInt16(type2.SubClassSetCount);




          Contextual2.SubClassRule[][] subClassRuleSets = new Contextual2.SubClassRule[type2.SubClassSetCount][];

          IntPtr sub_class_set_offset = Increment(lookup_entry, Marshal.SizeOf(type2));

          for (int i = 0; i < type2.SubClassSetCount; i++)
          {
            ushort offset = (ushort)Marshal.PtrToStructure(sub_class_set_offset, typeof(ushort));
            if (offset == 0)
              subClassRuleSets[i] = null;
            else
            {
              offset = SwapUInt16(offset);
              IntPtr sub_class_set_table = Increment(lookup_entry, offset);

              ushort count = (ushort)Marshal.PtrToStructure(sub_class_set_table, typeof(ushort));
              count = SwapUInt16(count);
              IntPtr sub_class_rule_offset = Increment(sub_class_set_table, Marshal.SizeOf(count));

              Contextual2.SubClassRule[] subClassSet = new Contextual2.SubClassRule[count];

              for (int j = 0; j < count; j++)
              {
                ushort offset2 = (ushort)Marshal.PtrToStructure(sub_class_rule_offset, typeof(ushort));
                offset2 = SwapUInt16(offset2);

                IntPtr sub_class_rule_table = Increment(sub_class_set_table, offset2);

                SubClassRule subClassRule = (SubClassRule)Marshal.PtrToStructure(sub_class_rule_table, typeof(SubClassRule));

                IntPtr subclassrule_table_arrays = Increment(sub_class_rule_table, Marshal.SizeOf(subClassRule));

                subClassRule.ClassCount = SwapUInt16(subClassRule.ClassCount);
                subClassRule.SubstitutionCount = SwapUInt16(subClassRule.SubstitutionCount);

                ushort[] glyphClassess = new ushort[subClassRule.ClassCount - 1];

                for (int k = 0; k < subClassRule.ClassCount - 1; k++)
                {
                  ushort glyphClass = (ushort)Marshal.PtrToStructure(subclassrule_table_arrays, typeof(ushort));
                  glyphClass = SwapUInt16(glyphClass);
                  subclassrule_table_arrays = Increment(subclassrule_table_arrays, Marshal.SizeOf(glyphClass));
                  glyphClassess[k] = glyphClass;
                }

                SubstLookupRecord[] records = new SubstLookupRecord[subClassRule.SubstitutionCount];
                for (int k = 0; k < subClassRule.SubstitutionCount; k++)
                {
                  SubstLookupRecord record = (SubstLookupRecord)Marshal.PtrToStructure(subclassrule_table_arrays, typeof(SubstLookupRecord));
                  record.GlyphSequenceIndex = SwapUInt16(record.GlyphSequenceIndex);
                  record.LookupListIndex = SwapUInt16(record.LookupListIndex);
                  subclassrule_table_arrays = Increment(subclassrule_table_arrays, Marshal.SizeOf(record));
                  records[k] = record;
                }
                subClassSet[j] = new Contextual2.SubClassRule(glyphClassess, records);

                sub_class_rule_offset = Increment(sub_class_rule_offset, Marshal.SizeOf(offset2));
              }
              subClassRuleSets[i] = subClassSet;
            }

            sub_class_set_offset = Increment(sub_class_set_offset, Marshal.SizeOf(offset));
          }

          return new Contextual2(this, subClassRuleSets, LoadCoverage(Increment(lookup_entry, type2.Coverage)), LoadClassDefinition(Increment(lookup_entry, type2.ClassDefOffset)));
        case 3:
          ContextSubstFormat3 type3 = (ContextSubstFormat3)Marshal.PtrToStructure(lookup_entry, typeof(ContextSubstFormat3));
          type3.SubstFormat = SwapUInt16(type3.SubstFormat);
          type3.GlyphCount = SwapUInt16(type3.GlyphCount);
          type3.SubstitutionCount = SwapUInt16(type3.SubstitutionCount);


          Coverage[] coverages = new Coverage[type3.GlyphCount];
          SubstLookupRecord[] subRecords = new SubstLookupRecord[type3.SubstitutionCount];


          IntPtr type3_arrays = Increment(lookup_entry, Marshal.SizeOf(type3));

          for (int i = 0; i < type3.GlyphCount; i++)
          {
            ushort offset = (ushort)Marshal.PtrToStructure(type3_arrays, typeof(ushort));
            offset = SwapUInt16(offset);

            coverages[i] = LoadCoverage(Increment(lookup_entry, offset));

            type3_arrays = Increment(type3_arrays, Marshal.SizeOf(offset));
          }

          for (int i = 0; i < type3.SubstitutionCount; i++)
          {
            SubstLookupRecord record = (SubstLookupRecord)Marshal.PtrToStructure(type3_arrays, typeof(SubstLookupRecord));
            record.GlyphSequenceIndex = SwapUInt16(record.GlyphSequenceIndex);
            record.LookupListIndex = SwapUInt16(record.LookupListIndex);
            subRecords[i] = record;

            type3_arrays = Increment(type3_arrays, Marshal.SizeOf(record));
          }

          return new Contextual3(this, subRecords, coverages);
      }

      return new VoidSubstitution();
    }

    private Substitution LoadMultipleSubstitution(IntPtr lookup_entry)
    {
      MultipleSubstFormat1 type1 = (MultipleSubstFormat1)Marshal.PtrToStructure(lookup_entry, typeof(MultipleSubstFormat1));
      type1.SubstFormat = SwapUInt16(type1.SubstFormat);
      type1.Coverage = SwapUInt16(type1.Coverage);
      type1.SequenceCount = SwapUInt16(type1.SequenceCount);
      //type1.SequenceOffset = SwapUInt16(type1.SequenceOffset);


      IntPtr sequence_offset = Increment(lookup_entry, Marshal.SizeOf(type1));
      ushort[][] sequences = new ushort[type1.SequenceCount][];
      for (int i = 0; i < type1.SequenceCount; i++)
      {
        ushort offset = (ushort)Marshal.PtrToStructure(sequence_offset, typeof(ushort));
        offset = SwapUInt16(offset);
        IntPtr sequence_offset_table = Increment(lookup_entry, offset);
        ushort glyph_count = (ushort)Marshal.PtrToStructure(sequence_offset_table, typeof(ushort));
        glyph_count = SwapUInt16(glyph_count);
        ushort[] glyphs = new ushort[glyph_count];
        for (int j = 0; j < glyph_count; j++)
        {
          glyphs[j] = (ushort)Marshal.PtrToStructure(sequence_offset_table, typeof(ushort));
          glyphs[j] = SwapUInt16(glyphs[j]);
          sequence_offset_table = Increment(lookup_entry, Marshal.SizeOf(glyphs[j]));
        }
        sequences[i] = glyphs;
        sequence_offset = Increment(lookup_entry, Marshal.SizeOf(offset));
      }

      return new Multiple(sequences, LoadCoverage(Increment(lookup_entry, type1.Coverage)));
      //return new VoidSubstitution();
    }

    private Substitution LoadSingleSubstitution(IntPtr lookup_entry)
    {
      SignleSubstitutionFormat1 type1 = (SignleSubstitutionFormat1)Marshal.PtrToStructure(lookup_entry, typeof(SignleSubstitutionFormat1));
      type1.SubstFormat = SwapUInt16(type1.SubstFormat);
      type1.Coverage = SwapUInt16(type1.Coverage);
      type1.DeltaGlyphID = SwapInt16(type1.DeltaGlyphID);

      switch (type1.SubstFormat)
      {
        case 1:
          return new Single1(type1.DeltaGlyphID, LoadCoverage(Increment(lookup_entry, type1.Coverage)));
        case 2:
          ushort[] substitute = new ushort[type1.DeltaGlyphID];
          IntPtr lookup_entry_substitute = Increment(lookup_entry, Marshal.SizeOf(typeof(SignleSubstitutionFormat1)));
          for (int i = 0; i < type1.DeltaGlyphID; i++)
          {
            substitute[i] = (ushort)Marshal.PtrToStructure(lookup_entry_substitute, typeof(ushort));
            substitute[i] = SwapUInt16(substitute[i]);
            lookup_entry_substitute = Increment(lookup_entry_substitute, Marshal.SizeOf(typeof(ushort)));
          }
          return new Single2(substitute, LoadCoverage(Increment(lookup_entry, type1.Coverage)));
      }
      return new VoidSubstitution();
    }

    private ClassDefinition LoadClassDefinition(IntPtr class_definition_ptr)
    {
      ushort class_format = (ushort)Marshal.PtrToStructure(class_definition_ptr, typeof(ushort));
      class_format = SwapUInt16(class_format);
      switch (class_format)
      {
        case 1:
          ClassDefFormat1 type1 = (ClassDefFormat1)Marshal.PtrToStructure(class_definition_ptr, typeof(ClassDefFormat1));
          type1.ClassFormat = SwapUInt16(type1.ClassFormat);
          type1.GlyphCount = SwapUInt16(type1.GlyphCount);
          type1.StartGlyphID = SwapUInt16(type1.StartGlyphID);
          IntPtr class_value_array = Increment(class_definition_ptr, Marshal.SizeOf(type1));
          ushort[] classValues = new ushort[type1.GlyphCount];
          for (int i = 0; i < type1.GlyphCount; i++)
          {
            ushort classValue = (ushort)Marshal.PtrToStructure(class_value_array, typeof(ushort));
            classValue = SwapUInt16(classValue);
            class_value_array = Increment(class_value_array, Marshal.SizeOf(classValue));
          }

          return new ClassDefinition1(type1.StartGlyphID, classValues);
        case 2:

          ClassDefFormat2 type2 = (ClassDefFormat2)Marshal.PtrToStructure(class_definition_ptr, typeof(ClassDefFormat2));
          type2.ClassFormat = SwapUInt16(type2.ClassFormat);
          type2.ClassRangeCount = SwapUInt16(type2.ClassRangeCount);
          IntPtr class_records_array = Increment(class_definition_ptr, Marshal.SizeOf(type2));

          ClassRangeRecord[] records = new ClassRangeRecord[type2.ClassRangeCount];

          for (int i = 0; i < type2.ClassRangeCount; i++)
          {
            ClassRangeRecord record = (ClassRangeRecord)Marshal.PtrToStructure(class_records_array, typeof(ClassRangeRecord));
            record.StartGlyphID = SwapUInt16(record.StartGlyphID);
            record.EndGlyphID = SwapUInt16(record.EndGlyphID);
            record.ClassValue = SwapUInt16(record.ClassValue);
            class_records_array = Increment(class_records_array, Marshal.SizeOf(record));
            records[i] = record;
          }

          return new ClassDefinition2(records);
      }

      return new VoidClassDefinition();
    }

    private Coverage LoadCoverage(IntPtr coverage_table_ptr)
    {
      CoverageHeader ch = (CoverageHeader)Marshal.PtrToStructure(coverage_table_ptr, typeof(CoverageHeader));
      ch.CoverageFormat = SwapUInt16(ch.CoverageFormat);
      ch.GlyphCount = SwapUInt16(ch.GlyphCount);
      switch (ch.CoverageFormat)
      {
        case 1:
          {

            ushort[] glyphs = new ushort[ch.GlyphCount];
            coverage_table_ptr = Increment(coverage_table_ptr, Marshal.SizeOf(ch));
            for (int i = 0; i < ch.GlyphCount; i++)
            {
              glyphs[i] = SwapUInt16((ushort)Marshal.PtrToStructure(coverage_table_ptr, typeof(ushort)));
              coverage_table_ptr = Increment(coverage_table_ptr, Marshal.SizeOf(typeof(ushort)));
            }
            /*if((new Coverage1(ch.GlyphCount, glyphs)).IsSubstituteGetIndex(296) >=0)
            {
                //to do remove
            }*/
            return new Coverage1(ch.GlyphCount, glyphs);
          }
          //break;
        case 2:
          {
            RangeRecord[] rrs = new RangeRecord[ch.GlyphCount];
            coverage_table_ptr = Increment(coverage_table_ptr, Marshal.SizeOf(ch));
            for (int i = 0; i < ch.GlyphCount; i++)
            {
              rrs[i] = (RangeRecord)Marshal.PtrToStructure(coverage_table_ptr, typeof(RangeRecord));
              rrs[i].Start = SwapUInt16(rrs[i].Start);
              rrs[i].End = SwapUInt16(rrs[i].End);
              rrs[i].StartCoverageIndex = SwapUInt16(rrs[i].StartCoverageIndex);

              coverage_table_ptr = Increment(coverage_table_ptr, Marshal.SizeOf(rrs[i]));
            }
            /*if ((new Coverage2(ch.GlyphCount, rrs)).IsSubstituteGetIndex(296) >= 0)
            {
                //to do remove
            }*/
            return new Coverage2(ch.GlyphCount, rrs);
          }
          //break;
      }
      return new VoidCoverage();
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct ExtensionSubstFormat
    {
      [FieldOffset(0)]
      public ushort SubstFormat;
      [FieldOffset(2)]
      public ushort ExtensionLookupType;
      [FieldOffset(4)]
      public uint ExtensionOffset;

      public override string ToString()
      {
        return ((LookupTypes)ExtensionLookupType).ToString();
      }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct LigatureSubst
    {
      [FieldOffset(0)]
      public ushort SubstFormat;  // 	Format identifier-format = 1
      [FieldOffset(2)]
      public ushort Coverage;     // 	Offset to Coverage table-from beginning of Substitution table
      [FieldOffset(4)]
      public ushort LigSetCount;  // 	Number of LigatureSet tables
                                  // public ushort [LigSetCount] 	Array of offsets to LigatureSet tables-from beginning of Substitution table-ordered by Coverage Index            
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct LigatureHeader
    {
      [FieldOffset(0)]
      public ushort LigGlyph;     //  GlyphID of ligature to substitute
      [FieldOffset(2)]
      public ushort CompCount;      // 	Number of components in the ligature
    }

    internal Substitution LoadLigaturesSubtable(IntPtr lookup_entry)
    {
      LigatureSubst type4 = (LigatureSubst)Marshal.PtrToStructure(lookup_entry, typeof(LigatureSubst));
      type4.SubstFormat = SwapUInt16(type4.SubstFormat);
      type4.Coverage = SwapUInt16(type4.Coverage);
      type4.LigSetCount = SwapUInt16(type4.LigSetCount);

      //To do not null
      //LoadCoverageTable(null, Increment(lookup_entry, type4.Coverage));
      Coverage coverage = LoadCoverage(Increment(lookup_entry, type4.Coverage));

      // Ligature Set
      IntPtr ligature_set_ptr = Increment(lookup_entry, Marshal.SizeOf(type4));
      IntPtr current_ptr = ligature_set_ptr;
      IntPtr[] LigatureSet = new IntPtr[type4.LigSetCount];

      LigatureSet[][] ligatureSets = new LigatureSet[type4.LigSetCount][];

      for (int i = 0; i < type4.LigSetCount; i++)
      {
        ushort offset = SwapUInt16((ushort)Marshal.PtrToStructure(current_ptr, typeof(ushort)));
        // Console.WriteLine("LigatresSet[" + i + "] at" + offset);
        LigatureSet[i] = Increment(lookup_entry, offset);

        // Ligature Table
        ushort LigatureCount = SwapUInt16((ushort)Marshal.PtrToStructure(LigatureSet[i], typeof(ushort)));
        IntPtr ligature_ptr = Increment(LigatureSet[i], sizeof(ushort));
        IntPtr[] LigaturePtrs = new IntPtr[LigatureCount];

        LigatureHeader[] lh = new LigatureHeader[LigatureCount];

        ligatureSets[i] = new LigatureSet[LigatureCount];

        for (int j = 0; j < LigatureCount; j++)
        {
          ushort lig_offset = SwapUInt16((ushort)Marshal.PtrToStructure(ligature_ptr, typeof(ushort)));

          LigaturePtrs[j] = Increment(LigatureSet[i], lig_offset);
          // Console.WriteLine("\tLigature[" + j + "] at " + lig_offset);

          //// Ligature
          lh[j] = (LigatureHeader)Marshal.PtrToStructure(LigaturePtrs[j], typeof(LigatureHeader));
          lh[j].LigGlyph = SwapUInt16(lh[j].LigGlyph);
          lh[j].CompCount = SwapUInt16(lh[j].CompCount);
          ushort[] glyphs = new ushort[lh[j].CompCount - 1];

          IntPtr ligature_array = Increment(LigaturePtrs[j], Marshal.SizeOf(lh[j]));
          for (int k = 0; k < lh[j].CompCount - 1; k++)
          {
            glyphs[k] = SwapUInt16((ushort)Marshal.PtrToStructure(ligature_array, typeof(ushort)));
            ligature_array = Increment(ligature_array, Marshal.SizeOf(glyphs[k]));
          }
          //// ---------
          ligatureSets[i][j] = new LigatureSet(lh[j].LigGlyph, glyphs);
          ligature_ptr = Increment(ligature_ptr, sizeof(ushort));
        }
        // -----------------

        current_ptr = Increment(current_ptr, sizeof(ushort));

      }

      return new Ligature(coverage, ligatureSets);

    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct CoverageHeader
    {
      [FieldOffset(0)]
      public ushort CoverageFormat;     // 	Format identifier-format = 1
      [FieldOffset(2)]
      public ushort GlyphCount;         // 	Number of glyphs in the GlyphArray        
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct RangeRecord
    {
      [FieldOffset(0)]
      public ushort Start;                // 	First GlyphID in the range
      [FieldOffset(2)]
      public ushort End;                  // 	Last GlyphID in the range
      [FieldOffset(4)]
      public ushort StartCoverageIndex;   // 	Coverage Index of first GlyphID in range
    }


    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct SignleSubstitutionFormat1
    {
      [FieldOffset(0)]
      public ushort SubstFormat;    // 	Format identifier-format = 1
      [FieldOffset(2)]
      public ushort Coverage;   // 	Offset to Coverage table-from beginning of Substitution table
      [FieldOffset(4)]
      public short DeltaGlyphID;   // 	Add to original GlyphID to get substitute GlyphID        

    }


    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct MultipleSubstFormat1
    {
      [FieldOffset(0)]
      public ushort SubstFormat;    // 	Format identifier-format = 1
      [FieldOffset(2)]
      public ushort Coverage;   // 	Offset to Coverage table-from beginning of Substitution table
      [FieldOffset(4)]
      public ushort SequenceCount; // Number of Sequence table offsets in the Sequence array
                                   //[FieldOffset(6)]
                                   //public ushort SequenceOffset;   // 	Array of offsets to Sequence tables-from beginning of Substitution table-ordered by Coverage Index        

    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct ContextSubstFormat1
    {
      [FieldOffset(0)]
      public ushort SubstFormat;    // 	Format identifier-format = 1
      [FieldOffset(2)]
      public ushort Coverage;   // 	Offset to Coverage table-from beginning of Substitution table
      [FieldOffset(4)]
      public ushort SubRuleSetCount; // Number of SubRuleSet tables Ч must equal glyphCount in Coverage table
                                     //[FieldOffset(6)]
                                     //public ushort[] subRuleSetOffsets;   // Array of offsets to SubRuleSet tables. Offsets are from beginning of substitution subtable, ordered by Coverage index
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct ContextSubstFormat2
    {
      [FieldOffset(0)]
      public ushort SubstFormat;    // 	Format identifier-format = 1
      [FieldOffset(2)]
      public ushort Coverage;   // 	Offset to Coverage table-from beginning of Substitution table
      [FieldOffset(4)]
      public ushort ClassDefOffset; // 	Offset to glyph ClassDef table, from beginning of substitution subtable
      [FieldOffset(6)]
      public ushort SubClassSetCount;   // Number of SubClassSet tables
                                        //[FieldOffset(8)]
                                        //public ushort[] subClassSetOffsets;   // Array of offsets to SubClassSet tables. Offsets are from beginning of substitution subtable, ordered by class (may be NULL).
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct ContextSubstFormat3
    {
      [FieldOffset(0)]
      public ushort SubstFormat;    // 	Format identifier-format = 1
      [FieldOffset(2)]
      public ushort GlyphCount;   // 	Number of glyphs in the input glyph sequence
      [FieldOffset(4)]
      public ushort SubstitutionCount; // Number of SubstLookupRecords
                                       /*
                                       Offset16	coverageOffsets[glyphCount]	            Array of offsets to Coverage tables. Offsets are from beginning of substitution subtable, in glyph sequence order.
                              SubstLookupRecord	substLookupRecords[substitutionCount]	Array of SubstLookupRecords, in design order.
                                        */
    }


    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct SubClassRule
    {
      [FieldOffset(0)]
      public ushort ClassCount;//GlyphCount;    // 	Total number of classes specified for the context in the rule Ч includes the first class
      [FieldOffset(2)]
      public ushort SubstitutionCount;   // 	Number of SubstLookupRecords
                                         /*
                                         uint16	inputSequence[glyphCount - 1]	Array of classes to be matched to the input glyph sequence, beginning with the second glyph position.
                              SubstLookupRecord	substLookupRecords[substitutionCount]	Array of Substitution lookups, in design order.
                                          */
    }




    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct SubstLookupRecord
    {
      [FieldOffset(0)]
      public ushort GlyphSequenceIndex;    // Index into current glyph sequence Ч first glyph = 0.
      [FieldOffset(2)]
      public ushort LookupListIndex;   // Lookup to apply to that position Ч zero-based index.
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct SubRuleTable
    {
      [FieldOffset(0)]
      public ushort GlyphCount;    // 	Total number of glyphs in input glyph sequence Ч includes the first glyph.
      [FieldOffset(2)]
      public ushort SubstitutionCount;   // 	Number of SubstLookupRecords
                                         //[FieldOffset(4)]
                                         //public ushort inputSequence; // [glyphCount - 1]	Array of input glyph IDs Ч start with second glyph
                                         //[FieldOffset(6)]
                                         //public SubstLookupRecord[] substLookupRecords;   // [substitutionCount]	Array of SubstLookupRecords, in design order

    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct SequenceFormat
    {
      [FieldOffset(0)]
      public ushort GlyphCount;    // 	Number of glyph IDs in the Substitute array. This should always be greater than 0.
                                   //      Substitute [GlyphCount] String of glyph IDs to substitute
    }


    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct ClassDefFormat1
    {
      [FieldOffset(0)]
      public ushort ClassFormat;    // 		Format identifier Ч format = 1
      [FieldOffset(2)]
      public ushort StartGlyphID;   // 	First glyph ID of the classValueArray
      [FieldOffset(4)]
      public ushort GlyphCount; // Size of the classValueArray
                                //[FieldOffset(6)]
                                //public ushort[] classValueArray;   // [glyphCount]	Array of Class Values Ч one per glyph ID

    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct ClassDefFormat2
    {
      [FieldOffset(0)]
      public ushort ClassFormat;    // 		Format identifier Ч format = 1
      [FieldOffset(2)]
      public ushort ClassRangeCount;   // 	First glyph ID of the classValueArray
                                       //[FieldOffset(4)]
                                       //public ClassRangeRecord[] classRangeRecords; // Array of ClassRangeRecords Ч ordered by startGlyphID
    }


    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ClassRangeRecord
    {
      [FieldOffset(0)]
      public ushort StartGlyphID;    // 		Format identifier Ч format = 1
      [FieldOffset(2)]
      public ushort EndGlyphID;   // 	First glyph ID of the classValueArray
      [FieldOffset(4)]
      public ushort ClassValue; // Array of ClassRangeRecords Ч ordered by startGlyphID
    }


    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct ChainContextSubstFormat1
    {
      [FieldOffset(0)]
      public ushort SubstFormat;    // 		Format identifier: format = 1
      [FieldOffset(2)]
      public ushort Coverage;   // 	Offset to Coverage table, from beginning of substitution subtable.
      [FieldOffset(4)]
      public ushort ChainSubRuleSetCount; // Number of ChainSubRuleSet tables Ч must equal GlyphCount in Coverage table.
                                          /*
                                          Offset16	chainSubRuleSetOffsets[chainSubRuleSetCount]	Array of offsets to ChainSubRuleSet tables. Offsets are from beginning of substitution subtable, ordered by Coverage index.
                                           */
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct ChainContextSubstFormat2
    {
      [FieldOffset(0)]
      public ushort SubstFormat;    // 		Format identifier: format = 2
      [FieldOffset(2)]
      public ushort Coverage;   // 	Offset to Coverage table, from beginning of substitution subtable.
      [FieldOffset(4)]
      public ushort BacktrackClassDefOffset; // Offset to glyph ClassDef table containing backtrack sequence data, from beginning of substitution subtable.
      [FieldOffset(6)]
      public ushort InputClassDefOffset; // Offset to glyph ClassDef table containing input sequence data, from beginning of substitution subtable.
      [FieldOffset(8)]
      public ushort LookaheadClassDefOffset; // Offset to glyph ClassDef table containing lookahead sequence data, from beginning of substitution subtable.
      [FieldOffset(10)]
      public ushort ChainSubClassSetCount; // Number of ChainSubClassSet tables
                                           /*
                                           Offset16	chainSubClassSetOffsets[chainSubClassSetCount]	Array of offsets to ChainSubClassSet tables. Offsets are from beginning of substitution subtable, ordered by input class (may be NULL)
                                            */
    }


    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct ChainContextSubstFormat3
    {
      [FieldOffset(0)]
      public ushort SubstFormat;    // 		Format identifier: format = 3
                                    /*
                                    uint16	backtrackGlyphCount	Number of glyphs in the backtracking sequence.
                                    Offset16	backtrackCoverageOffsets[backtrackGlyphCount]	Array of offsets to coverage tables in backtracking sequence. Offsets are from beginning of substition subtable, in glyph sequence order.
                                    uint16	inputGlyphCount	Number of glyphs in input sequence
                                    Offset16	inputCoverageOffsets[inputGlyphCount]	Array of offsets to coverage tables in input sequence. Offsets are from beginning of substition subtable, in glyph sequence order.
                                    uint16	lookaheadGlyphCount	Number of glyphs in lookahead sequence
                                    Offset16	lookaheadCoverageOffsets[lookaheadGlyphCount]	Array of offsets to coverage tables in lookahead sequence. Offsets are from beginning of substitution subtable, in glyph sequence order.
                                    uint16	substitutionCount	Number of SubstLookupRecords
                                    SubstLookupRecord	substLookupRecords[substitutionCount]	Array of SubstLookupRecords, in design order
                                     */
    }


    private Substitution LoadChainingContextSubstFormat1(IntPtr lookup_entry)
    {
      ChainContextSubstFormat1 type1 = (ChainContextSubstFormat1)Marshal.PtrToStructure(lookup_entry, typeof(ChainContextSubstFormat1));
      type1.SubstFormat = SwapUInt16(type1.SubstFormat);
      type1.ChainSubRuleSetCount = SwapUInt16(type1.ChainSubRuleSetCount);
      type1.Coverage = SwapUInt16(type1.Coverage);

      ChainingContextual1.ChainSubRule[][] chainSubRuleSets
          = new ChainingContextual1.ChainSubRule[type1.ChainSubRuleSetCount][];

      IntPtr chain_sub_rule_set_offsets = Increment(lookup_entry, Marshal.SizeOf(type1));



      for (int i = 0; i < type1.ChainSubRuleSetCount; i++)
      {
        ushort offset = (ushort)Marshal.PtrToStructure(chain_sub_rule_set_offsets, typeof(ushort));
        offset = SwapUInt16(offset);
        IntPtr chain_sub_rule_set_table = Increment(lookup_entry, offset);

        ushort count = (ushort)Marshal.PtrToStructure(chain_sub_rule_set_table, typeof(ushort));
        count = SwapUInt16(count);
        IntPtr chain_sub_rule_offset = Increment(chain_sub_rule_set_table, Marshal.SizeOf(count));

        ChainingContextual1.ChainSubRule[] chainSubRuleSet = new ChainingContextual1.ChainSubRule[count];

        for (int j = 0; j < count; j++)
        {
          ushort offset2 = (ushort)Marshal.PtrToStructure(chain_sub_rule_offset, typeof(ushort));
          offset2 = SwapUInt16(offset2);

          IntPtr chain_sub_rule_table = Increment(chain_sub_rule_set_table, offset2);

          IntPtr chain_sub_rule_arrays = chain_sub_rule_table;
          //start array
          ushort backtrackGlyphCount = (ushort)Marshal.PtrToStructure(chain_sub_rule_arrays, typeof(ushort));
          backtrackGlyphCount = SwapUInt16(backtrackGlyphCount);
          ushort[] backtrackSequence = new ushort[backtrackGlyphCount];
          chain_sub_rule_arrays = Increment(chain_sub_rule_arrays, Marshal.SizeOf(backtrackGlyphCount));
          for (int k = 0; k < backtrackGlyphCount; k++)
          {
            ushort glyph = (ushort)Marshal.PtrToStructure(chain_sub_rule_arrays, typeof(ushort));
            glyph = SwapUInt16(glyph);
            backtrackSequence[k] = glyph;
            chain_sub_rule_arrays = Increment(chain_sub_rule_arrays, Marshal.SizeOf(glyph));
          }
          //end array



          //start array
          ushort inputGlyphCount = (ushort)Marshal.PtrToStructure(chain_sub_rule_arrays, typeof(ushort));
          inputGlyphCount = SwapUInt16(inputGlyphCount);
          ushort[] inputSequence = new ushort[inputGlyphCount - 1];
          chain_sub_rule_arrays = Increment(chain_sub_rule_arrays, Marshal.SizeOf(inputGlyphCount));
          for (int k = 0; k < inputGlyphCount - 1; k++)
          {
            ushort glyph = (ushort)Marshal.PtrToStructure(chain_sub_rule_arrays, typeof(ushort));
            glyph = SwapUInt16(glyph);
            inputSequence[k] = glyph;
            chain_sub_rule_arrays = Increment(chain_sub_rule_arrays, Marshal.SizeOf(glyph));
          }
          //end array


          //start array
          ushort lookaheadGlyphCount = (ushort)Marshal.PtrToStructure(chain_sub_rule_arrays, typeof(ushort));
          lookaheadGlyphCount = SwapUInt16(lookaheadGlyphCount);
          ushort[] lookAheadSequence = new ushort[lookaheadGlyphCount];
          chain_sub_rule_arrays = Increment(chain_sub_rule_arrays, Marshal.SizeOf(lookaheadGlyphCount));
          for (int k = 0; k < lookaheadGlyphCount; k++)
          {
            ushort glyph = (ushort)Marshal.PtrToStructure(chain_sub_rule_arrays, typeof(ushort));
            glyph = SwapUInt16(glyph);
            lookAheadSequence[k] = glyph;
            chain_sub_rule_arrays = Increment(chain_sub_rule_arrays, Marshal.SizeOf(glyph));
          }
          //end array


          //start array
          ushort substitutionCount = (ushort)Marshal.PtrToStructure(chain_sub_rule_arrays, typeof(ushort));
          substitutionCount = SwapUInt16(substitutionCount);
          SubstLookupRecord[] records = new SubstLookupRecord[substitutionCount];
          chain_sub_rule_arrays = Increment(chain_sub_rule_arrays, Marshal.SizeOf(substitutionCount));
          for (int k = 0; k < substitutionCount; k++)
          {
            SubstLookupRecord record = (SubstLookupRecord)Marshal.PtrToStructure(chain_sub_rule_arrays, typeof(SubstLookupRecord));
            record.GlyphSequenceIndex = SwapUInt16(record.GlyphSequenceIndex);
            record.LookupListIndex = SwapUInt16(record.LookupListIndex);
            records[k] = record;
            chain_sub_rule_arrays = Increment(chain_sub_rule_arrays, Marshal.SizeOf(record));
          }
          //end array
          chainSubRuleSet[j] = new ChainingContextual1.ChainSubRule(backtrackSequence, inputSequence, lookAheadSequence, records);

          chain_sub_rule_offset = Increment(chain_sub_rule_offset, Marshal.SizeOf(offset2));
        }
        chainSubRuleSets[i] = chainSubRuleSet;

        chain_sub_rule_set_offsets = Increment(chain_sub_rule_set_offsets, Marshal.SizeOf(offset));
      }

      return new ChainingContextual1(this, LoadCoverage(Increment(lookup_entry, type1.Coverage)), chainSubRuleSets);
    }

    private Substitution LoadChainingContextSubstFormat2(IntPtr lookup_entry)
    {
      ChainContextSubstFormat2 type2 = (ChainContextSubstFormat2)Marshal.PtrToStructure(lookup_entry, typeof(ChainContextSubstFormat2));
      type2.SubstFormat = SwapUInt16(type2.SubstFormat);
      type2.Coverage = SwapUInt16(type2.Coverage);
      type2.BacktrackClassDefOffset = SwapUInt16(type2.BacktrackClassDefOffset);
      type2.InputClassDefOffset = SwapUInt16(type2.InputClassDefOffset);
      type2.LookaheadClassDefOffset = SwapUInt16(type2.LookaheadClassDefOffset);
      type2.ChainSubClassSetCount = SwapUInt16(type2.ChainSubClassSetCount);


      IntPtr chain_sub_class_set_offsets = Increment(lookup_entry, Marshal.SizeOf(type2));

      ChainingContextual2.ChainSubClassRule[][] chainSubClassRuleSets
          = new ChainingContextual2.ChainSubClassRule[type2.ChainSubClassSetCount][];

      for (int i = 0; i < type2.ChainSubClassSetCount; i++)
      {
        ushort offset = (ushort)Marshal.PtrToStructure(chain_sub_class_set_offsets, typeof(ushort));
        offset = SwapUInt16(offset);
                if(offset == 0)
                {
                    chain_sub_class_set_offsets = Increment(chain_sub_class_set_offsets, Marshal.SizeOf(offset));
                    continue;
                }
        IntPtr chain_sub_class_set_table = Increment(lookup_entry, offset);

        ushort count = (ushort)Marshal.PtrToStructure(chain_sub_class_set_table, typeof(ushort));
        count = SwapUInt16(count);
        IntPtr chain_sub_class_offset = Increment(chain_sub_class_set_table, Marshal.SizeOf(count));

        ChainingContextual2.ChainSubClassRule[] chainSubClassSet = new ChainingContextual2.ChainSubClassRule[count];

        for (int j = 0; j < count; j++)
        {
          ushort offset2 = (ushort)Marshal.PtrToStructure(chain_sub_class_offset, typeof(ushort));
          offset2 = SwapUInt16(offset2);

          IntPtr chain_sub_rule_table = Increment(chain_sub_class_set_table, offset2);

          IntPtr chain_sub_rule_arrays = chain_sub_rule_table;
          //start array
          ushort backtrackGlyphCount = (ushort)Marshal.PtrToStructure(chain_sub_rule_arrays, typeof(ushort));
          backtrackGlyphCount = SwapUInt16(backtrackGlyphCount);
          ushort[] backtrackSequence = new ushort[backtrackGlyphCount];
          chain_sub_rule_arrays = Increment(chain_sub_rule_arrays, Marshal.SizeOf(backtrackGlyphCount));
          for (int k = 0; k < backtrackGlyphCount; k++)
          {
            ushort glyph = (ushort)Marshal.PtrToStructure(chain_sub_rule_arrays, typeof(ushort));
            glyph = SwapUInt16(glyph);
            backtrackSequence[k] = glyph;
            chain_sub_rule_arrays = Increment(chain_sub_rule_arrays, Marshal.SizeOf(glyph));
          }
          //end array



          //start array
          ushort inputGlyphCount = (ushort)Marshal.PtrToStructure(chain_sub_rule_arrays, typeof(ushort));
          inputGlyphCount = SwapUInt16(inputGlyphCount);
          ushort[] inputSequence = new ushort[inputGlyphCount - 1];
          chain_sub_rule_arrays = Increment(chain_sub_rule_arrays, Marshal.SizeOf(inputGlyphCount));
          for (int k = 0; k < inputGlyphCount - 1; k++)
          {
            ushort glyph = (ushort)Marshal.PtrToStructure(chain_sub_rule_arrays, typeof(ushort));
            glyph = SwapUInt16(glyph);
            inputSequence[k] = glyph;
            chain_sub_rule_arrays = Increment(chain_sub_rule_arrays, Marshal.SizeOf(glyph));
          }
          //end array


          //start array
          ushort lookaheadGlyphCount = (ushort)Marshal.PtrToStructure(chain_sub_rule_arrays, typeof(ushort));
          lookaheadGlyphCount = SwapUInt16(lookaheadGlyphCount);
          ushort[] lookAheadSequence = new ushort[lookaheadGlyphCount];
          chain_sub_rule_arrays = Increment(chain_sub_rule_arrays, Marshal.SizeOf(lookaheadGlyphCount));
          for (int k = 0; k < lookaheadGlyphCount; k++)
          {
            ushort glyph = (ushort)Marshal.PtrToStructure(chain_sub_rule_arrays, typeof(ushort));
            glyph = SwapUInt16(glyph);
            lookAheadSequence[k] = glyph;
            chain_sub_rule_arrays = Increment(chain_sub_rule_arrays, Marshal.SizeOf(glyph));
          }
          //end array


          //start array
          ushort substitutionCount = (ushort)Marshal.PtrToStructure(chain_sub_rule_arrays, typeof(ushort));
          substitutionCount = SwapUInt16(substitutionCount);
          SubstLookupRecord[] records = new SubstLookupRecord[substitutionCount];
          chain_sub_rule_arrays = Increment(chain_sub_rule_arrays, Marshal.SizeOf(substitutionCount));
          for (int k = 0; k < substitutionCount; k++)
          {
            SubstLookupRecord record = (SubstLookupRecord)Marshal.PtrToStructure(chain_sub_rule_arrays, typeof(SubstLookupRecord));
            record.GlyphSequenceIndex = SwapUInt16(record.GlyphSequenceIndex);
            record.LookupListIndex = SwapUInt16(record.LookupListIndex);
            records[k] = record;
            chain_sub_rule_arrays = Increment(chain_sub_rule_arrays, Marshal.SizeOf(record));
          }
          //end array
          chainSubClassSet[j] = new ChainingContextual2.ChainSubClassRule(backtrackSequence, inputSequence, lookAheadSequence, records);

          chain_sub_class_offset = Increment(chain_sub_class_offset, Marshal.SizeOf(offset2));
        }
        chainSubClassRuleSets[i] = chainSubClassSet;

        chain_sub_class_set_offsets = Increment(chain_sub_class_set_offsets, Marshal.SizeOf(offset));
      }

      return new ChainingContextual2(this, chainSubClassRuleSets,
          LoadCoverage(Increment(lookup_entry, type2.Coverage)),
          LoadClassDefinition(Increment(lookup_entry, type2.BacktrackClassDefOffset)),
          LoadClassDefinition(Increment(lookup_entry, type2.InputClassDefOffset)),
          LoadClassDefinition(Increment(lookup_entry, type2.LookaheadClassDefOffset)));
    }

    private Substitution LoadChainingContextSubstFormat3(IntPtr lookup_entry)
    {
      IntPtr chain_format_3_array = Increment(lookup_entry, Marshal.SizeOf(typeof(ushort)));

      //start array
      ushort backtrackGlyphCount = (ushort)Marshal.PtrToStructure(chain_format_3_array, typeof(ushort));
      backtrackGlyphCount = SwapUInt16(backtrackGlyphCount);
      Coverage[] backtrackSequence = new Coverage[backtrackGlyphCount];
      chain_format_3_array = Increment(chain_format_3_array, Marshal.SizeOf(backtrackGlyphCount));
      for (int k = 0; k < backtrackGlyphCount; k++)
      {
        ushort offset = (ushort)Marshal.PtrToStructure(chain_format_3_array, typeof(ushort));
        offset = SwapUInt16(offset);
        backtrackSequence[k] = LoadCoverage(Increment(lookup_entry, offset));
        chain_format_3_array = Increment(chain_format_3_array, Marshal.SizeOf(offset));
      }
      //end array

      //start array
      ushort inputGlyphCount = (ushort)Marshal.PtrToStructure(chain_format_3_array, typeof(ushort));
      inputGlyphCount = SwapUInt16(inputGlyphCount);
      Coverage[] inputSequence = new Coverage[inputGlyphCount];
      chain_format_3_array = Increment(chain_format_3_array, Marshal.SizeOf(inputGlyphCount));
      for (int k = 0; k < inputGlyphCount; k++)
      {
        ushort offset = (ushort)Marshal.PtrToStructure(chain_format_3_array, typeof(ushort));
        offset = SwapUInt16(offset);
        inputSequence[k] = LoadCoverage(Increment(lookup_entry, offset));
        chain_format_3_array = Increment(chain_format_3_array, Marshal.SizeOf(offset));
      }
      //end array


      //start array
      ushort lookaheadGlyphCount = (ushort)Marshal.PtrToStructure(chain_format_3_array, typeof(ushort));
      lookaheadGlyphCount = SwapUInt16(lookaheadGlyphCount);
      Coverage[] lookAheadSequence = new Coverage[lookaheadGlyphCount];
      chain_format_3_array = Increment(chain_format_3_array, Marshal.SizeOf(lookaheadGlyphCount));
      for (int k = 0; k < lookaheadGlyphCount; k++)
      {
        ushort offset = (ushort)Marshal.PtrToStructure(chain_format_3_array, typeof(ushort));
        offset = SwapUInt16(offset);
        lookAheadSequence[k] = LoadCoverage(Increment(lookup_entry, offset));
        chain_format_3_array = Increment(chain_format_3_array, Marshal.SizeOf(offset));
      }
      //end array


      //start array
      ushort substitutionCount = (ushort)Marshal.PtrToStructure(chain_format_3_array, typeof(ushort));
      substitutionCount = SwapUInt16(substitutionCount);
      SubstLookupRecord[] records = new SubstLookupRecord[substitutionCount];
      chain_format_3_array = Increment(chain_format_3_array, Marshal.SizeOf(substitutionCount));
      for (int k = 0; k < substitutionCount; k++)
      {
        SubstLookupRecord record = (SubstLookupRecord)Marshal.PtrToStructure(chain_format_3_array, typeof(SubstLookupRecord));
        record.GlyphSequenceIndex = SwapUInt16(record.GlyphSequenceIndex);
        record.LookupListIndex = SwapUInt16(record.LookupListIndex);
        records[k] = record;
        chain_format_3_array = Increment(chain_format_3_array, Marshal.SizeOf(record));
      }
      //end array

      return new ChainingContextual3(this, records, backtrackSequence, inputSequence, lookAheadSequence);
    }

#if DEBUG_TTF
    static int debug = 0;
#endif

    //[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
    [System.Security.SecurityCritical]
    private Substitution LoadChainingContext(IntPtr lookup_entry)
    {
      try
      {
        ushort format = (ushort)Marshal.PtrToStructure(lookup_entry, typeof(ushort));

        format = SwapUInt16(format);

#if DEBUG_TTF
        debug++;
        Console.WriteLine("LoadChainingContext format" + format.ToString() + " Iteration " + debug.ToString());
        if (debug == 123)
          ;
#endif
        switch (format)
        {
          case 1:
            return LoadChainingContextSubstFormat1(lookup_entry);
          case 2:
            return LoadChainingContextSubstFormat2(lookup_entry);
          case 3:
            return LoadChainingContextSubstFormat3(lookup_entry);
          default:
            throw new NotSupportedException("Format of ChaingContext");
        }
      }
      catch (System.AccessViolationException e)
      {
        Console.WriteLine(e.StackTrace);
      }

      return new VoidSubstitution();
    }

    //[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
    [System.Security.SecurityCritical]
    internal override void Load(IntPtr font)
    {
      try
      {
        gsub_ptr = Increment(font, (int)this.Offset);
        header = (GSUB_Header)Marshal.PtrToStructure(gsub_ptr, typeof(GSUB_Header));
        ChangeEndian();

        LoadLookupList();

        LoadScriptList();

        //if(do_features) LoadFeatureList();

#if DEBUG_TTF
        Console.WriteLine("=====================================================");
#endif
    }
      catch (System.Exception e)
      {
        throw e;
      }


      //foreach (DictionaryEntry script in script_list)
      //{
      //    Console.WriteLine(script.Key);
      //    foreach (DictionaryEntry language in script.Value as Hashtable)
      //    {
      //        if (language.Key != "")
      //            Console.WriteLine("\t" + language.Key);
      //        else
      //            Console.WriteLine("\tDEFAULT");

      //        foreach (DictionaryEntry features in language.Value as Hashtable)
      //        {
      //            Console.WriteLine("\t\t" + features.Key /*+ "--> LookupTypes:" */ );
      //            foreach (ushort offset in features.Value as ushort[])
      //            {
      //                LookupEntry le = (LookupEntry)lookup_list[offset];

      //                //                            for (int i = 0; i < le.record_header.SubTableCount; i++)
      //                {
      //                    Console.Write("\t\t\t");
      //                    switch ((LookupTypes)le.record_header.LookupType)
      //                    {
      //                        case LookupTypes.Single:
      //                            Console.WriteLine("Single");
      //                            break;
      //                        case LookupTypes.Multiple:
      //                            Console.WriteLine("Multiple");
      //                            break;
      //                        case LookupTypes.Alternate:
      //                            Console.WriteLine("Alternate");
      //                            break;
      //                        case LookupTypes.Ligature:
      //                            Console.WriteLine("Ligature");
      //                            break;
      //                        case LookupTypes.Context:
      //                            Console.WriteLine("Context");
      //                            break;
      //                        case LookupTypes.ChainingContext:
      //                            Console.WriteLine("ChainingContext");
      //                            break;
      //                        case LookupTypes.ExtensionSubstitution:
      //                            Console.WriteLine("ExtensionSubstitution");
      //                            break;
      //                        case LookupTypes.ReverseChainingContextSingle:
      //                            Console.WriteLine("ReverseChainingContextSingle");
      //                            break;
      //                        default:
      //                            Console.WriteLine("!!! Unknown lookup type");
      //                            break;
      //                    }
      //                }
      //            }
      //        }

      //    }
      //}
      //Console.WriteLine("=====================================================");

    }




    internal override uint Save(IntPtr font, uint offset)
    {
      this.Offset = offset;
      return base.Save(font, offset);
    }

    private void ChangeEndian()
    {
      header.Version = SwapUInt32(header.Version);
      header.ScriptList = SwapUInt16(header.ScriptList);
      header.LookupList = SwapUInt16(header.LookupList);
      header.FeatureList = SwapUInt16(header.FeatureList);
    }

    public GlyphSubstitutionClass(TrueTypeTable src) : base(src) { }

    internal List<ushort> ApplyGlyph(string script, string lang, ushort[] chars)
    {
      if (script == null)
      {
        script = "latn";
        foreach (string name in script_list.Keys)
        {
          script = name;
          break;
        }
      }

      if (script_list.ContainsKey(script))
      {
        Hashtable lang_sys_hash = script_list[script] as Hashtable;
        Hashtable lang_sys = null;
        if (lang_sys_hash.ContainsKey(lang))
        {
          lang_sys = lang_sys_hash[lang] as Hashtable;
        }
        else if (lang_sys_hash.ContainsKey(string.Empty))
        {
          lang_sys = lang_sys_hash[string.Empty] as Hashtable;
        }
        if (lang_sys != null)
        {

          switch (script)
          {
            case "arab":
              return ApplyGlyphArabic(lang_sys, chars);
          }
        }
      }
      return new List<ushort>(chars);
    }

    private void ApplyGlyphFeature(List<ushort> result, ushort[] offsets, ushort[] chars, ref int i)
    {

      foreach (ushort offset in offsets)
      {
        LookupEntry le = lookup_list[offset];
        for (int j = 0; j < le.subs.Length; j++)
          if (le.subs[j].Apply(result, chars, ref i))
            return;
      }
      result.Add(chars[i]);
    }

    private bool IsApplyGlyphFeature(int index, ushort[] offsets, ushort[] chars)
    {
      if (index < 0) return false;
      if (index >= chars.Length) return false;
      foreach (ushort offset in offsets)
      {
        LookupEntry le = lookup_list[offset];
        for (int j = 0; j < le.subs.Length; j++)
          if (le.subs[j].IsApply(chars, index) >= 0)
            return true;
      }
      return false;
    }

    private List<ushort> ApplyGlyphArabic(Hashtable lang_sys, ushort[] chars)
    {

      List<ushort> result = new List<ushort>();
      if (lang_sys.Contains("ccmp"))
      {
        ushort[] ccmp = lang_sys["ccmp"] as ushort[];
        for (int i = 0; i < chars.Length; i++)
        {
          ApplyGlyphFeature(result, ccmp, chars, ref i);
        }
        chars = result.ToArray();
        result.Clear();
      }

      ushort[] isol;
      if (lang_sys.Contains("isol")) isol = lang_sys["isol"] as ushort[];
      else isol = new ushort[0];
      ushort[] init;
      if (lang_sys.Contains("init")) init = lang_sys["init"] as ushort[];
      else init = new ushort[0];
      ushort[] medi;
      if (lang_sys.Contains("medi")) medi = lang_sys["medi"] as ushort[];
      else medi = new ushort[0];
      ushort[] fina;
      if (lang_sys.Contains("fina")) fina = lang_sys["fina"] as ushort[];
      else fina = new ushort[0];


      ArabicCharType[] types = new ArabicCharType[chars.Length];

      //Ёто костыль, так как € не знаю каким образом вытащить нужные индексы, сделаю такой вариант буду гуглить

      for (int i = 0; i < chars.Length; i++)
      {
        bool nextMedi = IsApplyGlyphFeature(i + 1, medi, chars);
        bool nextFina = IsApplyGlyphFeature(i + 1, fina, chars);
        bool prevMedi = IsApplyGlyphFeature(i - 1, medi, chars);
        bool prevInit = IsApplyGlyphFeature(i - 1, init, chars);

        if (nextMedi || nextFina)
        {
          if (prevMedi || prevInit)
          {
            bool curMedi = IsApplyGlyphFeature(i, medi, chars);
            if (curMedi)
            {
              types[i] = ArabicCharType.Medi;
              continue;
            }
          }
          else
          {
            bool curInit = IsApplyGlyphFeature(i, init, chars);
            if (curInit)
            {
              types[i] = ArabicCharType.Init;
              continue;
            }
          }
          types[i] = ArabicCharType.None;
          continue;
        }
        if (prevInit || prevMedi)
        {

          bool curFina = IsApplyGlyphFeature(i, fina, chars);
          if (curFina)
          {
            types[i] = ArabicCharType.Fina;
            continue;
          }
          types[i] = ArabicCharType.None;
          continue;
        }

        if (!nextMedi && !nextFina && !prevInit && !prevMedi)
        {
          bool curIsol = IsApplyGlyphFeature(i, isol, chars);
          if (curIsol)
          {
            types[i] = ArabicCharType.Isol;
            continue;
          }

          types[i] = ArabicCharType.None;
          continue;
        }
        types[i] = ArabicCharType.None;
      }

      for (int i = 0; i < chars.Length; i++)
      {
        switch (types[i])
        {
          case ArabicCharType.None:
            result.Add(chars[i]);
            break;
          case ArabicCharType.Isol:
            ApplyGlyphFeature(result, isol, chars, ref i);
            break;
          case ArabicCharType.Fina:
            ApplyGlyphFeature(result, fina, chars, ref i);
            break;
          case ArabicCharType.Medi:
            ApplyGlyphFeature(result, medi, chars, ref i);
            break;
          case ArabicCharType.Init:
            ApplyGlyphFeature(result, init, chars, ref i);
            break;
        }
      }

      chars = result.ToArray();
      result.Clear();

      if (lang_sys.Contains("rlig"))
      {
        ushort[] rlig = lang_sys["rlig"] as ushort[];
        for (int i = 0; i < chars.Length; i++)
        {
          ApplyGlyphFeature(result, rlig, chars, ref i);
        }
        chars = result.ToArray();
        result.Clear();
      }



      if (lang_sys.Contains("calt"))
      {
        ushort[] calt = lang_sys["calt"] as ushort[];
        for (int i = 0; i < chars.Length; i++)
        {
          ApplyGlyphFeature(result, calt, chars, ref i);
        }
        chars = result.ToArray();
        result.Clear();
      }



      return new List<ushort>(chars);
    }


    enum ArabicCharType
    {
      None, Isol, Init, Medi, Fina
    }

    private ushort[] ApplySubstLookupRecord(ushort[] resultArr, SubstLookupRecord[] records)
    {

      List<ushort> result = new List<ushort>();
      for (int j = 0; j < records.Length; j++)
      {
        result.Clear();
        int resultIndex = 0;
        for (; resultIndex < records[j].GlyphSequenceIndex; resultIndex++)
          result.Add(resultArr[resultIndex]);

        LookupEntry le = lookup_list[records[j].LookupListIndex];
        for (int k = 0; k < le.subs.Length; k++)
        {
          if (le.subs[k].Apply(result, resultArr, ref resultIndex))
          {
            resultIndex += 1;
            break;
          }
        }

        for (; resultIndex < resultArr.Length; resultIndex++)
          result.Add(resultArr[resultIndex]);
        resultArr = result.ToArray();
      }
      return resultArr;
    }

    public interface Substitution
    {
      // need to index = index  + step - 1;
      /// <summary>
      /// Return true if was applied
      /// </summary>
      /// <param name="list"></param>
      /// <param name="chars"></param>
      /// <param name="index"></param>
      /// <returns></returns>
      bool Apply(List<ushort> list, ushort[] chars, ref int index);
      /// <summary>
      /// Return coverageIndex for ApplyForce or if fail then return -1 
      /// </summary>
      /// <param name="chars"></param>
      /// <param name="index"></param>
      /// <returns></returns>
      int IsApply(ushort[] chars, int index);
      /// <summary>
      /// Apply this Substitution with specified coverageIndex, cant be called only after IsApply
      /// </summary>
      /// <param name="list"></param>
      /// <param name="chars"></param>
      /// <param name="index"></param>
      /// <param name="coverageIndex"></param>
      void ApplyForce(List<ushort> list, ushort[] chars, ref int index, int coverageIndex);
      IEnumerable<KeyValuePair<ushort[], ushort[]>> GetList(LookupTypes[] types);
    }

    public interface Coverage
    {
      int IsSubstituteGetIndex(ushort ch);
      ushort[] GetGlyphs();
      ushort GetFirstGlyph();
    }

    public struct Coverage1 : Coverage
    {
      public ushort GlyphCount;
      public ushort[] Glyphs;
      public Coverage1(ushort glyphCount, ushort[] glyphs)
      {
        this.GlyphCount = glyphCount;
        this.Glyphs = glyphs;
      }

      public ushort GetFirstGlyph()
      {
        if (Glyphs.Length > 0)
          return Glyphs[0];
        return 1;
      }

      public ushort[] GetGlyphs()
      {
        return Glyphs;
      }

      public int IsSubstituteGetIndex(ushort ch)
      {
        for (int i = 0; i < GlyphCount; i++)
        {
          if (Glyphs[i] == ch)
            return i;
        }
        return -1;
      }
    }

    public struct Coverage2 : Coverage
    {
      public ushort GlyphCount;
      public RangeRecord[] RangeRecords;
      public Coverage2(ushort glyphCount, RangeRecord[] rangeRecords)
      {
        this.GlyphCount = glyphCount;
        this.RangeRecords = rangeRecords;
      }

      public ushort GetFirstGlyph()
      {
        if (RangeRecords.Length > 0)
          return RangeRecords[0].Start;
        return 1;
      }

      public ushort[] GetGlyphs()
      {
        Dictionary<int, ushort> results = new Dictionary<int, ushort>();

        for (int i = 0; i < RangeRecords.Length; i++)
        {
          for (int j = 0; j + RangeRecords[i].Start <= RangeRecords[i].End; j++)
          {
            results[j + RangeRecords[i].StartCoverageIndex] =
                (ushort)(RangeRecords[i].Start + j);
          }
        }
        ushort[] result = new ushort[results.Count];
        foreach (ushort key in results.Keys)
        {
          result[key] = results[key];
        }
        return result;
      }

      public int IsSubstituteGetIndex(ushort ch)
      {
        //var arr = RangeRecords.Select(a => a).OrderBy(a => a.StartCoverageIndex).ToArray();
        for (int i = 0; i < GlyphCount; i++)
        {
          if (RangeRecords[i].Start <= ch && RangeRecords[i].End >= ch)
            return RangeRecords[i].End - RangeRecords[i].Start + RangeRecords[i].StartCoverageIndex;
        }
        return -1;
      }
    }

    public struct Single1 : Substitution
    {
      public short DeltaGlyphIDOrGlyphCount;
      public Coverage Coverage;

      public Single1(short deltaGlyphID, Coverage coverage)
      {
        this.DeltaGlyphIDOrGlyphCount = deltaGlyphID;
        this.Coverage = coverage;
      }
      public bool Apply(List<ushort> list, ushort[] chars, ref int index)
      {

        int index2 = IsApply(chars, index);
        if (index2 >= 0)
        {
          ApplyForce(list, chars, ref index, index2);
          return true;
        }
        return false;
      }

      public void ApplyForce(List<ushort> list, ushort[] chars, ref int index, int coverageIndex)
      {
        list.Add((ushort)(chars[index] + DeltaGlyphIDOrGlyphCount));
      }

      public IEnumerable<KeyValuePair<ushort[], ushort[]>> GetList(LookupTypes[] types)
      {

        if (types != null && !ExtFor2005.Contains(types, LookupTypes.Single))
          yield break;
        ushort[] glyphs = Coverage.GetGlyphs();
        for (int i = 0; i < glyphs.Length; i++)
        {
          yield return new KeyValuePair<ushort[], ushort[]>(
              new ushort[] { glyphs[i] },
              new ushort[] { (ushort)(glyphs[i] + DeltaGlyphIDOrGlyphCount) }
              );
        }
      }

      public int IsApply(ushort[] chars, int index)
      {
        return Coverage.IsSubstituteGetIndex(chars[index]);
      }
    }

    public struct Single2 : Substitution
    {
      public ushort[] Substitutes;
      public Coverage Coverage;

      public Single2(ushort[] substitutes, Coverage coverage)
      {
        this.Substitutes = substitutes;
        this.Coverage = coverage;
      }

      public bool Apply(List<ushort> list, ushort[] chars, ref int index)
      {

        int index2 = IsApply(chars, index);
        if (index2 >= 0)
        {
          ApplyForce(list, chars, ref index, index2);
          return true;
        }
        return false;
      }



      public void ApplyForce(List<ushort> list, ushort[] chars, ref int index, int coverageIndex)
      {
        list.Add(Substitutes[coverageIndex]);
      }

      public IEnumerable<KeyValuePair<ushort[], ushort[]>> GetList(LookupTypes[] types)
      {

        if (types != null && !ExtFor2005.Contains(types, LookupTypes.Single))
          yield break;
        ushort[] glyphs = Coverage.GetGlyphs();
        for (int i = 0; i < glyphs.Length; i++)
        {
          yield return new KeyValuePair<ushort[], ushort[]>(
              new ushort[] { glyphs[i] },
              new ushort[] { Substitutes[i] }
              );
        }
      }

      public int IsApply(ushort[] chars, int index)
      {
        return Coverage.IsSubstituteGetIndex(chars[index]);
      }
    }

    public struct Multiple : Substitution
    {
      public ushort[][] Sequences;
      public Coverage Coverage;

      public Multiple(ushort[][] sequences, Coverage coverage)
      {
        this.Sequences = sequences;
        this.Coverage = coverage;
      }

      public bool Apply(List<ushort> list, ushort[] chars, ref int index)
      {

        int index2 = IsApply(chars, index);
        if (index2 >= 0)
        {
          ApplyForce(list, chars, ref index, index2);
          return true;
        }
        return false;
      }


      public void ApplyForce(List<ushort> list, ushort[] chars, ref int index, int coverageIndex)
      {
        list.AddRange(Sequences[coverageIndex]);
      }

      public IEnumerable<KeyValuePair<ushort[], ushort[]>> GetList(LookupTypes[] types)
      {

        if (types != null && !ExtFor2005.Contains(types, LookupTypes.Multiple))
          yield break;
        ushort[] glyphs = Coverage.GetGlyphs();
        for (int i = 0; i < glyphs.Length; i++)
        {
          yield return new KeyValuePair<ushort[], ushort[]>(
              new ushort[] { glyphs[i] },
              Sequences[i]);
        }
      }

      public int IsApply(ushort[] chars, int index)
      {
        return Coverage.IsSubstituteGetIndex(chars[index]);
      }
    }

    public struct Ligature : Substitution
    {
      public Coverage Coverage;
      public LigatureSet[][] LigatureSets;
      LigatureSet LastSetIsApply;

      public Ligature(Coverage coverage, LigatureSet[][] ligatureSets)
      {
        this.Coverage = coverage;
        this.LigatureSets = ligatureSets;
        LastSetIsApply = new LigatureSet();
      }

      public bool Apply(List<ushort> list, ushort[] chars, ref int index)
      {

        int index2 = IsApply(chars, index);
        if (index2 >= 0)
        {
          ApplyForce(list, chars, ref index, index2);
          return true;
        }
        return false;
      }


      public void ApplyForce(List<ushort> list, ushort[] chars, ref int index, int coverageIndex)
      {
        index += LastSetIsApply.Components.Length;
        list.Add(LastSetIsApply.LigGlyph);
      }

      public IEnumerable<KeyValuePair<ushort[], ushort[]>> GetList(LookupTypes[] types)
      {

        if (types != null && !ExtFor2005.Contains(types, LookupTypes.Ligature))
          yield break;
        ushort[] glyphs = Coverage.GetGlyphs();
        for (int i = 0; i < glyphs.Length; i++)
        {
          LigatureSet[] set = LigatureSets[i];
          for (int j = 0; j < set.Length; j++)
          {
            ushort[] key = new ushort[set[j].Components.Length + 1];
            key[0] = glyphs[i];
            for (int k = 0; k < set[j].Components.Length; k++)
              key[k + 1] = set[j].Components[k];


            yield return new KeyValuePair<ushort[], ushort[]>(
                key,
                new ushort[] { set[j].LigGlyph });
          }
        }
      }

      public int IsApply(ushort[] chars, int index)
      {

        int index2 = Coverage.IsSubstituteGetIndex(chars[index]);
        if (index2 >= 0)
        {
          LigatureSet[] sets = LigatureSets[index2];
          foreach (LigatureSet set in sets)
          {
            if (chars.Length - 1 - index - set.Components.Length >= 0)
            {
              bool flag = true;
              for (int i = 0; i < set.Components.Length; i++)
              {
                if (set.Components[i] != chars[index + 1 + i])
                {
                  flag = false;
                  break;
                }
              }
              if (flag)
              {
                LastSetIsApply = set;
                return index2;
              }
            }
          }
        }
        return -1;
      }
    }

    public struct LigatureSet
    {
      public ushort LigGlyph;
      public ushort[] Components;
      public LigatureSet(ushort ligGlyph, ushort[] components)
      {
        this.LigGlyph = ligGlyph;
        this.Components = components;
      }
    }


    public struct Contextual1 : Substitution
    {

      public struct SubRule
      {
        public ushort[] InputSequence;
        public SubstLookupRecord[] Records;

        public SubRule(ushort[] inputSequence, SubstLookupRecord[] records)
        {
          InputSequence = inputSequence;
          Records = records;
        }
      }


      public SubRule[][] SubRuleSets;
      public Coverage Coverage;
      private GlyphSubstitutionClass gsub_table;
      SubRule LastSubRuleIsApply;



      public Contextual1(GlyphSubstitutionClass gsub_table, SubRule[][] subRuleSets, Coverage coverage)
      {
        this.gsub_table = gsub_table;
        SubRuleSets = subRuleSets;
        Coverage = coverage;
        LastSubRuleIsApply = new SubRule();
      }


      public bool Apply(List<ushort> list, ushort[] chars, ref int index)
      {

        int index2 = IsApply(chars, index);
        if (index2 >= 0)
        {
          ApplyForce(list, chars, ref index, index2);
          return true;
        }
        return false;
      }


      public IEnumerable<KeyValuePair<ushort[], ushort[]>> GetList(LookupTypes[] types)
      {

        if (types != null && !ExtFor2005.Contains(types, LookupTypes.Context))
          yield break;
        ushort[] glyphs = Coverage.GetGlyphs();
        for (int i = 0; i < glyphs.Length; i++)
        {
          SubRule[] set = SubRuleSets[i];
          for (int j = 0; j < set.Length; j++)
          {
            ushort[] key = new ushort[set[j].InputSequence.Length + 1];
            key[0] = glyphs[i];
            for (int k = 0; k < set[j].InputSequence.Length; k++)
              key[k + 1] = set[j].InputSequence[k];

            yield return new KeyValuePair<ushort[], ushort[]>(
                key,
                gsub_table.ApplySubstLookupRecord(key, set[j].Records));
          }
        }
      }

      public int IsApply(ushort[] chars, int index)
      {
        int index2 = Coverage.IsSubstituteGetIndex(chars[index]);
        if (index2 >= 0)
        {
          SubRule[] subRules = SubRuleSets[index2];
          foreach (SubRule rule in subRules)
          {
            if (chars.Length - 1 - index - rule.InputSequence.Length >= 0)
            {
              bool flag = true;
              for (int i = 0; i < rule.InputSequence.Length; i++)
              {
                if (rule.InputSequence[i] != chars[index + 1 + i])
                {
                  flag = false;
                  break;
                }
              }
              if (flag)
              {
                LastSubRuleIsApply = rule;
                return index2;
              }
            }
          }
        }
        return -1;
      }

      public void ApplyForce(List<ushort> list, ushort[] chars, ref int index, int coverageIndex)
      {
        ushort[] resultArr = new ushort[LastSubRuleIsApply.InputSequence.Length + 1];
        for (int j = 0; j < resultArr.Length; j++)
        {
          resultArr[j] = chars[j + index];
        }
        list.AddRange(gsub_table.ApplySubstLookupRecord(resultArr, LastSubRuleIsApply.Records));
        index += LastSubRuleIsApply.InputSequence.Length;
      }
    }

    public struct Contextual2 : Substitution
    {
      public struct SubClassRule
      {
        public ushort[] InputSequence;
        public SubstLookupRecord[] Records;

        public SubClassRule(ushort[] inputSequence, SubstLookupRecord[] records)
        {
          InputSequence = inputSequence;
          Records = records;
        }
      }

      public SubClassRule[][] SubClassRuleSets;
      private GlyphSubstitutionClass gsub_table;
      public Coverage Coverage;
      public ClassDefinition ClassDefinition;
      SubClassRule LastSubClassRule;

      public Contextual2(GlyphSubstitutionClass gsub_table, SubClassRule[][] subClassRuleSets, Coverage coverage, ClassDefinition classDefinition)
      {
        this.gsub_table = gsub_table;
        SubClassRuleSets = subClassRuleSets;
        Coverage = coverage;
        ClassDefinition = classDefinition;
        LastSubClassRule = new SubClassRule();
      }

      public bool Apply(List<ushort> list, ushort[] chars, ref int index)
      {

        int index2 = IsApply(chars, index);
        if (index2 >= 0)
        {
          ApplyForce(list, chars, ref index, index2);
          return true;
        }
        return false;
      }

      public IEnumerable<KeyValuePair<ushort[], ushort[]>> GetList(LookupTypes[] types)
      {

        if (types != null && !ExtFor2005.Contains(types, LookupTypes.Context))
          yield break;
        ushort[] glyphs = Coverage.GetGlyphs();
        for (int i = 0; i < glyphs.Length; i++)
        {
          SubClassRule[] set = SubClassRuleSets[i];
          if (set != null)
            for (int j = 0; j < set.Length; j++)
            {
              ushort[] key = new ushort[set[j].InputSequence.Length + 1];
              key[0] = glyphs[i];
              for (int k = 0; k < set[j].InputSequence.Length; k++)
                key[k + 1] = ClassDefinition.GetFirstGlyphByClassValue(set[j].InputSequence[k]);

              yield return new KeyValuePair<ushort[], ushort[]>(
                  key,
                  gsub_table.ApplySubstLookupRecord(key, set[j].Records));
            }
        }
      }

      public int IsApply(ushort[] chars, int index)
      {
        int index2 = Coverage.IsSubstituteGetIndex(chars[index]);
        if (index2 >= 0)
        {
          SubClassRule[] subClassRules = SubClassRuleSets[index2];
          if (subClassRules != null)
            foreach (SubClassRule rule in subClassRules)
            {
              if (chars.Length - 1 - index - rule.InputSequence.Length >= 0)
              {
                bool flag = true;
                for (int i = 0; i < rule.InputSequence.Length; i++)
                {
                  if (rule.InputSequence[i] != ClassDefinition.GetClassValue(chars[index + 1 + i]))
                  {
                    flag = false;
                    break;
                  }
                }
                if (flag)
                {
                  LastSubClassRule = rule;
                  return index2;
                }
              }
            }
        }
        return -1;
      }

      public void ApplyForce(List<ushort> list, ushort[] chars, ref int index, int coverageIndex)
      {
        ushort[] resultArr = new ushort[LastSubClassRule.InputSequence.Length + 1];
        for (int j = 0; j < resultArr.Length; j++)
        {
          resultArr[j] = chars[j + index];
        }
        list.AddRange(gsub_table.ApplySubstLookupRecord(resultArr, LastSubClassRule.Records));
        index += LastSubClassRule.InputSequence.Length;
      }
    }


    public struct Contextual3 : Substitution
    {




      public SubstLookupRecord[] Records;
      public Coverage[] Coverages;

      private GlyphSubstitutionClass gsub_table;



      public Contextual3(GlyphSubstitutionClass gsub_table, SubstLookupRecord[] records, Coverage[] coverages)
      {
        this.gsub_table = gsub_table;
        Records = records;
        Coverages = coverages;
      }

      public bool Apply(List<ushort> list, ushort[] chars, ref int index)
      {

        int index2 = IsApply(chars, index);
        if (index2 >= 0)
        {
          ApplyForce(list, chars, ref index, index2);
          return true;
        }
        return false;
      }



      public void ApplyForce(List<ushort> list, ushort[] chars, ref int index, int coverageIndex)
      {
        ushort[] resultArr = new ushort[Coverages.Length];
        for (int j = 0; j < resultArr.Length; j++)
        {
          resultArr[j] = chars[j + index];
        }
        list.AddRange(gsub_table.ApplySubstLookupRecord(resultArr, Records));
        index += Coverages.Length - 1;
      }

      public IEnumerable<KeyValuePair<ushort[], ushort[]>> GetList(LookupTypes[] types)
      {

        if (types != null && !ExtFor2005.Contains(types, LookupTypes.Context))
          yield break;
        if (Coverages.Length > 0)
        {
          ushort[] glyphs = Coverages[0].GetGlyphs();
          for (int i = 0; i < glyphs.Length; i++)
          {

            ushort[] key = new ushort[Coverages.Length];
            key[0] = glyphs[i];
            for (int k = 1; k < Coverages.Length; k++)
              key[k] = Coverages[k].GetFirstGlyph();

            yield return new KeyValuePair<ushort[], ushort[]>(
                key,
                gsub_table.ApplySubstLookupRecord(key, Records));

          }
        }
      }

      public int IsApply(ushort[] chars, int index)
      {
        if (Coverages.Length > 0)
        {
          int index2 = Coverages[0].IsSubstituteGetIndex(chars[index]);
          if (index2 >= 0)
          {

            if (chars.Length - index - Coverages.Length >= 0)
            {
              bool flag = true;
              for (int i = 1; i < Coverages.Length; i++)
              {
                if (Coverages[i].IsSubstituteGetIndex(chars[index + i]) < 0)
                {
                  flag = false;
                  break;
                }
              }
              if (flag)
              {

                return index;
              }
            }
          }

        }
        return -1;
      }
    }



    public struct ChainingContextual1 : Substitution
    {

      public struct ChainSubRule
      {
        public ushort[] BacktrackSequence;
        public ushort[] InputSequence; // count -= 1;
        public ushort[] LookAheadSequence;
        public SubstLookupRecord[] Records;

        public ChainSubRule(ushort[] backtrackSequence, ushort[] inputSequence, ushort[] lookAheadSequence, SubstLookupRecord[] records)
        {
          BacktrackSequence = backtrackSequence;
          InputSequence = inputSequence;
          LookAheadSequence = lookAheadSequence;
          Records = records;
        }
      }

      public GlyphSubstitutionClass gsub_table;
      public ChainSubRule[][] ChainSubRuleSets;
      public Coverage Coverage;
      ChainSubRule LastChainSubRule;



      public ChainingContextual1(GlyphSubstitutionClass gsub_table, Coverage coverage, ChainSubRule[][] chainSubRuleSets)
      {
        this.gsub_table = gsub_table;
        ChainSubRuleSets = chainSubRuleSets;
        Coverage = coverage;
        LastChainSubRule = new ChainSubRule();
      }

      public bool Apply(List<ushort> list, ushort[] chars, ref int index)
      {

        int index2 = IsApply(chars, index);
        if (index2 >= 0)
        {
          ApplyForce(list, chars, ref index, index2);
          return true;
        }
        return false;
      }

      public IEnumerable<KeyValuePair<ushort[], ushort[]>> GetList(LookupTypes[] types)
      {

        if (types != null && !ExtFor2005.Contains(types, LookupTypes.ChainingContext))
          yield break;
        ushort[] glyphs = Coverage.GetGlyphs();
        for (int i = 0; i < glyphs.Length; i++)
        {
          ChainSubRule[] set = ChainSubRuleSets[i];
          for (int j = 0; j < set.Length; j++)
          {
            ushort[] key = new ushort[set[j].InputSequence.Length + 1];
            key[0] = glyphs[i];
            for (int k = 0; k < set[j].InputSequence.Length; k++)
              key[k + 1] = set[j].InputSequence[k];

            yield return new KeyValuePair<ushort[], ushort[]>(
                key,
                gsub_table.ApplySubstLookupRecord(key, set[j].Records));
          }
        }
      }

      public int IsApply(ushort[] chars, int index)
      {
        int index2 = Coverage.IsSubstituteGetIndex(chars[index]);
        if (index2 >= 0)
        {
          ChainSubRule[] subRules = ChainSubRuleSets[index2];
          foreach (ChainSubRule rule in subRules)
          {
            if (chars.Length - 1 - index - rule.InputSequence.Length >= 0 &&
                chars.Length - 1 - index - rule.LookAheadSequence.Length - rule.InputSequence.Length >= 0 &&
                index >= rule.BacktrackSequence.Length)
            {
              bool flag = true;
              for (int i = 0; i < rule.InputSequence.Length; i++)
              {
                if (rule.InputSequence[i] != chars[index + 1 + i])
                {
                  flag = false;
                  break;
                }
              }
              if (flag)
                for (int i = 0; i < rule.BacktrackSequence.Length; i++)
                {
                  if (rule.BacktrackSequence[i] != chars[index - i - 1])
                  {
                    flag = false;
                    break;
                  }
                }
              if (flag)
                for (int i = 0; i < rule.LookAheadSequence.Length; i++)
                {
                  if (rule.LookAheadSequence[i] != chars[index + i + 1 + rule.InputSequence.Length])
                  {
                    flag = false;
                    break;
                  }
                }
              if (flag)
              {
                LastChainSubRule = rule;
                return index2;
              }
            }
          }
        }
        return -1;
      }

      public void ApplyForce(List<ushort> list, ushort[] chars, ref int index, int coverageIndex)
      {
        ushort[] resultArr = new ushort[LastChainSubRule.InputSequence.Length + 1];
        for (int j = 0; j < resultArr.Length; j++)
        {
          resultArr[j] = chars[j + index];
        }
        list.AddRange(gsub_table.ApplySubstLookupRecord(resultArr, LastChainSubRule.Records));
        index += LastChainSubRule.InputSequence.Length;
      }
    }


    public struct ChainingContextual2 : Substitution
    {
      public struct ChainSubClassRule
      {
        public ushort[] BacktrackSequence;
        public ushort[] InputSequence; // count -= 1;
        public ushort[] LookAheadSequence;
        public SubstLookupRecord[] Records;

        public ChainSubClassRule(ushort[] backtrackSequence, ushort[] inputSequence, ushort[] lookAheadSequence, SubstLookupRecord[] records)
        {
          BacktrackSequence = backtrackSequence;
          InputSequence = inputSequence;
          LookAheadSequence = lookAheadSequence;
          Records = records;
        }
      }

      public ChainSubClassRule[][] SubClassRuleSets;
      private GlyphSubstitutionClass gsub_table;
      public Coverage Coverage;
      public ClassDefinition BacktrackClassDefinition;
      public ClassDefinition InputClassDefinition;
      public ClassDefinition LookaheadClassDefinition;

      ChainSubClassRule LastChainSubClassRule;

      public ChainingContextual2(
          GlyphSubstitutionClass gsub_table,
          ChainSubClassRule[][] subClassRuleSets,
          Coverage coverage,
          ClassDefinition backtrackClassDefinition,
          ClassDefinition inputClassDefinition,
          ClassDefinition lookaheadClassDefinition)
      {
        this.gsub_table = gsub_table;
        SubClassRuleSets = subClassRuleSets;
        Coverage = coverage;
        BacktrackClassDefinition = backtrackClassDefinition;
        InputClassDefinition = inputClassDefinition;
        LookaheadClassDefinition = lookaheadClassDefinition;
        LastChainSubClassRule = new ChainSubClassRule();
      }


      public bool Apply(List<ushort> list, ushort[] chars, ref int index)
      {

        int index2 = IsApply(chars, index);
        if (index2 >= 0)
        {
          ApplyForce(list, chars, ref index, index2);
          return true;
        }
        return false;
      }



      public IEnumerable<KeyValuePair<ushort[], ushort[]>> GetList(LookupTypes[] types)
      {

        if (types != null && !ExtFor2005.Contains(types, LookupTypes.ChainingContext))
          yield break;
        ushort[] glyphs = Coverage.GetGlyphs();
        for (int i = 0; i < glyphs.Length; i++)
        {
          ChainSubClassRule[] set = SubClassRuleSets[i];
          if (set != null)
            for (int j = 0; j < set.Length; j++)
            {
              ushort[] key = new ushort[set[j].InputSequence.Length + 1];
              key[0] = glyphs[i];
              for (int k = 0; k < set[j].InputSequence.Length; k++)
                key[k + 1] = InputClassDefinition.GetFirstGlyphByClassValue(set[j].InputSequence[k]);

              yield return new KeyValuePair<ushort[], ushort[]>(
                  key,
                  gsub_table.ApplySubstLookupRecord(key, set[j].Records));
            }
        }
      }

      public int IsApply(ushort[] chars, int index)
      {
        int index2 = Coverage.IsSubstituteGetIndex(chars[index]);
        if (index2 >= 0)
        {
          ChainSubClassRule[] subClassRules = SubClassRuleSets[index2];
          if (subClassRules != null)
            foreach (ChainSubClassRule rule in subClassRules)
            {
              if (chars.Length - 1 - index - rule.InputSequence.Length >= 0 &&
              chars.Length - 1 - index - rule.LookAheadSequence.Length - rule.InputSequence.Length >= 0 &&
              index >= rule.BacktrackSequence.Length)
              {
                bool flag = true;
                for (int i = 0; i < rule.InputSequence.Length; i++)
                {
                  if (rule.InputSequence[i] != InputClassDefinition.GetClassValue(chars[index + 1 + i]))
                  {
                    flag = false;
                    break;
                  }
                }
                if (flag)
                  for (int i = 0; i < rule.BacktrackSequence.Length; i++)
                  {
                    if (rule.BacktrackSequence[i] != BacktrackClassDefinition.GetClassValue(chars[index - i - 1]))
                    {
                      flag = false;
                      break;
                    }
                  }
                if (flag)
                  for (int i = 0; i < rule.LookAheadSequence.Length; i++)
                  {
                    if (rule.LookAheadSequence[i] !=
                        LookaheadClassDefinition.GetClassValue(
                            chars[index + i + 1 + rule.InputSequence.Length]))
                    {
                      flag = false;
                      break;
                    }
                  }
                if (flag)
                {
                  LastChainSubClassRule = rule;
                  return index2;
                }
              }
            }
        }
        return -1;
      }

      public void ApplyForce(List<ushort> list, ushort[] chars, ref int index, int coverageIndex)
      {
        ushort[] resultArr = new ushort[LastChainSubClassRule.InputSequence.Length + 1];
        for (int j = 0; j < resultArr.Length; j++)
        {
          resultArr[j] = chars[j + index];
        }
        list.AddRange(gsub_table.ApplySubstLookupRecord(resultArr, LastChainSubClassRule.Records));
        index += LastChainSubClassRule.InputSequence.Length;
      }
    }

    public struct ChainingContextual3 : Substitution
    {




      public SubstLookupRecord[] Records;
      public Coverage[] BacktrackCoverages;
      public Coverage[] InputCoverages;
      public Coverage[] LookaheadCoverages;



      private GlyphSubstitutionClass gsub_table;



      public ChainingContextual3(GlyphSubstitutionClass gsub_table, SubstLookupRecord[] records, Coverage[] backtrackCoverages, Coverage[] inputCoverages, Coverage[] lookaheadCoverages)
      {
        this.gsub_table = gsub_table;
        Records = records;
        BacktrackCoverages = backtrackCoverages;
        InputCoverages = inputCoverages;
        LookaheadCoverages = lookaheadCoverages;
      }


      public bool Apply(List<ushort> list, ushort[] chars, ref int index)
      {

        int index2 = IsApply(chars, index);
        if (index2 >= 0)
        {
          ApplyForce(list, chars, ref index, index2);
          return true;
        }
        return false;
      }


      public void ApplyForce(List<ushort> list, ushort[] chars, ref int index, int coverageIndex)
      {
        ushort[] resultArr = new ushort[InputCoverages.Length];
        for (int j = 0; j < resultArr.Length; j++)
        {
          resultArr[j] = chars[j + index];
        }
        list.AddRange(gsub_table.ApplySubstLookupRecord(resultArr, Records));
        index += InputCoverages.Length - 1;
      }

      public IEnumerable<KeyValuePair<ushort[], ushort[]>> GetList(LookupTypes[] types)
      {

        if (types != null && !ExtFor2005.Contains(types, LookupTypes.ChainingContext))
          yield break;
        if (InputCoverages.Length > 0)
        {
          ushort[] glyphs = InputCoverages[0].GetGlyphs();
          for (int i = 0; i < glyphs.Length; i++)
          {
            ushort[] key = new ushort[InputCoverages.Length];
            key[0] = glyphs[i];
            if (InputCoverages.Length > 0)
              for (int k = 1; k < InputCoverages.Length; k++)
                key[k] = InputCoverages[k].GetFirstGlyph();
            yield return new KeyValuePair<ushort[], ushort[]>(
                key,
                gsub_table.ApplySubstLookupRecord(key, Records));
          }
        }
      }

      public int IsApply(ushort[] chars, int index)
      {
        if (InputCoverages.Length > 0)
        {
          int index2 = InputCoverages[0].IsSubstituteGetIndex(chars[index]);
          if (index2 >= 0)
          {

            if (chars.Length - 1 - index - InputCoverages.Length >= 0 &&
                chars.Length - 1 - index - LookaheadCoverages.Length - InputCoverages.Length >= 0 &&
                index >= BacktrackCoverages.Length)
            {
              bool flag = true;
              for (int i = 1; i < InputCoverages.Length; i++)
              {
                if (InputCoverages[i].IsSubstituteGetIndex(chars[index + i]) < 0)
                {
                  flag = false;
                  break;
                }
              }
              if (flag)
                for (int i = 0; i < BacktrackCoverages.Length; i++)
                {
                  if (BacktrackCoverages[i].IsSubstituteGetIndex(chars[index - i - 1]) < 0)
                  {
                    flag = false;
                    break;
                  }
                }
              if (flag)
                for (int i = 0; i < LookaheadCoverages.Length; i++)
                {
                  if (LookaheadCoverages[i].IsSubstituteGetIndex(
                          chars[index + i + 1 + InputCoverages.Length]) < 0)
                  {
                    flag = false;
                    break;
                  }
                }
              if (flag)
              {
                return index2;
              }
            }
          }

        }
        return -1;
      }
    }

    public struct Extension : Substitution
    {
      public Substitution Substitution;
      public ushort LookupType;
      public ushort Format;

      public bool Apply(List<ushort> list, ushort[] chars, ref int index)
      {
        return Substitution.Apply(list, chars, ref index);
      }

      public void ApplyForce(List<ushort> list, ushort[] chars, ref int index, int coverageIndex)
      {
        Substitution.ApplyForce(list, chars, ref index, coverageIndex);
      }

      public IEnumerable<KeyValuePair<ushort[], ushort[]>> GetList(LookupTypes[] types)
      {

        if (types != null && !ExtFor2005.Contains(types, LookupTypes.ExtensionSubstitution))
          return new KeyValuePair<ushort[], ushort[]>[0];
        return Substitution.GetList(types);
      }

      public int IsApply(ushort[] chars, int index)
      {
        return Substitution.IsApply(chars, index);
      }
    }

    public struct VoidSubstitution : Substitution
    {
      public bool Apply(List<ushort> list, ushort[] chars, ref int index)
      {
        return false;
      }

      public void ApplyForce(List<ushort> list, ushort[] chars, ref int index, int coverageIndex)
      {

      }

      public IEnumerable<KeyValuePair<ushort[], ushort[]>> GetList(LookupTypes[] types)
      {
        yield break;
      }

      public int IsApply(ushort[] chars, int index)
      {
        return -1;
      }
    }

    internal IEnumerable<KeyValuePair<ushort[], ushort[]>> GetSubstitutions(string script, string language, string feature, LookupTypes[] types)
    {
      if (script_list.ContainsKey(script))
      {
        Hashtable lang_sys_hash = script_list[script] as Hashtable;
        Hashtable lang_sys = null;
        if (lang_sys_hash.ContainsKey(language))
        {
          lang_sys = lang_sys_hash[language] as Hashtable;
        }
        else if (lang_sys_hash.ContainsKey(string.Empty))
        {
          lang_sys = lang_sys_hash[string.Empty] as Hashtable;
        }
        if (lang_sys != null)
        {
          if (lang_sys.ContainsKey(feature))
          {
            foreach (ushort offset in (ushort[])lang_sys[feature])
            {
              LookupEntry le = (LookupEntry)lookup_list[offset];
              for (int i = 0; i < le.subs.Length; i++)
              {
                foreach (KeyValuePair<ushort[], ushort[]> sub in le.subs[i].GetList(types))
                  yield return sub;
              }
            }
          }

        }
      }
    }

    public struct VoidCoverage : Coverage
    {
      public ushort GetFirstGlyph()
      {
        return 1;
      }

      public ushort[] GetGlyphs()
      {
        return new ushort[0];
      }

      public int IsSubstituteGetIndex(ushort ch)
      {
        return -1;
      }
    }


    public interface ClassDefinition
    {
      ushort GetClassValue(ushort glyph);
      ushort GetFirstGlyphByClassValue(ushort v);
    }

    public struct ClassDefinition1 : ClassDefinition
    {
      ushort StartGlyphId;
      ushort[] ClassValues;

      public ClassDefinition1(ushort startGlyphId, ushort[] classValues)
      {
        StartGlyphId = startGlyphId;
        ClassValues = classValues;
      }

      public ushort GetClassValue(ushort glyph)
      {
        int index = glyph - StartGlyphId;
        if (index >= 0 && index < ClassValues.Length)
          return ClassValues[index];
        return 0;
      }

      public ushort GetFirstGlyphByClassValue(ushort v)
      {
        for (int i = 0; i < ClassValues.Length; i++)
          if (ClassValues[i] == v)
            return (ushort)(StartGlyphId + i);
        return StartGlyphId;
      }
    }

    public struct ClassDefinition2 : ClassDefinition
    {
      ClassRangeRecord[] Records;

      public ClassDefinition2(ClassRangeRecord[] records)
      {
        Records = records;
      }

      public ushort GetClassValue(ushort glyph)
      {
        foreach (ClassRangeRecord record in Records)
          if (record.StartGlyphID <= glyph && record.EndGlyphID >= glyph)
            return record.ClassValue;
        return 0;
      }

      public ushort GetFirstGlyphByClassValue(ushort v)
      {
        foreach (ClassRangeRecord record in Records)
          if (record.ClassValue == v)
            return record.StartGlyphID;
        if (Records.Length > 0)
          return Records[0].StartGlyphID;
        return 0;
      }
    }

    public struct VoidClassDefinition : ClassDefinition
    {
      public ushort GetClassValue(ushort glyph)
      {
        return 0;
      }

      public ushort GetFirstGlyphByClassValue(ushort v)
      {
        //TODO
        //TO DO
        // WHAT TO DO??? just return zero value, let it be space plz :)
        return 1;
      }
    }
  }
}
#pragma warning restore
