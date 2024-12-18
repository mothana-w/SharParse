using SharParse.Classes;

namespace SharParse;

public class Parser
{
  private List<Flag> flags { set; get; }
  private Stack<Flag> arguments { set; get; }

  public List<String> PositionalArgs { get; private set; }
  public string CurrentFlag { get; private set; }
  public string[] Values { get; private set; }
  public byte RequiredVals { get; private set; }

  private byte counter;

  public Parser(string[] args)
  {
    flags = new List<Flag>();
    arguments = new Stack<Flag>();

    PositionalArgs = new List<String>();
    CurrentFlag = "";
    Values = [];
    RequiredVals = 0;

    counter = 0;

    SetArgsList(args);
  }

  public void SetFlag(char shortFlag, string longFlag, byte valsCount){
    Flag flag = new Flag(shortFlag, longFlag, valsCount);
    flags.Add(flag);
  }
  public void SetFlag(char shortFlag, byte valsCount) => SetFlag(shortFlag, "", valsCount);
  public void SetFlag(string longFlag, byte valsCount) => SetFlag('\0', longFlag, valsCount);

  private string ParseArgs(){
    Flag arg = arguments.Pop();

    CurrentFlag = arg.GetFlag();
    Values = arg.FlagVals;

    if (CurrentFlag == "") return "--";

    foreach (Flag flag in flags){
      RequiredVals = flag.FlagValsCount;

      if (arg == flag){
        if (arg.FlagValsCount == flag.FlagValsCount) return CurrentFlag;
        else if (arg.FlagValsCount < flag.FlagValsCount) return "-";
        else return "+";
      }
    }
    return "?";
  }
  public string Parse(){
    CurrentFlag = "";
    RequiredVals = 0;
    Values = [];

    if (counter++ == 0 && arguments.Count == 0 && PositionalArgs.Count == 0) return ".";

    if (arguments.Count > 0){
      return ParseArgs();
    }

    return "";
  }

  private void AddVals(string[] args, ref byte i, List<String> flagVals){
    while (i + 1 < args.Length && !Flag.IsFlag(args[i + 1])){
      flagVals.Add(args[i + 1]);
      ++i;
    }
  }
  private void PushFlag(bool isLong, string flag, List<String> flagVals){
    if (isLong)
      arguments.Push(new Flag('\0', flag, (byte)flagVals.Count, flagVals.ToArray()));
    else
    {
      if (flag.Length == 0) flag = "\0";
      for (byte j = 0; j < flag.Length -1; ++j)
        arguments.Push(new Flag(Convert.ToChar(flag[j]), "", 0, []));

      arguments.Push(new Flag(Convert.ToChar(flag[^1]), "", (byte)flagVals.Count, flagVals.ToArray()));
    }
  }
  // takes end-user arguments and stack them.
  private void SetArgsList(string[] args){
    for (byte i = 0; i < args.Length; ++i){

      if (Flag.IsFlag(args[i]))
      {
        List<String> flagVals = new List<String>();
        bool isLong = Flag.IsLong(args[i]);
        string flag = args[i].TrimStart('-');

        AddVals(args, ref i, flagVals);
        PushFlag(isLong, flag, flagVals);
      }
      else
        PositionalArgs.Add(args[i]);
    }
  }
}