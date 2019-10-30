// <copyright file="StreamsExtension.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace StreamsDemo
{
    using System;
    using System.IO;
    using System.Text;

    // C# 6.0 in a Nutshell. Joseph Albahari, Ben Albahari. O'Reilly Media. 2015
    // Chapter 15: Streams and I/O
    // Chapter 6: Framework Fundamentals - Text Encodings and Unicode
    // https://msdn.microsoft.com/ru-ru/library/system.text.encoding(v=vs.110).aspx

    /// <summary>
    /// .
    /// </summary>
    public static class StreamsExtension
    {
        /// <summary>
        /// Implement by byte copy logic using class FileStream as a backing store stream .
        /// </summary>
        /// <param name="sourcePath">sourcePath.</param>
        /// <param name="destinationPath">destinationPath.</param>
        /// <returns>Amount Of Bytes.</returns>
        public static int ByByteCopy(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            using (var destinationFileStream = new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                FileStream sourceFileStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
                int amountOfBytes = 0;

                int readByte;
                do
                {
                    readByte = sourceFileStream.ReadByte();
                    if (readByte != -1)
                    {
                        destinationFileStream.WriteByte((byte)readByte);
                        amountOfBytes++;
                    }
                }
                while (readByte != -1);

                return amountOfBytes;
            }
        }

        /// <summary>
        /// Implement by byte copy logic using class MemoryStream as a backing store stream.
        /// </summary>
        /// <param name="sourcePath">sourcePath.</param>
        /// <param name="destinationPath">destinationPath.</param>
        /// <returns>Amount Of Bytes.</returns>
        public static int InMemoryByByteCopy(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            int amountOfBytes = 0;

            string readContents;

            // Use StreamReader to read entire file in string
            using (StreamReader streamReader = new StreamReader(sourcePath, Encoding.Default))
            {
                readContents = streamReader.ReadToEnd();
            }

            // Use MemoryStream instance to read from byte array
            // Use System.Text.Encoding class
            using (MemoryStream memoryStream = new MemoryStream(Encoding.Default.GetBytes(readContents)))
            {
                int count = 0;

                while (count < memoryStream.Length)
                {
                    memoryStream.WriteByte(Encoding.Default.GetBytes(readContents)[count++]);
                }

                // Set the position to the beginning of the stream.
                memoryStream.Seek(0, SeekOrigin.Begin);

                count = 0;

                // Use MemoryStream instance to write it content in new byte array
                while (count < memoryStream.Length)
                {
                    (new byte[memoryStream.Length])[count++] = Convert.ToByte(memoryStream.ReadByte());
                    amountOfBytes++;
                }

                // Use Encoding class instance (from step 2) to create char array on byte array content
                char[] defaultEncodingCharArray = new char[Encoding.Default.GetCharCount(new byte[memoryStream.Length], 0, count)];
                Encoding.Default.GetDecoder().GetChars(new byte[memoryStream.Length], 0, count, defaultEncodingCharArray, 0, true);

                // Use StreamWriter here to write char array content in new file
                using (StreamWriter writer = new StreamWriter(destinationPath))
                {
                    writer.Write(defaultEncodingCharArray);
                }
            }

            return amountOfBytes;
        }

        /// <summary>
        /// Implement by block copy logic using FileStream buffer.
        /// </summary>
        /// <param name="sourcePath">sourcePath.</param>
        /// <param name="destinationPath">destinationPath.</param>
        /// <returns>Amount Of Bytes.</returns>
        public static int ByBlockCopy(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            int bufferSize = 1024 * 1024;

            using (FileStream fileStream = new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                FileStream fs = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
                byte[] bytes = new byte[bufferSize];
                int bytesRead = -1;
                int amountOfBytes = 0;

                while ((bytesRead = fs.Read(bytes, 0, bytes.Length)) > 0)
                {
                    amountOfBytes += bytesRead;
                    fileStream.Write(bytes, 0, bytesRead);
                }

                return amountOfBytes;
            }
        }

        /// <summary>
        /// Implement by block copy logic using MemoryStream.
        /// </summary>
        /// <param name="sourcePath">sourcePath.</param>
        /// <param name="destinationPath">destinationPath.</param>
        /// <returns>New File Lenght.</returns>
        public static int InMemoryByBlockCopy(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            // Use InMemoryByByteCopy method's approach
            string readContents;

            // Use StreamReader to read entire file in string
            using (StreamReader streamReader = new StreamReader(sourcePath, Encoding.Default))
            {
                readContents = streamReader.ReadToEnd();
            }

            // Create byte array on base string content - use  System.Text.Encoding class
            byte[] byteArray = Encoding.Default.GetBytes(readContents);
            byte[] newByteArray;

            using (MemoryStream memoryStream = new MemoryStream(byteArray))
            {
                // Use MemoryStream instance to read from byte array
                memoryStream.Write(byteArray, 0, byteArray.Length);

                // Set the position to the beginning of the stream
                memoryStream.Seek(0, SeekOrigin.Begin);
                newByteArray = new byte[memoryStream.Length];

                // Use MemoryStream instance (from step 3) to write it content in new byte array
                memoryStream.Read(newByteArray, 0, newByteArray.Length);
            }

            // Use Encoding class instance to create char array on byte array content
            char[] defaultEncodingCharArray = new char[Encoding.Default.GetCharCount(newByteArray, 0, newByteArray.Length)];
            Encoding.Default.GetDecoder().GetChars(newByteArray, 0, newByteArray.Length, defaultEncodingCharArray, 0, true);

            // Use StreamWriter here to write char array content in new file
            using (StreamWriter streaWriter = new StreamWriter(destinationPath))
            {
                streaWriter.Write(defaultEncodingCharArray);
            }

            return newByteArray.Length;
        }

        /// <summary>
        /// Implement by block copy logic using class-decorator BufferedStream.
        /// </summary>
        /// <param name="sourcePath">sourcePath.</param>
        /// <param name="destinationPath">destinationPath.</param>
        /// <returns>Amount Of Bytes.</returns>
        public static int BufferedCopy(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            int bufferSize = 1024 * 1024;

            int amountOfBytes = 0;

            using (FileStream fileStream = new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.Write))
            using (FileStream fs = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
            using (BufferedStream bsForRead = new BufferedStream(fs, bufferSize))
            using (BufferedStream bsForWrite = new BufferedStream(fileStream, bufferSize))
            {
                byte[] bytes = new byte[bufferSize];
                int bytesRead = -1;

                while ((bytesRead = bsForRead.Read(bytes, 0, bytes.Length)) > 0)
                {
                    amountOfBytes += bytesRead;
                    bsForWrite.Write(bytes, 0, bytesRead);
                }
            }

            return amountOfBytes;
        }

        /// <summary>
        /// Implement by line copy logic using FileStream and classes text-adapters StreamReader/StreamWriter.
        /// </summary>
        /// <param name="sourcePath">sourcePath.</param>
        /// <param name="destinationPath">destinationPath.</param>
        /// <returns>Amount Of Bytes.</returns>
        public static int ByLineCopy(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            int amountOfLines = 0;
            using (FileStream sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
            using (FileStream destinationStream = new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.Write))
            using (StreamReader reader = new StreamReader(sourceStream, Encoding.Default))
            using (StreamWriter writer = new StreamWriter(destinationStream, Encoding.GetEncoding(1252)))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // to avoid adding new line (\n) in the end.
                    if (reader.EndOfStream)
                    {
                        writer.Write(line);
                        break;
                    }

                    writer.WriteLine(line);
                    amountOfLines++;
                }
            }

            return amountOfLines;
        }

        /// <summary>
        /// Implement content comparison logic of two files.
        /// </summary>
        /// <param name="sourcePath">sourcePath.</param>
        /// <param name="destinationPath">destinationPath.</param>
        /// <returns>True if contents equals. False if doesn't.</returns>
        public static bool IsContentEquals(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            byte[] sourceFile = File.ReadAllBytes(sourcePath);
            byte[] destinationFile = File.ReadAllBytes(destinationPath);

            if (sourceFile.Length == destinationFile.Length)
            {
                for (int i = 0; i < sourceFile.Length; i++)
                {
                    if (sourceFile[i] == destinationFile[i])
                    {
                        continue;
                    }

                    return false;
                }

                return true;
            }

            return false;
        }

        private static void InputValidation(string sourcePath, string destinationPath)
        {
            if (sourcePath is null)
            {
                throw new ArgumentNullException($"{sourcePath} is null");
            }

            if (sourcePath.Length == 0)
            {
                throw new ArgumentException($"{sourcePath} is empty");
            }

            if (destinationPath is null)
            {
                throw new ArgumentNullException($"{destinationPath} is null");
            }

            if (destinationPath.Length == 0)
            {
                throw new ArgumentException($"{destinationPath} is empty");
            }

            if (!File.Exists(sourcePath))
            {
                throw new FileNotFoundException("Source file is not found");
            }
        }
    }
}
