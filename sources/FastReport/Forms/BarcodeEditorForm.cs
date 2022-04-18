using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FastReport.Barcode.QRCode;
using System.Reflection;
using FastReport.Data;
using FastReport.Code;
using FastReport.Controls;
using FastReport.Utils;
using FastReport.Barcode;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Forms
{
    /// <summary>
    /// Form for barcode editor
    /// </summary>
    public partial class BarcodeEditorForm : ScaledSupportingForm
    {
        /// <summary>
        /// Generated text for barcode object
        /// </summary>
        public string result;

        private Report report;
        private static List<string> expandedNodes;
        private string brackets;
        private TextBox prevFocus;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeEditorForm"/> class.
        /// </summary>
        /// <param name="data">Text data for parsing</param>
        /// <param name="report">Report object for nodes</param>
        /// <param name="Brackets">Brackets symbols</param>
        /// <param name="isRichBarcode">Editor for rich barcode?</param>
        public BarcodeEditorForm(string data, Report report, string Brackets, bool isRichBarcode)
        {
            InitializeComponent();
            Localize();
            okButton.DialogResult = DialogResult.None;
            this.FormClosed += QREdit_FormClosed;
            this.report = report;
            qrWifiEncryption.SelectedIndex = 0;
            qrTabs.Appearance = TabAppearance.FlatButtons;
            qrTabs.ItemSize = new Size(0, 1);
            qrTabs.SizeMode = TabSizeMode.Fixed;
            tvData.CreateNodes(report.Dictionary);
            brackets = Brackets;
            tvData.NodeMouseDoubleClick += tvData_NodeMouseDoubleClick;
            tvData.ItemDrag += tvData_ItemDrag;

            if (expandedNodes != null)
                tvData.ExpandedNodes = expandedNodes;

            foreach (Control control in GetAllControls(Controls))
            {
                TextBox tb = control as TextBox;

                if (tb != null)
                {
                    tb.GotFocus += prevFocus_GotFocus;
                    tb.DragOver += prevFocus_DragOver;
                    tb.DragDrop += prevFocus_DragDrop;
                }
            }

            if (isRichBarcode)
            {
                parse(data);
            }
            else
            {
                qrText.Text = data;
                qrSelectType.SelectedIndex = 0;

                qrSelectType.Visible = false;
                qrTabs.Location = new Point(4, 4);
                qrText.Height = panel1.Height;
                qrTabs.Height = qrText.Height + DpiHelper.ConvertUnits(50, true);
            }
            Scale();
        }

        private void Localize()
        {
            MyRes res = new MyRes("Forms,BarcodeEditor");
            Text = res.Get("");

            okButton.Text = res.Get("Save");
            cancelButton.Text = res.Get("Cancel");

            qrSelectType.Items[0] = res.Get("Text");
            qrSelectType.Items[1] = res.Get("vCard");
            qrSelectType.Items[2] = res.Get("URI");
            qrSelectType.Items[3] = res.Get("EmailAddress");
            qrSelectType.Items[4] = res.Get("EmailMessage");
            qrSelectType.Items[5] = res.Get("Geolocation");
            qrSelectType.Items[6] = res.Get("SMS");
            qrSelectType.Items[7] = res.Get("Call");
            qrSelectType.Items[8] = res.Get("Event");
            qrSelectType.Items[9] = res.Get("Wifi");

            label16.Text = res.Get("FirstName");
            label17.Text = res.Get("LastName");
            label18.Text = res.Get("Title");
            label20.Text = res.Get("Company");
            label21.Text = res.Get("Website");
            label25.Text = res.Get("EmailPersonal");
            label26.Text = res.Get("EmailBusiness");
            label19.Text = res.Get("PhoneMobile");
            label27.Text = res.Get("PhonePersonal");
            label28.Text = res.Get("PhoneBusiness");
            label31.Text = res.Get("Street");
            label22.Text = res.Get("ZipCode");
            label23.Text = res.Get("City");
            label24.Text = res.Get("Country");
            label30.Text = res.Get("URI");
            label29.Text = res.Get("Email");
            label1.Text = res.Get("Email");
            label2.Text = res.Get("Subject");
            label3.Text = res.Get("Text");
            label5.Text = res.Get("Latitude");
            label4.Text = res.Get("Longitude");
            label6.Text = res.Get("Height");
            label7.Text = res.Get("PhoneNumber");
            label8.Text = res.Get("Text");
            label9.Text = res.Get("PhoneNumber");
            label10.Text = res.Get("Description");
            label11.Text = res.Get("From");
            label12.Text = res.Get("To");
            label13.Text = res.Get("Encryption");
            label14.Text = res.Get("NetworkName");
            label15.Text = res.Get("Password");
            qrWifiHidden.Text = res.Get("WifiHidden");
            label36.Text = res.Get("ZipCode");
            label37.Text = res.Get("City");
            label38.Text = res.Get("Country");
            label39.Text = res.Get("Street");
            label47.Text = res.Get("ZipCode");
            label48.Text = res.Get("City");
            label49.Text = res.Get("Country");
            label50.Text = res.Get("Street");
            label42.Text = res.Get("Text");

            res = new MyRes("Forms,BarcodeEditor,Swiss");
            label35.Text = res.Get("Name");
            label46.Text = res.Get("Name");
            label32.Text = res.Get("Iban");
            label33.Text = res.Get("IbanType");
            qrSwissCreditorBox.Text = res.Get("Creditor");
            label40.Text = res.Get("HouseNumber");
            qrSwissReferenceBox.Text = res.Get("Reference");
            label43.Text = res.Get("Type");
            label44.Text = res.Get("TextType");
            qrSwissDebitorBox.Text = res.Get("Debitor");
            label34.Text = res.Get("HouseNumber");
            qrSwissAddInfoBox.Text = res.Get("AdditionalInformation");
            label53.Text = res.Get("UnstructuredMessage");
            label54.Text = res.Get("BillInformation");
            label55.Text = res.Get("Currency");
            label56.Text = res.Get("AlternativeProcedure1");
            label57.Text = res.Get("AlternativeProcedure2");
            label58.Text = res.Get("Amount");
            
            res = new MyRes("Forms,BarcodeEditor,SberbankQr");
            sberQrName.Text = res.Get("Name");
            label41.Text = res.Get("PersonalAcc");
            label59.Text = res.Get("CorrespAcc");
            label52.Text = res.Get("BIC");   
            label45.Text = res.Get("BankName");

            label66.Text = res.Get("KPP");
            label64.Text = res.Get("PayerINN");
            label69.Text = res.Get("TaxPeriod");
            label72.Text = res.Get("DocNo");
            label65.Text = res.Get("Purpose");
            label68.Text = res.Get("OKTMO");
            label63.Text = res.Get("PayeeINN");
            label62.Text = res.Get("Sum");
            label74.Text = res.Get("DocDate");
            label70.Text = res.Get("PayReason");
            label73.Text = res.Get("TaxPaytKind");
            label67.Text = res.Get("CBC");
            label71.Text = res.Get("DrawerStatus");
            label104.Text = res.Get("TechCode");
            label103.Text = res.Get("UIN");
            label102.Text = res.Get("RegType");
            label101.Text = res.Get("ExecId");
            label100.Text = res.Get("RuleId");
            label99.Text = res.Get("AddAmount");
            label98.Text = res.Get("SpecFio");
            label97.Text = res.Get("ClassNum");
            label96.Text = res.Get("InstNum");
            label95.Text = res.Get("QuittDate");
            label94.Text = res.Get("QuittId");
            label91.Text = res.Get("CounterVal");
            label92.Text = res.Get("ServiceName");
            label93.Text = res.Get("CounterId");
            label90.Text = res.Get("Category");
            label89.Text = res.Get("PaymTerm");
            label88.Text = res.Get("BirthDate");
            label87.Text = res.Get("ChildFio");
            label86.Text = res.Get("PayerIdType");
            label85.Text = res.Get("PayerIdNum");
            label83.Text = res.Get("Flat");
            label84.Text = res.Get("Phone");
            label81.Text = res.Get("PersAcc");
            label80.Text = res.Get("PensAcc");
            label79.Text = res.Get("DocIdx");
            label78.Text = res.Get("Contract");
            label77.Text = res.Get("PersonalAccount");
            label76.Text = res.Get("PayerAdress");
            label75.Text = res.Get("MiddleName");
            label61.Text = res.Get("FirstName");
            label60.Text = res.Get("LastName");
            tabPage2.Text = res.Get("ObligatoryProps");
            tabPage3.Text = res.Get("AdditionalProps");
            tabPage4.Text = res.Get("AnotherProps");
            QuittDateCheckBox.Text = res.Get("NotEnterDate");
            BirthDateCheckBox.Text = res.Get("NotEnterDate");
            PaymTermCheckBox.Text = res.Get("NotEnterDate");
            DocDateCheckBox.Text = res.Get("NotEnterDate");
        }

        private void QREdit_FormClosed(object sender, FormClosedEventArgs e)
        {
            expandedNodes = tvData.ExpandedNodes;
        }

        private void tvData_ItemDrag(object sender, ItemDragEventArgs e)
        {
            tvData.SelectedNode = e.Item as TreeNode;
            if (tvData.SelectedItem != "")
                tvData.DoDragDrop(e.Item, DragDropEffects.Move);
            else
                tvData.DoDragDrop(e.Item, DragDropEffects.None);
        }

        private void prevFocus_DragOver(object sender, DragEventArgs e)
        {
            TextBox tb = sender as TextBox;

            int index = tb.GetCharIndexFromPosition(tb.PointToClient(new Point(e.X, e.Y)));
            if (index == tb.Text.Length - 1)
                index++;
            tb.Focus();
            tb.Select(index, 0);
            e.Effect = e.AllowedEffect;
        }

        private void prevFocus_DragDrop(object sender, DragEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.SelectedText = GetTextWithBrackets();
            tb.Focus();
        }

        List<Control> GetAllControls(Control.ControlCollection collection)
        {
            List<Control> result = new List<Control>();

            foreach (Control control in collection)
            {
                result.Add(control);
                result.AddRange(GetAllControls(control.Controls));
            }

            return result;
        }

        private void prevFocus_GotFocus(object sender, EventArgs e)
        {
            prevFocus = sender as TextBox;
        }

        private void tvData_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (tvData.SelectedItem != "" && prevFocus != null)
            {
                prevFocus.SelectedText = GetTextWithBrackets();
                prevFocus.Focus();
            }
        }

        private string GetTextWithBrackets()
        {
            string text = tvData.SelectedItem;
            string[] _brackets = brackets.Split(new char[] { ',' });
            // this check is needed if Brackets property is not "[,]"
            if (InsideBrackets(prevFocus.SelectionStart))
            {
                if (tvData.SelectedItemType == DataTreeSelectedItemType.Function ||
                  tvData.SelectedItemType == DataTreeSelectedItemType.DialogControl)
                    return text;
                return "[" + text + "]";
            }
            return _brackets[0] + text + _brackets[1];
        }

        private bool InsideBrackets(int pos)
        {
            string[] _brackets = brackets.Split(new char[] { ',' });
            FindTextArgs args = new FindTextArgs();
            args.Text = new FastString(prevFocus.Text);
            args.OpenBracket = _brackets[0];
            args.CloseBracket = _brackets[1];
            args.StartIndex = pos;
            return CodeUtils.IndexInsideBrackets(args);
        }

        private void parse(string data)
        {
            QRData qr = null;

            try
            {
                qr = QRData.Parse(data);
            }
            catch
            {
                try
                {
                    qr = new QRText(data);
                }
                catch
                {
                    MessageBox.Show("Can't parse", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }
            }

            if (qr is QRText)
            {
                qrText.Text = qr.data;
                qrSelectType.SelectedIndex = 0;
            }
            else if (qr is QRvCard)
            {
                QRvCard _qr = qr as QRvCard;
                qrVcFN.Text = _qr.firstName;
                qrVcLN.Text = _qr.lastName;
                qrVcTitle.Text = _qr.title;
                qrVcCompany.Text = _qr.org;
                qrVcWebsite.Text = _qr.url;
                qrVcEmailHome.Text = _qr.email_home_internet;
                qrVcEmailWork.Text = _qr.email_work_internet;
                qrVcPhone.Text = _qr.tel_cell;
                qrVcPhoneHome.Text = _qr.tel_home_voice;
                qrVcPhoneWork.Text = _qr.tel_work_voice;
                qrVcStreet.Text = _qr.street;
                qrVcZip.Text = _qr.zipCode;
                qrVcCity.Text = _qr.city;
                qrVcCountry.Text = _qr.country;
                qrSelectType.SelectedIndex = 1;
            }
            else if (qr is QRURI)
            {
                qrURI.Text = qr.data;
                qrSelectType.SelectedIndex = 2;
            }
            else if (qr is QREmailAddress)
            {
                qrEmail.Text = qr.data;
                qrSelectType.SelectedIndex = 3;
            }
            else if (qr is QREmailMessage)
            {
                QREmailMessage _qr = qr as QREmailMessage;
                qrEmailTo.Text = _qr.msg_to;
                qrEmailSub.Text = _qr.msg_sub;
                qrEmailText.Text = _qr.msg_body;
                qrSelectType.SelectedIndex = 4;
            }
            else if (qr is QRGeo)
            {
                QRGeo _qr = qr as QRGeo;
                qrGeoLatitude.Text = _qr.latitude;
                qrGeoLongitude.Text = _qr.longitude;
                qrGeoMeters.Text = _qr.meters;
                qrSelectType.SelectedIndex = 5;
            }
            else if (qr is QRSMS)
            {
                QRSMS _qr = qr as QRSMS;
                qrSMSTo.Text = _qr.sms_to;
                qrSMSText.Text = _qr.sms_text;
                qrSelectType.SelectedIndex = 6;
            }
            else if (qr is QRCall)
            {
                QRCall _qr = qr as QRCall;
                qrCall.Text = _qr.tel;
                qrSelectType.SelectedIndex = 7;
            }
            else if (qr is QREvent)
            {
                QREvent _qr = qr as QREvent;
                qrEventDesc.Text = _qr.summary;
                qrEventFrom.Value = _qr.dtStart;
                qrEventTo.Value = _qr.dtEnd;
                qrSelectType.SelectedIndex = 8;
            }
            else if (qr is QRWifi)
            {
                QRWifi _qr = qr as QRWifi;
                qrWifiEncryption.Text = _qr.encryption;
                qrWifiName.Text = _qr.networkName;
                qrWifiPass.Text = _qr.password;
                qrWifiHidden.Checked = _qr.hidden;
                qrSelectType.SelectedIndex = 9;
            }
            else if(qr is QRSberBank)
            {
                QRSberBank _qr = qr as QRSberBank;
                NameTextBox.Text = _qr.Name;
                PersonalAccTextBox.Text = _qr.PersonalAcc;
                BankNameTextBox.Text = _qr.BankName;
                BICTextBox.Text = _qr.BIC;
                CorrespAccTextBox.Text = _qr.CorrespAcc;
                sumTextBox.Text = _qr.Sum;
                PaymentPurpose.Text = _qr.Purpose;
                PayerInnTextBox.Text = _qr.PayerINN;
                ReceiverIINTextBox.Text = _qr.PayeeINNB;
                DrawerStatusTextBox.Text = _qr.DrawerStatus;
                ReceiverKPPTextBox.Text = _qr.KPP;
                OKTMOTextBox.Text = _qr.OKTMO;
                PayReasonTextBox.Text = _qr.PaytReason;
                TaxPeriodTextBox.Text = _qr.TaxPeriod;
                DocumentNumberTextBox.Text = _qr.DocNo;
                CBCTextBox.Text = _qr.CBC;
                if (_qr.DocDate != DateTime.MinValue)
                {
                    DocumentDatePicker.Value = _qr.DocDate;
                    DocDateCheckBox.Checked = false;
                }
                else
                {
                    DocumentDatePicker.Enabled = false;
                    DocDateCheckBox.Checked = true;
                }
                TaxPayKindTextBox.Text = _qr.TaxPaytKind;
                LastNameTextBox.Text = _qr.LastName;
                FirstNameTextBox.Text = _qr.FirstName;
                MiddleNameTextBox.Text = _qr.MiddleName;
                PayerAdressTextBox.Text = _qr.PayerAddress;
                PersonalAccoutTextBox.Text = _qr.PersonalAccount;
                DocIdxTextBox.Text = _qr.DocIdx;
                PensAccTextBox.Text = _qr.PensAcc;
                ContractTextBox.Text = _qr.Contract;
                PersAccTextBox.Text = _qr.PersAccp;
                FlatTextBox.Text = _qr.Flat;
                PhoneTextBox.Text = _qr.Phone;
                PayerIdTypeTextBox.Text = _qr.PayerIdType;
                PayerIdNumTextBox.Text = _qr.PayerIdNum;
                ChildFioTextBox.Text = _qr.ChildFio;
                if (_qr.BirthDate != DateTime.MinValue)
                {
                    BirthDatePicker.Value = _qr.BirthDate;
                    BirthDateCheckBox.Checked = false;
                }
                else
                {
                    BirthDateCheckBox.Checked = true;
                    BirthDatePicker.Enabled = false;
                }
                if (_qr.PaymTerm != DateTime.MinValue)
                {
                    PaymTermPicker.Value = _qr.PaymTerm;
                    PaymTermCheckBox.Checked = false;
                }
                else
                {
                    PaymTermPicker.Enabled = false;
                    PaymTermCheckBox.Checked = true;
                }
                CategoryTextBox.Text = _qr.Category;
                ServiceNameTextBox.Text = _qr.ServiceName;
                CounterIdValTextBox.Text = _qr.CounterId;
                CounterValTextBox.Text = _qr.CounterVal;
                QuittIdTextBox.Text = _qr.QuittId;
                if (_qr.QuittDate != DateTime.MinValue)
                {
                    QuittDateDatePicker.Value = _qr.QuittDate;
                    QuittDateCheckBox.Checked = false;
                }
                else
                {
                    QuittDateDatePicker.Enabled = false;
                    QuittDateCheckBox.Checked = true;
                }
                InstNumTextBox.Text = _qr.InstNum;
                ClassNumTextBox.Text = _qr.ClassNum;
                SpecFioTextBox.Text = _qr.SpecFio;
                AddAmountTextBox.Text = _qr.AddAmount;
                RuleIdTextBox.Text = _qr.RuleId;
                ExecId.Text = _qr.ExecId;
                RegType.Text = _qr.RegType;
                UINTextBox.Text = _qr.UIN;
                TechCodeTextBox.Text = _qr.TechCode;
                qrSelectType.SelectedIndex = 11;
            }
            else if (qr is QRSwiss)
            {
                QRSwiss _qr = qr as QRSwiss;

                qrSwissIban.Text = _qr._Iban._Iban;
                if (_qr._Iban.TypeIban == Iban.IbanType.Iban)
                    qrSwissIbanType.SelectedIndex = 0;
                else
                    qrSwissIbanType.SelectedIndex = 1;
                qrSwissCreditorName.Text = _qr.Creditor.Name;
                qrSwissCreditorCity.Text = _qr.Creditor.City;
                qrSwissCreditorCountry.Text = _qr.Creditor.Country;
                qrSwissCreditorHNumber.Text = _qr.Creditor.HouseNumberOrAddressline;
                qrSwissCreditorStreet.Text = _qr.Creditor.StreetOrAddressline;
                qrSwissCreditorZipCode.Text = _qr.Creditor.ZipCode;
                qrSwissRefText.Text = _qr._Reference.ReferenceText;
                switch (_qr._Reference.RefType)
                {
                    case Reference.ReferenceType.QRR:
                        qrSwissRefType.SelectedIndex = 0;
                        break;
                    case Reference.ReferenceType.SCOR:
                        qrSwissRefType.SelectedIndex = 1;
                        break;
                    case Reference.ReferenceType.NON:
                        qrSwissRefType.SelectedIndex = 2;
                        break;
                }
                switch (_qr._Reference._ReferenceTextType)
                {
                    case Reference.ReferenceTextType.QrReference:
                        qrSwissRefTextType.SelectedIndex = 0;
                        break;
                    case Reference.ReferenceTextType.CreditorReferenceIso11649:
                        qrSwissRefTextType.SelectedIndex = 1;
                        break;
                }
                if (_qr.Debitor != null)
                {
                    qrSwissDebitorName.Text = _qr.Debitor.Name.ToString();
                    qrSwissDebitorCity.Text = _qr.Debitor.City.ToString();
                    qrSwissDebitorCountry.Text = _qr.Debitor.Country.ToString();
                    qrSwissDebitorHNumber.Text = _qr.Debitor.HouseNumberOrAddressline.ToString();
                    qrSwissDebitorStreet.Text = _qr.Debitor.StreetOrAddressline.ToString();
                    qrSwissDebitorZipCode.Text = _qr.Debitor.ZipCode.ToString();
                }

                qrSwissUnstructMessage.Text = _qr._AdditionalInformation.UnstructureMessage != null ? _qr._AdditionalInformation.UnstructureMessage : "";
                qrSwissBillInfo.Text = _qr._AdditionalInformation.BillInformation != null ? _qr._AdditionalInformation.BillInformation : "";

                qrSwissAlt1.Text = _qr.AlternativeProcedure1 != null ? _qr.AlternativeProcedure1 : "";
                qrSwissAlt2.Text = _qr.AlternativeProcedure2 != null ? _qr.AlternativeProcedure2 : "";
                switch (_qr._Currency)
                {
                    case Currency.EUR:
                        qrSwissCurrency.SelectedIndex = 0;
                        break;
                    case Currency.CHF:
                        qrSwissCurrency.SelectedIndex = 1;
                        break;
                }
                qrSwissAmount.Text = _qr.Amount.ToString();

                qrSelectType.SelectedIndex = 10;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (qrTabs.SelectedTab == qrTypeText)
                {
                    QRText qr = new QRText();
                    qr.data = qrText.Text;
                    result = qr.Pack();
                }
                else if (qrTabs.SelectedTab == qrTypeVcard)
                {
                    QRvCard qr = new QRvCard();
                    qr.firstName = qrVcFN.Text;
                    qr.lastName = qrVcLN.Text;
                    qr.title = qrVcTitle.Text;
                    qr.org = qrVcCompany.Text;
                    qr.url = qrVcWebsite.Text;
                    qr.email_home_internet = qrVcEmailHome.Text;
                    qr.email_work_internet = qrVcEmailWork.Text;
                    qr.tel_cell = qrVcPhone.Text;
                    qr.tel_home_voice = qrVcPhoneHome.Text;
                    qr.tel_work_voice = qrVcPhoneWork.Text;
                    qr.street = qrVcStreet.Text;
                    qr.zipCode = qrVcZip.Text;
                    qr.city = qrVcCity.Text;
                    qr.country = qrVcCountry.Text;
                    result = qr.Pack();
                }
                else if (qrTabs.SelectedTab == qrTypeURI)
                {
                    QRURI qr = new QRURI();
                    qr.data = qrURI.Text;
                    result = qr.Pack();
                }
                else if (qrTabs.SelectedTab == qrTypeEmailAddress)
                {
                    QREmailAddress qr = new QREmailAddress();
                    qr.data = qrEmail.Text;
                    result = qr.Pack();
                }
                else if (qrTabs.SelectedTab == qrTypeEmailMessage)
                {
                    QREmailMessage qr = new QREmailMessage();
                    qr.msg_to = qrEmailTo.Text;
                    qr.msg_sub = qrEmailSub.Text;
                    qr.msg_body = qrEmailText.Text;
                    result = qr.Pack();
                }
                else if (qrTabs.SelectedTab == qrTypeGeo)
                {
                    QRGeo qr = new QRGeo();
                    qr.latitude = qrGeoLatitude.Text;
                    qr.longitude = qrGeoLongitude.Text;
                    qr.meters = qrGeoMeters.Text;
                    result = qr.Pack();
                }
                else if (qrTabs.SelectedTab == qrTypeSMS)
                {
                    QRSMS qr = new QRSMS();
                    qr.sms_to = qrSMSTo.Text;
                    qr.sms_text = qrSMSText.Text;
                    result = qr.Pack();
                }
                else if (qrTabs.SelectedTab == qrTypeCall)
                {
                    QRCall qr = new QRCall();
                    qr.tel = qrCall.Text;
                    result = qr.Pack();
                }
                else if (qrTabs.SelectedTab == qrTypeEvent)
                {
                    QREvent qr = new QREvent();
                    qr.summary = qrEventDesc.Text;
                    qr.dtStart = qrEventFrom.Value;
                    qr.dtEnd = qrEventTo.Value;
                    result = qr.Pack();
                }
                else if(qrTabs.SelectedTab == qrTypeSberbank){
                    if (String.IsNullOrWhiteSpace(PersonalAccTextBox.Text) || String.IsNullOrWhiteSpace(BankNameTextBox.Text)
                        || String.IsNullOrWhiteSpace(NameTextBox.Text) || String.IsNullOrWhiteSpace(BICTextBox.Text)
                        || String.IsNullOrWhiteSpace(CorrespAccTextBox.Text))
                    {
                        throw new Exception("Заполнены не все обязательные реквизиты.");
                    }
                        QRSberBank qe = new QRSberBank();
                   
                    qe.PersonalAcc = PersonalAccTextBox.Text;
                    qe.BankName = BankNameTextBox.Text;
                    qe.Name = NameTextBox.Text;
                    qe.BIC = BICTextBox.Text;
                    qe.CorrespAcc = CorrespAccTextBox.Text;
                    qe.Sum = sumTextBox.Text;
                    qe.PayeeINNB = ReceiverIINTextBox.Text;
                    qe.PayerINN = PayerInnTextBox.Text;
                    qe.CBC = CBCTextBox.Text;
                    qe.Purpose = PaymentPurpose.Text;
                    qe.DrawerStatus = DrawerStatusTextBox.Text;
                    qe.KPP = ReceiverKPPTextBox.Text;
                    qe.OKTMO = OKTMOTextBox.Text;
                    qe.PaytReason = PayReasonTextBox.Text;
                    qe.DocNo = DocumentNumberTextBox.Text;
                    qe.TaxPaytKind = TaxPayKindTextBox.Text;
                    qe.TaxPeriod = TaxPeriodTextBox.Text;
                    qe.DocDate = DocDateCheckBox.Checked ? DateTime.MinValue : DocumentDatePicker.Value;
                    qe.LastName = LastNameTextBox.Text;
                    qe.FirstName = FirstNameTextBox.Text;
                    qe.MiddleName = MiddleNameTextBox.Text;
                    qe.PayerAddress = PayerAdressTextBox.Text;
                    qe.PersonalAccount = PersonalAccoutTextBox.Text;
                    qe.DocIdx = DocIdxTextBox.Text;
                    qe.PensAcc = PensAccTextBox.Text;
                    qe.Contract = ContractTextBox.Text;
                    qe.PersAccp = PersAccTextBox.Text;
                    qe.Flat = FlatTextBox.Text;
                    qe.Phone = PhoneTextBox.Text;
                    qe.PayerIdType = PayerIdTypeTextBox.Text;
                    qe.PayerIdNum = PayerIdNumTextBox.Text;
                    qe.ChildFio = ChildFioTextBox.Text;
                    qe.BirthDate = BirthDateCheckBox.Checked ? DateTime.MinValue : BirthDatePicker.Value;
                    qe.PaymTerm = PaymTermCheckBox.Checked ? DateTime.MinValue : PaymTermPicker.Value;
              
                    /*
                     * период оплаты
                     */
                    qe.Category = CategoryTextBox.Text;
                    qe.ServiceName = ServiceNameTextBox.Text;
                    qe.CounterId = CounterIdValTextBox.Text;
                    qe.CounterVal = CounterValTextBox.Text;
                    qe.QuittId = QuittIdTextBox.Text;
                    qe.QuittDate = QuittDateCheckBox.Checked ? DateTime.MinValue : QuittDateDatePicker.Value;
                    qe.InstNum = InstNumTextBox.Text;
                    qe.ClassNum = ClassNumTextBox.Text;
                    qe.SpecFio = SpecFioTextBox.Text;
                    qe.AddAmount = AddAmountTextBox.Text;
                    qe.RuleId = RuleIdTextBox.Text;
                    qe.ExecId = ExecId.Text;
                    qe.RegType = RegType.Text;
                    qe.UIN = UINTextBox.Text;
                    qe.TechCode = TechCodeTextBox.Text;
                    result = qe.Pack();
                }
                else if (qrTabs.SelectedTab == qrTypeWifi)
                {
                    QRWifi qr = new QRWifi();
                    qr.encryption = qrWifiEncryption.Text;
                    qr.networkName = qrWifiName.Text;
                    qr.password = qrWifiPass.Text;
                    qr.hidden = qrWifiHidden.Checked;
                    result = qr.Pack();
                }
                else if (qrTabs.SelectedTab == qrTypeSwiss)
                {
                    MyRes res = new MyRes("Messages,Swiss");
                    string ibanText = qrSwissIban.Text;
                    Iban.IbanType? ibanType = null;
                    switch (qrSwissIbanType.SelectedIndex)
                    {
                        case 0:
                            ibanType = Iban.IbanType.Iban;
                            break;
                        case 1:
                            ibanType = Iban.IbanType.QrIban;
                            break;
                    }
                    if (ibanType == null || ibanText == "")
                        throw new SwissQrCodeException(res.Get("SwissNullIban"));

                    Iban iban = new Iban(ibanText, ibanType.Value);

                    string creditorName = qrSwissCreditorName.Text;
                    string creditorZipCode = qrSwissCreditorZipCode.Text;
                    string creditorCity = qrSwissCreditorCity.Text;
                    string creditorCountry = qrSwissCreditorCountry.Text;
                    string creditorHouseNumberOrAddressline = qrSwissCreditorHNumber.Text;
                    string creditorStreetOrAddressline = qrSwissCreditorStreet.Text;
                    Contact creditor = new Contact(creditorName, creditorZipCode, creditorCity, creditorCountry, creditorStreetOrAddressline, creditorHouseNumberOrAddressline);


                    string referenceText = qrSwissRefText.Text;
                    Reference.ReferenceType? referenceType = null;
                    Reference.ReferenceTextType? referenceTextType = null;
                    switch (qrSwissRefTextType.SelectedIndex)
                    {
                        case 0:
                            referenceTextType = Reference.ReferenceTextType.QrReference;
                            break;
                        case 1:
                            referenceTextType = Reference.ReferenceTextType.CreditorReferenceIso11649;
                            break;
                    }
                    switch (qrSwissRefType.SelectedIndex)
                    {
                        case 0:
                            referenceType = Reference.ReferenceType.QRR;
                            break;
                        case 1:
                            referenceType = Reference.ReferenceType.SCOR;
                            break;
                        case 2:
                            referenceType = Reference.ReferenceType.NON;
                            break;
                    }
                    Reference reference = null;
                    if (!String.IsNullOrEmpty(referenceText))
                        reference = new Reference(referenceType.Value, referenceText, referenceTextType);
                    else
                        reference = new Reference(referenceType.Value, null, null);

                    string debitorName = qrSwissDebitorName.Text;
                    string debitorZipCode = qrSwissDebitorZipCode.Text;
                    string debitorCity = qrSwissDebitorCity.Text;
                    string debitorCountry = qrSwissDebitorCountry.Text;
                    string debitorHouseNumberOrAddressline = qrSwissDebitorHNumber.Text;
                    string debitorStreetOrAddressline = qrSwissDebitorStreet.Text;
                    Contact debitor = null;
                    if (debitorName + debitorCity + debitorZipCode + debitorCountry + debitorStreetOrAddressline + debitorHouseNumberOrAddressline != "")
                        debitor = new Contact(debitorName, debitorZipCode, debitorCity, debitorCountry, debitorStreetOrAddressline, debitorHouseNumberOrAddressline);

                    string addInfoBillInformation = qrSwissBillInfo.Text;
                    string addInfoUnstructureMessage = qrSwissUnstructMessage.Text;
                    AdditionalInformation additionalInformation = new AdditionalInformation(addInfoUnstructureMessage, addInfoBillInformation);

                    Currency? currency = null;
                    switch (qrSwissCurrency.SelectedIndex)
                    {
                        case 0:
                            currency = Currency.EUR;
                            break;
                        case 1:
                            currency = Currency.CHF;
                            break;
                    }
                    decimal amount = 0m;
                    decimal? amountValue = null;
                    bool haveAmount = Decimal.TryParse(qrSwissAmount.Text.Replace('.', ','), out amount);

                    if (amount != 0)
                        amountValue = amount;
                    QRSwissParameters parameters = new QRSwissParameters();
                    parameters.Iban = iban;
                    parameters.Creditor = creditor;
                    parameters.Reference = reference;
                    parameters.AlternativeProcedure1 = qrSwissAlt1.Text;
                    parameters.AlternativeProcedure2 = qrSwissAlt2.Text;
                    parameters.AdditionalInformation = additionalInformation;
                    parameters.Amount = amountValue;
                    parameters.Currency = currency;
                    parameters.Debitor = debitor;
                    QRSwiss qr = new QRSwiss(parameters);
                    result = qr.Pack();
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                FRMessageBox.Error(ex.Message);
            }

        }

        private void qrSelectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            qrTabs.SelectedIndex = (sender as ComboBox).SelectedIndex;
        }

        private void tvData_AfterSelect(object sender, TreeViewEventArgs e)
        {
            bool descrVisible = tvData.SelectedNode != null &&
                (tvData.SelectedNode.Tag is MethodInfo || tvData.SelectedNode.Tag is SystemVariable);
            expandableSplitter1.Visible = descrVisible;
            lblDescription.Visible = descrVisible;

            if (descrVisible)
                lblDescription.ShowDescription(report, tvData.SelectedNode.Tag);
        }

        private void PaymTermCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (PaymTermCheckBox.Checked)
            {
                PaymTermPicker.Enabled = false;
            }
            else
            {
                PaymTermPicker.Enabled = true;
            }
        }

        private void BirthDateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (BirthDateCheckBox.Checked)
            {
                BirthDatePicker.Enabled = false;
            }
            else
            {
                BirthDatePicker.Enabled = true;
            }
        }

        private void DocDateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (DocDateCheckBox.Checked)
            {
                DocumentDatePicker.Enabled = false;
            }
            else
            {
                DocumentDatePicker.Enabled = true;
            }
        }

        private void QuittDateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (QuittDateCheckBox.Checked)
            {
                QuittDateDatePicker.Enabled = false;
            }
            else
            {
                QuittDateDatePicker.Enabled = true;
            }
        }

        /// </<inheritdoc/>
        protected override void Scale()
        {
            //this.Size = DpiHelper.ConvertUnits(this.Size, true);
            base.Scale();            
            int fontSize = qrSwissCurrency.PreferredSize.Height;
            editTextBoxes(this.qrTypeSwiss.Controls, fontSize);

        }

        private void editTextBoxes(Control.ControlCollection collection, int height)
        {
            foreach (Control c in collection)
            {
                if (c is TextBox)
                {
                    c.Font = DpiHelper.GetFontForTextBoxHeight(height, c.Font);
                }
                editTextBoxes(c.Controls, height);
            }
        }
    }
}
