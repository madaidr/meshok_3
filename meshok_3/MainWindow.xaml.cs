using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace meshok_3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IWebDriver driver;
        private string href, des;
        public List<lot> lots = new List<lot>();
        private string full;
        public MainWindow()
        {
            InitializeComponent();
            //listBox1.Items.Add(lots[0].Text.ToString());

        }
        public class lot
        {
            public string Text { get; set; }
            public string GoURL { get; set; }
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--user-data-dir=C:\\Users\\Admin\\AppData\\Local\\Google\\Chrome\\User Data\\Profile 1");
            driver = new OpenQA.Selenium.Chrome.ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMinutes(1); //ожидание
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("file:///C:/Users/Admin/Downloads/igor/Your_file.html"); //ссылка на html файл
            listBox1.Items.Add("Вы вошли на сайт");
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            dann();
            listBox1.Items.Add("Добавлено: " + lots.Count);
            dataGrid.ItemsSource = lots;
            for (int i = 0; i < lots.Count; i++)
            {
                driver.Navigate().GoToUrl(lots[i].GoURL);

                try
                {
                    des = driver.FindElement(By.XPath("//*[@id=\"desc\"]")).Text;
                }
                catch
                {
                    driver.Navigate().GoToUrl(lots[i].GoURL);
                    des = driver.FindElement(By.XPath("//*[@id=\"desc\"]")).Text;
                }
                //des = des.Substring(4);
                des = des.Substring(0, des.Length - 1445);// 1454 всего символов https:/ /text.ru/seo
                lots[i].Text = des;
            }
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            string[] a = { "1003", "1005", "1010", "1020", "1021", "1025", "1026", "1027", "1029", "1030", "1045", "1047", "1048", "1051", "1059 ", "11/26", "1126" };
            for (int i = 0; i < lots.Count; i++)
            {
                for (int j = 0; j < lots[i].Text.Length; j++)
                {
                    if (lots[i].Text[j] == 'Д' || lots[i].Text[j] == 'д')
                    {
                        if (char.IsDigit(lots[i].Text[j + 1]) == true)
                        {
                            if (j != 0)
                            {
                                if (lots[i].Text[j - 1] == '/')
                                {
                                    break;
                                }
                            }
                            for (int p = j; p < lots[i].Text.Length; p++)
                            {
                                if (lots[i].Text[p] == ' ' || lots[i].Text[p] == '.' || lots[i].Text[p] == ',' || p == lots[i].Text.Length - 1)
                                {
                                    full += lots[i].Text[p];
                                    lots[i].Text = full;
                                    full = "";
                                    break;
                                }
                                else
                                {
                                    full += lots[i].Text[p];
                                }
                            }
                        }

                    }
                    if (j + 3 < lots[i].Text.Length) //проверка на договоры
                    {
                        foreach (string o in a)
                        {
                            try
                            {
                                if (lots[i].Text[j] == o[0] && lots[i].Text[j + 1] == o[1] && lots[i].Text[j + 2] == o[2] && lots[i].Text[j + 3] == o[3])
                                {
                                    if (j != 0)
                                    {
                                        if (lots[i].Text[j - 1] == 'U' || lots[i].Text[j - 1] == 'u')
                                        {
                                            full = "U";
                                        }
                                    }
                                    for (int p = j; p < lots[i].Text.Length; p++)
                                    {
                                        if (lots[i].Text[p] == ' ' || lots[i].Text[p] == '.' || lots[i].Text[p] == ',' || p == lots[i].Text.Length - 1)
                                        {
                                            full += lots[i].Text[p];
                                            lots[i].Text = full;
                                            full = "";
                                            break;
                                        }
                                        else
                                        {
                                            full += lots[i].Text[p];
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                Console.WriteLine("Возникло исключение!");
                            }
                        }
                    }
                }
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e) //добавить в текстовый докумет
        {
            string writePath = @"C:\Users\Admin\Desktop\meshok_3\hta.txt";  //расположение вашего текстового документы
            for (int i = 0; i < lots.Count; i++)
            {
                using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
                {
                    sw.Write("\"" + lots[i].Text);
                    if (i != lots.Count - 1)
                    {
                        sw.Write("\",");
                    }

                }
            }
        }




        private void DG_Hyperlink_Click(object sender, RoutedEventArgs e) //открыть гипер ссылку в браузере
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            Process.Start(link.NavigateUri.AbsoluteUri);
        }
        public void dann() //добавление в столбец url
        {
            IReadOnlyList<IWebElement> row = driver.FindElements(By.XPath("//*[@id=\"inftable\"]/tbody/tr"));
            listBox1.Items.Add("Строк: " + row.Count);

            for (int i = 2; i < row.Count; i++)
            //for (int i = 2; i < 10; i++)
            {
                href = driver.FindElement(By.XPath("//*[@id=\"inftable\"]/tbody/tr[" + i + "]/td[1]/a")).GetAttribute("href");
                lots.Add(new lot() { Text = "", GoURL = href });
            }
        }
    }
}
