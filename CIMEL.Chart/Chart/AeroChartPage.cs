using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CIMEL.Core;

namespace CIMEL.Chart
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

        public DataConfigFile DataConfigFile
        {
            get { return this._aeroChart.DataConfigFile; }
            set { this._aeroChart.DataConfigFile = value; }
        }

        public string DataFolder
        {
            get { return this._aeroChart.DataFolder; }
            set { this._aeroChart.DataFolder = value; }
        }

        public override Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
                this._aeroChart.Font = value;
            }
        }

        private string _text=null;
        /// <summary>
        /// The chart name displaying on the tab 
        /// </summary>
        public override string Text
        {
            get
            {
                string text;
                if (this.DataConfigFile != null)
                {
                    text = this.DataConfigFile.Name;
                    if (text == @"AE_AAE")
                        text = @"AE/AAE";
                }
                else
                    text = base.Text;

                return text;
            }
        }

        /// <summary>
        /// Draw CIMEL chart
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        public void Draw(int year, int month, int day)
        {
            this._aeroChart.Draw(year,month,day);
        }

        /// <summary>
        /// Initial chart properties
        /// </summary>
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
