using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MetaphysicsIndustries.Solus;
//using MetaphysicsIndustries.Sandbox;
using System.Drawing.Printing;
using MetaphysicsIndustries.Collections;
using System.IO;
using MetaphysicsIndustries.Acuity;


namespace MetaphysicsIndustries.Ligra
{
    public partial class LigraForm : Form
    {
        private void MvCommand(string input, string[] args)
        {
            Font font = ligraControl1.Font;
            //System.IO.Directory.SetCurrentDirectory("C:\\data\\images\\photos\\contour for machine vision");
            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images");

            RenderItemContainer ric = new RenderItemContainer("Machine Vision");
            _renderItems.Add(ric);

            string filename = "tank256g.bmp";

            PrewittHorizontalMatrixFilter hf = new PrewittHorizontalMatrixFilter();
            PrewittVerticalMatrixFilter vf = new PrewittVerticalMatrixFilter();

            Matrix image = LoadImageForFilters(filename, "Original Image", ric, true);
            Matrix imageH = ApplyFilter(image, hf, "Prewitt H", ric, AcuityEngine.ConvertNegOneOneToZeroOne, null, false, true);
            Matrix imageV = ApplyFilter(image, vf, "Prewitt V", ric, AcuityEngine.ConvertNegOneOneToZeroOne, null, false, true);
            ric.Items.Add(new SpacerItem(256, 256));
            Matrix imageHH = ApplyFilter(imageH, hf, "HH", ric, AcuityEngine.ConvertNegOneOneToZeroOne, null, false, true);
            Matrix imageHV = ApplyFilter(imageH, vf, "HV", ric, AcuityEngine.ConvertNegOneOneToZeroOne, null, false, true);
            Matrix imageVH = ApplyFilter(imageV, hf, "VH", ric, AcuityEngine.ConvertNegOneOneToZeroOne, null, false, true);
            Matrix imageVV = ApplyFilter(imageV, vf, "VV", ric, AcuityEngine.ConvertNegOneOneToZeroOne, null, false, true);
        }

        private void Project3Test1Command(string input, string[] args)
        {
            Font font = ligraControl1.Font;
            //System.IO.Directory.SetCurrentDirectory("C:\\data\\images\\photos\\contour for machine vision");
            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images");

            RenderItemContainer ric = new RenderItemContainer("p3t1");
            _renderItems.Add(ric);

            string filename = "lena256g.bmp";

            if (args.Length > 1)
            {
                filename = args[1].Trim('\"', '\'');
            }

            MmseEdgeDetectionMatrixfilter mmseedFilter = new MmseEdgeDetectionMatrixfilter(7, 0.025f, 4);
            DualBellEdgeDetectorMatrixFilter dbedFilter = new DualBellEdgeDetectorMatrixFilter(3);
            ZetaTrimmedMeanMatrixFilter ztmFilter = new ZetaTrimmedMeanMatrixFilter(7, 2);
            IntervalFitMatrixFilter intervalFilter = new IntervalFitMatrixFilter();
            ExponentMatrixFilter expoFilter = new ExponentMatrixFilter(0.85f);

            Matrix image = LoadImageForFilters(filename, "Original Image", ric, true);
            //Matrix mmseed = ApplyFilter(image, mmseedFilter, "MMSE Edge Detect", ric, null, null, false, true);
            Matrix dbed;// = ApplyFilter(image, osedFilter, "Order Statistic, w=5, g=1", ric, null, null, false);

            //osedFilter.Gamma = 1.5;
            //osed = ApplyFilter(image, osedFilter, "Order Statistic, w=5, g=1.5", ric, null, null, false);

            //osedFilter.Gamma = 2;
            //osed = ApplyFilter(image, osedFilter, "Order Statistic, w=5, g=2", ric, null, null, false);

            //osedFilter.Gamma = 0.5;
            dbed = ApplyFilter(image, dbedFilter, "Dual Bell, w=3, g=1", ric, null, null, false, true);
            dbed = ApplyFilter(dbed, intervalFilter, "Interval Fitted", ric, null, null, false, true);
            //dbed = ApplyFilter(dbed, expoFilter, "Exponentiated, g=1.5", ric, null, null, false, true);

            //osedFilter.Gamma = 0.75;
            //osed = ApplyFilter(image, osedFilter, "Order Statistic, w=5, g=0.75", ric, null, null, false);

            SaveImageForFilters("..\\project 3\\dual_bell.bmp", dbed);

            //Matrix ztm = ApplyFilter(image, ztmFilter, "Zeta trimmed", ric, null, null, false);

            //OrderStatisticEdgeDetectorMatrixFilter osedFilter2 = new OrderStatisticEdgeDetectorMatrixFilter(3, 0.5);

            //osed = ApplyFilter(image, osedFilter2, "Order Statistic, w=3, g=0.5", ric, null, null, false);
            //osed = ApplyFilter(osed, intervalFilter, "Interval Fitted", ric, null, null, false);

            //ApplyFilter(image, new PrewittHorizontalMatrixFilter(), "Prewitt H", ric, null, null, false);
            //ApplyFilter(image, new PrewittVerticalMatrixFilter(), "Prewitt V", ric, null, null, false);
            Matrix sobel = ApplyFilter(image, new SobelMatrixFilter(), "Sobel", ric, null, null, false, true);
            sobel = ApplyFilter(sobel, intervalFilter, "IntervalFitted", ric, null, null, false, true);
            SaveImageForFilters("..\\project 3\\sobel.bmp", sobel);
        }
        private void Project3Test2Command(string input, string[] args)
        {
            Font font = ligraControl1.Font;
            //System.IO.Directory.SetCurrentDirectory("C:\\data\\images\\photos\\contour for machine vision");
            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images");

            int time = System.Environment.TickCount;

            RenderItemContainer ric = new RenderItemContainer("p3t2");
            _renderItems.Add(ric);

            string filename = "lena256g.bmp";

            if (args.Length > 1)
            {
                filename = args[1].Trim('\"', '\'');
            }

            DualBellEdgeDetectorMatrixFilter dbedFilter = new DualBellEdgeDetectorMatrixFilter(3);
            //IntervalFitBaseMatrixFilter intervalFilter = new IntervalFitBaseMatrixFilter();

            Matrix image = LoadImageForFilters(filename, "Original Image", ric, true);

            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\project 3\\");

            Matrix temp = dbedFilter.Apply(image);
            Pair<float> dbedInterval = IntervalFitMatrixFilter.CalcInterval(temp);
            Matrix dbedImage = IntervalFitBaseMatrixFilter.IntervalFit(temp, dbedInterval.First, dbedInterval.Second);
            SaveImageForFilters("dual_bell_image.bmp", dbedImage);

            temp = SobelMatrixFilter.GenerateMagnitudeMap(image);
            Pair<float> sobelInterval = IntervalFitMatrixFilter.CalcInterval(temp);
            Matrix sobelImage = IntervalFitBaseMatrixFilter.IntervalFit(temp, sobelInterval.First, sobelInterval.Second);
            SaveImageForFilters("sobel_image.bmp", sobelImage);

            List<List<string>> sobelResults = new List<List<string>>();
            List<List<string>> dbedResults = new List<List<string>>();

            List<string> sobelResultsLine = new List<string>();
            sobelResults.Add(sobelResultsLine);
            sobelResultsLine.Add("p");
            sobelResultsLine.Add("MSE");
            sobelResultsLine.Add("MSSIM");

            List<string> dbedResultsLine = new List<string>();
            dbedResults.Add(dbedResultsLine);
            dbedResultsLine.Add("p");
            dbedResultsLine.Add("w");
            dbedResultsLine.Add("a");
            dbedResultsLine.Add("MSE");
            dbedResultsLine.Add("MSSIM");
            dbedResultsLine.Add("t");

            foreach (float impulseProbability in new float[] { 0.01f, 0.05f, 0.1f, 0.15f, 0.2f, 0.25f })
            {
                ImpulseNoiseMatrixFilter noiseFilter = new ImpulseNoiseMatrixFilter(impulseProbability);
                Matrix noisyImage = noiseFilter.Apply(image);
                SaveImageForFilters("noisy_image_p_" + impulseProbability.ToString("G3") + ".bmp", noisyImage);

                temp = SobelMatrixFilter.GenerateMagnitudeMap(noisyImage);
                Matrix sobelNoiseImage = IntervalFitBaseMatrixFilter.IntervalFit(temp, sobelInterval.First, sobelInterval.Second);
                SaveImageForFilters("sobel_noise_image_p_" + impulseProbability.ToString("G3") + ".bmp", sobelNoiseImage);

                sobelResultsLine = new List<string>();
                sobelResults.Add(sobelResultsLine);
                sobelResultsLine.Add(impulseProbability.ToString());
                sobelResultsLine.Add(AcuityEngine.MeanSquareError(sobelImage, sobelNoiseImage).ToString());
                sobelResultsLine.Add(SsimErrorMeasure.Measure(sobelImage, sobelNoiseImage, 7).ToString());

                float alpha;
                int windowSize;

                for (windowSize = 3; windowSize <= 7; windowSize += 2)
                {
                    for (alpha = 0; alpha < 0.5; alpha += 0.05f)
                    {

                        AlphaTrimmedDualBellEdgeDetectorMatrixFilter dbFilter = new AlphaTrimmedDualBellEdgeDetectorMatrixFilter(alpha, windowSize);
                        int time2 = System.Environment.TickCount;
                        temp = dbFilter.Apply(noisyImage);
                        Matrix dbedNoiseImage = IntervalFitBaseMatrixFilter.IntervalFit(temp, dbedInterval.First, dbedInterval.Second);
                        time2 = System.Environment.TickCount - time2;
                        SaveImageForFilters("dbed_noise_image_p_" + impulseProbability.ToString() + "_w_" + windowSize.ToString() + "_a_" + alpha.ToString() + ".bmp", dbedNoiseImage);

                        dbedResultsLine = new List<string>();
                        dbedResults.Add(dbedResultsLine);
                        dbedResultsLine.Add(impulseProbability.ToString());
                        dbedResultsLine.Add(windowSize.ToString());
                        dbedResultsLine.Add(alpha.ToString());
                        dbedResultsLine.Add(AcuityEngine.MeanSquareError(dbedImage, dbedNoiseImage).ToString());
                        dbedResultsLine.Add(SsimErrorMeasure.Measure(dbedImage, dbedNoiseImage, 7).ToString());
                        dbedResultsLine.Add(time2.ToString());
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter("sobel_results.csv"))
            {
                foreach (List<string> line in sobelResults)
                {
                    sw.WriteLine(string.Join(",", line.ToArray()));
                }
            }

            using (StreamWriter sw = new StreamWriter("dbed_results.csv"))
            {
                foreach (List<string> line in dbedResults)
                {
                    sw.WriteLine(string.Join(",", line.ToArray()));
                }
            }

            MessageBox.Show("Done. Time = " + (System.Environment.TickCount - time).ToString() + "ms");
        }
        private void Project3Test3Command(string input, string[] args)
        {
            Font font = ligraControl1.Font;
            Font f = ligraControl1.Font;
            Pen p = Pens.Blue;
            Expression expr;

            //System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images");

            //int time = Environment.TickCount;

            //RenderItemContainer ric = new RenderItemContainer("p3t3");
            //_renderItems.Add(ric);

            //string filename = "lena256g.bmp";

            //if (exTokens.Length > 1)
            //{
            //    filename = exTokens[1].Token.Trim('\"', '\'');
            //}

            //DualBellEdgeDetectorMatrixFilter dbedFilter = new DualBellEdgeDetectorMatrixFilter(3);
            ////IntervalFitBaseMatrixFilter intervalFilter = new IntervalFitBaseMatrixFilter();

            //Matrix image = LoadImageForFilters(filename, "Original Image", ric);

            //System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\project 3\\");

            if (!_vars.ContainsKey("mu")) _vars.Add("mu", new Literal(0.5f));
            if (!_vars.ContainsKey("sigma")) _vars.Add("sigma", new Literal(0.2f));
            //_renderItems.Add(new ExpressionItem(new AssignExpression(_vars["mu"], new Literal(0.5)), p, f));
            //_renderItems.Add(new ExpressionItem(new AssignExpression(_vars["sigma"], new Literal(0.2)), p, f));

            expr = _parser.Compile("2*e ^ ((x-1) ^ 2 / (-0.5)) + 2*e ^ ((x+1) ^ 2 / (-0.5))", _vars);
            _renderItems.Add(new InfoItem("A complex expression, \"(1/(sigma*sqrt(2*pi))) * e ^ ( (x - mu)^2 / (-2 * sigma^2))\"", f));
            _renderItems.Add(new ExpressionItem(expr, p, f));
            //(1/(sigma*sqrt(2*pi))) * e ^ ( (x - mu)^2 / (-2 * sigma^2))

            _renderItems.Add(new InfoItem("A plot of the expression: ", f));
            _renderItems.Add(new GraphItem(expr, p, "x", _parser));

        }
    }
}