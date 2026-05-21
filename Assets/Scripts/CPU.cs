using UnityEngine;

public class CPU : MonoBehaviour
{
    public RomLoader romLoader;
    private readonly OpcodeHandler opcodeHandler;

    public bool[,] display = new bool[64, 32];
    public CubeDisplay cubeDisplay;
    public byte[] V = new byte[16];
    public ushort I;
    public byte delayTimer;
    public byte soundTimer;
    public ushort[] Stack = new ushort[16];
    public byte SP = 0;
    public bool[] Keys = new bool[16];
    private float timerAccumulator;
    [SerializeField] private float cyclesPerSecond = 500;
    private float cycleAccumulator;
    private bool isPaused = false;

    public CPU()
    {
        opcodeHandler = new OpcodeHandler(this);
        cubeDisplay = new CubeDisplay();
        romLoader = new RomLoader();
    }
    private void LoadFontSet()
    {
        for (int i = 0; i < Memory.fontSet.Length; i++)
        {
            Memory.memory[0x50 + i] = Memory.fontSet[i];
        }
    }
    public void LoadRomToMemory()
    {
        romLoader.LoadRom($"{UnityEngine.Application.dataPath}/Resources/Roms/Breakout.ch8");
    }
    public void LoadRom(byte[] romBytes)
    {
        Reset();
        romLoader.LoadRomBytes(romBytes);
    }

    public void Cycle()
    {
        // Fetch opcode (2 bytes)
        ushort opcode = (ushort)((Memory.memory[Memory.pc] << 8) | Memory.memory[Memory.pc + 1]);
        Debug.Log($"Fetched opcode: {opcode:X4} at PC: {Memory.pc:X3}");
        Memory.pc += 2;

        opcodeHandler.HandleOpcode(opcode);
    }

    void Start()
    {
        LoadFontSet();
        cubeDisplay.Initialize();
    }

    public void SetPaused(bool paused)
    {
        isPaused = paused;
    }
    private void Update()
    {
        if (isPaused)
        {
            return;
        }
        UpdateKeys();

        RunCpuCycles();

        UpdateTimers();

        cubeDisplay.RenderFullDisplay(display);
    }
    private void RunCpuCycles()
    {
        cycleAccumulator += Time.deltaTime * cyclesPerSecond;

        while (cycleAccumulator >= 1f)
        {
            Cycle();
            cycleAccumulator -= 1f;
        }
    }

    private void UpdateTimers()
    {
        timerAccumulator += Time.deltaTime;

        float timerInterval = 1f / 60f;

        while (timerAccumulator >= timerInterval)
        {
            if (delayTimer > 0)
            {
                delayTimer--;
            }

            if (soundTimer > 0)
            {
                soundTimer--;

                if (soundTimer > 0)
                {
                    // Play beep here later
                }
            }

            timerAccumulator -= timerInterval;
        }
    }

    public void ShowOpcodes()
    {
        for (int i = 0x200; i < 0x200 + Memory.fileSize; i += 2)
        {
            ushort opcode = (ushort)((Memory.memory[i] << 8) | Memory.memory[i + 1]);
            Debug.Log($"Opcode at {i:X3}: {opcode:X4}");
        }
    }

    public bool TogglePixel(int x, int y)
    {
        bool wasOn = display[x, y];

        display[x, y] ^= true;

        return wasOn && !display[x, y];
    }
    private void UpdateKeys()
    {
        Keys[0x1] = Input.GetKey(KeyCode.Alpha1);
        Keys[0x2] = Input.GetKey(KeyCode.Alpha2);
        Keys[0x3] = Input.GetKey(KeyCode.Alpha3);
        Keys[0xC] = Input.GetKey(KeyCode.Alpha4);

        Keys[0x4] = Input.GetKey(KeyCode.Q);
        Keys[0x5] = Input.GetKey(KeyCode.W);
        Keys[0x6] = Input.GetKey(KeyCode.E);
        Keys[0xD] = Input.GetKey(KeyCode.R);

        Keys[0x7] = Input.GetKey(KeyCode.A);
        Keys[0x8] = Input.GetKey(KeyCode.S);
        Keys[0x9] = Input.GetKey(KeyCode.D);
        Keys[0xE] = Input.GetKey(KeyCode.F);

        Keys[0xA] = Input.GetKey(KeyCode.Z);
        Keys[0x0] = Input.GetKey(KeyCode.X);
        Keys[0xB] = Input.GetKey(KeyCode.C);
        Keys[0xF] = Input.GetKey(KeyCode.V);

        if(Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
        }
    }

    public void Reset()
    {
        Memory.memory = new byte[4096];
        V = new byte[16];
        Stack = new ushort[16];

        I = 0;
        Memory.pc = 0x200;
        SP = 0;
        delayTimer = 0;
        soundTimer = 0;

        ClearDisplay();

        LoadFontSet();
    }
    public void ClearDisplay()
    {
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 32; y++)
            {
                display[x, y] = false;
                cubeDisplay.Render(x, y, false);
            }
        }
        Debug.Log("Display cleared.");
    }
    
}