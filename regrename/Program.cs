// See https://aka.ms/new-console-template for more information
using regrename;
using System.Text.RegularExpressions;

bool preview = false;



ConsoleOptions co = new ConsoleOptions(args);



if(co.GetByKey("h") != null || co.GetByKey("help") != null)
{
    ShowHelp();
    return;
}

// Get regex
string? regex = co.GetByOrdinal(0)?.value;
if (regex == null)
{
    regex = co.GetByKey("regex")?.value;
}
if (regex == null)
{
    throw new Exception("No regex provided");
}
if (co.GetByKey("preview") != null || co.GetByKey("p") != null || co.GetByKey("whatif") != null)
{
    preview = true;
}

// Get replace

string? replace = co.GetByOrdinal(1)?.value;
if(replace == null)
{
    replace = co.GetByKey("replace")?.value;
}
if (replace == null)
    throw new Exception("No replace regex provided");


// if regex and replace were combined?   This will not have good support due to .NET style regex, but might be good enough for general use
if( (regex.StartsWith('/') || regex.StartsWith("s/")) && regex.EndsWith('/') && replace == null )
{
    // perl style regex   s/
    int startregex = regex.IndexOf('/') + 1;
    int endregex = regex.IndexOf('/', startregex);
    int startreplace = endregex + 1;
    int endreplace = regex.IndexOf('/', startreplace + 1);

    replace = regex.Substring(startreplace, endreplace - startreplace);
    regex = regex.Substring(startregex, endregex - startregex);
}


string dir = co.GetByOrdinal(2)?.value;
if (dir == null)
    dir = co.GetByKey("dir")?.value;
if (dir == null)
    dir = Environment.CurrentDirectory;

System.IO.SearchOption dirsearchoption = co.GetByKey("r") != null ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly;


if(preview)
{
    Console.WriteLine("Running in preview mode, no changes will be made.");
    Console.WriteLine($"Target Directory: {dir}");
    Console.WriteLine($"Match Regex: {regex}");
    Console.WriteLine($"Replace Regex: {replace}");
    Console.WriteLine();
}
System.Text.RegularExpressions.Regex fileregex = null;
try
{
    fileregex = new System.Text.RegularExpressions.Regex(regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled);
}catch(RegexParseException rpe)
{
    Console.WriteLine("Could not parse regex: " + rpe.Message);
    return;
}
string[] files = Directory.GetFiles(dir, "*.*", dirsearchoption);
for (long i = 0; i < files.LongLength; i++)
{
    string originalfile = files[i];
    string ext = Path.GetExtension(originalfile);

    if (fileregex.IsMatch(files[i]))
    {
        string newfile = fileregex.Replace(Path.GetFileNameWithoutExtension(originalfile), replace);
        if (preview)
            Console.WriteLine($"{Path.GetFileNameWithoutExtension(originalfile) + ext} -> {newfile + ext}");
        else
        {
            // Do the rename
            string realnewfile = Path.Combine(Path.GetDirectoryName(originalfile), newfile + ext);
            File.Move(originalfile, realnewfile, false);
            //Console.WriteLine($"{originalfile} -> {realnewfile}");
        }
    }
}


void ShowHelp()
{
    Console.WriteLine("Rename files with .NET regular expressions");
    Console.WriteLine("regrename.exe <match> <replace> [directory] --preview -r --help");

    Console.WriteLine("  [directory] - optional. Defaults to current directory.");
    Console.WriteLine("  --preview   - optional. Shows preview of the rename");
    Console.WriteLine("  -r          - optional. Recursive search.  Will rename in all subdirectories as well. ");
    Console.WriteLine("  --help      - optional. Shows this help.");
    Console.WriteLine("Alternate format:  regrename.exe -regex <regex> --replace <replace> --dir <directory> --preview -r --help");
    Console.WriteLine();
    Console.WriteLine("The above regexes **only** work on the filenames, not directories or extensions.");

}
