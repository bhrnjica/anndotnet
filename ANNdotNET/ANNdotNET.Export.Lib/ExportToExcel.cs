//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool                                                  //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                       //
//////////////////////////////////////////////////////////////////////////////////////////
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANNdotNET.Export.Lib
{
    public static class ExportToExcel
    {
        public static void Export(float[][] dataTrain, float[][] dataTest, string strFilePath, string excelFormula)
        {
            try
            {
                //
                var ace = new AlphaCharEnum();
                var wb = new XLWorkbook();
                var ws1 = wb.Worksheets.Add("TRAINING DATA");
                var ws2 = wb.Worksheets.Add("TESTING DATA");
                ws1.Cell(1, 1).Value = "Training Data";
                if (dataTest != null)
                    ws2.Cell(1, 1).Value = "Testing Data";

                writeDataToExcel(dataTrain,ws1);

                if (dataTest != null)
                    writeDataToExcel(dataTest, ws2);

                var ind = ace.AlphabetFromIndex(dataTrain[0].Length-1)+"3";
                ws1.Cell(3, dataTrain[0].Length + 1).Value = string.Format(excelFormula, "A3", ind);
                if (dataTest != null)
                    ws2.Cell(3, dataTrain[0].Length + 1).Value = string.Format(excelFormula, "A3", ind);
                //
                wb.SaveAs(strFilePath, false);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Write data set to excel worksheet
        /// </summary>
        /// <param name="experiment"></param>
        /// <param name="ws"></param>
        /// <param name="isTest"></param>
        private static void writeDataToExcel(float[][] data, IXLWorksheet ws)
        {
            //TITLE
            ws.Range("A1", "D1").Style.Font.Bold = true;
            ws.Range("A1", "D1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            if (data == null || data.Length == 0)
                return;
            //get input parameter column
            var colCount = data[0].Length;
            int cellIndex = 1;//starting with offset of 2 cells  
            //Input variable names
            for (int j = 0; j < data[0].Length; j++)
            {
                if(j+1== data[0].Length)
                    ws.Cell(2, cellIndex).Value = $"Y{j + 1}";
                else
                    ws.Cell(2, cellIndex).Value = $"X{j + 1}";

                cellIndex++;
            }
            //one more for Model 
            ws.Cell(2, cellIndex).Value = $"ModelOutput";

           
            //data
            for (int i = 0; i < data.Length; i++)
            {
                cellIndex = 1;
                for (int j = 0; j < data[i].Length; j++)
                {
                    ws.Cell(3+i, cellIndex).Value = data[i][j];
                    cellIndex++;
                }
            }

        }


    }
}
