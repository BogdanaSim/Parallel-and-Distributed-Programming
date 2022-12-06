using System;

namespace Lab4
{
    public class Program
    {
        private static readonly List<string> urls = new List<string>()
        {
            "www.cs.ubbcluj.ro/~rlupsa/edu/pdp/lab-4-futures-continuations.html",
            "www.eed.usv.ro/~vladv/SFP.html",
            "www.cs.ubbcluj.ro/~rares/course/vr/",
            "www.dspbotosani.ro/acte-normative/",
            "www.master-taid.ro/Cursuri/MLAV_files/Introducere%20in%20Python%20MLAV.html"

        };

        static void Main()
        {
            DirectoryInfo di = new DirectoryInfo(@"..\..\..\files");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            CallbacksImpl callbacksImpl = new CallbacksImpl(urls);
            TasksCallbacksImpl tasksImpl = new TasksCallbacksImpl(urls);
            AsyncAwaitTasksCallbacksImpl asyncCallbacksImpl = new AsyncAwaitTasksCallbacksImpl(urls);
            callbacksImpl.Run();
            //tasksImpl.Run();
            //asyncCallbacksImpl.Run();


        }
    }
}
