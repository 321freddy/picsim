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
    private byte stackPos = 0;

    public byte get(byte addr)
    {
        if (addr == 0x07 || addr > 0x4F) return 0; // Unimplemented memory locations
        if (addr == Address.INDF) return get(get(Address.FSR)); // Indirect addressing
        if (addr == Address.PCLATH) return 0; // PCLATH is write only

        if (addr != Address.PCL &&      // These registers are the same for every bank
            addr != Address.STATUS &&
            addr != Address.FSR &&
            addr != Address.PCLATH &&
            addr != Address.INTCON)
        {
            addr = (byte) (addr + (Bank << 7));
        }
        
        return (byte) memory[addr];
    }

    public void set(byte addr, byte value)
    {
        if (addr == 0x07 || addr > 0x4F) return; // Unimplemented memory locations
        if (addr == Address.INDF) set(get(Address.FSR), value); // Indirect addressing

        if (addr != Address.PCL &&      // These registers are the same for every bank
            addr != Address.STATUS &&
            addr != Address.FSR &&
            addr != Address.PCLATH &&
            addr != Address.INTCON)
        {
            addr = (byte) (addr + (Bank << 7));
        }

        memory[addr] = value;
    }

    public byte w_Register
    {
        get => wReg;
        set
        {
            wReg = value;
            if (wReg == 0)
            {
                setZeroFlag();
            }
            else
            {
                clearZeroFlag();
            }
        }
    }

    public byte Bank
    {
        get => (byte) ((get(Address.STATUS) & 0b0011_0000) >> 5);
    }

    public ushort ProgramCounter // 13 bit
    {
        get => memory[Address.PCL];
        set
        {
            memory[Address.PCL] = (ushort) (value & 0b0001_1111_1111_1111);
        }
    }

    public byte ZeroFlag
    {
        get => (byte) ((get(Address.STATUS) & 0b0000_0100) >> 2);
    }

    public void setZeroFlag()
    {
        set(Address.STATUS, (byte)(get(Address.STATUS) | 0b0000_0100));
    }

    public void clearZeroFlag()
    {
        set(Address.STATUS, (byte) (get(Address.STATUS) & 0b1111_1011));
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
