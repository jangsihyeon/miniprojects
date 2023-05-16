using ControlzEx.Theming;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SmartHomeMonitoringApp.Logics;
using SmartHomeMonitoringApp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

namespace SmartHomeMonitoringApp
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        string DefaultTheme { get; set; } = "Light";
        string DefaultAccent { get; set; } = "Cobalt";

        public MainWindow()
        {
            InitializeComponent();
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;

        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // <Frame> == Page.xaml
            // <ContentControl>==>UserContorl.xaml
            // ActiveItem.Content = new Views.DataBaseControl();
        }

        // 끝내기 버튼 클릭 이벤트 
        private void MnuExitProgram_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill(); // 작업 관리자에서 프로세스 종료 
            //Environment.Exit(0);  // 둘중 하나만 쓰면 됨
        }

        // Mqtt 시작 메뉴 클릭 이벤트 핸들러 
        private void MnuStartSubscribe_Click(object sender, RoutedEventArgs e)
        {
            var mqttPopWin = new MqttPopupWindow();
            mqttPopWin.Owner = this;
            mqttPopWin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            var result = mqttPopWin.ShowDialog();

            if (result == true) 
            {
                var userControl = new Views.DataBaseControl();
                ActiveItem.Content = userControl;
                StsSelScreen.Content =  "DataBase Monitoring"; //typeof(Views.DataBaseControl);
            }
        }

        private async void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            var mySettings = new MetroDialogSettings 
                                              {
                                                    AffirmativeButtonText = "끝내기",
                                                    NegativeButtonText = "취소",
                                                    AnimateShow = true,
                                                    AnimateHide = true
                                              };
            var result = await this.ShowMessageAsync("종료","프로그램을 끝내시겠습니까?",
                                                                                MessageDialogStyle.AffirmativeAndNegative, mySettings);
            if (result == MessageDialogResult.Negative)
            {
                e.Cancel = true;
            }
            else
            {
                if(Commons.MQTT_CLIENT !=null && Commons.MQTT_CLIENT.IsConnected)
                {
                    Commons.MQTT_CLIENT.Disconnect();
                }
                Process.GetCurrentProcess().Kill();     // 가장 확실
            }
        }

        private void BtnExitProgram_Click(object sender, RoutedEventArgs e)
        {
            this.MetroWindow_Closing(sender, new CancelEventArgs());
        }

        private void MnuDataBaseMon_Click(object sender, RoutedEventArgs e)
        {
            var userControl = new Views.DataBaseControl();
            ActiveItem.Content = userControl;
            StsSelScreen.Content = "DataBase Monitoring"; //typeof(Views.DataBaseControl);
        }

        private void MnuRealTimeMon_Click(object sender, RoutedEventArgs e)
        {
            ActiveItem.Content = new Views.RealTimeControl();
            StsSelScreen.Content = "RealTime Monitoring"; //typeof(Views.DataBaseControl);

        }

        private void MnuVisualizationMon_Click(object sender, RoutedEventArgs e)
        {
            ActiveItem.Content = new Views.VisualizationControl();
            StsSelScreen.Content = "Visualizstion View";
        }

        private void MnuAbout_Click(object sender, RoutedEventArgs e)
        {
            var about = new About();
            about.Owner = this;
            about.ShowDialog();
        }

        // 모든 테마와 액센트를 전부 처리할 체크 이벤트 핸들러 
        private void MnuThemeAccent_Checked(object sender, RoutedEventArgs e)
        {
            // 클릭되는 테마가 라이트인지 다크인지 판단해서 라이트 클릭시 다크는 체크를 해제 , 다크를 클릭하면 라이트를 체크해제
            Debug.WriteLine((sender as MenuItem).Header);
            // 액센트도 체크하는 값을 나머지 액센트 전부 체크해제 
            switch ((sender as MenuItem).Header)
            {
                case "Light":
                    MnuLightTheme.IsChecked = true;
                    MnuDarkTheme.IsChecked = false;
                    DefaultTheme = "Light";
                    break;
                case "Dark":
                    MnuLightTheme.IsChecked = false;
                    MnuDarkTheme.IsChecked = true;
                    DefaultTheme = "Dark";
                    break;
                case "Amber":
                    MnuAccentAmber.IsChecked = true;
                    MnuAccentBlue.IsChecked = false;
                    MnuAccentBrown.IsChecked = false;
                    MnuAccentCoblat.IsChecked = false;
                    DefaultAccent = "Amber";
                    break;
                case "Blue":
                    MnuAccentAmber.IsChecked = false;
                    MnuAccentBlue.IsChecked = true;
                    MnuAccentBrown.IsChecked = false;
                    MnuAccentCoblat.IsChecked = false;
                    DefaultAccent = "Blue";
                    break;
                case "Brown":
                    MnuAccentAmber.IsChecked = false;
                    MnuAccentBlue.IsChecked = false;
                    MnuAccentBrown.IsChecked = true;
                    MnuAccentCoblat.IsChecked = false;
                    DefaultAccent = "Brown";
                    break;
                case "Cobalt":
                    MnuAccentAmber.IsChecked = false;
                    MnuAccentBlue.IsChecked = false;
                    MnuAccentBrown.IsChecked = false;
                    MnuAccentCoblat.IsChecked = true;
                    DefaultAccent = "Cobalt";
                    break;
            }

            ThemeManager.Current.ChangeTheme(this, $"{DefaultTheme}.{DefaultAccent}");
        }
    }
 }

