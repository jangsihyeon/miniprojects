using OxyPlot;
using OxyPlot.Series;
using MySql.Data.MySqlClient;
using SmartHomeMonitoringApp.Logics;
using System;
using System.Collections.Generic;
using System.Data;
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
using OxyPlot.Legends;

namespace SmartHomeMonitoringApp.Views
{
    /// <summary>
    /// VisualizationControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class VisualizationControl : UserControl
    {
        List<string> Divisions = null;

        string FirstSensingDate = string.Empty;

        int TotalDataCount = 0;  // 검색된 데이터 개수 
        public VisualizationControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            {
                // 룸선택 콤보박스 초기화 
                Divisions = new List<string> { "SELECT", "LIVING", "DINING", "BED", "BATH" };
                CboRommName.ItemsSource = Divisions;

                // 검색 시작일 날짜 -DB에서 제일 오래된 날짜를 가져와서 할당 
                using (MySqlConnection conn = new MySqlConnection(Commons.MYSQL_CONNSTIRNG))
                {
                    conn.Open();
                    var dtQuery = @"select f.Sensing_Date
                                                from(
	                                                SELECT date_format(Sensing_DateTime, '%y-%m-%d') as Sensing_Date
		                                                from smarthomesensor
                                                ) as f
                                                group by f.Sensing_Date
                                                order by f.Sensing_Date Asc limit 1";
                    MySqlCommand cmd = new MySqlCommand(dtQuery, conn);
                    var result = cmd.ExecuteScalar();
                    Debug.WriteLine(result.ToString());
                    FirstSensingDate =DtpStart.Text = result.ToString();
                    DtpEnd.Text = DateTime.Now.ToString("yyyy-MM-dd");
                }
            }
        }

        // 최초의 클릭 이벤트 핸들러 
        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            bool IsValid = true;
            string errorMsg = string.Empty;
            DataSet ds = new DataSet();

            // 검색,저장,수정,삭제 시 반드시 검증이 필요 (validation)
            if (CboRommName.SelectedValue.ToString() == "SELECT")
            {
                IsValid = false;
                errorMsg += "방구분을 선택하세요.\n";
            }
             //시스템이 시작된 날짜보다 더 옛날 날짜로 검색하려면 
            if (DateTime.Parse(DtpStart.Text)< DateTime.Parse(FirstSensingDate))
            {
                IsValid = false;
                errorMsg += $"검색 시작일은 {FirstSensingDate}부터 가능합니다.\n";
            }
            // 오늘 날짜 이후의 날짜로 검색하려면
            if(DateTime.Parse(DtpEnd.Text) >  DateTime.Now)
            {
                IsValid = false;
                errorMsg += "검색 종료일은 오늘까지 가능합니다.\n";
            }
            // 검색 시작일이 검색 종료일보다 이후면
            if(DateTime.Parse(DtpStart.Text) > DateTime.Parse(DtpEnd.Text))
            {
                IsValid = false;
                errorMsg += "검색시작일이 검색 종료일보다 최신일 수 없습니다.\n";
            }
            if(IsValid==false)
            {
                Commons.ShowCustomMessageAsync("검색", errorMsg);
                return;
            }

            // 드디어 검색 시작 
            TotalDataCount = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.MYSQL_CONNSTIRNG))
                {
                    conn.Open();
                    var searchQuery = @"SELECT id,
                                                                 Room_Name,
                                                                 Sensing_DateTime,
                                                                 Temp,
                                                                 Humid
                                                            FROM smarthomesensor
                                                          WHERE UPPER(Room_Name) = @Room_Name
                                                            AND date_format(Sensing_DateTime, '%Y-%m-%d' )
                                                            BETWEEN @StartDate AND @EndDate";
                    MySqlCommand cmd = new MySqlCommand(searchQuery, conn);
                    cmd.Parameters.AddWithValue("@Room_Name", CboRommName.SelectedValue.ToString());
                    cmd.Parameters.AddWithValue("@StartDate", DtpStart.Text);
                    cmd.Parameters.AddWithValue("@EndDate", DtpEnd.Text);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                   
                    adapter.Fill(ds, "smarthomesensor");
                    //MessageBox.Show("TotalData", ds.Tables["smarthomesensor"].Rows.Count.ToString());     데이터 개수 확인
                }
            }
            catch (Exception ex) 
            {
                Commons.ShowCustomMessageAsync("DB검색", $"DB 검색 오류 {ex.Message}");
            }

            // Create the plot model// 선택한 방의 이름이 타이틀로 나오도록 
            var tmp = new PlotModel { Title = $"{CboRommName.SelectedValue} ROOM", DefaultFont = "Arial" };
            var legend = new Legend
            {
                LegendBorder = OxyColors.DarkGray,
                LegendBackground = OxyColor.FromArgb(150, 255, 255, 255),
                LegendPosition = LegendPosition.TopRight,
                LegendPlacement = LegendPlacement.Outside,
            };

            tmp.Legends.Add(legend);  // 범례추가

            // Create two line series (markers are hidden by default)
            var Tempseries = new LineSeries
            {
                Title = "Temperature(℃)",
                MarkerType = MarkerType.Circle,
                Color = OxyColors.DarkOrange,
            };
            var Humidseries = new LineSeries
            {
                Title = "Humidity(%)",
                MarkerType = MarkerType.Square,
                Color = OxyColors.Aqua
            };
            // DB에서 가져온 데이터 차트에 뿌리도록 처리 
            if (ds.Tables[0].Rows.Count > 0)
            {
                TotalDataCount = ds.Tables[0].Rows.Count;

                var count = 0;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Tempseries.Points.Add(new DataPoint(count++, Convert.ToDouble(row["Temp"])));
                    Humidseries.Points.Add(new DataPoint(count++, Convert.ToDouble(row["Humid"])));
                }

                // Add the series to the plot model
                LblTotalCount.Content = $"검색 데이터 {TotalDataCount}개 ";
            }

            else // 데이터가 하나도 없을때는 모두 지워야함 
            {
                OpvSmartHome.Model = null;
            }
                tmp.Series.Add(Tempseries);
                tmp.Series.Add(Humidseries);

                OpvSmartHome.Model = tmp;
                LblTotalCount.Content = $"검색데이터 {TotalDataCount} 개";
        }
    }
}

