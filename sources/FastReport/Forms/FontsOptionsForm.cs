using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FastReport.Utils;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Forms
{
    public partial class FormsFonts : ScaledSupportingForm
    {
        List<FormsSet> forms = new List<FormsSet>();

        FormsSet formsSet;
        FormElement formElement;

        ///
        public FormsFonts() : base()
        {
            InitializeComponent();
            InitFontsName();
            InitFontSizes();
            InitList();
            Localize();
            Scale();
            updateExample();
        }

        private void Localize()
        {
            MyRes res = new MyRes("Forms,FontsPageOptions");
            Text = res.Get("FormName");
            labelSettingsFor.Text = res.Get("SettingsFor");
            fontLabel.Text = res.Get("Font");
            formElementLabel.Text = res.Get("FormElement");
            defaultsButton.Text = res.Get("DefaultButton");
            sizeLabel.Text = res.Get("Size");
            italicCheckBox.Text = res.Get("Italic");
            boldCheckBox.Text = res.Get("Bold");
            sampleLabel.Text = res.Get("Sample");
            cancelButton.Text = res.Get("CancelButton");
        }

        private void InitFontsName()
        {
            foreach (FontFamily font in System.Drawing.FontFamily.Families)
            {
                fontComboBox.Items.Add(font.Name);
            }
        }

        private void InitFontSizes()
        {
            for (int i = 6; i < 21; i++)
            {
                sizeComboBox.Items.Add(i);
            }
        }

        private void InitList()
        {

            XmlItem xi = Config.Root.FindItem("Designer").FindItem("Fonts");

            if (xi.Count != 4)
            {
                xi.Clear();
                initDefaultFontsOptions();
            }

            foreach (XmlItem item in xi.Items)
            {
                formsSet = new FormsSet(); // formset = Set the form as default
                formsSet.xmlProperty = item.Name;
                formsSet.FormName = item.GetProp("name");
                AddFormToList(formsSet);
                foreach (XmlItem element in item.Items)
                {
                    FormElement fe = new FormElement();
                    fe.xmlProperty = element.Name;
                    fe.ElementName = element.GetProp("element-name");
                    fe.FontName = element.GetProp("font-name");
                    fe.FontSize = int.Parse(element.GetProp("font-size"));
                    fe.Bold = element.GetProp("font-bold") == "1";
                    fe.Italic = element.GetProp("font-italic") == "1";
                    formsSet.formElements.Add(fe);

                }
            }

            formsSetComboBox.SelectedIndex = 0;
        }

        private void GetFormsSet(int FormSetID)
        {
            formsSet = forms[FormSetID];
            elementsListBox.Items.Clear();

            foreach (FormElement formElement in formsSet.formElements)
            {
                elementsListBox.Items.Add(formElement.ElementName);
            }

            getFormElement(0);
            elementsListBox.SelectedIndex = 0;
        }

        private void getFormElement(int ElementID)
        {
            formElement = formsSet.formElements[ElementID];

            formsSetComboBox.Text = formsSet.FormName;

            fontComboBox.Text = formElement.FontName;
            sizeComboBox.Text = formElement.FontSize.ToString();

            italicCheckBox.Checked = formElement.Italic;
            boldCheckBox.Checked = formElement.Bold;

            updateExample();
        }

        private void updateExample()
        {
            ExampleBox.Font = new Font(fontComboBox.Text, sizeComboBox.Text != "" ? DpiHelper.ParseFontSize(int.Parse(sizeComboBox.Text)) : 8,
                (italicCheckBox.Checked ? FontStyle.Italic : 0) | (boldCheckBox.Checked ? FontStyle.Bold : 0));
        }

        private void AddFormToList(FormsSet item)
        {
            forms.Add(item);
            formsSetComboBox.Items.Add(item.FormName);
        }

        private void FormSetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveOption();
            GetFormsSet(formsSetComboBox.SelectedIndex);
        }

        private void DefaultsButton_Click(object sender, EventArgs e)
        {
            formElement.setDefault();
            GetFormsSet(formsSetComboBox.SelectedIndex);
            updateExample();
        }

        private void ElementsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveOption();
            getFormElement(elementsListBox.SelectedIndex);
        }

        private void SaveOption()
        {
            if (formElement != null)
            {
                formElement.Italic = italicCheckBox.Checked;
                formElement.Bold = boldCheckBox.Checked;

                formElement.FontName = fontComboBox.Text;
                formElement.FontSize = sizeComboBox.Text != "" ? int.Parse(sizeComboBox.Text) : 8;
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            SaveOption();
            SaveFontsChanges();

        }

        private void SaveFontsChanges()
        {
            XmlItem xi = Config.Root.FindItem("Designer").FindItem("Fonts");
            foreach (FormsSet form in forms)
            {
                XmlItem f = xi.FindItem(form.xmlProperty);
                List<XmlProperty> n = new List<XmlProperty>();
                n.Add(XmlProperty.Create("name", form.FormName));
                f.Properties = n.ToArray();

                foreach (FormElement element in form.formElements)
                {
                    XmlItem el = f.FindItem(element.xmlProperty);

                    List<XmlProperty> p = new List<XmlProperty>();
                    p.Add(XmlProperty.Create("element-name", element.ElementName));
                    p.Add(XmlProperty.Create("font-size", element.FontSize.ToString()));
                    p.Add(XmlProperty.Create("font-name", element.FontName));
                    p.Add(XmlProperty.Create("font-italic", element.Italic ? "1" : "0"));
                    p.Add(XmlProperty.Create("font-bold", element.Bold ? "1" : "0"));

                    el.Properties = p.ToArray();
                }
            }
        }

        private void initDefaultFontsOptions()
        {
            XmlItem fontsConfig = Config.Root.FindItem("Designer").FindItem("Fonts");

            //Code page init
            XmlItem codePageDesigner = new XmlItem();
            codePageDesigner.Name = "CodePageDesigner";
            List<XmlProperty> cpdProp = new List<XmlProperty>();
            cpdProp.Add(XmlProperty.Create("name", "Code Page Designer"));
            codePageDesigner.Properties = cpdProp.ToArray();

            XmlItem codePage = new XmlItem();
            codePage.Name = "CodePage";
            List<XmlProperty> codePageProp = new List<XmlProperty>();
            codePageProp.Add(XmlProperty.Create("element-name", "Code page"));
            codePageProp.Add(XmlProperty.Create("font-size", "10"));
            codePageProp.Add(XmlProperty.Create("font-name", FontFamily.GenericMonospace.Name));
            codePageProp.Add(XmlProperty.Create("font-italic", "0"));
            codePageProp.Add(XmlProperty.Create("font-bold", "0"));
            codePage.Properties = codePageProp.ToArray();
            codePageDesigner.AddItem(codePage);
            fontsConfig.AddItem(codePageDesigner);

            //Query wizard form init
            XmlItem QueryWizardForm = new XmlItem();
            QueryWizardForm.Name = "QueryWizardForm";
            List<XmlProperty> qwfProp = new List<XmlProperty>();
            qwfProp.Add(XmlProperty.Create("name", "Query wizard form"));
            QueryWizardForm.Properties = qwfProp.ToArray();

            XmlItem queryWindow = new XmlItem();
            queryWindow.Name = "QueryWindow";
            List<XmlProperty> qwProp = new List<XmlProperty>();
            qwProp.Add(XmlProperty.Create("element-name", "Query window"));
            qwProp.Add(XmlProperty.Create("font-size", "8"));
            qwProp.Add(XmlProperty.Create("font-name", "Tahoma"));
            qwProp.Add(XmlProperty.Create("font-italic", "0"));
            qwProp.Add(XmlProperty.Create("font-bold", "0"));
            queryWindow.Properties = qwProp.ToArray();
            QueryWizardForm.AddItem(queryWindow);
            fontsConfig.AddItem(QueryWizardForm);

            //Text editor form init
            XmlItem TextEditorForm = new XmlItem();
            TextEditorForm.Name = "TextEditorForm";
            List<XmlProperty> tefProp = new List<XmlProperty>();
            tefProp.Add(XmlProperty.Create("name", "Text Editor Form"));
            TextEditorForm.Properties = tefProp.ToArray();

            XmlItem TextFieldEditor = new XmlItem();
            TextFieldEditor.Name = "TextFieldEditor";
            List<XmlProperty> tfeProp = new List<XmlProperty>();
            tfeProp.Add(XmlProperty.Create("element-name", "Text Field Editor"));
            tfeProp.Add(XmlProperty.Create("font-size", DrawUtils.DefaultReportFont.Size.ToString()));
            tfeProp.Add(XmlProperty.Create("font-name", DrawUtils.DefaultReportFont.Name));
            tfeProp.Add(XmlProperty.Create("font-italic", "0"));
            tfeProp.Add(XmlProperty.Create("font-bold", "0"));
            TextFieldEditor.Properties = tfeProp.ToArray();
            TextEditorForm.AddItem(TextFieldEditor);
            fontsConfig.AddItem(TextEditorForm);

            //Expression editor
            XmlItem ExpressionEditorForm = new XmlItem();
            ExpressionEditorForm.Name = "ExpressionEditorForm";
            List<XmlProperty> eef = new List<XmlProperty>();
            eef.Add(XmlProperty.Create("name", "Expression Editor Form"));
            ExpressionEditorForm.Properties = eef.ToArray();

            XmlItem FormulaEditor = new XmlItem();
            FormulaEditor.Name = "FormulaEditor";
            List<XmlProperty> feProp = new List<XmlProperty>();
            feProp.Add(XmlProperty.Create("element-name", "Formula Editor"));
            feProp.Add(XmlProperty.Create("font-size", DrawUtils.FixedFont.Size.ToString()));
            feProp.Add(XmlProperty.Create("font-name", DrawUtils.FixedFont.Name));
            feProp.Add(XmlProperty.Create("font-italic", "0"));
            feProp.Add(XmlProperty.Create("font-bold", "0"));
            FormulaEditor.Properties = feProp.ToArray();
            ExpressionEditorForm.AddItem(FormulaEditor);
            fontsConfig.AddItem(ExpressionEditorForm);


        }

        private void OptionChanged(object sender, EventArgs e)
        {
            updateExample();
        }
    }

    public class FormsSet : List<FormElement>
    {
        public String xmlProperty;
        public String FormName;
        public List<FormElement> formElements = new List<FormElement>();
    }

    public class FormElement
    {
        public String xmlProperty;
        public String ElementName;
        public String FontName;
        public int FontSize;
        public Boolean Italic;
        public Boolean Bold;

        public void setDefault()
        {
            switch (xmlProperty)
            {
                case "QueryWindow":
                    FontName = "Tahoma";
                    FontSize = 8;
                    Italic = false;
                    Bold = false;
                    break;
                case "CodePage":
                    FontName = FontFamily.GenericMonospace.Name;
                    FontSize = 10;
                    Italic = false;
                    Bold = false;
                    break;
                case "TextFieldEditor":
                    FontName = DrawUtils.DefaultReportFont.Name;
                    FontSize = (int)DrawUtils.DefaultReportFont.Size;
                    Italic = false;
                    Bold = false;
                    break;
                case "FormulaEditor":
                    FontName = DrawUtils.FixedFont.Name;
                    FontSize = (int)DrawUtils.FixedFont.Size;
                    Italic = false;
                    Bold = false;
                    break;
                default:
                    FontName = "Tahoma";
                    FontSize = 8;
                    Italic = false;
                    Bold = false;
                    break;
            }
        }

        public FormElement()
        {
            setDefault();
        }
    }
}
