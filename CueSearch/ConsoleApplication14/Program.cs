using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

namespace ConsoleApplication14
{


    class DirectorySearch
    {
        bool isTitle;
        bool isPerformer;
        string tmp;
        string search;
        string title;
        string performer;
        DirectoryInfo di;
        ArrayList list;
        StreamReader reader;
        FileInfo[] fi;
        ArrayList list_precise;
        //Class for searching directories by given path, title & performer 
        public DirectorySearch(string ti, string per, ArrayList ar, ArrayList ar_pre)
        {
            title = ti.Trim().ToLower();
            if (per != null)
            {
                performer = per.Trim().ToLower();
            }
            if (per.Count() == 0)

            {
        performer = null;
            }

            tmp = null;
            search = null;
            isPerformer = false;
            isTitle = false;
            list = ar;
            list_precise = ar_pre;

        }
        public DirectorySearch(DirectorySearch obj)
        {
            title = obj.title;


            tmp = null;
            search = null;
            isPerformer = obj.isPerformer;
            isTitle = obj.isTitle;
            list = obj.list;


        }
        //Old search for testing purpose
        public void DirectoryFiles(string path)
        {

            string file = null;
            try
            {
                di = new DirectoryInfo(path);
                DirectoryInfo[] di_info = di.GetDirectories();
                Console.WriteLine(path);
                fi = di.GetFiles();
                //Files are in main search directory

                if (fi.Length > 0)
                {

                    for (int j = 0; j < fi.Length; j++)
                    {
                        Console.WriteLine(fi[j].Name);
                        //Searching for .cue file
                        if (fi[j].Name.Contains(".cue")) file = fi[j].Name;


                        if (file != null)
                        {
                            tmp = tmp + "\\" + file;
                            //Opening .cue file if we found one
                            reader = new StreamReader(new FileStream(di.FullName + "\\" + file, FileMode.Open, FileAccess.Read));
                            while (!reader.EndOfStream)
                            {
                                search = reader.ReadLine().Trim().ToLower();

                                if (search.Contains(title)) isTitle = true;
                                if (performer != null && search.Contains(performer)) isPerformer = true;
                            }
                            //Closing stream
                            reader.Close();

                        }
                    }

                    //Returning path to ArrayList paths
                    if (isTitle | isPerformer)
                    {
                        list.Add(tmp);
                        Console.WriteLine(isTitle);
                        Console.WriteLine(isPerformer);
                        Console.WriteLine(tmp);
                    }
                    isTitle = false;
                    isPerformer = false;
                    tmp = path;

                    file = null;



                }
                if (di_info.Length > 0)
                {


                    for (int i = 0; i < di_info.Length; i++)
                    {
                        Console.WriteLine(di_info[i].Name);
                        new DirectorySearch(this).DirectoryFiles(path + "\\" + di_info[i].Name);
                    }





                }
            }
            catch (Exception) { throw; }

        }

        public void NewDirectorySearch(string path)
        {
            Console.WriteLine(path);
            StreamReader reader;
            var set = new HashSet<string> { ".cue" };
            var file_search = from file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                              from ext in set
                              where String.Equals(ext, new FileInfo(file).Extension, StringComparison.OrdinalIgnoreCase)
                              select file;
            foreach (var str in file_search)
            {
               
                if (str != null)
                {

                    //Opening .cue file if we found one
                    reader = new StreamReader(new FileStream(str, FileMode.Open, FileAccess.Read));
                    while (!reader.EndOfStream)
                    {
                        
                        search = reader.ReadLine().Trim().ToLower();

                        if (search.Contains(title)) isTitle = true;
                        if (performer != null && search.Contains(performer)) isPerformer = true;
                       

                        //Closing stream
                       
                    }
                   
                    if (performer != null && (isTitle && isPerformer))
                    {
                        list_precise.Add(str);

                    }
                    if (isTitle) list.Add(str);
                    reader.Close();

                    isTitle = false;
                    isPerformer = false;



                }
            }
        }




        class Start
        {

            static void Main(string[] args)
            {

                StreamWriter err = new StreamWriter(new FileStream("log.dat", FileMode.OpenOrCreate, FileAccess.Write));
                Console.SetError(err);
                Console.Title = "CueSearcher";
                string title = null;
                string performer = null;
                string path = null;
                Console.Write("Hello to CueSearcher.\nEnter title and performer.\nPress <q> to quit");
                Console.WriteLine();
                Console.Write("Title: ");
                title = Console.ReadLine();

                Console.Write("Performer: ");
                performer = Console.ReadLine();
                if (title.ToLower() == "q" | performer.ToLower() == "q")
                {
                    Console.WriteLine("Quiting");
                    return;
                }

                while (title == null)
                {
                    Console.Write("You need to input title name atleast: ");
                    Console.WriteLine();
                    Console.Write("Title: ");

                    Console.Write("Performer: ");
                    performer = Console.ReadLine();

                }

                ArrayList paths = new ArrayList();
                ArrayList paths_precise = new ArrayList();
                Console.Write("Enter path to search: ");
                try
                {
                    path = Console.ReadLine();
                    DirectorySearch search = new DirectorySearch(title, performer, paths, paths_precise);
                    search.NewDirectorySearch(path);
                }
                catch (ArgumentNullException)
                {
                    Console.Write("No directory entered.\nEnter valid directory: ");
                    path = Console.ReadLine();
                    DirectorySearch search = new DirectorySearch(title, performer, paths, paths_precise);
                    search.NewDirectorySearch(path);
                }
                catch (DirectoryNotFoundException exc)
                {
                    Console.Write("Error " + exc);
                    return;
                }
                catch (FileNotFoundException exc)
                {
                    Console.Write("Error " + exc);
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Unknown error occuered.\nProgram will now exit.");
                    Console.Error.WriteLine(exc);
                    err.Flush();
                    err.Close();
                    return;
                }
                Console.WriteLine();
                Console.Write("Searching for {0,-1}, performed by {1,-1}\n", title, performer);

                if (paths_precise.Count > 0)
                {
                    foreach (var str in paths_precise) Console.Write("Files containing title and performer are: " + str + " \n"+ paths_precise.Count);
                }
                else
                {
                    foreach (var str in paths) Console.Write("Files containing only title are: " + str + " \n");
                }
                err.Close();
            }
        }
    }
}

