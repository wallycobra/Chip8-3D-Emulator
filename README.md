# CHIP-8 3D Emulator

A custom CHIP-8 emulator built in Unity and C# featuring a fully interactive 3D cube-based display system instead of traditional 2D pixels.

This project was created to deepen my understanding of low-level systems, emulator architecture, bitwise operations, rendering pipelines, and CPU instruction decoding while combining them with real-time Unity rendering and object-oriented software design principles.

---

## Features

- Full CHIP-8 CPU implementation in C#
- Custom opcode decoder and execution pipeline
- 3D cube-based framebuffer rendered in Unity
- ROM selection menu with dynamic loading
- Keyboard input handling mapped to the CHIP-8 keypad
- Collision detection and XOR sprite rendering
- Delay and sound timer emulation at 60Hz

- Modular emulator architecture separating:
  - CPU logic
  - Opcode handling
  - Display rendering
  - Input systems
  - UI systems

---

## Technical Highlights

### Emulator Architecture

The emulator is structured using a modular object-oriented design:

```text
CPU
 ├── Memory
 ├── Registers
 ├── Stack
 ├── Timers
 ├── Input State
 └── Opcode Handler

Unity Layer
 ├── 3D Cube Display
 ├── ROM Menu System
 ├── Input Mapping
 └── Rendering