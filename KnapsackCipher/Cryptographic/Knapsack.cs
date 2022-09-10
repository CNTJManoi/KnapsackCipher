using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KnapsackCipher.Cryptographic
{
    class Knapsack
    {
        public enum Operation { Encrypt, Decrypt }
        public int[] PublicKey { get; private set; }
        public int[] PrivateKey { get; private set; }
        public int N { get; private set; }
        public int M { get; private set; }
        public Knapsack(int privateKeyLength)
        {
            Random r = new Random();
            RegeneratePrivateKey(privateKeyLength, 1);
            M = PrivateKey.Sum() + 1;
            for (int i = r.Next(1, M / 2); i < 999999; i++)
            {
                if(GCD(M, i) == 1)
                {
                    N = i;
                    break;
                }
            }
            RegeneratePublicKey();
        }
        public string Perform(string input, Operation op)
        {
            string result = string.Empty;

            if (op == Operation.Encrypt)
            {
                foreach (var chunk in ChunksUpto(TextToBitsList(input), PublicKey.Length))
                {
                    int cipherNum = 0;
                    for (int i = 0; i < chunk.Count; i++)
                    {
                        if (chunk.ElementAt(i) == true)
                        {
                            cipherNum += PublicKey[i];
                        }
                    }
                    result += cipherNum + " ";
                }
            }
            else
            {
                int multInverseOfMN = ModInverse(N, M);
                List<bool> bitsList = new List<bool>();
                foreach (string cipherNumStr in input.Trim().Split(' '))
                {
                    int cipherNum = int.Parse(cipherNumStr);
                    int plainNum = (cipherNum * multInverseOfMN) % M;
                    List<bool> chunk = new List<bool>();
                    for (int i = PrivateKey.Length - 1; i >= 0; i--)
                    {
                        if (PrivateKey[i] <= plainNum)
                        {
                            chunk.Add(true);
                            plainNum -= PrivateKey[i];
                        }
                        else
                        {
                            chunk.Add(false);
                        }
                    }
                    chunk.Reverse();
                    bitsList.AddRange(chunk);
                }
                result = BitsListToText(bitsList);
            }

            return result;
        }
        private int[] GenerateSupergrowthSequence(int length, int seed)
        {
            int[] supergrowthSequence = new int[length];
            var rnd = new Random(seed);

            int lastSumOfPrev = 0;
            for (int i = 0; i < length; i++)
            {
                supergrowthSequence[i] = rnd.Next(lastSumOfPrev, lastSumOfPrev + 10);
                lastSumOfPrev += supergrowthSequence[i];
            }

            return supergrowthSequence;
        }
        private void RegeneratePrivateKey(int langth, int seed)
        {
            PrivateKey = GenerateSupergrowthSequence(langth, seed);
        }
        private void RegeneratePublicKey()
        {
            PublicKey = new int[PrivateKey.Length];
            for (int i = 0; i < PrivateKey.Length; i++)
            {
                PublicKey[i] = PrivateKey[i] * N % M;
            }
        }
        private int GCD(int a, int b)
        {
            return (b != 0) ? GCD(b, a % b) : a;
        }
        private List<bool> TextToBitsList(string text)
        {
            List<bool> bitsList = new List<bool>();

            foreach (var oneByte in Encoding.UTF8.GetBytes(text))
            {
                for (int i = 7; i >= 0; i--)
                {
                    bitsList.Add(((oneByte >> i) & 1) == 1);
                }
            }

            return bitsList;
        }
        private string BitsListToText(List<bool> bitsList)
        {
            byte[] byteArray = new byte[bitsList.Count / 8];
            BitArray bitsArray = new BitArray(bitsList.Count - (bitsList.Count % 8));
            for (int i = 0; i < bitsArray.Length; i++)
                bitsArray[i] = bitsList.ElementAt(bitsArray.Length - i - 1);

            bitsArray.CopyTo(byteArray, 0);
            Array.Reverse(byteArray);

            return Encoding.UTF8.GetString(byteArray);
        }
        private int ModInverse(int n, int m)
        {
            n = n % m;
            for (int x = 1; x < m; x++)
                if ((n * x) % m == 1)
                    return x;
            return 1;
        }
        public IEnumerable<List<bool>> ChunksUpto(List<bool> bitsLit, int maxChunkSize)
        {
            for (int i = 0; i < bitsLit.Count; i += maxChunkSize)
                yield return bitsLit.GetRange(i, Math.Min(maxChunkSize, bitsLit.Count - i));
        }
    }
}
