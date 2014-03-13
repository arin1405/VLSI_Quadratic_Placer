using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace set_TestField
{
    class Program
    {
        static void Main(string[] args)
        {
            int counter = 0, t1, NumberofGates = 2, NumberofNets = 2, NumberofPads = 1;
            string line;

            // Read the file and display it line by line.
            System.IO.StreamReader file1 = new System.IO.StreamReader("G:\\placer\\circuit1");
            StreamWriter sw1 = new StreamWriter(@"G:\placer\netToGate.txt");
            StreamWriter sw2 = new StreamWriter(@"G:\placer\padToNet.txt");
            StreamWriter sw3 = new StreamWriter(@"G:\placer\padXY.txt");
            List<padXY> pxy = new List<padXY>();

            try
            {
                 while ((line = file1.ReadLine()) != null)
                {
                    //we will use a temp matrix for all matrix activities inside while loop. T_ means temp 
                    if (counter == 0)
                    {
                        string[] lineWords = line.Split(new[] { ' ' });
                        //for (int i = 0; i < 2; i++)
                        //  Console.WriteLine(lineWords[i]);
                        NumberofGates = int.Parse(lineWords[0]);
                        NumberofNets = int.Parse(lineWords[1]);
                    }

                    int[,] T_netToGate = new int[(NumberofNets + 1), (NumberofGates + 1)];//T_netToGate is assigned dimensions

                    if (counter > 0 && counter <= NumberofGates)
                    {
                        string[] lineWords1 = line.Split(new[] { ' ' });
                        int gateId = int.Parse(lineWords1[0]);
                        int connectedNetsNo = int.Parse(lineWords1[1]);
                        for (int k = 2; k <= (1 + connectedNetsNo); k++)
                        {
                            t1 = int.Parse(lineWords1[k]);
                            T_netToGate[t1, gateId] = 1;
                            sw1.WriteLine(t1.ToString() + " " + gateId.ToString());
                        }
                    }

                    if (counter == (NumberofGates + 1))
                    {
                        string[] lineWords2 = line.Split(new[] { ' ' });
                        NumberofPads = int.Parse(lineWords2[0]);
                    }
                    if (counter > (NumberofGates + 1))
                    {
                        int t = NumberofPads + 1;
                        int[] T_padToNet = new int[t];
                        //1-D array, padToNet[i] = x means 'i'th pad is connected to x net,+1 to make the starting address arr[1] instead of arr[0]
                        //double[] T_padX = new double[t];
                        //double[] T_padY = new double[t];
                        //1-d array - padX[i] = 25 means pad no. i is having 25 coordinte in X axis

                        string[] lineWords3 = line.Split(new[] { ' ' });
                        T_padToNet[int.Parse(lineWords3[0])] = int.Parse(lineWords3[1]);
                        sw2.WriteLine((lineWords3[0]) + " " + lineWords3[1]);

                        //T_padX[int.Parse(lineWords3[0])] = double.Parse(lineWords3[2]);
                        //T_padY[int.Parse(lineWords3[0])] = double.Parse(lineWords3[3]);
                        //sw3.WriteLine(lineWords3[0] + " " + lineWords3[2] + " " + lineWords3[3]); 

                        /* instead of writing Pxy in a file, we are storing it in a List*/
                        pxy.Add(new padXY() { padID = int.Parse(lineWords3[0]), padX = double.Parse(lineWords3[2]), padY = double.Parse(lineWords3[3])});
                    }

                    counter++;
                }//end of while loop


                file1.Close();
                sw1.Close();
                sw2.Close();
                sw3.Close();

                //2nd day: netToGate, gateToGate and cost matrix generation
                /* if memory leakage problem comes for netToGate matrix, we can write sparse matrix of this matrix into a file
                 * the sparse matrix will be in the form of "for a sigle net, what are the gates connected
                 * 3 2
                 * 3 4
                 * 3 6 -->> net 3 is connected to gate 2, 4, 6
                 * 2 5
                 * 2 7 -->> net 2 is connected to gate 5, 7
                 * have to make a 1-D array of nets (LHS entries as shown above), will have to sort that array*/

                int[,] netToGate = new int[NumberofNets + 1, NumberofGates + 1];

                //gateToGate sparse matrix in a file
                StreamWriter sw4 = new StreamWriter(@"G:\placer\gateToGate.txt");
                StreamWriter sw5 = new StreamWriter(@"G:\placer\costMatrix_C.txt");

                System.IO.StreamReader file2 = new System.IO.StreamReader("G:\\placer\\netToGate.txt");

                while ((line = file2.ReadLine()) != null)
                {
                    string[] lineWords = line.Split(new[] { ' ' });
                    netToGate[int.Parse(lineWords[0]), int.Parse(lineWords[1])] = 1;  //netToGate matrix holds relation between net and gates.                   
                }
                file2.Close();

                int fixedVar = 0;
                for (int a = 1; a <= (NumberofNets + 0); a++)
                    for (int b = 1; b <= (NumberofGates + 0); b++)
                        if (netToGate[a, b] == 1)
                        {
                            fixedVar = b;
                            for (int c = (b + 1); c <= (NumberofGates + 0); c++)
                                if (netToGate[a, c] == 1)
                                {
                                    sw4.WriteLine(fixedVar + " " + c);
                                    //sw4.WriteLine(c + " " + fixedVar); 
                                    /*this above line is commented because cost matrix 'C' (gateToGate) is a symmetric matrix
                                     * so each line indicates that there is an implicit next line having just opposite combination
                                     * for ex: if a line is wriiten as 4 5 that means next implicit line is 5 4
                                     * this 5 4 line, we are ommitting*/
                                }

                        }
                sw4.Close();
                System.IO.StreamReader file3 = new System.IO.StreamReader("G:\\placer\\gateToGate.txt");

                int noOfGatesInANet, noOfEdgeInAClique, count, noOfPadsInANet;
                int[,] netToPad = new int[NumberofNets + 1, NumberofPads + 1];
                System.IO.StreamReader file4 = new System.IO.StreamReader("G:\\placer\\padToNet.txt");
                while ((line = file4.ReadLine()) != null)
                {
                    string[] lineWords = line.Split(new[] { ' ' });
                    netToPad[int.Parse(lineWords[1]), int.Parse(lineWords[0])] = 1;  //netToGate matrix holds relation between net and gates.                   
                }
                file4.Close();

                //double[] costClique = new double[25]; made for debugging
                mathClass mc = new mathClass();
                double costOfCliqueEdge = 0;
                int padCount = 0, flagA = 0, flagB = 0;

                List<matrixAdiagonal> mAd = new List<matrixAdiagonal>();
                List<Bxy> bxy = new List<Bxy>();

                for (int i = 1; i <= NumberofGates; i++)
                {
                    mAd.Add(new matrixAdiagonal() { gateNo = i, diagCost = 0 });
                    bxy.Add(new Bxy() { gateID = i, Bx = 0, By = 0 });
                }
                

                for (int a = 1; a <= (NumberofNets + 0); a++)
                {
                    noOfGatesInANet = count = noOfPadsInANet = padCount= 0;
                    for (int b = 1; b <= (NumberofGates + 0); b++)
                    {
                        if (netToGate[a, b] == 1)
                        {
                            noOfGatesInANet++;
                        }
                    }
                    for (int d = 1; d <= (NumberofPads + 0); d++)
                    {
                        if (netToPad[a, d] == 1)
                        {
                            noOfPadsInANet++;
                        }
                    }
                    noOfEdgeInAClique = mc.oneToNumSum(noOfGatesInANet - 1);
                    if (noOfGatesInANet > 1 && count == 0)
                    {
                        costOfCliqueEdge = (1 / (double)((noOfGatesInANet + noOfPadsInANet) - 1));//formula of 1/(k-1) for k points clique;k = noOfGatesInANet + noOfPadsInANet
                        //while ((line = file3.ReadLine()) != null)
                        while (count < noOfEdgeInAClique)
                        {
                            line = file3.ReadLine();
                            sw5.WriteLine(line + " " + costOfCliqueEdge.ToString());
                            //costClique[count] = costOfCliqueEdge;   ---for debugging---
                            //Console.WriteLine(costClique[count]);   --- """" ----------
                            count++;
                        }
                    }
                        while (padCount < noOfPadsInANet)
                        {
                            padCount++;
                            for (int b = 1; b <= (NumberofGates + 0); b++)
                            {
                                if (netToGate[a, b] == 1)
                                {
                                    foreach (matrixAdiagonal md in mAd)
                                    {
                                        //Console.WriteLine(mat.coordX.ToString() + " " + mat.coordY.ToString() + " " + mat.cost.ToString());
                                        if (md.gateNo == b)
                                        {
                                            md.diagCost = md.diagCost + costOfCliqueEdge;
                                            flagA = 1;
                                        }
                                    }

                                    /*if (flagA == 0)
                                        mAd.Add(new matrixAdiagonal() { gateNo = b, diagCost = costOfCliqueEdge });*/

                                    for (int d = 1; d <= (NumberofPads + 0); d++)
                                    {
                                        if (netToPad[a, d] == 1)
                                        {
                                            foreach (Bxy b1 in bxy)
                                            {
                                                //Console.WriteLine(mat.coordX.ToString() + " " + mat.coordY.ToString() + " " + mat.cost.ToString());
                                                if (b1.gateID == b)
                                                {
                                                    b1.Bx = b1.Bx + (pxy[d-1].padX * costOfCliqueEdge);//to be checked and changed
                                                    b1.By = b1.By + (pxy[d-1].padY * costOfCliqueEdge);
                                                    flagB = 1;
                                                }
                                            }

                                            /*if (flagB == 0)
                                                bxy.Add(new Bxy() { gateID = b, Bx = (pxy[d-1].padX * costOfCliqueEdge), By = (pxy[d-1].padY * costOfCliqueEdge)});*/
                                        }
                                    }
                                    
                                    flagA = flagB = 0;
                                }
                            }
                        }
                }


                sw5.Close();
                file2.Close();
                file3.Close();
                //making matrix 'A'

                System.IO.StreamReader file5 = new System.IO.StreamReader("G:\\placer\\costMatrix_C.txt");
              

                //double[,] matA = new double[NumberofGates + 1, NumberofGates + 1];
                int flag = 0, row, col;
                double value;
                List<matrixA> mA = new List<matrixA>();
                while ((line = file5.ReadLine()) != null) // its going out of memory, so excluding the matrix we will write value in a file as a sparse matrix
                {
                    flag = 0;
                    string[] lineWords = line.Split(new[] { ' ' });
                    row = int.Parse(lineWords[0]);
                    col = int.Parse(lineWords[1]);
                    value = double.Parse(lineWords[2]);
                    //matA[col, row] = matA[row, col] =  matA[row, col] + value;

                    foreach (matrixA mat in mA)
                    {
                        //Console.WriteLine(mat.coordX.ToString() + " " + mat.coordY.ToString() + " " + mat.cost.ToString());
                        if (mat.coordX == row && mat.coordY == col)
                        {
                            mat.cost = mat.cost + value;
                            flag = 1;
                        }
                    }

                    if (flag == 0)
                        mA.Add(new matrixA() { coordX = row, coordY = col, cost = value });

                }
                file5.Close();
                //sw6.Close();

                //now making a complete matrix A: for (Ai Bi cost) the next line would be (Bi Ai cost)
                int temCount = mA.Count;
                matrixA mm = new matrixA();
                for (int i = 0; i < temCount; i++)
                {
                    row = mA[i].coordX;
                    col = mA[i].coordY;
                    value = mA[i].cost;
                    mA.Add(new matrixA() { coordX = col, coordY = row, cost = value });
                }

                foreach (matrixAdiagonal md in mAd)
                {
                    foreach (matrixA mat in mA)
                    {
                        if (mat.coordX == md.gateNo)
                        {
                            md.diagCost = md.diagCost + mat.cost;
                        }
                    }
                }
                //just debug
                /*for (int a = 1; a <= (NumberofGates + 0); a++)
                {
                    for (int b = 1; b <= (NumberofGates + 0); b++)
                    {
                        Console.Write(matA[a,b]+" ");
                    }
                    Console.WriteLine();
                }*/
                
                 /*foreach (matrixA mat in mA)
                 {
                     Console.WriteLine(mat.coordX.ToString() + " " + mat.coordY.ToString() + " " + mat.cost.ToString());
                 }
                 Console.WriteLine("line number:"+ mA.Count.ToString());*/
                /*foreach (matrixAdiagonal md in mAd)
                {
                    Console.WriteLine(md.gateNo.ToString()+" "+md.diagCost.ToString());
                }*/
                /*foreach (padXY pp in pxy)
                {
                    Console.WriteLine(pp.padID.ToString() + " " + pp.padX.ToString()+" "+pp.padY.ToString());
                }*/

            

                StreamWriter matHelm = new StreamWriter(@"G:\placer\psd.txt");
                StreamWriter bxTxt = new StreamWriter(@"G:\placer\b.txt");
                StreamWriter byTxt = new StreamWriter(@"G:\placer\b2.txt");
                 matHelm.WriteLine(NumberofGates.ToString() + " " + ((mA.Count + NumberofGates).ToString()));
                 
                 foreach (matrixA mat in mA)
                 {
                     matHelm.WriteLine((mat.coordX - 1).ToString() + " " + (mat.coordY - 1).ToString() + " " + (mat.cost * -1).ToString());
                 }
                 foreach (matrixAdiagonal md in mAd)
                 {
                     matHelm.WriteLine((md.gateNo - 1).ToString() + " " + (md.gateNo - 1).ToString() + " " + md.diagCost.ToString());
                 }
                 foreach (Bxy b1 in bxy)
                 {
                     bxTxt.Write(b1.Bx.ToString()+"\n ");
                     byTxt.Write(b1.By.ToString()+"\n ");
                 }

                 matHelm.Close();
                 bxTxt.Close();
                 byTxt.Close();
                 //pad.Close();
                 Console.Write("Success " + counter);
                 System.Diagnostics.Process.Start(@"G:\placer\cmd_line_execution.bat");
            }
            catch (Exception e)
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(e, true);
                Console.WriteLine(trace.GetFrame(0).GetMethod().ReflectedType.FullName);
                Console.WriteLine("Line: " + trace.GetFrame(0).GetFileLineNumber());
                Console.WriteLine("Column: " + trace.GetFrame(0).GetFileColumnNumber());
                Console.WriteLine(e.Message);
            }
        }
    }
}
