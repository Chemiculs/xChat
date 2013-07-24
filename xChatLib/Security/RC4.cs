using System;
using System.Collections.Generic;
using System.Text;
namespace xChatLib
{
    #region RC4
    /**
    * ARC4
    * 
    * A C# implementation of RC4
    * Copyright (c) 2012 -jD-
    * 
    * Derived from:
    * 		The as3crypto library, Copyright (c) 2007 Henri Torgemane
    */
    public class RC4
    {
        private int i = 0;
        private int j = 0;
        private byte[] S;
        private const uint psize = 256;
        Random random = new Random();
        public RC4(byte[] key = null)
        {
            S = new byte[psize];
            if (key != null)
            {
                init(key);
            }
        }
        public uint getPoolSize()
        {
            return psize;
        }
        public void init(byte[] key)
        {
            int i;
            int j;
            int t;
            for (i = 0; i < 256; ++i)
            {
                S[i] = (byte)i;
            }
            j = 0;
            for (i = 0; i < 256; ++i)
            {
                j = (j + S[i] + key[i % key.Length]) & 255;
                t = S[i]; 
                S[i] = S[j];
                S[j] = (byte)t;
            }
            this.i = 0;
            this.j = 0;
        }
        public uint next()
        {
            int t;
            i = (i + 1) & 255;
            j = (j + S[i]) & 255;
            t = S[i];
            S[i] = S[j];
            S[j] = (byte)t;
            return S[(t + S[i]) & 255];
        }
        public uint getBlockSize()
        {
            return 1;
        }
        public void encrypt(byte[] block)
        {
            uint i = 0;
            while (i < block.Length)
            {
                block[i++] ^= (byte)next();
            }
        }
        public void encrypt(byte[] block, int offset, int length)
        {
            uint i = 0;
            int off = offset;
            while (i < length)
            {
                block[off++] ^= (byte)next();
                i++;
            }
        }
        public void decrypt(byte[] block)
        {
            encrypt(block);
        }
        public void dispose()
        {
            uint i = 0;
            if (S != null)
            {
                for (i = 0; i < S.Length; i++)
                {
                    S[i] = (byte)(random.Next(1) * 256);
                }
                S = null;
            }
            this.i = 0;
            this.j = 0;
        }
        public string toString()
        {
            return "rc4";
        }
    }
    #endregion
}
