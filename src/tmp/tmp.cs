using System;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Configuration;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var filename = "d.out.txt";
        var list = File.ReadLines(filename);
        list = list.Distinct();
		var start = 
			int.Parse(ConfigurationManager.AppSettings["start"]);
		var count = int.Parse(ConfigurationManager.AppSettings["count"]);
		var repeat  = int.Parse(ConfigurationManager.AppSettings["repeatCount"]);
		
        while ( string.IsNullOrEmpty(Console.ReadLine()))
        {
			var result = list.Skip(start).Take(count).Select(e=>
			string.Join("\n",Enumerable.Repeat(e,repeat))
			);			
            var content = string.Join("\n",result);
			Clipboard.SetText(content);
        }
    }
}