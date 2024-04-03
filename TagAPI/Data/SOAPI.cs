using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace TagAPI.Data
{
    public static class SOAPI
    {
        public class TagSOdto
        {
            public List<TagSO> items { get; set; }
        }
        public class TagSO
        {
            public string name { get; set; }
            public int count { get; set; }
        }
        public class TagSQL
        {
            [Key]
            public int Id { get; set; }
            [Required]
            public string Name { get; set; }
            [Required]
            public float Count { get; set; }
        }
        private static async Task<TagSOdto> DownloadTags(int page)
        {
            string apiUrl = "http://api.stackexchange.com/2.3/tags?pagesize=100&order=desc&sort=popular&site=stackoverflow&page=" + page;

            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            };

            TagSOdto? output = new TagSOdto();
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();

                    string responseBody2 = await response.Content.ReadAsStringAsync();
                    output = JsonSerializer.Deserialize<TagSOdto>(responseBody2);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request exception: {e.Message}");
                }
            }
            return output!;
        }
        private static List<TagSQL> CountTags(List<TagSO> tags)
        {
            int count = 0;

            foreach (TagSO tag in tags)
            {
                count += tag.count;
            }

            var sqlTags = new List<TagSQL>();

            foreach (TagSO tag in tags)
            {
                var sqlTag = new TagSQL();
                sqlTag.Name = tag.name;
                sqlTag.Count = tag.count * 100f / count;
                sqlTags.Add(sqlTag);
            }

            return sqlTags;
        }
        public static async Task<List<TagSQL>> CallApiAsync()
        {
            List<Task<TagSOdto>> tasks = new List<Task<TagSOdto>>();

            for (int i = 1; i < 12; i++)
            {
                tasks.Add(DownloadTags(i));
            }

            var results = await Task.WhenAll(tasks);

            TagSOdto? output = new TagSOdto();
            output.items = new List<TagSO>();

            foreach (var task in results)
            {
                if (task.items != null) output.items.AddRange(task.items);
            }

            var sqlTags = CountTags(output.items);

            return sqlTags;
        }
    }
}
