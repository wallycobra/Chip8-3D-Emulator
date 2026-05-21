using System;
using UnityEngine;

public class RomLoader
{
    public void LoadRom(string filePath)
    {
        byte[] romData = System.IO.File.ReadAllBytes(filePath);
        for (int i = 0; i < romData.Length; i++)
        {
            Memory.memory[0x200 + i] = romData[i];
        }
        Memory.fileSize = (uint)romData.Length;
    }
    public void LoadRomBytes(byte[] romBytes)
    {
        for (int i = 0; i < romBytes.Length; i++)
        {
            Memory.memory[0x200 + i] = romBytes[i];
        }
        Memory.fileSize = (uint)romBytes.Length;
    }
}