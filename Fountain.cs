using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PipeHat_fountain
{
  class Fountain
  {
    public const string TF = @"yyyy-MM-dd HH:mm:ss";
    public const string DTSTR = @"yyyyMMddHHmmss";
    public const int MegaByte = 1024 * 1024;
    // other stuff
    public static BlockingCollection<String> msgQueue = new BlockingCollection<string>(512);
    private bool isRunning = false;
    private Random rnd = new Random();
    private string[] theDoctors = {"Dr. Mabuse", "Dr. Zhivago", "Dr. Hannibal Lecter", "Doctor Dolittle", "Dr. Jekyll", "Dr. Hyde",
      "Dr. John H. Watson", "Dr. Emmett Brown", "Dr. Montgomery Montgomery", "Dr. Evil", "Dr. Frankenstein", "Dr. Stephen Strange",
      "Dr. Strangelove", "Dr. Who", "Dr. Beverly Crusher", "Dr. Leonard McCoy", "EMH Program AK-1", "Dr. Charles Xavier","Dr. Gaius Baltar",
      "Dr. John Becker", " 	Dr. Frasier Winslow Crane", "Dr. River Song", "Dr. Douglas Howser", "Dr. Maureen Robinson", "Dr. Bob Kelso", "Dr. Perry Cox",
    };
    private string[] thePatients = {
    "Abbott^Hannah", "Bagman^Ludo", "Bagshot^Bathilda", "Bell^Katie", "Binns^Cuthbert", "Black^Phineas Nigellus", "Black^Sirius", "Bones^Amelia", "Bones^Susan",
"Boot^Terry", "Brown^Lavender", "Bulstrode^Millicent", "Burbage^Charity", "Bryce^Frank", "Carrow^Alecto", "Carrow^Amycus", "Cattermole^Reginald", "Chang^Cho",
"Clearwater^Penelope", "Crabbe^Vincent", "Creevey^Colin", "Creevey^Dennis", "Cresswell^Dirk", "Crouch,Sr^Barty", "Crouch,Jr^Barty", "Dawlish^John", "Delacour^Fleur",
"Delacour^Gabrielle", "Diggle^Dedalus", "Diggory^Amos", "Diggory^Cedric", "Doge^Elphias", "Dolohov^Antonin", "Dumbledore^Aberforth", "Dumbledore^Albus", "Dumbledore^Ariana",
"Dumbledore^Kendra", "Dumbledore^Percival", "Dursley^Dudley", "Dursley^Marge", "Dursley^Petunia", "Dursley^Vernon", "Edgecombe^Marietta", "Figg^Arabella", "Filch^Argus",
"Finch-Fletchley^Justin", "Finnigan^Seamus", "Flamel^Nicolas", "Fletcher^Mundungus", "Flitwick^Filius", "Fudge^Cornelius", "Gaunt^Marvolo", "Gaunt^Merope", "Gaunt^Morfin",
"Goldstein^Anthony", "Goyle^Gregory", "Granger^Hermione", "Greengrass^Astoria", "Greyback^Fenrir", "Grindelwald^Gellert", "Grubbly-Plank^Wilhelmina", "Gryffindor^Godric", "Hagrid^Rubeus",
"Hooch^Rolanda", "Hopkirk^Mafalda", "Hufflepuff^Helga", "Johnson^Angelina", "Jordan^Lee", "Jorkins^Bertha", "Karkaroff^Igor", "Krum^Viktor", "Kettleburn^Silvanus",
"Lestrange^Bellatrix", "Lockhart^Gilderoy", "Longbottom^Alice", "Longbottom^Frank", "Longbottom^Augusta", "Longbottom^Neville", "Lovegood^Luna", "Lovegood^Xenophilius", "Lupin^Remus",
"Macnair^Walden", "Malfoy^Draco", "Malfoy^Lucius", "Malfoy^Narcissa", "Malfoy^Scorpius", "Malkin^Madam", "Marchbanks^Griselda", "Maxime^Olympe", "Macmillan^Ernie",
"McGonagall^Minerva", "Midgen^Eloise", "McLaggen^Cormac", "Montague^Graham", "Moody^Alastor", "Nott^Theodore", "Sr^Nott", "Ogden^Bob", "Ollivander^Garrick",
"Parkinson^Pansy", "Patil^Padma", "Patil^Parvati", "Pettigrew^Peter", "Pince^Irma", "Podmore^Sturgis", "Pomfrey^Poppy", "Potter^Harry", "Potter^James",
"Potter^Lily", "Potter^Albus Severus", "Potter^James Sirius", "Potter^Lily Luna", "Quirrell^Quirinus", "Ravenclaw^Helena", "Ravenclaw^Rowena", "Riddle^Delphi", "Riddle^Mary",
"Sr.^Thomas Riddle", "Jr.^Thomas Riddle", "Riddle^Thomas Marvolo", "Robins^Demelza", "Rookwood^Augustus", "Rowle^Thorfinn", "Runcorn^Albert", "Scamander^Newt", "Scrimgeour^Rufus",
"Shacklebolt^Kingsley", "Shunpike^Stan", "Sinistra^Aurora", "Skeeter^Rita", "Slughorn^Horace", "Slytherin^Salazar", "Smith^Zacharias", "Snape^Severus", "Spinnet^Alicia",
"Sprout^Pomona", "Thicknesse^Pius", "Thomas^Dean", "Tonks^Andromeda", "Tonks^Nymphadora", "Tonks^Ted", "Trelawney^Sybill", "Twycross^Wilkie", "Umbridge^Dolores",
"Vance^Emmeline", "Vane^Romilda", "Vector^Septima", "Riddle^Tom Marvolo", "Warren^Myrtle", "Weasley^Arthur", "Weasley^Bill", "Weasley^Charlie", "Weasley^Fred",
"Weasley^George", "Weasley^Ginny", "Weasley^Hugo", "Weasley^Molly", "Weasley^Percy", "Weasley^Ron", "Wood^Oliver", "Weasley^Rose", "Yaxley^Corban", "Zabini^Blaise"
    };
    public string[] theNotes = {"was kissed by a dementor (test message)", "harmed by a spell backfired (test message)",
      "poisened from using wrong ingredient in potion (test message)", "drank wrong potion (test message)",
      "got the wrong end of a love potion (test message)", "broke magical contract (test message)",
      "got hit by a blunger during quidditch (test message)", "got the bones removed by magic quackery (test message)",
      "suffers from imposter syndrome like Harry Potter (test message)", "suffers from PTSD due to encounters with Lord Voldemort (test message)"};
    public enum MenuItem
    {
      /// <summary>
      /// Default menue
      /// </summary>
      DEF,
      /// <summary>
      /// Configuration menu
      /// </summary>
      CONF
    }
    // public string dBServerName = "PMSW-DFSCADMD51";
    // public string dBCatalog = "matthias_test_sql";
    public IPAddress hl7server = IPAddress.Parse("127.0.0.1");
    public int hl7port = 12345;
    private int hl7batchSize = 8;
    private double frequency = 2.0;
    private int numberOfBatches = 1;
    private bool intermittent = true;
    private Thread receiverThread;
    private Thread senderThread;
    private NetworkStream conStream;

    static void Main(string[] args)
    {
      System.Reflection.Assembly assembly = typeof(Fountain).Assembly;
      var version = assembly.GetName().Version;
      if (version == null)
        version = new Version(4711, 0);
      Console.Clear();
      var sd = new DateTime(2000, 1, 1);
      sd = sd.AddDays(version.Build);
      sd = sd.AddSeconds(version.Revision * 2);
      var dstr = sd.ToString(TF);
      FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
      Console.WriteLine($"PipeHat fountain, version: {version.Major}.{version.Minor}, compiled: {dstr}, file version {fvi.FileVersion}");
      try
      {
        var f = new Fountain();
        f.Interact();
        f.isRunning = false;
        Console.WriteLine("Good bye");
        Thread.Sleep(1000);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Exception thrown: {ex.Message}");
        Console.ReadKey();
      }
    }

    private void Interact()
    {
      PrintHelp(MenuItem.DEF);
      while (true)
      {
        ConsoleKeyInfo c = Wait4Key("main >");
        switch (c.Key)
        {
          case ConsoleKey.C:
            Configure();
            break;
          case ConsoleKey.S:
            {
              if (isRunning)
                // benign stopping
                isRunning = false;
              else
                StartTransmission();
            }
            break;
          case ConsoleKey.T:
            CloseConsStopTheads();
            break;
          case ConsoleKey.Q:
          case ConsoleKey.Escape:
            isRunning = false;
            return;
          default:
            PrintHelp(MenuItem.DEF);
            break;
        }
      }

    }

    private void StartTransmission()
    {
      Console.WriteLine("initiating ...");
      isRunning = true;
      try
      {
        if (conStream == null || !conStream.CanWrite)
        {
          Connect2Server();
        }
        if (receiverThread == null)
        {
          receiverThread = new Thread(new ThreadStart(ReceiveMessages))
          {
            IsBackground = true
          };
          receiverThread.Start();
          Thread.Yield();
        }
        senderThread?.Abort();
        senderThread = new Thread(new ThreadStart(SendMessages))
        {
          IsBackground = true
        };
        senderThread.Start();
      }
      catch (Exception ex)
      {
        while (ex != null)
        {
          Console.WriteLine($"{ex.GetType()}: {ex.Message}");
          ex = ex.InnerException;
        }
      }
      Console.WriteLine("running");
    }

    private void Connect2Server()
    {
      try
      {

        conStream?.Dispose();
        conStream = null;
        var con = new TcpClient();
        con.Connect(hl7server, hl7port);
        con.NoDelay = noDelay;
        conStream = con.GetStream();
        msgQueue.Add("connection established");
      }
      catch (Exception ex)
      {
        SyncLog.Write($"Connect2Server got an exception {ex.GetType()}: {ex.Message}");
        msgQueue.Add($"Connection failed: {ex.GetType()}: {ex.Message}");
      }
    }

    private void ReceiveMessages()
    {
      byte[] buffer = new byte[MegaByte];
      SyncLog.Write("receiver started");
      try
      {
        while (isRunning)
        {
          try
          {
            var s = conStream;
            var trial = 0;
            while (s == null)
            {
              if (++trial > 10000)
                return;
              Thread.Yield();
              continue;
            }
            var r = s.Read(buffer, 0, MegaByte);
            var str = Encoding.ASCII.GetString(buffer, 0, r);
            SyncLog.Write($"received {r} bytes: {str}");
          }
          catch (Exception ex)
          {
            if (!isRunning)
              return;
            // wait until senderThread is done with the recreation of that stream
            lock (senderThread ?? receiverThread)
            {
              SyncLog.Write($"receiver got an exception {ex.Message}");
              msgQueue.Add($"exception in receiver {ex.Message}");
            }
          }
        }
      }
      finally
      {
        receiverThread = null;
        SyncLog.Write("receiver stopped");
        msgQueue.Add("receiver stopped");
      }
    }

    private long msgId = 0;
    private bool noDelay = true;

    private void SendMessages()
    {
      try
      {
        for (var bn = 0; bn < numberOfBatches; ++bn)
        {
          if (!isRunning)
            return;
          var batchStart = DateTime.Now;
          var interval = 1000 / frequency;
          for (int i = 0; i < hl7batchSize; ++i)
          {
            var st = batchStart.AddMilliseconds(i * interval);
            var wms = (st - DateTime.Now).TotalMilliseconds;
            if (wms > 0)
            {
              Thread.Sleep(Convert.ToInt32(wms));
            }
            if (!isRunning)
              return;
            var ds = DateTime.Now.ToString(DTSTR);
            var patient = thePatients[rnd.Next(thePatients.Length)];
            // var doctor = theDoctors[rnd.Next(theDoctors.Length)];
            var note = theNotes[rnd.Next(theNotes.Length)];
            var patientFH = GenFHnumber();
            var practitionerFH = GenFHnumber();
            var msg = $"\x0bMSH|^~\\&|415|Notes|Epic|Notes|{ds}||MDM^T02|{++msgId}|P|2.5|||||NO|8859/1|\r" +
              $"PID|||{patientFH}^^^^NIN ||{patient}||19670507||||||||||||{patientFH}|\r" +
              $"PV1|||103||||{practitionerFH}^McCoy^Lenard^^^Dr^Md^SERFNR^^^^SERFNR||||||||||||Testing-{patientFH}-{practitionerFH}|\r" +
              $"ZTX|Notes (Extensor)|Test message|\r" +
              $"TXA||Test message (Extensor)||{ds}||||||||Testing-{patientFH}-{practitionerFH}|\r" +
              $"OBX|1|TX|||{note}|\r\x1c\r";
            var bb = Encoding.ASCII.GetBytes(msg);
            conStream.Write(bb, 0, bb.Length);
            SyncLog.Write($"send msg {msgId}");
            conStream.Flush();
          }
          if (intermittent && bn + 1 < numberOfBatches)
          {
            lock (senderThread)
            {
              Connect2Server();
            }
          }
        }
      }
      finally
      {
        senderThread = null;
        msgQueue.Add("transmission stopped");
      }
    }

    private ConsoleKeyInfo Wait4Key(String prompt)
    {
      var putPrompt = true;
      while (!Console.KeyAvailable)
      {
        if (msgQueue.Count > 0)
        {
          Console.WriteLine();
          Console.WriteLine("===== messages =====");
          while (msgQueue.Count > 0)
            Console.WriteLine(msgQueue.Take());
          Console.WriteLine("===== =====");
          putPrompt = true;
        }
        if (putPrompt)
        {
          Console.Write(prompt);
          putPrompt = false;
        }
        Thread.Sleep(250);
      }
      var c = Console.ReadKey(true);
      Console.WriteLine();
      return c;
    }

    /// <summary>
    /// Gives user the chance to interactively change the configuration
    /// </summary>
    private void Configure()
    {
      CloseConsStopTheads();
      PrintHelp(MenuItem.CONF);
      while (true)
      {
        var c = Wait4Key("config >");
        switch (c.Key)
        {
          case ConsoleKey.A:

            Console.Write("Enter new filename: ");
            SyncLog.LogFileName = Console.ReadLine();
            Console.WriteLine($"new answer filename is {SyncLog.LogFileName}");
            break;
          case ConsoleKey.B:
            {
              Console.Write("Enter new batchsize: ");
              var bs = Console.ReadLine();
              hl7batchSize = Int32.Parse(bs);
              Console.WriteLine($"new batchsize is {hl7batchSize}");
            }
            break;
          case ConsoleKey.F:
            {
              Console.Write("Enter new frequency:  ");
              var f = Console.ReadLine();
              frequency = Double.Parse(f);
              Console.WriteLine($"new frequency is {frequency}");
            }
            break;
          case ConsoleKey.I:
            {
              intermittent = !intermittent;
              if (intermittent)
                Console.WriteLine("reconnecting after each batch");
              else
                Console.WriteLine("connection is open all the time");
            }
            break;

          case ConsoleKey.N:
            {
              Console.Write("Enter the new number of batches: ");
              var n = Console.ReadLine();
              numberOfBatches = Int32.Parse(n);
              Console.WriteLine($"new number of batches is {numberOfBatches}");
            }
            break;
          case ConsoleKey.P:
            {
              Console.Write("Enter new port number: ");
              var ps = Console.ReadLine();
              this.hl7port = Int16.Parse(ps);
              Console.WriteLine($"new port number is {hl7port}");
              break;
            }
          case ConsoleKey.S:
            Console.Write("Enter new server: ");
            var sn = Console.ReadLine();
            this.hl7server = IPAddress.Parse(sn);
            Console.WriteLine($"new server is {hl7server}");
            break;
          case ConsoleKey.T:
            CloseConsStopTheads();
            break;
          case ConsoleKey.Q:
          case ConsoleKey.Escape:
            return;
          default:
            PrintHelp(MenuItem.CONF);
            break;
        }
      }
    }

    private void CloseConsStopTheads()
    {
      senderThread?.Abort();
      receiverThread?.Abort();
      conStream?.Dispose();
      msgQueue.Add("closed connection, stopped threads");
      Thread.Sleep(250);
      senderThread = null;
      receiverThread = null;
    }

    private void PrintHelp(MenuItem item)
    {
      switch (item)
      {
        case MenuItem.CONF:
          Console.Write($"the config commands:\r\n" +
            $"a - answer file, currently '{SyncLog.LogFileName}'\r\n" +
            $"s - hl7 server ip address, currently '{hl7server.ToString()}'\r\n" +
            $"p - hl7 server port, currently '{hl7port}'\r\n" +
            $"b - batch size, currently '{hl7batchSize}'\r\n" +
            $"f - frequency, currently '{frequency}'\r\n" +
            $"n - number of batches '{numberOfBatches}'\r\n" +
            $"i - intermittent '{intermittent}\r\n" +
            $"d - nodelay is set to '{noDelay}'\r\n" +
            $"ESC - exit and go back to main menu\r\n");
          break;
        default:
          {
            var ss = this.isRunning ? "stop" : "start";
            Console.Write($"the key commands:\r\n" +
              $"c - change configuration\r\n" +
              $"s - {ss} the sender(s)\r\n" +
              $"t - terminate all senders\r\n" +
              $"ESC - exit the program\r\n");
          }
          break;
      }
    }

    /// <summary>
    /// Generates a new FH number according to the fødselsnummer scheme
    /// </summary>
    /// <returns>a string of 11 digits satisfying the checksum requirements</returns>
    public string GenFHnumber()
    {
      int[] digits = new int[11];
      digits[0] = 9;
      var rnd = new Random();
      for (var t = 0; t < 10000; ++t)
      {

        for (int i = 1; i < 9; ++i)
        {
          digits[i] = rnd.Next(0, 10);
        }
        // k1 = 11 - ((3 × d1 + 7 × d2 + 6 × m1 + 1 × m2 + 8 × å1 + 9 × å2 + 4 × i1 + 5 × i2 + 2 × i3) mod 11),
        var k1 = 3 * digits[0] + 7 * digits[1] + 6 * digits[2] + digits[3] + 8 * digits[4] + 8 * digits[5] + 4 * digits[6] + 5 * digits[7] + 2 * digits[8];
        k1 = 11 - (k1 % 11);
        if (k1 >= 10)
          continue;
        digits[9] = k1;
        // k2 = 11 - ((5 × d1 + 4 × d2 + 3 × m1 + 2 × m2 + 7 × å1 + 6 × å2 + 5 × i1 + 4 × i2 + 3 × i3 + 2 × k1) mod 11).
        var k2 = 5 * digits[0] + 4 * digits[1] + 3 * digits[2] + 2 * digits[3] + 7 * digits[4] + 6 * digits[5] + 5 * digits[6] + 4 * digits[7] + 3 * digits[8] + 2 * digits[9];
        k2 = 11 - (k2 % 11);
        if (k2 >= 10)
          continue;
        digits[10] = k2;
        break;
      }
      return $"{digits[0]}{digits[1]}{digits[2]}{digits[3]}{digits[4]}{digits[5]}{digits[6]}{digits[7]}{digits[8]}{digits[9]}{digits[10]}";
    }
  }
}
