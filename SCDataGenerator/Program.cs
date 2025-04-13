using SCDataGenerator.Games;

using System.Text;

namespace SCDataGenerator{
    public class Program {
        private static ClashOfClansGenerator cocGenerator = new();

        private static async Task Main(string[] args) {
            if (args.Length == 8 && (args[0] == "--game" && args[2] == "--file" && args[4] == "--class" && args[6] == "--base")) {
                string file = args[3];
                string mainClass = args[5];
                string baseClass = args[7];

                cocGenerator.SetClassName(mainClass);
                cocGenerator.SetBaseClassName(baseClass);

                FileStream list = File.OpenRead(file);

                byte[] buffer = new byte[list.Length];

                await list.ReadAsync(buffer, 0, buffer.Length);

                string fileData = Encoding.Default.GetString(buffer);

                string[] lines_old = fileData.Split(["\r\n", "\n"], StringSplitOptions.None);

                List<string> lines_new = new();

                for (int i = 0; i < lines_old.Length; i++) {
                    if (lines_old[i] == "int-o") lines_new.Add("ulong");
                    else lines_new.Add(lines_old[i]);
                }

                for (int i = 0; i < lines_new.Count; i += 3) {
                    cocGenerator.AppendType(lines_new[i]);
                   
                    int next = i + 1;

                    cocGenerator.AppendName(lines_new[next]);
                }

                cocGenerator.Generate();
                cocGenerator.SaveOutput();

                Console.WriteLine($"\nOutput was written to {mainClass}.cs sucsessfully\n");
            }

            else if (args.Length == 1 && args[0] == "--notice") {
                Console.WriteLine("\n1) Data classes cant start with List<>, it they does, you need to enter one non-list type to make it.\n" +
                                  "2) tAdd anpty line between type and name");
            }
            else {
                Console.WriteLine("\nUsage: SCDataGenerator --game coc --file <file> --class <class_name> --base <base_name>\n" +
                                    "       SCDataGenerator --notice");
            }
        }
    }
}