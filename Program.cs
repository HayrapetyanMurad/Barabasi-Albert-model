using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Collections;

namespace bianconi_barabasi_model
{
    class Program
    {
        static void Main(string[] args)
        {
            OptParser parser = new OptParser();
            if (!parser.init(args))
            {
                Console.WriteLine("Invalid arguments");
                parser.print_usage();
                return;
            }

            BianconiBarabasiModel model = new BianconiBarabasiModel(parser.vertexes, parser.edgespervertex, parser.function);
            model.generate_network();

            BitArray[] network = model.get_network();
            double[] fitnesses = model.get_fitnesses();

            string[] network_print = BitArrayToString(network);
            string[] fitnesses_print = DoubleArrayToString(fitnesses);

            if (parser.output_file.Length == 0)
            {
                for (int i = 0; i < network.Length; i++)
                {
                    Console.WriteLine(network_print[i]);
                }
                Console.WriteLine();

                for (int i = 0; i < fitnesses.Length; i++)
                {
                    Console.WriteLine(fitnesses_print[i]);
                }
            }
            else
            {
                Debug.Assert(File.Exists(parser.output_file), "incorrect path" + parser.output_file);

                File.WriteAllLines(parser.output_file, network_print);
                File.AppendAllText(parser.output_file, Environment.NewLine);
                File.AppendAllLines(parser.output_file, fitnesses_print);
            }

        }

        static string[] BitArrayToString(BitArray[] array)
        {
            string[] result = new string[array.Length];
            for (int i=0; i<array.Length; ++i)
            {
                result[i] = "";
                for(int j=0; j<array[i].Length; ++j)
                {
                    result[i] += (array[i][j]) ? "1" : "0";
                }

            }
            return result;
        }

        static string[] DoubleArrayToString(double[] array)
        {
            string[] result = new string[array.Length];
            for(int i=0; i<array.Length; ++i)
            {
                result[i] = array[i].ToString();
            }

            return result;
        }
    }

    struct OptParser
    {
        public int vertexes;
        public int edgespervertex;
        public string function;
        public string output_file;

        public void print_usage()
        {
            Console.WriteLine("Usage: \n" +
                "\t-vertex_count <value> (optional: default value: 5)\n" +
                "\t\t number of vertexes for network\n" +
                "\t-edges_per_vertex <value> (optional: default value: 1)\n" +
                "\t\t number of edges per vertex for network\n" +
                "\t-function <value> (optional: default value: 1)\n" +
                "\t\t probability density function for network, should be expression with numbers, argument x\n" +
                "\t\t operators +,-,*,/,^ and functions sin, cos, tan, ln, sqrt, brackets can be used\n" +
                "\t\t function should be continuous and positive in [0,1] interval \n" +
                "\t-output_file <value> (optional: default value: <empty>)\n" +
                "\t\t file where the output network will be written,\n " +
                "\t\t if not specified network will be written into output stream\n");
        }
        public bool init(string[] args)
        {
            vertexes = 5;
            edgespervertex = 1;
            function = "1";
            output_file = "";

            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i] == "-vertex_count")
                {
                    if (i + 1 == args.Length)
                    {
                        return false;
                    }

                    bool res = Int32.TryParse(args[i + 1], out vertexes);
                    if (res == false)
                    {
                        return false;
                    }
                    i++;
                }
                else if (args[i] == "-edges_per_vertex")
                {
                    if (i + 1 == args.Length)
                    {
                        return false;
                    }

                    bool res = Int32.TryParse(args[i + 1], out edgespervertex);
                    if (res == false)
                    {
                        return false;
                    }
                    i++;
                }
                else if (args[i] == "-function")
                {
                    if (i + 1 == args.Length)
                    {
                        return false;
                    }

                    function = args[i + 1];
                    i++;
                }
                else if (args[i] == "-output_file")
                {
                    if (i + 1 == args.Length)
                    {
                        return false;
                    }

                    output_file = args[i + 1];
                    i++;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}
