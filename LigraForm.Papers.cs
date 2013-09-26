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
        private void AtmmseCommand(string input, string[] args)
        {
            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images");

            int time = Environment.TickCount;

            RenderItemContainer ric = new RenderItemContainer("atmmse paper");
            _renderItems.Add(ric);

            string filename = "lena256g.bmp";

            if (args.Length > 1)
            {
                filename = args[1].Trim('\"', '\'');
            }

            Matrix image = LoadImageForFilters(filename, "Original Image", ric, true);

            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\my papers\\atmmse");



            List<List<string>> atmmseResults = new List<List<string>>();
            List<List<string>> atmmseatmResults = new List<List<string>>();
            List<List<string>> mmseResults = new List<List<string>>();
            List<List<string>> mmseatmResults = new List<List<string>>();

            List<string> resultsLine;

            resultsLine = new List<string>(new string[] { "v", "p", "w", "a", "MSE", "MSSIM", "t", });
            atmmseResults.Add(resultsLine);                    
                                                               
            resultsLine = new List<string>(new string[] { "v", "p", "w", "a", "MSE", "MSSIM", "t", });
            atmmseatmResults.Add(resultsLine);                 
                                                               
            resultsLine = new List<string>(new string[] { "v", "p", "w", "a", "MSE", "MSSIM", "t", });
            mmseResults.Add(resultsLine);                      
                                                               
            resultsLine = new List<string>(new string[] { "v", "p", "w", "a", "MSE", "MSSIM", "t", });
            mmseatmResults.Add(resultsLine);


            float gaussianNoiseStdev;
            for (gaussianNoiseStdev = 0.01f; gaussianNoiseStdev <= 0.07f; gaussianNoiseStdev += 0.01f)
            {
                float gaussianNoiseVariance = gaussianNoiseStdev * gaussianNoiseStdev;

                resultsLine = new List<string>(new string[] { "v", "p", "w", "a", });
                resultsLine[0] = gaussianNoiseVariance.ToString();

                GaussianNoiseMatrixFilter gaussianFilter = new GaussianNoiseMatrixFilter(gaussianNoiseVariance);
                Matrix gaussianNoisyImage = gaussianFilter.Apply(image);

                foreach (float impulseProbability in new float[] { 0.01f, 0.05f, 0.1f, 0.15f, 0.2f, 0.25f })
                {
                    resultsLine[1] = impulseProbability.ToString();

                    ImpulseNoiseMatrixFilter noiseFilter = new ImpulseNoiseMatrixFilter(impulseProbability);
                    Matrix noisyImage = noiseFilter.Apply(gaussianNoisyImage);
                    SaveImageForFilters(
                        "noisy_image_v_" + gaussianNoiseVariance.ToString("G4") +
                        "p_" + impulseProbability.ToString("G3") +
                        ".bmp", noisyImage);

                    float alpha;
                    int windowSize;

                    for (windowSize = 3; windowSize <= 7; windowSize += 2)
                    {
                        MinimalMeanSquareErrorMatrixFilter mmseFilter = new MinimalMeanSquareErrorMatrixFilter(windowSize, gaussianNoiseVariance);

                        resultsLine[2] = windowSize.ToString();
                        resultsLine[3] = string.Empty;
                        ApplyFilterForExperiment(noisyImage, mmseFilter, 
                            "mmse_v_" + gaussianNoiseVariance.ToString("G4") + 
                            "p_" + impulseProbability.ToString("G3") + 
                            "w_" + windowSize.ToString() + ".bmp", 
                            image, resultsLine, mmseResults, null);

                        for (alpha = 0; alpha < 0.5; alpha += 0.05f)
                        {
                            resultsLine[3] = alpha.ToString();

                            AlphaTrimmedMmseMatrixFilter atmmseFilter = new AlphaTrimmedMmseMatrixFilter(windowSize, gaussianNoiseVariance, alpha);
                            AlphaTrimmedMmsePlusAtmMatrixFilter atmmseatmFilter = new AlphaTrimmedMmsePlusAtmMatrixFilter(windowSize, gaussianNoiseVariance, alpha);
                            MmsePlusAtmMatrixFilter mmseatmFilter = new MmsePlusAtmMatrixFilter(alpha, windowSize, gaussianNoiseVariance);

                            ApplyFilterForExperiment(noisyImage, atmmseFilter, "atmmse_v_" + gaussianNoiseVariance.ToString("G4") +
                                "p_" + impulseProbability.ToString("G3") +
                                "w_" + windowSize.ToString() +
                                "a_" + alpha.ToString("G3") +
                                ".bmp", image, resultsLine, atmmseResults, null);

                            ApplyFilterForExperiment(noisyImage, atmmseatmFilter, "atmmseatm_v_" + gaussianNoiseVariance.ToString("G4") +
                                "p_" + impulseProbability.ToString("G3") +
                                "w_" + windowSize.ToString() +
                                "a_" + alpha.ToString("G3") +
                                ".bmp", image, resultsLine, atmmseatmResults, null);

                            ApplyFilterForExperiment(noisyImage, mmseatmFilter, "mmseatm_v_" + gaussianNoiseVariance.ToString("G4") +
                                "p_" + impulseProbability.ToString("G3") +
                                "w_" + windowSize.ToString() +
                                "a_" + alpha.ToString("G3") +
                                ".bmp", image, resultsLine, mmseatmResults, null);
                        }
                    }
                }
            }

            WriteResultsToFile(atmmseResults, "atmmse_results.csv");
            WriteResultsToFile(atmmseatmResults, "atmmseatm_results.csv");
            WriteResultsToFile(mmseResults, "mmse_results.csv");
            WriteResultsToFile(mmseatmResults, "mmseatm_results.csv");

            MessageBox.Show("Done. Time = " + (Environment.TickCount - time).ToString() + "ms");
        }

        private static void WriteResultsToFile(List<List<string>> results, string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                foreach (List<string> line in results)
                {
                    sw.WriteLine(string.Join(",", line.ToArray()));
                }
            }
        }

        public class BestImageInfo
        {
            public float mssim = 0;
            public Matrix image = null;
            public List<string> filterParameters = null;
        }

        public struct ApplyInfo
        {
            public ApplyInfo(float gnv, int ws, float z)
            {
                gaussianNoiseVariance = gnv;
                windowSize = ws;
                zeta = z;
                alpha = 0;
                impulseProbability = 0;
            }
            public ApplyInfo(float gnv, int ws, float z, float a) 
            {
                gaussianNoiseVariance = gnv;
                windowSize = ws;
                zeta = z;
                alpha = a;
                impulseProbability = 0;
            }
            public float gaussianNoiseVariance;
            public float impulseProbability;
            public int windowSize;
            public float zeta;
            public float alpha;
        }

        private void ApplyFilterForExperiment(Matrix noisyImage, MatrixFilter filter, string filename, Matrix image, List<string> filterParameters, List<List<string>> results, BestImageInfo bestImage)
        {
            int time;
            Matrix filtered;

            time = Environment.TickCount;
            filtered = filter.Apply(noisyImage);
            time = Environment.TickCount - time;

            if (!string.IsNullOrEmpty(filename))
            {
                SaveImageForFilters(filename, filtered);
            }

            if (results != null)
            {
                List<string> resultsLine;
                if (filterParameters != null)
                {
                    resultsLine = new List<string>(filterParameters);
                }
                else
                {
                    resultsLine = new List<string>();
                }

                if (image != null)
                {
                    float mssim = SsimErrorMeasure.Measure(filtered, image, 7);

                    if (bestImage != null && mssim > bestImage.mssim)
                    {
                        bestImage.mssim = mssim;
                        bestImage.image = filtered;
                        bestImage.filterParameters = filterParameters;
                    }

                    resultsLine.Add(AcuityEngine.MeanSquareError(filtered, image).ToString());
                    resultsLine.Add(mssim.ToString());
                }
                resultsLine.Add(time.ToString());

                results.Add(resultsLine);
            }
        }

        private void ZtmCommand(string input, string[] args)
        {
        }

        private void ZtmmseCommand(string input, string[] args)
        {
            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images");

            int time = Environment.TickCount;

            RenderItemContainer ric = new RenderItemContainer("atmmse paper");
            _renderItems.Add(ric);

            string filename = "lena256g.bmp";

            if (args.Length > 1)
            {
                filename = args[1].Trim('\"', '\'');
            }

            Matrix image = LoadImageForFilters(filename, "Original Image", ric, true);




            List<List<string>> ztmmseResults = new List<List<string>>();
            List<List<string>> ztmmseatmResults = new List<List<string>>();
            List<List<string>> ztmmseztmResults = new List<List<string>>();

            List<string> resultsLine;
            List<string> resultsHeader;

            resultsLine = new List<string>(new string[] { "v", "p", "w", "z", "a", "MSE", "MSSIM", "t", });
            resultsHeader = resultsLine;

            ztmmseResults.Add(resultsLine);
            ztmmseatmResults.Add(resultsLine);
            ztmmseztmResults.Add(resultsLine);

            BestImageInfo ztmmseBestImage = new BestImageInfo();
            BestImageInfo ztmmseztmBestImage = new BestImageInfo();
            BestImageInfo ztmmseatmBestImage = new BestImageInfo();

            //float gaussianNoiseStdev;

            Dictionary<ApplyInfo, ZetaTrimmedMmseMatrixFilter> ztmmseFilters = new Dictionary<ApplyInfo, ZetaTrimmedMmseMatrixFilter>();
            Dictionary<ApplyInfo, ZetaTrimmedMmsePlusZtmMatrixFilter> ztmmseztmFilters = new Dictionary<ApplyInfo, ZetaTrimmedMmsePlusZtmMatrixFilter>();
            Dictionary<ApplyInfo, ZetaTrimmedMmsePlusAtmMatrixFilter> ztmmseatmFilters = new Dictionary<ApplyInfo, ZetaTrimmedMmsePlusAtmMatrixFilter>();
            List<ApplyInfo> parameterGroups = new List<ApplyInfo>();
            List<ApplyInfo> ztmmseatmParameterGroups = new List<ApplyInfo>();

            float[] gaussianNoiseVarianceList = new float[] { 0.0001f, 0.0004f, 0.0009f, 0.0016f, 0.0025f, 0.0036f, 0.0049f };
            float[] impulseProbabilityList = new float[] { 0.01f, 0.05f, 0.1f, 0.15f, 0.2f, 0.25f };
            int[] windowSizeList = new int[] { 3, 5, 7 };
            float[] zetaList = new float[] { 1, 1.25f, 1.5f, 1.75f, 2f, 2.25f };
            float[] alphaList = new float[] { 0, 0.05f, 0.1f, 0.15f, 0.2f, 0.25f, 0.3f, 0.35f, 0.4f, 0.45f, 0.5f, };

            foreach (float gaussianNoiseVariance in gaussianNoiseVarianceList)
            {
                foreach (float impulseProbability in impulseProbabilityList)
                {
                    foreach (int windowSize in windowSizeList)
                    {
                        foreach (float zeta in zetaList)
                        {
                            ApplyInfo info = new ApplyInfo();
                            info.gaussianNoiseVariance = gaussianNoiseVariance;
                            info.impulseProbability = impulseProbability;
                            info.windowSize = windowSize;
                            info.zeta = zeta;
                            parameterGroups.Add(info);

                            foreach (float alpha in alphaList)
                            {
                                ApplyInfo info2 = info;
                                info2.alpha = alpha;
                                ztmmseatmParameterGroups.Add(info2);
                            }
                        }
                    }
                }
            }

            foreach (float gaussianNoiseVariance in gaussianNoiseVarianceList)
            {
                foreach (int windowSize in windowSizeList)
                {
                    foreach (float zeta in zetaList)
                    {
                        ApplyInfo info = new ApplyInfo();
                        info.gaussianNoiseVariance = gaussianNoiseVariance;
                        info.windowSize = windowSize;
                        info.zeta = zeta;

                        ztmmseFilters[info] = new ZetaTrimmedMmseMatrixFilter(windowSize, gaussianNoiseVariance, zeta);
                        ztmmseztmFilters[info] = new ZetaTrimmedMmsePlusZtmMatrixFilter(windowSize, gaussianNoiseVariance, zeta);

                        foreach (float alpha in alphaList)
                        {
                            ApplyInfo info2 = info;
                            info2.alpha = alpha;

                            ztmmseatmFilters[info2] = new ZetaTrimmedMmsePlusAtmMatrixFilter(windowSize, gaussianNoiseVariance, zeta, alpha);
                        }
                    }
                }
            }


            foreach (ApplyInfo info in parameterGroups)
            {
                float gnv = info.gaussianNoiseVariance;
                float ip = info.impulseProbability;
                int ws = info.windowSize;
                float z = info.zeta;


            }

            foreach (ApplyInfo info in ztmmseatmParameterGroups)
            {
                float gnv = info.gaussianNoiseVariance;
                float ip = info.impulseProbability;
                int ws = info.windowSize;
                float z = info.zeta;
                float a = info.alpha;


            }

            foreach (float gaussianNoiseVariance in gaussianNoiseVarianceList)
            {
                resultsLine = new List<string>(new string[] { "v", "p", "w", "z", "a", });
                resultsLine[0] = gaussianNoiseVariance.ToString();

                foreach (float impulseProbability in impulseProbabilityList)
                {
                    resultsLine[1] = impulseProbability.ToString();

                    System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\my papers\\atmmse");

                    string noisyFilename =
                        "noisy_image_" +
                        "v_" + gaussianNoiseVariance.ToString("G4") +
                        "p_" + impulseProbability.ToString("G3") +
                        ".bmp";

                    Matrix noisyImage = LoadImageForFilters(noisyFilename, string.Empty, null, false);

                    System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\my papers\\ztmmse");

                    SaveImageForFilters(noisyFilename, noisyImage);

                    foreach (int windowSize in windowSizeList)
                    {
                        resultsLine[2] = windowSize.ToString();
                        resultsLine[3] = string.Empty;

                        foreach (float zeta in zetaList)
                        {
                            ZetaTrimmedMmseMatrixFilter ztmmseFilter = new ZetaTrimmedMmseMatrixFilter(windowSize, gaussianNoiseVariance, zeta);
                            ZetaTrimmedMmsePlusZtmMatrixFilter ztmmseztmFilter = new ZetaTrimmedMmsePlusZtmMatrixFilter(windowSize, gaussianNoiseVariance, zeta);

                            resultsLine[3] = zeta.ToString();
                            resultsLine[4] = string.Empty;

                            ApplyFilterForExperiment(noisyImage, ztmmseFilter, "ztmmse_" + 
                                "v_" + gaussianNoiseVariance.ToString("G4") +
                                "p_" + impulseProbability.ToString("G3") +
                                "w_" + windowSize.ToString() +
                                "z_" + zeta.ToString("G3") +
                                ".bmp", image, resultsLine, ztmmseResults, ztmmseBestImage);

                            ApplyFilterForExperiment(noisyImage, ztmmseztmFilter, "ztmmseztm_" +
                                "v_" + gaussianNoiseVariance.ToString("G4") +
                                "p_" + impulseProbability.ToString("G3") +
                                "w_" + windowSize.ToString() +
                                "z_" + zeta.ToString("G3") +
                                ".bmp", image, resultsLine, ztmmseztmResults, ztmmseztmBestImage);

                            foreach (float alpha in alphaList)
                            {
                                ZetaTrimmedMmsePlusAtmMatrixFilter ztmmseatmFilter = new ZetaTrimmedMmsePlusAtmMatrixFilter(windowSize, gaussianNoiseVariance, zeta, alpha);
                                resultsLine[4] = alpha.ToString();
                                ApplyFilterForExperiment(noisyImage, ztmmseatmFilter, "ztmmseatm_" +
                                    "v_" + gaussianNoiseVariance.ToString("G4") +
                                    "p_" + impulseProbability.ToString("G3") +
                                    "w_" + windowSize.ToString() +
                                    "z_" + zeta.ToString("G3") +
                                    "a_" + alpha.ToString("G3") +
                                    ".bmp", image, resultsLine, ztmmseatmResults, ztmmseatmBestImage);
                            }
                        }
                    }
                }
            }


            WriteResultsToFile(ztmmseResults, "ztmmse_results.csv");
            WriteResultsToFile(ztmmseatmResults, "ztmmseatm_results.csv");
            WriteResultsToFile(ztmmseztmResults, "ztmmseztm_results.csv");


            ric.Items.Add(new GraphMatrixItem(
                ztmmseBestImage.image, "ZTMMSE " +
                string.Join(", ", ztmmseBestImage.filterParameters.ToArray()) +
                "  MSSIM = " + ztmmseBestImage.mssim.ToString("G3")));
            ric.Items.Add(new GraphMatrixItem(
                ztmmseatmBestImage.image, "ZTMMSE+ATM " +
                string.Join(", ", ztmmseatmBestImage.filterParameters.ToArray()) +
                "  MSSIM = " + ztmmseatmBestImage.mssim.ToString("G3")));
            ric.Items.Add(new GraphMatrixItem(
                ztmmseztmBestImage.image, "ZTMMSE+ZTM " +
                string.Join(", ", ztmmseztmBestImage.filterParameters.ToArray()) +
                "  MSSIM = " + ztmmseztmBestImage.mssim.ToString("G3")));


            MessageBox.Show("Done. Time = " + (Environment.TickCount - time).ToString() + "ms");
        }
    }
}