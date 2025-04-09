namespace SCDataGenerator{
    public class Program {
        private static SupercellDataGenerator scGenerator = new();
        private static void Main() {
            string title = "SupercellDataGenerator v1.1 by @fastwoIf";

            Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, Console.CursorTop);
            Console.WriteLine(title);
            
            Console.Write("Enter class name: ");
            string className = Console.ReadLine();

            Console.Write("Enter base class: ");
            string baseClass = Console.ReadLine();
            
            scGenerator.SetClassName(className);
            scGenerator.SetBaseClassName(baseClass);

            string type = "";
            string name = "";

            int count = 1;
            while (true) {
                Console.WriteLine();

                Console.Write($"Enter {count} type: ");
                type = Console.ReadLine();

                scGenerator.AppendType(type);

                if (type == "\\") break;

                Console.Write($"Enter {count} name: ");
                name = Console.ReadLine();

                scGenerator.AppendName(name);
                count++;
            }
            scGenerator.Generate();
            scGenerator.SaveOutput();

            Console.WriteLine($"\nResult was written to {className}.cs");
            Console.ReadKey();
        }
    }
}