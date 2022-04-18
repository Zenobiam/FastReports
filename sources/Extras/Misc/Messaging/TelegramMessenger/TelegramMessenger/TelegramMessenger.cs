using FastReport.Cloud;
using FastReport.Cloud.StorageClient.FastCloud;
using FastReport.Export;
using FastReport.Utils;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TeleSharp.TL;
using TeleSharp.TL.Account;
using TeleSharp.TL.Contacts;
using TLSharp.Core;
using TLSharp.Core.Utils;

namespace FastReport.Messaging
{
    /// <summary>
    /// Represents the Telegram messenger.
    /// </summary>
    /// How to use: 
    ///        TelegramMessenger telegram = new TelegramMessenger("78005553535", "myTelegramPass", "78002000600");
    ///        telegram.Connect();
    ///        if (!telegram.Authorized)
    ///        {
    ///            telegram.RequestCode();
    ///            string codeToAuthenticate;
    ///            Console.Write("Enter the Authentication Code:");
    ///            codeToAuthenticate = Console.ReadLine();
    ///            Console.ReadKey();
    ///            telegram.Authorize(codeToAuthenticate, password);
    ///        }
    ///        Report report = new Report();
    ///        report.LoadPrepared("Complex (Master-detail + Group).fpx");
    ///        PDFExport pdf = new PDFExport();
    ///        telegram.SendReport(report, pdf);
    public class TelegramMessenger : MessengerBase, IDisposable
    {
        #region Constants
        private const int API_ID = 193294;
        private const string API_HASH = "90d997bc546d749f0991f3b209ef4f58";
        #endregion //Constants

        #region Fields
        private string senderPhone;
        private string password;
        private string recipientPhone;
        private string hash;
        private string codeToAuthenticate;
        private TLUser sender;
        private TLUser recipient;
        private TelegramClient client;
        private bool connected;
        private bool disposed;
        private bool authorized;
        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets or sets the sender's phone number.
        /// </summary>
        public string SenderPhone
        {
            get { return senderPhone; }
            set { senderPhone = value; }
        }
        /// <summary>
        /// Gets or sets the cloud password.
        /// </summary>
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        /// <summary>
        /// Gets or sets the recipient's phone number.
        /// </summary>
        public string RecipientPhone
        {
            get { return recipientPhone; }
            set { recipientPhone = value; }
        }

        /// <summary>
        /// Gets or sets the authentication code
        /// </summary>
        public string CodeToAuthenticate
        {
            get { return codeToAuthenticate; }
            set { codeToAuthenticate = value; }
        }

        public bool Authorized
        {
            get { return authorized; }
        }

        public bool Connected
        {
            get { return connected; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramMessenger"/> class.
        /// </summary>
        public TelegramMessenger()
        {
            password = "";
            recipientPhone = "";
            hash = "";
            codeToAuthenticate = "";
            recipient = new TLUser();
            sender = new TLUser();
            client = new TelegramClient(API_ID, API_HASH);
            authorized = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramMessenger"/> class with specified parameters.
        /// </summary>
        /// <param name="senderPhone"> Sender's phone number</param>
        /// <param name="recipientPhone">Recipient's phone number</param>
        public TelegramMessenger(string senderPhone, string recipientPhone)
        {
            this.senderPhone = senderPhone;
            this.recipientPhone = recipientPhone;
            authorized = false;
            hash = "";
            codeToAuthenticate = "";
            recipient = new TLUser();
            sender = new TLUser();
            client = new TelegramClient(API_ID, API_HASH);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramMessenger"/> class with specified parameters.
        /// </summary>
        /// <param name="senderPhone"> Sender's phone number</param>
        /// <param name="password">Sender's cloud password</param>
        /// <param name="recipientPhone">Recipient's phone number</param>
        public TelegramMessenger(string senderPhone, string password, string recipientPhone)
        {
            this.senderPhone = senderPhone;
            this.password = password;
            this.recipientPhone = recipientPhone;
            authorized = false;
            hash = "";
            codeToAuthenticate = "";
            recipient = new TLUser();
            sender = new TLUser();
            client = new TelegramClient(API_ID, API_HASH);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Authenticates the user on the server.
        /// </summary>
        private async Task MakeAuthAsync()
        {
            if (!string.IsNullOrEmpty(CodeToAuthenticate))
            {
                try
                {
                    sender = await client.MakeAuthAsync(senderPhone, hash, codeToAuthenticate);
                    authorized = client.IsUserAuthorized();
                }
                catch (CloudPasswordNeededException ex)
                {
                    TLPassword tlPassword = await client.GetPasswordSetting();
                    if (!string.IsNullOrEmpty(password))
                    {
                        sender = await client.MakeAuthWithPasswordAsync(tlPassword, password);
                        authorized = client.IsUserAuthorized();
                    }
                    else
                        throw new Exception("Cloud password wasn't set! Fill the Password property, please!");
                }
                catch (InvalidPhoneCodeException ex)
                {
                    throw new Exception("CodeToAuthenticate is wrong", ex);
                }
            }
            else
                throw new Exception("CodeToAuthenticate didn't set!");
        }

        #endregion // Private Methods

        #region Public Methods

        public async Task<bool> ConnectAsync(bool reconnect = false)
        {
            await Task.Delay(500).ConfigureAwait(false);

            await client.ConnectAsync(reconnect);
            connected = client.IsConnected;
            if (connected)
                authorized = client.IsUserAuthorized();
            return connected;
        }

        public bool Connect(bool reconnect = false)
        {
            bool connected = false;
            try
            {
                ConnectAsync(reconnect).Wait();
                connected = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return connected;
        }
        /// <summary>
        /// Requests code from Telegram server
        /// </summary>
        /// <returns></returns>
        public async Task RequestCodeAsync()
        {
            if (!string.IsNullOrEmpty(senderPhone))
                try
                {
                    hash = await client.SendCodeRequestAsync(senderPhone);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            else
                throw new Exception("SenderPhone property is null or empty! Fill it, please!");
        }

        /// <summary>
        /// Requests code from Telegram server
        /// </summary>
        /// <returns></returns>
        public async Task RequestCodeAsync(string senderPhone)
        {
            this.senderPhone = senderPhone;
            await RequestCodeAsync();
        }

        /// <summary>
        /// Requests code from Telegram server
        /// </summary>
        /// <returns></returns>
        public void RequestCode()
        {
            RequestCodeAsync().Wait();
        }

        /// <summary>
        /// Requests code from Telegram server
        /// </summary>
        /// <returns></returns>
        public void RequestCode(string senderPhone)
        {
            this.senderPhone = senderPhone;
            RequestCodeAsync().Wait();
        }

        /// <summary>
        /// Authorizes the user.
        /// </summary>
        /// <param name="codeToAuthenticate">Code from SMS or Telegram</param>
        /// <returns>True if user has been successfully authorized.</returns>
        public async Task<bool> AuthorizeAsync(string codeToAuthenticate)
        {
            this.codeToAuthenticate = codeToAuthenticate;
            await MakeAuthAsync();
            return authorized;
        }

        /// <summary>
        /// Authorizes the user.
        /// </summary>
        /// <param name="codeToAuthenticate">Code from SMS or Telegram</param>
        /// <returns>True if user has been successfully authorized.</returns>
        public bool Authorize(string codeToAuthenticate)
        {
            AuthorizeAsync(codeToAuthenticate).Wait();
            return authorized;
        }

        /// <summary>
        /// Authorizes the user.
        /// </summary>
        /// <param name="codeToAuthenticate">Code from SMS or Telegram</param>
        /// <param name="password">Two-Step Verification password</param>
        /// <returns>True if user has been successfully authorized.</returns>
        public async Task<bool> AuthorizeAsync(string codeToAuthenticate, string password)
        {
            this.codeToAuthenticate = codeToAuthenticate;
            this.password = password;
            return await AuthorizeAsync(codeToAuthenticate);
        }

        /// <summary>
        /// Authorizes the user.
        /// </summary>
        /// <param name="codeToAuthenticate">Code from SMS or Telegram</param>
        /// <param name="password">Two-Step Verification password</param>
        /// <returns>True if user has been successfully authorized.</returns>
        public bool Authorize(string codeToAuthenticate, string password)
        {
            AuthorizeAsync(codeToAuthenticate, password).Wait();
            return authorized;
        }

        /// <summary>
        /// Sends the report.
        /// </summary>
        /// <param name="report">The report template that should be sent.</param>
        /// <param name="export">The export filter that should export template before.</param>
        /// <returns>True if report has been successfully sent.</returns>
        public async Task<bool> SendReportAsync(Report report, ExportBase export)
        {
            bool result = false;
            await Task.Delay(500).ConfigureAwait(false);
            if (connected && authorized)
            {
                if (string.IsNullOrEmpty(recipientPhone))
                    throw new Exception("Recipient phone is empty! You should assign it!");
                try
                {
                    TLContacts contacts = await client.GetContactsAsync();
                    recipient = contacts.users.lists
                    .Where(x => x.GetType() == typeof(TLUser))
                    .Cast<TLUser>()
                    .FirstOrDefault(x => x.phone == recipientPhone);
                    if (recipient == null)
                        throw new Exception("There is no such user in your contacts!");
                    if (report != null)
                        using (MemoryStream ms = PrepareToSave(report, export))
                        {
                            var uploadedFile = await client.
                                UploadFile(Filename, new StreamReader(ms, Encoding.ASCII, true));

                            TLDocumentAttributeFilename filenameAttr = new TLDocumentAttributeFilename();
                            filenameAttr.file_name = Filename;
                            TLVector<TLAbsDocumentAttribute> attrs = new TLVector<TLAbsDocumentAttribute>();
                            attrs.lists.Add(filenameAttr);

                            await client.SendUploadedDocument(new TLInputPeerUser() { user_id = recipient.id },
                           uploadedFile, "", "", attrs);
                        }
                    else
                        throw new Exception("Assign report!");
                    result = true;
                }
                catch (Exception ex)
                {
                    throw new Exception("Something going wrong!", ex);
                }
                finally
                {
                    Dispose();
                }
            }
            return result;
        }

        /// <inheritdoc/>
        public override bool SendReport(Report report, ExportBase export)
        {
            bool sent = false;
            try
            {
                SendReportAsync(report, export).Wait();
                sent = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return sent;
        }

        /// <summary>
        /// Releases all the resources used by the XMPP messenger.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                client.Dispose();
                disposed = true;
            }
        }

        /// <summary>
        /// Invokes the object's editor.
        /// </summary>
        /// <returns><b>true</b> if object was edited succesfully.</returns>
        public bool InvokeForm(Report report)
        {
            this.Connect();
            if (authorized)
                using (TelegramMessengerForm form = new TelegramMessengerForm(this, report))
                {
                    return form.ShowDialog() == DialogResult.OK;
                }
            else
                using (TelegramMessengerLogInForm form = new TelegramMessengerLogInForm(report))
                {
                    return form.ShowDialog() == DialogResult.OK;
                }
        }
        #endregion // Public Methods
    }
}
