/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Appio.ObjectModel;
using Newtonsoft.Json.Linq;
using Appio.Resources.text.logging;
using Appio.Resources.text.output;

namespace Appio.ObjectModel.CommandStrategies.GenerateCommands
{
    public class GenerateCertificateStrategy : ICommand<GenerateStrategy>
    {
        private enum ParamId { AppName, KeySize, Days, Org }
        
        private readonly AbstractCertificateGenerator _certificateGenerator;
        private readonly ParameterResolver<ParamId> _resolver;
        private readonly IFileSystem _fileSystem;

        public GenerateCertificateStrategy(IFileSystem fileSystem, AbstractCertificateGenerator certificateGenerator)
        {
            _fileSystem = fileSystem;
            _certificateGenerator = certificateGenerator;
            _resolver = new ParameterResolver<ParamId>(Constants.CommandName.Generate + " " + Name, new []
            {
                new StringParameterSpecification<ParamId>
                {
                    Identifier = ParamId.AppName,
                    Short = Constants.GenerateCertificateCommandArguments.AppName,
                    Verbose = Constants.GenerateCertificateCommandArguments.VerboseAppName
                }, 
                new StringParameterSpecification<ParamId>
                {
                    Identifier = ParamId.KeySize,
                    Verbose = Constants.GenerateCertificateCommandArguments.VerboseKeySize,
                    Default = Constants.ExternalExecutableArguments.OpenSSLDefaultKeySize.ToString()
                }, 
                new StringParameterSpecification<ParamId>
                {
                    Identifier = ParamId.Days,
                    Verbose = Constants.GenerateCertificateCommandArguments.VerboseDays,
                    Default = Constants.ExternalExecutableArguments.OpenSSLDefaultDays.ToString()
                }, 
                new StringParameterSpecification<ParamId>
                {
                    Identifier = ParamId.Org,
                    Verbose = Constants.GenerateCertificateCommandArguments.VerboseOrganization,
                    Default = Constants.ExternalExecutableArguments.OpenSSLDefaultOrganization
                }
            });
        }

        public string Name => Constants.GenerateCommandArguments.Certificate;
        
        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var (error, stringParams, _) = _resolver.ResolveParams(inputParams);
            
            if (error != null)
                return new CommandResult(false, new MessageLines{{error, string.Empty}});

            var appName = stringParams[ParamId.AppName];

            uint keySize, days;
            
            try
            {
                keySize = uint.Parse(stringParams[ParamId.KeySize]);
                days = uint.Parse(stringParams[ParamId.Days]);
            }
            catch (FormatException)
            {
                AppioLogger.Warn(LoggingText.GenerateCertificateFailureNotParsable);
                return new CommandResult(false, new MessageLines{{OutputText.GenerateCertificateCommandFailureNotParsable, string.Empty}});
            }
            
            Stream appioprojContent;
            try
            {
                appioprojContent = _fileSystem.ReadFile(_fileSystem.CombinePaths(appName, appName + Constants.FileExtension.Appioproject));
            }
            catch (Exception)
            {
                AppioLogger.Warn(string.Format(LoggingText.GenerateCertificateFailureNotFound, appName));
                return new CommandResult(false, new MessageLines{{string.Format(OutputText.GenerateCertificateCommandFailureNotFound, appName), string.Empty}});
            }
            
            var reader = new StreamReader(appioprojContent, Encoding.ASCII);
            var appType = (string) JObject.Parse(reader.ReadToEnd())["type"];

            if (appType == Constants.ApplicationType.ClientServer)
            {
                _certificateGenerator.Generate(appName, Constants.FileName.ClientCryptoPrefix, keySize,
                    days, stringParams[ParamId.Org]);
                _certificateGenerator.Generate(appName, Constants.FileName.ServerCryptoPrefix, keySize,
                    days, stringParams[ParamId.Org]);
            }
            else
            {
                _certificateGenerator.Generate(appName, string.Empty, keySize, days, stringParams[ParamId.Org]);
            }
            
            AppioLogger.Info(LoggingText.GenerateCertificateSuccess);
            return new CommandResult(true, new MessageLines{{string.Format(OutputText.GenerateCertificateCommandSuccess, appName), string.Empty}});
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}