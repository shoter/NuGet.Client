using System;
using System.IO;
using Microsoft.VisualStudio.OLE.Interop;

namespace NuGet.Tools.Test.Utils
{
    internal class VsIStreamWrapper : IStream, IDisposable
    {
        private Stream _stream;
        private bool _disposed = false;

        public VsIStreamWrapper(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public void Read(byte[] pv, uint cb, out uint pcbRead)
        {
            pcbRead = (uint)_stream.Read(pv, 0, (int)cb);
        }

        public void Write(byte[] pv, uint cb, out uint pcbWritten)
        {
            _stream.Write(pv, 0, (int)cb);
            pcbWritten = cb;
        }

        public void Seek(LARGE_INTEGER dlibMove, uint dwOrigin, ULARGE_INTEGER[] plibNewPosition)
        {
            throw new System.NotImplementedException();
        }

        public void SetSize(ULARGE_INTEGER libNewSize)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(IStream pstm, ULARGE_INTEGER cb, ULARGE_INTEGER[] pcbRead, ULARGE_INTEGER[] pcbWritten)
        {
            throw new System.NotImplementedException();
        }

        public void Commit(uint grfCommitFlags)
        {
            throw new System.NotImplementedException();
        }

        public void Revert()
        {
            throw new System.NotImplementedException();
        }

        public void LockRegion(ULARGE_INTEGER libOffset, ULARGE_INTEGER cb, uint dwLockType)
        {
            throw new System.NotImplementedException();
        }

        public void UnlockRegion(ULARGE_INTEGER libOffset, ULARGE_INTEGER cb, uint dwLockType)
        {
            throw new System.NotImplementedException();
        }

        public void Stat(STATSTG[] pstatstg, uint grfStatFlag)
        {
            throw new System.NotImplementedException();
        }

        public void Clone(out IStream ppstm)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                throw new InvalidOperationException("Do not dispose more than once");
            }
            else
            {
                _disposed = true;
            }
        }
    }
}
