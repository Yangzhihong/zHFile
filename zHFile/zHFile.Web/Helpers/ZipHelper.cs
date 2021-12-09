
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace zHFile.Web.Helpers
{
    public class ZipHelper
    {
        private static byte[] buffer = new byte[2048];



 
        public static bool Compress(string dir, string targetFileName, bool recursive)
        {
 
            if (File.Exists(targetFileName))
            {
                // if (!_ProcessOverwrite(targetFileName))  
                return false;
            }
            string[] ars = new string[2];
            if (recursive == false)
            {
                //return Compress(dir, targetFileName);  
                ars[0] = dir;
                ars[1] = targetFileName;
                return ZipFileDictory(ars);
            }
            FileStream ZipFile;
            ZipOutputStream ZipStream;

            //open  
            ZipFile = File.Create(targetFileName);
            ZipStream = new ZipOutputStream(ZipFile);

            if (dir != String.Empty)
            {
                _CompressFolder(dir, ZipStream, dir.Substring(3));
            }

            //close  
            ZipStream.Finish();
            ZipStream.Close();

            if (File.Exists(targetFileName))
                return true;
            else
                return false;
        }


        public static bool ZipFileDictory(string[] args)
        {
            ZipOutputStream s = null;
            try
            {
                string[] filenames = Directory.GetFiles(args[0]);

                Crc32 crc = new Crc32();
                s = new ZipOutputStream(File.Create(args[1]));
                s.SetLevel(6);

                foreach (string file in filenames)
                {
                    
                    FileStream fs = File.OpenRead(file);

                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    ZipEntry entry = new ZipEntry(file);

                    entry.DateTime = DateTime.Now;

                    entry.Size = fs.Length;
                    fs.Close();

                    crc.Reset();
                    crc.Update(buffer);

                    entry.Crc = crc.Value;

                    s.PutNextEntry(entry);

                    s.Write(buffer, 0, buffer.Length);

                }

            }
            catch (Exception e)
            {
                return false;
            }

            finally
            {
                s.Finish();
                s.Close();
            }
            return true;
        }




        private static void _CompressFolder(string basePath, ZipOutputStream zips, string zipfolername)
        {
            if (File.Exists(basePath))
            {
                _AddFile(basePath, zips, zipfolername);
                return;
            }
            string[] names = Directory.GetFiles(basePath);
            foreach (string fileName in names)
            {
                _AddFile(fileName, zips, zipfolername);
            }

            names = Directory.GetDirectories(basePath);
            foreach (string folderName in names)
            {
                _CompressFolder(folderName, zips, zipfolername);
            }

        }
        private static void _AddFile(string fileName, ZipOutputStream zips, string zipfolername)
        {
            if (File.Exists(fileName))
            {
                _CreateZipFile(fileName, zips, zipfolername);
            }
        }

        
        private static void _CreateZipFile(string FileToZip, ZipOutputStream zips, string zipfolername)
        {
            try
            {
                FileStream StreamToZip = new FileStream(FileToZip, FileMode.Open, FileAccess.Read);
                string temp = FileToZip;
                string temp1 = zipfolername;
                if (temp1.Length > 0)
                {
                    int i = temp1.LastIndexOf("\\") + 1;
                    int j = temp.Length - i;
                    temp = temp.Substring(i, j);
                }
                ZipEntry ZipEn = new ZipEntry(temp.Substring(3));

                zips.PutNextEntry(ZipEn);
                byte[] buffer = new byte[16384];
                System.Int32 size = StreamToZip.Read(buffer, 0, buffer.Length);
                zips.Write(buffer, 0, size);
                try
                {
                    while (size < StreamToZip.Length)
                    {
                        int sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                        zips.Write(buffer, 0, sizeRead);
                        size += sizeRead;
                    }
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }

                StreamToZip.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
 



 
        public static void UnZipDirectory(string zipDirectoryPath, string unZipDirecotyPath, string Password)
        {
            while (unZipDirecotyPath.LastIndexOf("\\") + 1 == unZipDirecotyPath.Length)
            {
                unZipDirecotyPath = unZipDirecotyPath.Substring(0, unZipDirecotyPath.Length - 1);
            }

            using (ZipInputStream zipStream = new ZipInputStream(File.OpenRead(zipDirectoryPath)))
            {

                
                if (Password != null && Password.Length > 0)
                {
                    zipStream.Password = Password;
                }

                ZipEntry zipEntry = null;
                while ((zipEntry = zipStream.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(zipEntry.Name);
                    string fileName = Path.GetFileName(zipEntry.Name);

                    if (!string.IsNullOrEmpty(directoryName))
                    {
                        Directory.CreateDirectory(unZipDirecotyPath + @"\" + directoryName);
                    }

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        if (zipEntry.CompressedSize == 0)
                            break;
                        if (zipEntry.IsDirectory)
                        {
                            directoryName = Path.GetDirectoryName(unZipDirecotyPath + @"\" + zipEntry.Name);
                            Directory.CreateDirectory(directoryName);
                        }
                        else
                        {
                            if (!Directory.Exists(unZipDirecotyPath))
                            {
                                Directory.CreateDirectory(unZipDirecotyPath);
                            }
                        }

                        using (FileStream stream = File.Create(unZipDirecotyPath + @"\" + zipEntry.Name))
                        {
                            while (true)
                            {
                                int size = zipStream.Read(buffer, 0, buffer.Length);
                                if (size > 0)
                                {
                                    stream.Write(buffer, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }




    }
}