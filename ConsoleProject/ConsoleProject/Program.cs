using System.Collections;
using System.Reflection;

public class Program
{
    static Queue<string> isimQueue = new Queue<string>();
    static object isimKilidi = new object();
    static bool tamamlandi = false;
    public static void Main(string[] args)
    {

        Thread threadNot = new Thread(NotdanEkle); // Nottan Okuması için Thread başlattım.
        threadNot.Start();

        for (int i = 0; i<5; i++) // Queuedan okuması için Thread Başlattım
        {
            Thread threadOku = new Thread(QueuedanOku);
            threadOku.Start();
        }

        threadNot.Join();


        lock(isimKilidi) 
        {
            tamamlandi = true;
            Monitor.PulseAll(isimKilidi);
        }
    }

    static void NotdanEkle() 
    {
            lock (isimKilidi)
            {
            string dosyaYolu = "dosya.txt"; // Denemek istiyen bunu değiştirmesi lazımdır.
            string[] satırlar = File.ReadAllLines(dosyaYolu);
            foreach (var s in satırlar)
            {
                isimQueue.Enqueue(s);
            }
        }
    }

    static void QueuedanOku()
    {
        while (true)
        {
            string isim = null;

            lock (isimKilidi)
            {

                while (isimQueue.Count == 0 && !tamamlandi)
                {
                    Monitor.Wait(isimKilidi);
                }

                if(isimQueue.Count > 0)
                {
                    isim = isimQueue.Dequeue();
                    Console.WriteLine("Thread Id:" + Thread.CurrentThread.ManagedThreadId + " İsim:" + isim + " : " + isim.Length);
                }

                if (tamamlandi && isimQueue.Count == 0)
                {
                    break;
                }
                Thread.Sleep(3000);
            }
        }
    }
}
