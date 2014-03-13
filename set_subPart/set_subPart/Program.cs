using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace set_subPart
{
    class Program
    {
        static void Main(string[] args)
        {
            int NumberofGates = 18, gateCount = 1, id = 0;
            string line;
            StreamReader file1 = new System.IO.StreamReader(@"G:\placer\checkMe.txt");
            StreamWriter final = new StreamWriter(@"G:\placer\FINAL.txt");
            List<gateXY> gxy = new List<gateXY>();
            List<gateCalculation> gc = new List<gateCalculation>();

            while ((line = file1.ReadLine()) != null)
            {
                if (gateCount <= 6417)
                    gxy.Add(new gateXY() { gateID = gateCount, gateX = double.Parse(line), gateY =0 });

                else
                {
                    gxy[id].gateY = gxy[id].gateY + double.Parse(line);
                    id++;
                }
                gateCount++;                
            }
            foreach (gateXY gg in gxy)
            {
                gc.Add(new gateCalculation() { gateID = gg.gateID, get_calc = ((100000 * gg.gateX) + gg.gateY) });
            }
            double T_get_calc=0;
            int T_gateId = 0;
            ///sorting of the values 
            for(int i = 0; i<NumberofGates;i++)
                for (int j = 0; j < i; j++)
                {
                    if (gc[i].get_calc > gc[j].get_calc)
                    {
                        T_get_calc = gc[i].get_calc;
                        T_gateId = gc[i].gateID;
                        gc[i].get_calc = gc[j].get_calc;
                        gc[i].gateID = gc[j].gateID;
                        gc[j].get_calc = T_get_calc;
                        gc[j].gateID = T_gateId;
                    }
                }
            gc.Reverse();
            /*foreach (gateCalculation gcc in gc)
            {
                Console.WriteLine(gcc.gateID.ToString()+" "+gcc.get_calc.ToString());
            }*/
            foreach (gateXY gg in gxy)
            {
                final.WriteLine(gg.gateID.ToString()+" "+gg.gateX.ToString()+" "+gg.gateY.ToString());
            }
            final.Close();
            file1.Close();
 
        }
    }
}
