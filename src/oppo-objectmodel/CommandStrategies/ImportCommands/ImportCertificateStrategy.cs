using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using Newtonsoft.Json.Linq;
using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;

namespace Oppo.ObjectModel.CommandStrategies.ImportCommands
{
    public class ImportCertificateStrategy : ICommand<ImportStrategy>
    {
        private enum ParamId { Key, Certificate, Project, CertFormat, KeyFormat, ForServer, ForClient }
        private enum FileFormat { DER, PEM, Empty }
            
        private readonly IFileSystem _fileSystem;
        private readonly ParameterResolver<ParamId> _resolver;

        public ImportCertificateStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _resolver = new ParameterResolver<ParamId>(Constants.CommandName.Import + " " + Name, new []
            {
                new StringParameterSpecification<ParamId>
                {
                    Identifier = ParamId.Key,
                    Short = Constants.ImportCertificateCommandArguments.Key,
                    Verbose = Constants.ImportCertificateCommandArguments.VerboseKey,
                },
                new StringParameterSpecification<ParamId>
                {
                    Identifier = ParamId.Certificate,
                    Short = Constants.ImportCertificateCommandArguments.Certificate,
                    Verbose = Constants.ImportCertificateCommandArguments.VerboseCertificate,
                },  
                new StringParameterSpecification<ParamId>
                {
                    Identifier = ParamId.Project,
                    Short = Constants.ImportCertificateCommandArguments.Project,
                    Verbose = Constants.ImportCertificateCommandArguments.VerboseProject,
                }
            }, new []
            {
                new BoolParameterSpecification<ParamId>
                {
                    Identifier = ParamId.ForServer,
                    Verbose = Constants.ImportCertificateCommandArguments.VerboseServer
                },
                new BoolParameterSpecification<ParamId>
                {
                    Identifier = ParamId.ForClient,
                    Verbose = Constants.ImportCertificateCommandArguments.VerboseClient
                }
            });
        }

        public string Name => Constants.CommandName.ImportCertificate;
        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var (error, stringParams, options) = _resolver.ResolveParams(inputParams);

            if (error != null)
                return new CommandResult(false, new MessageLines{{error, string.Empty}});

            var project = stringParams[ParamId.Project];
            var certificate = stringParams[ParamId.Certificate];
            var key = stringParams[ParamId.Key];
            var forServer = options[ParamId.ForServer];
            var forClient = options[ParamId.ForClient];
            
            string GetExtension(string str) => str.Substring(Math.Max(0, str.Length - 3));

            var isCertPEM = GetExtension(certificate) != "der";
            var isKeyPEM = GetExtension(key) != "der";
            
            if (forClient && forServer)
                return FailureWrongClientServer();
            
            var oppoProjContent = _fileSystem.ReadFile(_fileSystem.CombinePaths(project, project + Constants.FileExtension.OppoProject));
            string appType;
            using (var reader = new StreamReader(oppoProjContent, Encoding.ASCII))
            {
                appType = (string) JObject.Parse(reader.ReadToEnd())["type"];
            }

            if (appType != Constants.ApplicationType.ClientServer && (forClient || forServer))
                    return FailureWrongClientServer();
            
            var keyTarget = Constants.FileName.PrivateKeyDER;
            var certTarget = Constants.FileName.Certificate;

            if (appType == Constants.ApplicationType.ClientServer)
            {
                string prefix;
                if (forServer)
                    prefix = Constants.FileName.ServerCryptoPrefix;
                else if (forClient)
                    prefix = Constants.FileName.ClientCryptoPrefix;
                else
                    return FailureMissingClientServer();
                
                certTarget = prefix + "_" + Constants.FileName.Certificate;
                keyTarget = prefix + "_" + Constants.FileName.PrivateKeyDER;
            }
            
            Import(project, isKeyPEM,Constants.ExternalExecutableArguments.OpenSSLConvertKeyFromPEM, key, keyTarget);
            Import(project, isCertPEM, Constants.ExternalExecutableArguments.OpenSSLConvertCertificateFromPEM, certificate, certTarget);

            OppoLogger.Info(string.Format(LoggingText.ImportCertificateSuccess, certificate, key));
            return new CommandResult(true, new MessageLines{{OutputText.ImportCertificateCommandSuccess, string.Empty}});
        }

        private void Import(string appName, bool isPEM, string openSSLArgsFmt, string source, string targetFileName)
        {
            var targetPath = _fileSystem.CombinePaths(appName, targetFileName);
            if (isPEM)
            {
                var openSSLArgs = string.Format(openSSLArgsFmt, source, targetPath);
                _fileSystem.CallExecutable(Constants.ExecutableName.OpenSSL, null, openSSLArgs);
            }
            else
            {
                _fileSystem.CopyFile(source, appName);
            }
        }

        private static CommandResult FailureWrongClientServer()
        {
            OppoLogger.Warn(LoggingText.ImportCertificateFailureWrongClientServer);
            return new CommandResult(false, new MessageLines{{OutputText.ImportCertificateCommandWrongServerClient, string.Empty}});
        }
        
        private static CommandResult FailureMissingClientServer()
        {
            OppoLogger.Warn(LoggingText.ImportCertificateFailureMissingClientServer);
            return new CommandResult(false, new MessageLines{{OutputText.ImportCertificateCommandWrongServerClient, string.Empty}});
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}