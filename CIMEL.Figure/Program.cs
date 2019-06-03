using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CIMELDraw;
using MathWorks.MATLAB.NET.Arrays;

namespace CIMEL.Figure
{
    class Program
    {
        static void Main(string[] args)
        {
            // draw aeronent inversion
            Drawing drawing = new Drawing();

            try
            {
                //// 1 calculate Matrix of aeronent
                //OnInformed("Drawing multiple lines");
                //if (args == null || args.Length < 3)
                //    throw new ArgumentException("Missing arguments!\r\ndraw [inputPath] [outputfile] [lat|lon]");
                //string mwInput = args[0];
                //string mwOutput = args[1];
                //string location = args[2];
                //string[] arrLocation = location.Split(new char[] { '|' }, StringSplitOptions.None);
                //if (arrLocation.Length < 2)
                //    throw new ArgumentException("invalid [location]!\r\n[location]= \"[lat|lon]\"");

                //// get lat and lon of region
                //double lat = ToDouble(arrLocation[0]);//region.Lat;
                //double lon = ToDouble(arrLocation[1]);//region.Lon;
                //                                      /*
                //                                      object[] results = 
                //                                    
                string[] strOptions = new string[] { "440", "550", "675", "870", "1020" };
                MWCellArray dataOptions = new MWCellArray(new MWCharArray(strOptions));

                MWCharArray strTitle = new MWCharArray("AAOD");

                string[] strDates = new string[] { "2016-12-1", "2016-12-2" };
                MWCellArray arrDates = new MWCellArray(new MWCharArray(strDates));
                //{{'1:2:44';'1:18:35';'5:19:29'};{'1:4:1';'1:19:25';'3:19:37';'5:20:41';'6:19:5';'7:18:56';'7:39:18'}};
                string[] strTimes=new string[]{"1:2:44","1:18:35", "5:19:29"};

                MWCellArray arrTimes = new MWCellArray(2);
                arrTimes[1] = new MWCellArray( new MWCharArray(new string[] { "1:2:44", "1:18:35", "5:19:29" }));
                arrTimes[2]= new MWCellArray(new MWCharArray(new string[] { "1:4:1", "1:19:25" ,"3:19:37", "5:20:41","6:19:5", "7:18:56", "7:39:18" }));

                MWArray result = drawing.DrawMultiplelines(strTitle, dataOptions, arrDates, arrTimes, new MWLogicalArray(true));
                string file = result.ToString();
            }
            catch (Exception ex)
            {
                OnFailed(ex.Message);
            }
            finally
            {
                drawing.Dispose();
            }
        }

        /// <summary>
        /// put the error message to error stream
        /// </summary>
        /// <param name="error"></param>
        private static void OnFailed(string error)
        {
            Console.Error.WriteLine(error);
        }

        /// <summary>
        /// put the info message to std output stream
        /// </summary>
        /// <param name="info"></param>
        private static void OnInformed(string info)
        {
            Console.Out.WriteLine(info);
        }

        private static double ToDouble(string value)
        {
            double result;
            if (!double.TryParse(value, out result))
                result = 0f;
            return result;
        }
    }
}
