﻿namespace Nca.Valdr
{
    using System;
    using System.IO;
    using System.Reflection;
    public static class Program
    {
        private const string ProgramId = "Nca.Valdr";

        /// <summary>
        /// CLI main entry
        /// </summary>
        /// <param name="args">CLI parameters.</param>
        public static void Main(string[] args)
        {
            Console.Title = ProgramId;
            string assemblyFile = null;
            string targetNamespace = string.Empty;
            string outputFile = null;
            string application = "app";

            foreach (var arg in args)
            {
                switch (arg.Substring(0, 2))
                {
                    case "-i":  // Input assembly path
                        assemblyFile = arg.Substring(3);
                        break;
                    case "-n":  // Target namespace
                        targetNamespace = arg.Substring(3);
                        break;
                    case "-o":  // Output file path
                        outputFile = arg.Substring(3);
                        break;
                    case "-a":  // Angular application name
                        application = arg.Substring(3);
                        break;
                    default:
                        throw new ArgumentException("Invalid parameter.");
                }
            }

            if (string.IsNullOrEmpty(assemblyFile) ||
                string.IsNullOrEmpty(outputFile))
            {
                throw new ArgumentNullException("args");
            }

            Console.WriteLine("{0} -> {1}", ProgramId, outputFile);

            try
            {
                var result = Parser.Parse(assemblyFile, targetNamespace);
                using (var writer = new StreamWriter(outputFile))
                {
                    writer.WriteLine("/*! Auto-generated. This file was generated by {0}. */", ProgramId);
                    writer.WriteLine("(function() {");
                    writer.WriteLine("    \"use strict\";");
                    writer.WriteLine("    angular");
                    writer.WriteLine("        .module(\"{0}\")", application);
                    writer.WriteLine("        .config(config);");
                    writer.WriteLine("    config.$inject = [\"valdrProvider\"];");
                    writer.WriteLine("    function config(valdrProvider) {");
                    writer.WriteLine("        valdrProvider.addConstraints({0});", result);
                    writer.WriteLine("}})();");
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (var item in ex.LoaderExceptions)
                {
                    Console.WriteLine(item.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

#if DEBUG
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(true);
#endif
        }
    }
}
