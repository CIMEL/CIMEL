using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CIMEL.Chart.Chart;
using CIMEL.Core;

namespace CIMEL.Chart
{
    public class AeroChartPage:TabPage
    {
        private AeroChart _aeroChart = new AeroChart();
        private ThreeDChart _pb3DChart = new ThreeDChart();
        private FigureType _figureType;
        private DataConfigFile _dataConfigFile;
        private string _dataFolder;

        public AeroChartPage():base()
        {
            this._aeroChart.Dock = DockStyle.Fill;
            this._aeroChart.Name = "chtAeroChart";
            this._pb3DChart.Dock = DockStyle.Fill;
            this._pb3DChart.Name = "pb3d";

            this.Controls.Add(this._aeroChart);
            this.Controls.Add(this._pb3DChart);
        }

        public DataConfigFile DataConfigFile
        {
            get { return this._dataConfigFile; } //this._aeroChart.DataConfigFile; }
            set { this._dataConfigFile = value; }//this._aeroChart.DataConfigFile = value; }
        }

        public string DataFolder
        {
            get { return this._dataFolder; } //return this._aeroChart.DataFolder; }
            set { this._dataFolder = value; }//this._aeroChart.DataFolder = value; }
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

        public void Draw(int year, int month, int[] days) {
            this._pb3DChart.Draw(year, month, days);
        }


        /// <summary>
        /// Initial chart properties
        /// </summary>
        public void Init(FigureType figureType)
        {
            this._figureType = figureType;

            if (figureType == FigureType.TwoD)
            {
                this._aeroChart.Visible = true;
                this._pb3DChart.Visible = false;
                this._aeroChart.DataConfigFile = this._dataConfigFile;
                this._aeroChart.DataFolder = this._dataFolder;
                this._aeroChart.Init();
            }
            else
            {
                this._aeroChart.Visible = false;
                this._pb3DChart.Visible = true;
                this._pb3DChart.DataConfigFile = this._dataConfigFile;
                this._pb3DChart.DataFolder = this._dataFolder;
                this._pb3DChart.Init(figureType);
            }
        }

        public void Disable()
        {
            this.Enabled = false;
            this._aeroChart.Disable();
            this._pb3DChart.Disable();
        }

        public void Enable()
        {
            this.Enabled = true;
            this._aeroChart.Enable();
            this._pb3DChart.Enable();
        }
    }
}
