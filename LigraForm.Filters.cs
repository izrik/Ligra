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
        private void FiltersCommand(string input, string[] args)
        {
            Font font = ligraControl1.Font;
            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images");

            bool useSsim = false;

            //Matrix tank = _engine.LoadImage("tank256.bmp");
            //Vector a = new Vector(10, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1);//tank.GetRow(0);

            //_renderItems.Add(new GraphVectorItem(a));

            //VectorFilter filter = new ConvolutionVectorFilter(new Vector(3, 1, 0, 1));
            //    //new MovingAverageVectorFilter(5);

            //Vector b = filter.Apply(a);
            //_renderItems.Add(new GraphVectorItem(b));





            //_renderItems.Add(new GraphMatrixItem(tank));
            //Matrix filterMat = new Matrix(16, 16);

            //filterMat[0, 0] = Literal.One;
            //filterMat[15, 15] = Literal.One;

            //tank.ApplyToAll(AcuityEngine.Convert24gToFloat);

            //MatrixFilter filter2 = new ConvolutionMatrixFilter(filterMat);
            //Matrix tank2 = filter2.Apply(tank);

            //tank2.ApplyToAll(new Modulator((new AcuityEngine.MultiplyModulator(0.5)).Modulate));
            //tank2.ApplyToAll(AcuityEngine.ConvertFloatTo24g);

            //_renderItems.Add(new GraphMatrixItem(tank2));


            int i;
            Vector v;
            Vector u;
            VectorFilter filter;

            v = new Vector(500);
            for (i = 200; i < 300; i++)
            {
                v[i] = 1;
            }

            _renderItems.Add(new IntroItem());

            RenderItemContainer ric = new RenderItemContainer("1D Moving Average Filter");
            _renderItems.Add(ric);

            ric.Items.Add(new GraphVectorItem(v, "Original signal"));

            filter = new GaussianNoiseVectorFilter(0, 0.01f);
            v = filter.Apply(v);

            ric.Items.Add(new GraphVectorItem(v, "Noisy signal (Gaussian)"));

            filter = new MovingAverageVectorFilter(11);
            u = filter.Apply(v);
            ric.Items.Add(new GraphVectorItem(u, "Noisy signal after applying 11-point moving average."));

            filter = new MovingAverageVectorFilter(51);
            u = filter.Apply(v);
            ric.Items.Add(new GraphVectorItem(u, "Noisy signal after applying 51-point moving average."));

            ric = new RenderItemContainer("1D Moving Average Filter");
            _renderItems.Add(ric);

            v = new Vector(252, 228.62f, 228.34f, 228.62f, 229.95f, 230.93f,
                        236.04f, 227.36f, 226.31f, 229.67f, 232.82f, 232.82f,
                        230.3f, 224.77f, 218.05f, 219.45f, 212.8f, 215.88f,
                        215.6f, 217f, 222.46f, 221.9f, 222.88f, 221.69f, 223.23f,
                        221.83f, 223.86f, 223.3f, 225.05f, 226.1f, 227.29f,
                        231.14f, 233.1f, 232.96f, 236.32f, 235.76f, 235.41f,
                        238.42f, 242.69f, 243.6f, 243.74f, 243.74f, 244.09f,
                        242.48f, 245.7f, 241.5f, 243.88f, 246.4f, 248.15f,
                        249.83f, 247.38f, 248.5f, 247.66f, 247.03f, 243.95f,
                        243.39f, 241.36f, 251.23f, 254.38f, 255.15f, 258.65f,
                        259.35f, 259.63f, 259f, 251.44f, 255.22f, 261.31f,
                        262.29f, 261.73f, 263.06f, 263.2f, 268.45f, 271.74f,
                        265.02f, 263.97f, 269.5f, 266.7f, 267.26f, 267.19f,
                        266f, 267.47f, 267.26f, 266.84f, 261.1f, 261.66f,
                        262.08f, 264.74f, 268.59f, 272.86f, 271.6f, 271.67f,
                        275.17f, 271.18f, 272.44f, 270.62f, 270.13f, 267.26f,
                        269.5f, 270.69f, 270.9f, 274.4f, 279.16f, 276.43f,
                        277.2f, 279.09f, 276.5f, 279.3f, 281.33f, 278.11f,
                        276.78f, 275.73f, 278.53f, 279.16f, 280f, 280f, 278.6f,
                        275.8f, 267.75f, 262.29f, 267.75f, 266.35f, 272.86f,
                        270.9f, 267.82f, 264.6f, 268.1f, 272.3f, 256.2f, 252f,
                        250.6f, 220.08f, 226.1f, 229.95f, 236.04f, 234.5f,
                        238.84f, 241.5f, 242.83f, 245.84f, 244.65f, 244.86f,
                        248.36f, 254.1f, 256.13f, 252.77f, 252.07f, 252.63f,
                        255.01f, 253.4f, 252.91f, 255.01f, 253.61f, 251.86f,
                        252.98f, 260.75f, 259.77f, 259.84f, 257.39f, 254.8f,
                        260.05f, 259f, 258.3f, 258.79f, 258.3f, 258.58f, 258.09f,
                        261.8f, 264.11f, 265.37f, 266.49f, 265.93f, 262.08f,
                        265.37f, 269.99f, 269.85f, 269.15f, 268.87f, 258.37f,
                        258.58f, 259.49f, 259f, 256.27f, 253.68f, 251.16f, 252.98f,
                        255.57f, 254.45f, 248.57f, 250.39f, 249.41f, 246.82f, 241.15f,
                        230.09f, 236.04f, 238.35f, 235.97f, 244.93f, 254.94f, 250.53f,
                        253.96f, 258.09f, 257.88f, 255.29f, 259.21f, 265.3f, 265.93f,
                        262.36f, 263.2f, 265.44f, 264.81f, 265.58f, 266.28f, 265.09f,
                        262.92f, 257.81f, 256.2f, 252.56f, 249.41f, 245.77f, 249.34f,
                        256.2f, 259.63f, 257.6f, 261.03f, 259.56f, 261.1f, 256.69f,
                        254.1f, 249.06f, 247.38f, 247.52f, 244.23f, 247.1f, 256.9f,
                        264.81f, 252.35f, 250.25f, 249.9f, 246.12f, 223.79f, 221.83f,
                        234.29f, 238.35f, 232.19f, 235.2f, 234.22f, 235.06f, 238f,
                        240.73f, 236.6f, 238f, 233.66f, 225.33f);
            ric.Items.Add(new GraphVectorItem(v, "Stock data for 12 months."));


            int n = 5;
            filter = new MovingAverageVectorFilter(n);
            u = filter.Apply(v).GetSlice(n, v.Length - n);
            ric.Items.Add(new GraphVectorItem(u, "Stock data with " + n.ToString() + "-day moving average"));



            Vector ff;// = Vector.FromUniformSequence(0, 32);
            //ff[0] = Literal.One;
            Vector ff2;// = (new FourierTransformVectorFilter()).Apply(ff);

            ric = new RenderItemContainer("Convolution");
            _renderItems.Add(ric);

            ff = Vector.FromUniformSequence(0, 256);
            ff[1] = 1;
            ff[60] = 3;
            ff[255] = 1;
            ff2 = new Vector(256);
            ff2 = (new SineWaveGeneratorVectorFilter(1, 0.010f, 0)).Apply(ff2);
            ric.Items.Add(new GraphVectorItem(ff2, "Sine Wave"));
            ric.Items.Add(new GraphVectorItem(ff, "Impulse Response"));
            ric.Items.Add(new GraphVectorItem(ff.Convolution(ff2), "Convolution"));



            FourierTransformVectorFilter dft = new FourierTransformVectorFilter();
            InverseFourierTransformVectorFilter idft = new InverseFourierTransformVectorFilter();

            ric = new RenderItemContainer("1D Discrete Fourier Transform");
            _renderItems.Add(ric);

            ff = new Vector(256);
            for (i = 0; i < 256; i++)
            {
                ff[i] = (float)Math.Cos(i/10.0);
            }
            ff2 = dft.Apply(ff);
            Pair<Vector> ffc = dft.Apply2(ff);

            ric.Items.Add(new GraphVectorItem(ff, "Sinusoidal function"));
            ric.Items.Add(new GraphVectorItem(ff2, "Discrete Fourier transform"));
            //ric.Items.Add(
            //            new GraphVectorItem(
            //                idft.Apply(ff2),
            //                "Reconstructed"));
            ric.Items.Add(new GraphVectorItem(idft.Apply2(ffc).First, "Reconstructed"));

            ric.Items.Add(new SpacerItem(new SizeF(250, 250)));
            int j;
            ff = new Vector(256);
            for (i = 0; i < 4; i++)
            {
                for (j = 0; j < 32; j++)
                {
                    ff[64 * i + j] = 1;
                }
                for (j = 0; j < 32; j++)
                {
                    ff[64 * i + j + 32] = -1;
                }
            }
            ff2 = dft.Apply(ff);

            ffc = dft.Apply2(ff);

            ric.Items.Add(new GraphVectorItem(ff, "Square wave"));
            ric.Items.Add(new GraphVectorItem(ff2, "Discrete Fourier transform"));
            ric.Items.Add(
                        new GraphVectorItem(
                            idft.Apply2(ffc).First,
                            "Reconstructed"));


            //ric = new RenderItemContainer("High pass and Low pass filters");

            //ff = new Vector(256);
            //for (i = 0; i < 256; i++)
            //{
            //    ff[i] = new Literal(
            //        15*Math.Cos(i/30.0) + 
            //        10*Math.Cos(i/10.0) + 
            //        8 * Math.Cos(i / 5.00)
            //        );
            //}
            //ric.Items.Add(new GraphVectorItem(ff, "Original signal"));
            //ric.Items.Add(new GraphVectorItem((new FourierTransformVectorFilter()).Apply(ff), "Discrete Fourier transform"));
            //ff2 = new Vector(256);
            //for (i = 0; i < 256; i++)
            //{
            //}

            //_renderItems.Add(ric);


            //return;


            ric = new RenderItemContainer("2D Prewitt Edge Detection");
            _renderItems.Add(ric);
            Matrix tank = LoadImageForFilters("tank256.bmp", "Original Image", ric, true);
            Modulator mod = (new AcuityEngine.MultiplyModulator(8)).Modulate;
            ApplyFilter(tank, new PrewittHorizontalMatrixFilter(), "Prewitt Hx Filter", ric, mod, null, useSsim, true);
            ApplyFilter(tank, new PrewittVerticalMatrixFilter(), "Prewitt Hy Filter", ric, mod, null, useSsim, true);


            //ric = new RenderItemContainer("2d Discrete Fourier Transform");
            //_renderItems.Add(ric);

            //ApplyFilter(tank, new FourierTransformMatrixFilter(), "Fourier", ric, null, null);



        }
        private void FiltersCommand2(string input, string[] args)
        {
            return;
            Font font = ligraControl1.Font;

            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images");

            List<Matrix> sources = new List<Matrix>(6);
            List<Matrix> sources2 = new List<Matrix>(18);
            Dictionary<Matrix, string> sourceNames = new Dictionary<Matrix, string>(18);
            List<MatrixFilter> filters = new List<MatrixFilter>(9);
            Dictionary<MatrixFilter, string> filterNames = new Dictionary<MatrixFilter, string>(9);
            List<List<Matrix>> outputs = new List<List<Matrix>>(18);
            Dictionary<Matrix, string> outputNames = new Dictionary<Matrix, string>(18 * 9);

            //Matrix tank = _engine.LoadImage2("tank256.bmp");
            //Matrix lena = _engine.LoadImage2("lena256g.bmp");
            //Matrix moon = _engine.LoadImage2("moon256.bmp");
            //Matrix boxx = _engine.LoadImage2("Cornell_box_smg.bmp");
            //Matrix bars = _engine.LoadImage2("test3.bmp");
            //Matrix chex = _engine.LoadImage2("checker3.bmp");
            Matrix block = AcuityEngine.LoadImage2("ftest2.bmp");

            //tank.ApplyToAll(AcuityEngine.Convert24gToFloat);
            //lena.ApplyToAll(AcuityEngine.Convert24gToFloat);
            //moon.ApplyToAll(AcuityEngine.Convert24gToFloat);
            //boxx.ApplyToAll(AcuityEngine.Convert24gToFloat);
            //bars.ApplyToAll(AcuityEngine.Convert24gToFloat);
            //chex.ApplyToAll(AcuityEngine.Convert24gToFloat);
            block.ApplyToAll(AcuityEngine.Convert24gToFloat);

            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images\\output");

            //GaussianMatrixNoise gaussianNoise = new GaussianMatrixNoise(0.005, 0.01);
            //SaltAndPepperMatrixNoise saltAndPepperNoise = new SaltAndPepperMatrixNoise(0.05);

            //ArithmeticMeanFilter mean3 = new ArithmeticMeanFilter(3);
            //ArithmeticMeanFilter mean7 = new ArithmeticMeanFilter(7);
            //ArithmeticMeanFilter mean9 = new ArithmeticMeanFilter(9);

            //MinMaxMatriXFilter minMax = new MinMaxMatriXFilter(5);
            ////MaxMinMatrixFilter maxMin = new MaxMinMatrixFilter(5);

            //MedianMatrixFilter median3 = new MedianMatrixFilter(5);
            //MedianMatrixFilter median7 = new MedianMatrixFilter(7);
            //MedianMatrixFilter median9 = new MedianMatrixFilter(9);

            FourierTransformMatrixFilter fourier = new FourierTransformMatrixFilter();


            Pair<Matrix> pair = fourier.Apply2(block);
            Matrix output;

            int i;
            int j;
            float rr;
            float ii;

            output = pair.First.Clone();
            for (i = 0; i < output.RowCount; i++)
            {
                for (j = 0; j < output.ColumnCount; j++)
                {
                    rr = pair.First[i, j];
                    ii = pair.Second[i, j];
                    output[i, j] = (float)Math.Sqrt(rr * rr + ii * ii);
                }
            }
            //output.ApplyToAll(Math.Log);
            output.ApplyToAll(AcuityEngine.ConvertFloatTo24g);
            AcuityEngine.SaveImage("block_fourier_amplitude.bmp", output);

            output = pair.First.Clone();
            //output.ApplyToAll(Math.Log);
            output.ApplyToAll(AcuityEngine.ConvertFloatTo24g);
            AcuityEngine.SaveImage("block_fourier_real.bmp", output);

            output = pair.First.Clone();
            //output.ApplyToAll(Math.Log);
            output.ApplyToAll(AcuityEngine.ConvertFloatTo24g);
            AcuityEngine.SaveImage("block_fourier_imaginary.bmp", output);






            //////////sources.Add(tank); sourceNames.Add(tank, "tank");
            //////////sources.Add(lena); sourceNames.Add(lena, "lena");
            //////////sources.Add(moon); sourceNames.Add(moon, "moon");
            //////////sources.Add(boxx); sourceNames.Add(boxx, "box");
            //////////sources.Add(bars); sourceNames.Add(bars, "bars");
            //////////sources.Add(chex); sourceNames.Add(chex, "checker3");
            ////////sources.Add(block); sourceNames.Add(block, "block");

            ////////foreach (Matrix m in sources)
            ////////{
            ////////    sources2.Add(m);
            ////////}
            //////////foreach (Matrix m in sources)
            //////////{
            //////////    Matrix m2 = gaussianNoise.Apply(m);
            //////////    sources2.Add(m2);
            //////////    sourceNames.Add(m2, sourceNames[m] + "_gaussianNoise");
            //////////}
            //////////foreach (Matrix m in sources)
            //////////{
            //////////    Matrix m2 = saltAndPepperNoise.Apply(m);
            //////////    sources2.Add(m2);
            //////////    sourceNames.Add(m2, sourceNames[m] + "_saltAndPepperNoise");
            //////////}

            //////////filters.Add(mean3); filterNames.Add(mean3, "mean3");
            //////////filters.Add(mean7); filterNames.Add(mean7, "mean7");
            //////////filters.Add(mean9); filterNames.Add(mean9, "mean9");
            //////////filters.Add(minMax); filterNames.Add(minMax, "minMax");
            ////////////filters.Add(maxMin);    filterNames.Add(maxMin, "maxMin");
            //////////filters.Add(median3); filterNames.Add(median3, "median3");
            //////////filters.Add(median7); filterNames.Add(median7, "median7");
            //////////filters.Add(median9); filterNames.Add(median9, "median9");
            ////////filters.Add(fourier);   filterNames.Add(fourier, "fourier");

            ////////Matrix output;
            //////////InverseFourierTransformMatrixFilter inverse = new InverseFourierTransformMatrixFilter();

            ////////foreach (Matrix m in sources2)
            ////////{
            ////////    List<Matrix> list = new List<Matrix>(filters.Count);
            ////////    outputs.Add(list);
            ////////    list.Add(m);

            ////////    output = m.Clone();
            ////////    outputNames.Add(output, sourceNames[m] + ".bmp");
            ////////    output.ApplyToAll(AcuityEngine.ConvertFloatTo24g);
            ////////    _engine.SaveImage(outputNames[output], output);

            ////////    foreach (MatrixFilter f in filters)
            ////////    {
            ////////        output = f.Apply(m);
            ////////        list.Add(output);

            ////////        //Matrix output2 = inverse.Apply(output);

            ////////        outputNames.Add(output, sourceNames[m] + "_" + filterNames[f] + ".bmp");
            ////////        output.ApplyToAll(AcuityEngine.ConvertFloatTo24g);
            ////////        _engine.SaveImage(outputNames[output], output);

            ////////        //outputNames.Add(output2, sourceNames[m] + "_" + filterNames[f] + "_inverse.bmp");
            ////////        //output2.ApplyToAll(AcuityEngine.ConvertFloatTo24g);
            ////////        //_engine.SaveImage(outputNames[output2], output2);

            ////////    }
            ////////}
        }
        private void FiltersCommand3(string input, string[] args)
        {
            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images");

            bool useSsim = false;

            RenderItemContainer ric = new RenderItemContainer("Coordinate Transform Filters");
            _renderItems.Add(ric);
            Matrix tank = LoadImageForFilters("tank256.bmp", "Original Image", ric, true);
            Matrix fishTank = ApplyFilter(tank, new FishEyeMatrixFilter(), "Fish Eye Lens", ric, null, null, useSsim, true);
            ric.Items.Add(new SpacerItem(new SizeF(500, 200)));



            Matrix rotate15 = ApplyFilter(tank, new RotateCoordinatesMatrixFilter((float)(Math.PI / 12)), "Rotation 15 degrees", ric, null, null, useSsim, true);
            Matrix rotate45 = ApplyFilter(tank, new RotateCoordinatesMatrixFilter((float)(Math.PI / 4)), "Rotation 45 degrees", ric, null, null, useSsim, true);
            Matrix rotate90 = ApplyFilter(tank, new RotateCoordinatesMatrixFilter((float)(Math.PI / 2)), "Rotation 90 degrees", ric, null, null, useSsim, true);
            Matrix rotateNeg90 = ApplyFilter(tank, new RotateCoordinatesMatrixFilter((float)(-Math.PI / 2)), "Rotation -90 degrees", ric, null, null, useSsim, true);
            Matrix swirl15 = ApplyFilter(tank, new SwirlMatrixFilter((float)(Math.PI / 12)), "Swirl 15 degrees", ric, null, null, useSsim, true);
            Matrix swirl45 = ApplyFilter(tank, new SwirlMatrixFilter((float)(Math.PI / 4)), "Swirl 45 degrees", ric, null, null, useSsim, true);
            Matrix swirl90 = ApplyFilter(tank, new SwirlMatrixFilter((float)(Math.PI / 2)), "Swirl 90 degrees", ric, null, null, useSsim, true);
            Matrix swirlNeg90 = ApplyFilter(tank, new SwirlMatrixFilter((float)(-Math.PI / 2)), "Swirl -90 degrees", ric, null, null, useSsim, true);

            //SaveImageForFilters("output\\fishtank.bmp", fishTank);
            //SaveImageForFilters("output\\rotate15.bmp", rotate15);
            //SaveImageForFilters("output\\rotate45.bmp", rotate45);
            //SaveImageForFilters("output\\rotate90.bmp", rotate90);
            //SaveImageForFilters("output\\rotateNeg90.bmp", rotateNeg90);
            //SaveImageForFilters("output\\swirl15.bmp", swirl15);
            //SaveImageForFilters("output\\swirl45.bmp", swirl45);
            //SaveImageForFilters("output\\swirl90.bmp", swirl90);
            //SaveImageForFilters("output\\swirlNeg90.bmp", swirlNeg90);

            //return;

            Variable x;
            if (_vars.ContainsKey("x"))
            {
                x = _vars["x"];
            }
            else
            {
                x = new Variable("x");
                _vars.Add(x);
            }

            Variable y;
            if (_vars.ContainsKey("y"))
            {
                y = _vars["y"];
            }
            else
            {
                y = new Variable("y");
                _vars.Add(y);
            }

            //_renderItems.Add(new GraphItem(SolusParser.Compile("sin(x * y + t)", _vars), Pens.Blue, y));
            ric = new RenderItemContainer("Live Examples");
            _renderItems.Add(ric);
            ric.Items.Add(
                new ApplyMatrixFilterItem(tank,
                    new VariableSwirlMatrixFilter(_vars, x), "Swirl"));
            ric.Items.Add(
                new ApplyMatrixFilterItem(tank,
                    new VariableRotateCoordinatesMatrixFilter(_vars, x), "Rotate"));

            LigraFormsControl varControl = new LigraFormsControl(_vars, x, AcuityEngine.ConvertDegreesToRadians);
            varControl.Minimum = -360;
            varControl.Maximum = 360;
            varControl.TickFrequency = 30;
            _renderItems.Add(new ControlItem(varControl, ligraControl1));
            //varControl.Width = 400;

        }
        private void FiltersCommand4(string input, string[] args)
        {
            Font font = ligraControl1.Font;
            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images");

            bool useSsim = false;

            RenderItemContainer ric;

            ric = new RenderItemContainer("2D Arithmetic Mean Filter");
            _renderItems.Add(ric);

            Matrix bars;
            Matrix mat2;
            MatrixFilter filter2;

            bars = AcuityEngine.LoadImage2("Patterns.bmp");
            ric.Items.Add(new GraphMatrixItem(bars, "Original Image"));
            bars.ApplyToAll(AcuityEngine.Convert24gToFloat);


            filter2 = new ArithmeticMeanFilter(3);
            mat2 = filter2.Apply(bars);
            mat2.ApplyToAll(AcuityEngine.ConvertFloatTo24g);
            ric.Items.Add(new GraphMatrixItem(mat2, "3x3 Arithmetic Mean"));

            filter2 = new ArithmeticMeanFilter(7);
            mat2 = filter2.Apply(bars);
            mat2.ApplyToAll(AcuityEngine.ConvertFloatTo24g);
            ric.Items.Add(new GraphMatrixItem(mat2, "7x7 Arithmetic Mean"));

            filter2 = new ArithmeticMeanFilter(9);
            mat2 = filter2.Apply(bars);
            //_vars.Add(new Variable("mat2"), mat2.Clone());
            mat2.ApplyToAll(AcuityEngine.ConvertFloatTo24g);
            ric.Items.Add(new GraphMatrixItem(mat2, "9x9 Arithmetic Mean"));

            //return;
            ric = new RenderItemContainer("2D Median Filter");
            _renderItems.Add(ric);

            Matrix checker = LoadImageForFilters("checker.bmp", "Original Image", ric, true);
            ApplyFilter(checker, new MedianMatrixFilter(3), "3x3 median filter", ric, null, null, useSsim, true);
            ApplyFilter(checker, new MedianMatrixFilter(7), "7x7 median filter", ric, null, null, useSsim, true);
            ApplyFilter(checker, new MedianMatrixFilter(9), "9x9 median filter", ric, null, null, useSsim, true);



            ric = new RenderItemContainer("2D Comparison of Arithmetic Mean and Median Filters");
            _renderItems.Add(ric);

            Matrix house = LoadImageForFilters("kodak\\kodim01_sm.bmp", "Original Image", ric, true);
            ApplyFilter(house, new ArithmeticMeanFilter(5), "5x5 Arithmetic Mean", ric, null, null, useSsim, true);
            ApplyFilter(house, new MedianMatrixFilter(5), "5x5 Median", ric, null, null, useSsim, true);
            ric.Items.Add(new SpacerItem(new SizeF(256, 256)));
            Matrix moon = LoadImageForFilters("moon256.bmp", "Original Image", ric, true);
            ApplyFilter(moon, new ArithmeticMeanFilter(5), "5x5 Arithmetic Mean", ric, null, null, useSsim, true);
            ApplyFilter(moon, new MedianMatrixFilter(5), "5x5 Median", ric, null, null, useSsim, true);



            ric = new RenderItemContainer("Compare arithmetic mean and median against gaussian noise");
            _renderItems.Add(ric);

            Matrix lena = LoadImageForFilters("lena256g.bmp", "Original Image", ric, true);

            //lena = ApplyFilter(lena, new GaussianMatrixNoise(0.005, 0.001), "Applied Gaussian noise with mean=0.005 and variance=0.001", ric, null, false);

            Matrix lena2 = (new GaussianNoiseMatrixFilter(
                //0.005, 
                0.001f)).Apply(lena);

            ApplyFilter(lena2, new ArithmeticMeanFilter(3), "Noisy image after applying 3x3 arithmetic mean filter", ric, null, lena, useSsim, true);
            ApplyFilter(lena2, new ArithmeticMeanFilter(5), "Noisy image after applying 5x5 arithmetic mean filter", ric, null, lena, useSsim, true);
            ApplyFilter(lena2, new ArithmeticMeanFilter(7), "Noisy image after applying 7x7 arithmetic mean filter", ric, null, lena, useSsim, true);

            mat2 = lena2.Clone();
            mat2.ApplyToAll(AcuityEngine.ConvertFloatTo24g);
            ric.Items.Add(new GraphMatrixItem(mat2, "Applied Gaussian noise with mean=0.005 and variance=0.001"));

            ApplyFilter(lena2, new MedianMatrixFilter(3), "Noisy image after applying 3x3 median filter", ric, null, lena, useSsim, true);
            ApplyFilter(lena2, new MedianMatrixFilter(5), "Noisy image after applying 5x5 median filter", ric, null, lena, useSsim, true);
            ApplyFilter(lena2, new MedianMatrixFilter(7), "Noisy image after applying 7x7 median filter", ric, null, lena, useSsim, true);


            ric = new RenderItemContainer("Compare arithmetic mean and median against salt & pepper noise");
            _renderItems.Add(ric);

            Matrix box = LoadImageForFilters("Cornell_box_smg.bmp", "Original Image", ric, true);
            box = ApplyFilter(box, new SaltAndPepperNoiseMatrixFilter(0.05f), "Applied 5% Salt & Pepper noise", ric, null, box, useSsim, true);
            ApplyFilter(box, new ArithmeticMeanFilter(3), "Noisy image after applying 3x3 arithmetic mean filter", ric, null, box, useSsim, true);
            ApplyFilter(box, new MedianMatrixFilter(3), "Noisy image after applying 3x3 median filter", ric, null, box, useSsim, true);

            ric = new RenderItemContainer("Compare median and weighted median against salt & pepper noise");
            _renderItems.Add(ric);

            Matrix checker2 = LoadImageForFilters("checker8.bmp", "Checker pattern", ric, true);
            checker2 = ApplyFilter(checker2, new SaltAndPepperNoiseMatrixFilter(0.05f), "Applied 5% Salt & Pepper noise", ric, null, checker2, useSsim, true);
            ApplyFilter(checker2, new MedianMatrixFilter(5), "Noisy image after using a 5x5 uniform median filter", ric, null, checker2, useSsim, true);
            Matrix weights = new Matrix(5, 5,
                                                1, 1, 2, 1, 1,
                                                1, 2, 4, 2, 1,
                                                2, 4, 8, 4, 2,
                                                1, 2, 4, 2, 1,
                                                1, 1, 2, 1, 1);
            ApplyFilter(checker2, new WeightedMedianMatrixFilter(weights), "Noisy image after using a 5x5 weighted median filter with the following weights: ", ric, null, checker2, useSsim, true);
            ric.Items.Add(new ExpressionItem(weights, Pens.Blue, font));

        }
        private void FourierCommand(string input, string[] args)
        {
            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images");

            bool useSsim = false;

            RenderItemContainer ric = new RenderItemContainer("Discrete 2D Fourier Transform");
            _renderItems.Add(ric);

            Matrix tank = LoadImageForFilters("tank256.bmp", "Original Image", ric, true);

            FourierTransformMatrixFilter dft = new FourierTransformMatrixFilter();
            InverseFourierTransformMatrixFilter idft = new InverseFourierTransformMatrixFilter();

            Pair<Matrix> ff = dft.Apply2(tank);

            AcuityEngine.MultiplyModulator mod = new AcuityEngine.MultiplyModulator(1 / (float)tank.RowCount);
            ff.First.ApplyToAll(mod.Modulate);
            ff.Second.ApplyToAll(mod.Modulate);

            IntervalFitMatrixFilter fit = new IntervalFitMatrixFilter();

            bool doFit = false;

            if (doFit)
            {
                ApplyFilter(ff.First, fit, "DFT Real Part", ric, null, null, useSsim, true);
                ApplyFilter(ff.Second, fit, "DFT Imaginary Part", ric, null, null, useSsim, true);
            }
            else
            {
                AddMatrixImage("DFT Real Part", ric, ff.First, null);
                AddMatrixImage("DFT Imaginary Part", ric, ff.Second, null);
            }

            BiModulatorMatrixFilter magFilter = new BiModulatorMatrixFilter(AcuityEngine.ComplexMagnitude);
            BiModulatorMatrixFilter phaseFilter = new BiModulatorMatrixFilter(AcuityEngine.ComplexPhase);

            if (doFit)
            {
                ApplyFilter(magFilter.Apply2(ff), fit, "DFT Magnitude", ric, null, null, useSsim, true);
                ApplyFilter(phaseFilter.Apply2(ff), fit, "DFT Phase", ric, null, null, useSsim, true);
            }
            else
            {
                AddMatrixImage("DFT Magnitude", ric, magFilter.Apply2(ff), null);
                AddMatrixImage("DFT Phase", ric, phaseFilter.Apply2(ff), null);
            }

            mod = new AcuityEngine.MultiplyModulator((float)tank.RowCount);
            ff.First.ApplyToAll(mod.Modulate);
            ff.Second.ApplyToAll(mod.Modulate);

            AddMatrixImage("Reconstructed", ric, idft.Apply2(ff).First, null);
        }
        private void FiltersCode(string input, SolusParser.Ex[] exTokens)
        {
            _renderItems.Add(new CodeItem());
        }
        private void FiltersExperiment(string input, string[] args)
        {
            List<Matrix> sourceImages = new List<Matrix>();
            List<string> sourceNames = new List<string>();
            List<MatrixFilter> filters = new List<MatrixFilter>();
            List<string> filterNames = new List<string>();
            List<MatrixFilter> noises = new List<MatrixFilter>();
            List<string> noiseNames = new List<string>();

            Font font = ligraControl1.Font;
            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images");

            Matrix minMax1 = AcuityEngine.LoadImage2("minmax1.bmp");
            Matrix minMax2 = AcuityEngine.LoadImage2("minmax2.bmp");
            Matrix weights1 = new Matrix(3, 3, 1, 2, 1, 2, 4, 2, 1, 2, 1);
            Matrix weights2 = new Matrix(5, 5, 1, 1, 2, 1, 1, 1, 2, 4, 2, 1, 2, 4, 8, 4, 2, 1, 2, 4, 2, 1, 1, 1, 2, 1, 1);
            Matrix weights3 = new Matrix(5, 5, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1);
            Matrix convTest = AcuityEngine.LoadImage2("convTest.bmp");

            sourceImages.Add(AcuityEngine.LoadImage2("tank256.bmp")); sourceNames.Add("Tank");
            sourceImages.Add(AcuityEngine.LoadImage2("lena256g.bmp")); sourceNames.Add("Lena");
            sourceImages.Add(AcuityEngine.LoadImage2("moon256.bmp")); sourceNames.Add("Moon");
            sourceImages.Add(AcuityEngine.LoadImage2("checker8.bmp")); sourceNames.Add("Checker");
            sourceImages.Add(AcuityEngine.LoadImage2("patterns.bmp")); sourceNames.Add("Patterns");
            sourceImages.Add(AcuityEngine.LoadImage2("Cornell_box_smg.bmp")); sourceNames.Add("Box");
            sourceImages.Add(AcuityEngine.LoadImage2("kodak\\kodim01_sm.bmp")); sourceNames.Add("House");
            sourceImages.Add(AcuityEngine.LoadImage2("land.bmp")); sourceNames.Add("Land");
            foreach (Matrix mat in sourceImages) { mat.ApplyToAll(AcuityEngine.Convert24gToFloat); }

            noises.Add(new GaussianNoiseMatrixFilter(
                //0.005, 
                0.001f)); noiseNames.Add("Gaussian005by001");
            noises.Add(new SaltAndPepperNoiseMatrixFilter(0.05f)); noiseNames.Add("SaltPepper05");

            filters.Add(new ArithmeticMeanFilter(3)); filterNames.Add("Mean3x3");
            filters.Add(new ArithmeticMeanFilter(5)); filterNames.Add("Mean5x5");
            filters.Add(new ArithmeticMeanFilter(7)); filterNames.Add("Mean7x7");

            filters.Add(new MinMaxMatrixFilter(minMax1)); filterNames.Add("MinMax1");
            filters.Add(new MinMaxMatrixFilter(minMax2)); filterNames.Add("MinMax2");
            filters.Add(new MaxMinMatrixFilter(minMax1)); filterNames.Add("MaxMin1");
            filters.Add(new MaxMinMatrixFilter(minMax2)); filterNames.Add("MaxMin2");
            filters.Add(new WeightedPMatrixFilter(0.25f, weights1)); filterNames.Add("WeightedP1at25");
            filters.Add(new WeightedPMatrixFilter(0.25f, weights2)); filterNames.Add("WeightedP2at25");
            filters.Add(new WeightedPMatrixFilter(0.25f, weights3)); filterNames.Add("WeightedP3at25");
            filters.Add(new WeightedPMatrixFilter(0.75f, weights1)); filterNames.Add("WeightedP1at75");
            filters.Add(new WeightedPMatrixFilter(0.75f, weights2)); filterNames.Add("WeightedP2at75");
            filters.Add(new WeightedPMatrixFilter(0.75f, weights3)); filterNames.Add("WeightedP3at75");
            filters.Add(new WeightedMedianMatrixFilter(weights1)); filterNames.Add("WeightedMedian1");
            filters.Add(new WeightedMedianMatrixFilter(weights2)); filterNames.Add("WeightedMedian2");
            filters.Add(new WeightedMedianMatrixFilter(weights3)); filterNames.Add("WeightedMedian3");
            filters.Add(new MedianMatrixFilter(3)); filterNames.Add("Median3x3");
            filters.Add(new MedianMatrixFilter(5)); filterNames.Add("Median5x5");
            filters.Add(new MedianMatrixFilter(7)); filterNames.Add("Median7x7");
            filters.Add(new WindowMinMatrixFilter(3)); filterNames.Add("WindowMin3x3");
            filters.Add(new WindowMinMatrixFilter(5)); filterNames.Add("WindowMin5x5");
            filters.Add(new WindowMinMatrixFilter(7)); filterNames.Add("WindowMin7x7");
            filters.Add(new WindowMaxMatrixFilter(3)); filterNames.Add("WindowMax3x3");
            filters.Add(new WindowMaxMatrixFilter(5)); filterNames.Add("WindowMax5x5");
            filters.Add(new WindowMaxMatrixFilter(7)); filterNames.Add("WindowMax7x7");
            //filters.Add(new PrewittHorizontalMatrixFilter()); filterNames.Add("PrewittH");
            //filters.Add(new PrewittVerticalFilter()); filterNames.Add("PrewittV");
            //fourier
            //fisheye, rotate, twirl

            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images\\output2");

            List<float> maxErrors = new List<float>(filters.Count + 1);
            float errorScale = 255;

            int i;
            int j;
            int k;
            for (i = 0; i < sourceImages.Count; i++)
            {
                Matrix mat = sourceImages[i];
                string imageName = sourceNames[i];

                SaveImageForFilters(imageName + "___no_noise___unfiltered.bmp", mat);

                StringWriter sw = new StringWriter();

                sw.Write("\t");

                for (k = 0; k < filters.Count; k++)
                {
                    MatrixFilter filter = filters[k];
                    string filterName = filterNames[k];

                    SaveImageForFilters(imageName + "___no_noise___" + filterName + ".bmp", filter.Apply(mat));

                    sw.Write(filterName + "\t");
                }

                sw.WriteLine();

                for (j = 0; j < noises.Count; j++)
                {
                    MatrixFilter noise = noises[j];
                    string noiseName = noiseNames[j];

                    Matrix mat2 = noise.Apply(mat);

                    SaveImageForFilters(imageName + "___" + noiseName + "___unfiltered.bmp", mat2);

                    sw.Write(noiseName + "\t");

                    maxErrors.Clear();

                    for (k = 0; k < filters.Count; k++)
                    {
                        MatrixFilter filter = filters[k];
                        string filterName = filterNames[k];
                        Matrix mat3 = filter.Apply(mat2);

                        SaveImageForFilters(imageName + "___" + noiseName + "___" + filterName + ".bmp", mat3);

                        Matrix after = mat3;
                        if (mat3.ColumnCount != mat2.ColumnCount ||
                            mat3.RowCount != mat2.RowCount)
                        {
                            after = mat3.GetSlice((mat3.RowCount - mat2.RowCount) / 2,
                                (mat3.ColumnCount - mat2.ColumnCount) / 2,
                                mat2.RowCount, mat2.ColumnCount);
                        }
                        sw.Write(ScaleAndFormatError(errorScale, AcuityEngine.MeanSquareError(mat2, after)));
                        sw.Write("\t");
                        maxErrors.Add(AcuityEngine.MaxError(mat2, after));
                    }

                    sw.WriteLine();
                    sw.Write("\t");

                    foreach (float error in maxErrors)
                    {
                        sw.Write(ScaleAndFormatError(errorScale, error));
                        sw.Write("\t");
                    }

                    sw.WriteLine();
                }

                using (StreamWriter writer = new StreamWriter(imageName + "_error.txt"))
                {
                    writer.Write(sw.ToString());
                }
            }
        }

        private void FiltersProject2Test1(string input, string[] args)
        {
            Font font = ligraControl1.Font;
            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images");

            RenderItemContainer ric = new RenderItemContainer("p2t1");
            _renderItems.Add(ric);

            string filename = "lena256g.bmp";

            if (args.Length > 2)
            {
                filename = args[2];
                filename = filename.Trim('\"');
            }

            bool useSsim = true;

            float gNoiseVariance = 0.0025f;
            float impulseProbability = 0.05f;
            float optimumAtmmseAlpha = impulseProbability / 2;
            int optimumAtmmseWindowSize = (int)(Math.Round(1 / Math.Sqrt(optimumAtmmseAlpha)));
            optimumAtmmseWindowSize += 1 - optimumAtmmseWindowSize % 2;
            float optimumZtmmseZeta;

            if (true)
            {
                optimumZtmmseZeta = ZetaTrimmedMmseMatrixFilter.CalculateOptimumZtmmseZeta(impulseProbability);
            }

            int optimumZtmmseWindowSize = (int)(Math.Ceiling(Math.Sqrt(2 / impulseProbability)));
            optimumZtmmseWindowSize += 1 - optimumZtmmseWindowSize % 2;





            Matrix image = LoadImageForFilters(filename, "Original Image", ric, true);
            GaussianNoiseMatrixFilter gNoise = new GaussianNoiseMatrixFilter(gNoiseVariance);
            ImpulseNoiseMatrixFilter iNoise = new ImpulseNoiseMatrixFilter(impulseProbability);
            HistogramMatrixFilter hist = new HistogramMatrixFilter();
            Matrix noisyImage = image;// noise.Apply(image);
            
            //noisyImage = ApplyFilter(noisyImage, noise, "Applied 5% impulse & AWGN with stdev=0.2/1", ric, null, image);

            if (args.Length <= 1 || args[1] == "1")
            {
                noisyImage = gNoise.Apply(noisyImage);
            }
            else
            {
                gNoiseVariance = 0;
            }
            if (args.Length <= 1 || args[1] == "2")
            {
                noisyImage = iNoise.Apply(noisyImage);
            }

            float mseMeasure;
            float ssimMeasure;

            mseMeasure = 65536 * AcuityEngine.MeanSquareError(image, noisyImage);

            SsimErrorMeasure em = new SsimErrorMeasure();
            ssimMeasure = em.Measure(image, noisyImage);

            AddMatrixImage("Noisy Image   MSE = " + mseMeasure.ToString("G3") + "   SSIM = " + ssimMeasure.ToString("G3"), ric, noisyImage, null);

            SaveImageForFilters("p2t1\\noisyImage.bmp", noisyImage);

            Matrix filteredImage;

            filteredImage = hist.Apply(image);
            SaveImageForFilters("p2t1\\hist_original.bmp", filteredImage);
            filteredImage = hist.Apply(noisyImage);
            SaveImageForFilters("p2t1\\hist_noisyImage.bmp", filteredImage);

            filteredImage = ApplyFilter(noisyImage, new AlphaTrimmedMeanMatrixFilter(3, 0.1f), "ATM with alpha=0.1 window=3", ric, null, image, useSsim, true);
            SaveImageForFilters("p2t1\\atm___0_1___3.bmp", filteredImage);
            filteredImage = hist.Apply(filteredImage);
            SaveImageForFilters("p2t1\\hist_atm___0_1___3.bmp", filteredImage);
            filteredImage = ApplyFilter(noisyImage, new MedianMatrixFilter(3), "median with window=3", ric, null, image, useSsim, true);
            SaveImageForFilters("p2t1\\median___3.bmp", filteredImage);
            filteredImage = hist.Apply(filteredImage);
            SaveImageForFilters("p2t1\\hist_median___3.bmp", filteredImage);

            //filteredImage = ApplyFilter(noisyImage, new MinimalMeanSquareErrorMatrixfilter(3, gNoiseVariance), "MMSE filter with window=3", ric, null, image);
            //SaveImageForFilters("p2t1\\mmse___3.bmp", filteredImage);
            //filteredImage = ApplyFilter(noisyImage, new MinimalMeanSquareErrorMatrixfilter(5, gNoiseVariance), "MMSE filter with window=5", ric, null, image);
            //SaveImageForFilters("p2t1\\mmse___5.bmp", filteredImage);
            filteredImage = ApplyFilter(noisyImage, new MinimalMeanSquareErrorMatrixFilter(7, gNoiseVariance), "MMSE filter with window=7", ric, null, image, useSsim, true);
            SaveImageForFilters("p2t1\\mmse___7.bmp", filteredImage);
            //filteredImage = ApplyFilter(noisyImage, new MinimalMeanSquareErrorMatrixfilter(15, gNoiseVariance), "MMSE filter with window=15", ric, null, image);
            //SaveImageForFilters("p2t1\\mmse___15.bmp", filteredImage);

            filteredImage = hist.Apply(filteredImage);
            SaveImageForFilters("p2t1\\hist_mmse___7.bmp", filteredImage);

            //filteredImage = ApplyFilter(noisyImage, new AlphaTrimmedMmseMatrixFilter(3, gNoiseVariance, (int)1), "ATMMSE w=3 t=1", ric, null, image);
            //SaveImageForFilters("p2t1\\atmmse___3___1.bmp", filteredImage);
            //filteredImage = ApplyFilter(noisyImage, new AlphaTrimmedMmseMatrixFilter(3, gNoiseVariance, (int)2), "ATMMSE w=3 t=2", ric, null, image);
            //SaveImageForFilters("p2t1\\atmmse___3___2.bmp", filteredImage);
            //filteredImage = ApplyFilter(noisyImage, new AlphaTrimmedMmseMatrixFilter(5, gNoiseVariance, (int)1), "ATMMSE w=5 t=1", ric, null, image);
            //SaveImageForFilters("p2t1\\atmmse___5___1.bmp", filteredImage);
            //filteredImage = ApplyFilter(noisyImage, new AlphaTrimmedMmseMatrixFilter(5, gNoiseVariance, (int)2), "ATMMSE w=5 t=2", ric, null, image);
            //SaveImageForFilters("p2t1\\atmmse___5___2.bmp", filteredImage);
            //filteredImage = ApplyFilter(noisyImage, new AlphaTrimmedMmseMatrixFilter(5, gNoiseVariance, (int)3), "ATMMSE w=5 t=3", ric, null, image);
            //SaveImageForFilters("p2t1\\atmmse___5___3.bmp", filteredImage);
            //filteredImage = ApplyFilter(noisyImage, new AlphaTrimmedMmseMatrixFilter(5, gNoiseVariance, (int)4), "ATMMSE w=5 t=4", ric, null, image);
            //SaveImageForFilters("p2t1\\atmmse___5___4.bmp", filteredImage);
            //filteredImage = ApplyFilter(noisyImage, new AlphaTrimmedMmseMatrixFilter(7, gNoiseVariance, (int)1), "ATMMSE w=7 t=1", ric, null, image);
            //SaveImageForFilters("p2t1\\atmmse___7___1.bmp", filteredImage);
            //filteredImage = ApplyFilter(noisyImage, new AlphaTrimmedMmseMatrixFilter(7, gNoiseVariance, (int)2), "ATMMSE w=7 t=2", ric, null, image);
            //SaveImageForFilters("p2t1\\atmmse___7___2.bmp", filteredImage);
            //filteredImage = ApplyFilter(noisyImage, new AlphaTrimmedMmseMatrixFilter(7, gNoiseVariance, (int)3), "ATMMSE w=7 t=3", ric, null, image);
            //SaveImageForFilters("p2t1\\atmmse___7___3.bmp", filteredImage);
            //filteredImage = ApplyFilter(noisyImage, new AlphaTrimmedMmseMatrixFilter(7, gNoiseVariance, (int)4), "ATMMSE w=7 t=4", ric, null, image);
            //SaveImageForFilters("p2t1\\atmmse___7___4.bmp", filteredImage);
            //filteredImage = ApplyFilter(noisyImage, new AlphaTrimmedMmseMatrixFilter(7, gNoiseVariance, (int)5), "ATMMSE w=7 t=5", ric, null, image);
            //SaveImageForFilters("p2t1\\atmmse___7___5.bmp", filteredImage);
            //filteredImage = ApplyFilter(noisyImage, new AlphaTrimmedMmseMatrixFilter(15, gNoiseVariance, (int)5), "ATMMSE w=15 t=5", ric, null, image);
            //SaveImageForFilters("p2t1\\atmmse___15___5.bmp", filteredImage);

            filteredImage = ApplyFilter(noisyImage, new AlphaTrimmedMmseMatrixFilter(optimumAtmmseWindowSize, gNoiseVariance, optimumAtmmseAlpha), "ATMMSE w=7 a=0.025", ric, null, image, useSsim, true);
            SaveImageForFilters("p2t1\\atmmse___7___0_025.bmp", filteredImage);
            filteredImage = hist.Apply(filteredImage);
            SaveImageForFilters("p2t1\\hist_atmmse___7___0_025.bmp", filteredImage);
            filteredImage = ApplyFilter(noisyImage, new AlphaTrimmedMmsePlusAtmMatrixFilter(optimumAtmmseWindowSize, gNoiseVariance, optimumAtmmseAlpha), "ATMMSE+ATM w=7 a=0.025 w2=3", ric, null, image, useSsim, true);
            SaveImageForFilters("p2t1\\atmmse+atm___7___0_025___3.bmp", filteredImage);
            filteredImage = hist.Apply(filteredImage);
            SaveImageForFilters("p2t1\\hist_atmmse+atm___7___0_025___3.bmp", filteredImage);

            //filteredImage = ApplyFilter(noisyImage, new ZetaTrimmedMmseMatrixFilter(3, gNoiseVariance, 2), "IRMMSE w=3 z=2", ric, null, image);
            //SaveImageForFilters("p2t1\\ztmmse___3___2.bmp", filteredImage);
            //filteredImage = ApplyFilter(noisyImage, new ZetaTrimmedMmseMatrixFilter(3, gNoiseVariance, 3), "IRMMSE w=3 z=3", ric, null, image);
            //SaveImageForFilters("p2t1\\ztmmse___3___3.bmp", filteredImage);
            //filteredImage = ApplyFilter(noisyImage, new ZetaTrimmedMmseMatrixFilter(5, gNoiseVariance, 1), "IRMMSE w=5 z=1", ric, null, image);
            //SaveImageForFilters("p2t1\\ztmmse___5___1.bmp", filteredImage);
            //filteredImage = ApplyFilter(noisyImage, new ZetaTrimmedMmseMatrixFilter(5, gNoiseVariance, 2), "IRMMSE w=5 z=2", ric, null, image);
            //SaveImageForFilters("p2t1\\ztmmse___5___2.bmp", filteredImage);
            //filteredImage = ApplyFilter(noisyImage, new ZetaTrimmedMmseMatrixFilter(5, gNoiseVariance, 3), "IRMMSE w=5 z=3", ric, null, image);
            //SaveImageForFilters("p2t1\\ztmmse___5___3.bmp", filteredImage);
            //filteredImage = ApplyFilter(noisyImage, new ZetaTrimmedMmseMatrixFilter(7, gNoiseVariance, 1), "IRMMSE w=7 z=1", ric, null, image);
            //SaveImageForFilters("p2t1\\ztmmse___7___1.bmp", filteredImage);
            //filteredImage = ApplyFilter(noisyImage, new ZetaTrimmedMmseMatrixFilter(7, gNoiseVariance, 2), "IRMMSE w=7 z=2", ric, null, image);
            //SaveImageForFilters("p2t1\\ztmmse___7___2.bmp", filteredImage);
            //filteredImage = ApplyFilter(noisyImage, new ZetaTrimmedMmseMatrixFilter(7, gNoiseVariance, 3), "IRMMSE w=7 z=3", ric, null, image);
            //SaveImageForFilters("p2t1\\ztmmse___7___3.bmp", filteredImage);

            filteredImage = ApplyFilter(noisyImage, new ZetaTrimmedMmseMatrixFilter(optimumZtmmseWindowSize, gNoiseVariance, optimumZtmmseZeta), "ZTMMSE+ATM w=7 z=1.96 w2=3", ric, null, image, useSsim, true);
            SaveImageForFilters("p2t1\\ztmmse+atm___7___1_96.bmp", filteredImage);
            filteredImage = hist.Apply(filteredImage);
            SaveImageForFilters("p2t1\\hist_ztmmse+atm___7___1_96.bmp", filteredImage);
        }

        //protected static float CalculateOptimumZtmmseZeta(float impulseProbability)
        //{
        //    float optimumZtmmseZeta;
        //    int kMax = 128;
        //    float[] c = new float[kMax];
        //    c[0] = 1;
        //    float sum = 0;
        //    float s = Math.Sqrt(Math.PI) / 2;
        //    for (int k = 0; k < kMax; k++)
        //    {
        //        int kk = 2 * k + 1;
        //        float cc = c[k];
        //        for (int m = 0; m <= k - 1; m++)
        //        {
        //            cc += c[m] * c[k - 1 - m] / ((m + 1) * (2 * m + 1));
        //        }
        //        c[k] = cc;

        //        sum += cc * Math.Pow(s * (impulseProbability - 1), 2 * k + 1) / (2 * k + 1);
        //    }
        //    optimumZtmmseZeta = sum * -Math.Sqrt(2);
        //    return optimumZtmmseZeta;
        //}

        private void SsimTest(string input, string[] args)
        {
            Font font = ligraControl1.Font;
            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images");
            RenderItemContainer ric = new RenderItemContainer("p2t1");
            _renderItems.Add(ric);


            string filename = "lena256g.bmp";

            Matrix image = LoadImageForFilters(filename, "Original Image", ric, true);

            SsimErrorMeasure ssim = new SsimErrorMeasure();

            Matrix map = ssim.GenerateMap(image, image);
            float error = ssim.Measure(image, image);

            AddMatrixImage("SSIM map    MSSIM = " + error.ToString("G3"), ric, map, null);

            float gNoiseVariance = 0.0025f;
            float impulseProbability = 0.05f;

            GaussianNoiseMatrixFilter gNoise = new GaussianNoiseMatrixFilter(gNoiseVariance);
            ImpulseNoiseMatrixFilter iNoise = new ImpulseNoiseMatrixFilter(impulseProbability);
            HistogramMatrixFilter hist = new HistogramMatrixFilter();
            Matrix noisyImage = image;// noise.Apply(image);

            int optimumZtmmseWindowSize = (int)(Math.Ceiling(Math.Sqrt(2 / impulseProbability)));
            optimumZtmmseWindowSize += 1 - optimumZtmmseWindowSize % 2;
            float optimumZtmmseZeta = ZetaTrimmedMmseMatrixFilter.CalculateOptimumZtmmseZeta(impulseProbability);
            float optimumAtmmseAlpha = impulseProbability / 2;
            int optimumAtmmseWindowSize = (int)(Math.Round(1 / Math.Sqrt(optimumAtmmseAlpha)));
            optimumAtmmseWindowSize += 1 - optimumAtmmseWindowSize % 2;

            Matrix filteredImage;

            noisyImage = gNoise.Apply(noisyImage);
            noisyImage = iNoise.Apply(noisyImage);

            AddMatrixImage("Noisy Image", ric, noisyImage, null);

            map = ssim.GenerateMap(image, noisyImage);
            error = ssim.Measure(image, noisyImage);
            AddMatrixImage("SSIM map for noisy image   SSIM = " + error.ToString("G3"), ric, map, null);




            filteredImage = ApplyFilter(noisyImage, new AlphaTrimmedMeanMatrixFilter(3, 0.025f), "ATM w=3 a=0.025", ric, null, image, true, true);
            map = ssim.GenerateMap(image, filteredImage);
            error = ssim.Measure(image, filteredImage);
            AddMatrixImage("SSIM map for ATM w=3", ric, map, null);



            filteredImage = ApplyFilter(noisyImage, new ZetaTrimmedMeanMatrixFilter(3, optimumZtmmseZeta), "ZTM w=3 z=1.96", ric, null, image, true, true);
            map = ssim.GenerateMap(image, filteredImage);
            error = ssim.Measure(image, filteredImage);
            AddMatrixImage("SSIM map for ZTM w=3", ric, map, null);

            //filteredImage = ApplyFilter(noisyImage, new ZetaTrimmedMeanMatrixFilter(5, optimumZtmmseZeta), "ZTM w=5 z=1.96", ric, null, image, true);
            //map = ssim.GenerateMap(image, filteredImage);
            //error = ssim.Measure(image, filteredImage);
            //AddMatrixImage("SSIM map for ZTM w=5", ric, map, null);

            //filteredImage = ApplyFilter(noisyImage, new ZetaTrimmedMeanMatrixFilter(7, optimumZtmmseZeta), "ZTM w=7 z=1.96", ric, null, image, true);
            //map = ssim.GenerateMap(image, filteredImage);
            //error = ssim.Measure(image, filteredImage);
            //AddMatrixImage("SSIM map for ZTM w=7", ric, map, null);



            filteredImage = ApplyFilter(noisyImage, new AlphaTrimmedMmseMatrixFilter(optimumAtmmseWindowSize, gNoiseVariance, optimumAtmmseAlpha), "ATMMSE w=7 a=0.025", ric, null, image, true, true);
            map = ssim.GenerateMap(image, filteredImage);
            error = ssim.Measure(image, filteredImage);
            AddMatrixImage("SSIM map for ATMMSE", ric, map, null);

            filteredImage = ApplyFilter(noisyImage, new AlphaTrimmedMmsePlusAtmMatrixFilter(optimumAtmmseWindowSize, gNoiseVariance, optimumAtmmseAlpha), "ATMMSE+ATM w=7 a=0.025", ric, null, image, true, true);
            map = ssim.GenerateMap(image, filteredImage);
            error = ssim.Measure(image, filteredImage);
            AddMatrixImage("SSIM map for ATMMSE+ATM", ric, map, null);

            filteredImage = ApplyFilter(noisyImage, new ZetaTrimmedMmseMatrixFilter(optimumZtmmseWindowSize, gNoiseVariance, optimumZtmmseZeta), "ZTMMSE+ATM w=7 z=1.96 w2=3", ric, null, image, true, true);
            map = ssim.GenerateMap(image, filteredImage);
            error = ssim.Measure(image, filteredImage);
            AddMatrixImage("SSIM map for ZTMMSE+ATM", ric, map, null);



            //filteredImage = ApplyFilter(noisyImage, new ZetaTrimmedMmsePlusZtmMatrixFilter(optimumZtmmseWindowSize, gNoiseVariance, optimumZtmmseZeta, optimumZtmmseZeta), "ZTMMSE+ZTM w=3 z=1.96", ric, null, image, true);
            //map = ssim.GenerateMap(image, filteredImage);
            //error = ssim.Measure(image, filteredImage);
            //AddMatrixImage("SSIM map for ZTMMSE+ZTM", ric, map, null);
        }

        private void FiltersProject2Trial2(string input, string[] args)
        {
            Font font = ligraControl1.Font;
            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images\\p2t2");

            RenderItemContainer ric = new RenderItemContainer("p2t2");
            _renderItems.Add(ric);

            string filename = "lena256g.bmp";
            string imageName = filename.Substring(0, filename.LastIndexOf('.'));

            Matrix image = LoadImageForFilters(filename, "Original Image", ric, true);

            float gaussianNoiseVariance = 0.0025f;
            float impulseProbability = 0.05f;

            SimpleScaleMatrixFilter scaler = new SimpleScaleMatrixFilter(18);

            Matrix originalWindow = image.GetSlice(140, 35, 7, 7);
            Matrix originalWindow2 = scaler.Apply(originalWindow);

            GaussianNoiseMatrixFilter gaussianNoise = new GaussianNoiseMatrixFilter(gaussianNoiseVariance);
            ImpulseNoiseMatrixFilter impulseNoise = new ImpulseNoiseMatrixFilter(impulseProbability);
            ChainMatrixFilter combinedNoise = new ChainMatrixFilter(gaussianNoise, impulseNoise);

            Matrix gaussianNoisyImage = gaussianNoise.Apply(image);
            Matrix impulseNoisyImage = impulseNoise.Apply(image);
            Matrix combinedNoisyImage = combinedNoise.Apply(image);

            AddMatrixImage("Noisy image", ric, gaussianNoisyImage, null);

            MinimalMeanSquareErrorMatrixFilter mmseFilter = new MinimalMeanSquareErrorMatrixFilter(7, gaussianNoiseVariance);
            Matrix mmseResult = ApplyFilter(gaussianNoisyImage, mmseFilter, "After applying MMSE", ric, null, image, true, true);

            AlphaTrimmedMmseMatrixFilter atmmseFilter = new AlphaTrimmedMmseMatrixFilter(7, gaussianNoiseVariance, 0.12f);
            Matrix atmmseResult = //atmmseFilter.Apply(gaussianNoisyImage);
                ApplyFilter(gaussianNoisyImage, atmmseFilter, "After applying ATMMSE", ric, null, image, true, true);

            ric.Items.Add(new SpacerItem(1200, 25));

            Matrix window = gaussianNoisyImage.GetSlice(140, 35, 7, 7);
            Matrix window2 = scaler.Apply(window);
            AddMatrixImage("Window of noisy image", ric, window2, null);

            HistogramMatrixFilter hist = new HistogramMatrixFilter();
            Matrix windowHist = hist.Apply(window);
            AddMatrixImage("Histogram of window of noisy image", ric, windowHist, null);

            List<float> values = new List<float>(window.Count);
            foreach (float value in window)
            {
                values.Add(value);
            }

            List<float> ordered = new List<float>(values);
            ordered.Sort(MinimalMeanSquareErrorMatrixFilter.Compare);

            ric.Items.Add(new GraphVectorItem(new Vector(values.Count, values.ToArray()), "window "));

            ric.Items.Add(new GraphVectorItem(new Vector(ordered.Count, ordered.ToArray()), "Order Statistic"));
            float signalMean = (new SignalMeanMeasure()).Measure(window);
            float signalVariance = (new SignalVarianceMeasure()).Measure(window);
            ric.Items.Add(new SpacerItem(1200, 25));
            ric.Items.Add(new TextItem("mean=" + (signalMean).ToString("G3") + "   variance=" + (signalVariance).ToString("G3"), font));
            ric.Items.Add(new SpacerItem(1200, 25));
            float mmseValue = ((1 - gaussianNoiseVariance / signalVariance) * window[3, 3] + gaussianNoiseVariance / signalVariance * signalMean);
            ric.Items.Add(new TextItem("Normal MMSE pixel result: " + (mmseValue).ToString("G3"), font));
            ric.Items.Add(new SpacerItem(1200, 100));

            Matrix mmseWindow = mmseResult.GetSlice(140, 35, 7, 7);
            Matrix mmseWindow2 = scaler.Apply(mmseWindow);
            AddMatrixImage("Window after MMSE", ric, mmseWindow2, null);
            AddMatrixImage("Window in original", ric, originalWindow2, null);

            ric.Items.Add(new SpacerItem(1200, 25));

            //values in ATM calculation

            ric.Items.Add(new TextItem("values in ATM calculation, alpha=0.12", font));
            ric.Items.Add(new SpacerItem(1200, 25));

            List<float> ordered2 = new List<float>(ordered);

            ordered2.RemoveRange(0, 6);
            ordered2.Reverse();
            ordered2.RemoveRange(0, 6);
            ordered2.Reverse();

            ric.Items.Add(new GraphVectorItem(new Vector(ordered2.Count, ordered2.ToArray()), "Order Statistic"));
            float signalMean2 = (new SignalMeanMeasure()).Measure(ordered2);
            float signalVariance2 = (new SignalVarianceMeasure()).Measure(ordered2);
            ric.Items.Add(new SpacerItem(1200, 25));
            ric.Items.Add(new TextItem("mean=" + (signalMean2).ToString("G3") + "   variance=" + (signalVariance2).ToString("G3"), font));
            ric.Items.Add(new SpacerItem(1200, 25));
            float mmseValue2 = ((1 - gaussianNoiseVariance / signalVariance2) * window[3, 3] + gaussianNoiseVariance / signalVariance2 * signalMean2);
            ric.Items.Add(new TextItem("ATMMSE pixel result: " + (mmseValue2).ToString("G3"), font));
            ric.Items.Add(new SpacerItem(1200, 100));

            Matrix atmmseWindow = atmmseResult.GetSlice(140, 35, 7, 7);
            Matrix atmmseWindow2 = scaler.Apply(mmseWindow);
            AddMatrixImage("Window after ATMMSE", ric, mmseWindow2, null);
            AddMatrixImage("Window in original", ric, originalWindow2, null);


        }

        private void FiltersProject2Trial3(string input, string[] args)
        {
            Font font = ligraControl1.Font;
            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images\\");

            RenderItemContainer ric = new RenderItemContainer("p2t3");
            _renderItems.Add(ric);

            string filename = "lena256g.bmp";
            Matrix image = LoadImageForFilters(filename, "Original Image", ric, true);

            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\project 2");

            float noiseVariance = 0.0025f;
            float impulseProbability;

            GaussianNoiseMatrixFilter gauss = new GaussianNoiseMatrixFilter(noiseVariance);

            Matrix image2 = gauss.Apply(image);

            SsimErrorMeasure ssim = new SsimErrorMeasure();

            for (impulseProbability = 0.05f; impulseProbability <= 0.5f; impulseProbability += 0.05f)
            {
                ImpulseNoiseMatrixFilter impulse = new ImpulseNoiseMatrixFilter(impulseProbability);

                Matrix image3 = impulse.Apply(image2);

                float optimumAtmmseAlpha = impulseProbability / 2;
                int optimumAtmmseWindowSize = (int)(Math.Round(1 / Math.Sqrt(optimumAtmmseAlpha)));
                optimumAtmmseWindowSize += 1 - optimumAtmmseWindowSize % 2;

                MinimalMeanSquareErrorMatrixFilter mmse = new MinimalMeanSquareErrorMatrixFilter(optimumAtmmseWindowSize, noiseVariance);
                MmsePlusAtmMatrixFilter mmseAtm = new MmsePlusAtmMatrixFilter(optimumAtmmseAlpha, optimumAtmmseWindowSize, noiseVariance);
                AlphaTrimmedMmseMatrixFilter atmmse = new AlphaTrimmedMmseMatrixFilter(optimumAtmmseWindowSize, noiseVariance, optimumAtmmseAlpha);
                AlphaTrimmedMmsePlusAtmMatrixFilter atmmseAtm = new AlphaTrimmedMmsePlusAtmMatrixFilter(optimumAtmmseWindowSize, noiseVariance, optimumAtmmseAlpha);

                Matrix mmseImage = mmse.Apply(image3);
                Matrix mmseAtmImage = mmseAtm.Apply(image3);
                Matrix atmmseImage = atmmse.Apply(image3);
                Matrix atmmseAtmImage = atmmseAtm.Apply(image3);


                string text = "Impulse Probability = " + impulseProbability.ToString() + "\r\n" +
                                "MSSIM for MMSE = " + ssim.Measure(image, mmseImage).ToString() + "\r\n" +
                                "MSE for MMSE = " + AcuityEngine.MeanSquareError(image, mmseImage).ToString() + "\r\n" +
                                "MSSIM for MMSE+ATM = " + ssim.Measure(image, mmseAtmImage).ToString() + "\r\n" +
                                "MSE for MMSE+ATM = " + AcuityEngine.MeanSquareError(image, mmseAtmImage).ToString() + "\r\n" +
                                "MSSIM for ATMMSE = " + ssim.Measure(image, atmmseImage).ToString() + "\r\n" +
                                "MSE for ATMMSE = " + AcuityEngine.MeanSquareError(image, atmmseImage).ToString() + "\r\n" +
                                "MSSIM for ATMMSE+ATM = " + ssim.Measure(image, atmmseAtmImage).ToString() + "\r\n" +
                                "MSE for ATMMSE+ATM = " + AcuityEngine.MeanSquareError(image, atmmseAtmImage).ToString() + "\r\n" +
                                "\r\n";
                TextItem textItem = new TextItem(text, font);
                ric.Items.Add(textItem);

                SaveImageForFilters("mmse_" + impulseProbability.ToString() + ".bmp", mmseImage);
                SaveImageForFilters("atmmse_" + impulseProbability.ToString() + ".bmp", atmmseImage);
            }
        }

        private void SaveImageCommand(string input, string[] args)
        {
            //System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images\\output3");

            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                Point pt = new Point(//ligraControl1.ClientSize.Width, ligraControl1.ClientSize.Height);
                    ligraControl1.AutoScrollMinSize.Width, ligraControl1.AutoScrollMinSize.Height);
                Bitmap b = new Bitmap(pt.X, pt.Y, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                ligraControl1.DrawToBitmap(b, new Rectangle(0, 0, pt.X, pt.Y));
                b.Save(sfd.FileName);
            }
        }

        private static string ScaleAndFormatError(float errorScale, float error)
        {
            return (error * errorScale).ToString("G3");
        }

        private Matrix ApplyFilter(Matrix image, MatrixFilter filter, string caption, RenderItemContainer ric, Modulator mod, Matrix imageForError, bool useSsim, bool showTime)
        {
            float error = 0;

            string errorName;

            if (useSsim)
            {
                errorName = "MSSIM";
            }
            else
            {
                errorName = "MSE";
            }

            Matrix mat;

            int time1 = Environment.TickCount;
            mat = filter.Apply(image);
            int time2 = Environment.TickCount;

            if (imageForError != null)
            {
                Matrix after = mat;

                if (mat.ColumnCount != image.ColumnCount ||
                    mat.RowCount != image.RowCount)
                {
                    after = mat.GetSlice((mat.RowCount - image.RowCount) / 2,
                        (mat.ColumnCount - image.ColumnCount) / 2,
                        image.RowCount, image.ColumnCount);
                }

                if (useSsim)
                {
                    SsimErrorMeasure em = new SsimErrorMeasure();
                    error = em.Measure(after, imageForError);
                }
                else
                {
                    error = 65536 * AcuityEngine.MeanSquareError(after, imageForError);
                }
                caption += "  " + errorName + " = " + error.ToString("G3");
            }

            if (showTime)
            {
                caption += "  " + (time2 - time1).ToString() + "ms";
            }

            AddMatrixImage(caption, ric, mat, mod);
            return mat;
        }

        private void AddMatrixImage(string caption, RenderItemContainer ric, Matrix mat, Modulator mod)
        {
            Matrix mat2;
            mat2 = mat.Clone();
            if (mod != null)
            {
                mat2.ApplyToAll(mod);
            }
            mat2.ApplyToAll(AcuityEngine.ConvertFloatTo24g);
            if (ric == null)
            {
                _renderItems.Add(new GraphMatrixItem(mat2, caption));
            }
            else
            {
                ric.Items.Add(new GraphMatrixItem(mat2, caption));
            }
        }

        private Matrix LoadImageForFilters(string filename, string caption, RenderItemContainer ric, bool addToRenderItemsOrContainer)
        {
            Matrix image = AcuityEngine.LoadImage2(filename);

            if (addToRenderItemsOrContainer)
            {
                if (ric == null)
                {
                    _renderItems.Add(new GraphMatrixItem(image, caption));
                }
                else
                {
                    ric.Items.Add(new GraphMatrixItem(image, caption));
                }
            }
            image.ApplyToAll(AcuityEngine.Convert24gToFloat);
            return image;
        }

        private void SaveImageForFilters(string filename, Matrix image)
        {
            AcuityEngine.SaveImage2(filename, image);
        }

    }
}


