using System.Text;

namespace SCDataGenerator {
    public abstract class GameGenerator {
        public abstract void Generate();

        public abstract void CreateVariables();
        public abstract void CreateConstructor();
        public abstract void CreateReferences();

        public abstract void AddCSVTypes();

        public abstract void SaveOutput();

        public abstract void SetClassName(string className);
        public abstract void SetBaseClassName(string baseClassName);

        public abstract void AppendType(string type);
        public abstract void AppendName(string name);


        public static void Error(string message) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Black;

            Environment.Exit(-2);
        }

        public static bool IsVaildType(string type) {
            if (type == "List<int>" || type == "List<string>" || type.StartsWith("List<Logic") ||
                type == "int" || type == "string" || type == "bool" || type == "ulong" || type.StartsWith("Logic")) {
                return true;
            }
            else {
                Error($"Unallowed type was entered ({type}).\nExpected: int, string, bool, Logic...Data, List<int>, List<string>, List<bool>, List<Logic...Data>");

                return false;
            }
        }

        public static bool IsVaildClassName(string className, bool isMainClass) {
            if (isMainClass) Error("Invaild class name, expected: Logic...Data");
            else Error("Invaild base class name, expected: Logic...Data");

            return className.StartsWith("Logic") && className.EndsWith("Data");
        }

        public static void SaveOutput(string className, string data) {
            FileStream? stream = new(className + ".generated.cs", FileMode.OpenOrCreate);

            stream.Write(Encoding.Default.GetBytes(data), 0, data.Length);
            stream.Close();
        }
    }
}
