using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public unsafe class UnmanagedStream : IDisposable
{
    Stream _stream;
    int _length;
    byte* _startPointer;
    byte* _currentPointer;
    public byte* StartPointer => _startPointer;
    public byte* CurrentPointer => _currentPointer;
    public int Length { get { if (_stream != null) return (int)_stream.Length; return _length; } }

    // Developed by LADI(Libegon)
    // still need to finish it, but its actualy works

    public UnmanagedStream(Span<byte> buffer)
    {
        _length = buffer.Length;
        fixed (byte* ptr = &buffer[0])
        {
            _startPointer = ptr;
            _currentPointer = ptr;
        }
    }

    public UnmanagedStream(byte* pointer, int length)
    {
        _length = length;
        _startPointer = pointer;
        _currentPointer = pointer;
    }

    public UnmanagedStream(Stream stream)
    {
        _stream = stream;
    }

    public U ReadValue<U>() where U : unmanaged
    {
        U val;
        var size = sizeof(U);
        if (_stream!=null)
        {
            byte[] buf = new byte[size];
            if (_stream.Read(buf, 0, size) != size) throw new Exception("EOF");
            fixed (byte* a = &buf[0])
                val = ((U*)a)[0];
        }
        else
        {
            val = ((U*)_currentPointer)[0];
            _currentPointer += size;
        }
        return val;
    }

    public U[] ReadValues<U>(int count) where U : unmanaged
    {
        U[] vals = new U[count];
        for (int i = 0; i < count; i++)
            vals[i] = ReadValue<U>();
        return vals;
    }

    public void WriteValue<U>(U value) where U : unmanaged
    {
        var size = sizeof(U);
        if (_stream != null)
        {
            byte[] buf = new byte[size];
            fixed (byte* a = &buf[0])
                ((U*)a)[0] = value;
            _stream.Write(buf, 0, size);
        }
        else
        {
            ((U*)_currentPointer)[0] = value;
            _currentPointer += size;
        }
    }

    public void WriteValues<U>(U[] values) where U : unmanaged
    {
        for (int i = 0; i < values.Length; i++)
            WriteValue(values[i]);
    }

    public void Seek(SeekOrigin where, int offset)
    {
        if (_stream!=null)
        {
            _stream.Seek(offset, where);
            return;
        }
        switch (where)
        {
            case SeekOrigin.Begin: _currentPointer = _startPointer+offset; break;
            case SeekOrigin.Current: _currentPointer += offset; break;
            case SeekOrigin.End: _currentPointer = _startPointer + _length-1 + offset; break;
            default: throw new ArgumentException("SeekOrigin unk type");
        }
    }

    public void Dispose()
    {
        _stream.Close();
        _startPointer = null;
        _currentPointer = null;
        _length = 0;
        _stream = null;
        GC.SuppressFinalize(this);
    }

    public int Pos
    {
        get 
        {
            if (_stream != null)
                return (int)_stream.Position;
            else
                return (int)(_currentPointer-_startPointer);
        }
    }
}