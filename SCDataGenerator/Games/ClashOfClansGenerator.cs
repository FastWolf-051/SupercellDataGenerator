using System.Text;

namespace SCDataGenerator.Games {
    public class ClashOfClansGenerator : GameGenerator {
        private string _className;
        private string _baseClassName;

        private List<string> _types;
        private List<string> _names;
        private List<string> _csvTypes;

        private string _totalData;

        private int _tempNumber;

        public ClashOfClansGenerator() {
            _types = new();
            _names = new();
            _csvTypes = new();

            _tempNumber = 1;
        }

        public override void Generate() {
            _totalData += "public class " + _className + " : " + _baseClassName + " {\n\t";

            CreateVariables();
            CreateConstructor();
            CreateReferences();

            _totalData += "\n}";
        }

        public override void CreateVariables() {
            for (int i = 0; i < _names.Count; i++) {
                if (i > _names.Count - 1 && _types[i] != _types[i + 1]) {
                    _totalData += "\n\t";
                }
                else if (i > 0 && _names.Count > i - 1 && _types[i - 1] != _types[i]) _totalData += "\n\t";

                _totalData += "private " + _types[i] + " _" + _names[i] + ";\n\t";
            }
            _totalData += "\n\t";
        }

        public override void CreateConstructor() {
            _totalData += "public " + _className + "(CSVRow row, LogicDataTable table) : base(row, table) {\n\t\t";

            string name = "";
            string defaultValue = "";
            for (int i = 0; i < _names.Count ; i++) {
                if (_types[i].StartsWith("List<")) continue;

                if (i > _names.Count - 1 && _types[i] != _types[i + 1]) _totalData += "\n\t\t";
                else if (i > 0 && _names.Count > i - 1 && _types[i - 1] != _types[i]) _totalData += "\n\t\t";

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

                    if (i != _names.Count - 1) {
                        if (i > _names.Count - 1) _totalData += "\t";
                        else _totalData += "\t";
                    }
                }
            }
            _totalData += "}\n\n\t";
        }

        public override void CreateReferences() {
            _totalData += "public override void CreateReferences() {\n\t\tbase.CreateReferences();\n\n\t\tint longestArraySize = _row.GetLongestArraySize();\n\n\t\t";

            AddCSVTypes();

            bool IsInCycle = false;

            for (int i = 0; i < _names.Count; i++) {
                if (!IsInCycle) {
                    if (i > _names.Count - 1 && _types[i] != _types[i + 1]) {
                        _totalData += "\n\t\t";
                    }
                    else if (i > 0 && _names.Count > i - 1 && _types[i - 1] != _types[i]) _totalData += "\n\t\t";

                    if (_types[i] == "int" || _types[i] == "string" || _types[i] == "bool" || _types[i] == "ulong") {
                        string name = string.Concat(_names[i][0].ToString().ToUpper(), _names[i].AsSpan(1));

                        string csvType = $"{_csvTypes[i]}(\"{name}\", 0)";
                        string nonObfuscated = $"_{_names[i]} = " + csvType;

                        string obfuscated = $"ulong temp{_tempNumber} = (ulong)(1374389535 * ({csvType}) >> 9)) >> 32;\n\t\t_{_names[i]} = (temp{_tempNumber} >> 5) + (temp{_tempNumber} >> 1);\n\t\t";
                        
                        if (_types[i] == "ulong") {
                            _totalData += obfuscated;
                            _tempNumber++;
                        }
                        else _totalData += nonObfuscated + ";";

                        if (i < _names.Count - 1) {
                            _totalData += "\n\t\t";
                        }
                    }

                    if (i < _names.Count - 1 && (_types[i] == "int" || _types[i] == "string" || _types[i] == "bool") && _types[i + 1].StartsWith("List<")) {
                        _totalData += "\n\t\tif (longestArraySize >= 1) {\n\t\t\tfor (int j = 0; i < longestArraySize; i++) {";

                        if (_csvTypes[i].StartsWith("List<Logic")) _totalData += "\n\t\t\t\t";

                        IsInCycle = true;
                    }

                    if (_types[i].StartsWith("Logic")) {
                        string dataType = $"LogicDataTables.Get{_types[i][5..^5]}ByName";
                        string name = string.Concat(_names[i][0].ToString().ToUpper(), _names[i].AsSpan(1));

                       _totalData += $"_{_names[i]} = {dataType}({_csvTypes[i]}(\"{name}\", i), this);\n\t\t\t\t";
                    }
                }
                else if (IsInCycle) {
                    if (i > _names.Count - 1 && _types[i] != _types[i + 1]) {
                        _totalData += "\n\t\t\t\t";
                    }
                    else if (i > 0 && _names.Count > i - 1 && _types[i - 1] != _types[i]) _totalData += "\n\t\t\t\t";

                    if (_types[i].StartsWith("List<Logic")) {
                        string dataType = $"LogicDataTables.Get{_types[i][10..^5]}ByName";
                        string name = _names[i][0].ToString().ToUpper() + _names[i].Substring(1);

                        _totalData += $"_{_names[i]}.Add({dataType}({_csvTypes[i]}(\"{name}\", i), this);\n\t\t\t";

                        bool isCycleEnd = _types[i + 1] == "int" || _types[i + 1] == "string" || _types[i + 1] == "bool" || _types[i + 1].StartsWith("Logic");

                        if (!isCycleEnd) _totalData += "\t";

                        if (isCycleEnd) {
                            _totalData += "}\n\t\t}\n\t";
                            IsInCycle = false;
                        }
                    }
                    else if (_types[i].StartsWith("List<")) {
                        string name = _names[i][0].ToString().ToUpper() + _names[i].Substring(1);

                        _totalData += $"_{_names[i]}.Add({_csvTypes[i]}(\"{name}\", i));\n\t\t\t";

                        if (_types[i + 1] == "int" || _types[i + 1] == "string" || _types[i + 1] == "bool" || _types[i + 1].StartsWith("Logic")) {
                            _totalData += "\t}\n\t\t}\n\t\t";
                            IsInCycle = false;
                        }

                        if (i < _names.Count - 1) _totalData += "\t";
                        else _totalData += "\t";
                    }
                }
            }
            _totalData += "\n\t}";
        }
        public override void AddCSVTypes() {
            for (int i = 0; i < _names.Count; i++) {
                if (_types[i] == "int" || _types[i] == "ulong") _csvTypes.Add("_row.GetIntegerValue");
                else if (_types[i] == "string") _csvTypes.Add("_row.GetValue");
                else if (_types[i] == "bool") _csvTypes.Add("_row.GetBooleanValue");

                else if (_types[i] == "List<int>") _csvTypes.Add("_row.GetClampedIntegerValue");
                else if (_types[i] == "List<string>") _csvTypes.Add("_row.GetClampedValue");
                else if (_types[i] == "List<bool>") _csvTypes.Add("_row.GetClampedBooleanValue");

                else if (_types[i].StartsWith("List<Logic")) _csvTypes.Add("_row.GetClampedValue");
                else if (_types[i].StartsWith("Logic")) _csvTypes.Add("_row.GetValue");
            }

            for (int i = 0; i < _names.Count; i++) {
                if (_types[i].StartsWith("List<")) {
                    _totalData += "_" + _names[i] + " = new(longestArraySize);\n\t\t";

                    if (i > _names.Count - 1 && _types[i] != _types[i + 1]) {
                        _totalData += "\n\t\t\t\t";
                    }
                    else if (_types[i] != _types[i + 1]) _totalData += "\n\t\t";
                }
            }
        }

        public override void SaveOutput() {
            SaveOutput(_className, _totalData);
        }

        public override void SetClassName(string className) {
            if (IsVaildClassName(className, true)) _className = className;
        }

        public override void SetBaseClassName(string baseClassName) {
            if (IsVaildClassName(baseClassName, false)) _baseClassName = baseClassName;
        }

        public override void AppendType(string type) {
            if (IsVaildType(type)) _types.Add(type);
        }

        public override void AppendName(string name) {
            _names.Add(name);
        }
    }
}