using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            double[,] data = getTrainData();
            int[] best_node = HillClimbingSearch(data);

            Console.WriteLine("");
            Console.WriteLine("Indices of best nodes are :");
             for (int i=0; i<best_node.Length; i++)
             {
                 Console.WriteLine("Factor : " + best_node[i]);
             }
            Console.ReadKey();
        }

        private static int[] HillClimbingSearch(double[,] data)
        {
            double initialState = 0.0;
            bool r = true;
            double GoalState = 1.0;
            int Fac = data.GetLength(1) - 1;
            List<int> BestFactors = new List<int>();
            List<int> RemainFactors = new List<int>();

            for (int i = 0; i < Fac; i++) RemainFactors.Add(i);
            int round = 0;
            double CurrentState = initialState;
            double result = 0.0;
            while (r == true && !initialState.Equals(GoalState))
            {
                List<int> ScoreFac = new List<int>(BestFactors);
                
                for (int i = 0; i < RemainFactors.Count; i++)
                {
                    ScoreFac.Add(i);
                    ScoreFac.Add(Fac);
                    double[,] x = getMultiColumn(data, ScoreFac.ToArray());
                    result = ComputeCorrelationSum(ScoreFac.Count - 1, ComputeFactorEffort(x), ComputeFactorFactor(x));
                    ScoreFac = new List<int>(BestFactors);
                    if (result > CurrentState)
                    {
                        Console.WriteLine("-------------------Layer : " + ++round + "--------------------");
                        BestFactors.Add(RemainFactors.ElementAt(i));
                        CurrentState = result;
                        Console.WriteLine("The Factors : " + i + " = " + result);
                        break;
                    }
                    else
                    {
                        r = false;
                    }
                }

               
            }
            return BestFactors.ToArray();
        }
        private static double[,] mapArrays(double[] fac, double[] eff)
        {
            double[,] newArrays = new double[fac.Length, 2];
            for(int i=0;(i<fac.Length)&(i<fac.Length); i++)
            {
                newArrays[i, 0] = fac[i];
                newArrays[i, 1] = eff[i];
            }
            return newArrays;
        }

        private static double[,] getMultiColumn(double[,] data, int[] index)
        {
            int rowLen = data.GetLength(0);
            int colLen = index.Length;
            double[,] combineData = new double[rowLen, colLen];
            for (int i = 0; i < rowLen; i++)
            {
                for (int j = 0; j < colLen; j++)
                {
                    combineData[i, j] = data[i, index[j]];
                }
            }
            return combineData;
        }
        private static double[] getColumn(double[,] data, int column)
        {
            int row = data.GetLength(0);
            double[] x = new double[row];

            for (int i=0; i<row; i++)
            {
                x[i] = data[i, column];
            }
            return x;
        }
        private static bool IsDuplicate(double[] x, double[] y)
        {
            double[] ax = x;
            double[] ay = y;
            Array.Sort(ax);
            Array.Sort(ay);
            return Enumerable.SequenceEqual(ax, ay);
        }
        private static double ComputeCorrelationSum(int m, double Factor_Effort, double Factor_Factor)
        {
            
                return (m * Factor_Effort) / Math.Sqrt(m + (m * (m - 1) * Factor_Factor));
            

        }
        private static double ComputeFactorEffort(double[,] x)
        {
            double r_sum = 0;
            int x_length = x.GetLength(1) - 1;
            double[] Factor;
            double[] Effort = getColumn(x, x_length);
            for (int i = 0; i < x_length; i++)
            {
                Factor = getColumn(x, i);
                //double r = Math.Abs(Correlation(Factor, Effort));
                double r = Correlation(Factor, Effort);
                if (Double.IsNaN(r) || Double.IsInfinity(r)) r = 0;
                r_sum = r_sum + r;

            }
            return r_sum / x_length;
        }
        private static double ComputeFactorFactor(double[,] x)
        {
            double r_sum = 0;
            int count = 0;
            int x_length = x.GetLength(1) - 1;

            if (x_length == 1) return 1; 
            for (int i = 0; i < x_length - 1; i++)
            {
                for (int j = 1; j < x_length; j++)
                { 
                    double r = Correlation(getColumn(x, i), getColumn(x, j));
                    if (Double.IsNaN(r) || Double.IsInfinity(r)) r = 0;
                    r_sum = r_sum + r;
                    count++;
                }
            }
            return r_sum / count;
        
        }
        private static int maxIndex(List<double> x)
        {
            int indexMax
                = !x.Any() ? -1 :
                x
                .Select((value, index) => new { Value = value, Index = index })
                .Aggregate((a, b) => (a.Value > b.Value) ? a : b)
                .Index;
            return indexMax;
        }
        private static double Correlation(IEnumerable<Double> xs, IEnumerable<Double> ys)
        {
            // sums of x, y, x squared etc.
            double sx = 0.0;
            double sy = 0.0;
            double sxx = 0.0;
            double syy = 0.0;
            double sxy = 0.0;

            int n = 0;

            using (var enX = xs.GetEnumerator())
            {
                using (var enY = ys.GetEnumerator())
                {
                    while (enX.MoveNext() && enY.MoveNext())
                    {
                        double x = enX.Current;
                        double y = enY.Current;

                        n += 1;
                        sx += x;
                        sy += y;
                        sxx += x * x;
                        syy += y * y;
                        sxy += x * y;
                    }
                }
            }

            // covariation
            double cov = sxy / n - sx * sy / n / n;
            // standard error of x
            double sigmaX = Math.Sqrt(sxx / n - sx * sx / n / n);
            // standard error of y
            double sigmaY = Math.Sqrt(syy / n - sy * sy / n / n);

            // correlation is just a normalized covariation
            return cov / sigmaX / sigmaY;
        }
        private static double[,] getTrainData()
        {
            double[,] x = { { 0.27, 3.72, 1.01, 5.65, 3.29, 4.68, 1, 0.9, 1, 1.24, 0.91, 1, 1, 0.87, 0.85, 0.88, 0.81, 1, 1, 1, 1, 0.8, 1, 208 },
                            {1.02,4.96,1.01,5.65,2.19,4.68,1,0.9,1,1.24,0.91,1,1,0.87,0.85,0.88,0.81,1.1,1,1,1,0.8,1,195 },
                            {2.52,3.72,1.01,5.65,3.29,4.68,1,0.9,1,1.24,0.91,1,1,0.87,0.85,0.88,0.81,1,1,1,1,0.8,1,162 },
                            { 4.02,3.72,3.04,2.83,0,3.12,1.1,1,1,1.07,1.11,1,1,0.87,0.85,0.88,0.81,0.88,0.91,0.91,1,0.8,1,113 },
                            { 4.28,4.96,1.01,5.65,3.29,4.68,1,0.9,1.17,1.24,0.91,1,1,0.87,0.85,0.88,0.81,1.1,1,1,1,0.8,1,277},
                            { 4.48,4.96,1.01,5.65,1.1,4.68,1.1,0.9,1,1,0.91,1,1,0.87,0.85,0.88,0.81,1.1,0.91,0.91,1,0.8,1,175},
                            { 5,4.96,1.01,5.65,1.1,4.68,1,0.9,1,1,0.91,1,1,0.87,0.85,0.88,0.81,1,0.91,0.91,1,0.8,1,120},
                            { 5.11,2.48,1.01,5.65,1.1,4.68,1,0.9,1,1,0.91,1,1,0.87,0.85,0.88,0.81,0.88,0.91,0.91,1,0.8,1,189},
                            { 5.29,1.24,1.01,5.65,0,4.68,1,0.9,1,1,0.91,1,1,0.87,0.85,0.88,0.81,1.1,1.19,1.2,1,0.8,1,417},
                            { 7.44,4.96,1.01,5.65,1.1,4.68,1,0.9,1.17,1,0.91,1,1,0.87,0.85,0.88,0.81,1.1,0.91,0.91,1,0.8,1,302},
                            { 7.7,4.96,1.01,5.65,0,4.68,1,0.9,1,1,0.91,1,1,0.87,0.85,0.88,0.81,1.1,1,0.91,1,0.8,1,567},
                            { 8.41,4.96,1.01,5.65,1.1,4.68,1,0.9,1,1,0.91,1,1,0.87,0.85,0.88,0.81,1,0.91,0.91,1,0.8,1,153},
                            { 8.8,4.96,1.01,5.65,2.19,4.68,1,0.9,1,1,0.91,1,1,0.87,0.85,0.88,0.81,1.1,1.09,1,1,0.8,1,142},
                            { 9.99,4.96,1.01,5.65,3.29,4.68,1,0.9,1,1.24,0.91,1,1,0.87,0.85,0.88,0.81,1.1,1,1,1,0.8,1,526},
                            { 25.84,4.96,1.01,5.65,1.1,4.68,1.1,0.9,1,1,0.91,1,1,0.87,0.85,0.88,0.81,1.1,0.91,0.91,1,0.8,1,202},
                            { 26.22,4.96,1.01,4.24,1.1,4.68,1,0.9,1,0.95,1,1,1,0.87,0.85,1,0.81,0.81,0.91,0.91,1,0.8,1,175},
                            { 30.13,3.72,1.01,5.65,2.19,4.68,1,0.9,1,1,0.91,1,1,0.87,0.85,0.88,0.81,1,1.09,1,1,0.8,1,206},
                            { 32.56,4.96,1.01,5.65,2.19,4.68,1,0.9,1,1.24,0.91,1,1,0.87,0.85,0.88,0.81,1.1,0.91,0.91,1,0.8,1,794},
                            { 36.27,4.96,1.01,5.65,1.1,4.68,1,0.9,1,1.24,0.91,1,1,0.87,0.85,0.88,0.81,1.1,0.91,0.91,1,0.8,1,667},
                            { 38.85,4.96,1.01,4.24,1.1,4.68,1,0.9,1,0.95,1,1,1,0.87,0.85,1,0.81,0.81,0.85,0.84,1,0.8,1,230},
                            { 42.81,3.72,3.04,5.65,1.1,4.68,1,0.9,0.87,1,0.91,1,1,0.87,0.85,0.88,0.81,1,1.09,1.09,1,0.8,1,439},
                            { 42.9,2.48,1.01,5.65,2.19,4.68,1,0.9,1,1,0.91,1,1,0.87,0.85,0.88,0.81,0.88,0.91,0.91,1,0.8,1,361},
                            { 45.14,2.48,3.04,5.65,1.1,4.68,1,0.9,0.87,1,0.91,1,1,0.87,0.85,0.88,0.81,0.88,1,1,1,0.8,1,486},
                            { 55.55,4.96,1.01,5.65,1.1,4.68,1,0.9,1,1,0.91,1,1,0.87,0.85,0.88,0.81,1.1,0.91,0.91,1,0.8,1,763},
                            { 57.77,2.48,2.03,1.41,0,3.12,1.1,1,1,1.07,1.11,1.29,1,0.87,0.85,0.88,0.81,0.88,0.91,0.91,1,0.8,1,405},
                            { 59.14,1.24,1.01,5.65,2.19,4.68,1,0.9,1,1,0.91,1,1,0.87,0.85,0.88,0.81,0.81,0.85,0.84,1,0.8,1,331},
                            { 62.78,6.2,3.04,4.24,0,3.12,1,1,1,1.07,1.11,1,1,0.87,0.85,0.88,0.81,0.88,0.91,0.91,1,0.8,1,422},
                            { 67.32,1.24,1.01,5.65,0,4.68,1,0.9,1,1,0.91,1,1,0.87,0.85,0.88,0.81,0.81,0.85,0.84,1,0.8,1,234},
                            { 79.82,4.96,1.01,5.65,1.1,4.68,1,0.9,1,1,0.91,1,1,0.87,0.85,0.88,0.81,1,0.91,0.91,1,0.8,1,636},
                            { 112.28,4.96,1.01,5.65,1.1,4.68,1,0.9,1,1,0.91,1,1,0.87,0.85,0.88,0.81,1.1,0.91,0.91,1,0.8,1,1278} };
            return x;
        }
    }

}
