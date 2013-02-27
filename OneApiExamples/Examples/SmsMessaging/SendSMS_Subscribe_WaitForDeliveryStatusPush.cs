using System;
using System.IO;
using log4net.Config;
using OneApi.Client.Impl;
using OneApi.Config;
using OneApi.Listeners;
using OneApi.Model;
using OneApi.Exceptions;

namespace OneApi.Examples.SmsMessaging
{

    /**
      * To run this example follow these 4 steps:
      *
      *  1.) Download 'Parseco C# library' - available at www.github.com/parseco   
      *
      *  2.) Open 'OneApi.sln' in 'Visual Studio 2010' and locate 'OneApiExamples' project    
      *
      *  3.) Open 'Examples.SendSMS_Subscribe_WaitForDeliveryStatusPush' class to edit where you should populate the following fields: 
      *		'senderAddress'     'notifyUrl'   'username'
      *		'message'           'criteria'    'password'        
      *		'recipientAddress'   
      *
      *  4.) Run the 'OneApiExample' project, where an a example list with ordered numbers will be displayed in the console. 
      *      There you will enter the appropriate example number in the console and press 'Enter' key 
      *      on which the result will be displayed in the Console.
      *
      *  Note: 'Delivery Status Notifications' push server is started automatically by adding 'DeliveryStatusNotificationsListener'
      *  using the 'AddPushDeliveryStatusNotificationListener' method. Default server port is 3000 and it can be changed by set the 
      *  'Configuration' property 'DlrStatusPushServerSimulatorPort'. Used port should match the one used in the 'notifyUrl' property when 
      *  subscribing for the notifications using the 'SubscribeToDeliveryStatusNotifications' method.
      **/

    public class SendSMS_Subscribe_WaitForDeliveryStatusPush
    {
        private static string username = "parseco";//"FILL USERNAME HERE !!!";
        private static string password = "Parseco+1";//"FILL PASSWORD HERE !!!";
        private static string senderAddress = "mirko";
        private static string message = "pacofffff offffca";
        private static string recipientAddress = "385922442858";
        private static string notifyUrl = "http://192.168.10.102:3000/";//""; //e.g. "http://127.0.0.1:3000/" 3000=Default port for 'Delivery Info Notifications' server simulator
        private static string criteria = "njok";

        public static void Execute()
        {
            // Configure in the 'app.config' which Logger levels are enabled(all levels are enabled in the example)
            // Check http://logging.apache.org/log4net/release/manual/configuration.html for more informations about the log4net configuration
            //XmlConfigurator.Configure(new FileInfo("OneApiExamples.exe.config"));


            // Initialize Configuration object 
            Configuration configuration = new Configuration(username, password);

            // Initialize SMSClient using the Configuration object
            SMSClient smsClient = new SMSClient(configuration);

            // Add listener(start push server and wait for the 'Delivery Info Notifications')    
            smsClient.SmsMessagingClient.AddPushDeliveryStatusNotificationsListener(new DeliveryStatusNotificationsListener((deliveryInfoNotification) =>
            {
                // Handle pushed 'Delivery Info Notification'
                if (deliveryInfoNotification != null)
                {
                    string deliveryStatus = deliveryInfoNotification.DeliveryInfo.DeliveryStatus;
                    Console.WriteLine(deliveryStatus);
                }
            }));

            // Store 'Delivery Info Notifications' subscription id because we can later remove subscription with it:
            string subscriptionId = smsClient.SmsMessagingClient.SubscribeToDeliveryStatusNotifications(new SubscribeToDeliveryNotificationsRequest(senderAddress, notifyUrl, criteria, "", ""));

            // Send SMS 
            smsClient.SmsMessagingClient.SendSMS(new SMSRequest(senderAddress, criteria + message, recipientAddress));

            // Wait 30 seconds for 'Delivery Info Notification' push-es before removing subscription and closing the server connection 
            System.Threading.Thread.Sleep(30000);

            // Remove 'Delivery Info Notifications' subscription
            smsClient.SmsMessagingClient.RemoveDeliveryNotificationsSubscription(subscriptionId);

            // Remove 'Delivery Info Notifications' push listeners and stop the server
            smsClient.SmsMessagingClient.RemovePushDeliveryStatusNotificationsListeners();

        }
    }
}