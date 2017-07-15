using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moonsolver
{
    class Program
    {
        static Vector<float> calc(uint p)
        {
            float r = (float)(p * Math.PI / 180.0);
            float c = (float)Math.Cos(r);
            float s = (float)Math.Sin(r);
            Matrix<float> m = DenseMatrix.OfArray(new float[,]
            {
                // GLSL matn loads matrices by column!! Math.NET does it by row
                // https://en.wikibooks.org/wiki/GLSL_Programming/Vector_and_Matrix_Operations
                /*{ c, -s, 0.0f },
                { s, c, 0.0f, },
                { 0.0f, 0.0f, 1.0f }*/
                { c, s, 0.0f },
                { -s, c, 0.0f },
                { 0.0f, 0.0f, 1.0f }
            });
            Vector<float> pt = Vector<float>.Build.SparseOfArray(new float[]
            {
                1024.0f, 0.0f, 0.0f
            });
            Vector<float> res = m.Multiply(pt);
            res = res.Add(Vector<float>.Build.SparseOfArray(new float[] { 2048.0f, 2048.0f, 0.0f }));
            return res;
        }

        static uint extend(uint e)
        {
            uint i;
            uint r = e ^ (uint)0x5f208c26;
            for (i = 15; i < 31; i += 3)
            {
                uint f = e << (int)i;
                r ^= f;
            }
            return r;
        }

        static uint hash_alpha(uint p)
        {
            Vector<float> res = calc(p);
            return extend((uint)res.At(0));
        }

        static uint hash_beta(uint p)
        {
            Vector<float> res = calc(p);
            return extend((uint)res.At(1));
        }

        static void hashfunc(uint idx, byte[] password, ref uint[] hash)
        {
            uint final;
            /*if (state[idx] != 1)
            {
                return;
            }*/
            if ((idx & 1) == 0)
            {
                final = hash_alpha(password[idx / 2]);
            }
            else
            {
                final = hash_beta(password[idx / 2]);
            }
            uint i;
            for (i = 0; i < 32; i += 6)
            {
                final ^= idx << (int)i;
            }
            uint h = 0x5a;
            for (i = 0; i < 32; i++)
            {
                uint p = password[i];
                uint r = (i * 3) & 7;
                p = (p << (int)r) | (p >> (int)(8 - r));
                p &= 0xff;
                h ^= p;
            }
            final ^= (h | (h << 8) | (h << 16) | (h << 24));
            hash[idx] = final;
        }

        static uint hashfunc1(uint idx, byte password, uint h)
        {
            uint final;
            /*if (state[idx] != 1)
            {
                return;
            }*/
            if ((idx & 1) == 0)
            {
                final = hash_alpha(password);
            }
            else
            {
                final = hash_beta(password);
            }
            uint i;
            for (i = 0; i < 32; i += 6)
            {
                final ^= idx << (int)i;
            }
            /*uint h = 0x5a;
            for (i = 0; i < 32; i++)
            {
                uint p = password[i];
                uint r = (i * 3) & 7;
                p = (p << (int)r) | (p >> (int)(8 - r));
                p &= 0xff;
                h ^= p;
            }
            h ^= p;*/
            final ^= (h | (h << 8) | (h << 16) | (h << 24));
            return final;
        }
        
        static byte findh(uint[] targetHash)
        {
            for (byte h = 0; h < 255; h++)
            {
                for (byte c = 0; c < 255; c++)
                {
                    if ((hashfunc1(0, c, h) == targetHash[0]) &&
                        (hashfunc1(1, c, h) == targetHash[1]))
                    {
                        return h;
                    }
                }
            }
            return 0;
        }

        static void Main(string[] args)
        {
            string sTargetHash = "30c7ead97107775969be4ba00cf5578f1048ab1375113631dbb6871dbe35162b1c62e982eb6a7512f3274743fb2e55c818912779ef7a34169a838666ff3994bb4d3c6e14ba2d732f14414f2c1cb5d3844935aebbbe3fb206343a004e18a092daba02e3c0969871548ed2c372eb68d1af41152cb3b61f300e3c1a8246108010d282e16df8ae7bff6cb6314d4ad38b5f9779ef23208efe3e1b699700429eae1fa93c036e5dcbe87d32be1ecfac2452ddfdc704a00ea24fbc2161b7824a968e9da1db756712be3e7b3d3420c8f33c37dba42072a941d799ba2eebbf86191cb59aa49a80ebe0b61a79741888cb62341259f62848aad44df2b809383e09437928980f";

            uint[] targetHash = new uint[sTargetHash.Length / 2 / 4];
            for (int i = 0; i < targetHash.Length; i++)
            {
                targetHash[i] = uint.Parse(sTargetHash.Substring(i * 4 * 2, 4 * 2), System.Globalization.NumberStyles.HexNumber);
            }

            Console.Write("Cracking h... ");
            byte p = findh(targetHash);
            Console.WriteLine("h = {0}!", p);

            Console.Write("Cracking password: ");
            for (uint i = 0; i < 32; i++)
            {
                for (byte c = 0; c < 255; c++)
                {
                    if (
                         (hashfunc1(i * 2, c, p) == targetHash[i * 2]) &&
                        (hashfunc1(i * 2 + 1, c, p) == targetHash[i * 2 + 1])
                        )
                    {
                        Console.Write((char) c);
                        break;
                    }
                }
            }
        }
    }
}
