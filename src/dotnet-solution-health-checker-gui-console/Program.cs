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
        char[] BUFFER = new char[BUFFER_SIZE];
        //Memory<byte> MEMORY = new(BYTES, start:0, length: BUFFER_SIZE);

        try
        {

            LookupAssemblies();

            if (File.Exists(FILE_PATH) == false)
            {
                throw new FileNotFoundException(message: $"{FILE_PATH} File path doesn't exist");
            }
            using FileStream FILE_STREAM = new FileStream(path: FILE_PATH, mode: FileMode.Open, access: FileAccess.ReadWrite, share: FileShare.ReadWrite, bufferSize: BUFFER_SIZE, useAsync: false);

            StreamReader STREAM_READER = new StreamReader(stream: FILE_STREAM, encoding: System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: BUFFER_SIZE);

            if (FILE_STREAM.CanRead == false)
            {
                throw new FieldAccessException(message: $"Cannot read from file stream {FILE_PATH}");
            }

            for(int i = 0; i<BUFFER_SIZE; i++)
            {
                if(STREAM_READER.EndOfStream == false && STREAM_READER.Peek() != -1)
                {
                    STREAM_READER.ReadBlock(BUFFER, i, 1);
                }
            }

            Console.WriteLine($"BUFFER LENGTH: {BUFFER.Length}");

            if(STREAM_READER.BaseStream.Length == 0)
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
            throw new FieldAccessException(message: $"Attempt to read/write memory failed", inner: ex);
        }
        catch (FileLoadException ex) 
        {
            throw new FieldAccessException(message: $"Managed Assmebly is found but cannot be loaded", inner: ex);
        }
    }

    private static int LookupAssemblies()
    {
        string currentDir = AppDomain.CurrentDomain.BaseDirectory;

        if(currentDir != null)
        {
            string solutionFile = Directory.GetParent(currentDir).Parent.Parent.Parent.FullName;
            Console.WriteLine(solutionFile);

            string[] csprojFiles = Directory.GetFiles(solutionFile, "*.csproj");

            foreach(string file in csprojFiles)
            {
                Console.WriteLine(file);
            }

        }
        
        return 0;
    }
}
