﻿using System;
using System.Runtime.InteropServices;


namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  
  public struct CLRDATA_IL_ADDRESS_MAP {

    public uint ilOffset;

    public ulong startAddress;

    public ulong endAddress;

    public CLRDataSourceType type;

  }

}