using Aeronet.Core;
using AeronetDrawNative;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aeronet.Draw
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // draw aeronent inversion
            Drawing drawing = new Drawing();
            string STNS_FN = args[0];
            string STNS_ID = args[1];

            try
            {
                OnInformed("***************************************************************");
                string inputbase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output", STNS_FN) + Path.DirectorySeparatorChar;
                string outputbase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "cimel_network", STNS_FN,
                        "dubovik") + Path.DirectorySeparatorChar;
                string outputfile = Path.Combine(outputbase,
                    String.Format("Dubovik_stats_{0}_{1}_{2:yyyyMMdd}.dat", STNS_FN, STNS_ID, DateTime.Now));

                if (!Directory.Exists(outputbase))
                    Directory.CreateDirectory(outputbase);

                // 1 calculate Matrix of aeronent
                OnInformed("Calculating Aeronet inversion Matrix");
                string mwInput = inputbase;
                string mwOutput = outputfile;
                // get lat and lon of region
                Region region = RegionStore.Singleton.FindRegion(STNS_FN);

                double lat = region.Lat;
                double lon = region.Lon;
                OnInformed("\tARGUMENTS:");
                OnInformed(String.Format("\t{0} : {1}", "INPUT", mwInput));
                OnInformed(String.Format("\t{0} : {1}", "OUTPUT", mwOutput));
                object[] results = drawing.MatrixAeronet(2, lat, lon, mwInput, mwOutput);
                var stats_inversion = results[0];
                var r = results[1];

                OnInformed("stats_inversions");
                PrintMatrix((double[,])stats_inversion, OnInformed);
                OnInformed("r");
                PrintMatrix((double[,])r, OnInformed);
                OnInformed("DONE to Calculate Aeronet inversion Matrix");

                // 2 draw SSA
                OnInformed("Drawing SSA figures");
                // MWArray mwYear = new MWNumericArray(new int[] {2013});
                // MWArray mwOuputbase = new MWCharArray(new string[]{ outputbase});
                double mwYear = 2013;
                string mwOuputbase = outputbase;
                string mwRegion = STNS_FN;

                OnInformed("\tARGUMENTS:");
                OnInformed(String.Format("\t{0} : {1}", "YEAR", mwYear));
                OnInformed(String.Format("\t{0} : {1}", "OUTPUT", mwOuputbase));
                drawing.DrawSSA(stats_inversion, r, mwYear, mwRegion, mwOuputbase);
                drawing.WaitForFiguresToDie();
                OnInformed("DONE to draw SSA figures");

                // 3 draw SSA Statistic
                OnInformed("Drawing SSA Statistic figures");
                // MWArray mwRegion = new MWCharArray(new string[]{ STNS_FN});
                OnInformed("\tARGUMENTS:");
                OnInformed(String.Format("\t{0} : {1}", "YEAR", mwYear));
                OnInformed(String.Format("\t{0} : {1}", "REGION", mwRegion));
                OnInformed(String.Format("\t{0} : {1}", "OUTPUT", mwOuputbase));
                drawing.DrawSSAStatistisc(stats_inversion, r, mwYear, mwRegion, mwOuputbase);
                drawing.WaitForFiguresToDie();
                OnInformed("DONE to draw SSA Statistic figures");

                // 4 draw Aeronet Inversions
                OnInformed("Drawing Aeronet Inversions figures");
                OnInformed("\tARGUMENTS:");
                OnInformed(String.Format("\t{0} : {1}", "OUTPUT", mwOuputbase));
                drawing.DrawAeronetInversions(stats_inversion, r, mwOuputbase);
                drawing.WaitForFiguresToDie();
                OnInformed("DONE to drawing Aeronet Inversions figures");
            }
            catch (Exception ex)
            {
                OnFailed(ex.Message);
                success = false;
            }
            finally
            {
                OnInformed("***************************************************************");
                drawing.Dispose();
            }
        }

        public static void PrintMatrix(double[,] arrary, Action<string, bool> log)
        {
            int d1 = arrary.GetLength(0);
            int d2 = arrary.GetLength(1);
            for (int i = 0; i < d1; i++)
            {
                string line = String.Empty;
                for (int j = 0; j < d2; j++)
                {
                    line += arrary[i, j] + " ";
                }
                log.Invoke(line, true);
            }
        }
    }
}