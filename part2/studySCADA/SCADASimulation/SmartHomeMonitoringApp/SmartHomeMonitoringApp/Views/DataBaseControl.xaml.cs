using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SmartHomeMonitoringApp.Logics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace SmartHomeMonitoringApp.Views
{
    /// <summary>
    /// DataBaseControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DataBaseControl : UserControl
    {
        public bool IsConnected { get; set; }

        Thread MqttTread { get; set; }             //  없으면 UI 처리가 어려워짐

        public DataBaseControl()
        {
            InitializeComponent();
        }

        // 화면 유저컨트롤 로드 이벤트
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TxbBorkerUrl.Text = Commons.BROKERHOST;
            TxbMqttTopic.Text = Commons.MQTTTOPIC;
            TxtConnString.Text = Commons.MYSQL_CONNSTIRNG;

            IsConnected = false;
        }

        // 토글 버튼 클릭 이벤트 핸들러 
        private void BtnConnDb_Click(object sender, RoutedEventArgs e)
        {
            if (IsConnected == false)
            {
                // Mqtt 브로커 생성
                Commons.MQTT_CLIENT = new uPLibrary.Networking.M2Mqtt.MqttClient(Commons.BROKERHOST);

                try
                {
                    // mqtt subscriber ( 구독할) 로직
                    if(Commons.MQTT_CLIENT.IsConnected == false)
                    {
                        // mqtt 접속 
                        Commons.MQTT_CLIENT.MqttMsgPublishReceived += MQTT_CLIENT_MqttMsgPublishReceived;
                        Commons.MQTT_CLIENT.Connect("MONITOR");  // 클라이언트 아이디 = 모니터 
                        Commons.MQTT_CLIENT.Subscribe(new string[] {Commons.MQTTTOPIC}, new byte[] {MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE});   // QOS는 네트워크 통신의 옵션 
                        UpdateLog(">>>> MQTT Broker Connected");

                        BtnConnDb.IsChecked = true;
                        BtnConnDb.Content = "MQTT 연결중";
                        IsConnected = true;  // 예외가 발생하면 true로 변경할 필요없음
                    }
                }
                catch (Exception ex)
                {
                    UpdateLog($"!!!  Mqtt Error 발생 : {ex.Message}");
                }
            }
            else
            {
                try 
                {
                    if(Commons.MQTT_CLIENT.IsConnected)
                    {
                        Commons.MQTT_CLIENT.MqttMsgPublishReceived -= MQTT_CLIENT_MqttMsgPublishReceived;
                        Commons.MQTT_CLIENT.Disconnect();
                        UpdateLog(">>>> MQTT Broker Disconnected...");

                        BtnConnDb.IsChecked = false;
                        BtnConnDb.Content = "MQTT 연결 종료";
                        IsConnected = false;
                    }
                }
                catch (Exception ex) 
                {
                    UpdateLog($"!!! Mqtt Error 발생 : {ex.Message}");
                }

            }
        }
        
        private void UpdateLog (string msg)
        {
            // 예외처리 필요 
            this.Invoke(() => { 
                TxtLog.Text += $"{msg}\n";
                TxtLog.ScrollToEnd();
            });
        }

        // Subscribe가 발생할 때 이벤트 핸들러 
        private void MQTT_CLIENT_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            var msg = Encoding.UTF8.GetString(e.Message);
            UpdateLog(msg);
            SetToDataBase(msg, e.Topic);  // 실제 DB 저장처리
        }

        private void SetToDataBase(string msg, string topic)
        {
            var currValue = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg);
            if(currValue != null) 
            {
                //Debug.WriteLine(currValue["Home_Id"]);
                //Debug.WriteLine(currValue["Room_Name"]);
                //Debug.WriteLine(currValue["Sensing_DateTime"]);
                //Debug.WriteLine(currValue["Temp"]);
                //Debug.WriteLine(currValue["Humid"]);

                try 
                {
                    using (MySqlConnection conn = new MySqlConnection(Commons.MYSQL_CONNSTIRNG))
                    {
                        if (conn.State == System.Data.ConnectionState.Closed) conn.Open();
                        string insQuery = @"INSERT INTO smarthomesensor
                                                                            (Home_Id,
                                                                            Room_Name,
                                                                            Sensing_DateTime,
                                                                            Temp,
                                                                            Humid)
                                                                            VALUES
                                                                            (@Home_Id,
                                                                            @Room_Name,
                                                                            @Sensing_DateTime,
                                                                            @Temp,
                                                                            @Humid)";

                        MySqlCommand cmd = new MySqlCommand(insQuery, conn);
                        cmd.Parameters.AddWithValue("@Home_Id", currValue["Home_Id"]);
                        cmd.Parameters.AddWithValue("@Room_Name", currValue["Room_Name"]);
                        cmd.Parameters.AddWithValue("@Sensing_DateTime", currValue["Sensing_DateTime"]);
                        cmd.Parameters.AddWithValue("@Temp", currValue["Temp"]);
                        cmd.Parameters.AddWithValue("@Humid", currValue["Humid"]);

                        if (cmd.ExecuteNonQuery()==1) 
                        {
                            UpdateLog(">>>> DB Insert suceed");
                        }
                        else
                        {
                            UpdateLog(">>>> DB Insert failed");   // 일어날 일이 거의 없음 
                        }
                    }
                }
                catch (Exception ex)
                {
                    UpdateLog($"!!! DB Error 발생 : {ex.Message}");
                }
            }
        }
    }
}
