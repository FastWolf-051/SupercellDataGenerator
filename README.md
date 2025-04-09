# SupercellDataGenerator
 tool for supercell games to make data classes creation easier (for C#)

# Requirements
 brain, IDA

# How to use
 Install net8.0 (if you don't have it now), run the Program.cs
 
# How this works
 The Supercell company have an Data classes in their games, like Brawl stars.
 Those classes are needed for getting information from different CSV classes.
 The process of writing those are really routine, due of amount of similar code.
 That's why i decided to write this tool

# Usage
 The structure of Data classes is like this:
 ```cpp
  v7 = (CSVRow *)*((_DWORD *)this + 1);
  String::String(v11, "HasEvenSpaceCharacters");
  *((_BYTE *)this + 116) = CSVRow::getBooleanValue(v7, v11, 0);
  String::~String(v11);
 ```
 That code disassembles to 
 ```csharp
 private bool _hasEvenSpaceCharacters;
 
 _hasEvenSpaceCharacters = _row.GetBooleanValue("HasEvenSpaceCharacters", 0);
 ```
 Let's look into massives. The IDA code:
 ```cpp
 if ( *((int *)this + 37) > 0 )
  {
    v2 = 0;
    do
    {
      v3 = (CSVRow *)*((_DWORD *)this + 1);
      v4 = *((_DWORD *)this + 41);
      String::String(v247, "Damage");
      ClampedIntegerValue = (LogicGamePlayUtil *)CSVRow::getClampedIntegerValue(v3, v247, v2);
      v7 = LogicGamePlayUtil::DPSToSingleHit(ClampedIntegerValue, 1000, v6);
      v8 = *(_DWORD *)(v4 + 8);
      v9 = v7;
 ```
 It can be disassembled into
 ```csharp
 private List<int> _damage;

 // CreateReferences()

 int longestArraySize = _row.GetLongestArraySize();

 _damage = new(longestArraySize);
 
 for (int i = 0; i < longestArraySize; i++) {
 	if (longestArraySize > 0) {
		int dps = _row.GetClampedIntegetValue("Damage", i);
		
 		_damage.Add(LogicGamePlayUtil.DPSToSingleHit(dps, 1000);
	}
 }
 ```

 I hope, that you understand, how to use this tool. Good luck