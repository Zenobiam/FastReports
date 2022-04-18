using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using FastReport.Export.Pdf;

namespace FastReport.Messaging.Tests
{
    [TestClass]
    public class TelegramMessengerTests
    {
        private const string SENDER_PHONE = "";
        private const string RECIPIENT_PHONE = "";

        [TestMethod]
        public async Task ConnectAsync_default_trueReturnedAsync()
        {
            //arrange
            bool expected = true;

            //act
            TelegramMessenger telegram = new TelegramMessenger();
            bool actual = await telegram.ConnectAsync();

            //assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task ConnectAsync_reconnect_false_trueReturnedAsync()
        {
            //arrange
            bool expected = true;

            //act
            TelegramMessenger telegram = new TelegramMessenger();
            bool actual = await telegram.ConnectAsync(false);

            //assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task ConnectAsync_reconnect_true_trueReturnedAsync()
        {
            //arrange
            bool expected = true;

            //act
            TelegramMessenger telegram = new TelegramMessenger();
            bool actual = await telegram.ConnectAsync(true);

            //assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Connect_default_trueReturnedAsync()
        {
            //arrange
            bool expected = true;

            //act
            TelegramMessenger telegram = new TelegramMessenger();
            bool actual = telegram.Connect();

            //assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Connect_reconnect_false_trueReturnedAsync()
        {
            //arrange
            bool expected = true;

            //act
            TelegramMessenger telegram = new TelegramMessenger();
            bool actual = telegram.Connect(false);

            //assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Connect_reconnect_true_trueReturnedAsync()
        {
            //arrange
            bool expected = true;

            //act
            TelegramMessenger telegram = new TelegramMessenger();
            bool actual = telegram.Connect(true);

            //assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task RequestCodeAsync_Default_hashNotNullAsync()
        {
            //act
            TelegramMessenger telegram = new TelegramMessenger();
            telegram.SenderPhone = SENDER_PHONE;
            telegram.Connect();

            //assert
            try
            {
                await telegram.RequestCodeAsync();
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }            
        }

        [TestMethod]
        public async Task RequestCodeAsync_WithSenderPhone_hashNotNullAsync()
        {
            //act
            TelegramMessenger telegram = new TelegramMessenger();
            telegram.Connect();

            //assert
            try
            {
                await telegram.RequestCodeAsync(SENDER_PHONE);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void RequestCode_Default_hashNotNull()
        {
            //act
            TelegramMessenger telegram = new TelegramMessenger();
            telegram.SenderPhone = SENDER_PHONE;
            telegram.Connect();

            //assert
            try
            {
                telegram.RequestCode();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void RequestCode_WithSenderPhone_hashNotNull()
        {
            //act
            TelegramMessenger telegram = new TelegramMessenger();
            telegram.Connect();

            //assert
            try
            {
                telegram.RequestCode(SENDER_PHONE);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public async Task SendReportAsync_trueReturnedAsync()
        {
            //arrange
            bool expected = true;
            Report report = new Report();
            report.Prepare();
            bool actual = false;
            //act
            TelegramMessenger telegram = new TelegramMessenger();
            telegram.SenderPhone = SENDER_PHONE;
            telegram.RecipientPhone = RECIPIENT_PHONE;
            telegram.Connect();
            if(telegram.Connected && telegram.Authorized)
            {
                actual = await telegram.SendReportAsync(report, null);
            }

            //assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SendReport_trueReturned()
        {
            //arrange
            bool expected = true;
            Report report = new Report();
            report.Prepare();
            bool actual = false;
            //act
            TelegramMessenger telegram = new TelegramMessenger();
            telegram.SenderPhone = SENDER_PHONE;
            telegram.RecipientPhone = RECIPIENT_PHONE;
            telegram.Connect();
            if (telegram.Connected && telegram.Authorized)
            {
                actual = telegram.SendReport(report, null);
            }

            //assert
            Assert.AreEqual(expected, actual);
        }
    }
}
