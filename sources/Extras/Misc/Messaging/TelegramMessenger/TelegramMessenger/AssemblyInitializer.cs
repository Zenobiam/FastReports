using FastReport.Utils;

namespace FastReport.Messaging
{
    class TelegramAssemblyInitializer : AssemblyInitializerBase
    {
        public TelegramAssemblyInitializer()
        {
            RegisteredObjects.AddMessenger(typeof(TelegramMessenger), "Messaging,Telegram,Name");
        }
    }
}
