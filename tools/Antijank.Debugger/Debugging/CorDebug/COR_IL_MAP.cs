﻿using System;
using System.Runtime.InteropServices;


namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  
  public struct COR_IL_MAP {

    public uint oldOffset;

    public uint newOffset;

    public int fAccurate;

  }

}