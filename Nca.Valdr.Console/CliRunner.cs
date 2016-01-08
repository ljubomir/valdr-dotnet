﻿using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Nca.Valdr.Console
{
    /// <summary>
    /// Runs the request and creates the output file
    /// </summary>
    public class CliRunner
    {
        private const string ProgramId = "Nca.Valdr";
        private readonly CliOptions _options;
        private readonly CliWriter _writer;

        public CliRunner(CliOptions options, CliWriter writer)
        {
            _options = options;
            _writer = writer;
        }

        public bool Execute()
        {
            _writer.WriteLine("{0} -> {1} {2}", ProgramId, _options.OutputFilename, _options.Culture);

            try
            {
                var parser = new Parser(_options.AssemblyFilename, _options.TargetNamespace, _options.Culture);
                var result = parser.Parse();
                var output = BuildJavaScript(_options.Application, result.ToString());
                using (var writer = new StreamWriter(_options.OutputFilename))
                {
                    writer.Write(output);
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (var item in ex.LoaderExceptions)
                {
                    _writer.WriteLine(item.Message);
                }
            }
            catch (Exception ex)
            {
                _writer.WriteLine(ex.Message);
            }

            return true;
        }

        /// <summary>
        /// Build JavaScript
        /// </summary>
        /// <param name="application">Application name.</param>
        /// <param name="metadata">Valdr metadata.</param>
        /// <returns></returns>
        public static string BuildJavaScript(string application, string metadata)
        {
            var output = new StringBuilder();
            output.AppendLine($"/*! Auto-generated. This file was generated by {ProgramId}. */");
            output.AppendLine("(function() {");
            output.AppendLine("    \"use strict\";");
            output.AppendLine("    angular");
            output.AppendLine($"        .module(\"{application}\")");
            output.AppendLine("        .config(config);");
            output.AppendLine("    config.$inject = [\"valdrProvider\"];");
            output.AppendLine("    function config(valdrProvider) {");
            output.AppendLine($"        valdrProvider.addConstraints({metadata});");
            output.AppendLine("}})();");

            return output.ToString();
        }
    }
}