using System;
using System.IO;
using Newtonsoft.Json;

namespace Oppo.ObjectModel
{
    public static class SlnUtility
    {
		static public Solution DeserializeSolutionFile(string solutionFullName, IFileSystem fileSystem)
		{
			var slnMemoryStream = fileSystem.ReadFile(solutionFullName);
			StreamReader readerSln = new StreamReader(slnMemoryStream);
			var slnContent = readerSln.ReadToEnd();

			Solution oppoSolution;
			try
			{
				oppoSolution = JsonConvert.DeserializeObject<Solution>(slnContent);
				if (oppoSolution == null)
				{
					throw null;
				}
			}
			catch (Exception)
			{
				return null;
			}
			slnMemoryStream.Close();
			slnMemoryStream.Dispose();

			return oppoSolution;
		}
	}
}