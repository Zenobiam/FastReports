using FastReport.Cloud.StorageClient;
using FastReport.Messaging;
using FastReport.Wizards;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace FastReport.Utils
{
    partial class ObjectInfo
    {
        #region Private Fields

        private Bitmap image;
        private int imageIndex;
        private int buttonIndex;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Image.
        /// </summary>
        public Bitmap Image
        {
            get { return image; }
            set
            {
                image = value;
                if (image != null)
                {
                    Res.AddImage(image);  
                    imageIndex = Res.GetImages().Images.Count - 1;
                }
            }
        }

        /// <summary>
        /// Image index.
        /// </summary>
        public int ImageIndex
        {
            get { return imageIndex; }
            set
            {
                imageIndex = value;
                if (imageIndex != -1)
                    image = (Bitmap)Res.GetImage(imageIndex);
            }
        }

        /// <summary>
        /// Button index.
        /// </summary>
        public int ButtonIndex
        {
            get { return buttonIndex; }
            set { buttonIndex = value; }
        }

        #endregion Public Properties

        #region Private Methods

        private void UpdateDesign(object obj, Bitmap image, int imageIndex, string text, int flags, bool multiInsert)
        {
            Image = image;
            ButtonIndex = -1;
            if (image == null)
                ImageIndex = imageIndex;
        }

        private void UpdateDesign(object obj, Bitmap image, int imageIndex, int buttonIndex, string text, int flags,
            bool multiInsert)
        {
            Image = image;
            ButtonIndex = buttonIndex;
            if (image == null)
                ImageIndex = imageIndex;
        }

        #endregion Private Methods
    }

    
     enum ItemWizardEnum : int
    {
        /// <summary>
        /// 
        /// </summary>
        Report = 0,
        /// <summary>
        /// 
        /// </summary>
        ReportItem = 1,
        /// <summary>
        /// 
        /// </summary>
        Game = 2
    }

    partial class RegisteredObjects
    {
        #region Public Methods

        /// <summary>
        /// Registers a new cloud storage client.
        /// </summary>
        /// <param name="obj">Type of cloud storage client.</param>
        /// <param name="text">Text for cloud storage client's menu item.</param>
        /// <remarks>
        /// The <b>obj</b> must be of <see cref="CloudStorageClient"/> type.
        /// </remarks>
        /// <example>
        /// <code>
        /// // register own cloud storage client
        /// RegisteredObjects.AddCloud(typeof(MyCloud), "My Cloud");
        /// </code>
        /// </example>
        public static void AddCloud(Type obj, string text)
        {
            if (!obj.IsSubclassOf(typeof(Cloud.StorageClient.CloudStorageClient)))
            {
                throw new Exception("The 'obj' parameter must be of CloudStorageClient type.");
            }
            InternalAdd(obj, "", null, -1, text, 0, false);
        }

        /// <summary>
        /// Registers a new messenger.
        /// </summary>
        /// <param name="obj">Type of messenger.</param>
        /// <param name="text">Text messenger's menu item.</param>
        /// <remarks>
        /// The <b>obj</b> must be of <see cref="MessengerBase"/> type.
        /// </remarks>
        /// <example>
        /// <code>
        /// // register own messenger
        /// RegisteredObjects.AddMessenger(typeof(MyMessenger), "My Messenger");
        /// </code>
        /// </example>
        public static void AddMessenger(Type obj, string text)
        {
            if (!obj.IsSubclassOf(typeof(Messaging.MessengerBase)))
            {
                throw new Exception("The 'obj' parameter must be of MessengerBase type.");
            }
            InternalAdd(obj, "", null, -1, text, 0, false);
        }

        /// <summary>
        /// This method updates the image for each object in the object collection. Need to be called after changing Dpi and updating resources.
        /// </summary>
        public static void UpdateItemsImages()
        {
            foreach (ObjectInfo obj in Objects.Items)
                UpdateItemsImages(obj);
        }

        /// <summary>
        /// Registers a new wizard.
        /// </summary>
        /// <param name="obj">Type of wizard.</param>
        /// <param name="image">Image for wizard item.</param>
        /// <param name="text">Text for wizard item.</param>
        /// <param name="isReportItemWizard"><b>true</b> if this wizard creates some items in existing report.</param>
        /// <remarks>
        /// The <b>obj</b> must be of <see cref="WizardBase"/> type.
        /// </remarks>
        /// <example>This example shows how to register own wizard that is used to create some items in the
        /// current report. If you want to register a wizard that will be used to create a new report,
        /// set the <b>isReportItemWizard</b> to <b>false</b>.
        /// <code>
        /// // register own wizard
        /// RegisteredObjects.AddWizard(typeof(MyWizard), myWizBmp, "My Wizard", true);
        /// </code>
        /// </example>
        public static void AddWizard(Type obj, Bitmap image, string text, bool isReportItemWizard)
        {
            if (!obj.IsSubclassOf(typeof(WizardBase)))
                throw new Exception("The 'obj' parameter must be of WizardBase type.");
            InternalAdd(obj, "", image, -1, text, isReportItemWizard ? 1 : 0, false);
        }

        #endregion Public Methods

        #region private methods

        private static void UpdateItemsImages(ObjectInfo obj)
        {
            obj.ImageIndex = obj.ImageIndex;
            foreach (ObjectInfo info in obj.Items)
                UpdateItemsImages(info);
        }

        #endregion


        #region Internal Methods

        internal static void AddCloud(Type obj, string text, int imageIndex)
        {
            if (!obj.IsSubclassOf(typeof(Cloud.StorageClient.CloudStorageClient)))
            {
                throw new Exception("The 'obj' parameter must be of CloudStorageClient type.");
            }
            InternalAdd(obj, "", null, imageIndex, text, 0, false);
        }

        internal static void AddMessenger(Type obj, string text, int imageIndex)
        {
            if (!obj.IsSubclassOf(typeof(Messaging.MessengerBase)))
            {
                throw new Exception("The 'obj' parameter must be of MessengerBase type.");
            }
            InternalAdd(obj, "", null, imageIndex, text, 0, false);
        }

        internal static void AddWizard(Type obj, int imageIndex, string text, ItemWizardEnum itemWizardEnum)
        {
            InternalAdd(obj, "", null, imageIndex, text, (int)itemWizardEnum, false);
        }

        internal static ObjectInfo FindFunctionsRoot()
        {
            List<ObjectInfo> list = new List<ObjectInfo>();
            FObjects.EnumItems(list);

            foreach (ObjectInfo item in list)
            {
                if (item.Name == "Functions")
                    return item;
            }
            return null;
        }

        #endregion Internal Methods
    }
}