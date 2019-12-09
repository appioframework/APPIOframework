/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

namespace Appio.ObjectModel
{
    public static class Constants
    {
        public const string HelloString = "Hello from APPIO";
        public const string IncludeSnippet = "#include";
        public const string server = "server";
        public const string definitionXmlElement = "<Definition";
		public const string opcuaappConverterSerializationException = "Use default serialization.";
		public const string HelpOptionSeparator = " | ";

        public static class FileExtension
        {
            public const string Appiosln = ".appiosln";
            public const string Appioproject = ".appioproj";
            public const string DebianInstaller = ".deb";
            public const string InformationModel = ".xml";
            public const string ModelTypes = ".bsd";
            public const string ModelTypeDescriptions = ".csv";            
            public const string CFile = ".c";
			public const string ZipFile = ".zip";
			public const string DerFile = ".der";
			public const string PemCertificateFile = ".pem";
			public const string PemKeyFile = ".key";
        }

        public static class CommandName
        {
            public const string New = "new";
            public const string Hello = "hello";
            public const string Build = "build";
            public const string Publish = "publish";
            public const string Help = "help";
            public const string HelpDash = "-h";
            public const string HelpDashVerbose = "--help";
            public const string ShortHelp = "?";
            public const string Version = "version";
            public const string Clean = "clean";
            public const string Deploy = "deploy";
            public const string Import = "import";
            public const string ImportCertificate = "certificate";
            public const string Generate = "generate";
            public const string GenerateInformationModel = "information-model";
            public const string Sln = "sln";
            public const string Reference = "reference";
        }

        public static class CommandResults
        {
            public const string Success = "success";
            public static string Failure { get; set; } = "failure";
        }

        public static class SlnCommandArguments
        {
            public const string Add = "add";
            public const string Remove = "remove";
			public const string Build = "build";
			public const string Publish = "publish";
			public const string Deploy = "deploy";
        }
		
		public static class ReferenceCommandArguments
		{
			public const string Add = "add";
			public const string Remove = "remove";
		}

		public static class DirectoryName
        {
	        public const string Certificates = "certificates";
	        public const string SourceCode = "src";
            public const string ClientApp = "client";
            public const string ServerApp = "server";
            public const string MesonBuild = "build";
            public const string Publish = "publish";
            public const string Deploy = "deploy";
            public const string Temp = "temp";
            public const string OpcuaappInstaller = "appio-opcuaapp";
            public const string Usr = "usr";
            public const string Bin = "bin";
            public const string Models = "models";
            public const string InformationModels = "information-models";
        }

        public static class ExecutableName
        {
            public const string OpenSSL = "openssl";
            public const string Meson = "meson";
            public const string Ninja = "ninja";
            public const string AppClient = "client-app";
            public const string AppServer = "server-app";
            public const string CreateDebianInstaller = "dpkg";
            public static readonly string CreateDebianInstallerArguments = "--build " + DirectoryName.OpcuaappInstaller;
            public const string AppioResourcesDll = "appio-resources.dll";

            public const string PythonScript = @"python3";
            public static readonly string GenerateDatatypesScriptPath = System.IO.Path.Combine(new string[] { " ", "etc", "appio", "tools", "open62541", "v1.0.0", "python-scripts", "generate_datatypes.py" });
            // 0 bsd types source path
            public const string GenerateDatatypesTypeBsd = @" --type-bsd={0} --type-csv={1} --type-csv={2}";
            public static readonly string NodesetCompilerCompilerPath = System.IO.Path.Combine(new string[] { " ", "etc", "appio", "tools", "open62541", "v1.0.0", "python-scripts", "nodeset_compiler", "nodeset_compiler.py" });
            public const string NodesetCompilerInternalHeaders = @" --internal-headers";
            // 0 ua types
            public const string NodesetCompilerTypesArray = @" --types-array={0}";
            public const string NodesetCompilerBasicTypes = @"UA_TYPES";
            public const string NodesetCompilerExisting = @" --existing {0}";
            // 0 existing model xml source path
            public static readonly string NodesetCompilerBasicNodeset = System.IO.Path.Combine(new string[] { " ", "etc", "appio", "tools", "open62541", "v1.0.0", "existing-nodes", "Opc.Ua.NodeSet2.xml" });
            public static readonly string NodesetCompilerBasicNodeIdsTypeDescriptionsFile = System.IO.Path.Combine(new string[] {"etc", "appio", "tools", "open62541", "v1.0.0", "existing-nodes", "NodeIds.csv" });
            // 0 xml model source path
            // 1 output directory with name for generated files and method
            public const string NodesetCompilerXml = @" --xml {0} {1}";
        }

        public static class FileName
        {
            public const string SourceCode_main_c                   = "main.c";
			public const string SourceCode_globalVariables_h		= "globalVariables.h";
            public const string SourceCode_loadInformationModels_c  = "loadInformationModels.c";
			public const string SourceCode_constants_h				= "constants.h";
			public const string SourceCode_mainCallbacks_c			= "mainCallbacks.c";
            public const string SourceCode_meson_build              = "meson.build";
            public const string SampleInformationModelFile          = "DiNodeset.xml";
			public const string SampleInformationModelTypesFile     = "DiTypes.bsd";
            public const string SampleInformationModelTypeDescriptionsFile = "DiTypes.csv";            
            public const string CertificateConfig                   = "certConfig.cnf";
            public const string PrivateKeyDER                       = "priv.der";
            public const string PrivateKeyPEM                       = "priv.pem";
            public const string Certificate                         = "cert.der";
            public const string ServerCryptoPrefix                  = "server";
            public const string ClientCryptoPrefix                  = "client";
            public const string NodeIdsTypeDescriptionsFile = "NodeIds.csv";            
        }

        public static class ExternalExecutableArguments
        {
            public static readonly string OpenSSL = $"req -new -x509 -out {{0}}{FileName.Certificate} -outform der -nodes -keyout {{0}}{FileName.PrivateKeyPEM} -config {FileName.CertificateConfig} -days {{1}} -extensions v3";
            public const uint OpenSSLDefaultKeySize = 1024;
            public const uint OpenSSLDefaultDays = 365;
            public const string OpenSSLDefaultOrganization = "MyOrg";

            public const string OpenSSLConvertCertificateFromPEM = "x509 -inform PEM -outform DER -in {0} -out {1}";
            public const string OpenSSLConvertKeyFromPEM = "rsa -outform der -in {0} -out {1}";
        }
        
        public static class NewCommandOptions
        {
            public const string Name = "-n";
            public const string VerboseName = "--name";
			public const string Type = "-t";
			public const string VerboseType = "--type";
			public const string Url = "-u";
			public const string VerboseUrl = "--url";
			public const string Port = "-p";
			public const string VerbosePort = "--port";
            public const string VerboseNoCert = "--nocert";
			public const string Help = "-h";
			public const string VerboseHelp = "--help";
		}

		public static class NewCommandArguments
		{
			public const string Sln = "sln";
			public const string OpcuaApp = "opcuaapp";
		}


		public static class BuildCommandOptions
        {
            public const string Name = "-n";
            public const string VerboseName = "--name";
            public const string VerboseHelp = "--help";
            public const string Help = "-h";
        }

        public static class PublishCommandOptions
        {
            public const string Help = "-h";
            public const string VerboseHelp = "--help";
            public const string Name = "-n";
            public const string VerboseName = "--name";
        }

        public static class CleanCommandOptions
        {
            public const string Help = "-h";
            public const string VerboseHelp = "--help";
            public const string Name = "-n";
            public const string VerboseName = "--name";
        }

        public static class DeployCommandOptions
        {
            public const string Help = "-h";
            public const string VerboseHelp = "--help";
            public const string Name = "-n";
            public const string VerboseName = "--name";
        }

        public static class GenerateCommandOptions
        {
            public const string Name                    = "-n";
            public const string VerboseName             = "--name";
			public const string Help					= "-h";
			public const string VerboseHelp             = "--help";
        }
        
        public static class GenerateCertificateCommandArguments
        {
            public const string AppName                    = "-n";
            public const string VerboseAppName             = "--name";
            public const string VerboseKeySize             = "--keysize";
            public const string VerboseDays                = "--days";
            public const string VerboseOrganization                 = "--organization";
        }

        public static class ImportCommandArguments
        {
            public const string InformationModel = "information-model";
			public const string Certificate = "certificate";
        }

        public static class GenerateCommandArguments
        {
            public const string InformationModel = "information-model";
			public const string Certificate = "certificate";
        }

        public static class ImportCommandOptions
        {
            public const string Name = "-n";
            public const string VerboseName = "--name";
            public const string Path = "-p";
            public const string VerbosePath = "--path";
			public const string Types = "-t";
			public const string VerboseTypes = "--types";
            public const string TypeDescriptions = "-td";
            public const string VerboseTypeDescriptions = "--typedescriptions";
            public const string Help = "-h";
            public const string VerboseHelp = "--help";
            public const string Sample = "-s";
            public const string VerboseSample = "--sample";

        }
        
        public static class ImportCertificateCommandArguments
        {
            public const string Key = "-k";
            public const string VerboseKey = "--key";
            public const string Certificate = "-c";
            public const string VerboseCertificate = "--certificate";
            public const string Name = "-n";
            public const string VerboseName = "--name";
            public const string VerboseServer = "--server";
            public const string VerboseClient = "--client";
        }

        public static class InformationModelsName
        {
            public const string FileSnippet     = "files('information-models/{0}.c'),";
            public const string Types           = "_types";
            public const string TypesGenerated  = "_types_generated";
        }

        public static class LoadInformationModelsContent
        {
            public const string ReturnLine = "return UA_STATUSCODE_GOOD;";
            public const string FunctionSnippetPart1 = "\tif ({0}(server) != UA_STATUSCODE_GOOD)";
            public const string FunctionSnippetPart2 = "\t{";
            public const string FunctionSnippetPart3 = "\t\tUA_LOG_ERROR(UA_Log_Stdout, UA_LOGCATEGORY_SERVER, \"Could not add the {0} nodeset. Check previous output for any error.\");";
            public const string FunctionSnippetPart4 = "\t\treturn UA_STATUSCODE_BADUNEXPECTEDERROR;";
            public const string FunctionSnippetPart5 = "\t}";
        }

        public static class SlnCommandOptions
        {
            public const string Solution = "-s";
            public const string VerboseSolution = "--solution";
            public const string Project = "-p";
            public const string VerboseProject = "--project";
			public const string Help = "-h";
			public const string VerboseHelp = "--help";
            
        }

		public static class ReferenceCommandOptions
		{
			public const string Server = "-s";
			public const string VerboseServer = "--server";
			public const string Client = "-c";
			public const string VerboseClient = "--client";
			public const string Help = "-h";
			public const string VerboseHelp = "--help";
		}

		public static class ApplicationType
		{
			public const string Client = "Client";
			public const string Server = "Server";
			public const string ClientServer = "ClientServer";
		}

		public static class ServerConstants
		{
			public const string ServerAppHostname = "const char* SERVER_APP_HOSTNAME";
			public const string ServerAppPort = "const UA_UInt16 SERVER_APP_PORT";
			public const string ServerAppNamespaceVariable = "const UA_UInt16 ";
			public const string ServerAppOpen62541Include = "#include \"open62541.h\"";
		}

		public static class ClientGlobalVariables
		{
			public const string FirstLines = "#define numberOfReferences {0}\n\nconst char* SERVER_APP_URL[numberOfReferences] = {{";
			public const string Hostname = " \"opc.tcp://{0}:{1}/\",";
			public const string LastLines = " };\nUA_Client* client[numberOfReferences];";
		}
		
		public static class UAMethodCallback
		{
			public const string UAMethod = "UAMethod";
			public const string BrowseName = "BrowseName";
			public const string NodeId = "NodeId";
			public const string AddCallbacks = "addCallbacks";
			public const string FunctionName = "{0}_i{1}_Callback";
			public const string FunctionBody = "// callback function of: {0} UAMethod found in {1}\nUA_MethodCallback {2}_i{3}_Callback(UA_Server* server, const UA_NodeId* sessionId, void* sessionContext, const UA_NodeId* methodId, void* methodContext, const UA_NodeId* objectId, void* objectContext, size_t inputSize, const UA_Variant* input, size_t outputSize, UA_Variant* output)\n{{\n\t/* TODO: place your code here */\n\treturn UA_STATUSCODE_GOOD;\n}}\n";
			public const string FunctionCall = "\t// set callback of: {0} UAMethod found in {1}\n\tif(UA_Server_setMethodNode_callback(server, UA_NODEID_NUMERIC({2}, {3}), {2}_i{3}_Callback) != UA_STATUSCODE_GOOD)\n\t{{\n\t\treturn UA_STATUSCODE_BADUNEXPECTEDERROR;\n\t}}\n";
			public const string ReturnLine = "return UA_STATUSCODE_GOOD;";
		}

		public static class NodesetXml
		{
			public const string NamespaceVariablePrefix = "ns_";
			public const string UANodeSetNamespaceScheme = "http";
			public const string UANodeSetNamespaceHost = "opcfoundation.org";
			public const string UANodeSetNamespaceBasicValuePath = "UA/";
			public const string UANodeSetNamespaceValuePath = "UA/2011/03/UANodeSet.xsd";
			public const string UANodeSetNamespaceShortcut = "ns";
			public const string UANodeSetNamespaceFullPath = "//{0}:UANodeSet//{0}:Models//{0}:Model";
			public const string UANodeSetNamespaceModelUri = "ModelUri";
			public const string UANodeSetUAMethod = "//{0}:UANodeSet//{0}:UAMethod";
			public const string UANodeSetUAMethodBrowseName = "BrowseName";
			public const string UANodeSetUaMethodNodeId = "NodeId";
		}

		public static class NumericValues
		{
			public const int TextNotFound = -1;
			public const int PortNumberNotSpecified = -1;
			public const int StartInNewLine = 1;
			public const int HelpAlignmentSpace = 3;
		}
    }
}