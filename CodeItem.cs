using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Solus;
using System.Drawing;

namespace MetaphysicsIndustries.Ligra
{
    public class CodeItem : TextItem
    {
        public override string Text
        {
            get
            {
                return
                    "Vector data = LoadSignalData(\"data1.txt\");\r\n" +
                    "\r\n" +
                    "//find the fft\r\n" +
                    "VectorFilter fft = new FourierTransformVectorFilter();\r\n" +
                    "Vector frequency = fft.Apply(data);\r\n" +
                    "\r\n" +
                    "//create an averaging filter with a window size of 5\r\n" +
                    "VectorFilter mavf = new MovingAverageVectorFilter(5);\r\n" +
                    "Vector average = mavf.Apply(data);\r\n" +
                    "\r\n" +
                    "Vector data2 = LoadSignalData(\"data2.txt\");\r\n" +
                    "\r\n" +
                    "//convolution\r\n" +
                    "Vector conv = data.Convolution(data2);\r\n" +
                    "\r\n" +
                    "Matrix tank = LoadImage(\"tank.bmp\");\r\n" +
                    "//5% salt & pepper noise\r\n" +
                    "MatrixFilter saltpepper = new SaltAndPepperMatrixNoise(5);\r\n" +
                    "Matrix noisy = saltpepper.Apply(tank);\r\n" +
                    "\r\n" +
                    "MatrixFilter arith = new ArithmeticMeanFilter(3);\r\n" +
                    "Matrix arithTank = arith.Apply(noisy);\r\n" +
                    "\r\n" +
                    "MatrixFilter med = new MedianMatrixFilter(7);\r\n" +
                    "MatrixFilter medianTank = med.Apply(tank);\r\n";
            }
        }

        public  override Font GetFont(LigraControl control)
        {
            return control.Font;
        }
    }
}
