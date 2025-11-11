using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ude;

namespace TextSpeedReader
{
    public class JTextFileLib
    {
        public static JTextFileLib instance = null;
        public static JTextFileLib Instance()
        {
            if (instance == null)
            {
                instance = new JTextFileLib();
            }
            return instance;
        }

        char Char10LF = Convert.ToChar(10); //LF
        char Char13CR = Convert.ToChar(13); //CR
        string Str10LF = Convert.ToChar(10).ToString(); //LF
        string Str13CR = Convert.ToChar(13).ToString(); //CR
        string Str13_10CRLF = Convert.ToChar(13).ToString() + Convert.ToChar(10).ToString(); //CRLF

        public bool ReadTxtFile(string fileName, ref string textString, bool isLF2CRLF)
        {
            try
            {
                // 讀取TXT檔內文串
                /*
                    StreamReader str = new StreamReader(@"E:\pixnet\20160614\Lab2_TXT_Read_Write\Read.TXT");
                    StreamReader str = new StreamReader(讀取TXT檔路徑)
                    str.ReadLine(); (一行一行讀取)
                    str.ReadToEnd();(一次讀取全部)
                    str.Close(); (關閉str)
                */
                // Open the text file using a stream reader.
                //using (StreamReader str = new StreamReader(fileName, Encoding.Default)) //添加Encode參數解決中文亂碼問題 
                Encoding encoding = DetectEncoding(fileName);
                using (StreamReader str = new StreamReader(fileName, encoding)) //添加Encode參數解決中文亂碼問題 
                {
                    string tmpString = str.ReadToEnd(); //讀取Txt檔案內容
                    if (isLF2CRLF)
                    {
                        tmpString = ChangeLF2CRLF(tmpString); //還原斷行符號
                    }
                    textString = tmpString; //傳給原文
                    str.Close();
                    // Read the stream to a string, and write the string to the console.
                    //String line = str.ReadToEnd();
                    Console.WriteLine(fileName + " 檔案讀取完成");
                    return true;
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(fileName + " 檔案無法讀取");
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public static Encoding DetectEncoding(string fileName)
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                CharsetDetector detector = new CharsetDetector();
                detector.Feed(fs);
                detector.DataEnd();

                if (detector.Charset != null)
                {
                    // detector.Charset 可能回傳 "UTF-8", "Big5", "windows-1252" 等
                    return Encoding.GetEncoding(detector.Charset);
                }
                else
                {
                    // 無法判斷時，預設 Big5
                    return Encoding.GetEncoding("Big5");
                }
            }
        }

        public static Encoding DetectEncoding_OLD(string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                if (fileStream.Length >= 3)
                {
                    byte[] bom = new byte[4];
                    fileStream.Read(bom, 0, 4);

                    // UTF-8 BOM: EF BB BF
                    if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
                        return Encoding.UTF8;
                    // UTF-16 LE BOM: FF FE
                    if (bom[0] == 0xFF && bom[1] == 0xFE)
                        return Encoding.Unicode;
                    // UTF-16 BE BOM: FE FF
                    if (bom[0] == 0xFE && bom[1] == 0xFF)
                        return Encoding.BigEndianUnicode;
                }
            }
            // 預設 Big5（繁體中文 ANSI）
            return Encoding.GetEncoding("Big5");
        }

        public string ChangeLF2CRLF(string inputString)
        {
            Console.WriteLine("開始 ChangeLF2CRLF()");
            //for (int i = 0; i < 100; i++)
            //{
            //    int N = Convert.ToInt32(inputString[i]);
            //   Console.WriteLine(i + " = " + N);
            //}
            int countPos = 0;
            int countNewPos = 0;
            int countTime = 0;
            while (countPos < inputString.Length && countNewPos != -1)
            {
                countNewPos = inputString.IndexOf(Char10LF, countPos);
                Console.WriteLine(countTime++.ToString() + " " + countNewPos);
                if (countNewPos > 0 && inputString.Substring(countNewPos - 1, 2) != Str13_10CRLF)
                {
                    inputString = inputString.Insert(countNewPos, Str13CR);
                }
                countPos = countNewPos + 1;
            }
            Console.WriteLine("完成 ChangeLF2CRLF()");
            return inputString;
        }

        public bool SaveTxtFile(string fileName, string textString, bool isAppend)
        {
            try
            {
                // These examples assume a "C:\Users\Public\TestFolder" folder on your machine.
                // You can modify the path if necessary.

                // Example #1: Write an array of strings to a file.
                // Create a string array that consists of three lines.
                //string[] textString = { "First line", "Second line", "Third line" };
                // WriteAllLines creates a file, writes a collection of strings to the file,
                // and then closes the file.  You do NOT need to call Flush() or Close().
                //System.IO.File.WriteAllLines(@fileName, textString);

                if (!isAppend) //覆蓋檔案內容
                {
                    // Example #2: Write one string to a text file.
                    //string textString = "A class is the most powerful data type in C#. Like a structure, " +
                    //               "a class defines the data and behavior of the data type. ";
                    // WriteAllText creates a file, writes the specified string to the file,
                    // and then closes the file.    You do NOT need to call Flush() or Close().
                    System.IO.File.WriteAllText(@fileName, textString);
                }
                else //添加在檔案後面
                {
                    using (System.IO.StreamWriter file =
                        new System.IO.StreamWriter(@"C:\Users\Public\TestFolder\WriteLines2.txt", isAppend))
                    {
                        file.WriteLine(textString);
                    }
                }

                // Example #4: Append new text to an existing file.
                // The using statement automatically flushes AND CLOSES the stream and calls
                // IDisposable.Dispose on the stream object.
                return true;
            }
            catch (IOException e)
            {
                Console.WriteLine(fileName + " 檔案無法寫入");
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool SaveTxtArrayFile(string fileName, string[] textString)
        {
            try
            {
                // Example #1: Write an array of strings to a file.
                // Create a string array that consists of three lines.
                //string[] lines = { "First line", "Second line", "Third line" };
                // WriteAllLines creates a file, writes a collection of strings to the file,
                // and then closes the file.  You do NOT need to call Flush() or Close().
                System.IO.File.WriteAllLines(fileName, textString);

                // Example #3: Write only some strings in an array to a file.
                // The using statement automatically flushes AND CLOSES the stream and calls
                // IDisposable.Dispose on the stream object.
                // NOTE: do not use FileStream for text files because it writes bytes, but StreamWriter
                // encodes the output as text.
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fileName))
                //{
                //    foreach (string line in textString)
                //    {
                //        // If the line doesn't contain the word 'Second', write the line to the file.
                //        if (!line.Contains("Second"))
                //        {
                //            file.WriteLine(line);
                //        }
                //    }
                //}


                return true;
            }
            catch (IOException e)
            {
                Console.WriteLine(fileName + " 檔案無法寫入");
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
