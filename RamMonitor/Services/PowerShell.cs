using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Remoting;
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;

namespace RamMonitor.Services
{
    public class PowerShellResult
    {
        public bool HasError { set; get; }
        public string ResultText { set; get; }
        public string ErrorText { set; get; }
    }




    public class PowerShell
    {
        public static PowerShellResult RunCleosCode(string text)
        {
            return RunScriptText(@"docker exec -i keosd /opt/eosio/bin/cleos --wallet-url http://127.0.0.1:8900 -u http://mainnet.genereos.io " + text);
        }

        public static PowerShellResult RunScriptText(string text)
        {
            // create Powershell runspace
            Runspace runspace = RunspaceFactory.CreateRunspace();
            // open it
            runspace.Open();
            // create a pipeline and feed it the script text
            Pipeline pipeline = runspace.CreatePipeline();

            string scriptText = text;

            pipeline.Commands.AddScript(scriptText);
            // add an extra command to transform the script output objects into nicely formatted strings
            // remove this line to get the actual objects that the script returns. For example, the script
            // "Get-Process" returns a collection of System.Diagnostics.Process instances.
            //pipeline.Commands.Add("Out-String");
            // execute the script
            System.Collections.ObjectModel.Collection<PSObject> results = pipeline.Invoke();
            // close the runspace
            runspace.Close();
            // convert the script result into a single string

            PowerShellResult powerShellResult = new PowerShellResult();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (PSObject obj in results)
            {
                stringBuilder.AppendLine(obj.ToString());
            }
            powerShellResult.ResultText = stringBuilder.ToString();



            Collection<object> errors = pipeline.Error.ReadToEnd(); // Streams property is not available
            StringBuilder stringBuilderError = new StringBuilder();
            if (errors != null && errors.Count > 0)
            {
                foreach (PSObject er in errors)
                {
                    stringBuilderError.AppendLine(er.ToString());
                }
                powerShellResult.ErrorText = stringBuilder.ToString();
                powerShellResult.HasError = true;
            }

            return powerShellResult;
        }
    }
}
