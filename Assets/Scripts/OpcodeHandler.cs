using UnityEngine;

public class OpcodeHandler
{
    private readonly CPU cpu;

    public OpcodeHandler(CPU cpu)
    {
        this.cpu = cpu;
    }
    public void HandleOpcode(ushort opcode)
    {
        ushort address = (ushort)(opcode & 0x0FFF);
        byte xRegister = (byte)((opcode & 0x0F00) >> 8);
        byte yRegister = (byte)((opcode & 0x00F0) >> 4);
        byte n = (byte)(opcode & 0x000F);
        byte nn = (byte)(opcode & 0x00FF);

        switch (opcode & 0xF000)
        {
            case 0x0000:
                switch (opcode)
                {
                    case 0x00E0: // Clear the display
                        Debug.Log("Opcode 00E0: Clear the display");
                        ClearDisplay();
                        break;
                    case 0x00EE: // Return from subroutine
                        Debug.Log("Opcode 00EE: Return from subroutine");
                        cpu.SP--;
                        Memory.pc = cpu.Stack[cpu.SP];
                        break;
                    default:
                        Debug.LogWarning($"Unknown opcode: {opcode:X4}");
                        break;
                }
                break;

            case 0x1000: // Jump to address NNN
                Debug.Log($"Opcode {opcode:X4}: Jump to address {address:X3}");
                Memory.pc = address;
                break;

            case 0x2000: // Call subroutine at NNN
                Debug.Log($"Opcode {opcode:X4}: Call subroutine at {address:X3}");
                cpu.Stack[cpu.SP] = Memory.pc;
                cpu.SP++;

                Memory.pc = address;
                break;
            case 0x3000: // Skip next instruction if Vx equals NN
                Debug.Log($"Opcode {opcode:X4}: Skip next instruction if V{xRegister} equals {n:X2}");
                if (cpu.V[xRegister] == nn)
                {
                    Memory.pc += 2;
                }
                break;

            case 0x4000: // Skip next instruction if Vx does not equal NN
                Debug.Log($"Opcode {opcode:X4}: Skip next instruction if V{xRegister} does not equal {n:X2}");
                if (cpu.V[xRegister] != nn)
                {
                    Memory.pc += 2;
                }
                break;
            
            case 0x5000: // Skip next instruction if Vx equals Vy
                Debug.Log($"Opcode {opcode:X4}: Skip next instruction if V{xRegister} equals V{yRegister}");
                if (cpu.V[xRegister] == cpu.V[yRegister])
                {
                    Memory.pc += 2;
                }
                break;

            case 0x6000: // Set Vx to NN
                Debug.Log($"Opcode {opcode:X4}: Set V{xRegister} to {n:X2}");
                cpu.V[xRegister] = nn;
                break;    
            case 0x7000: // Add NN to Vx
                Debug.Log($"Opcode {opcode:X4}: Add {n:X2} to V{xRegister}");
                cpu.V[xRegister] += nn;
                break;
            case 0x8000:
                switch (opcode & 0x000F)
                {
                    case 0x0000: // Set Vx to the value of Vy
                        Debug.Log($"Opcode {opcode:X4}: Set V{xRegister} to the value of V{yRegister}");
                        cpu.V[xRegister] = cpu.V[yRegister];
                        break;
                    case 0x0002: // Set Vx to Vx AND Vy
                        Debug.Log($"Opcode {opcode:X4}: Set V{xRegister} to V{xRegister} AND V{yRegister}");
                        cpu.V[xRegister] &= cpu.V[yRegister];
                        break;
                    case 0x0003: // Set Vx to Vx XOR Vy
                        Debug.Log($"Opcode {opcode:X4}: Set V{xRegister} to V{xRegister} XOR V{yRegister}");
                        cpu.V[xRegister] ^= cpu.V[yRegister];
                        break;
                    case 0x0004: // Add Vy to Vx, set VF to 1 if there's a carry
                        Debug.Log($"Opcode {opcode:X4}: Add V{yRegister} to V{xRegister}, set VF to 1 if there's a carry");
                        int sum = cpu.V[xRegister] + cpu.V[yRegister];
                        cpu.V[0xF] = (byte)(sum > 255 ? 1 : 0); // Set carry flag
                        cpu.V[xRegister] = (byte)(sum & 0xFF); // Keep only the last 8 bits
                        break;
                    case 0x0005: // Subtract Vy from Vx, set VF to 0 if there's a borrow
                        Debug.Log($"Opcode {opcode:X4}: Subtract V{yRegister} from V{xRegister}, set VF to 0 if there's a borrow");
                        cpu.V[0xF] = (byte)(cpu.V[xRegister] >= cpu.V[yRegister] ? 1 : 0); // Set borrow flag
                        cpu.V[xRegister] = (byte)(cpu.V[xRegister] - cpu.V[yRegister]);
                        break;
                    case 0x0006: // Shift Vx right by 1, set VF to the least significant bit before the shift
                        Debug.Log($"Opcode {opcode:X4}: Shift V{xRegister} right by 1, set VF to the least significant bit before the shift");
                        cpu.V[0xF] = (byte)(cpu.V[xRegister] & 0x1); // Set VF to LSB before shift
                        cpu.V[xRegister] >>= 1;
                        break;
                    case 0x000E:
                        byte vx = cpu.V[xRegister];
                        cpu.V[0xF] = (byte)((vx & 0x80) >> 7);
                        cpu.V[xRegister] = (byte)(vx << 1);
                    break;


                    default:
                        Debug.LogWarning($"Unknown opcode: {opcode:X4}");
                        break;
                    
                }
                break;
            
            case 0x9000: // Skip next instruction if Vx does not equal Vy
                Debug.Log($"Opcode {opcode:X4}: Skip next instruction if V{xRegister} does not equal V{yRegister}");
                if (cpu.V[xRegister] != cpu.V[yRegister])
                {
                    Memory.pc += 2;
                }
                break;

            case 0xC000: // Set Vx to a random byte AND NN
                Debug.Log($"Opcode {opcode:X4}: Set V{xRegister} to a random byte AND {n:X2}");
                cpu.V[xRegister] = (byte)(Random.Range(0, 256) & nn);
                break;

            case 0xE000:
                switch (opcode & 0x00FF)
                {
                    case 0x009E: // Skip next instruction if key with the value of Vx is pressed
                        Debug.Log($"Opcode {opcode:X4}: Skip next instruction if key with the value of V{xRegister} is pressed");
                        byte key = cpu.V[xRegister];
                        Debug.Log($"Checking key: {key:X2} (V{xRegister})");
                        if (cpu.Keys[key])
                        {
                            Memory.pc += 2;
                        }
                        break;

                    case 0x00A1: // Skip next instruction if key with the value of Vx is not pressed
                        Debug.Log($"Opcode {opcode:X4}: Skip next instruction if key with the value of V{xRegister} is not pressed");
                        key = cpu.V[xRegister];
                        Debug.Log($"Checking key: {key:X2} (V{xRegister})");
                        if (!cpu.Keys[key])
                        {
                            Memory.pc += 2;
                        }
                        break;
                    default:
                        Debug.LogWarning($"Unknown opcode: {opcode:X4}");
                        break;
                }
                break;    

            case 0xF000:
                switch (opcode & 0x00FF)
                {
                    case 0x0007: // Set Vx to the value of the delay timer
                        Debug.Log($"Opcode {opcode:X4}: Set V{xRegister} to the value of the delay timer");
                        cpu.V[xRegister] = cpu.delayTimer;
                        break;
                    case 0x0015: // Set the delay timer to Vx
                        Debug.Log($"Opcode {opcode:X4}: Set the delay timer to V{xRegister}");
                        cpu.delayTimer = cpu.V[xRegister];
                        break;
                    case 0x0018: // Set the sound timer to Vx
                        Debug.Log($"Opcode {opcode:X4}: Set the sound timer to V{xRegister}");
                        cpu.soundTimer = cpu.V[xRegister];
                        break;
                    case 0x0029: // Set I to the location of the sprite for the character in Vx
                        Debug.Log($"Opcode {opcode:X4}: Set I to the location of the sprite for the character in V{xRegister}");
                        byte digit = cpu.V[xRegister];
                        cpu.I = (ushort)(0x50 + digit * 5);
                        break;
                    case 0x0033: // Store the binary-coded decimal representation of Vx in memory locations I, I+1, and I+2
                        Debug.Log($"Opcode {opcode:X4}: Store the binary-coded decimal representation of V{xRegister} in memory locations I, I+1, and I+2");
                        byte value = cpu.V[xRegister];
                        Memory.memory[cpu.I] = (byte)(value / 100); // Hundreds digit
                        Memory.memory[cpu.I + 1] = (byte)((value / 10) % 10); // Tens digit
                        Memory.memory[cpu.I + 2] = (byte)(value % 10); // Ones digit
                        break;
                    case 0x0055: // Store V0 to Vx in memory starting at address I
                        for (int i = 0; i <= xRegister; i++)
                        {
                            Memory.memory[cpu.I + i] = cpu.V[i];
                        }
                        break;
                    case 0x0065:
                        for (int i = 0; i <= xRegister; i++)
                        {
                            cpu.V[i] = Memory.memory[cpu.I + i];
                        }
                        break;
                    case 0x001E: // Add Vx to I
                        Debug.Log($"Opcode {opcode:X4}: Add V{xRegister} to I");
                        cpu.I += cpu.V[xRegister];
                        break;
                    default:
                        Debug.LogWarning($"Unknown opcode: {opcode:X4}");
                        break;
                }
                break;

            case 0xD000: // Draw sprite at (Vx, Vy) with height N
                Debug.Log($"Opcode {opcode:X4}: Draw sprite at (V{xRegister}, V{yRegister}) with height {n}");
                byte x = cpu.V[xRegister];
                byte y = cpu.V[yRegister];
                byte height = n;

                cpu.V[0xF] = 0; // Reset collision flag

                for (int i = 0; i < height; i++)
                {
                    byte spriteByte = Memory.memory[cpu.I + i];

                    for (int j = 0; j < 8; j++)
                    {
                        bool showSpritePixel = (spriteByte & (0x80 >> j)) != 0;
                        if (!showSpritePixel)
                        {
                            continue;
                        }

                        int screenX = (x + j) % 64;
                        int screenY = (y + i) % 32;
                        
                        bool pixelErased = cpu.TogglePixel(screenX, screenY);

                        if(pixelErased)
                        {
                            cpu.V[0xF] = 1; // Set collision flag
                            Debug.Log($"Collision at {screenX}, {screenY}");
                        }
                    }
                }
                break;  

            case 0xA000: // Set I to the address NNN
                Debug.Log($"Opcode {opcode:X4}: Set I to {address:X3}");
                cpu.I = address;
                break;  

            default:
                Debug.LogWarning($"Unknown opcode: {opcode:X4}");
                break;
        }
    }
    private void ClearDisplay()
    {
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 32; y++)
            {
                cpu.display[x, y] = false;
                cpu.cubeDisplay.Render(x, y, false);
            }
        }
        Debug.Log("Display cleared.");
    }
}