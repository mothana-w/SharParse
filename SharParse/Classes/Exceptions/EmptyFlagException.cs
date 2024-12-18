namespace SharParse.Classes.Exceptions;

internal class EmptyFlagException : Exception
{
  public EmptyFlagException(string message) : base(message){}
}