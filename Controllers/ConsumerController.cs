using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Text.Json;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
    public class ConsumerController(IHttpClientFactory clientFactory) : Controller
    {
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            HttpClient httpClient = clientFactory.CreateClient();
            HttpResponseMessage res = await httpClient.GetAsync("https://dummyjson.com/todos");

            if (res.IsSuccessStatusCode)
            {
                string content = await res.Content.ReadAsStringAsync();
                TodosModel? todos = JsonSerializer.Deserialize<TodosModel>(
                    content, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return View(todos.Todos ?? []);
            }
            return BadRequest();
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new TodoModel());
        }

        [HttpPost]
        public async Task<ActionResult> Create(TodoModel todo)
        {
            var httpClient = clientFactory.CreateClient();
            var httpContent = new StringContent(JsonSerializer.Serialize(
                todo, 
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true}
            ),
                Encoding.UTF8, "application/json");
            var res = await httpClient.PostAsync(
                "https://jsonplaceholder.typicode.com/posts", httpContent);
            if (res.IsSuccessStatusCode)
            {
                //var content2 = await res.Content.ReadAsStringAsync();
                return RedirectToAction("Index");
            }
            return BadRequest();
        }
        public IActionResult Put(int id)
        {
            return View(new TodoModel() { Id = id });
        }
        [HttpPost]
        public async Task<ActionResult> Put(int id, TodoModel todo)
        {
            var httpClient = clientFactory.CreateClient();
            var httpContent = new StringContent(JsonSerializer.Serialize(
               todo,
               new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
           ),
               Encoding.UTF8, "application/json");
            var res = await httpClient.PutAsync(
               $"https://jsonplaceholder.typicode.com/posts/{id}", httpContent);
            if (res.IsSuccessStatusCode)
            {
                //var content2 = await res.Content.ReadAsStringAsync();
                Console.WriteLine("gooooood");
                return RedirectToAction("Index");
            }
            Console.WriteLine("baaaaaaaaaad");
            return BadRequest();
        }
        

        public async Task<ActionResult> Delete(int id)
        {
            var httpClient = clientFactory.CreateClient();
            var res = await httpClient.DeleteAsync(
               $"https://jsonplaceholder.typicode.com/posts/{id}");
            if (res.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return BadRequest();
        }

    }
}
