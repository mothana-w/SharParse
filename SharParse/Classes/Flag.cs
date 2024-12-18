using System.Text.RegularExpressions;
using SharParse.Classes.Exceptions;

namespace SharParse.Classes;

internal class Flag 
{
  public char ShortFlag {get; private set;}
  public string LongFlag {get; private set;}

  public string[] FlagVals {get; private set;}
  public byte FlagValsCount {get; private set;}

  public Flag(char shortFlag, string longFlag, byte valsCount, string[] flagVals){
    ShortFlag = shortFlag;
    LongFlag = longFlag;
    FlagValsCount = valsCount;

    FlagVals = flagVals;
  }
  public Flag(char shortFlag, string longFlag, byte valsCount) : this(shortFlag, longFlag, valsCount, []){
    validate();
  }

  private void validate(){
    if (ShortFlag == '\0' && LongFlag == "")
      throw new EmptyFlagException("No flags passed.");

    if (ShortFlag != '\0')
      if (!Regex.Match(Convert.ToString(ShortFlag), @"[a-zA-Z0-9]").Success)
        throw new InvalidFlagException($"Flag {ShortFlag} has invalid form.");

    if (LongFlag != "")
      if (!Regex.Match(LongFlag, @"^[^-][a-zA-Z0-9-]+[^-]$").Success)
        throw new InvalidFlagException($"Flag {LongFlag} has invalid form.");
  }

  public string GetFlag(){
    if (ShortFlag == '\0' && LongFlag == "") return "";
    if (ShortFlag == '\0') return LongFlag;
    return Convert.ToString(ShortFlag);
  }

  public static bool IsFlag(string arg){
    return !arg.StartsWith("---") && arg.StartsWith("-");
  }

  public static bool IsLong(string arg){
    return arg.StartsWith("--");
  }

  public static bool operator == (Flag f1, Flag f2){
    return (f1.ShortFlag  != '\0' && f1.ShortFlag == f2.ShortFlag ||
            f1.LongFlag   != ""   && f1.LongFlag  == f2.LongFlag);
  }
  public static bool operator != (Flag f1, Flag f2){
    return (f1.ShortFlag != f2.ShortFlag && f1.LongFlag != f2.LongFlag);
  }

  public override bool Equals(object? obj)
  {
    if (!(obj is Flag)) return false;
    return this == (Flag)obj;
  }
  public override int GetHashCode(){
    return HashCode.Combine(ShortFlag, LongFlag, FlagValsCount);
  }
  public override string ToString()
  {
    return  $"Short Flag\t\t: {ShortFlag}\n" + 
            $"Long Flag\t\t: {LongFlag}\n" + 
            $"Flag Args Count\t: {FlagValsCount}\n" + 
            $"Flag Args\t\t: {String.Join(", ", FlagVals)}\n";
  }
}
