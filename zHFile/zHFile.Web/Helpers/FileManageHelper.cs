using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace zHFile.Web.Helpers
{
    public class FileManageHelper
    {
        //public static string FileRootPath = Server.MapPath(@"~/UserFiles/Resource");
        //public static string RootPath = @"UserFiles/Resource";

        public static string FileRootPath = @"/";
        public static string RootPath = @"/";

        /// Function Name:	GetFiles
        /// Create Date:	12 June 2018 
        /// Description:	
        /// Parameters:		path, all
        /// Returns:		System.Collections.Generic.List<object>
        public static List<object> GetFiles(string path, bool all)
        {
            List<object> list = new List<object>(); string str = FileManageHelper.FileRootPath;
            string str2 = string.Format(@"{0}\{1}", str, path);
            if (!Directory.Exists(str2))
            {
                Directory.CreateDirectory(str2);
            }

            DirectoryInfo info = new DirectoryInfo(str2);
            foreach (DirectoryInfo info2 in from x in info.GetDirectories()
                                            orderby x.CreationTime
                                            select x)
            {
                list.Add(new { filename = info2.Name, extension = "folder", typeIcon = "", size = "folder", createtime = info2.CreationTime.ToString(), updatetime = info2.LastWriteTime.ToString(), path = info2.FullName.Replace(str, "") });
            }
            if (all)
            {
                foreach (FileInfo info3 in from x in info.GetFiles()
                                           orderby x.CreationTime
                                           select x)
                {
                    list.Add(FileManageHelper.CreateJsonObject(str, info3));
                }
            }
            return list;

        }

        /// Function Name:	GetDirectoryTree
        /// Create Date:	12 June 2018 
        /// Description:	
        /// Parameters:		id, file
        /// Returns:		System.Collections.Generic.List<object>
        public static List<object> GetDirectoryTree(string id, string file)
        {

            List<object> list = new List<object>();

            string str = FileManageHelper.FileRootPath;
            var isUserTablePng = true;
            var idArray = id.Split('\\');
            if (idArray.Count() > 1)
            {

            }
            if (id == "all")
            {
                ;
            }
            else
            {
                str = string.Format(@"{0}\{1}", str, id);
            }
            if (!Directory.Exists(str))
            {
                Directory.CreateDirectory(str);
            }
            DirectoryInfo info = new DirectoryInfo(str);
            var theGiretorises = (from x in info.GetDirectories()
                                  orderby x.Name
                                  select x).ToList();
            for (int i = 0; i < theGiretorises.Count(); i++)
            {
                DirectoryInfo info2 = theGiretorises[i];
                var thePath = info2.Name;
                if (id != "all")
                {
                    thePath = string.Format(@"{0}\{1}", id, info2.Name);
                }

                var children = FileManageHelper.GetDirectoryTree(thePath, "closed");
                if (file != "closed")
                {
                    list.Add(new { id = thePath, iconCls = "folder_table", state = "open", text = info2.Name, children = children });
                }

                else
                {

                    list.Add(new { id = thePath, state = "closed", text = info2.Name, children = children });

                }
            }
            return list;
        }

        /// Function Name:	CreateFolder
        /// Create Date:	12 June 2018 
        /// Description:	
        /// Parameters:		path
        /// Returns:		object
        public static object CreateFolder(string path)
        {

            string str = FileManageHelper.FileRootPath;
            string str2 = string.Format(@"{0}\{1}", str, path);
            if (Directory.Exists(str2))
            {
                return new { result = "error", message = "The directory is existed！" };
            }
            Directory.CreateDirectory(str2);
            return new { result = "ok", message = "Create successfully" };




        }

        /// Function Name:	DeleteFile
        /// Create Date:	12 June 2018 
        /// Description:	
        /// Parameters:		files
        /// Returns:		object
        public static object DeleteFile(string files)
        {
            try
            {
                string str = FileManageHelper.FileRootPath;

                string[] strArray = files.Split(new char[] { '*' });
                string path = str + strArray[0];
                if (File.Exists(path) && (strArray[1] == "1"))
                {
                    File.Delete(path);
                }
                else if (Directory.Exists(path) && (strArray[1] == "0"))
                {
                    Directory.Delete(path, true);
                }
                else
                {
                    return new { result = "error", message = strArray[0] + "Error" };
                }

                return new { result = "ok", message = "OK" };
            }
            catch (Exception exception)
            {
                return new { result = "error", message = exception.Message };
            }
        }
        /// Function Name:	Delete
        /// Create Date:	12 June 2018 
        /// Description:	
        /// Parameters:		filePath
        /// Returns:		bool
        public static bool Delete(string filePath)
        {
            try
            {

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                else if (Directory.Exists(filePath))
                {
                    Directory.Delete(filePath, true);
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        /// Function Name:	Zip
        /// Create Date:	12 June 2018 
        /// Description:	
        /// Parameters:		path, zipName, files
        /// Returns:		object
        //public static object Zip(string path, string zipName, string[] files)
        //{

        //    string str = FileManageHelper.FileRootPath;
        //    string str2 = string.Format(@"{0}\{1}", str, path);
        //    string str3 = string.Format(@"{0}\{1}.zip", str2, zipName);
        //    if (File.Exists(str3))
        //    {
        //        return new { result = "error", message = "Filename is existed." };
        //    }
        //    try
        //    {

        //        var thepath = files[0].Split('*');
        //        var Path = string.Format(@"{0}\{1}", str, thepath[0]);
        //        if (ZIPHelper.Compress(Path, str3, true))
        //        {
        //            FileInfo fi = new FileInfo(str3);
        //            object obj2 = FileManageHelper.CreateJsonObject(str, fi);
        //            return new { result = "ok", data = obj2 };
        //        }
        //        else
        //        {
        //            return new { result = "error", message = "Error" };
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        return new { result = "error", message = exception.Message };
        //    }
        //}

        /// Function Name:	Move
        /// Create Date:	12 June 2018 
        /// Description:	
        /// Parameters:		path, files
        /// Returns:		object
        public static object Move(string path, string[] files)
        {

            try
            {
                string str = FileManageHelper.FileRootPath;
                string str2 = string.Format(@"{0}\{1}", str, path);
                foreach (string str3 in files)
                {
                    string[] strArray = str3.Split(new char[] { '*' });
                    string str4 = strArray[0].Substring(strArray[0].LastIndexOf(@"/") + 1);
                    string str5 = string.Format(@"{0}\{1}", str, strArray[0]);
                    string str6 = string.Format(@"{0}\{1}", str2, str4);
                    if (str5 == str6)
                    {
                        return new { result = "error", message = "Can't move to same directory." };
                    }
                    if (File.Exists(str6) && (strArray[1] == "1"))
                    {
                        return new { result = "error", message = string.Format("A same filename is existed.", str4) };
                    }
                    if (File.Exists(str5) && (strArray[1] == "1"))
                    {
                        File.Move(str5, str6);
                    }
                    else
                    {
                        if (Directory.Exists(str6) && (strArray[1] == "0"))
                        {
                            return new { result = "error", message = string.Format("A same directory is existed.", str4) };
                        }
                        if (Directory.Exists(str5) && (strArray[1] == "0"))
                        {
                            Directory.Move(str5, str6);
                        }
                        else
                        {
                            return new { result = "error", message = strArray[0] + "-Error." };
                        }
                    }
                }
                return new { result = "ok", message = "Cut OK." };
            }
            catch (Exception exception)
            {
                return new { result = "error", message = exception.Message };
            }
        }
        /// Function Name:	Copy
        /// Create Date:	12 June 2018 
        /// Description:	
        /// Parameters:		path, files
        /// Returns:		object
        public static object Copy(string path, string[] files)
        {
            try
            {
                string str = FileManageHelper.FileRootPath;
                string str2 = string.Format(@"{0}\{1}", str, path);
                foreach (string str3 in files)
                {
                    string[] strArray = str3.Split(new char[] { '*' });
                    string str4 = strArray[0].Substring(strArray[0].LastIndexOf(@"/") + 1);
                    string str5 = string.Format(@"{0}\{1}", str, strArray[0]);
                    string str6 = string.Format(@"{0}\{1}", str2, str4);
                    if (str5 == str6)
                    {
                        return new { result = "error", message = "Can't copy to the same directory" };
                    }
                    if (File.Exists(str6) && (strArray[1] == "1"))
                    {
                        return new { result = "error", message = string.Format("A same filename is existed.", str4) };
                    }
                    if (File.Exists(str5) && (strArray[1] == "1"))
                    {
                        File.Copy(str5, str6);
                    }
                    else
                    {
                        if (Directory.Exists(str6) && (strArray[1] == "0"))
                        {
                            return new { result = "error", message = string.Format("A same directory is existed.", str4) };
                        }
                        if (Directory.Exists(str5) && (strArray[1] == "0"))
                        {
                            FileManageHelper.DireactoryCopy(str5, str6);
                        }
                        else
                        {
                            return new { result = "error", message = strArray[0] + "-Error" };
                        }
                    }
                }
                return new { result = "ok", message = "Copy OK." };
            }
            catch (Exception exception)
            {
                return new { result = "error", message = exception.Message };
            }
        }

        /// Function Name:	Rename
        /// Create Date:	12 June 2018 
        /// Description:	
        /// Parameters:		path, oldFileName, newFileName, type
        /// Returns:		object
        public static object Rename(string path, string oldFileName, string newFileName, string type)
        {
            try
            {

                string str = FileManageHelper.FileRootPath;
                string str2 = string.Format(@"{0}\{1}\{2}", str, path, oldFileName);
                string str3 = string.Format(@"{0}\{1}\{2}{3}", new object[] { str, path, newFileName, Path.GetExtension(oldFileName) });
                if (type == "0")
                {
                    if (!Directory.Exists(str2))
                    {
                        return new { result = "error", message = "Source directory is not existed." };
                    }
                    if (Directory.Exists(str3))
                    {
                        return new { result = "error", message = "The directory is existed." };
                    }
                    Directory.Move(str2, str3);
                    return new { result = "ok", message = "Rename OK" };
                }
                if (!File.Exists(str2))
                {
                    return new { result = "error", message = "Source file is not existed." };
                }
                if (File.Exists(str3))
                {
                    return new { result = "error", message = "The file is existed." };
                }
                File.Move(str2, str3);
                return new { result = "ok", message = "Rename OK." };
            }
            catch (Exception exception)
            {
                return new { result = "error", message = exception.Message };
            }
        }

        /// Function Name:	Upload
        /// Create Date:	12 June 2018 
        /// Description:	
        /// Parameters:		path, file
        /// Returns:		object
        public static object Upload(string path, IFormFile file)
        {
            try
            {
                string str = FileManageHelper.FileRootPath;
                string destpath = string.Format(@"{0}\{1}", str, path);
                if (FileManageHelper.SaveAs(file, destpath).Result > 0)
                {
                    return new { result = "ok", message = "Upload OK." };
                }
                return new { result = "null", message = "No file is moved." };
            }
            catch (Exception exception)
            {
                return new { result = "error", message = exception.Message };
            }
        }

        /// Function Name:	SaveAs
        /// Create Date:	12 June 2018 
        /// Description:	
        /// Parameters:		file, destpath
        /// Returns:		int
        public async static Task<int> SaveAs(IFormFile file, string destpath)
        {
            string fileName = string.Empty;
            string dir = string.Empty;
            if (file != null)
            {
                fileName = Path.GetFileName(file.FileName);
                if (!Directory.Exists(destpath))
                {
                    Directory.CreateDirectory(destpath);
                }
                try
                {
                    string filePath = Path.Combine(destpath, fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return 1;
                }
                catch (Exception)
                {

                    return 0;
                }

            }
            else
            {
                return 0;
            }


        }

        /// Function Name:	Download
        /// Create Date:	12 June 2018 
        /// Description:	
        /// Parameters:		basePath, paths, newFileName
        /// Returns:		string
        public static string Download(string basePath, string[] paths, string newFileName)
        {




            if (!Directory.Exists(newFileName))
            {
                Directory.CreateDirectory(newFileName);
            }

            foreach (var path in paths)
            {
                bool isDireactory = false; string[] thepath = path.Split('*');
                if (thepath[1] != "1")
                {
                    isDireactory = true;
                }

                string str2 = string.Format(@"{0}\{1}", basePath, thepath[0]); 
                string str3 = string.Format(@"{0}\{1}", newFileName, thepath[0]);

                if (isDireactory)
                {

                    DireactoryCopy(str2, str3);
                }
                else
                {
                    FileCopy(str2, str3);
                }


            }


            return newFileName;

        }

        /// Function Name:	DireactoryCopy
        /// Create Date:	12 June 2018 
        /// Description:	
        /// Parameters:		source, target
        /// Returns:		void
        private static void DireactoryCopy(string source, string target)
        {
            if (!target.StartsWith(source, StringComparison.CurrentCultureIgnoreCase))
            {
                DirectoryInfo info = new DirectoryInfo(source);
                DirectoryInfo info2 = new DirectoryInfo(target);
                info2.Create();
                FileInfo[] files = info.GetFiles();
                foreach (FileInfo info3 in files)
                {
                    File.Copy(info3.FullName, info2.FullName + @"/" + info3.Name, true);
                }
                DirectoryInfo[] directories = info.GetDirectories();
                foreach (DirectoryInfo info4 in directories)
                {
                    FileManageHelper.DireactoryCopy(info4.FullName, info2.FullName + @"/" + info4.Name);
                }
            }
        }
        /// Function Name:	FileCopy
        /// Create Date:	12 June 2018 
        /// Description:	
        /// Parameters:		source, target
        /// Returns:		void
        public static void FileCopy(string source, string target)
        {
            bool isrewrite = true;
            System.IO.File.Copy(source, target, isrewrite);
        }

        /// Function Name:	CreateJsonObject
        /// Create Date:	12 June 2018 
        /// Description:	
        /// Parameters:		rootPath, fi
        /// Returns:		object
        public static object CreateJsonObject(string rootPath, FileInfo fi)
        {
            string extension = Path.GetExtension(fi.FullName);
            double num = ((double)fi.Length) / 1024.0;

            string path = fi.FullName.Replace(rootPath, "");

            string spath = path.Replace(@"\\", @"/").Replace(@"\", @"/");
            string fullpath = $"/{RootPath}/{spath}";



            return new
            {
                filename = fi.Name,
                extension = extension,
                typeIcon = "",
                size = num.ToString("0.00") + " KB",
                createtime = fi.CreationTime.ToString(),
                updatetime = fi.LastWriteTime.ToString(),
                path,
                fullpath
            };
        }

    }
}