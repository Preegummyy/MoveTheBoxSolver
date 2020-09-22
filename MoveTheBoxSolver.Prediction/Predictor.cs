using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using ReadWriteCsv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MoveTheBoxSolver.Prediction
{
    public class Predictor
    {
        public Assembly assembly { get; set; }
        public int[] feature_index { get; set; }

        public Predictor(Assembly assembly)
        {
            this.assembly = assembly;

            List<int> feature_index = new List<int>();
            Stream feature_index_stream = assembly.GetManifestResourceStream("MoveTheBoxSolver.feature.feature_index.csv");
            using (CsvFileReader reader = new CsvFileReader(feature_index_stream))
            {
                CsvRow row = new CsvRow();
                while (reader.ReadRow(row))
                {
                    foreach (string s in row)
                    {
                        feature_index.Add(int.Parse(s, System.Globalization.NumberStyles.Float));
                    }
                }
            }

            this.feature_index = feature_index.ToArray();
        }

        public async Task<int> PredictUnUsedMoveAsync(double[] image)
        {
            return await Task.Run(() =>
            {
                if (image == null || image.Length != 7581600)
                {
                    return -1;
                }

                var image_feature = FeatureSelection(image, feature_index);

                var bias = new List<double[]>();
                for (int i = 0; i < 3; i++)
                {
                    bias.Add(ReadBias($"bias_of_unuse_move{i}.csv"));
                }

                var weight = new List<double[][]>();
                for (int i = 0; i < 3; i++)
                {
                    weight.Add(ReadWeight($"weight_of_unuse_move{i}.csv"));
                }

                return PredictForward(image_feature, weight.ToArray(), bias.ToArray());
            });
        }

        public async Task<int> PredictMoveLimitAsync(double[] image)
        {
            return await Task.Run(() =>
            {
                if (image == null || image.Length != 7581600)
                {
                    return -1;
                }

                var image_feature = FeatureSelection(image, feature_index);

                var bias = new List<double[]>();
                for (int i = 0; i < 3; i++)
                {
                    bias.Add(ReadBias($"bias_of_move_limit{i}.csv"));
                }

                var weight = new List<double[][]>();
                for (int i = 0; i < 3; i++)
                {
                    weight.Add(ReadWeight($"weight_of_move_limit{i}.csv"));
                }

                return PredictForward(image_feature, weight.ToArray(), bias.ToArray());
            });
        }

        

        private int PredictForward(double[] image_feature, double[][][] weight, double[][] bias)
        {
            List<double[]> output = new List<double[]>();
            List<double[]> activated_output = new List<double[]>();
            for (int i = 0; i < 3; i++)
            {
                double[] output_layer_i;
                if (i <= 0)
                {
                    var output_layer_i_arr = Plus(Dot(image_feature, weight[i]), bias[i]);
                    output_layer_i = output_layer_i_arr[0];
                }
                else
                {
                    var output_layer_i_arr = Plus(Dot(activated_output.Last(), weight[i]), bias[i]);
                    output_layer_i = output_layer_i_arr[0];
                }

                double[] activated_output_i = new double[output_layer_i.Length];
                if (i <= 1)
                {
                    //layer 0,1 using PReLU 0.1
                    for (int j = 0; j < output_layer_i.Length; j++)
                    {
                        if (output_layer_i[j] <= 0)
                        {
                            activated_output_i[j] = output_layer_i[j] * 0.1;
                        }
                        else
                        {
                            activated_output_i[j] = output_layer_i[j];
                        }
                    }
                }
                else
                {
                    //layer 2 using softmax
                    var exp_output_i_arr = Exp(output_layer_i);
                    var exp_output_i = exp_output_i_arr[0];
                    var sum_exp_output_i = Sum(exp_output_i);
                    var activated_output_i_arr = Devine(exp_output_i, sum_exp_output_i);
                    activated_output_i = activated_output_i_arr[0];
                    //activated_output_i = Devine(Exp(output_layer_i)[0], Sum(Exp(output_layer_i)[0]))[0];
                }
                activated_output.Add(activated_output_i);
            }

            return Argmax(activated_output.Last());
        }

        private double[] ReadBias(string file_name)
        {
            List<double> bias_i = new List<double>();
            Stream bias_i_stream = assembly.GetManifestResourceStream("MoveTheBoxSolver.bias." + file_name);
            using (CsvFileReader reader = new CsvFileReader(bias_i_stream))
            {
                CsvRow row = new CsvRow();
                while (reader.ReadRow(row))
                {
                    foreach (string s in row)
                    {
                        bias_i.Add(double.Parse(s, System.Globalization.NumberStyles.Float));
                    }
                }
            }
            return bias_i.ToArray();
        }

        private double[][] ReadWeight(string file_name)
        {
            List<double[]> weight_i = new List<double[]>();
            Stream weight_i_stream = assembly.GetManifestResourceStream("MoveTheBoxSolver.weight." + file_name);
            using (CsvFileReader reader = new CsvFileReader(weight_i_stream))
            {
                CsvRow row = new CsvRow();
                while (reader.ReadRow(row))
                {
                    var weight_i_j = new List<double>();
                    foreach (string s in row)
                    {
                        weight_i_j.Add(double.Parse(s, System.Globalization.NumberStyles.Float));
                    }
                    weight_i.Add(weight_i_j.ToArray());
                }
            }
            return weight_i.ToArray();
        }

        private double[] FeatureSelection(double[] image, int[] feature_index_arr)
        {
            List<double> selected_feature = new List<double>();
            feature_index_arr = feature_index_arr.OrderBy(x => x).ToArray();
            foreach (var pixal in feature_index_arr)
            {
                selected_feature.Add(image[pixal]);
            }

            return selected_feature.ToArray();
        }

        private double[][] Plus(double[][] metrix_a, double[] metrix_b)
        {
            Matrix<double> A = DenseMatrix.OfColumnArrays(metrix_a).Transpose();
            Matrix<double> B = DenseMatrix.OfColumnArrays(metrix_b).Transpose();
            var result = A + B;
            return result.Transpose().ToColumnArrays();
        }

        private double[][] Dot(double[] metrix_a, double[][] metrix_b)
        {
            Matrix<double> A = DenseMatrix.OfColumnArrays(metrix_a).Transpose();
            Matrix<double> B = DenseMatrix.OfColumnArrays(metrix_b).Transpose();
            var result = A * B;
            return result.Transpose().ToColumnArrays();
        }

        private double[][] Exp(double[] metrix_a)
        {
            Matrix<double> A = DenseMatrix.OfColumnArrays(metrix_a).Transpose();
            var result = Matrix.Exp(A);
            return result.Transpose().ToColumnArrays();
        }

        private double Sum(double[] metrix_a)
        {
            var sum = 0.0;
            foreach (var item in metrix_a)
            {
                sum += item;
            }
            return sum;
        }

        private double[][] Devine(double[] metrix_a, double constant_a)
        {
            Matrix<double> A = DenseMatrix.OfColumnArrays(metrix_a).Transpose();
            var result = A / constant_a;
            return result.Transpose().ToColumnArrays();
        }

        private int Argmax(double[] metrix_a)
        {
            double max_column = 0;
            int index = -1;
            for (int i = 0; i < metrix_a.Length; i++)
            {
                if (max_column < metrix_a[i])
                {
                    max_column = metrix_a[i];
                    index = i;
                }
            }
            return index;
        }
    }
}