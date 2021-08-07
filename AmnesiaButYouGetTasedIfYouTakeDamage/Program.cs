using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO.Ports;

using Memory;

namespace AmnesiaButYouGetTasedifYouTakeDamage {
	class Program {
		private const string HEALTH_POINTER = "Amnesia.exe+0x00768C54,0x84,0x284,0x2C,0x30,0x84"; //"Amnesia.exe+0x00768C54,0x84,0x30,0x2C,0x284,0x84";

		private static Mem memLib;
		private static Process amnesiaProcess;
		private static SerialPort port;

		private static int oldHp = int.MinValue;

		static void Main(string[] args) {
			string[] ports = SerialPort.GetPortNames();

			while (true) {
				Console.WriteLine("Please enter the number of the serial port you want to use");

				for (int i = 0; i < ports.Length; i++) {
					Console.WriteLine((i + 1) + " : " + ports[i]);
				}

				string input = Console.ReadLine();

				Console.Clear();

				string portName = null;

				try {
					int selected = int.Parse(input);
					portName = ports[selected - 1];
				} catch (Exception e) {
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Please enter a valid number");
					Console.ResetColor();
					continue;
				}

				try {
					port = new SerialPort(portName, 9600);
					port.Open();
					port.Write("test\n");
				} catch (Exception e) {
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Failed to open port");
					Console.ResetColor();
					continue;
				}

				Console.WriteLine("Did the LED on the arduino blink y/n");
				string yn = Console.ReadLine();

				if (yn.ToLower() != "y") {
					continue;
				}

				break;
			}


			Console.WriteLine("Wait for the game to start and load your save / start a new game before pressing any key");
			Console.WriteLine("Do not connect the tazer to yourself until you have verified that the health is diplayed correctly");
			Console.WriteLine("Press any key to continue . . .");
			Console.ReadKey();

			memLib = new Mem();


			Process[] processes = Process.GetProcessesByName("Amnesia");

			try {
				amnesiaProcess = processes[0];
			} catch (Exception e) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Could not find the Amnesia.exe process. Please make sure that you are running the steam version");
				Console.ResetColor();
				Console.ReadKey();
				return;
			}

			memLib.OpenProcess(amnesiaProcess.ProcessName);

			Console.Clear();

			Console.WriteLine("Ready");

			while (true) {
				int hp = (int)Math.Ceiling(memLib.ReadFloat(HEALTH_POINTER));

				Console.SetCursorPosition(0, 1);

				Console.Write("Health: " + hp + "     ");

				if (hp != oldHp) {
					if (hp < oldHp) {
						port.Write("zap\n");
					}
					oldHp = hp;
				}

				Thread.Sleep(100);
			}
		}
	}
}