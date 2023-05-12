using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using uPLibrary.Networking.M2Mqtt;

namespace SmartHomeMonitoringApp.Logics
{
    public class Commons
    {
        // 화면마다 공유할 MQTT 브로커 아이피 변수
        public static string BROKERHOST { get; set; } = "127.0.0.1";

        public static string MQTTTOPIC { get; set; } = "SmartHome/IoTData/";

        public static string MYSQL_CONNSTIRNG { get; set; }  = "Server=localhost;" + "Port=3306;" + "DataBase=miniproject;" + "Uid=root;" + "Pwd=12345;";

        // mqtt 클라이언트 공용 객체 
        public static MqttClient MQTT_CLIENT {get; set;}
        
        // 자식 클래스이면서 메트로 윈도우를 사용하지 않아 마흐앱스를 사용할 수 없을 경우 사용 
        public static async Task<MessageDialogResult> ShowCustomMessageAsync (string title, string message,
            MessageDialogStyle style = MessageDialogStyle.Affirmative)
        {
            return await((MetroWindow)Application.Current.MainWindow).ShowMessageAsync(title, message, style, null);
        }
    }
}
