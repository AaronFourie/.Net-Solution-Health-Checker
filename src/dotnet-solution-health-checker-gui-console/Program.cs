using System.Reflection;
using System.Text;

namespace dotnet_solution_health_checker_gui_console;

class Program
{
    const string LOOKUP_VERB = ".csproj";
    const string FILE_PATH = "C://Users/User/Downloads/customers-100.csv";
    const ushort BUFFER_SIZE = 4096;
    static void Main(string[] args)
    {
        //Memory<byte> MEMORY = new(BYTES, start:0, length: BUFFER_SIZE);

        try
        {
            ScanDirOnDisk(folderPath: "C://Users/User/Downloads/");
            ScanSolutionFilesFromCurrentAppDomainBaseDir();
            ReadFromFile(FILE_PATH);
        }
        catch (AccessViolationException ex) 
        {
            throw new FieldAccessException(message: $"Attempt to read/write memory failed", inner: ex);
        }
        catch (FileLoadException ex) 
        {
            throw new FieldAccessException(message: $"Managed Assmebly is found but cannot be loaded", inner: ex);
        }
    }

    private static int ScanSolutionFilesFromCurrentAppDomainBaseDir()
    {
        string currentDir = AppDomain.CurrentDomain.BaseDirectory;

        Console.WriteLine($"BASE DIR THAT THE ASSEMBLY RESOLVER USES TO PROBE FOR ASSEMBLIES {currentDir}");

        if(currentDir != null)
        {
            //lookup parent solution
            bool dirCheck;
            string searchParam = "*.sln";


            string currentDirName = Directory.GetParent(currentDir)?.Parent?.FullName;
            Console.WriteLine($"CURRENT DIR NAME {currentDirName}");

            //do
            //{
            //    string[] matchedFiles = Directory.GetFiles(path: currentDirName, searchPattern: searchParam);
            //    dirCheck = matchedFiles.Length > 0;
            //    if (dirCheck == true)
            //    {
            //        dirCheck = true;
            //    }
            //    else
            //    {
            //        currentDirName = Directory.GetParent(currentDirName).Parent.FullName;
            //    }
            //}
            //while (dirCheck == false);

            string path = ScanSolutionFilesFromCurrentAppDomainBaseDir2(currentDirName, searchParam) ?? "Not found";
            Console.WriteLine($"{searchParam} file found in dir {path}");


        }
        
        return 0;
    }

    private static string? ScanSolutionFilesFromCurrentAppDomainBaseDir2(string dir, string searchParam)
    {

        try
        {

            string[] matchedFiles = Directory.GetFiles(path: dir, searchPattern: searchParam);
            if(matchedFiles.Length == 0)
            {
                string nextDir = ScanSolutionFilesFromCurrentAppDomainBaseDir2(Directory.GetParent(dir)?.Parent?.FullName, searchParam: searchParam);
                if(nextDir is not null)
                {
                    dir = nextDir;
                }
            }
            return dir;
        }
        catch (DirectoryNotFoundException ex)
        {
            throw new DirectoryNotFoundException($"{dir} directory not found", innerException: ex);
        }

    }

    
    private static bool ScanDirOnDisk(string folderPath)
    {
        try
        {

            ArgumentNullException.ThrowIfNullOrEmpty(argument: folderPath);

            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"{folderPath} directory does not exist on disk");
                return false;
            }

            Console.WriteLine($"{folderPath} directory exists on disk");
            return true;
        }
        catch(DirectoryNotFoundException ex){
            throw new DirectoryNotFoundException(message: $"{folderPath} directory not found", innerException: ex);
        }
    }

    private static void ReadFromFile(string filePath)
    {
        char[] BUFFER = new char[BUFFER_SIZE];

        try
        {
            if (File.Exists(filePath) == false)
            {
                throw new FileNotFoundException(message: $"{filePath} File path doesn't exist");
            }
            using FileStream FILE_STREAM = new FileStream(path: filePath, mode: FileMode.Open, access: FileAccess.ReadWrite, share: FileShare.ReadWrite, bufferSize: BUFFER_SIZE, useAsync: false);

            using StreamReader STREAM_READER = new StreamReader(stream: FILE_STREAM, encoding: System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: BUFFER_SIZE);

            if (FILE_STREAM.CanRead == false)
            {
                throw new FieldAccessException(message: $"Cannot read from file stream {filePath}");
            }

            for (int i = 0; i < BUFFER_SIZE; i++)
            {
                if (STREAM_READER.EndOfStream == false && STREAM_READER.Peek() != -1)
                {
                    STREAM_READER.ReadBlock(BUFFER, i, 1);
                }
            }

            Console.WriteLine($"BUFFER LENGTH: {BUFFER.Length}");

            if (STREAM_READER.BaseStream.Length == 0)
            {
                Console.WriteLine("Base stream reader is empty");

            }
            StringBuilder STREAM_STRING = new();

            foreach (char i in BUFFER)
            {
                STREAM_STRING.Append(i);
            }

            if (STREAM_STRING.Length == 0)
            {
                Console.WriteLine("Stream string is empty");
            }

            Console.WriteLine(STREAM_STRING);
        }
        catch (AccessViolationException ex)
        {
            throw new AccessViolationException(message: $"Attempt to read/write memory failed", innerException: ex);
        }
        catch (FileLoadException ex)
        {
            throw new FieldAccessException(message: $"Managed Assmebly is found but cannot be loaded", inner: ex);
        }

    }
}
