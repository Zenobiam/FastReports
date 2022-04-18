using FastReport.Fonts;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace FastReport.Utils
{
    public static partial class Config
    {
        private static FastReport.Export.Email.EmailSettings FEmailSettings = new FastReport.Export.Email.EmailSettings();
        private static TrueTypeCollection FFontCollection = new TrueTypeCollection();

        /// <summary>
        /// Gets or sets the settings for the "Send Email" window.
        /// </summary>
        public static FastReport.Export.Email.EmailSettings EmailSettings
        {
            get { return FEmailSettings; }
            set { FEmailSettings = value; }
        }


        /// <summary>
        /// Get access to font collection
        /// </summary>
        public static TrueTypeCollection FontCollection
        {
            get { return FFontCollection; }
        }

        private static void ProcessMainAssembly()
        {
            new AssemblyInitializer();
#if !FRCORE
            new AssemblyInitializerDesignExt();
#endif
            new AssemblyInitializerBaseExt();
        }

    }
}