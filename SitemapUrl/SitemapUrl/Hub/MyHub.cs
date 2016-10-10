using Microsoft.AspNet.SignalR;
using SitemapUrl.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SitemapUrl
{
    public class MyHub : Hub
    {
        private Context context = new Context();
        private int[] speed = null;
        public void GetSitemap(string url)
        {
            if (url!=null)
            {
                if (new Regex(@"^((http|https?|ftp)\:\/\/)?([a-z0-9]{1})((\.[a-z0-9-])|([a-z0-9-]))*\.([a-z]{2,6})(\/?)$").IsMatch(url))
                {
                    List<Url> AllUrls = new List<Url>();
                    List<string> Urls = GetUrlChildren(url).Result;
                    speed = new int[3] { 0, 0, 0 };
                    for (int i = 0; i < Urls.Count; i++)
                    {
                        var time = GetRequestTime(Urls[i]);
                        AllUrls.Add(new Url() { UrlName = Urls[i], Time = time });
                        //Clients.All.addNewItem(Urls[i], time);
                        AllUrls.Sort((x, y) => y.Time.CompareTo(x.Time));
                        CreateTableInClient(AllUrls);
                        speed = SortBySpeed(AllUrls, time);
                        Clients.Caller.createDiagram(speed, AllUrls.Count);
                    }
                    SaveUrl(AllUrls, url);
                }
                else
                {
                    return;
                }
               
            }
            else
            {
                return;
            }

        }
        public void RemoveHistory()
        {
            var allParents = context.ParentUrls.ToList();
            var allUrls = context.Urls.ToList();
            for (int a = 0; a < allParents.Count; a++)
            {
                context.ParentUrls.Remove(allParents[a]);
            }
            for (int i = 0; i < allUrls.Count; i++)
            {
                context.Urls.Remove(allUrls[i]);
            }
            context.SaveChanges();
        }
        public void LoadHistory(int id)
        {
            try
            {
                var list = context.Urls.Where(u => u.ParentId == id).ToList();
                CreateTableInClient(list);
                speed = new int[3] { 0, 0, 0 };
                for (int i = 0; i < list.Count; i++)
                {
                    speed = SortBySpeed(list, list[i].Time);
                }
                Clients.Caller.createDiagram(speed, list.Count);
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            
        }
        #region Вспомогательные функции
        private void SaveUrl(List<Url> list, string url)
        {
            context.ParentUrls.Add(new ParentUrl() { Url = url });
            for (int i = 0; i < list.Count; i++)
            {
                context.Urls.Add(list[i]);
            }
            context.SaveChanges();

        }
        private void CreateTableInClient(List<Url> list)
        {
            try
            {
                string[] currentUrls = list.Select(u => u.UrlName).ToArray();
                double[] currentTimes = list.Select(u => u.Time).ToArray();
                Clients.Caller.createGraphic(currentUrls, currentTimes);
            }
            catch (ArgumentNullException ex)
            {  
                throw ex;
            }
           
        }

        private int[] SortBySpeed(List<Url> list, double time)
        {
            //var speed = new int[3] { 0, 0, 0 };

            var avr = list.Select(u => u.Time).Average();

            if (time > 1.32 * avr)
            {
                speed[2] += 1;
            }
            if (time < 0.7 * avr)
            {
                speed[0] += 1;
            }
            if (time > 0.7 * avr && time < 1.32 * avr)
            {
                speed[1] += 1;
            }
            return speed;
        }
        private double GetRequestTime(string url)
        {
            try
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                timer.Stop();
                double ts = Math.Round(timer.Elapsed.TotalSeconds, 4);
                return ts;
            }
            catch (Exception)
            {
                
                throw;
            }
           
        }
        private async Task<List<string>> GetUrlChildren(string url)
        {
            List<string> urlsChild = new List<string>();

            HttpClient client = new HttpClient();

            var response = client.GetAsync(url).Result;
            var htmlContent = await response.Content.ReadAsStringAsync();

            Match m;

            string HRefPattern = "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";

            try
            {
                m = Regex.Match(htmlContent, HRefPattern,
                                RegexOptions.IgnoreCase | RegexOptions.Compiled,
                                TimeSpan.FromSeconds(1));
                while (m.Success)
                {
                    urlsChild.Add(m.Groups[1].Value);
                    m = m.NextMatch();
                }
            }
            catch (RegexMatchTimeoutException)
            {
                Console.WriteLine("The matching operation timed out.");
            }
            RemoveDomain(urlsChild, url);

            return urlsChild.Distinct().ToList();
        }

        private void RemoveDomain(IList<string> list, string url)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].Contains(url))
                {
                    list.RemoveAt(i);
                    i--;
                }
            }

        }
        #endregion
       
    }
}