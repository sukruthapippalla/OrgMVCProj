using Microsoft.AspNetCore.Mvc;
using OrgMVC.Models;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace OrgMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await GetDepartmentAsync();

            return View("DepartmentView", departments);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Create()
        {
            Department department = new Department();
            return View("DeptCreateView", department);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Department department)
        {
            if (ModelState.IsValid)
            {
                var isSuccess = await PostDepartmentAsync(department);
                if (isSuccess)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to insert department.");
                }

            }
            return View(department);
        }
        private async Task<bool> PostDepartmentAsync(Department department)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:7165/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var json = JsonSerializer.Serialize(department);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("api/Departments", content);

            return response.IsSuccessStatusCode;
        }

        private async Task<List<Department>> GetDepartmentAsync()
        {

            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:7165/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //https://localhost:7165/api/Departments
            HttpResponseMessage response = await client.GetAsync("api/Departments");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Department>>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                throw new Exception("Failed to retrieve departments from API");
            }

        }

        public async Task<IActionResult> Edit(int id)
        {
            var department = await GetDepartmentByIdAsync(id);
            if (department == null) return NotFound();
            return View("EditDeptView", department);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Department department)
        {
            if (ModelState.IsValid)
            {
                var isSuccess = await UpdateDepartmentAsync(department);
                if (isSuccess) return RedirectToAction("Index");
                ModelState.AddModelError(string.Empty, "Failed to update department.");
            }
            return View("EditDeptView", department);
        }

        private async Task<Department> GetDepartmentByIdAsync(int id)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:7165/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync($"api/Departments/{id}");
            if (response.IsSuccessStatusCode) 
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Department>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;

        }

        private async Task<bool> UpdateDepartmentAsync(Department department)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:7165/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Serialize the department object to JSON
            var json = JsonSerializer.Serialize(department);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send the PUT request to the Web API
            HttpResponseMessage response = await client.PutAsync($"api/Departments/{department.DeptId}", content);

            // Return true if the request was successful
            return response.IsSuccessStatusCode;
        }

        [HttpGet]
        [Route("Home/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var department = await GetDepartmentByIdAsync(id);
            if(department == null)
            {
                return NotFound();
            }
            return View("DetailsView",department);
        }

        [HttpGet]
        [Route("Home/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await GetDepartmentByIdAsync(id);
            if(department == null)
            {
                return NotFound();
            }
            return View("DeleteDeptView",department);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [Route("Home/DeleteConfirmed/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var isSuccess = await DeleteDepartmentAsync(id);
            if (isSuccess)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Failed to delete department");
            }
            return RedirectToAction("Index");
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7165/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.DeleteAsync($"api/Departments/{id}");

            return response.IsSuccessStatusCode;
           
        }

        


    }
}
