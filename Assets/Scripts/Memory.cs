using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Memory
{
    private byte wReg = 0;
    private ushort[] memory = new ushort[0x100];
    private ushort[] stack = new ushort[8];
    private int stackPos = 0;
    private byte[] eeprom = new byte[64];

    private byte lastValueOfPortA;
    private byte lastValueOfPortB;


    public Memory()
    {
        Reset(true);
    }

    public void Reset(bool powerup = false)
    {
        var oldMemory = memory;
        wReg = 0;
        memory = new ushort[0x100];
        stack = new ushort[8];
        stackPos = 0;
        Prescaler = 0;
        OPTION = 0xFF;
        TRISA = 0xFF;
        TRISB = 0xFF;
        Sleeping = false;
        

        // Unchanged Registers
        memory[Address.TMR0]   = oldMemory[Address.TMR0];
        memory[Address.FSR]    = oldMemory[Address.FSR];
        memory[Address.PORTA]  = oldMemory[Address.PORTA];
        memory[Address.PORTB]  = oldMemory[Address.PORTB];
        memory[Address.EEDATA] = oldMemory[Address.EEDATA];
        memory[Address.EEADR]  = oldMemory[Address.EEADR];

        RBIF                   = (byte) Bit.get(oldMemory[Address.INTCON], Bit.RBIF);
        ZeroFlag               = (byte) Bit.get(oldMemory[Address.STATUS], Bit.Z);
        Carry                  = (byte) Bit.get(oldMemory[Address.STATUS], Bit.C);
        DigitCarry             = (byte) Bit.get(oldMemory[Address.STATUS], Bit.DC);
        Status                 = (byte) Bit.set(oldMemory[Address.STATUS], Bit.TO);
        Status                 = (byte) Bit.set(oldMemory[Address.STATUS], Bit.PD);

        lastValueOfPortA       = (byte) memory[Address.PORTA];
        lastValueOfPortB       = (byte) memory[Address.PORTB];

        if (powerup)
        {
            Status = (byte) Bit.set(Status, Bit.PD, 2); // set TO and PD on power on 
        }
    }

    public void pushStack(ushort value)
    {
        stack[stackPos] = value;
        stackPos = (stackPos + 1) % stack.Length;
    }

    public ushort popStack()
    {
        stackPos = (stackPos + stack.Length - 1) % stack.Length;
        return stack[stackPos];
    }

    public byte this[byte addr] // Overload indexing operator of memory object
    {
        get
        {
            addr = (byte) Bit.mask(addr, 7); // 7 bit

            if (addr == 0x07 || addr > 0x4F) return 0; // Unimplemented memory locations
            if (addr == Address.INDF) return this[this[Address.FSR]]; // Indirect addressing
            if (addr == Address.PCLATH) return 0; // PCLATH is write only
            if (addr == Address.EECON2) return 0;

            if (addr != Address.PCL &&      // These registers are the same for every bank
                addr != Address.STATUS &&
                addr != Address.FSR &&
                addr != Address.PCLATH &&
                addr != Address.INTCON &&
                addr < 0x0C)
            {
                addr = (byte)(addr + (Bank << 7)); // Include bank bit in address
            }

            return (byte)memory[addr]; // Read value
        }
        set
        {
            addr = (byte) Bit.mask(addr, 7); // 7 bit

            if (addr == 0x07 || addr > 0x4F) return; // Unimplemented memory locations
            if (addr == Address.INDF) this[this[Address.FSR]] = value; // Indirect addressing

            if (addr != Address.PCL &&      // These registers are the same for every bank
                addr != Address.STATUS &&
                addr != Address.FSR &&
                addr != Address.PCLATH &&
                addr != Address.INTCON &&
                addr < 0x0C)
            {
                addr = (byte)(addr + (Bank << 7)); // Include bank bit in address
            }
            
            if (addr == Address.TMR0 && Bit.get(OPTION, Bit.PSA) == 0)
            {
                Prescaler = 0; // Reset prescaler on TMR0 write and Prescaler assigned to Timer0
            }

            if (addr == Address.EECON1)
            {
                EECON1 = (byte) (value & 0b11100); // only set EEIF,WRERR,WREN bits

                if (Bit.get(value, Bit.WR) == 1)
                {
                    EECON1 = (byte) Bit.set(EECON1, Bit.WR); // set WR bit
                }
                if (Bit.get(value, Bit.RD) == 1)
                {
                    readEEPROM(); // read directly
                }
            }
            else if (addr == Address.EECON2) // initiate EEPROM write
            {
                if (EECON2 == 0x55 && value == 0xAA)
                {
                    if (Bit.get(value, Bit.WR) == 1)
                    {
                        writeEEPROM();
                        EECON1 = (byte) Bit.clear(EECON1, Bit.WR); // clear WR bit
                    }
                }

                EECON2 = value;
            }
            else if (addr == Address.PCL)
            {
                // apply upper PC bits of PCLATH
                ProgramCounter = (ushort) (value + (Bit.mask(PCLATH, 5) << 8));
            }
            else
            {
                memory[addr] = value; // Write value
            }
        }
    }


    public byte getRaw(byte addr)
    {
        return (byte)memory[addr]; // Read value
    }

    public void setRaw(byte addr, byte value)
    {
        memory[addr] = (ushort) value; // Write value
    }

    public byte getStack(int index)
    {
        return (byte)stack[index];
    }


    public byte w_Register
    {
        get => wReg;
        set => wReg = value;
    }
    public byte Status
    {
        get => (byte) memory[Address.STATUS];
        set => memory[Address.STATUS] = value;
    }

    public byte Bank
    {
        get => (byte)Bit.get(Status, Bit.RP0, 2);
    }

    public ushort ProgramCounter // 13 bit
    {
        get => memory[Address.PCL];
        set => memory[Address.PCL] = (ushort) Bit.mask(value, 13);
    }


    public byte PCL
    {

        get => (byte) memory[Address.PCL];
        set => memory[Address.PCL] = value;
    }


    public byte PCLATH
    {

        get => (byte) memory[Address.PCLATH];
        set => memory[Address.PCLATH] = value;
    }


    public byte ZeroFlag
    {
        get => (byte) Bit.get(Status, Bit.Z);
        set => Status = (byte) Bit.setTo(Status, Bit.Z, value);
    }

    public byte Carry
    {
        get => (byte) Bit.get(Status, Bit.C);
        set => Status = (byte) Bit.setTo(Status, Bit.C, value);
    }

    public byte DigitCarry
    {
        get => (byte) Bit.get(Status, Bit.DC);
        set => Status = (byte) Bit.setTo(Status, Bit.DC, value);
    }

    public byte INTCON
    {
        get => (byte) memory[Address.INTCON];
        set => memory[Address.INTCON] = value;
    }
    
    public byte RBIF
    {
        get => (byte) Bit.get(INTCON, Bit.RBIF);
        set => Status = (byte) Bit.setTo(INTCON, Bit.RBIF, value);
    }
    
    public byte TMR0
    {
        get => (byte) memory[Address.TMR0];
        set => memory[Address.TMR0] = value;
    }
    
    public byte OPTION
    {
        get => (byte) memory[Address.OPTION];
        set => memory[Address.OPTION] = value;
    }
    
    public byte PS
    {
        get => (byte) Bit.mask(OPTION, 3);
        set => OPTION = (byte) (Bit.maskInverted(OPTION, 3) + Bit.mask(value, 3));
    }

    public byte Prescaler {get; set;}

    public byte PORTA
    {
        get => (byte) memory[Address.PORTA];
        set {
            lastValueOfPortA = (byte) memory[Address.PORTA];
            memory[Address.PORTA] = value;
        }
    }

    public byte PORTB
    {
        get => (byte) memory[Address.PORTB];
        set {
            lastValueOfPortB = (byte) memory[Address.PORTB];
            memory[Address.PORTB] = value;
        }
    }

    public byte TRISA
    {
        get => (byte) memory[Address.TRISA];
        set => memory[Address.TRISA] = value;
    }

    public byte TRISB
    {
        get => (byte) memory[Address.TRISB];
        set => memory[Address.TRISB] = value;
    }
    
    public byte EEADR
    {
        get => (byte) memory[Address.EEADR];
        set => memory[Address.EEADR] = value;
    }

    public byte EEDATA
    {
        get => (byte) memory[Address.EEDATA];
        set => memory[Address.EEDATA] = value;
    }
    
    public byte EECON1
    {
        get => (byte) memory[Address.EECON1];
        set => memory[Address.EECON1] = value;
    }

    public byte EECON2
    {
        get => (byte) memory[Address.EECON2];
        set => memory[Address.EECON2] = value;
    }
    
    public bool runT0CKIEdgeDetection() // Returns true if selected edge is detected (only once for every edge)
    {
        var now = Bit.get(PORTA, Bit.RA4);
        var last = Bit.get(lastValueOfPortA, Bit.RA4);
        lastValueOfPortA = PORTA;

        if (now == last) return false;

        if (Bit.get(OPTION, Bit.T0SE) == 0) // rising edge 0->1
        {
            return last < now;
        }
        else // falling edge 1->0
        {
            return last > now;
        }
    }
    
    public bool Sleeping {get; set;}

    public void runRBEdgeDetection() // Returns true if selected edge is detected (only once for every edge)
    {
        // RB0/INT
        var now = Bit.get(PORTB, Bit.INT);
        var last = Bit.get(lastValueOfPortB, Bit.INT);
        bool intEdge = false;

        if (Bit.get(OPTION, Bit.INTEDG) == 1) // rising edge 0->1
        {
            intEdge = last < now;
        }
        else // falling edge 1->0
        {
            intEdge = last > now;
        }

        if (intEdge)
        {
            INTCON = (byte) Bit.set(INTCON, Bit.INTF);

            // Return from sleep if INTCON.INTE is set
            // But only jump ISR if GIE is set
            if (Bit.get(INTCON, Bit.INTE) == 1)
            {
                Sleeping = false;
            }
        }

        // RB:4-7 changed?
        if (Bit.get(PORTB & TRISB, 4, 4) != Bit.get(lastValueOfPortB & TRISB, 4, 4))
        {
            INTCON = (byte) Bit.set(INTCON, Bit.RBIF);
        }

        lastValueOfPortB = PORTB;
    }

    public void readEEPROM()
    {
        if (EEADR >= eeprom.Length)
        {
            EEDATA = 0; // unimplemented eeprom locations
        }
        else
        {
            EEDATA = eeprom[EEADR];
        }
    }

    public void writeEEPROM()
    {
        if (Bit.get(EECON1, Bit.WREN) == 1)
        {
            if (EEADR < eeprom.Length) // limited to 64 bytes
            {
                eeprom[EEADR] = EEDATA;
            }

            EECON1 = (byte) Bit.set(EECON1, Bit.EEIF);
            EECON1 = (byte) Bit.clear(EECON1, Bit.WRERR);
        }
    }
}

public static class Address
{
    public const byte INDF = 0x00;
    public const byte TMR0 = 0x01;
    public const byte OPTION = 0x81;
    public const byte PCL = 0x02;
    public const byte STATUS = 0x03;
    public const byte FSR = 0x04;
    public const byte PORTA = 0x05;
    public const byte PORTB = 0x06;
    public const byte TRISA = 0x85;
    public const byte TRISB = 0x86;
    public const byte EEDATA = 0x08;
    public const byte EEADR = 0x09;
    public const byte EECON1 = 0x88;
    public const byte EECON2 = 0x89;
    public const byte PCLATH = 0x0A;
    public const byte INTCON = 0x0B;
}

public static class Bit
{
    // STATUS register
    public const byte C = 0;
    public const byte DC = 1;
    public const byte Z = 2;
    public const byte PD = 3;
    public const byte TO = 4;
    public const byte RP0 = 5;
    public const byte RP1 = 6;
    public const byte IRP = 7;

    // PORTA register
    public const byte RA0 = 0;
    public const byte RA1 = 1;
    public const byte RA2 = 2;
    public const byte RA3 = 3;
    public const byte RA4 = 4;
    public const byte T0CKI = 4;

    // PORTB register
    public const byte RB0 = 0;
    public const byte INT = 0;
    public const byte RB1 = 1;
    public const byte RB2 = 2;
    public const byte RB3 = 3;
    public const byte RB4 = 4;
    public const byte RB5 = 5;
    public const byte RB6 = 6;
    public const byte RB7 = 7;

    // INTCON register
    public const byte RBIF = 0;
    public const byte INTF = 1;
    public const byte T0IF = 2;
    public const byte RBIE = 3;
    public const byte INTE = 4;
    public const byte T0IE = 5;
    public const byte EEIE = 6;
    public const byte GIE = 7;

    // OPTION register
    public const byte PS0 = 0;
    public const byte PS1 = 1;
    public const byte PS2 = 2;
    public const byte PSA = 3;
    public const byte T0SE = 4;
    public const byte T0CS = 5;
    public const byte INTEDG = 6;
    public const byte RBPU = 7;

    // EECON1 register
    public const byte RD = 0;
    public const byte WR = 1;
    public const byte WREN = 2;
    public const byte WRERR = 3;
    public const byte EEIF = 4;


    public static int get(int value, int bit, int length = 1, bool invertMask = false)
    {
        if (invertMask)
        {
            return (value & ~(((1 << length) - 1) << bit)) >> bit;
        }
        else
        {
            return (value & (((1 << length) - 1) << bit)) >> bit;
        }
    }

    public static int mask(int value, int length)
    {
        return get(value, 0, length);
    }

    public static int maskInverted(int value, int length)
    {
        return get(value, 0, length, true);
    }

    public static int setTo(int value, int bit, int setTo, int length = 1)
    {
        if (setTo == 0)
        {
            return value & ~(((1 << length) - 1) << bit);
        }
        else
        {
            return value | (((1 << length) - 1) << bit);
        }
    }

    public static int set(int value, int bit, int length = 1)
    {
        return setTo(value, bit, 1, length);
    }

    public static int clear(int value, int bit, int length = 1)
    {
        return setTo(value, bit, 0, length);
    }
}
