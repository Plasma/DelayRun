using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace DelayRun
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0) {
				Console.WriteLine("DelayRun: fork <delay secs> <process name> [arguments]");
				return;
			}

			var forkTask = args[0] == "fork";
			var waitTime = int.Parse(args[1]);
			var processName = args[2];
			var processArgs = args.Skip(3).Select(x => x.Contains(' ') ? string.Format("\"{0}\"", x) : x).ToArray();
			Console.WriteLine("Ars found: " + processArgs.Length);
			foreach(var arg in processArgs)
				Console.WriteLine("Arg-> " + arg);
			var textArgs = string.Join(" ", processArgs);

			// Execute
			if (forkTask) {
				var process = new Process();
				process.StartInfo = new ProcessStartInfo();
				process.StartInfo.UseShellExecute = true;
				process.StartInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
				process.StartInfo.Arguments = string.Format("run {0} {1} {2}", waitTime, processName, textArgs);
				var success = process.Start();
				if (success)
					Console.WriteLine("Forker - Successfully executed self with run command: {0}", process.StartInfo.Arguments);
				else
					Console.WriteLine("Forker - Failed to execute self with run command: {0}", process.StartInfo.Arguments);

				// Exit immediately, even though a process is open
				Environment.Exit(0);
			} else {
				Console.WriteLine("Executor - Sleeping {0} secs (Command: {1} |  Args: {2})", waitTime, processName, textArgs);
				Thread.Sleep(TimeSpan.FromSeconds(waitTime));

				Console.WriteLine("Sleep finished; executing process");
				var process = new Process();
				process.StartInfo = new ProcessStartInfo();
				process.StartInfo.UseShellExecute = true;
				process.StartInfo.FileName = processName;
				process.StartInfo.Arguments = textArgs;
				process.Start();
			}
		}
	}
}
