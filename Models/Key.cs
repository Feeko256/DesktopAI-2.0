using System;
using DesktopAI.Core;

namespace DesktopAI.Models;

[Serializable]
public class Key
{
  public string _Key { get; set; }

  public Key(string key)
  {
    _Key = key;
  }
}