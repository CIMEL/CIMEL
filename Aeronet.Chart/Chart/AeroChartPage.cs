using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aeronet.Chart.Chart
{
    public class AeroChartPage:TabPage
    {
        private AeroChart _aeroChart=new AeroChart();

        public AeroChartPage():base()
        {
            this._aeroChart.Dock = DockStyle.Fill;
            this._aeroChart.Name = "chtAeroChart";
            this.Controls.Add(this._aeroChart);
        }

        public string DataConfigFile
        {
            get { return this._aeroChart.DataConfigFile; }
            set { this._aeroChart.DataConfigFile = value; }
        }

        public void Draw(int year, int month, int day)
        {
            this._aeroChart.Draw(year,month,day);
        }

        public void Init()
        {
            this._aeroChart.Init();
        }

        public void Disable()
        {
            this.Enabled = false;
            this._aeroChart.Disable();
        }

        public void Enable()
        {
            this.Enabled = true;
            this._aeroChart.Enable();
        }
    }
}
