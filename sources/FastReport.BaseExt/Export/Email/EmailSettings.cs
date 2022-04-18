using System.ComponentModel;

namespace FastReport.Export.Email
{
  /// <summary>
  /// Contains the email account settings such as host, email address, name.
  /// </summary>
  /// <remarks>
  /// You have to set up at least the <see cref="Address"/> and <see cref="Host"/> properties. If your
  /// host requires authentication, provide the <see cref="UserName"/> and <see cref="Password"/>
  /// properties as well.
  /// <para/>Set <see cref="UseMAPI"/> property to <b>true</b> if you want to use default email client
  /// such as Outlook to send an email. In this case, all other properties will be ignored.
  /// </remarks>
  [TypeConverter(typeof(FastReport.TypeConverters.FRExpandableObjectConverter))]
  public class EmailSettings
  {
    #region Fields
    private string address;
    private string name;
    private string messageTemplate;
    private string host;
    private int port;
    private string userName;
    private string password;
    private bool enableSSL;
    private bool allowUI;
    private bool useMAPI;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the sender's email address.
    /// </summary>
    /// <remarks>
    /// This property contains your email address (for example, "john@site.com").
    /// </remarks>
    public string Address
    {
      get { return address; }
      set { address = value; }
    }

    /// <summary>
    /// Gets or sets the sender's name.
    /// </summary>
    /// <remarks>
    /// This property contains your name (for example, "John Smith").
    /// </remarks>
    public string Name
    {
      get { return name; }
      set { name = value; }
    }

    /// <summary>
    /// Gets or sets the template that will be used to create a new message.
    /// </summary>
    public string MessageTemplate
    {
      get { return messageTemplate; }
      set { messageTemplate = value; }
    }

    /// <summary>
    /// Gets or sets the SMTP host name or IP address.
    /// </summary>
    public string Host
    {
      get { return host; }
      set { host = value; }
    }

    /// <summary>
    /// Gets or sets the SMTP port.
    /// </summary>
    /// <remarks>
    /// The default value for this property is <b>25</b>.
    /// </remarks>
    [DefaultValue(25)]
    public int Port
    {
      get { return port; }
      set { port = value; }
    }

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    /// <remarks>
    /// Specify the <see cref="UserName"/> and <see cref="Password"/> properties if your host requires
    /// authentication.
    /// </remarks>
    public string UserName
    {
      get { return userName; }
      set { userName = value; }
    }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <remarks>
    /// Specify the <see cref="UserName"/> and <see cref="Password"/> properties if your host requires
    /// authentication.
    /// </remarks>
    public string Password
    {
      get { return password; }
      set { password = value; }
    }

    /// <summary>
    /// Gets or sets a value that determines whether to enable the SSL protocol.
    /// </summary>
    [DefaultValue(false)]
    public bool EnableSSL
    {
      get { return enableSSL; }
      set { enableSSL = value; }
    }
    
    /// <summary>
    /// Gets or sets a value that determines whether the account setting page 
    /// in the "Send Email" window is enabled.
    /// </summary>
    [DefaultValue(true)]
    public bool AllowUI
    {
      get { return allowUI; }
      set { allowUI = value; }
    }
    
    /// <summary>
    /// Gets or sets a value that determines whether to use MAPI instead of SMTP when sending an email.
    /// </summary>
    [DefaultValue(false)]
    public bool UseMAPI
    {
      get { return useMAPI; }
      set { useMAPI = value; }
    }
    #endregion
    
    #region Public Methods
    /// <summary>
    /// Copies email settings from another source.
    /// </summary>
    /// <param name="source">Source to copy settings from.</param>
    public void Assign(EmailSettings source)
    {
      Address = source.Address;
      Name = source.Name;
      MessageTemplate = source.MessageTemplate;
      Host = source.Host;
      Port = source.Port;
      UserName = source.UserName;
      Password = source.Password;
      EnableSSL = source.EnableSSL;
      AllowUI = source.AllowUI;
      UseMAPI = source.UseMAPI;
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailSettings"/> class with default settings.
    /// </summary>
    public EmailSettings()
    {
      address = "";
      name = "";
      messageTemplate = "";
      host = "";
      port = 25;
      userName = "";
      password = "";
      allowUI = true;
    }
  }
}
