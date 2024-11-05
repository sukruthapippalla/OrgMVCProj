using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrgMVC.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace OrgMVC.Controllers
{
    public class EmployeeController : Controller
    {
        
        public async Task<IActionResult> Index()
        {
            var employees = await GetEmployeeAsync();

            return View("EmployeeView", employees);
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
            Employee emp = new Employee();
            return View("EmpCreateView", emp);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employee emp)
        {
            if (ModelState.IsValid)
            {
                var isSuccess = await PostEmployeeAsync(emp);
                if (isSuccess)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to insert employee.");
                }

            }
            return View(emp);
        }
        private async Task<bool> PostEmployeeAsync(Employee emp)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:7165/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var json = JsonSerializer.Serialize(emp);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("api/Employees", content);

            return response.IsSuccessStatusCode;
        }

        private async Task<List<Employee>> GetEmployeeAsync()
        {

            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:7165/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            HttpResponseMessage response = await client.GetAsync("api/Employees");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Employee>>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                throw new Exception("Failed to retrieve employees from API");
            }

        }

        public async Task<IActionResult> Edit(int id)
        {
            var emp = await GetEmployeeByIdAsync(id);
            if (emp == null) return NotFound();
            return View("EditEmpView", emp);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee emp)
        {
            if (ModelState.IsValid)
            {
                var isSuccess = await UpdateEmployeeAsync(emp);
                if (isSuccess) return RedirectToAction("Index");
                ModelState.AddModelError(string.Empty, "Failed to update employee.");
            }
            return View("EditEmpView", emp);
        }

        private async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:7165/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync($"api/Employees/{id}");
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Employee>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;

        }

        private async Task<bool> UpdateEmployeeAsync(Employee emp)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:7165/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Serialize the department object to JSON
            var json = JsonSerializer.Serialize(emp);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send the PUT request to the Web API
            HttpResponseMessage response = await client.PutAsync($"api/Departments/{emp.EmpId}", content);

            // Return true if the request was successful
            return response.IsSuccessStatusCode;
        }

        [HttpGet]
        [Route("Employee/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var emp = await GetEmployeeByIdAsync(id);
            if (emp == null)
            {
                return NotFound();
            }
            return View("DetailsView", emp);
        }

        [HttpGet]
        [Route("Employee/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var emp= await GetEmployeeByIdAsync(id);
            if (emp == null)
            {
                return NotFound();
            }
            return View("DeleteEmpView", emp);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [Route("Employee/DeleteConfirmed/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var isSuccess = await DeleteEmployeeAsync(id);
            if (isSuccess)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Failed to delete employee");
            }
            return RedirectToAction("Index");
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7165/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.DeleteAsync($"api/Employees/{id}");

            return response.IsSuccessStatusCode;

        }
    }
}
