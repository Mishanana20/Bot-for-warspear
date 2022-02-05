using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using MyImgClassNameSpace;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        ///переменные формы
        ///
        public string idProductFromForm = "";
        public string idProductFromFormCombo = "";
        public int priceidProductFromForm = 1;
        public int kolvoProductFromForm = 1;
        bool client_running = true;
        //Thread myThread;
        //private static ManualResetEventSlim _reset = new ManualResetEventSlim();




        //На этом этапе нужно реализовать клик мышью куда-либо
        //и распознавание "куда" по ключевым точкам

        //для клавиатуры
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        const int BM_SETSTATE = 243;
        const int WM_LBUTTONDOWN = 513;
        const int WM_LBUTTONUP = 514;
        const int WM_KEYDOWN = 256;
        const int WM_CHAR = 258;
        const int WM_KEYUP = 257;
        const int WM_SETFOCUS = 7;
        const int WM_SYSCOMMAND = 274;
        const int SC_MINIMIZE = 32;
        ////////



        [DllImport("user32.dll")]
        private static extern void mouse_event(UInt32 dwFlags, UInt32 dx, UInt32 dy, UInt32 dwData, IntPtr dwExtraInfo);

        private const UInt32 MOVE = 0x0001;
        private const UInt32 LEFTDOWN = 0x0002;
        private const UInt32 LEFTUP = 0x0004;
        private const UInt32 RIGHTDOWN = 0x0008;
        private const UInt32 RIGHTUP = 0x0010;
        private const UInt32 MIDDLEDOWN = 0x0020;
        private const UInt32 MIDDLEUP = 0x0040;
        private const UInt32 WHEEL = 0x0800;
        private const UInt32 ABSOLUTE = 0x8000;






        public Form1()
        {
            InitializeComponent();

            // ProcessStartInfo infoStartProcess = new ProcessStartInfo();
            //Process.Start(@"steam://rungameid/326360");
            //infoStartProcess.FileName = @"C:\Documents and Settings\миша - поля\Главное меню\Программы\Warspear Online";
            //  Process proc = new Process();
            // Process.Start(infoStartProcess);
            //proc.WaitForExit();
            // Thread.Sleep(10000);


            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox1.Items.Add("Arena");
            comboBox1.Items.Add("Teleport");
            comboBox1.Items.Add("Remont");
            comboBox1.Items.Add("Znaku");

            //Работа с мышью
            this.MouseMove += new MouseEventHandler(this.Form1_MouseMove);


            // обрезка нашего изображения
            var vm_left = 8;
            var vm_right = 8;
            var vm_top = 50; //50, обрезали title 
            var vm_bottom = 10;
            //используем название своего окна
            var vm_title = "Warspear Online";

            //если окно не активно
            var handle = FindWindow(null, vm_title);
            if (handle == IntPtr.Zero)
                throw new Exception("Окно не найдено");


            //делаем скрин нашего окна
            RECT rect;
            GetWindowRect(handle, out rect);
            var gameScreenRect = new System.Drawing.Rectangle(rect.Left + vm_left, rect.Top + vm_top, rect.Right - rect.Left - vm_right - vm_left, rect.Bottom - rect.Top - vm_bottom - vm_top);
            var gameBmp = GetScreenImage(gameScreenRect);
            //вставляем изображение в окно
            pictureBox1.Image = gameBmp;

           
           // MessageBox.Show(rect.Left + " " + rect.Top + "\r\n " + this.Location);



            ////// Сейчас попробую масштабировать изображение
            ///
            var gameWindow = MyImgClass.ScaleImageScreen(gameBmp, gameBmp.Width/2, gameBmp.Height/2);
            pictureBox1.Image = gameWindow;

        }


        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            client_running = false;
            //myThread.Abort();
            this.Dispose();
            Application.Exit();
        }


        //private  void TriggerEventOld()
        //{
        //    if (myThread != null)
        //    {
        //        _reset.Set();
        //        myThread.Abort();
        //        // корректнее будет дождаться завершения
        //        // myThread.Join();
        //    }

        //    myThread = new Thread(StartGame);
        //    myThread.IsBackground = true;
        //    _reset.Reset();
        //    myThread.Start();
        //}


        //Еще работа с мышью
        //ага, значит вверху выводятся координаты мыши
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            this.Text = String.Format("X={0}, Y={1}", e.X, e.Y);

        }

        void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            idProductFromFormCombo = comboBox1.SelectedItem.ToString();
            //MessageBox.Show(selectedState);
        }


        public static void LeftClick()
        {
            mouse_event(LEFTDOWN | LEFTUP, 0, 0, 0, IntPtr.Zero);
        }

        //получение координат мыши


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

        public static Bitmap GetScreenImage(Rectangle rect)
        {
            var bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);//Format32bppArgb
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
            }
            return bmp;
        }

        public static Bitmap Get24Image(Bitmap bmp2)
        {
            var bmp = new Bitmap(bmp2.Width, bmp2.Height, PixelFormat.Format24bppRgb);//Format32bppArgb
            Bitmap imgsource = bmp2;

            Bitmap imgtarget = imgsource.Clone(
                    new Rectangle(0, 0, imgsource.Width, imgsource.Height),
                    PixelFormat.Format24bppRgb);

            //imgtarget.Save(@"C:\Users\Public\Pictures\Sample Pictures\" + "test.bmp", ImageFormat.Bmp);
            return imgtarget;
        }



        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //idProductFromForm = id_product.Text;
            //idProductFromFormCombo = idProductFromForm;
            try
            {
                priceidProductFromForm = Convert.ToInt32(price.Text);
            }
            catch { priceidProductFromForm = 1000; price.Text = Convert.ToString(priceidProductFromForm); }
           
            try
            {
                kolvoProductFromForm = Convert.ToInt32(Kolvo.Text);
            }
            catch {kolvoProductFromForm = 1; Kolvo.Text = Convert.ToString(kolvoProductFromForm); }

            //// обрезка нашего изображения
            //var vm_left = 8;
            //var vm_right = 8;
            //var vm_top = 50; //50, обрезали title 
            //var vm_bottom = 10;
            ////используем название своего окна
            //var vm_title = "Warspear Online";

            ////если окно не активно
            //var handle = FindWindow(null, vm_title);
            //if (handle == IntPtr.Zero)
            //    throw new Exception("Окно не найдено");


            ////делаем скрин нашего окна
            //RECT rect;
            //GetWindowRect(handle, out rect);
            //var gameScreenRect = new System.Drawing.Rectangle(rect.Left + vm_left, rect.Top + vm_top, rect.Right - rect.Left - vm_right - vm_left, rect.Bottom - rect.Top - vm_bottom - vm_top);
            //System.Drawing.Bitmap gameBmp = new Bitmap(gameScreenRect.Width, gameScreenRect.Height, PixelFormat.Format24bppRgb);
            // gameBmp = GetScreenImage(gameScreenRect);
            ////вставляем изображение в окно
            //pictureBox1.Image = gameBmp;
            //System.Drawing.Bitmap gameBmp2 = new Bitmap(gameScreenRect.Width, gameScreenRect.Height, PixelFormat.Format24bppRgb);
            //gameBmp2 = GetScreenImage(gameScreenRect);
            //Debug.WriteLine("gameBmp " + gameBmp.PixelFormat.ToString());
            //Debug.WriteLine("gameBmp2 " + gameBmp2.PixelFormat.ToString());


            //////// Сейчас попробую масштабировать изображение
            /////
            ////Bitmap gameWindow = (Bitmap)MyImgClass.ScaleImageScreen(gameBmp, gameBmp.Width / 2, gameBmp.Height / 2);
            ////pictureBox1.Image = gameWindow;

            ////Пробую вставить функцию AForge


            ////System.Drawing.Bitmap sample = new Bitmap(gameWindow);
            ////var sample2 = new Bitmap(gameWindow.Width, gameWindow.Height, PixelFormat.Format24bppRgb);
            ////sample2 =gameWindow;

            ////System.Drawing.Bitmap orig = new Bitmap(gameBmp.Width, gameBmp.Height, PixelFormat.Format24bppRgb);
            ////sample2 = gameWindow;
            ////orig = new Bitmap(gameBmp);



            ////int x, y;

            ////// Loop through the images pixels to reset color.
            ////for (x = 0; x < orig.Width; x++)
            ////{
            ////    for (y = 0; y < orig.Height; y++)
            ////    {
            ////        Color pixelColor = orig.GetPixel(x, y);
            ////        Color newColor = Color.FromArgb(pixelColor.R/100000, 0, 0);
            ////        orig.SetPixel(x, y, newColor);
            ////    }
            ////}



            ////// Display the pixel format in Label1.
            ////Debug.WriteLine("Similar: " + orig.PixelFormat.ToString());



            //TemplateMatch[] matchings ;

            //if (pictureBox1.Image != null) //если в pictureBox есть изображение
            //{
            //    try
            //    {
            //        // path - путь, который был выбран в FolderBrowserDialog()
            //        //image_name - имя для сохранения, можете сделать отдельный TextBox где будете сами его прописывать.
            //        pictureBox1.Image.Save("gameWindowSave" + ".Bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            //    }
            //    catch
            //    {
            //        MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
            //        MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}





            //ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0.90f);
            ////matchings=tm.ProcessImage(orig, sample);
            //matchings = tm.ProcessImage(gameBmp, gameBmp2);
            //Debug.WriteLine("Similar: " + matchings[0].Similarity);

            ////Так, без преобразования координат нажимает на кнопку старт
            ////Вроде как всё отлично работает
            ////System.Drawing.Point p = new System.Drawing.Point(270, 400); //позиция кнопки старт
            ////Cursor.Position = p; //задаем курсору данную позицию
            ////LeftClick(); //вызываем событие нажатия кнопки
            /////АААААААА//
            /////прикинь! можно тоже самое делать
            /////а потом просто от нужной позиции самому делать преобразование
            /////типа "умножить координаты на разность соотношения окна и скрина"
            /////а далее прибавляем разницу от нулевой позиции в игре и нуля в приложении
            /////но это уже потом. Далее работаеv в AForge!!!

            //StartGame();


            (new System.Threading.Thread(delegate ()
            {
                StartGame();
            })).Start();




            //myThread = new Thread(new ThreadStart(StartGame));
            //myThread.IsBackground = true;
            //myThread.Start();

            //TriggerEventOld();
            //myThread.Join();



            //Thread th = new Thread(delegate ()
            //{



            //});
            //th.IsBackground = true;
            //th.Start(StartGame());
            //threads.Add(th);

            //FunctionOfStart();

            // do { FunctionOfNext(); }
            // while (FunctionOfNext());


        }


        private void FunctionOfStart()
        {
            // обрезка нашего изображения
            var vm_left = 8;
            var vm_right = 8;
            var vm_top = 50; //50, обрезали title 
            var vm_bottom = 10;
            //используем название своего окна
            var vm_title = "Warspear Online";

            //если окно не активно
            var handle = FindWindow(null, vm_title);
            if (handle == IntPtr.Zero)
                throw new Exception("Окно не найдено");

            RECT rect;
            GetWindowRect(handle, out rect);

            //делаем скрин игры в 24 бит формате
            var gameScreenRect = new System.Drawing.Rectangle(rect.Left + vm_left, rect.Top + vm_top, rect.Right - rect.Left - vm_right - vm_left, rect.Bottom - rect.Top - vm_bottom - vm_top);
            System.Drawing.Bitmap gameBmpStart = new Bitmap(gameScreenRect.Width, gameScreenRect.Height, PixelFormat.Format24bppRgb);
            gameBmpStart = GetScreenImage(gameScreenRect);
            gameBmpStart = (Bitmap)MyImgClass.ScaleImageScreen(gameBmpStart, gameBmpStart.Width / 2, gameBmpStart.Height / 2); //меняем разрешение в 10 раз
            gameBmpStart = Get24Image((Bitmap)gameBmpStart);
            pictureBox1.Image = gameBmpStart;

            System.Drawing.Bitmap gameBmpStart2 = new Bitmap(gameScreenRect.Width, gameScreenRect.Height, PixelFormat.Format24bppRgb);
            System.Drawing.Image img = System.Drawing.Image.FromFile("Start.Bmp");
            gameBmpStart2 = Get24Image((Bitmap)img);
            gameBmpStart2 = (Bitmap)MyImgClass.ScaleImageScreen(gameBmpStart2, gameBmpStart2.Width / 2, gameBmpStart2.Height / 2); //меняем разрешение в 10 раз
            gameBmpStart2 = Get24Image((Bitmap)gameBmpStart2);


            Debug.WriteLine("gameBmp Start " + gameBmpStart.PixelFormat.ToString());
            Debug.WriteLine("gameBmp2 Start " + gameBmpStart2.PixelFormat.ToString());

            TemplateMatch[] matchings;
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0.90f);
            //matchings=tm.ProcessImage(orig, sample);
            matchings = tm.ProcessImage(gameBmpStart, gameBmpStart2);

            try
            {
                Debug.WriteLine("Similar: " + matchings[0].Similarity);


            

            if (matchings[0].Similarity > 0.9)
            {
                System.Drawing.Point p = new System.Drawing.Point(rect.Left+ 270,rect.Top+ 400); //позиция кнопки старт
                Cursor.Position = p; //задаем курсору данную позицию
                LeftClick();
            }
        }
            catch { }



        }


        private void StartGame()
        {
            var vm_left = 8;
            var vm_right = 8;
            var vm_top = 50; //50, обрезали title 
            var vm_bottom = 10;
            //используем название своего окна
            var vm_title = "Warspear Online";
            var handle = FindWindow(null, vm_title);
          
            RECT rect;
            GetWindowRect(handle, out rect);



            FunctionOfStart(); //Включаю игру

            int countOfTry = 0;
            //do { FunctionOfNext(); countOfTry++; } //нажимаю далее пока не достигну истины
            //while (FunctionOfNext() || countOfTry >= 3); //3 попытки подключиться

            // MyFunctionOfClickXY("Noizemag", 310, 140, 1000);//Выбор перса. Можно изменить
            //MyFunctionOfClick("Start_Play", 2000); //нажимаем старт
            //MyFunctionOfClick("Close", 6000); //закрывваем окно скидок
            //MyFunctionOfClick("No", 2000); //закрывваем окно подарка
            do
            {
                MyFunctionOfClickXYIcon("Perekup", 500);
                MyFunctionOfClick("MyPerekup", 3000);
                Thread.Sleep(400);
                LeftClick();
                //MyFunctionOfClick("MyPerekup", 100);
                //выбираем пустое место
                //                                      //MyFunctionOfClick("EmptyPlace", 100);
                //do {  } //нажимаю далее пока не достигну истины
                if (MyFunctionOfClick("EmptyPlace", 800))
                {
                    Thread.Sleep(400);
                    LeftClick();
                    MyFunctionOfClick(idProductFromFormCombo, 400); //выставляем предмет из формы
                                                                    //FunctionOfNext();
                    //Thread.Sleep(400);
                    //LeftClick();
                    MyFunctionOfClick("Next", 400);
                    //Thread.Sleep(400);
                    //LeftClick();
                    if (kolvoProductFromForm > 2)
                    {
                        MyFunctionOfClick("Right", 400);
                        for (int k = 0; k < (kolvoProductFromForm - 2); k++)
                        {
                            Thread.Sleep(600);
                            LeftClick();
                        }
                        MyFunctionOfClick("OkXP", 600);
                        Thread.Sleep(3000);
                    }
                    MyFunctionOfClick("OkXP", 600);

                    MyFunctionOfClickXY("SellXP",490, 180, 500);


                    Thread.Sleep(3000);
                    MyFunctionOfSell();
                    MyFunctionOfClick("SellXP", 600);
                }
                    
                System.Drawing.Point p = new System.Drawing.Point(rect.Left  + vm_left+50, rect.Top  + vm_top+50); //позиция кнопки 
                Cursor.Position = p; //задаем курсору данную позицию
                Thread.Sleep(500);
                LeftClick();
                Thread.Sleep(500);
                LeftClick();
                MyFunctionOfClickXY("Perekup", 500, 188, 500);
                Thread.Sleep(35000);

            } while(true) ;//было просто true(!_reset.IsSet)









            // return true;
        }

        private bool FunctionOfNext()
        {
            Thread.Sleep(4000); // вариант 
            // обрезка нашего изображения
            var vm_left = 8;
            var vm_right = 8;
            var vm_top = 50; //50, обрезали title 
            var vm_bottom = 10;
            //используем название своего окна
            var vm_title = "Warspear Online";

            //если окно не активно
            var handle = FindWindow(null, vm_title);
            if (handle == IntPtr.Zero)
                throw new Exception("Окно не найдено");

            RECT rect;
            GetWindowRect(handle, out rect);

            //делаем скрин игры в 24 бит формате
            var gameScreenRect = new System.Drawing.Rectangle(rect.Left + vm_left, rect.Top + vm_top, rect.Right - rect.Left - vm_right - vm_left, rect.Bottom - rect.Top - vm_bottom - vm_top);
            System.Drawing.Bitmap gameBmpStart = new Bitmap(gameScreenRect.Width, gameScreenRect.Height, PixelFormat.Format24bppRgb);
            gameBmpStart = GetScreenImage(gameScreenRect);
            gameBmpStart = (Bitmap)MyImgClass.ScaleImageScreen(gameBmpStart, gameBmpStart.Width / 2, gameBmpStart.Height / 2); //меняем разрешение в 2 раз
            gameBmpStart = Get24Image((Bitmap)gameBmpStart);
            pictureBox1.Image = gameBmpStart;

            System.Drawing.Bitmap gameBmpStart2 = new Bitmap(gameScreenRect.Width, gameScreenRect.Height, PixelFormat.Format24bppRgb);
            System.Drawing.Image img = System.Drawing.Image.FromFile("Next-Start.Bmp");
            gameBmpStart2 = Get24Image((Bitmap)img);
            gameBmpStart2 = (Bitmap)MyImgClass.ScaleImageScreen(gameBmpStart2, gameBmpStart2.Width / 2, gameBmpStart2.Height / 2); //меняем разрешение в 2 раз
            gameBmpStart2 = Get24Image((Bitmap)gameBmpStart2);


            Debug.WriteLine("gameBmp Start " + gameBmpStart.PixelFormat.ToString());
            Debug.WriteLine("gameBmp2 Start " + gameBmpStart2.PixelFormat.ToString());

            TemplateMatch[] matchings;
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0.90f);
            //matchings=tm.ProcessImage(orig, sample);
            matchings = tm.ProcessImage(gameBmpStart, gameBmpStart2);

            try
            {
                Debug.WriteLine("Similar: " + matchings[0].Similarity);


                if (matchings[0].Similarity > 0.9)
                {
                    //System.Drawing.Point p = new System.Drawing.Point(rect.Left+ 400,rect.Top+ 500); //позиция кнопки старт
                    //Cursor.Position = p; //задаем курсору данную позицию
                    //LeftClick();

                    int ClickY = matchings[0].Rectangle.Top + (matchings[0].Rectangle.Height/2);
                    int ClickX = matchings[0].Rectangle.Left + (matchings[0].Rectangle.Width/2);

                    //Без соотношения экрана
                    System.Drawing.Point p = new System.Drawing.Point(rect.Left + (ClickX*2)+vm_left, rect.Top + (ClickY*2)+vm_top); //позиция кнопки 
                    Cursor.Position = p; //задаем курсору данную позицию
                    LeftClick();


                }
                return true;
            }
            catch { return false; }



        }

        private bool MyFunctionOfClick(string str, int timer)
        {
            Thread.Sleep(timer); // вариант 
            // обрезка нашего изображения
            var vm_left = 8;
            var vm_right = 8;
            var vm_top = 50; //50, обрезали title 
            var vm_bottom = 10;
            //используем название своего окна
            var vm_title = "Warspear Online";

            //если окно не активно
            var handle = FindWindow(null, vm_title);
            if (handle == IntPtr.Zero)
                throw new Exception("Окно не найдено");

            RECT rect;
            GetWindowRect(handle, out rect);

            //делаем скрин игры в 24 бит формате
            var gameScreenRect = new System.Drawing.Rectangle(rect.Left + vm_left, rect.Top + vm_top, rect.Right - rect.Left - vm_right - vm_left, rect.Bottom - rect.Top - vm_bottom - vm_top);
            System.Drawing.Bitmap gameBmpStart = new Bitmap(gameScreenRect.Width, gameScreenRect.Height, PixelFormat.Format24bppRgb);
            gameBmpStart = GetScreenImage(gameScreenRect);
            gameBmpStart = (Bitmap)MyImgClass.ScaleImageScreen(gameBmpStart, gameBmpStart.Width / 2, gameBmpStart.Height / 2); //меняем разрешение в 2 раз
            gameBmpStart = Get24Image((Bitmap)gameBmpStart);
            pictureBox1.Image = gameBmpStart;

            System.Drawing.Bitmap gameBmpStart2 = new Bitmap(gameScreenRect.Width, gameScreenRect.Height, PixelFormat.Format24bppRgb);
            System.Drawing.Image img = System.Drawing.Image.FromFile(str+".Bmp");
            gameBmpStart2 = Get24Image((Bitmap)img);
            gameBmpStart2 = (Bitmap)MyImgClass.ScaleImageScreen(gameBmpStart2, gameBmpStart2.Width / 2, gameBmpStart2.Height / 2); //меняем разрешение в 2 раз
            gameBmpStart2 = Get24Image((Bitmap)gameBmpStart2);


            Debug.WriteLine("gameBmp Start " + gameBmpStart.PixelFormat.ToString());
            Debug.WriteLine("gameBmp2 Start " + gameBmpStart2.PixelFormat.ToString());

            TemplateMatch[] matchings;
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0.8f);//хотя бы 85% совпадения
            matchings = tm.ProcessImage(gameBmpStart, gameBmpStart2);

            try
            {
                Debug.WriteLine("Similar: " + matchings[0].Similarity);


                if (matchings[0].Similarity > 0.9)
                {
                    int ClickY = matchings[0].Rectangle.Top + (matchings[0].Rectangle.Height / 2);
                    int ClickX = matchings[0].Rectangle.Left + (matchings[0].Rectangle.Width / 2);

                    System.Drawing.Point p = new System.Drawing.Point(rect.Left + (ClickX * 2) + vm_left, rect.Top + (ClickY * 2) + vm_top); //позиция кнопки 
                    Cursor.Position = p; //задаем курсору данную позицию
                    LeftClick();
                    return true;
                }
                return false;
            }
            catch { return false; }
        }



        private bool MyFunctionOfDoubleClick(string str, int timer)
        {
            Thread.Sleep(timer); // вариант 
            // обрезка нашего изображения
            var vm_left = 8;
            var vm_right = 8;
            var vm_top = 50; //50, обрезали title 
            var vm_bottom = 10;
            //используем название своего окна
            var vm_title = "Warspear Online";

            //если окно не активно
            var handle = FindWindow(null, vm_title);
            if (handle == IntPtr.Zero)
                throw new Exception("Окно не найдено");

            RECT rect;
            GetWindowRect(handle, out rect);

            //делаем скрин игры в 24 бит формате
            var gameScreenRect = new System.Drawing.Rectangle(rect.Left + vm_left, rect.Top + vm_top, rect.Right - rect.Left - vm_right - vm_left, rect.Bottom - rect.Top - vm_bottom - vm_top);
            System.Drawing.Bitmap gameBmpStart = new Bitmap(gameScreenRect.Width, gameScreenRect.Height, PixelFormat.Format24bppRgb);
            gameBmpStart = GetScreenImage(gameScreenRect);
            gameBmpStart = (Bitmap)MyImgClass.ScaleImageScreen(gameBmpStart, gameBmpStart.Width / 2, gameBmpStart.Height / 2); //меняем разрешение в 2 раз
            gameBmpStart = Get24Image((Bitmap)gameBmpStart);
            pictureBox1.Image = gameBmpStart;

            System.Drawing.Bitmap gameBmpStart2 = new Bitmap(gameScreenRect.Width, gameScreenRect.Height, PixelFormat.Format24bppRgb);
            System.Drawing.Image img = System.Drawing.Image.FromFile(str + ".Bmp");
            gameBmpStart2 = Get24Image((Bitmap)img);
            gameBmpStart2 = (Bitmap)MyImgClass.ScaleImageScreen(gameBmpStart2, gameBmpStart2.Width / 2, gameBmpStart2.Height / 2); //меняем разрешение в 2 раз
            gameBmpStart2 = Get24Image((Bitmap)gameBmpStart2);


            Debug.WriteLine("gameBmp Start " + gameBmpStart.PixelFormat.ToString());
            Debug.WriteLine("gameBmp2 Start " + gameBmpStart2.PixelFormat.ToString());

            TemplateMatch[] matchings;
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0.85f);//хотя бы 85% совпадения
            matchings = tm.ProcessImage(gameBmpStart, gameBmpStart2);

            try
            {
                Debug.WriteLine("Similar: " + matchings[0].Similarity);


                if (matchings[0].Similarity > 0.9)
                {
                    int ClickY = matchings[0].Rectangle.Top + (matchings[0].Rectangle.Height / 2);
                    int ClickX = matchings[0].Rectangle.Left + (matchings[0].Rectangle.Width / 2);

                    System.Drawing.Point p = new System.Drawing.Point(rect.Left + (ClickX * 2) + vm_left, rect.Top + (ClickY * 2) + vm_top); //позиция кнопки 
                    Cursor.Position = p; //задаем курсору данную позицию
                    LeftClick();
                }
                return true;
            }
            catch { return false; }
        }

        private bool MyFunctionOfClickXY(string str,int X, int Y, int timer)
        {
            Thread.Sleep(timer); // вариант 
            // обрезка нашего изображения
            var vm_left = 8;
            var vm_right = 8;
            var vm_top = 50; //50, обрезали title 
            var vm_bottom = 10;
            //используем название своего окна
            var vm_title = "Warspear Online";

            //если окно не активно
            var handle = FindWindow(null, vm_title);
            if (handle == IntPtr.Zero)
                throw new Exception("Окно не найдено");

            RECT rect;
            GetWindowRect(handle, out rect);

            //делаем скрин игры в 24 бит формате
            var gameScreenRect = new System.Drawing.Rectangle(rect.Left + vm_left, rect.Top + vm_top, rect.Right - rect.Left - vm_right - vm_left, rect.Bottom - rect.Top - vm_bottom - vm_top);
            System.Drawing.Bitmap gameBmpStart = new Bitmap(gameScreenRect.Width, gameScreenRect.Height, PixelFormat.Format24bppRgb);
            gameBmpStart = GetScreenImage(gameScreenRect);
            gameBmpStart = (Bitmap)MyImgClass.ScaleImageScreen(gameBmpStart, gameBmpStart.Width / 2, gameBmpStart.Height / 2); //меняем разрешение в 2 раз
            gameBmpStart = Get24Image((Bitmap)gameBmpStart);
            pictureBox1.Image = gameBmpStart;

            System.Drawing.Bitmap gameBmpStart2 = new Bitmap(gameScreenRect.Width, gameScreenRect.Height, PixelFormat.Format24bppRgb);
            System.Drawing.Image img = System.Drawing.Image.FromFile(str + ".Bmp");
            gameBmpStart2 = Get24Image((Bitmap)img);
            gameBmpStart2 = (Bitmap)MyImgClass.ScaleImageScreen(gameBmpStart2, gameBmpStart2.Width / 2, gameBmpStart2.Height / 2); //меняем разрешение в 2 раз
            gameBmpStart2 = Get24Image((Bitmap)gameBmpStart2);


            Debug.WriteLine("gameBmp Start " + gameBmpStart.PixelFormat.ToString());
            Debug.WriteLine("gameBmp2 Start " + gameBmpStart2.PixelFormat.ToString());

            TemplateMatch[] matchings;
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0.85f);//хотя бы 85% совпадения
            matchings = tm.ProcessImage(gameBmpStart, gameBmpStart2);

            try
            {
                Debug.WriteLine("Similar: " + matchings[0].Similarity);


                if (matchings[0].Similarity > 0.9)
                {
                    int ClickY = matchings[0].Rectangle.Top + (matchings[0].Rectangle.Height / 2);
                    int ClickX = matchings[0].Rectangle.Left + (matchings[0].Rectangle.Width / 2);

                    System.Drawing.Point p = new System.Drawing.Point(rect.Left + (X) + vm_left, rect.Top + (Y) + vm_top); //позиция кнопки 
                    //System.Drawing.Point p = new System.Drawing.Point(rect.Left + (X) , rect.Top + (Y)); //позиция кнопки 
                    Cursor.Position = p; //задаем курсору данную позицию
                    LeftClick();
                }
                return true;
            }
            catch { return false; }
        }

        private bool MyFunctionOfClickXYIcon(string str, int timer)
        {
            Thread.Sleep(timer); // вариант 
            // обрезка нашего изображения
            var vm_left = 8;
            var vm_right = 8;
            var vm_top = 50; //50, обрезали title 
            var vm_bottom = 10;
            //используем название своего окна
            var vm_title = "Warspear Online";

            //если окно не активно
            var handle = FindWindow(null, vm_title);
            if (handle == IntPtr.Zero)
                throw new Exception("Окно не найдено");

            RECT rect;
            GetWindowRect(handle, out rect);

            //делаем скрин игры в 24 бит формате
            var gameScreenRect = new System.Drawing.Rectangle(rect.Left + vm_left, rect.Top + vm_top, rect.Right - rect.Left - vm_right - vm_left, rect.Bottom - rect.Top - vm_bottom - vm_top);
            System.Drawing.Bitmap gameBmpStart = new Bitmap(gameScreenRect.Width, gameScreenRect.Height, PixelFormat.Format24bppRgb);
            gameBmpStart = GetScreenImage(gameScreenRect);
            gameBmpStart = (Bitmap)MyImgClass.ScaleImageScreen(gameBmpStart, gameBmpStart.Width / 2, gameBmpStart.Height / 2); //меняем разрешение в 2 раз
            gameBmpStart = Get24Image((Bitmap)gameBmpStart);
            pictureBox1.Image = gameBmpStart;

            System.Drawing.Bitmap gameBmpStart2 = new Bitmap(gameScreenRect.Width, gameScreenRect.Height, PixelFormat.Format24bppRgb);
            System.Drawing.Image img = System.Drawing.Image.FromFile(str + ".Bmp");
            gameBmpStart2 = Get24Image((Bitmap)img);
            gameBmpStart2 = (Bitmap)MyImgClass.ScaleImageScreen(gameBmpStart2, gameBmpStart2.Width / 2, gameBmpStart2.Height / 2); //меняем разрешение в 2 раз
            gameBmpStart2 = Get24Image((Bitmap)gameBmpStart2);


            Debug.WriteLine("gameBmp Start " + gameBmpStart.PixelFormat.ToString());
            Debug.WriteLine("gameBmp2 Start " + gameBmpStart2.PixelFormat.ToString());

            TemplateMatch[] matchings;
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0.85f);//хотя бы 85% совпадения
            matchings = tm.ProcessImage(gameBmpStart, gameBmpStart2);

            try
            {
                Debug.WriteLine("Similar: " + matchings[0].Similarity);


                if (matchings[0].Similarity > 0.85)
                {
                    int ClickY = matchings[0].Rectangle.Top + (matchings[0].Rectangle.Height / 2);
                    int ClickX = matchings[0].Rectangle.Left + (matchings[0].Rectangle.Width / 2);

                    System.Drawing.Point p = new System.Drawing.Point(rect.Left + (ClickX * 2) + vm_left, rect.Top + (ClickY * 2) + vm_top +135 ); //позиция кнопки 
                    Cursor.Position = p; //задаем курсору данную позицию
                    LeftClick();
                }
                return true;
            }
            catch { return false; }
        }

        private void MyFunctionOfSell()
        {
            string Key = "";
            //SendKeys.SendWait("{BACKSPACE}");
            SendKeys.SendWait("{BACKSPACE}");
            Thread.Sleep(500);
            SendKeys.SendWait("{BACKSPACE}");
            Thread.Sleep(500);
            SendKeys.SendWait("{BACKSPACE}");
            Thread.Sleep(500);
            SendKeys.SendWait("{BACKSPACE}");
            Thread.Sleep(500);
            SendKeys.SendWait("{BACKSPACE}");
            Thread.Sleep(500);
            SendKeys.SendWait("{BACKSPACE}");


            string strPrice = price.Text;
            int Price = Convert.ToInt32(strPrice)*kolvoProductFromForm;
            strPrice = Convert.ToString(Price);
            for (int i =0; i<strPrice.Length;i++)
            {
                Thread.Sleep(500);
                Key = Convert.ToString(strPrice[i]);
                SendKeys.SendWait(Key);
            }


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
