using System.Text;

namespace SCDataGenerator {
    public class SupercellDataGenerator {
        private string _className;
        private string _baseClassName;

        private List<string> _types;
        private List<string> _names;
        private List<string> _csvTypes;

        private string _totalData;

        public SupercellDataGenerator() {
            _types = new();
            _names = new();
            _csvTypes = new();
        }

        public void Generate() {
            _totalData += "public class " + _className + " : " + _baseClassName + " {\n\t";

            CreateVariables();
            CreateConstructor();
            CreateReferences();

            _totalData += "\n}";
        }

        private void CreateVariables() {
            for (int i = 0; i < _types.Count - 1; i++) {
                if (i > 0 && _types[i] != _types[i + 1]) {
                    _totalData += "\n\t";
                }
                _totalData += "private " + _types[i] + " _" + _names[i] + ";\n\t";
            }
            _totalData += "\n\t";
        }

        private void CreateConstructor() {
            _totalData += "public " + _className + "(CSVRow row, LogicDataTable table) : base(row, table) {\n\t\t";

            string name = "";
            string defaultValue = "";
            for (int i = 0; i < _types.Count - 1; i++) {
                if (_types[i].StartsWith("List<")) continue;

                if (i > 0 && (_types[i] != _types[i + 1])) _totalData += "\n\t\t";

                name = "_" + _names[i];

                if (_types[i].StartsWith("Logic")) {
                    defaultValue = "new(row, table);";
                }
                else if (_types[i] == "string") defaultValue = "\"\";";
                else if (_types[i] == "int") defaultValue = "0;";
                else if (_types[i] == "bool") defaultValue = "false;";

                if (!_types[i].StartsWith("List<") ||
                     _types[i].StartsWith("List<") != (_types[i + 1] == "int" || _types[i + 1] == "string" || _types[i + 1] == "bool")) {
                    _totalData += name + " = " + defaultValue + "\n\t";

                    if (_types[i + 1] != "\\") {
                        _totalData += "\t";
                    }
                }
            }
            _totalData += "}\n\n\t";
        }

        private void CreateReferences() {
            _totalData += "public override void CreateReferences() {\n\t\tbase.CreateReferences();\n\n\t\tint longestArraySize = _row.GetLongestArraySize();\n\n\t\t";

            AddCSVTypes();

            bool IsInCycle = false;

            for (int i = 0; i < _types.Count - 1; i++) {
                if (!IsInCycle) {
                    if (i > 0 && (_types[i] != _types[i + 1])) {
                        _totalData += "\n\t\t";
                    }

                    if (_types[i] == "int" || _types[i] == "string" || _types[i] == "bool") {
                        string name = string.Concat(_names[i][0].ToString().ToUpper(), _names[i].AsSpan(1));

                        _totalData += $"_{_names[i]} = {_csvTypes[i]}(\"{name}\", 0);";

                        if (_types[i + 1] != "\\") {
                            _totalData += "\n\t\t";
                        }
                    }

                    if ((_types[i] == "int" || _types[i] == "string" || _types[i] == "bool") && _types[i + 1].StartsWith("List<")) {
                        _totalData += "\n\t\tif (longestArraySize >= 1) {\n\t\t\tfor (int j = 0; i < longestArraySize; i++) {";

                        if (_csvTypes[i].StartsWith("List<Logic")) _totalData += "\n\t\t\t\t";

                        IsInCycle = true;
                    }

                    if (_types[i].StartsWith("Logic")) {
                        string dataType = $"LogicDataTables.Get{_types[i][5..^4]}ByName";
                        string name = string.Concat(_names[i][0].ToString().ToUpper(), _names[i].AsSpan(1));

                       _totalData += $"_{_names[i]} = {dataType}({_csvTypes[i]}(\"{name}\", i), null);\n\t\t\t\t";
                    }
                }
                else if (IsInCycle) {
                    if (i > 0 && (_types[i] != _types[i + 1])) {
                        _totalData += "\n\t\t\t\t";
                    }

                    if (_types[i].StartsWith("List<Logic")) {
                        string dataType = $"LogicDataTables.Get{_types[i][10..^4]}ByName";
                        string name = _names[i][0].ToString().ToUpper() + _names[i].Substring(1);

                        _totalData += $"_{_names[i]}.Add({dataType}({_csvTypes[i]}(\"{name}\", i), null);\n\t\t\t\t";

                        if (_types[i + 1] == "int" || _types[i + 1] == "string" || _types[i + 1] == "bool") {
                            _totalData += "}\n\t\t}\n\t";
                            IsInCycle = false;
                        }
                    }
                    else if (_types[i].StartsWith("List<")) {
                        string name = _names[i][0].ToString().ToUpper() + _names[i].Substring(1);

                        _totalData += $"_{_names[i]}.Add({_csvTypes[i]}(\"{name}\", i));\n\t\t";

                        if (_types[i + 1] == "int" || _types[i + 1] == "string" || _types[i + 1] == "bool") {
                            _totalData += "}\n\t\t}\n\t\t";
                            IsInCycle = false;
                        }

                        if (_types[i + 1] != "\\") {
                            _totalData += "\t";
                        }
                    }
                }
            }
            _totalData += "\n\t}";
        }

        private void AddCSVTypes() {
            for (int i = 0; i < _types.Count - 1; i++) {
                if (_types[i] == "int") _csvTypes.Add("_row.GetIntegerValue");
                else if (_types[i] == "string") _csvTypes.Add("_row.GetValue");
                else if (_types[i] == "bool") _csvTypes.Add("_row.GetBooleanValue");

                else if (_types[i] == "List<int>") _csvTypes.Add("_row.GetClampedIntegerValue");
                else if (_types[i] == "List<string>") _csvTypes.Add("_row.GetClampedValue");
                else if (_types[i] == "List<bool>") _csvTypes.Add("_row.GetClampedBooleanValue");

                else if (_types[i].StartsWith("List<Logic")) _csvTypes.Add("_row.GetClampedValue");
                else if (_types[i].StartsWith("Logic")) _csvTypes.Add("_row.GetValue");
            }

            for (int i = 0; i < _types.Count - 1; i++) {
                if (_types[i].StartsWith("List<")) {
                    _totalData += "_" + _names[i] + " = new(longestArraySize);\n\t\t";

                    if (i > 0 && (_types[i] != _types[i + 1])) {
                        _totalData += "\n\t\t\t\t";
                    }
                }
            }
        }

        public void SaveOutput() {
            FileStream? stream = new(_className + ".cs", FileMode.OpenOrCreate);

            stream.Write(Encoding.Default.GetBytes(_totalData), 0, _totalData.Length);
        }

        public void SetClassName(string className) {
            if (className.StartsWith("Logic")) _className = className;
            else Error("Invaild class name");
        }

        public void SetBaseClassName(string baseClassName) {
            if (baseClassName.StartsWith("Logic")) _baseClassName = baseClassName;
            else Error("Invaild base class name");
        }

        public string GetTotalData() {
            return _totalData;
        }

        public void AppendType(string type) {
            if (type == "List<int>" || type == "List<string>" || type.StartsWith("List<Logic") ||
                type == "int" || type == "string" || type == "bool" || type.StartsWith("Logic") || type == "\\") {
                _types.Add(type);
            }
            else {
                Error("Unallowed type was entered.\nExpected: int, string, bool, Logic...Data, List<int>, List<string>, List<bool>, List<Logic...Data>");
            }
        }

        public void AppendName(string name) {
            _names.Add(name);
        }

        private void Error(string message) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Black;

            Environment.Exit(-2);
        }
    }
}