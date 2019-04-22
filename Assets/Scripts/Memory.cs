using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Memory
{
    private byte wReg = 0;
    private ushort[] memory = new ushort[0x100];
    private ushort[] stack = new ushort[8];
    private int stackPos = 0;


    public Memory()
    {
        Reset();
    }

    public void Reset() 
    {
        var oldMemory = memory;
        wReg = 0;
        memory = new ushort[0x100];
        stack = new ushort[8];
        stackPos = 0;

        memory[Address.TMR0] = oldMemory[Address.TMR0];
        memory[Address.FSR] = oldMemory[Address.FSR];
        memory[Address.PORTA] = oldMemory[Address.PORTA];
        memory[Address.PORTB] = oldMemory[Address.PORTB];
        memory[Address.EEDATA] = oldMemory[Address.EEDATA];
        memory[Address.EEADR] = oldMemory[Address.EEADR];
        RBIF = (byte) Bit.get(oldMemory[Address.INTCON], Bit.RBIF);
        ZeroFlag = (byte) Bit.get(oldMemory[Address.STATUS], Bit.Z);
        Carry = (byte) Bit.get(oldMemory[Address.STATUS], Bit.C);
        DigitCarry = (byte) Bit.get(oldMemory[Address.STATUS], Bit.DC);
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
            if (addr == 0x07 || addr > 0x4F) return 0; // Unimplemented memory locations
            if (addr == Address.INDF) return this[this[Address.FSR]]; // Indirect addressing
            if (addr == Address.PCLATH) return 0; // PCLATH is write only

            if (addr != Address.PCL &&      // These registers are the same for every bank
                addr != Address.STATUS &&
                addr != Address.FSR &&
                addr != Address.PCLATH &&
                addr != Address.INTCON &&
                addr < 0x0C)
            {
                addr = (byte) (addr + (Bank << 7)); // Include bank bit in address
            }

            return (byte) memory[addr]; // Read value
        }
        set
        {
            if (addr == 0x07 || addr > 0x4F) return; // Unimplemented memory locations
            if (addr == Address.INDF) this[this[Address.FSR]] = value; // Indirect addressing

            if (addr != Address.PCL &&      // These registers are the same for every bank
                addr != Address.STATUS &&
                addr != Address.FSR &&
                addr != Address.PCLATH &&
                addr != Address.INTCON &&
                addr < 0x0C)
            {
                addr = (byte) (addr + (Bank << 7)); // Include bank bit in address
            }

            memory[addr] = value; // Write value
        }
    }



    public byte w_Register
    {
        get => wReg;
        set
        {
            wReg = value;
        }
    }

    public byte Status
    {
        get => (byte) memory[Address.STATUS];
        set
        {
            memory[Address.STATUS] = value;
        }
    }

    public byte Bank
    {
        get => (byte) Bit.get(Status, Bit.RP0, 2);
    }

    public ushort ProgramCounter // 13 bit
    {
        get => memory[Address.PCL];
        set
        {
            memory[Address.PCL] = (ushort) Bit.mask(value, 13);
        }
    }

    public byte PCL
    {
        get => (byte) memory[Address.PCL];
        set
        {
            memory[Address.PCL] = value;
        }
    }

    public byte PCLATH
    {
        get => (byte) memory[Address.PCLATH];
        set
        {
            memory[Address.PCLATH] = value;
        }
    }

    public byte ZeroFlag
    {
        get => (byte) Bit.get(Status, Bit.Z);
        set
        {
            Status = (byte) Bit.setTo(Status, Bit.Z, value);
        }
    }

    public byte Carry
    {
        get => (byte) Bit.get(Status, Bit.C);
        set
        {
            Status = (byte) Bit.setTo(Status, Bit.C, value);
        }
    }

    public byte DigitCarry
    {
        get => (byte) Bit.get(Status, Bit.DC);
        set
        {
            Status = (byte) Bit.setTo(Status, Bit.DC, value);
        }
    }

    public byte INTCON
    {
        get => (byte) memory[Address.INTCON];
        set
        {
            memory[Address.INTCON] = value;
        }
    }

    public byte RBIF
    {
        get => (byte) Bit.get(INTCON, Bit.RBIF);
        set
        {
            Status = (byte) Bit.setTo(INTCON, Bit.RBIF, value);
        }
    }

}

public static class Address
{
    public const byte INDF = 0x00;
    public const byte TMR0 = 0x01;
    public const byte OPTION = 0x01;
    public const byte PCL = 0x02;
    public const byte STATUS = 0x03;
    public const byte FSR = 0x04;
    public const byte PORTA = 0x05;
    public const byte PORTB = 0x06;
    public const byte TRISA = 0x05;
    public const byte TRISB = 0x06;
    public const byte EEDATA = 0x08;
    public const byte EEADR = 0x09;
    public const byte EECON1 = 0x08;
    public const byte EECON2 = 0x09;
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
